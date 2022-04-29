using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;

namespace LumSplitVarianceCost.DAC
{
    [Serializable]
    [PXCacheName("LumCostSplit")]
    public class LumCostSplit : IBqlTable
    {
        #region LineNbr
        [PXDBIdentity(IsKey = true)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region UsrSplited
        [PXDBBool()]
        [PXUIField(DisplayName = "Splited", Enabled = true)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSplited { get; set; }
        public abstract class usrSplited : PX.Data.BQL.BqlBool.Field<usrSplited> { }
        #endregion

        #region Module
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Module")]
        public virtual string Module { get; set; }
        public abstract class module : PX.Data.BQL.BqlString.Field<module> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr")]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [BatchStatus.List()]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region Curyid
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cury ID")]
        public virtual string Curyid { get; set; }
        public abstract class curyid : PX.Data.BQL.BqlString.Field<curyid> { }
        #endregion

        #region Description
        [PXDBString(512, IsUnicode = true)]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID", Visible = false)]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account")]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region AccountDescription
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Description")]
        public virtual string AccountDescription { get; set; }
        public abstract class accountDescription : PX.Data.BQL.BqlString.Field<accountDescription> { }
        #endregion

        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region DebitAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Debit Amt")]
        public virtual Decimal? DebitAmt { get; set; }
        public abstract class debitAmt : PX.Data.BQL.BqlDecimal.Field<debitAmt> { }
        #endregion

        #region CreditAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Credit Amt")]
        public virtual Decimal? CreditAmt { get; set; }
        public abstract class creditAmt : PX.Data.BQL.BqlDecimal.Field<creditAmt> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID", Visible = false)]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region InventoryDescr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Descr")]
        public virtual string InventoryDescr { get; set; }
        public abstract class inventoryDescr : PX.Data.BQL.BqlString.Field<inventoryDescr> { }
        #endregion

        #region Released
        [PXDBBool()]
        [PXUIField(DisplayName = "Released")]
        public virtual bool? Released { get; set; }
        public abstract class released : PX.Data.BQL.BqlBool.Field<released> { }
        #endregion

        #region Tranid
        [PXDBInt()]
        [PXUIField(DisplayName = "Tranid")]
        public virtual int? Tranid { get; set; }
        public abstract class tranid : PX.Data.BQL.BqlInt.Field<tranid> { }
        #endregion

        #region TranType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Tran Type")]
        public virtual string TranType { get; set; }
        public abstract class tranType : PX.Data.BQL.BqlString.Field<tranType> { }
        #endregion

        #region StdCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Std Cost")]
        public virtual Decimal? StdCost { get; set; }
        public abstract class stdCost : PX.Data.BQL.BqlDecimal.Field<stdCost> { }
        #endregion

        #region StdCostDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Std Cost Date")]
        public virtual DateTime? StdCostDate { get; set; }
        public abstract class stdCostDate : PX.Data.BQL.BqlDateTime.Field<stdCostDate> { }
        #endregion

        #region UnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Unit Cost")]
        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region CostDiffer
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Differ")]
        public virtual string CostDiffer { get; set; }
        public abstract class costDiffer : PX.Data.BQL.BqlString.Field<costDiffer> { }
        #endregion

        #region STDCostVariance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "STDCost Variance")]
        public virtual Decimal? STDCostVariance { get; set; }
        public abstract class sTDCostVariance : PX.Data.BQL.BqlDecimal.Field<sTDCostVariance> { }
        #endregion

        #region Subid
        [PXDBInt()]
        [PXUIField(DisplayName = "Subid")]
        public virtual int? Subid { get; set; }
        public abstract class subid : PX.Data.BQL.BqlInt.Field<subid> { }
        #endregion
    }
}