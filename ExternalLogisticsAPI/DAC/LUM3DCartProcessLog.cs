using System;
using PX.Data;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("3DCart Process Log")]
    public class LUM3DCartProcessLog : IBqlTable
    {
        #region LineNumber
        [PXDBIdentity(IsKey = true)]
        public virtual int? LineNumber { get; set; }
        public abstract class lineNumber : PX.Data.BQL.BqlInt.Field<lineNumber> { }
        #endregion

        #region ProcessID
        [PXDBInt()]
        [PXUIField(DisplayName = "Process ID")]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : PX.Data.BQL.BqlInt.Field<processID> { }
        #endregion

        #region OrderID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order ID")]
        public virtual string OrderID { get; set; }
        public abstract class orderID : PX.Data.BQL.BqlString.Field<orderID> { }
        #endregion

        #region AcumaticaOrderID
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Order ID")]
        public virtual string AcumaticaOrderID { get; set; }
        public abstract class acumaticaOrderID : PX.Data.BQL.BqlString.Field<acumaticaOrderID> { }
        #endregion

        #region AcumaticaOrderType
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Acumatica Order Type")]
        public virtual string AcumaticaOrderType { get; set; }
        public abstract class acumaticaOrderType : PX.Data.BQL.BqlString.Field<acumaticaOrderType> { }
        #endregion

        #region ImportStatus
        [PXDBBool()]
        [PXUIField(DisplayName = "Import Status")]
        public virtual bool? ImportStatus { get; set; }
        public abstract class importStatus : PX.Data.BQL.BqlBool.Field<importStatus> { }
        #endregion

        #region ErrorDesc
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error Desc")]
        public virtual string ErrorDesc { get; set; }
        public abstract class errorDesc : PX.Data.BQL.BqlString.Field<errorDesc> { }
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
    }
}