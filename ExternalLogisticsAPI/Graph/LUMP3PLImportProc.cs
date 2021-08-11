using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.SO;
using PX.Data.BQL;
using ExternalLogisticsAPI.Descripter;
using Newtonsoft.Json;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMP3PLImportProc : PXGraph<LUMP3PLImportProc>
    {
        public PXCancel<LUMP3PLImportProcess> Cancel;
        public PXProcessingJoin<LUMP3PLImportProcess, LeftJoin<LUMP3PLImportProcessLog, On<LUMP3PLImportProcess.warehouseOrder, Equal<LUMP3PLImportProcessLog.warehouseOrder>>>,
                                Where<LUMP3PLImportProcess.createdByID, Equal<Current<AccessInfo.userID>>>> ImportDataList;

        [PXHidden]
        [PXCacheName("ImportLog")]
        public SelectFrom<LUMP3PLImportProcessLog>.View ImportLog;

        [PXHidden]
        public SelectFrom<LUM3PLUKSetup>.View P3PLSetup;

        [PXHidden]
        public SelectFrom<LUMMiddleWareSetup>.View MiddlewareSetup;

        public LUMP3PLImportProc()
        {
            //LUMP3PLImportProc graph = null;
            ImportDataList.SetProcessDelegate(delegate (List<LUMP3PLImportProcess> list)
                                              {
                                                  GoUpdate(list);
                                              });
        }

        public PXAction<LUMP3PLImportProcess> prepareImport;
        [PXButton]
        [PXUIField(DisplayName = "Prepare", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrepareImport(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                try
                {
                    // Clear Data
                    PXDatabase.Delete<LUMP3PLImportProcess>();
                    this.ImportDataList.Cache.Clear();

                    // Load FTP File
                    FTP_Config config = GetConfig();
                    FTPHelper helper = new FTPHelper(config);
                    var ftpFileContents = helper.LoadFileStringFromFTP();
                    foreach (var item in ftpFileContents)
                    {
                        var line = item.Value.Split(';');
                        var model = this.ImportDataList.Insert((LUMP3PLImportProcess)this.ImportDataList.Cache.CreateInstance());
                        model.WarehouseOrder = line[0];
                        model.CustomerOrderRef = line[1];
                        model.OrderStatus = line[2];
                        model.UnitsSent = int.Parse(string.IsNullOrEmpty(line[3]) ? "0" : line[3]);
                        model.Carrier = line[4];
                        model.TrackingNumber = line[5];
                        model.FreightCost = decimal.Parse(string.IsNullOrEmpty(line[6]) ? "0" : line[6]);
                        model.FreightCurrency = line[7];
                        model.FtpFileName = item.Key;
                    }
                    this.Actions.PressSave();
                }
                catch (Exception e)
                {
                    throw new PXOperationCompletedWithErrorException(e.Message, e);
                }
            });
            return adapter.Get();
        }

        public static void GoUpdate(List<LUMP3PLImportProcess> list)
        {
            LUMP3PLImportProc graph = PXGraph.CreateInstance<LUMP3PLImportProc>();
            graph.UpdateTrackingInfo(graph, list);
            //graph.ImportDataList.View.RequestRefresh();
        }

        /// <summary> Update Shipment Tracking Info For P3PL </summary>
        public virtual void UpdateTrackingInfo(LUMP3PLImportProc graph, List<LUMP3PLImportProcess> list)
        {
            PXLongOperation.StartOperation(this, delegate ()
            { 
                var logData = SelectFrom<LUMP3PLImportProcessLog>.View.Select(graph).RowCast<LUMP3PLImportProcessLog>();
                foreach (var row in list)
                {
                    try
                    {
                        var _soOrder = SelectFrom<SOOrder>.Where<SOOrder.orderNbr.IsEqual<P.AsString>>.View.Select(graph, row.WarehouseOrder).RowCast<SOOrder>()?.FirstOrDefault();

                        #region Check is already updated

                        if (logData.Any(x => x.WarehouseOrder == row.WarehouseOrder && (x.IsProcess ?? false)))
                            continue;

                        #endregion

                        #region Check SOOrder is Exists

                        if (_soOrder == null)
                            throw new Exception("SOOrder is not exists");

                        #endregion

                        #region Check data 

                        if (string.IsNullOrEmpty(row.WarehouseOrder))
                            throw new Exception("ERP Order Nbr can not be empty!");
                        if (string.IsNullOrEmpty(row.Carrier) || row.Carrier == "null")
                            throw new Exception("Carrier can not be empty!");
                        if (string.IsNullOrEmpty(row.TrackingNumber))
                            throw new Exception("Tracking Nbr can not be empty!");

                        #endregion

                        if (_soOrder.OrderType == "FM")
                        {
                            var setup = graph.MiddlewareSetup.Select().RowCast<LUMMiddleWareSetup>().FirstOrDefault();
                            var shippingCarrier = row.Carrier;
                            var _merchant = string.IsNullOrEmpty(PXAccess.GetCompanyName()?.Split(' ')[1]) ? "us" :
                                            PXAccess.GetCompanyName()?.Split(' ')[1].ToLower() == "uk" ? "gb" : PXAccess.GetCompanyName()?.Split(' ')[1].ToLower();
                            MiddleWare_Shipment metaData = new MiddleWare_Shipment()
                            {
                                merchant = _merchant,
                                amazon_order_id = _soOrder.CustomerOrderNbr,
                                shipment_date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                                shipping_method = "Standard",
                                carrier = shippingCarrier,
                                tracking_number = row.TrackingNumber
                            };
                            // Update FBM
                            var updateResult = MiddleWareHelper.CallMiddleWareToUpdateFBM(setup, metaData);
                            // Check HttpStatusCode
                            if (updateResult.StatusCode != System.Net.HttpStatusCode.OK)
                                throw new PXException($"Update MiddleWare FBM fail , Code = {updateResult.StatusCode}");
                            // Check Response status
                            var updateModel = JsonConvert.DeserializeObject<MiddleWare_Response>(updateResult.ContentResult);
                            if (!updateModel.Status)
                                throw new PXException($"Update Middleware FBM fail, Msg = {updateModel.Message}");
                            // update SOOrder_UsrSendToMiddleware
                            var soorderGraph = PXGraph.CreateInstance<SOOrderEntry>();
                            _soOrder.GetExtension<SOOrderExt>().UsrSendToMiddleware = true;
                            soorderGraph.Document.Update(_soOrder);
                            soorderGraph.Save.Press();
                            InsertLogAndDeleteFile(graph, row, true, string.Empty);
                        }
                        else
                        {
                            var _soOrderShipment = SelectFrom<SOOrderShipment>
                                                  .Where<SOOrderShipment.orderNbr.IsEqual<P.AsString>
                                                        .And<SOOrderShipment.orderType.IsEqual<P.AsString>>>
                                                  .View.Select(graph, _soOrder.OrderNbr, _soOrder.OrderType).RowCast<SOOrderShipment>()?.FirstOrDefault();
                            #region Check SOOrderShipment is Exists

                            if (_soOrderShipment == null)
                                throw new Exception("SOOrder need to create Shipment first!");

                            #endregion

                            var _soShipment = SelectFrom<SOShipment>.Where<SOShipment.shipmentNbr.IsEqual<P.AsString>>
                                             .View.Select(graph, _soOrderShipment?.ShipmentNbr).RowCast<SOShipment>().FirstOrDefault();
                            #region Check SOShipment is Exists

                            if (_soShipment == null)
                                throw new Exception("Shipment is not exists!");

                            #endregion

                            // Update Shipment
                            var shipmentGraph = PXGraph.CreateInstance<SOShipmentEntry>();
                            _soShipment.GetExtension<SOShipmentExt>().UsrTrackingNbr = row.TrackingNumber;
                            _soShipment.GetExtension<SOShipmentExt>().UsrCarrier = row.Carrier;
                            shipmentGraph.Document.Update(_soShipment);
                            shipmentGraph.Save.Press();
                            // Confirm Shipment
                            shipmentGraph.confirmShipmentAction.Press();
                            // Prepare Invoice
                            shipmentGraph.createInvoice.Press();
                            // Insert Log
                            InsertLogAndDeleteFile(graph, row, true, string.Empty);
                        }

                        graph.Actions.PressSave();
                    }
                    catch (Exception ex)
                    {
                        PXProcessing.SetError(ex.Message);
                        InsertLogAndDeleteFile(graph, row, false, ex.Message);
                    }
                }
            });
        }

        /// <summary> Insert log & Delete FTP File </summary>
        public virtual void InsertLogAndDeleteFile(LUMP3PLImportProc graph, LUMP3PLImportProcess row, bool status, string msg)
        {
            var IsExists = graph.ImportLog
                    .Select()
                    .RowCast<LUMP3PLImportProcessLog>()
                    .Any(x => x.WarehouseOrder == row.WarehouseOrder);
            var model = graph.ImportLog
                    .Select()
                    .RowCast<LUMP3PLImportProcessLog>()
                    .FirstOrDefault(x => x.WarehouseOrder == row.WarehouseOrder) ?? graph.ImportLog.Insert((LUMP3PLImportProcessLog)graph.ImportLog.Cache.CreateInstance());
            #region Log Entity
            model.WarehouseOrder = row.WarehouseOrder;
            model.CustomerOrderRef = row.CustomerOrderRef;
            model.OrderStatus = row.OrderStatus;
            model.UnitsSent = row.UnitsSent;
            model.Carrier = row.Carrier;
            model.TrackingNumber = row.TrackingNumber;
            model.FreightCost = row.FreightCost;
            model.FreightCurrency = row.FreightCurrency;
            model.FtpFileName = row.FtpFileName;
            model.IsProcess = status;
            model.ProcessMessage = msg;
            #endregion

            if (IsExists)
                graph.ImportLog.Update(model);

            // Delete File
            if (model.IsProcess.Value)
            {
                //FTP_Config config = GetConfig();
                //FTPHelper helper = new FTPHelper(config);
                //var ftpFileContents = helper.DeleteFTPFile(model.FtpFileName);
            }
        }

        /// <summary> Get Config </summary>
        public FTP_Config GetConfig()
        {
            var configP3PL = P3PLSetup.Select().RowCast<LUM3PLUKSetup>().FirstOrDefault();
            FTP_Config config = new FTP_Config()
            {
                FtpHost = configP3PL.FtpHost,
                FtpUser = configP3PL.FtpUser,
                FtpPass = configP3PL.FtpPass,
                FtpPort = configP3PL.FtpPort,
                FtpPath = configP3PL.FtpOutPath
            };
            return config;
        }

    }
}
