using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;

namespace LumSplitVarianceCost.DAC
{
    [Serializable]
    [PXCacheName("v_LumCostSplit")]
    public class v_LumCostSplit : IBqlTable
    {
        #region UsrSplited
        [PXDBBool()]
        [PXUIField(DisplayName = "Splited", Enabled = true)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSplited { get; set; }
        public abstract class usrSplited : PX.Data.BQL.BqlBool.Field<usrSplited> { }
        #endregion

        #region Module
        [PXDBString(2, IsKey = true, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Module", Enabled = false)]
        public virtual string Module { get; set; }
        public abstract class module : PX.Data.BQL.BqlString.Field<module> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr", Enabled = false)]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [BatchStatus.List()]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Fin Period ID", Enabled = false)]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region CuryID
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cury ID", Enabled = false)]
        public virtual string CuryID { get; set; }
        public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
        #endregion

        #region BatchDescription
        [PXDBString(512, IsUnicode = true)]
        [PXUIField(DisplayName = "Batch Description", Enabled = false)]
        public virtual string BatchDescription { get; set; }
        public abstract class batchDescription : PX.Data.BQL.BqlString.Field<batchDescription> { }
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
        [PXUIField(DisplayName = "Account ID", Visible = false, Enabled = false)]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account", Enabled = false)]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlString.Field<accountCD> { }
        #endregion

        #region AccountDescription
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Description", Enabled = false)]
        public virtual string AccountDescription { get; set; }
        public abstract class accountDescription : PX.Data.BQL.BqlString.Field<accountDescription> { }
        #endregion

        #region AccountType
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Type", Enabled = false)]
        [AccountType.List()]
        public virtual string AccountType { get; set; }
        public abstract class accountType : PX.Data.BQL.BqlString.Field<accountType> { }
        #endregion

        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty", Enabled = false)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region DebitAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Debit Amt", Enabled = false)]
        public virtual Decimal? DebitAmt { get; set; }
        public abstract class debitAmt : PX.Data.BQL.BqlDecimal.Field<debitAmt> { }
        #endregion

        #region CreditAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Credit Amt", Enabled = false)]
        public virtual Decimal? CreditAmt { get; set; }
        public abstract class creditAmt : PX.Data.BQL.BqlDecimal.Field<creditAmt> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID", Visible = false, Enabled = false)]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory ID", Enabled = false)]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region INDescr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Descr", Enabled = false)]
        public virtual string INDescr { get; set; }
        public abstract class iNDescr : PX.Data.BQL.BqlString.Field<iNDescr> { }
        #endregion

        #region GLReleased
        [PXDBBool()]
        [PXUIField(DisplayName = "GLTran Released", Enabled = false)]
        public virtual bool? GLReleased { get; set; }
        public abstract class gLRleased : PX.Data.BQL.BqlBool.Field<gLRleased> { }
        #endregion

        #region GLTranID
        [PXDBInt()]
        [PXUIField(DisplayName = "GLTran TranID", Enabled = false)]
        public virtual int? GLTranID { get; set; }
        public abstract class gLTranID : PX.Data.BQL.BqlInt.Field<gLTranID> { }
        #endregion

        #region GLTranType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "GLTran Type", Enabled = false)]
        public virtual string GLTranType { get; set; }
        public abstract class gLTranType : PX.Data.BQL.BqlString.Field<gLTranType> { }
        #endregion

        #region GLTranLineNbr
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "GLTran LineNbr", Enabled = false)]
        public virtual int? GLTranLineNbr { get; set; }
        public abstract class gLTranLineNbr : PX.Data.BQL.BqlInt.Field<gLTranLineNbr> { }
        #endregion

        #region StdCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Std Cost", Enabled = false)]
        public virtual Decimal? StdCost { get; set; }
        public abstract class stdCost : PX.Data.BQL.BqlDecimal.Field<stdCost> { }
        #endregion

        #region StdCostDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Std Cost Date", Enabled = false)]
        public virtual DateTime? StdCostDate { get; set; }
        public abstract class stdCostDate : PX.Data.BQL.BqlDateTime.Field<stdCostDate> { }
        #endregion

        #region UnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Unit Cost", Enabled = false)]
        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region CostDiffer
        [PXDBInt()]
        //[PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Differ", Enabled = false)]
        public virtual int? CostDiffer { get; set; }
        public abstract class costDiffer : PX.Data.BQL.BqlInt.Field<costDiffer> { }
        #endregion

        #region STDCostVariance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "STDCost Variance", Enabled = false)]
        public virtual Decimal? STDCostVariance { get; set; }
        public abstract class sTDCostVariance : PX.Data.BQL.BqlDecimal.Field<sTDCostVariance> { }
        #endregion

        #region SubID
        [PXDBInt()]
        [PXUIField(DisplayName = "SubID", Enabled = false)]
        public virtual int? SubID { get; set; }
        public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
        #endregion
    }
}