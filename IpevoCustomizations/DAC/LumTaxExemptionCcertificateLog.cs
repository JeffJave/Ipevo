using System;
using PX.Data;

namespace IpevoCustomizations.DAC
{
    [Serializable]
    [PXCacheName("LumTaxExemptionCcertificateLog")]
    public class LumTaxExemptionCcertificateLog : IBqlTable
    {
        #region FilePath
        [PXDBString(100, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "File Path")]
        public virtual string FilePath { get; set; }
        public abstract class filePath : PX.Data.BQL.BqlString.Field<filePath> { }
        #endregion

        #region CustomerID
        [PXDBString(100, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Customer ID")]
        public virtual string CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlString.Field<customerID> { }
        #endregion

        #region IsProcess
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Process")]
        public virtual bool? IsProcess { get; set; }
        public abstract class isProcess : PX.Data.BQL.BqlBool.Field<isProcess> { }
        #endregion

        #region ErrorMsg
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error Msg")]
        public virtual string ErrorMsg { get; set; }
        public abstract class errorMsg : PX.Data.BQL.BqlString.Field<errorMsg> { }
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