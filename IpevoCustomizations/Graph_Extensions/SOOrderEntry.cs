using PX.Data;
using IpevoCustomizations.DAC;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extensions : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
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
