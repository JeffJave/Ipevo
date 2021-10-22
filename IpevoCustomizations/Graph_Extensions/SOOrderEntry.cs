using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using IpevoCustomizations.DAC;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extensions : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        public const string ItemClass_PackMtrl = "PACKMTRL";
        public const string ItemAttr_DefBox = "DEFBOX";
        public const string DefBoxHeader = "Multiple Default Box";
        public const string DefBoxInfo = "There Are Multiple Default Box Stock Items, System Will Use the First One.";

        #region Override Methods
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

        public delegate void PersistDelegate();

        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            var doc = Base.Document.Current;
            // Setting SO Tax Rate(KvExt)
            if (doc != null)
            {
                var customerData = Customer.PK.Find(Base, doc.CustomerID);
                var sumTaxableAmt = Base.Taxes.Select().RowCast<SOTaxTran>().FirstOrDefault()?.CuryTaxableAmt;
                var sumTaxAmt = Base.Taxes.Select().RowCast<SOTaxTran>().Sum(x => x?.CuryTaxAmt ?? 0);
                var UsrAPIOrderType = Base.Document.Cache.GetValueExt(Base.Document.Current, "UsrAPIOrderType") as string;
                if (customerData != null && (customerData.AcctCD.Trim().ToUpper() != "SELLERCENTRAL" || string.IsNullOrEmpty(UsrAPIOrderType)))
                    Base.Document.Cache.SetValueExt(doc, "AttributeTAXRATE", (sumTaxableAmt == 0 || sumTaxAmt == 0 || sumTaxableAmt == null) ? 0 : Math.Round((decimal)(sumTaxAmt / sumTaxableAmt), 5));
            }

            var transDatas = Base.Transactions.Select().RowCast<SOLine>();
            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            foreach (SOLine row in transDatas)
            {
                var itemData = InventoryItem.PK.Find(Base, row.InventoryID);
                decimal? rate = 0;
                if (row != null && itemData != null && doc != null)
                {
                    var itemClass = INItemClass.PK.Find(Base, itemData.ItemClassID);

                    #region Verify qty can not be zero
                    if (row.OrderQty == 0)
                        Base.Transactions.Cache.RaiseExceptionHandling<SOLine.orderQty>(row, row.OrderQty,
                            new PXSetPropertyException<SOLine.orderQty>("Item quantity cannot be 0, please check.", PXErrorLevel.Error));
                    #endregion

                    #region Set MSRP Discount Rate
                    var curyInfoData = PX.Objects.CM.CurrencyInfo.PK.Find(Base, doc.CuryInfoID);
                    if (curyInfoData == null)
                    {
                        var baseCuryID = SelectFrom<PX.Data.PXAccess.Organization>.View.Select(Base).RowCast<PX.Data.PXAccess.Organization>().FirstOrDefault().BaseCuryID;
                        var curyRate = SelectFrom<CurrencyRate>
                                       .Where<CurrencyRate.fromCuryID.IsEqual<P.AsString>
                                              .And<CurrencyRate.toCuryID.IsEqual<P.AsString>>>
                                       .View.Select(Base, doc.CuryID, baseCuryID).RowCast<CurrencyRate>().OrderByDescending(x => x.CuryEffDate).FirstOrDefault();
                        rate = curyRate?.CuryRate ?? 1;
                    }
                    else
                        rate = curyInfoData?.CuryRate;
                    var newValue = itemData?.RecPrice == 0 || itemClass.ItemClassCD.ToUpper().Trim() == "NONSTOCK"
                                   ? 0
                                   : (itemData?.RecPrice - row.CuryLineAmt / row.OrderQty * rate) / itemData?.RecPrice;
                    Base.Transactions.SetValueExt<SOLineExtension.usrMSRPDiscountRate>(row, newValue);
                    #endregion

                    #region Set Account & subAccount only for TW Tenant
                    if (curCoutry?.BranchCD.Trim() == "IPEVOTW" && itemClass?.ItemClassCD.ToUpper().Trim() == "NONSTOCK")
                    {
                        Base.Transactions.SetValueExt<SOLine.salesAcctID>(row, itemData.SalesAcctID);
                        Base.Transactions.SetValueExt<SOLine.salesSubID>(row, itemData.SalesSubID);
                    }
                    #endregion
                }
            }

            baseMethod();
        }

        #endregion

        #region Actions
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

        public PXAction<SOOrder> addBoxSOLine;
        [PXLookupButton()]
        [PXUIField(DisplayName = "aDD Box", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable AddBoxSOLine(PXAdapter adapter)
        {
            SOLine line = Base.Transactions.Cache.CreateInstance() as SOLine;

            int? inventoryID = GetPackMatlBox(Base);

            if (inventoryID != null)
            {
                line.InventoryID = inventoryID;
                line.OrderQty = 1;
                line.UnitPrice = 0;

                Base.Transactions.Cache.Insert(line);
            }

            return adapter.Get();
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<SOOrder> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            addBoxSOLine.SetEnabled(Base.Transactions.Cache.AllowInsert);
            addBoxSOLine.SetVisible(Base.Accessinfo.CompanyName.Contains("TW"));
        }

        protected void _(Events.FieldUpdated<SOOrder.customerLocationID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            // Becasue SO shipping address complete information comes after updating customer location record.
            if (e.Row != null && CheckNonTaxableState(Base, Base.Shipping_Address.Select().TopFirst?.State) == false)
            {
                e.Cache.SetValue<SOOrder.freightTaxCategoryID>(e.Row, null);
            }
        }

        protected void _(Events.FieldSelecting<SOOrderExt.usrRemainingCreditLimit> e)
        {
            var row = e.Row as SOOrder;

            if (row != null && row.CustomerID != null && e.ReturnValue == null)
            {
                Customer customer = Customer.PK.Find(Base, row.CustomerID);

                ARBalances remBal = CustomerMaint.GetCustomerBalances<AR.Override.Customer.sharedCreditCustomerID>(Base, customer?.SharedCreditCustomerID);

                e.ReturnValue = customer?.CreditLimit - ((remBal?.CurrentBal ?? 0) + (remBal?.UnreleasedBal ?? 0) + (remBal?.TotalOpenOrders ?? 0) + (remBal?.TotalShipped ?? 0) - (remBal?.TotalPrepayments ?? 0)) + row.UnpaidBalance;
            }
        }

        protected void _(Events.FieldUpdated<SOLine.inventoryID> e, PXFieldUpdated baeHandler)
        {
            baeHandler?.Invoke(e.Cache, e.Args);

            var shipAddrState = Base.Shipping_Address?.Current?.State;
            var item = SelectFrom<InventoryItem>
                                .Where<InventoryItem.inventoryID.IsEqual<P.AsInt>>
                                .View.Select(Base, e.NewValue)
                                .RowCast<InventoryItem>().FirstOrDefault();
            var IsnonTaxable = SelectFrom<LUMRestockNonTaxState>
                            .Where<LUMRestockNonTaxState.stateID.IsEqual<P.AsString>>
                            .View.Select(Base, shipAddrState).RowCast<LUMRestockNonTaxState>().Any();

            if (item != null && (item.InventoryCD ?? string.Empty).Trim() == "RESTOCKING" && IsnonTaxable)
            {
                Base.Document.Cache.SetValueExt<SOOrder.overrideTaxZone>(Base.Document.Current, true);
                Base.Document.Cache.SetValueExt<SOOrder.taxZoneID>(Base.Document.Current, "TAXEXEMPT");
                Base.Document.Cache.MarkUpdated(Base.Document.Current);
            }
        }
        #endregion

        #region Static Methods
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

        /// <summary>
        /// Get InventoryItem when item class = 'PACKMTRL' and attribute 'DEFBOX' = TRUE
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static int? GetPackMatlBox(SOOrderEntry graph)
        {
            var item = SelectFrom<InventoryItem>.InnerJoin<CSAnswers>.On<CSAnswers.refNoteID.IsEqual<InventoryItem.noteID>>
                                                .InnerJoin<INItemClass>.On<INItemClass.itemClassID.IsEqual<InventoryItem.itemClassID>>
                                                .Where<CSAnswers.attributeID.IsEqual<@P.AsString>
                                                       .And<CSAnswers.value.IsEqual<@P.AsString>>
                                                            .And<INItemClass.itemClassCD.StartsWith<@P.AsString>>>.View.Select(graph, ItemAttr_DefBox, Convert.ToInt32(true).ToString(), ItemClass_PackMtrl);

            if (item.Count > 1)
            {
                WebDialogResult wdr = graph.Transactions.Ask(DefBoxHeader, DefBoxInfo, MessageButtons.OKCancel);

                if (wdr == WebDialogResult.Cancel) { return null; }
            }

            return item.TopFirst?.InventoryID;
        }
        #endregion
    }
}
