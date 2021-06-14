using System;
using PX.Data;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("LUM3PLUKSetup")]
    public class LUM3PLUKSetup : IBqlTable
    {
        #region FtpHost
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Host")]
        public virtual string FtpHost { get; set; }
        public abstract class ftpHost : PX.Data.BQL.BqlString.Field<ftpHost> { }
        #endregion

        #region FtpUser
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "User")]
        public virtual string FtpUser { get; set; }
        public abstract class ftpUser : PX.Data.BQL.BqlString.Field<ftpUser> { }
        #endregion

        #region FtpPass
        [PXRSACryptString(IsUnicode = true)]
        [PXUIField(DisplayName = "Password")]
        public virtual string FtpPass { get; set; }
        public abstract class ftpPass : PX.Data.BQL.BqlString.Field<ftpPass> { }
        #endregion

        #region FtpPath
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Path")]
        public virtual string FtpPath { get; set; }
        public abstract class ftpPath : PX.Data.BQL.BqlString.Field<ftpPath> { }
        #endregion

        #region FtpPort
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Port")]
        public virtual string FtpPort { get; set; }
        public abstract class ftpPort : PX.Data.BQL.BqlString.Field<ftpPort> { }
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