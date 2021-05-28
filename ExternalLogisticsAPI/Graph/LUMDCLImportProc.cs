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
                    And<LUMVendCntrlProcessOrder.orderDate,LessEqual<Current<DCLFilter.received_to>>>>>> ImportOrderList;

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
                    var result = DCLHelper.CallDCLToGetSOByFilter(this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(),this.DocFilter.Current);
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        // Delete temp table data
                        PXDatabase.Delete<LUMVendCntrlProcessOrder>(
                            new PXDataFieldRestrict<LUMVendCntrlProcessOrder.createdByID>(Accessinfo.UserID));
                        this.ImportOrderList.Cache.Clear();

                        int count = 1;
                        var _dclOrders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                        // insert data to temp table
                        foreach (var orders in _dclOrders.orders)
                        {

                            if(this.ImportLog.Select().RowCast<LUMVendCntrlProcessLog>().Any(x => x.OrderID == orders.order_number && !string.IsNullOrEmpty(x.AcumaticaOrderID) && x.ImportStatus == true))
                                continue;
                            var _soOrder = this.ImportOrderList.Insert(
                                (LUMVendCntrlProcessOrder)this.ImportOrderList.Cache.CreateInstance());
                            _soOrder.LineNumber = count++;
                            _soOrder.OrderID = orders.order_number;
                            _soOrder.CustomerID = orders.customer_number;
                            _soOrder.OrderDate = DateTime.Parse(orders.ordered_date);
                            _soOrder.OrderStatusID = orders.order_status.ToString();
                            _soOrder.OrderQty = orders.order_lines.Sum(x => x.quantity);
                            _soOrder.OrderAmount = orders.order_subtotal;
                            _soOrder.PoNumber = orders.po_number;
                            _soOrder.LastUpdated = DateTime.Parse(orders.modified_at);
                            _soOrder.Processed = false;
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
                var setup = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
                var result = DCLHelper.CallDCLToGetSOByFilter(this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(), this.DocFilter.Current);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var _apiOrders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                    var inventoryitems = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>().ToList();
                    var LogDatas = SelectFrom<LUMVendCntrlProcessLog>
                        .Where<LUMVendCntrlProcessLog.importStatus.IsEqual<True>
                            .And<LUMVendCntrlProcessLog.acumaticaOrderID.IsNotNull>>.View.Select(this)
                        .RowCast<LUMVendCntrlProcessLog>().ToList();

                    // Import Data 
                    foreach (var item in list.Where(x => !x.Processed.Value))
                    {
                        var _impRow = _apiOrders.orders.Where(x => x.order_number == item.OrderID).FirstOrDefault();
                        try
                        {
                            if (_impRow == null)
                                throw new Exception("Cant not mapping API Data");
                            // Check Data is Exists
                            var existsLog = LogDatas.Where(x =>
                                x.OrderID == item.OrderID && x.CustomerID == item.CustomerID).ToList();
                            if (existsLog.Any())
                                throw new Exception($"This Order has been Created, SONbr: {existsLog.FirstOrDefault()?.AcumaticaOrderType} {existsLog.FirstOrDefault()?.AcumaticaOrderID}");
                            // SOOrder
                            var graph = PXGraph.CreateInstance<SOOrderEntry>();
                            var newOrder = (SOOrder)graph.Document.Cache.CreateInstance();
                            newOrder.OrderType = setup.OrderType;
                            newOrder.OrderDate = DateTime.Parse(_impRow.ordered_date);
                            newOrder.CustomerID = setup.CustomerID;
                            newOrder.CustomerOrderNbr = _impRow.po_number;
                            newOrder.CustomerRefNbr = _impRow.order_number;
                            newOrder.OrderDesc =
                                $"Create SO BY Import Process |Tacking Number: {_impRow.shipments.FirstOrDefault().packages.FirstOrDefault()?.tracking_number}";
                            newOrder = (SOOrder)graph.Document.Cache.Insert(newOrder);

                            foreach (var line in _impRow.order_lines)
                            {
                                // SOLine
                                var newTranc = (SOLine)graph.Transactions.Cache.CreateInstance();
                                newTranc.InventoryID = inventoryitems
                                    .FirstOrDefault(x => x.InventoryCD.Trim() == line.item_number)?.InventoryID;
                                //newTranc.ManualPrice = true;
                                newTranc.OrderQty = (decimal)line.quantity;
                                newTranc.OpenQty = (decimal)line.quantity;
                                newTranc.UnitPrice = (decimal)line.price;
                                graph.Transactions.Cache.Insert(newTranc);
                            }
                            graph.Persist();
                            item.Processed = true;
                            // Update Process Order
                            this.ImportOrderList.Cache.Update(item);
                            InsertImpLog(setup, item, true, string.Empty, newOrder.OrderNbr);
                        }
                        catch (Exception e)
                        {
                            InsertImpLog(setup, item, false, e.Message);
                        }
                    }// end ImpRow
                    this.Actions.PressSave();
                }
            }
            catch (Exception e)
            {
                throw new PXOperationCompletedWithErrorException(e.Message, e);
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
