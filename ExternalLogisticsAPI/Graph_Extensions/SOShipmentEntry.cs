using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.CarrierService;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.DependencyInjection;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
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

        public PXAction<SOShipment> printFedexLabel;

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

        #region Method

        protected virtual bool UseCarrierService(SOShipment row, Carrier carrier)
           => carrier != null && carrier.IsExternal == true ;

        #endregion
    }
}
