using PX.Data;
using IpevoCustomizations.DAC;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extensions : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        protected void _(Events.FieldUpdated<SOOrder.shipVia> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            if (e.Row != null && CheckNonTaxableState(Base, Base.Shipping_Address.Current.State) == false )
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
            if (string.IsNullOrEmpty(stateID))
            {
                return false;
            }
            else
            {
                return LUMFreightNonTaxState.PK.Find(graph, stateID) == null;
            }
        }
        #endregion
    }
}
