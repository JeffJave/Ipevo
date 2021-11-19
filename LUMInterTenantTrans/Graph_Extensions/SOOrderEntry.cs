using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUMInterTenantTrans;
using PX.Common;
using PX.Data;
using PX.Data.Update;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PO;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extension : PXGraphExtension<SOOrderEntry>
    {
        public override void Initialize()
        {
            base.Initialize();

            CreateICPOAction.SetEnabled(false);
            Base.action.AddMenuAction(CreateICPOAction);
        }

        #region Material Issues Action
        public PXAction<SOOrder> CreateICPOAction;
        [PXButton]
        [PXUIField(DisplayName = "Create IC PO")]
        [Obsolete]
        protected void createICPOAction()
        {
            Base.Save.Press();

            var curCompanyID = PXInstanceHelper.CurrentCompany;
            var curUserName = PXLogin.ExtractUsername(PXContext.PXIdentity.IdentityName);

            //A.By using LMICCustomer table to identify the destination tenant of PO need to be created
            var curSOOrder = (SOOrder)Base.Caches[typeof(SOOrder)].Current;
            var curLMICCustomer = PXSelect<LMICCustomer, Where<LMICCustomer.customerID, Equal<Required<LMICCustomer.customerID>>>>.Select(Base, curSOOrder.CustomerID).TopFirst;
            
            //B.Go to the destination tenant, and ensure vendor is defined in LMICVendor.If not, throw error message “The vendor is not defined in destination tenant”.
            var curCompanyName = PXLogin.ExtractCompany(PXContext.PXIdentity.IdentityName);
            var curLMICVendor = new LMICVendor();
            try
            {
                using (PXLoginScope pXLoginScope = new PXLoginScope($"{curUserName}@{curLMICCustomer.LoginName}"))
                {
                    curLMICVendor = PXSelect<LMICVendor, Where<LMICVendor.tenantID, Equal<Required<LMICVendor.tenantID>>>>.Select(PXGraph.CreateInstance<POOrderEntry>(), curCompanyID).TopFirst;
                }
            }
            catch (Exception)
            {
                throw new PXException("Please try again.");
            }

            /*Get InventoryCD in each line*/
            var inventoryCDList = new List<string>();
            foreach (SOLine line in Base.Transactions.Cache.Cached)
            {
                inventoryCDList.Add(PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(Base, line.InventoryID).TopFirst?.InventoryCD);
            }

            if (curLMICCustomer != null)
            {
                if (curLMICVendor != null)
                {
                    try
                    {
                        PXLongOperation.StartOperation(Base, () =>
                        {
                            var SOOrderCustomerOrderNbr = string.Empty;
                            

                            //C.Create a purchase order in destination tenant by uing the following mapping.
                            using (PXLoginScope pXLoginScope = new PXLoginScope($"{curUserName}@{curLMICCustomer.LoginName}"))
                            {
                                POOrderEntry pOOrderEntry = PXGraph.CreateInstance<POOrderEntry>();
                                /* PO Header */
                                POOrder pOOrder = new POOrder();
                                pOOrder = pOOrderEntry.Document.Insert(pOOrder);

                                pOOrder.BranchID = curLMICVendor.BranchID;
                                pOOrder.VendorID = curLMICVendor.VendorID;

                                pOOrderEntry.Document.Cache.RaiseFieldUpdated<POOrder.vendorID>(pOOrder, pOOrder.VendorID);

                                pOOrder.VendorRefNbr = curSOOrder.OrderNbr;
                                pOOrder.OrderDate = curSOOrder.OrderDate;
                                pOOrder.OrderDesc = "IC PO | " + curCompanyName + " | " + curSOOrder.OrderNbr;

                                /* PO Line */
                                int i = 0;
                                foreach (SOLine line in Base.Transactions.Cache.Cached)
                                {
                                    POLine pOLine = new POLine();
                                    var tempInventoryID = PXSelect<InventoryItem, Where<InventoryItem.inventoryCD, Equal<Required<InventoryItem.inventoryCD>>>>.Select(pOOrderEntry, inventoryCDList[i])?.TopFirst?.InventoryID;
                                    pOLine.InventoryID =
                                        tempInventoryID == null ?
                                        PXSelect<INItemXRef, Where<INItemXRef.alternateID, Equal<Required<INItemXRef.alternateID>>>>.Select(pOOrderEntry, inventoryCDList[i])?.TopFirst?.InventoryID : tempInventoryID;
                                    pOLine.OrderQty = line.OrderQty;
                                    pOLine = pOOrderEntry.Transactions.Insert(pOLine);
                                    
                                    pOLine.CuryUnitCost = line.CuryUnitPrice;
                                    pOLine = pOOrderEntry.Transactions.Update(pOLine);

                                    i++;
                                }

                                //D.If the PO is saved successfully, please update the following fields
                                // - POOrder, ICSOCreated = true
                                POOrderExt pOOrderExt = pOOrder.GetExtension<POOrderExt>();
                                pOOrderExt.UsrICSOCreated = true;
                                pOOrderEntry.Document.Update(pOOrder);
                                pOOrderEntry.Actions.PressSave();

                                SOOrderCustomerOrderNbr = pOOrder.OrderNbr;
                            }

                            //D.If the PO is saved successfully, please update the following fields
                            // - SOOrder, ICPOCreated = true; Customer Order Nbr = POOrder.OrderNbr
                            SOOrderExt sOOrderExt = curSOOrder.GetExtension<SOOrderExt>();
                            sOOrderExt.UsrICPOCreated = true;
                            curSOOrder.CustomerOrderNbr = SOOrderCustomerOrderNbr;
                            Base.Document.Update(curSOOrder);
                            Base.Save.Press();
                        });
                    }
                    catch (Exception)
                    {
                        throw new PXException("Please try again.");
                    }
                }
                else
                {
                    throw new PXException("The vendor is not defined in destination tenant");
                }
            }
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<SOOrder> e)
        {
            var row = e.Row as SOOrder;
            
            if (row == null) { return; }

            var customerIsDefined = PXSelect<LMICCustomer, Where<LMICCustomer.customerID, Equal<Required<LMICCustomer.customerID>>>>.Select(Base, row.CustomerID).TopFirst;

            if (row.Status == "N" && row.OrderQty > 0 && customerIsDefined != null) CreateICPOAction.SetEnabled(true);
            else CreateICPOAction.SetEnabled(false);

            // check the ICPOCreated checkbox
            if (row.GetExtension<SOOrderExt>()?.UsrICPOCreated == true)
            {
                Base.Document.AllowDelete = false;
                CreateICPOAction.SetEnabled(false);
            }
            else
            {
                Base.Document.AllowDelete = true;
                CreateICPOAction.SetEnabled(true);
            }
        }

        protected void _(Events.RowDeleting<SOOrder> e)
        {
            var row = e.Row as SOOrder;
            if (row == null) return;

            // check the ICPOCreated checkbox
            if (row.GetExtension<SOOrderExt>()?.UsrICPOCreated == true) throw new PXException("You cannot delete this PO because ICPOCreated is clicked.");
        }
        #endregion
    }
}
