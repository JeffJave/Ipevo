using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Descripter;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Extensions;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.SM;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMDCLImportProc : PXGraph<LUMDCLImportProc>
    {
        public PXCancel<DCLFilter> Cancel;
        public PXFilter<DCLFilter> DocFilter;

        [PXFilterable]
        public PXFilteredProcessingJoin<LUMVendCntrlProcessOrder, DCLFilter,
            LeftJoin<LUMVendCntrlProcessLog, On<LUMVendCntrlProcessOrder.orderID, Equal<LUMVendCntrlProcessLog.orderID>,
                And<LUMVendCntrlProcessOrder.processID, Equal<LUMVendCntrlProcessLog.processID>>>>,
            Where<LUMVendCntrlProcessOrder.processed, Equal<False>,
                And<LUMVendCntrlProcessOrder.orderDate, GreaterEqual<Current<DCLFilter.received_from>>,
                    And<LUMVendCntrlProcessOrder.orderDate, LessEqual<Current<DCLFilter.received_to>>>>>> ImportOrderList;

        [PXHidden]
        [PXCacheName("ImportLog")]
        public SelectFrom<LUMVendCntrlProcessLog>.View ImportLog;

        [PXHidden]
        public SelectFrom<LUMVendCntrlSetup>.View DCLSetup;

        #region Constructor
        public LUMDCLImportProc()
        {
            DCLFilter currentFilter = this.DocFilter.Current;

            ImportOrderList.SetProcessCaption(PX.Objects.CR.Messages.Import);
            ImportOrderList.SetProcessAllCaption(PX.Objects.CR.Messages.Prepare + " & " + PX.Objects.CR.Messages.Import);
            ImportOrderList.SetProcessDelegate(
                delegate (List<LUMVendCntrlProcessOrder> list)
                {
                    ImportRecords(list, currentFilter);
                });
        }
        #endregion

        #region Action

        public PXAction<DCLFilter> prepareImport;
        [PXButton]
        [PXUIField(DisplayName = "Prepare", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrepareImport(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                try
                {
                    var result = DCLHelper.CallDCLToGetSOByFilter(this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(), this.DocFilter.Current);
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        // Delete temp table data
                        PXDatabase.Delete<LUMVendCntrlProcessOrder>(
                            new PXDataFieldRestrict<LUMVendCntrlProcessOrder.customerID>("AMZ"));
                        this.ImportOrderList.Cache.Clear();

                        int count = 1;
                        var _dclOrders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                        if (_dclOrders.orders == null)
                            return;
                        // insert data to temp table
                        foreach (var orders in _dclOrders.orders)
                        {

                            if (this.ImportLog.Select().RowCast<LUMVendCntrlProcessLog>().Any(x => x.OrderID == orders.order_number && !string.IsNullOrEmpty(x.AcumaticaOrderID) && x.ImportStatus == true))
                                continue;
                            var impRow = this.ImportOrderList.Insert(
                                (LUMVendCntrlProcessOrder)this.ImportOrderList.Cache.CreateInstance());
                            impRow.LineNumber = count++;
                            impRow.OrderID = orders.order_number;
                            impRow.CustomerID = orders.customer_number;
                            if (orders.shipments != null)
                                impRow.InvoiceNbr = string.IsNullOrEmpty(orders?.shipments.FirstOrDefault().ship_id) ? null : "8" + orders.shipments.FirstOrDefault()?.ship_id;
                            impRow.OrderDate = DateTime.Parse(orders.ordered_date);
                            impRow.OrderStatusID = orders.order_stage.ToString();
                            try
                            {
                                impRow.OrderQty = orders.shipments.SelectMany(x => x.packages).SelectMany(x => x.shipped_items).Sum(q => q.quantity);
                            }
                            catch (Exception)
                            {
                                impRow.OrderQty = 0;
                            }
                            impRow.OrderAmount = orders.order_subtotal;
                            impRow.PoNumber = orders.po_number;
                            impRow.LastUpdated = DateTime.Parse(orders.modified_at);
                            impRow.Processed = false;
                        }

                        this.Actions.PressSave();
                        return;
                    }
                    else
                        throw new Exception($"StatusCode:{result.StatusCode} Content: {result.ContentResult}");
                }
                catch (Exception e)
                {
                    throw new PXOperationCompletedWithErrorException(e.Message, e);
                }
            });
            return adapter.Get();
        }

        #endregion

        #region Method

        /// <summary> Create SO Data </summary>
        public virtual void ImportRecords(List<LUMVendCntrlProcessOrder> list, DCLFilter currentFilter)
        {
            try
            {
                PXLongOperation.StartOperation(this, delegate ()
                {
                    var setup = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
                    var result = DCLHelper.CallDCLToGetSOByFilter(
                        this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(), this.DocFilter.Current);
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        var apiOrders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                        var inventoryitems = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>()
                            .ToList();
                        var LogDatas = SelectFrom<LUMVendCntrlProcessLog>
                            .Where<LUMVendCntrlProcessLog.importStatus.IsEqual<True>
                                .And<LUMVendCntrlProcessLog.acumaticaOrderID.IsNotNull>>.View.Select(this)
                            .RowCast<LUMVendCntrlProcessLog>().ToList();

                        // Import Data 

                        int count = 0;
                        foreach (var item in list.Where(x => !x.Processed.Value))
                        {
                            // 
                            var logNewOrderNbr = string.Empty;
                            var logExceptionMsg = string.Empty;
                            var impRow = apiOrders.orders.Where(x => x.order_number == item.OrderID).FirstOrDefault();
                            try
                            {
                                if (impRow == null)
                                    throw new Exception("Cant not mapping API Data");
                                if (impRow.order_stage != 60)
                                    throw new Exception("Order stage is not equal Fully Shipped");
                                // Check Data is Exists
                                var existsLog = LogDatas.Where(x =>
                                    x.OrderID == item.OrderID && x.CustomerID == item.CustomerID).ToList();
                                if (existsLog.Any())
                                    throw new Exception(
                                        $"This Order has been Created, SONbr: {existsLog.FirstOrDefault()?.AcumaticaOrderType} {existsLog.FirstOrDefault()?.AcumaticaOrderID}");
                                // SOOrder
                                var graph = PXGraph.CreateInstance<SOOrderEntry>();
                                var newOrder = (SOOrder)graph.Document.Cache.CreateInstance();
                                newOrder.OrderType = setup.OrderType;
                                newOrder.OrderDate = DateTime.Parse(impRow.ordered_date);
                                newOrder.CustomerID = setup.CustomerID;
                                newOrder.CustomerOrderNbr = string.IsNullOrEmpty(impRow?.shipments.FirstOrDefault().ship_id) ? null : "8" + impRow.shipments.FirstOrDefault()?.ship_id;
                                newOrder.CustomerRefNbr = impRow.order_number;
                                newOrder.OrderDesc = $"Po Number : {impRow.po_number} | Carrier: {impRow.shipping_carrier} | Tacking Number: {impRow.shipments.FirstOrDefault().packages.FirstOrDefault()?.tracking_number}";

                                newOrder = (SOOrder)graph.Document.Cache.Insert(newOrder);

                                foreach (var line in impRow.shipments.SelectMany(x => x.packages).SelectMany(x => x.shipped_items).GroupBy(x => x.item_number).Select(x => new { qty = x.Sum(y => y.quantity),item_number = x.Key}))
                                {
                                    // SOLine
                                    var newTranc = (SOLine)graph.Transactions.Cache.CreateInstance();
                                    newTranc.InventoryID = inventoryitems
                                        .FirstOrDefault(x => x.InventoryCD.Trim() == line.item_number)?.InventoryID;
                                    newTranc.ManualPrice = true;
                                    newTranc.OrderQty = (decimal)line.qty;
                                    newTranc.OpenQty = (decimal)line.qty;
                                    newTranc.CuryUnitPrice = (decimal)impRow.order_lines.Where(x => x.item_number == line.item_number).FirstOrDefault()?.price;
                                    graph.Transactions.Insert(newTranc);
                                }
                                graph.Save.Press();
                                item.Processed = true;
                                // Update Process Order
                                this.ImportOrderList.Cache.Update(item);
                                logNewOrderNbr = newOrder.OrderNbr;

                                #region Prepare Invoice

                                var newAdapter = new PXAdapter(graph.Document)
                                { Searches = new Object[] { newOrder.OrderType, newOrder.OrderNbr } };
                                graph.PrepareInvoice(newAdapter);

                                #endregion
                            }
                            catch (Exception e)
                            {
                                // Prepare Invoice完成是throw exception
                                if (e.Message?.ToLower() != "invoice")
                                {
                                    logExceptionMsg = e.Message;
                                    PXProcessing.SetError<LUMVendCntrlProcessOrder>(count, e.Message);
                                }
                            }
                            finally
                            {
                                InsertImpLog(
                                    setup,
                                    item,
                                    string.IsNullOrEmpty(logExceptionMsg),
                                    logExceptionMsg,
                                    string.IsNullOrEmpty(logNewOrderNbr) ? null : logNewOrderNbr);
                            }
                            count++;
                        } // end ImpRow

                        this.Actions.PressSave();
                    }
                });
            }
            catch (Exception e)
            {
                throw new PXOperationCompletedWithErrorException(e.Message);
            }
        }

        /// <summary> Insert Log Data </summary>
        public void InsertImpLog(LUMVendCntrlSetup setUp, LUMVendCntrlProcessOrder prcData, bool success, string errMsg, string acumaticaOrderID = null)
        {
            // Insert Or Updated
            var IsUpdated = this.ImportLog
                .Select()
                .RowCast<LUMVendCntrlProcessLog>()
                .Any(x => x.OrderID == prcData.OrderID && x.ProcessID == prcData.ProcessID);
            var model =
                this.ImportLog
                    .Select()
                    .RowCast<LUMVendCntrlProcessLog>()
                    .FirstOrDefault(x => x.OrderID == prcData.OrderID && x.ProcessID == prcData.ProcessID) ?? this.ImportLog.Insert((LUMVendCntrlProcessLog)this.ImportLog.Cache.CreateInstance());
            model.ProcessID = prcData.ProcessID;
            model.OrderID = prcData.OrderID;
            model.CustomerID = prcData.CustomerID;
            model.InvoiceNbr = prcData.InvoiceNbr;
            if (success)
            {
                model.AcumaticaOrderID = acumaticaOrderID;
                model.ErrorDesc = null;
            }

            model.AcumaticaOrderType = setUp.OrderType;
            model.ImportStatus = success;
            if (!string.IsNullOrEmpty(errMsg))
                model.ErrorDesc = errMsg;

            if (IsUpdated)
                this.ImportLog.Cache.Update(model);
        }

        #endregion
    }

}
