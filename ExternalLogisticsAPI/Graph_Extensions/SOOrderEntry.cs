using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using APILibrary.Model.Interface;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Descripter;
using Newtonsoft.Json;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Licensing;
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.SM;

namespace ExternalLogisticsAPI.Graph_Extensions
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {
        [PXHidden]
        public SelectFrom<LUMVendCntrlSetup>.View DCLSetup;

        [PXHidden]
        public SelectFrom<LUMMiddleWareSetup>.View MiddlewareSetup;

        #region  Actions
        public PXAction<SOOrder> createDCLShipment;
        public PXAction<SOOrder> lumCallDCLShipemnt;
        public PXAction<SOOrder> lumGererateYUSENNLFile;
        public PXAction<SOOrder> lumGenerateYUSENCAFile;
        public PXAction<SOOrder> lumGenerate3PLUKFile;

        public override void Initialize()
        {
            base.Initialize();
            Base.action.AddMenuAction(createDCLShipment);
            Base.action.AddMenuAction(lumCallDCLShipemnt);
        }

        /// <summary> 在DCL 產生Shipment(Call API) </summary>
        [PXButton]
        [PXUIField(DisplayName = "Create Shipment in DCL", Enabled = true, MapEnableRights = PXCacheRights.Select, Visible = true)]
        protected virtual IEnumerable CreateDCLShipment(PXAdapter adapter)
        {
            List<SOOrder> list = adapter.Get<SOOrder>().ToList();

            var adapterSlice = (adapter.MassProcess, adapter.QuickProcessFlow, adapter.AllowRedirect);
            for (int i = 0; i < list.Count; i++)
            {
                var order = list[i];
                if (adapterSlice.MassProcess)
                    PXProcessing<SOOrder>.SetCurrentItem(order);

                DCLShipmentRequestEntity model = new DCLShipmentRequestEntity();
                CombineDLCShipmentEntity(model, order);
                PXNoteAttribute.SetNote(Base.Document.Cache, order, APIHelper.GetJsonString(model));
                var dclSetUp = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
                if (!dclSetUp.IsGoLive ?? false)
                    order.GetExtension<SOOrderExt>().UsrDCLShipmentCreated = true;
                else
                {
                    #region  Send Data to DCL for Create Shipment(Implement)

                    var dclResult = DCLHelper.CallDCLToCreateShipment(dclSetUp, model);
                    var response = APIHelper.GetObjectFromString<DCLShipmentResponseEntity>(dclResult.ContentResult);
                    if (dclResult.StatusCode == System.Net.HttpStatusCode.Created)
                        order.GetExtension<SOOrderExt>().UsrDCLShipmentCreated = true;
                    else
                        throw new PXException(response.order_statuses.FirstOrDefault()?.error_message);

                    #endregion
                }
                Base.Document.Cache.Update(order);
            }

            Base.Persist();
            return adapter.Get();
        }

        /// <summary> 根據DCL的Shipment資料來產生系統Shipment(逐筆執行) </summary>
        [PXButton]
        [PXUIField(DisplayName = "Call DCL for Shipment", Enabled = true, MapEnableRights = PXCacheRights.Select, Visible = true)]
        protected virtual IEnumerable LumCallDCLShipemnt(PXAdapter adapter, [PXDate] DateTime? shipDate, [PXInt] int? siteID, [SOOperation.List] string operation)
        {
            try
            {
                var _soOrder = adapter.Get<SOOrder>().ToList()[0];
                var dclSetup = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
                var dclOrders = new OrderResponse();
                // Get DCL SO. Data(正式:order_number = SO.OrderNbr)
                if (dclSetup.IsGoLive ?? false)
                {
                    dclOrders = JsonConvert.DeserializeObject<OrderResponse>(
                            DCLHelper.CallDCLToGetSOByOrderNumbers(
                            this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(), _soOrder.OrderNbr).ContentResult);
                }
                else
                {
                    // Get DCL SO. Data
                    dclOrders = JsonConvert.DeserializeObject<OrderResponse>(
                            DCLHelper.CallDCLToGetSOByOrderNumbers(
                            this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(), _soOrder.CustomerRefNbr).ContentResult);
                }

                if (dclOrders.orders == null)
                    throw new Exception("Can not Mapping DCL Data");

                if (!dclOrders.orders.Any(x => x.order_stage == 60))
                    throw new Exception("DCL Order stage is not Fully Shipped");

                if (_soOrder.OrderType == "FM")
                {
                    var setup = this.MiddlewareSetup.Select().RowCast<LUMMiddleWareSetup>().FirstOrDefault();
                    var shippingCarrier = dclOrders.orders.FirstOrDefault()?.shipping_carrier;
                    var packagesInfo = dclOrders.orders.FirstOrDefault().shipments.SelectMany(x => x.packages);
                    var _merchant = string.IsNullOrEmpty(PXAccess.GetCompanyName()?.Split(' ')[1]) ? "us" :
                                    PXAccess.GetCompanyName()?.Split(' ')[1].ToLower() == "uk" ? "gb" : PXAccess.GetCompanyName()?.Split(' ')[1].ToLower();
                    MiddleWare_Shipment metaData = new MiddleWare_Shipment()
                    {
                        merchant = _merchant,
                        amazon_order_id = dclOrders.orders.FirstOrDefault().po_number,
                        shipment_date = dclOrders.orders.FirstOrDefault()?.shipments?.FirstOrDefault()?.ship_date + " 00:00:00",
                        shipping_method = "Standard",
                        carrier = shippingCarrier,
                        tracking_number = string.Join("|", packagesInfo.Select(x => x.tracking_number))
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
                    _soOrder.GetExtension<SOOrderExt>().UsrSendToMiddleware = true;
                    Base.Document.Update(_soOrder);
                    Base.Save.Press();
                }
                else
                {
                    using (PXTransactionScope sc = new PXTransactionScope())
                    {
                        adapter.MassProcess = true;
                        // Get Carrier and TrackingNbr
                        var shippingCarrier = dclOrders.orders.FirstOrDefault().shipping_carrier;
                        var packagesInfo = dclOrders.orders.FirstOrDefault().shipments.SelectMany(x => x.packages);

                        // update SOorder Descr
                        _soOrder.OrderDesc += $"|Carrier: {shippingCarrier}|" +
                                              $" TrackingNbr: {packagesInfo.Select(x => x.tracking_number).FirstOrDefault()}";

                        var tempDesc = _soOrder.OrderDesc;

                        var aa = Base.CreateShipmentIssue(adapter, shipDate, siteID);
                        var processResult = PXProcessing<SOOrder>.GetItemMessage();
                        if (processResult.ErrorLevel != PXErrorLevel.RowInfo)
                            return adapter.Get();

                        // Create SOShipment Graph
                        var graph = PXGraph.CreateInstance<SOShipmentEntry>();

                        // Find SOShipment
                        var _soOrderShipments =
                            FbqlSelect<SelectFromBase<SOOrderShipment, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrderShipment.orderType, Equal<P.AsString>>>>>.And<BqlOperand<SOOrderShipment.orderNbr, IBqlString>.IsEqual<P.AsString>>.And<BqlOperand<SOOrderShipment.confirmed,IBqlBool>.IsEqual<False>>>, SOOrderShipment>.View.Select(Base, _soOrder.OrderType, _soOrder.OrderNbr)
                                .RowCast<SOOrderShipment>();
                        foreach (var refItem in _soOrderShipments)
                        {
                            // Create new Adapter
                            var newAdapter = new PXAdapter(graph.Document) { Searches = new Object[] { refItem.ShipmentNbr } };
                            // Select Current Shipment
                            var _soShipment = newAdapter.Get<SOShipment>().ToList()[0];

                            try
                            {
                                // Get Carrier and TrackingNbr
                                var dclShipdate = dclOrders.orders.FirstOrDefault()?.shipments.FirstOrDefault()?.ship_date ?? DateTime.Now.ToString("yyyy/MM/dd");
                                _soShipment.ShipDate = DateTime.Parse(dclShipdate);
                                _soShipment.GetExtension<SOShipmentExt>().UsrCarrier = shippingCarrier;
                                _soShipment.GetExtension<SOShipmentExt>().UsrTrackingNbr = packagesInfo.Select(x => x.tracking_number).FirstOrDefault();
                                _soShipment.ShipmentDesc = tempDesc;
                                if (_soShipment.ShipmentDesc.Length > 256)
                                    _soShipment.ShipmentDesc = _soShipment.ShipmentDesc.Substring(0, 255);
                            }
                            catch (Exception e)
                            {
                                _soShipment.ShipmentDesc = e.Message;
                            }

                            // Update Data
                            graph.Document.Update(_soShipment);

                            // Remove Hold
                            graph.releaseFromHold.PressButton(newAdapter);
                            // Confirm Shipment
                            graph.confirmShipmentAction.PressButton(newAdapter);
                            // Prepare Invoice For 3D Orders
                            try
                            {
                                if (_soOrder.OrderType == "3D" || _soOrder.OrderType == "SO")
                                {
                                    newAdapter.AllowRedirect = true;
                                    graph.createInvoice.PressButton(newAdapter);
                                }
                            }
                            catch (PXRedirectRequiredException ex)
                            {
                                SOInvoiceEntry invoiceEntry = ex.Graph as SOInvoiceEntry;
                                var soTax = SelectFrom<SOTaxTran>
                                         .Where<SOTaxTran.orderNbr.IsEqual<P.AsString>
                                              .And<SOTaxTran.orderType.IsEqual<P.AsString>>>
                                         .View.Select(Base, _soOrder.OrderNbr, _soOrder.OrderType)
                                         .RowCast<SOTaxTran>().FirstOrDefault();
                                var balance = invoiceEntry.Document.Current.CuryDocBal;
                                var refNbr = invoiceEntry.Document.Current.RefNbr;
                                var invTax = SelectFrom<ARTaxTran>.Where<ARTaxTran.refNbr.IsEqual<P.AsString>>
                                             .View.Select(Base, refNbr).RowCast<ARTaxTran>().FirstOrDefault();
                                var adjd = SelectFrom<ARAdjust2>.Where<ARAdjust2.adjdRefNbr.IsEqual<P.AsString>>
                                             .View.Select(Base, refNbr).RowCast<ARAdjust2>().FirstOrDefault();
                                if (soTax != null)
                                {
                                    // setting Tax
                                    //invoiceEntry.Taxes.SetValueExt<ARTaxTran.curyTaxAmt>(invTax, soTax.CuryTaxAmt);
                                    //invoiceEntry.Taxes.Update(invTax);
                                    //// setting Document
                                    //invoiceEntry.Document.SetValueExt<ARInvoice.curyTaxTotal>(invoiceEntry.Document.Current, soTax.CuryTaxAmt);
                                    //invoiceEntry.Document.SetValueExt<ARInvoice.curyDocBal>(invoiceEntry.Document.Current, balance + (soTax.CuryTaxAmt ?? 0));
                                    //invoiceEntry.Document.Update(invoiceEntry.Document.Current);
                                    //if (adjd != null)
                                    //{
                                    //    invoiceEntry.Adjustments.SetValueExt<ARAdjust2.curyAdjdAmt>(adjd, adjd.CuryAdjdAmt + (soTax.CuryTaxAmt ?? 0));
                                    //    invoiceEntry.Adjustments.Update(adjd);
                                    //}
                                    // only 3DCart order need to release invoice
                                    if (_soOrder.OrderType == "3D")
                                    {
                                        invoiceEntry.releaseFromCreditHold.Press();
                                        invoiceEntry.release.Press();
                                    }
                                    invoiceEntry.Save.Press();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }

                        sc.Complete();
                    }
                }
            }
            catch (Exception e)
            {
                PXProcessing.SetError<SOOrder>(e.Message);
            }

            return adapter.Get();
        }

        /// <summary> 產生NL Shipping File(批次執行) </summary>
        [PXButton]
        [PXUIField(DisplayName = "Generate YUSEN NL Shipping File", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumGererateYUSENNLFile(PXAdapter adapter, [PXDate] DateTime? shipDate, [PXInt] int? siteID, [SOOperation.List] string operation)
        {
            using (PXTransactionScope sc = new PXTransactionScope())
            {
                try
                {
                    // variable
                    var shipmentList = new List<SOShipment>();
                    var soList = adapter.Get<SOOrder>().ToList();
                    var soListwithoutFM = new List<object>();
                    soListwithoutFM.AddRange(soList.Where(x => x.OrderType != "FM"));
                    var graph = PXGraph.CreateInstance<SOShipmentEntry>();

                    // Find soOrder type != FM
                    var newAdapter = new PXAdapter(new LumShipmentDocView(Base, adapter.View.BqlSelect, soListwithoutFM));
                    newAdapter.MassProcess = true;
                    newAdapter.Arguments = adapter.Arguments;
                    // Create SOShipment Graph
                    Base.CreateShipmentIssue(newAdapter, shipDate, siteID);
                    if ((PXLongOperation.GetCustomInfoForCurrentThread("PXProcessingState") as PXProcessingInfo).Errors != 0)
                        return adapter.Get();

                    // Get Shipments
                    foreach (var order in soList.Where(x => x.OrderType != "FM"))
                    {
                        var soOrderShipment = SelectFrom<SOOrderShipment>.Where<SOOrderShipment.orderNbr.IsEqual<P.AsString>
                                                                               .And<SOOrderShipment.orderType.IsEqual<P.AsString>>>
                                               .View.Select(Base, order.OrderNbr, order.OrderType).RowCast<SOOrderShipment>().FirstOrDefault();
                        var shipment = SelectFrom<SOShipment>.Where<SOShipment.shipmentNbr.IsEqual<P.AsString>>
                                .View.Select(Base, soOrderShipment.ShipmentNbr).RowCast<SOShipment>().FirstOrDefault();

                        // update field
                        shipment.GetExtension<SOShipmentExt>().UsrSendToWareHouse = true;
                        graph.Document.Update(shipment);

                        // Remove Hold (Shipments)
                        var shipAdapter = new PXAdapter(graph.Document) { Searches = new Object[] { shipment.ShipmentNbr } };
                        graph.releaseFromHold.PressButton(shipAdapter);
                        if ((PXLongOperation.GetCustomInfoForCurrentThread("PXProcessingState") as PXProcessingInfo).Errors != 0)
                            return adapter.Get();

                        shipmentList.Add(shipment);
                    }
                    // Generate NL File, Success will throw PXRedirectToFileException 
                    int totalLine = 1;
                    StringBuilder sb = new StringBuilder();
                    string line = string.Empty;

                    #region FileHeader - HDR

                    sb = graph.GetExtension<SOShipmentEntryExt>().CombineYusenHedaer(sb);

                    #endregion

                    // General Detail
                    var result = graph.GetExtension<SOShipmentEntryExt>().CombineYusenDetail(sb, shipmentList, totalLine);
                    sb = result.sb;

                    // FBM Yuesn Detail
                    result = graph.GetExtension<SOShipmentEntryExt>().CombineYusenDetailForFBM(sb, soList.Where(x => x.OrderType == "FM").ToList(), result.totalLine);
                    sb = result.sb;

                    #region Filetrailer – TRL

                    sb = graph.GetExtension<SOShipmentEntryExt>().CombineYusenFooter(sb, result.totalLine);

                    #endregion

                    // Create SM.FileInfo
                    var fileName = $"Yusen-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
                    var data = new UTF8Encoding(true).GetBytes(sb.ToString());
                    FileInfo fi = new FileInfo(fileName, null, data);

                    // DownLoad File
                    if ((PXLongOperation.GetCustomInfoForCurrentThread("PXProcessingState") as PXProcessingInfo).Errors == 0)
                        throw new PXRedirectToFileException(fi, true);
                }
                // Success
                catch (PXRedirectToFileException)
                {
                    sc.Complete();
                    throw;
                }
                catch (Exception ex)
                {
                    PXProcessing.SetError(ex.Message);
                }
            }

            return adapter.Get();
        }

        /// <summary> 產生CA Shipping File(逐筆執行) </summary>
        [PXButton]
        [PXUIField(DisplayName = "Generate YUSEN CA Shipping File", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumGenerateYUSENCAFile(PXAdapter adapter, [PXDate] DateTime? shipDate, [PXInt] int? siteID, [SOOperation.List] string operation)
        {
            try
            {
                // Create SOShipment Graph
                var graph = PXGraph.CreateInstance<SOShipmentEntry>();
                var soOrder = adapter.Get<SOOrder>().FirstOrDefault();
                using (PXTransactionScope sc = new PXTransactionScope())
                {
                    // FBM wont create Shipment, only upload file to FTP
                    if (soOrder.OrderType == "FM")
                    {
                        // Combine csv data
                        var result = graph.GetExtension<SOShipmentEntryExt>().CombineCSVForFBM(soOrder, "YUSEN");
                        // Upload Graph
                        UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                        // Create SM.FileInfo
                        var fileName = $"{soOrder.OrderNbr}.csv";
                        var data = new UTF8Encoding(true).GetBytes(result.csvText.ToString());
                        FileInfo fi = new FileInfo(fileName, null, data);

                        // upload file to FTP
                        #region Yusen CA FTP
                        var configYusen = SelectFrom<LUMYusenCASetup>.View.Select(Base).RowCast<LUMYusenCASetup>().FirstOrDefault();
                        FTP_Config config = new FTP_Config()
                        {
                            FtpHost = configYusen.FtpHost,
                            FtpUser = configYusen.FtpUser,
                            FtpPass = configYusen.FtpPass,
                            FtpPort = configYusen.FtpPort,
                            FtpPath = configYusen.FtpPath
                        };
                        var ftpResult = graph.GetExtension<SOShipmentEntryExt>().UploadFileByFTP(config, fileName, data);
                        //var ftpResult = true;
                        if (!ftpResult)
                            throw new Exception("Ftp Upload Fail!!");
                        #endregion

                        // upload file to Attachment
                        upload.SaveFile(fi);
                        PXNoteAttribute.SetFileNotes(Base.Document.Cache, Base.Document.Current, fi.UID.Value);
                        Base.Save.Press();
                        PXProcessing.SetProcessed();
                    }
                    else
                    {
                        // Create Shipment
                        Base.CreateShipmentIssue(adapter, shipDate, siteID);
                        if (PXProcessing<SOOrder>.GetItemMessage().ErrorLevel != PXErrorLevel.RowInfo)
                            return null;

                        // Find SOShipment
                        var _soOrderShipment =
                            FbqlSelect<SelectFromBase<SOOrderShipment, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrderShipment.orderType, Equal<P.AsString>>>>>.And<BqlOperand<SOOrderShipment.orderNbr, IBqlString>.IsEqual<P.AsString>>>, SOOrderShipment>.View.Select(Base, soOrder.OrderType, soOrder.OrderNbr)
                                .RowCast<SOOrderShipment>().FirstOrDefault();

                        // Create new Adapter
                        var newAdapter = new PXAdapter(graph.Document) { Searches = new Object[] { _soOrderShipment.ShipmentNbr } };
                        // Generate CA csv file and upload to FTP
                        graph.GetExtension<SOShipmentEntryExt>().lumGenerateYUSENCAFile.PressButton(newAdapter);
                        // Remove Hold
                        graph.releaseFromHold.PressButton(newAdapter);

                    }
                    if (PXProcessing<SOOrder>.GetItemMessage().ErrorLevel == PXErrorLevel.RowInfo)
                        sc.Complete();
                }
            }
            catch (Exception ex)
            {
                PXProcessing.SetError<SOOrder>(ex.Message);
            }
            return adapter.Get();
        }

        /// <summary> 產生 3PL UK Shipping File(逐筆執行) </summary>
        [PXButton]
        [PXUIField(DisplayName = "Generate 3PL UK Shipping File", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumGenerate3PLUKFile(PXAdapter adapter, [PXDate] DateTime? shipDate, [PXInt] int? siteID, [SOOperation.List] string operation)
        {
            try
            {
                // Create SOShipment Graph
                var graph = PXGraph.CreateInstance<SOShipmentEntry>();
                var soOrder = adapter.Get<SOOrder>().FirstOrDefault();
                using (PXTransactionScope sc = new PXTransactionScope())
                {
                    // FBM wont create Shipment, only upload file to FTP
                    if (soOrder.OrderType == "FM")
                    {
                        // Combine csv data
                        var result = graph.GetExtension<SOShipmentEntryExt>().CombineCSVForFBM(soOrder, "P3PL");
                        // Upload Graph
                        UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                        // Create SM.FileInfo
                        var fileName = $"{soOrder.OrderNbr}.csv";
                        var data = new UTF8Encoding(true).GetBytes(result.csvText.ToString());
                        FileInfo fi = new FileInfo(fileName, null, data);

                        // upload file to FTP
                        #region 3PL UK FTP
                        var configYusen = SelectFrom<LUM3PLUKSetup>.View.Select(Base).RowCast<LUM3PLUKSetup>().FirstOrDefault();
                        FTP_Config config = new FTP_Config()
                        {
                            FtpHost = configYusen.FtpHost,
                            FtpUser = configYusen.FtpUser,
                            FtpPass = configYusen.FtpPass,
                            FtpPort = configYusen.FtpPort,
                            FtpPath = configYusen.FtpPath
                        };

                        var ftpResult = graph.GetExtension<SOShipmentEntryExt>().UploadFileByFTP(config, fileName, data);
                        //var ftpResult = true;
                        if (!ftpResult)
                            throw new Exception("Ftp Upload Fail!!");
                        #endregion

                        // upload file to Attachment
                        upload.SaveFile(fi);
                        PXNoteAttribute.SetFileNotes(Base.Document.Cache, Base.Document.Current, fi.UID.Value);
                        Base.Save.Press();
                        PXProcessing.SetProcessed();
                    }
                    else
                    {
                        // Create Shipment
                        Base.CreateShipmentIssue(adapter, shipDate, siteID);
                        if (PXProcessing<SOOrder>.GetItemMessage().ErrorLevel != PXErrorLevel.RowInfo)
                            return null;

                        // Find SOShipment
                        var _soOrderShipment =
                            FbqlSelect<SelectFromBase<SOOrderShipment, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrderShipment.orderType, Equal<P.AsString>>>>>.And<BqlOperand<SOOrderShipment.orderNbr, IBqlString>.IsEqual<P.AsString>>>, SOOrderShipment>.View.Select(Base, soOrder.OrderType, soOrder.OrderNbr)
                                .RowCast<SOOrderShipment>().FirstOrDefault();

                        // Create new Adapter
                        var newAdapter = new PXAdapter(graph.Document) { Searches = new Object[] { _soOrderShipment.ShipmentNbr } };
                        // Generate UK csv file and upload to FTP
                        graph.GetExtension<SOShipmentEntryExt>().lumGenerate3PLUKFile.PressButton(newAdapter);
                        // Remove Hold
                        graph.releaseFromHold.PressButton(newAdapter);

                    }
                    if (PXProcessing<SOOrder>.GetItemMessage().ErrorLevel == PXErrorLevel.RowInfo)
                        sc.Complete();
                }
            }
            catch (Exception ex)
            {
                PXProcessing.SetError<SOOrder>(ex.Message);
            }
            return adapter.Get();
        }

        public PXAction<SOOrder> createSOContact;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Create Contact", MapEnableRights = PXCacheRights.Select, Visible = true)]
        protected virtual IEnumerable CreateSOContact(PXAdapter adapter)
        {
            SOOrder order = adapter.Get().RowCast<SOOrder>().First();
            SOContact contact = Base.Billing_Contact.Current;
            SOAddress address = Base.Billing_Address.Current;

            if (contact != null && address != null)
            {
                if (string.IsNullOrEmpty(contact.Email))
                {
                    throw new ArgumentNullException(string.Format("{0} {1}", PX.Objects.AR.Messages.ARBillingContact.Substring(3), nameof(SOContact.Email)));
                }

                MyArray myArray = new MyArray()
                {
                    BillingEmail = contact.Email,
                    BillingLastName = contact.Attention,
                    BillingPhoneNumber = contact.Phone1,
                    BillingAddress = address.AddressLine1,
                    BillingAddress2 = address.AddressLine2,
                    BillingCity = address.City,
                    BillingCountry = address.CountryID,
                    BillingZipCode = address.PostalCode,
                    BillingState = address.State
                };

                order.ContactID = ThreeDCartHelper.CreateSOContact(order.CustomerID, myArray);

                Base.CurrentDocument.Cache.MarkUpdated(order);
                Base.Save.Press();
            }

            return adapter.Get();
        }

        public PXAction<SOOrder> updateOpenInvoice;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Update Open Invoice", MapEnableRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable UpdateOpenInvoice(PXAdapter adapter)
        {
            var order = Base.CurrentDocument.Current;

            if (order != null)
            {
                string noteInfo = PXNoteAttribute.GetNote(Base.CurrentDocument.Cache, order);

                APILibrary.Model.Root root = JsonConvert.DeserializeObject<APILibrary.Model.Root>(noteInfo);

                if (root != null)
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        if (root.description == null) { goto Item; }

                        order.OrderDesc = root.description;
                        Base.CurrentDocument.Cache.MarkUpdated(order);

                        PXUpdateJoin<Set<ARInvoice.docDesc, Required<SOOrder.orderDesc>>,
                                     ARInvoice,
                                     InnerJoin<SOOrderShipment, On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                                                                   And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>,
                                     Where<SOOrderShipment.orderNoteID, Equal<Required<SOOrder.noteID>>>>.Update(Base, root.description, order.NoteID);

                        Item:
                        if (root.item != null)
                        {
                            for (int i = 0; i < root.item.Count; i++)
                            {
                                if (root.item[i].sku == null) { goto Invoice; }

                                foreach (SOLine row in Base.Transactions.Select())
                                {
                                    if (string.Format("{0}-{1}", (int)row.OrderQty, InventoryItem.PK.Find(Base, row.InventoryID).InventoryCD.TrimEnd()).CompareTo(root.item[i].qty + "-" + root.item[i].sku) == 0)
                                    {
                                        row.GetExtension<SOLineExt>().UsrAmazWHTaxAmt = (decimal)root.item[i].whTax;

                                        Base.Transactions.Cache.MarkUpdated(row);
                                    }
                                }
                            }

                            Base.Save.Press();
                        }

                        Invoice:
                        if (root.taxAmount != 0)
                        {
                            PXUpdateJoin<Set<TaxTran.curyTaxAmt, Required<TaxTran.curyTaxAmt>>,
                                         TaxTran,
                                         LeftJoin<ARInvoice, On<TaxTran.module, Equal<PX.Objects.GL.BatchModule.moduleAR>,
                                                                And<ARInvoice.docType, Equal<TaxTran.tranType>,
                                                                    And<ARInvoice.refNbr, Equal<TaxTran.refNbr>>>>,
                                                  InnerJoin<SOOrderShipment, On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                                                                                And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>>,
                                         Where<SOOrderShipment.orderNoteID, Equal<Required<SOOrder.noteID>>>>.Update(Base, (decimal)root.taxAmount, order.NoteID);

                            PXUpdateJoin<Set<ARInvoice.curyTaxTotal, Required<ARInvoice.curyTaxTotal>>,
                                         ARInvoice,
                                         InnerJoin<SOOrderShipment, On<SOOrderShipment.invoiceType, Equal<ARInvoice.docType>,
                                                                       And<SOOrderShipment.invoiceNbr, Equal<ARInvoice.refNbr>>>>,
                                         Where<SOOrderShipment.orderNoteID, Equal<Required<SOOrder.noteID>>>>.Update(Base, (decimal)root.taxAmount, order.NoteID);
                        }

                        PXNoteAttribute.SetNote(Base.CurrentDocument.Cache, order, null);

                        ts.Complete();
                    }
                }
            }

            return adapter.Get();
        }
        #endregion

        #region Event

        public virtual void _(Events.RowSelected<SOOrder> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row != null)
            {
                // Set Create shipment is DCL
                if (row.Status == SOOrderStatus.Open && !(row.GetExtension<SOOrderExt>().UsrDCLShipmentCreated ?? false))
                    Base.action.SetEnabled(nameof(CreateDCLShipment), true);
                else
                    Base.action.SetEnabled(nameof(CreateDCLShipment), false);

                // Set Call DCL for Shipment
                if (row.Status == SOOrderStatus.Open && (row.GetExtension<SOOrderExt>().UsrDCLShipmentCreated ?? false) && row.OrderType != "VC")
                    Base.action.SetEnabled(nameof(LumCallDCLShipemnt), true);
                else
                    Base.action.SetEnabled(nameof(LumCallDCLShipemnt), false);
            }
        }

        #endregion

        #region Method
        /// <summary> Combine DCL Shipment MetaData(JSON) </summary>
        /// Shipping Carrier and Server Rule : soOrder.OrderWeight >= 150 -> UPS FREIGHT ;other -> UPS(Default or ShipVia)
        public void CombineDLCShipmentEntity(DCLShipmentRequestEntity model, SOOrder soOrder)
        {
            if (model == null)
                return;

            var soLine = SelectFrom<SOLine>.Where<SOLine.orderNbr.IsEqual<P.AsString>
                .And<SOLine.orderType.IsEqual<P.AsString>>>
                .View
                .Select(Base, soOrder.OrderNbr, soOrder.OrderType)
                .RowCast<SOLine>()
                .ToList();

            var shippingContact = SOShippingContact.PK.Find(Base, soOrder.ShipContactID) ?? new SOShippingContact();
            var shippingAddress = SOShippingAddress.PK.Find(Base, soOrder.ShipAddressID) ?? new SOShippingAddress();
            var billingContact = SOBillingContact.PK.Find(Base, soOrder.BillContactID) ?? new SOBillingContact();
            var billingAddress = SOBillingAddress.PK.Find(Base, soOrder.BillAddressID) ?? new SOBillingAddress();
            model.allow_partial = true;
            model.location = "LA";

            List<APILibrary.Model.Line> dclLines = soLine.Any() ? new List<APILibrary.Model.Line>() : null;
            int count = 1;
            foreach (var item in soLine)
            {
                var inventory = InventoryItem.PK.Find(Base, item.InventoryID);
                APILibrary.Model.Line dclItem = new APILibrary.Model.Line()
                {
                    line_number = count++,
                    item_number = inventory.InventoryCD == "5-884-4-01-02" ? "5-884-4-01-00" : inventory.InventoryCD,
                    description = inventory.Descr,
                    quantity = (int)item?.OrderQty,
                    price = (double?)item.UnitPrice
                };
                dclLines.Add(dclItem);
            }

            string shippingCarrier = string.Empty;
            string shippingService = string.Empty;

            if (soOrder.OrderWeight >= 150 || soOrder.ShipVia == "UPSFREIGHT")
            {
                shippingCarrier = "UPS FREIGHT";
                shippingService = "STANDARD";
            }
            else if (soOrder.ShipVia == "UPSGROUND")
            {
                shippingCarrier = "UPS";
                shippingService = "GROUND";
            }
            model.orders = new List<Order>()
            {
                new Order()
                {
                    order_number = soOrder.OrderNbr,
                    account_number = soOrder.ShipVia == "UPSGROUND" ?"19311" : "19310",
                    ordered_date = soOrder.OrderDate?.ToString("yyyy-MM-dd"),
                    po_number = soOrder.CustomerOrderNbr,
                    customer_number = GetCurrAcctCD(soOrder.CustomerID),
                    acknowledgement_email = "logistic@ipevo.com",
                    freight_account = "00500",
                    shipping_carrier = shippingCarrier,
                    shipping_service = shippingService,
                    system_id = "P",
                    shipping_address =  new ShippingAddress()
                    {
                        company = shippingContact.FullName?.Length > 40 ? shippingContact.FullName.Substring(0,40) : shippingContact.FullName,
                        attention = shippingContact.Attention?.Length > 40 ? shippingContact.Attention.Substring(0,40) : shippingContact.FullName,
                        address1 = shippingAddress.AddressLine1,
                        address2 = shippingAddress.AddressLine2,
                        city = shippingAddress.City,
                        phone = shippingContact.Phone1,
                        email = shippingContact.Email,
                        state_province = shippingAddress.State,
                        postal_code = shippingAddress.PostalCode,
                        country_code = "US"
                    },
                    billing_address = new BillingAddress()
                    {
                        company = billingContact.FullName?.Length>40 ?  billingContact.FullName?.Substring(0,40) : billingContact.FullName,
                        attention = billingContact.Attention?.Length>40 ?  billingContact.Attention?.Substring(0,40) : billingContact.Attention,
                        address1 = billingAddress.AddressLine1,
                        address2 = billingAddress.AddressLine2,
                        phone = billingContact.Phone1,
                        email = billingContact.Email,
                        state_province = billingAddress.State,
                        postal_code = billingAddress.PostalCode,
                        country_code = "US"
                    },
                    international_code = 0,
                    order_subtotal = (double?)soOrder.OrderTotal,
                    shipping_handling = (double?)soOrder.FreightAmt,
                    sales_tax = (double?)soOrder.TaxTotal,
                    international_handling = 0,
                    total_due = (double?)soOrder.LineTotal,
                    amount_paid = 0,
                    net_due_currency = 0,
                    balance_due_us = 0,
                    international_declared_value = 0,
                    insurance = 0,
                    payment_type = string.Empty,
                    terms = string.Empty,
                    fob = string.Empty,
                    custom_field1 = soOrder.OrderNbr,
                    packing_list_type = 0,
                    packing_list_comments = string.Empty,
                    shipping_instructions = $@"1.Please note that use the plastic wrapper to secure all cartons onto the pallets for this order.\r\n
2.Please contact {shippingContact.FullName}@{shippingContact.Phone1} for delivery.",
                    lines = dclLines
                }
            };
        }

        /// <summary> Get BAAccount CD </summary>
        public string GetCurrAcctCD(int? baccountId)
        {
            return SelectFrom<PX.Objects.CR.BAccount>.Where<PX.Objects.CR.BAccount.bAccountID.IsEqual<P.AsInt>>
                .View.SelectSingleBound(Base, null, baccountId)
                .RowCast<PX.Objects.CR.BAccount>().FirstOrDefault()?.AcctCD;
        }

        #endregion
    }

    internal class LumShipmentDocView : PXView
    {
        List<object> _Records;
        internal LumShipmentDocView(PXGraph graph, BqlCommand command, List<object> records)
            : base(graph, true, command)
        {
            _Records = records;
        }
        public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
        {
            return _Records;
        }
    }
}
