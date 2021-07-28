using System;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("vPACAdjCostIssue")]
    public class vPACAdjCostIssue : IBqlTable
    {
        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region ItemClassID
        [PXDBInt()]
        [PXUIField(DisplayName = "Item Class ID")]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
        #endregion

        #region FinPtdCostIssued
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Fin Ptd Cost Issued")]
        public virtual Decimal? FinPtdCostIssued { get; set; }
        public abstract class finPtdCostIssued : PX.Data.BQL.BqlDecimal.Field<finPtdCostIssued> { }
        #endregion

        #region FinPtdQtyIssued
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Fin Ptd Qty Issued")]
        public virtual Decimal? FinPtdQtyIssued { get; set; }
        public abstract class finPtdQtyIssued : PX.Data.BQL.BqlDecimal.Field<finPtdQtyIssued> { }
        #endregion

        #region PACUnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PACUnit Cost")]
        public virtual Decimal? PACUnitCost { get; set; }
        public abstract class pACUnitCost : PX.Data.BQL.BqlDecimal.Field<pACUnitCost> { }
        #endregion

        #region InventoryID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region PACIssueCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PACIssue Cost")]
        public virtual Decimal? PACIssueCost { get; set; }
        public abstract class pACIssueCost : PX.Data.BQL.BqlDecimal.Field<pACIssueCost> { }
        #endregion

        #region Siteid
        [PXDBInt()]
        [PXUIField(DisplayName = "Siteid")]
        public virtual int? Siteid { get; set; }
        public abstract class siteid : PX.Data.BQL.BqlInt.Field<siteid> { }
        #endregion

        #region AssemblyAdjAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Issue Adj Amount")]
        public virtual Decimal? IssueAdjAmount { get; set; }
        public abstract class issueAdjAmount : PX.Data.BQL.BqlDecimal.Field<issueAdjAmount> { }
        #endregion
    }
}