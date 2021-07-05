using APILibrary.Model;
using ExternalLogisticsAPI.Descripter;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.PXImportAttribute;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMWarehouseImportProc : PXGraph<LUMWarehouseImportProc>, PXImportAttribute.IPXPrepareItems, IPXProcess
    {
        public PXSave<LUMWarehouseImportProcess> Save;
        public PXDelete<LUMWarehouseImportProcess> Delete;
        public PXCancel<LUMWarehouseImportProcess> Cancel;

        [PXImport(typeof(LUMWarehouseImportProcess))]
        public SelectFrom<LUMWarehouseImportProcess>
                .LeftJoin<LUMWarehouseImportProcessLog>.On<LUMWarehouseImportProcess.erporder.IsEqual<LUMWarehouseImportProcessLog.erporder>
                            .And<LUMWarehouseImportProcess.shipmentID.IsEqual<LUMWarehouseImportProcessLog.shipmentID>>>
            .Where<LUMWarehouseImportProcess.createdByID.IsEqual<AccessInfo.userID.FromCurrent>>.View ImportShipmentList;

        [PXHidden]
        public SelectFrom<LUMWarehouseImportProcessLog>.View ImportLog;

        [PXHidden]
        public SelectFrom<LUMMiddleWareSetup>.View MiddlewareSetup;

        #region Action

        public PXAction<LUMWarehouseImportProcess> lumProcess;
        [PXButton]
        [PXUIField(DisplayName = "Process", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LumProcess(PXAdapter adapter)
        {
            this.Save.PressButton(adapter);
            var warehouseData = adapter.Get<LUMWarehouseImportProcess>().Where(x => x.Selected ?? false).ToList();
            PXLongOperation.StartOperation(this, () =>
            {
                UpdateShipmentTrackingInfo(warehouseData);
                this.Actions.PressSave();
            });

            return adapter.Get();
        }

        public PXAction<LUMWarehouseImportProcess> lumProcessAll;
        [PXButton]
        [PXUIField(DisplayName = "ProcessAll", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LumProcessAll(PXAdapter adapter)
        {
            this.Save.PressButton(adapter);
            var warehouseData = adapter.Get<LUMWarehouseImportProcess>().ToList();
            PXLongOperation.StartOperation(this, () =>
            {
                UpdateShipmentTrackingInfo(warehouseData);
                this.Actions.PressSave();
            });

            return adapter.Get();
        }

        /// <summary> Delete all record by login User</summary>
        public PXAction<LUMWarehouseImportProcess> clearData;
        [PXButton]
        [PXUIField(DisplayName = "Clear", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable ClearData(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                PXDatabase.Delete<LUMWarehouseImportProcess>(
                              new PXDataFieldRestrict<LUMWarehouseImportProcess.createdByID>(Accessinfo.UserID));
                this.ImportShipmentList.Cache.Clear();
            });

            return adapter.Get();
        }

        #endregion

        #region Method

        /// <summary> Update Shipment Tracking Info </summary>
        public virtual bool UpdateShipmentTrackingInfo(List<LUMWarehouseImportProcess> warehouseData)
        {
            var isAllSuccess = true;
            var logData = SelectFrom<LUMWarehouseImportProcessLog>.View.Select(this).RowCast<LUMWarehouseImportProcessLog>();
            foreach (var row in warehouseData)
            {
                try
                {
                    var _soOrder = SelectFrom<SOOrder>.Where<SOOrder.orderNbr.IsEqual<P.AsString>>.View.Select(this, row.Erporder).RowCast<SOOrder>()?.FirstOrDefault();

                    #region Check is already updated

                    if (logData.Any(x => x.Erporder == row.Erporder && x.ShipmentID == row.ShipmentID && (x.IsProcess ?? false)))
                    {
                        //this.ImportShipmentList.Delete(row);
                        continue;
                    }

                    #endregion

                    #region Check SOOrder is Exists

                    if (_soOrder == null)
                        throw new Exception("SOOrder is not exists");

                    #endregion

                    #region Check data 

                    if (string.IsNullOrEmpty(row.Erporder) || row.Erporder == "null")
                        throw new Exception("ERP Order Nbr can not be empty!");
                    if (string.IsNullOrEmpty(row.Carrier) || row.Carrier == "null")
                        throw new Exception("Carrier can not be empty!");
                    if (!row.ShipmentDate.HasValue)
                        throw new Exception("Shipment Date can not be empty!");
                    if (string.IsNullOrEmpty(row.TrackingNbr) || row.TrackingNbr == "null")
                        throw new Exception("Tracking Nbr can not be empty!");

                    #endregion

                    if (_soOrder.OrderType == "FM")
                    {
                        var setup = this.MiddlewareSetup.Select().RowCast<LUMMiddleWareSetup>().FirstOrDefault();
                        var shippingCarrier = row.Carrier;
                        var _merchant = string.IsNullOrEmpty(PXAccess.GetCompanyName()?.Split(' ')[1]) ? "us" :
                                        PXAccess.GetCompanyName()?.Split(' ')[1].ToLower() == "uk" ? "gb" : PXAccess.GetCompanyName()?.Split(' ')[1].ToLower();
                        MiddleWare_Shipment metaData = new MiddleWare_Shipment()
                        {
                            merchant = _merchant,
                            amazon_order_id = _soOrder.CustomerOrderNbr,
                            shipment_date = row.ShipmentDate?.ToString("yyyy-MM-dd hh:mm:ss"),
                            shipping_method = "Standard",
                            carrier = shippingCarrier,
                            tracking_number = row.TrackingNbr
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
                        InsertLog(row, true, string.Empty);
                    }
                    else
                    {
                        var _soOrderShipment = SelectFrom<SOOrderShipment>
                                              .Where<SOOrderShipment.orderNbr.IsEqual<P.AsString>
                                                    .And<SOOrderShipment.orderType.IsEqual<P.AsString>>>
                                              .View.Select(this, _soOrder.OrderNbr, _soOrder.OrderType).RowCast<SOOrderShipment>()?.FirstOrDefault();
                        #region Check SOOrderShipment is Exists

                        if (_soOrderShipment == null)
                            throw new Exception("SOOrder need to create Shipment first!");

                        #endregion

                        var _soShipment = SelectFrom<SOShipment>.Where<SOShipment.shipmentNbr.IsEqual<P.AsString>>
                                         .View.Select(this, _soOrderShipment?.ShipmentNbr).RowCast<SOShipment>().FirstOrDefault();
                        #region Check SOShipment is Exists

                        if (_soShipment == null)
                            throw new Exception("Shipment is not exists!");

                        #endregion

                        // Update Shipment
                        var shipmentGraph = PXGraph.CreateInstance<SOShipmentEntry>();
                        _soShipment.GetExtension<SOShipmentExt>().UsrTrackingNbr = row.TrackingNbr;
                        _soShipment.GetExtension<SOShipmentExt>().UsrCarrier = row.Carrier;
                        shipmentGraph.Document.Update(_soShipment);
                        shipmentGraph.Save.Press();

                        InsertLog(row, true, string.Empty);
                    }

                }
                catch (Exception ex)
                {
                    isAllSuccess = false;
                    InsertLog(row, false, ex.Message);
                }
            }
            return isAllSuccess;
        }

        /// <summary> Insert log </summary>
        public virtual void InsertLog(LUMWarehouseImportProcess row, bool status, string msg)
        {
            var IsExists = this.ImportLog
                    .Select()
                    .RowCast<LUMWarehouseImportProcessLog>()
                    .Any(x => x.Erporder == row.Erporder && x.ShipmentID == row.ShipmentID);
            var model = this.ImportLog
                    .Select()
                    .RowCast<LUMWarehouseImportProcessLog>()
                    .FirstOrDefault(x => x.Erporder == row.Erporder && x.ShipmentID == row.ShipmentID) ?? this.ImportLog.Insert((LUMWarehouseImportProcessLog)this.ImportLog.Cache.CreateInstance());
            model.Erporder = row.Erporder;
            model.ShipmentID = row.ShipmentID;
            model.TrackingNbr = row.TrackingNbr;
            model.Carrier = row.Carrier;
            model.ShipmentDate = row.ShipmentDate;
            model.IsProcess = status;
            model.ProcessMessage = msg;
            //if (status)
            //    this.ImportShipmentList.Delete(row);
            if (IsExists)
                this.ImportLog.Update(model);
        }

        #endregion

        #region Implemt Import function
        public void ImportDone(ImportMode.Value mode)
        {
            //this.Save.Press();
        }

        public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
        {
            return true;
        }

        public void PrepareItems(string viewName, IEnumerable items)
        {

        }

        public bool RowImported(string viewName, object row, object oldRow)
        {
            return true;
        }

        public bool RowImporting(string viewName, object row)
        {

            return true;
        }
        #endregion
    }
}
