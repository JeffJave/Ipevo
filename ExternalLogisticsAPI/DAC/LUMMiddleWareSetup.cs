using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("LUMMiddleWareSetup")]
    public class LUMMiddleWareSetup : IBqlTable
    {
        #region SecureURL_login
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Secure URL_login")]
        public virtual string SecureURL_login { get; set; }
        public abstract class secureURL_login : PX.Data.BQL.BqlString.Field<secureURL_login> { }
        #endregion

        #region SecureURL_fbm
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Secure URL_fbm")]
        public virtual string SecureURL_fbm { get; set; }
        public abstract class secureURL_fbm : PX.Data.BQL.BqlString.Field<secureURL_fbm> { }
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

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
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

        #region AuthToken
        [PXString(100)]
        [PXUIField(DisplayName = "Auth Token")]
        public virtual string AuthToken { get; set; }
        public abstract class authToken : PX.Data.BQL.BqlString.Field<authToken> { }
        #endregion

    }
}
