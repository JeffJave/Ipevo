using System;
using PX.Data;
using PX.Objects.GL;
using ExternalLogisticsAPI.Descripter;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("Amazon Interface API")]
    public class LUMAmazonInterfaceAPI : IBqlTable
    {
        #region Selected
        [PXBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region OrderType
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Order Type")]
        [PXDefault()]
        [AmazonOrderType.List]
        public virtual int? OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlInt.Field<orderType> { }
        #endregion

        #region OrderNbr
        [PXDBString(30, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Order Nbr.")]
        [PXDefault()]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region SequenceNo
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Sequence No", Visible = false)]
        [PXDefault(0)]
        public virtual int? SequenceNo { get; set; }
        public abstract class sequenceNo : PX.Data.BQL.BqlInt.Field<sequenceNo> { }
        #endregion

        #region BranchID
        [Branch()]
        [PXDefault(typeof(AccessInfo.branchID), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region Marketplace
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Marketplace")]
        public virtual string Marketplace { get; set; }
        public abstract class marketplace : PX.Data.BQL.BqlString.Field<marketplace> { }
        #endregion

        #region Data1
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Data 1")]
        public virtual string Data1 { get; set; }
        public abstract class data1 : PX.Data.BQL.BqlString.Field<data1> { }
        #endregion

        #region Data2
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Data 2", Visible = false)]
        public virtual string Data2 { get; set; }
        public abstract class data2 : PX.Data.BQL.BqlString.Field<data2> { }
        #endregion

        #region Write2Acumatica1
        [PXDBBool()]
        [PXUIField(DisplayName = "Write To Acumatica_1")]
        public virtual bool? Write2Acumatica1 { get; set; }
        public abstract class write2Acumatica1 : PX.Data.BQL.BqlBool.Field<write2Acumatica1> { }
        #endregion

        #region Write2Acumatica2
        [PXDBBool()]
        [PXUIField(DisplayName = "Write To Acumatica_2", Visible = false)]
        public virtual bool? Write2Acumatica2 { get; set; }
        public abstract class write2Acumatica2 : PX.Data.BQL.BqlBool.Field<write2Acumatica2> { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
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
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    }
}