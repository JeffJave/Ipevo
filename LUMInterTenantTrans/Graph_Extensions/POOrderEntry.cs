using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUMInterTenantTrans;
using PX.Common;
using PX.Data;
using PX.Objects.SO;

namespace PX.Objects.PO
{
    public class POOrderEntry_Extension : PXGraphExtension<POOrderEntry>
    {
        #region Event Handlers
        [Obsolete]
        protected void _(Events.RowDeleting<POOrder> e)
        {
            var row = e.Row as POOrder;
            if (row == null) return;

            var curPOOrder = (POOrder)Base.Caches[typeof(POOrder)].Current;
            POOrderExt pOOrderExt = curPOOrder.GetExtension<POOrderExt>();

            if ((bool)pOOrderExt.UsrICSOCreated)
            {
                var curUserName = PXLogin.ExtractUsername(PXContext.PXIdentity.IdentityName);
                var curLMICVendor = PXSelect<LMICVendor, Where<LMICVendor.vendorid, Equal<Required<LMICVendor.vendorid>>>>.Select(Base, curPOOrder.VendorID).TopFirst;

                using (PXLoginScope pXLoginScope = new PXLoginScope($"{curUserName}@{curLMICVendor.LoginName}"))
                {
                    SOOrderEntry sOOrderEntry = PXGraph.CreateInstance<SOOrderEntry>();

                    // check the shipment base on this SO is created or not
                    var isShipmentCreated = PXSelect<SOOrderShipment, Where<SOOrderShipment.orderNbr, Equal<Required<SOOrderShipment.orderNbr>>>>.Select(sOOrderEntry, row.VendorRefNbr).Count() > 0 ? true : false;
                    if (isShipmentCreated) throw new PXException("You cannot delete the PO because there is shipment created in IC SO.");
                }
            }
        }

        [Obsolete]
        protected void _(Events.RowDeleted<POOrder> e)
        {
            var row = e.Row as POOrder;
            if (row == null) return;
            
            var curPOOrder = (POOrder)Base.Caches[typeof(POOrder)].Current;
            POOrderExt pOOrderExt = curPOOrder.GetExtension<POOrderExt>();

            if ((bool)pOOrderExt.UsrICSOCreated)
            {
                var curPOLines = (POLine)Base.Caches[typeof(POLine)].Current;
                var curUserName = PXLogin.ExtractUsername(PXContext.PXIdentity.IdentityName);
                var curLMICVendor = PXSelect<LMICVendor, Where<LMICVendor.vendorid, Equal<Required<LMICVendor.vendorid>>>>.Select(Base, curPOOrder.VendorID)?.TopFirst;

                if (curLMICVendor != null)
                {
                    using (PXLoginScope pXLoginScope = new PXLoginScope($"{curUserName}@{curLMICVendor.LoginName}"))
                    {
                        // set SOorder.ICPOCreated = False, SOOrder.Customerordernbr = Blank
                        SOOrderEntry sOOrderEntry = PXGraph.CreateInstance<SOOrderEntry>();
                        SOOrder sOOrder = PXSelect<SOOrder, Where<SOOrder.orderNbr, Equal<Required<SOOrder.orderNbr>>>>.Select(sOOrderEntry, row.VendorRefNbr)?.TopFirst;
                        sOOrder.CustomerOrderNbr = "IC PO had been deleted";
                        SOOrderExt sOOrderExt = sOOrder.GetExtension<SOOrderExt>();
                        sOOrderExt.UsrICPOCreated = false;
                        sOOrder = sOOrderEntry.Document.Update(sOOrder);
                        sOOrderEntry.Actions.PressSave();
                    }
                    Base.Document.Delete(curPOOrder);
                    Base.Transactions.Delete(curPOLines);
                    Base.Actions.PressSave();
                }
            }
        }
        #endregion
    }
}
