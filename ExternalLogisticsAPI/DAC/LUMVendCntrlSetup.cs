using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.SO;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("Vendor Central Setup")]
    public class LUMVendCntrlSetup : IBqlTable
    {
        #region SecureURL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Secure URL")]
        public virtual string SecureURL { get; set; }
        public abstract class secureURL : PX.Data.BQL.BqlString.Field<secureURL> { }
        #endregion

        #region ClientID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Client ID")]
        public virtual string ClientID { get; set; }
        public abstract class clientID : PX.Data.BQL.BqlString.Field<clientID> { }
        #endregion

        #region ClientSecret
        [PXRSACryptString(IsUnicode = true)]
        [PXUIField(DisplayName = "Client Secret")]
        public virtual string ClientSecret { get; set; }
        public abstract class clientSecret : PX.Data.BQL.BqlString.Field<clientSecret> { }
        #endregion

        #region AuthToken
        [PXRSACryptString(IsUnicode = true)]
        [PXUIField(DisplayName = "Auth Token")]
        public virtual string AuthToken { get; set; }
        public abstract class authToken : PX.Data.BQL.BqlString.Field<authToken> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, IsUnicode = true, InputMask = ">aa")]
        [PXUIField(DisplayName = "Order Type")]
        [PXSelector(typeof(Search<SOOrderType.orderType>), CacheGlobal = true)]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region CustomerID
        [Customer()]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
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