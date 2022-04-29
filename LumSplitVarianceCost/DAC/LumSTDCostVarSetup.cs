using System;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.GL;
using PX.Objects.IN;

namespace LumSplitVarianceCost.DAC
{
    [Serializable]
    [PXCacheName("LumSTDCostVarSetup")]
    public class LumSTDCostVarSetup : IBqlTable
    {
        #region VarAcctID
        [Account(DisplayName = "Variance Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
        public virtual int? VarAcctID { get; set; }
        public abstract class varAcctID : PX.Data.BQL.BqlInt.Field<varAcctID> { }
        #endregion

        #region InvtAcctID
        [Account(DisplayName = "Inventory Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
        //[PXDefault(typeof(SelectFromBase<INPostClass, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<INPostClass.postClassID, IBqlString>.IsEqual<BqlField<InventoryItem.postClassID, IBqlString>.FromCurrent>>), SourceField = typeof(INPostClass.invtAcctID), CacheGlobal = true, PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXForeignReference(typeof(InventoryItem.FK.InventoryAccount))]
        //[PXDBInt()]
        //[PXDefault(typeof(Account.accountID))]
        //[PXUIField(DisplayName = "Account", Enabled = true)]
        //[PXSelector(typeof(Search<Account.accountID>),
        //    typeof(Account.accountID),
        //    typeof(Account.description),
        //    SubstituteKey = typeof(Account.accountCD),
        //    DescriptionField = typeof(Account.description))]
        public virtual int? InvtAcctID { get; set; }
        public abstract class invtAcctID : PX.Data.BQL.BqlInt.Field<invtAcctID> { }
        #endregion

        #region InvtSubID
        [SubAccount(typeof(InventoryItem.invtAcctID), DisplayName = "Sub-Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        //[PXDefault(typeof(SelectFromBase<INPostClass, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<INPostClass.postClassID, IBqlString>.IsEqual<BqlField<InventoryItem.postClassID, IBqlString>.FromCurrent>>), SourceField = typeof(INPostClass.invtSubID), CacheGlobal = true, PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXForeignReference(typeof(InventoryItem.FK.InventorySubaccount))]
        public virtual int? InvtSubID { get; set; }
        public abstract class invtSubID : PX.Data.BQL.BqlInt.Field<invtSubID> { }
        #endregion

        #region InvtSplitSubID
        [SubAccount(typeof(InventoryItem.invtAcctID), DisplayName = "Split Sub-account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
        //[PXDefault(typeof(SelectFromBase<INPostClass, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlOperand<INPostClass.postClassID, IBqlString>.IsEqual<BqlField<InventoryItem.postClassID, IBqlString>.FromCurrent>>), SourceField = typeof(INPostClass.invtSubID), CacheGlobal = true, PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXForeignReference(typeof(InventoryItem.FK.InventorySubaccount))]
        public virtual int? InvtSplitSubID { get; set; }
        public abstract class invtSplitSubID : PX.Data.BQL.BqlInt.Field<invtSplitSubID> { }
        #endregion

        #region EnableCreateAdjmOnLastDayInLastMonth
        [PXDBBool()]
        [PXUIField(DisplayName = "Enable Create Adjustment On Last Day In Last Month")]
        [PXDefault(false)]
        public virtual bool? EnableCreateAdjmOnLastDayInLastMonth { get; set; }
        public abstract class enableCreateAdjmOnLastDayInLastMonth : PX.Data.BQL.BqlBool.Field<enableCreateAdjmOnLastDayInLastMonth> { }
        #endregion

        #region CreateAdjmTranDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Create Adjm Tran Date")]
        public virtual DateTime? CreateAdjmTranDate { get; set; }
        public abstract class createAdjmTranDate : PX.Data.BQL.BqlDateTime.Field<createAdjmTranDate> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
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

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}