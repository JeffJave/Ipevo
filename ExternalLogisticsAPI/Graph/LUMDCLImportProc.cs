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
        [PXCacheName("Document")]
        public SelectFrom<LUMVendCntrlProcessOrder>
        .LeftJoin<LUMVendCntrlProcessLog>.On<LUMVendCntrlProcessOrder.orderID.IsEqual<LUMVendCntrlProcessLog.orderID>
            .And<LUMVendCntrlProcessOrder.processID.IsEqual<LUMVendCntrlProcessLog.processID>>>
        .View Document;

        [PXHidden]
        [PXCacheName("ImportLog")]
        public SelectFrom<LUMVendCntrlProcessLog>.View ImportLog;

        [PXHidden]
        public SelectFrom<LUMVendCntrlSetup>.View DCLSetup;

        public PXFilter<DCLFilter> DocFilter;

        public PXSave<LUMVendCntrlProcessOrder> Save;
        public PXCancel<LUMVendCntrlProcessOrder> Cancel;

        public PXAction<LUMVendCntrlProcessOrder> prepareImport;
        public PXAction<LUMVendCntrlProcessOrder> importSalesOrder;

        [PXButton]
        [PXUIField(DisplayName = "Prepare Import", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrepareImport(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                try
                {
                    var result = CallApi();
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        // Delete temp table data
                        PXDatabase.Delete<LUMVendCntrlProcessOrder>(
                            new PXDataFieldRestrict<LUMVendCntrlProcessOrder.createdByID>(Accessinfo.UserID));
                        this.Document.Cache.Clear();
                        // Delete log table data
                        //PXDatabase.Delete<LUMVendCntrlProcessLog>(
                        //    new PXDataFieldRestrict<LUMVendCntrlProcessLog.createdByID>(Accessinfo.UserID));

                        int count = 1;
                        var _orders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                        // insert data to temp table
                        foreach (var orders in _orders.orders)
                        {
                            var temp = this.Document.Insert(
                                (LUMVendCntrlProcessOrder)this.Document.Cache.CreateInstance());
                            temp.LineNumber = count++;
                            temp.OrderID = orders.order_number;
                            temp.CustomerID = orders.customer_number;
                            temp.OrderDate = DateTime.Parse(orders.ordered_date);
                            temp.OrderStatusID = orders.order_status.ToString();
                            temp.OrderAmount = orders.order_lines.FirstOrDefault()?.quantity;
                            temp.SalesTaxAmt = orders.order_lines.FirstOrDefault()?.price;
                            temp.LastUpdated = DateTime.Parse(orders.modified_at);
                            temp.Processed = false;
                        }

                        this.Save.Press();
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

        [PXButton]
        [PXUIField(DisplayName = "Import SalesOrder", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable ImportSalesOrder(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                try
                {
                    var setup = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
                    var result = CallApi();
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        var tempData = this.Document.Select().RowCast<LUMVendCntrlProcessOrder>().ToList();
                        var _apiOrders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                        var intersectedList =
                            from t in _apiOrders.orders.ToList()
                            join v in tempData on t.order_number equals v.OrderID
                            select new { apiOrder = t, LineNbr = v.LineNumber };
                        int count = 0;

                        var inventoryitems = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>().ToList();
                        var LogData = SelectFrom<LUMVendCntrlProcessLog>
                            .Where<LUMVendCntrlProcessLog.importStatus.IsEqual<True>
                                .And<LUMVendCntrlProcessLog.acumaticaOrderID.IsNotNull>>.View.Select(this)
                            .RowCast<LUMVendCntrlProcessLog>().ToList();

                        // Import Data 
                        foreach (var item in tempData.Where(x => !x.Processed.Value))
                        {
                            var _impRow = _apiOrders.orders.Where(x => x.order_number == item.OrderID).FirstOrDefault();
                            try
                            {
                                if (_impRow == null)
                                    throw new Exception("Cant not mapping API Data");
                                // Check Data is Exists
                                var existsData = LogData.Where(x =>
                                    x.OrderID == item.OrderID && x.CustomerID == item.CustomerID).ToList();
                                if (existsData.Any())
                                    throw new Exception($"This Order has been Created, SONbr: {existsData.FirstOrDefault()?.AcumaticaOrderType} {existsData.FirstOrDefault()?.AcumaticaOrderID}");

                                var graph = PXGraph.CreateInstance<SOOrderEntry>();
                                var newOrder = (SOOrder)graph.Document.Cache.CreateInstance();
                                newOrder.OrderType = setup.OrderType;
                                newOrder.OrderDate = DateTime.Parse(_impRow.ordered_date);
                                newOrder.CustomerID = setup.CustomerID;
                                newOrder.CustomerRefNbr = _impRow.order_number;
                                newOrder.OrderDesc =
                                    $"Create SO BY Import Process |Tacking Number: {_impRow.shipments.FirstOrDefault().packages.First()?.tracking_number}";
                                newOrder = (SOOrder)graph.Document.Cache.Insert(newOrder);

                                foreach (var line in _impRow.order_lines)
                                {
                                    var newTrancs = (SOLine)graph.Transactions.Cache.CreateInstance();
                                    graph.Transactions.Cache.SetValueExt<SOLine.inventoryID>(newTrancs, inventoryitems.Where(x => x.InventoryCD.Trim() == line.item_number)
                                        .FirstOrDefault()?.InventoryID);
                                    graph.Transactions.Cache.SetValueExt<SOLine.orderQty>(newTrancs, decimal.Parse(line.quantity.ToString()));
                                    graph.Transactions.Cache.SetValueExt<SOLine.openQty>(newTrancs, decimal.Parse(line.quantity.ToString()));
                                    graph.Transactions.Cache.SetValueExt<SOLine.unitPrice>(newTrancs, line.price);
                                    graph.Transactions.Cache.Insert(newTrancs);
                                }
                                graph.Persist();
                                item.Processed = true;
                                this.Document.Cache.Update(item);
                                InsertImpLog(setup, item, true, string.Empty, newOrder.OrderNbr);
                            }
                            catch (Exception e)
                            {
                                InsertImpLog(setup, item, false, e.Message);
                            }
                        }// end ImpRow
                        this.Save.Press();
                    }
                }
                catch (Exception e)
                {
                    throw new PXOperationCompletedWithErrorException(e.Message, e);
                }
            }); // End Operation
            return adapter.Get();
        }

        public LumAPIResultModel CallApi()
        {
            var setup = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
            var config = new DCL_Config()
            {
                RequestMethod = HttpMethod.Get,
                RequestUrl = APIFuncitoin.CombineQueryString(
                    setup.SecureURL,
                    new
                    {
                        Received_from = this.DocFilter.Current.Received_from?.ToString("yyyy-MM-dd"),
                        Received_to = this.DocFilter.Current.Received_to?.ToString("yyyy-MM-dd"),
                        Filter = $"Customer_number eq {this.DocFilter.Current.Customer_number}"
                    }),
                AuthType = setup.AuthType,
                Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{setup.ClientID}:{setup.ClientSecret}"))
            };
            var caller = new APICaller(config);
            return caller.CallApi();
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
    }
}
