using PX.CarrierService;
using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static PX.Objects.SO.SOOrderEntry;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Objects.IN;
using PX.Objects.SO;
using static PX.Objects.SO.SOOrderEntry;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;

namespace PX.Objects.CR
{
    public class QuoteMaintExt : PXGraphExtension<QuoteMaint>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.actionsFolder.AddMenuAction(lumCalcFreightCost);
        }

        public PXAction<CRQuote> lumCalcFreightCost;
        [PXButton]
        [PXUIField(DisplayName = "Calacuate Freight Cost", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable LumCalcFreightCost(PXAdapter adapter)
        {
            CalculateFreightCost(false);
            return adapter.Get();
        }

        public CarrierRates CarrierRatesExt(SOOrderEntry graph)
            => graph.FindImplementation<CarrierRates>();

        private void CalculateFreightCost(bool supressErrors)
        {
            Carrier carrier = Carrier.PK.Find(Base, "UPSGROUND");
            if (carrier != null && carrier.IsExternal == true)
            {
                var _doc = new SOOrder();
                //_doc = SelectFrom<SOOrder>.Where<SOOrder.orderNbr.IsEqual<P.AsString>.And<SOOrder.orderType.IsEqual<P.AsString>>>.View.Select(Base, "SUS2100212", "SO").RowCast<SOOrder>().FirstOrDefault();
                _doc.CuryID = Base.Quote.Current.CuryID;
                _doc.ShipVia = "UPSGROUND";
                _doc.CuryInfoID = Base.Quote.Current.CuryInfoID;
                _doc.DocDate = Base.Quote.Current.DocumentDate;
                _doc.IsPackageValid = false;
                _doc.IsManualPackage = false;
                CarrierPlugin plugin = CarrierPlugin.PK.Find(Base, carrier.CarrierPluginID);
                ICarrierService cs = CarrierPluginMaint.CreateCarrierService(Base, plugin);
                cs.Method = carrier.PluginMethod;

                var graph = PXGraph.CreateInstance<SOOrderEntry>();
                _doc = graph.Document.Insert(_doc);
                graph.Shipping_Address.Cache.Current = new SOShippingAddress();
                CarrierRatesExt(graph).RecalculatePackagesForOrder(graph.Document.Current);
                CarrierRequest cr = CarrierRatesExt(graph).BuildRateRequest(_doc);
                CarrierResult<RateQuote> result = cs.GetRateQuote(cr);

                if (result != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Message message in result.Messages)
                    {
                        sb.AppendFormat("{0}:{1} ", message.Code, message.Description);
                    }

                    if (result.IsSuccess)
                    {
                        throw new PXException(result.Result.Amount.ToString());
                        //decimal baseCost = ConvertAmtToBaseCury(result.Result.Currency, arsetup.Current.DefaultRateTypeID, Document.Current.OrderDate.Value, result.Result.Amount);
                        //SetFreightCost(baseCost);
                    }
                }
            }
        }

    }

}
