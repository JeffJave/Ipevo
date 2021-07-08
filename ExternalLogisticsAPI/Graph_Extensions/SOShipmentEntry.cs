using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;
using PX.CarrierService;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.DependencyInjection;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.SM;

namespace PX.Objects.SO
{
    public class SOShipmentEntryExt : PXGraphExtension<SOShipmentEntry>, IGraphWithInitialization
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.report.AddMenuAction(printFedexLabel);
        }

        #region Action

        public PXAction<SOShipment> printFedexLabel;
        public PXAction<SOShipment> lumGererateYUSENNLFile;
        public PXAction<SOShipment> lumGenerateYUSENCAFile;
        public PXAction<SOShipment> lumGenerate3PLUKFile;

        [PXButton]
        [PXUIField(DisplayName = "Print Fedex Label", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrintFedexLabel(PXAdapter adapter)
        {
            var shiporder = Base.Document.Current;
            var carrier = Carrier.PK.Find(Base, shiporder.ShipVia);
            if (!UseCarrierService(shiporder, carrier))
                return adapter.Get();

            if (shiporder.ShippedViaCarrier != true)
            {
                // Build Fedex Request object
                ICarrierService cs = CarrierMaint.CreateCarrierService(Base, shiporder.ShipVia);
                CarrierRequest cr = Base.CarrierRatesExt.BuildRequest(shiporder);
                var warehouseInfo = SelectFrom<INSite>.Where<INSite.siteID.IsEqual<P.AsInt>>.View
                    .Select(Base, shiporder.SiteID).RowCast<INSite>().FirstOrDefault();
                // Replace ShipTo Info to DCL warehouse
                Address warehouseAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Address.addressID>>>>.Select(Base, warehouseInfo?.AddressID);
                Contact warehouseContact = PXSelect<Contact, Where<Contact.contactID, Equal<Required<Contact.contactID>>>>.Select(Base, warehouseInfo?.ContactID);
                cr.Destination = warehouseAddress;
                cr.DestinationContact = warehouseContact;

                if (cr.Packages.Count > 0)
                {
                    // Get Fedex web service data
                    CarrierResult<ShipResult> result = cs.Ship(cr);

                    if (result != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (Message message in result.Messages)
                        {
                            sb.AppendFormat("{0}:{1} ", message.Code, message.Description);
                        }

                        if (result.IsSuccess)
                        {
                            using (PXTransactionScope ts = new PXTransactionScope())
                            {
                                UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();

                                foreach (PackageData pd in result.Result.Data)
                                {
                                    if (pd.Image != null)
                                    {
                                        string fileName = string.Format("Label #{0}.{1}", pd.TrackingNumber, pd.Format);
                                        FileInfo file = new FileInfo(fileName, null, pd.Image);
                                        try
                                        {
                                            upload.SaveFile(file);
                                        }
                                        catch (PXNotSupportedFileTypeException exc)
                                        {
                                            throw new PXException(exc, Messages.NotSupportedFileTypeFromCarrier, pd.Format);
                                        }
                                        PXNoteAttribute.SetFileNotes(Base.Document.Cache, Base.Document.Current, file.UID.Value);
                                    }
                                    Base.Document.UpdateCurrent();
                                }

                                Base.Save.Press();
                                ts.Complete();
                            }
                            //show warnings:
                            if (result.Messages.Count > 0)
                            {
                                Base.Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(shiporder, shiporder.CuryFreightCost,
                                    new PXSetPropertyException(sb.ToString(), PXErrorLevel.Warning));
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(result.RequestData))
                                PXTrace.WriteError(result.RequestData);

                            Base.Document.Cache.RaiseExceptionHandling<SOShipment.curyFreightCost>(shiporder, shiporder.CuryFreightCost,
                                    new PXSetPropertyException(Messages.CarrierServiceError, PXErrorLevel.Error, sb.ToString()));

                            throw new PXException(Messages.CarrierServiceError, sb.ToString());
                        }
                    }
                }
            }

            return adapter.Get();
        }

        [PXButton]
        [PXUIField(DisplayName = "Resend YUSEN NL Shipping File", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumGererateYUSENNLFile(PXAdapter adapter)
        {
            int totalLine = 1;
            try
            {
                var currtShipments = adapter.Get<SOShipment>().ToList();

                StringBuilder sb = new StringBuilder();
                string line = string.Empty;

                #region FileHeader - HDR

                sb = CombineYusenHedaer(sb);

                #endregion

                var result = CombineYusenDetail(sb, currtShipments, totalLine);
                sb = result.sb;

                #region Filetrailer – TRL

                sb = CombineYusenFooter(sb, result.totalLine);

                #endregion

                // Create SM.FileInfo
                var fileName = $"Yusen-{DateTime.Now.ToString("yyyyMMddHHmmss")}.csv";
                var data = new UTF8Encoding(true).GetBytes(sb.ToString());
                FileInfo fi = new FileInfo(fileName, null, data);

                // DownLoad File
                if ((PXLongOperation.GetCustomInfoForCurrentThread("PXProcessingState") as PXProcessingInfo).Errors == 0)
                    throw new PXRedirectToFileException(fi, true);
            }
            catch (PXRedirectToFileException)
            {
                throw;
            }
            catch (Exception ex)
            {
                PXProcessing.SetError(ex.Message);
            }
            return adapter.Get();
        }

        /// <summary> Resend YUSEN CA Shipping File </summary>
        [PXButton]
        [PXUIField(DisplayName = "Resend YUSEN CA Shipping File", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumGenerateYUSENCAFile(PXAdapter adapter)
        {
            try
            {
                SOShipment soShipment = adapter.Get<SOShipment>()?.FirstOrDefault();
                // Get Csv String Builder
                var result = CombineCSV(soShipment, "YUSEN");
                // Upload Graph
                UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                // Create SM.FileInfo
                var fileName = $"{result.OrderNbr}.csv";
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
                var ftpResult = UploadFileByFTP(config, fileName, data);
                //var ftpResult = true;
                if (!ftpResult)
                    throw new Exception("Ftp Upload Fail!!");
                #endregion

                // upload file to Attachment
                upload.SaveFile(fi);
                PXNoteAttribute.SetFileNotes(Base.Document.Cache, Base.Document.Current, fi.UID.Value);
                Base.Document.Current.GetExtension<SOShipmentExt>().UsrSendToWareHouse = true;
                Base.Document.UpdateCurrent();
                Base.Save.Press();
            }
            catch (Exception ex)
            {
                //PXProcessing.SetError<SOShipment>(ex.Message);
                PXProcessing.SetError(ex.Message);
            }
            return adapter.Get();
        }

        /// <summary> Resend 3PL UK Shipping File </summary>
        [PXButton]
        [PXUIField(DisplayName = "Resend 3PL UK Shipping File", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumGenerate3PLUKFile(PXAdapter adapter)
        {
            try
            {
                SOShipment soShipment = adapter.Get<SOShipment>()?.FirstOrDefault();
                // Get Csv String Builder
                var result = CombineCSV(soShipment, "P3PL");
                // Upload Graph
                UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                // Create SM.FileInfo
                var fileName = $"{result.OrderNbr}.csv";
                var data = new UTF8Encoding(true).GetBytes(result.csvText.ToString());
                FileInfo fi = new FileInfo(fileName, null, data);
                // Upload file to FTP
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

                var ftpResult = UploadFileByFTP(config, fileName, data);
                //var ftpResult = true;
                if (!ftpResult)
                    throw new Exception("Ftp Upload Fail!!");
                #endregion

                // Upload file to Attachment
                upload.SaveFile(fi);
                PXNoteAttribute.SetFileNotes(Base.Document.Cache, Base.Document.Current, fi.UID.Value);
                Base.Document.Current.GetExtension<SOShipmentExt>().UsrSendToWareHouse = true;
                Base.Document.UpdateCurrent();
                Base.Save.Press();
            }
            catch (Exception ex)
            {
                PXProcessing.SetError(ex.Message);
            }

            return adapter.Get();
        }

        #endregion

        #region Method

        protected virtual bool UseCarrierService(SOShipment row, Carrier carrier)
           => carrier != null && carrier.IsExternal == true;

        #region CA/UK csv method

        /// <summary> Gnerate CA/UK csv data </summary>
        public virtual (StringBuilder csvText, string OrderNbr) CombineCSV(SOShipment soShipment, string csvType)
        {
            StringBuilder sb = new StringBuilder();
            string line = string.Empty;

            var inventoryItems = SelectFrom<InventoryItem>.View.Select(Base).RowCast<InventoryItem>().ToList();
            var shipLines = SelectFrom<SOShipLine>.Where<SOShipLine.shipmentNbr.IsEqual<P.AsString>>
                               .OrderBy<SOShipLine.lineNbr.Asc>
                               .View.Select(Base, soShipment?.ShipmentNbr).RowCast<SOShipLine>();
            SOShipmentContact shipContact = SOShipmentContact.PK.Find(Base, soShipment.ShipContactID);
            SOShipmentAddress shipAddress = SOShipmentAddress.PK.Find(Base, soShipment.ShipAddressID);
            SOOrderShipment soOrderShipment = SelectFrom<SOOrderShipment>
                                              .Where<SOOrderShipment.shipmentNbr.IsEqual<P.AsString>>
                                              .View.Select(Base, soShipment.ShipmentNbr).RowCast<SOOrderShipment>().FirstOrDefault();
            SOOrder soOrder = SelectFrom<SOOrder>
                              .Where<SOOrder.orderType.IsEqual<P.AsString>.And<SOOrder.orderNbr.IsEqual<P.AsString>>>
                              .View.Select(Base, soOrderShipment.OrderType, soOrderShipment.OrderNbr)
                              .RowCast<SOOrder>().FirstOrDefault();

            if (!shipLines.Any())
                throw new Exception("Can not find ShipLine");

            #region Header
            line += "\"CustomerCode\";\"OrderRefNo\";\"SKUCode\";\"Qty\";\"DeliveryReqDate\";\"ReceiverName\";\"ReceiverCountry\";\"ReceiverCity\";\"ReceiverPostCode\";\"ReceiverAddress\";\"ReceiverPhone\";\"BatchNumber\";\"Notes\"";
            if (csvType == "P3PL")
                line += ";\"CarrierCode\";\"CarrierServiceCode\"";
            sb.AppendLine(line);
            #endregion

            #region Row

            foreach (var item in shipLines)
            {
                var _cd = inventoryItems.Where(x => x.InventoryID == item.InventoryID).FirstOrDefault()?.InventoryCD;
                line = string.Empty;
                // CustomerCode
                line += $"\"IPEVOMAN\";";
                // OrderRefNo
                line += $"\"{soShipment.CustomerOrderNbr}\";";
                // SKUCode
                line += $"\"{_cd}\";";
                // Qty
                line += $"\"{item?.ShippedQty}\";";
                // DeliveryReqDate
                line += $"\"{DateTime.Now.ToString("yyyyMMdd")}\";";
                // ReceiverName
                line += $"\"{shipContact.Attention}/{shipContact.FullName}\";";
                // ReceiverCountry
                line += $"\"{shipAddress.CountryID}\";";
                // ReceiverCity
                line += $"\"{shipAddress.City}\";";
                // ReceiverPostCode
                line += $"\"{shipAddress.PostalCode}\";";
                // ReceiverAddress
                line += $"\"{(shipAddress?.AddressLine1 + shipAddress?.AddressLine2)}\";";
                // ReceiverPhone
                line += $"\"{shipContact.Phone1}\";";
                // BatchNumber
                line += $"\"{soOrder.OrderNbr}\";";
                // Notes
                var note = string.Empty;
                if (_cd == "5-883-4-01-00" || _cd == "5-884-4-01-00")
                    note = "VZ-R(sku#5-883-4-01-00) or VZ-X(sku#5-884-4-01-00) please scan the serial number for us.";
                if (soOrderShipment?.OrderType == "FM" && !string.IsNullOrEmpty(soOrder?.OrderDesc))
                    note = soOrder?.OrderDesc;
                line += $"\"{note}\"";
                if (csvType == "P3PL")
                    line += $";\"{string.Empty}\";\"{string.Empty}\"";
                sb.AppendLine(line);
            }

            #endregion

            return (sb, soOrder.OrderNbr);
        }

        /// <summary> Gnerate CA/UK csv data for FBM </summary>
        public virtual (StringBuilder csvText, string OrderNbr) CombineCSVForFBM(SOOrder _soOrder, string csvType)
        {
            StringBuilder sb = new StringBuilder();
            string line = string.Empty;

            var inventoryItems = SelectFrom<InventoryItem>.View.Select(Base).RowCast<InventoryItem>().ToList();
            var soLines = SelectFrom<SOLine>.Where<SOLine.orderNbr.IsEqual<P.AsString>
                                                    .And<SOLine.orderType.IsEqual<P.AsString>>>
                               .OrderBy<SOLine.lineNbr.Asc>
                               .View.Select(Base, _soOrder.OrderNbr, _soOrder.OrderType).RowCast<SOLine>();
            SOShipmentContact shipContact = SOShipmentContact.PK.Find(Base, _soOrder.ShipContactID);
            SOShipmentAddress shipAddress = SOShipmentAddress.PK.Find(Base, _soOrder.ShipAddressID);

            if (!soLines.Any())
                throw new Exception("Can not find SOLine");

            #region Header
            line += "\"CustomerCode\";\"OrderRefNo\";\"SKUCode\";\"Qty\";\"DeliveryReqDate\";\"ReceiverName\";\"ReceiverCountry\";\"ReceiverCity\";\"ReceiverPostCode\";\"ReceiverAddress\";\"ReceiverPhone\";\"BatchNumber\";\"Notes\"";
            if (csvType == "P3PL")
                line += ";\"CarrierCode\";\"CarrierServiceCode\"";
            sb.AppendLine(line);
            #endregion

            #region Row

            foreach (var item in soLines)
            {
                var _cd = inventoryItems.Where(x => x.InventoryID == item.InventoryID).FirstOrDefault()?.InventoryCD;
                line = string.Empty;
                // CustomerCode
                line += $"\"IPEVOMAN\";";
                // OrderRefNo
                line += $"\"{_soOrder.CustomerOrderNbr}\";";
                // SKUCode
                line += $"\"{_cd}\";";
                // Qty
                line += $"\"{item?.ShippedQty}\";";
                // DeliveryReqDate
                line += $"\"{DateTime.Now.ToString("yyyyMMdd")}\";";
                // ReceiverName
                line += $"\"{shipContact.Attention}/{shipContact.FullName}\";";
                // ReceiverCountry
                line += $"\"{shipAddress.CountryID}\";";
                // ReceiverCity
                line += $"\"{shipAddress.City}\";";
                // ReceiverPostCode
                line += $"\"{shipAddress.PostalCode}\";";
                // ReceiverAddress
                line += $"\"{(shipAddress?.AddressLine1 + shipAddress?.AddressLine2)}\";";
                // ReceiverPhone
                line += $"\"{shipContact.Phone1}\";";
                // BatchNumber
                line += $"\"{_soOrder.OrderNbr}\";";
                // Notes
                var note = string.Empty;
                if (_cd == "5-883-4-01-00" || _cd == "5-884-4-01-00")
                    note = "VZ-R(sku#5-883-4-01-00) or VZ-X(sku#5-884-4-01-00) please scan the serial number for us.";
                if (_soOrder?.OrderType == "FM" && !string.IsNullOrEmpty(_soOrder?.OrderDesc))
                    note = _soOrder?.OrderDesc;
                line += $"\"{note}\"";
                if (csvType == "P3PL")
                    line += $";\"{string.Empty}\";\"{string.Empty}\"";
                sb.AppendLine(line);
            }

            #endregion

            return (sb, _soOrder.OrderNbr);
        }

        #endregion

        #region YUSEN csv method

        /// <summary> Combine Yusen csv Header (HDR)  </summary>
        public virtual StringBuilder CombineYusenHedaer(StringBuilder sb)
        {
            #region FileHeader - HDR

            string line = $"{FillUpLen(10, "YS_OR_V01")}{FillUpLen(3, "HDR")}{FillUpLen(15, "IPEVO")}{FillUpLen(15, "Yusen")}{DateTime.Now.ToString("yyyyMMdd")}{DateTime.Now.ToString("HH:mm")}\r\n";
            sb.Append(line);

            #endregion
            return sb;
        }

        /// <summary> Combine Yusen csv Detail (ORH、ORL、ORA、FTO、FTD)  </summary>
        public virtual (StringBuilder sb, int totalLine) CombineYusenDetail(StringBuilder sb, List<SOShipment> currtShipments, int totalLine)
        {
            for (int i = 0; i < currtShipments.Count; i++)
            {
                string line = string.Empty;
                SOShipment soShip = currtShipments[i];
                var inventoryItems = SelectFrom<InventoryItem>.View.Select(Base).RowCast<InventoryItem>().ToList();

                var shipLines = SelectFrom<SOShipLine>.Where<SOShipLine.shipmentNbr.IsEqual<P.AsString>>
                                .OrderBy<SOShipLine.lineNbr.Asc>
                                .View.Select(Base, soShip?.ShipmentNbr).RowCast<SOShipLine>();
                SOShipmentContact shipContact = SOShipmentContact.PK.Find(Base, soShip.ShipContactID);
                SOShipmentAddress shipAddress = SOShipmentAddress.PK.Find(Base, soShip.ShipAddressID);

                if (!shipLines.Any())
                    throw new Exception("Can not find ShipLine");

                #region OrderHeader - ORH
                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "ORH")}";
                line += $"{FillUpLen(8, soShip?.ShipDate?.ToString("yyyyMMdd"))}";
                line += $"{FillUpLen(8, shipLines.FirstOrDefault()?.ExpireDate?.ToString("yyyyMMdd"))}";
                line += $"L";
                line += $"{FillUpLen(15, shipLines.FirstOrDefault()?.OrigOrderNbr)}";
                line += $"{FillUpLen(20, soShip?.CustomerOrderNbr)}";
                line += $"{FillUpLen(11, " ")}";
                line += $"{FillUpLen(40, soShip?.ShipmentDesc)}";
                line += $"{FillUpLen(40, " ")}";
                line += $"{FillUpLen(5, "DPD")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

                #region Physical delivery address – ORL
                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "ORL")}";
                line += $"{FillUpLen(10, " ")}";
                line += $"{FillUpLen(40, shipContact?.FullName)}";
                line += $"{FillUpLen(40, " ")}";
                line += (shipAddress?.AddressLine1 + shipAddress?.AddressLine2).Length > 30 ? $"{(shipAddress?.AddressLine1 + shipAddress?.AddressLine2).Substring(0, 30)}" : $"{FillUpLen(30, (shipAddress?.AddressLine1 + shipAddress?.AddressLine2))}";
                line += $"{FillUpLen(10, shipAddress?.PostalCode)}";
                line += $"{FillUpLen(30, shipAddress?.City)}";
                line += $"{FillUpLen(3, shipAddress?.CountryID)}";
                line += $"{FillUpLen(30, shipContact?.Attention)}";
                line += $"{FillUpLen(20, shipContact?.Phone1)}";
                line += $"{FillUpLen(40, shipContact?.Email)}";
                line += $"{FillUpLen(3, "DE")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

                #region Articleline - ORA

                foreach (var item in shipLines)
                {
                    line = string.Empty;
                    line += $"{FillUpLen(10, "YS_OR_V01")}";
                    line += $"{FillUpLen(3, "ORA")}";
                    line += $"{FillUpLen(10, item?.LineNbr.ToString())}";
                    line += $"{FillUpLen(20, inventoryItems?.FirstOrDefault(x => x.InventoryID == item?.InventoryID).InventoryCD)}";
                    line += $"{FillUpLen(11, item?.ShippedQty?.ToString())}";
                    line += $"{FillUpLen(25, " ")}";
                    line += $"{FillUpLen(25, " ")}";
                    line += "\r\n";
                    sb.Append(line);
                    totalLine++;
                }

                #endregion

                #region  Free Text Orderpick - FTO

                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "FTO")}";
                line += $"{FillUpLen(40, "")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

                #region Free Text Delivery - FTD

                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "FTD")}";
                line += $"{FillUpLen(40, " ")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

            }
            return (sb, totalLine);
        }

        /// <summary> Combine Yusen csv Detail (ORH、ORL、ORA、FTO、FTD)  </summary>
        public virtual (StringBuilder sb, int totalLine) CombineYusenDetailForFBM(StringBuilder sb, List<SOOrder> currtOrders, int totalLine)
        {
            for (int i = 0; i < currtOrders.Count; i++)
            {
                string line = string.Empty;
                SOOrder _soOrder = currtOrders[i];
                var inventoryItems = SelectFrom<InventoryItem>.View.Select(Base).RowCast<InventoryItem>().ToList();

                var soLines = SelectFrom<SOLine>.Where<SOLine.orderType.IsEqual<P.AsString>
                                                    .And<SOLine.orderNbr.IsEqual<P.AsString>>>
                                .OrderBy<SOLine.lineNbr.Asc>
                                .View.Select(Base, _soOrder.OrderType, _soOrder.OrderNbr).RowCast<SOLine>();
                SOShipmentContact shipContact = SOShipmentContact.PK.Find(Base, _soOrder.ShipContactID);
                SOShipmentAddress shipAddress = SOShipmentAddress.PK.Find(Base, _soOrder.ShipAddressID);

                if (!soLines.Any())
                    throw new Exception("Can not find SOLine");

                #region OrderHeader - ORH
                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "ORH")}";
                line += $"{FillUpLen(8, _soOrder?.OrderDate?.ToString("yyyyMMdd"))}";
                line += $"{FillUpLen(8, _soOrder?.RequestDate?.ToString("yyyyMMdd"))}";
                line += $"L";
                line += $"{FillUpLen(15, _soOrder?.OrderNbr)}";
                line += $"{FillUpLen(20, _soOrder?.CustomerOrderNbr)}";
                line += $"{FillUpLen(11, " ")}";
                line += $"{FillUpLen(40, (_soOrder.OrderDesc?.Length > 40 ? _soOrder?.OrderDesc.Substring(0, 40) : _soOrder?.OrderDesc))}";
                line += $"{FillUpLen(40, (_soOrder.OrderDesc?.Length > 40 ? _soOrder?.OrderDesc.Substring(40) : " "))}";
                line += $"{FillUpLen(5, "DPD")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

                #region Physical delivery address – ORL
                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "ORL")}";
                line += $"{FillUpLen(10, " ")}";
                line += $"{FillUpLen(40, shipContact?.FullName)}";
                line += $"{FillUpLen(40, " ")}";
                line += (shipAddress?.AddressLine1 + shipAddress?.AddressLine2).Length > 30 ? $"{(shipAddress?.AddressLine1 + shipAddress?.AddressLine2).Substring(0, 30)}" : $"{FillUpLen(30, (shipAddress?.AddressLine1 + shipAddress?.AddressLine2))}";
                line += $"{FillUpLen(10, shipAddress?.PostalCode)}";
                line += $"{FillUpLen(30, shipAddress?.City)}";
                line += $"{FillUpLen(3, shipAddress?.CountryID)}";
                line += $"{FillUpLen(30, shipContact?.Attention)}";
                line += $"{FillUpLen(20, shipContact?.Phone1)}";
                line += $"{FillUpLen(40, shipContact?.Email)}";
                line += $"{FillUpLen(3, "DE")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

                #region Articleline - ORA

                foreach (var item in soLines)
                {
                    line = string.Empty;
                    line += $"{FillUpLen(10, "YS_OR_V01")}";
                    line += $"{FillUpLen(3, "ORA")}";
                    line += $"{FillUpLen(10, item?.LineNbr.ToString())}";
                    line += $"{FillUpLen(20, inventoryItems?.FirstOrDefault(x => x.InventoryID == item?.InventoryID).InventoryCD)}";
                    line += $"{FillUpLen(11, item?.OpenQty?.ToString())}";
                    line += $"{FillUpLen(25, " ")}";
                    line += $"{FillUpLen(25, " ")}";
                    line += "\r\n";
                    sb.Append(line);
                    totalLine++;
                }

                #endregion

                #region  Free Text Orderpick - FTO

                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "FTO")}";
                line += $"{FillUpLen(40, "")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

                #region Free Text Delivery - FTD

                line = string.Empty;
                line += $"{FillUpLen(10, "YS_OR_V01")}";
                line += $"{FillUpLen(3, "FTD")}";
                line += $"{FillUpLen(40, " ")}";
                line += "\r\n";
                sb.Append(line);
                totalLine++;

                #endregion

            }
            return (sb, totalLine);
        }

        /// <summary> Combine Yusen csv Footer (TRL)  </summary>
        public virtual StringBuilder CombineYusenFooter(StringBuilder sb, int totalLine)
        {
            #region Filetrailer – TRL

            string line = string.Empty;
            line += $"{FillUpLen(10, "YS_OR_V01")}";
            line += $"{FillUpLen(3, "TRL")}";
            line += $"{FillUpLen(5, totalLine.ToString())}";
            line += "\r\n";
            sb.Append(line);

            #endregion
            return sb;
        }

        #endregion

        /// <summary> Upload csv to UK/CA FTP </summary>
        public virtual bool UploadFileByFTP(FTP_Config config, string fileName, byte[] data)
        {
            FTPHelper helper = new FTPHelper(config);
            var ftpResult = helper.UploadFileToFTP(data, fileName);
            return ftpResult;
        }

        /// <summary> Fill Up string length </summary>
        public virtual string FillUpLen(int len, string str)
        {
            str = str ?? string.Empty;
            if (str.Length > len)
                return str.Substring(0, len);
            return str.PadRight(len, ' ');
        }

        #endregion
    }
}
