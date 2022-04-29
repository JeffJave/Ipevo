using System;
using PX.Data;
using PX.Objects.GL;

namespace LumSplitVarianceCost.DAC
{
    [Serializable]
    [PXCacheName("LumSTDCostVarSplit")]
    public class LumSTDCostVarSplit : IBqlTable
    {
        #region LineNbr
        [PXDBIdentity(IsKey = true)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Fin Period")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID")]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account")]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region GLTranType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "GL TranType")]
        public virtual string GLTranType { get; set; }
        public abstract class gLTranType : PX.Data.BQL.BqlString.Field<gLTranType> { }
        #endregion

        #region AccountDescription
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Description")]
        public virtual string AccountDescription { get; set; }
        public abstract class accountDescription : PX.Data.BQL.BqlString.Field<accountDescription> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region InventoryDescr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Description")]
        public virtual string InventoryDescr { get; set; }
        public abstract class inventoryDescr : PX.Data.BQL.BqlString.Field<inventoryDescr> { }
        #endregion

        #region KitInventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Kit Inventory ID")]
        public virtual int? KitInventoryID { get; set; }
        public abstract class kitInventoryID : PX.Data.BQL.BqlInt.Field<kitInventoryID> { }
        #endregion

        #region KitInventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Kit Inventory")]
        public virtual string KitInventoryCD { get; set; }
        public abstract class kitInventoryCD : PX.Data.BQL.BqlString.Field<kitInventoryCD> { }
        #endregion

        #region KitInventoryDescr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Kit Inventory Description")]
        public virtual string KitInventoryDescr { get; set; }
        public abstract class kitInventoryDescr : PX.Data.BQL.BqlString.Field<kitInventoryDescr> { }
        #endregion

        #region SubID
        //[PXDBInt()]
        [SubAccount(typeof(accountID))]
        [PXUIField(DisplayName = "SubAccount")]
        public virtual int? SubID { get; set; }
        public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
        #endregion

        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region SplitQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Split Qty")]
        public virtual Decimal? SplitQty { get; set; }
        public abstract class splitQty : PX.Data.BQL.BqlDecimal.Field<splitQty> { }
        #endregion

        #region Split
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Split(%)")]
        public virtual Decimal? Split { get; set; }
        public abstract class split : PX.Data.BQL.BqlDecimal.Field<split> { }
        #endregion

        #region VarianceCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Variance Cost")]
        public virtual Decimal? VarianceCost { get; set; }
        public abstract class varianceCost : PX.Data.BQL.BqlDecimal.Field<varianceCost> { }
        #endregion

        #region SplitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Split Cost")]
        public virtual Decimal? SplitCost { get; set; }
        public abstract class splitCost : PX.Data.BQL.BqlDecimal.Field<splitCost> { }
        #endregion

        #region STDCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "STD Cost")]
        public virtual Decimal? STDCost { get; set; }
        public abstract class sTDCost : PX.Data.BQL.BqlDecimal.Field<sTDCost> { }
        #endregion

        #region INCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "IN Cost")]
        public virtual Decimal? INCost { get; set; }
        public abstract class iNCost : PX.Data.BQL.BqlDecimal.Field<iNCost> { }
        #endregion

        #region NewSTDCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "New STD Cost")]
        public virtual Decimal? NewSTDCost { get; set; }
        public abstract class newSTDCost : PX.Data.BQL.BqlDecimal.Field<newSTDCost> { }
        #endregion
    }

}
