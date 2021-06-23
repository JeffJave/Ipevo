using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL;
using System.Collections;
using System.Collections.Generic;
using IpevoCustomizations.DAC;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extensions : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public override void Initialize()
        {
            base.Initialize();
            PrepaymentRequest.SetVisible(false);

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.CountryID == "US" || curCoutry?.BaseCuryID == "USD")
            {
                PrepaymentRequest.SetVisible(true);
                Base.report.AddMenuAction(PrepaymentRequest);
            }
        }

        #region Action
        public PXAction<SOOrder> PrepaymentRequest;
        [PXButton]
        [PXUIField(DisplayName = "Print Prepayment Request", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable prepaymentRequest(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["OrderNbr"] = Base.Document.Current.OrderNbr;
                parameters["OrderType"] = Base.Document.Current.OrderType;
                throw new PXReportRequiredException(parameters, "LM641015", "Report LM641015");
            }
            return adapter.Get();
        }
        #endregion

        protected void _(Events.FieldUpdated<SOOrder.customerLocationID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            // Becasue SO shipping address complete information comes after updating customer location record.
            if (e.Row != null && CheckNonTaxableState(Base, Base.Shipping_Address.Select().TopFirst?.State) == false)
            {
                e.Cache.SetValue<SOOrder.freightTaxCategoryID>(e.Row, null);
            }
        }

        #region Static Method
        /// <summary>
        /// If SOShippingAddress.State is within the table FreightNonTaxStates, then put blank into Freight Tax Category.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="stateID"></param>
        /// <returns></returns>
        public static bool CheckNonTaxableState(PXGraph graph, string stateID)
        {
            return string.IsNullOrEmpty(stateID) || LUMFreightNonTaxState.PK.Find(graph, stateID) == null;
        }
        #endregion
    }
}
