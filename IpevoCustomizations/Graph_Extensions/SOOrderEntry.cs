using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using IpevoCustomizations.DAC;
using System.Linq;

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
                line.OrderQty    = 1;
                line.UnitPrice   = 0;

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

        protected void _(Events.FieldUpdated<SOLine.inventoryID> e, PXFieldUpdated baeHandler)
        {
            baeHandler?.Invoke(e.Cache,e.Args);

            var item = SelectFrom<InventoryItem>
                                .Where<InventoryItem.inventoryID.IsEqual<P.AsInt>>
                                .View.Select(Base,e.NewValue)
                                .RowCast<InventoryItem>().FirstOrDefault();

            if(item != null && (item.InventoryCD ?? string.Empty).Trim() == "RESTOCKING")
            {
                Base.Document.Cache.SetValueExt<SOOrder.overrideTaxZone>(Base.Document.Current,true);
                Base.Document.Cache.SetValueExt<SOOrder.taxZoneID>(Base.Document.Current, "TAXEXEMPT");
                Base.Document.Cache.MarkUpdated(Base.Document.Current);
            }

        }
        #endregion

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
