using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("LUMLobAPISetup")]
    public class LUMLobAPISetup : IBqlTable
    {
        #region IsProd
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Is Prod")]
        [PXStringList(new string[] {"T", "P"}, new string[] {"Test Env.", "Prod Env."})]
        [PXDefault()]
        public virtual string IsProd { get; set; }
        public abstract class isProd : PX.Data.BQL.BqlString.Field<isProd> { }
        #endregion

        #region Lobapiurl
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Lob API URL")]
        [PXDefault()]
        public virtual string Lobapiurl { get; set; }
        public abstract class lobapiurl : PX.Data.BQL.BqlString.Field<lobapiurl> { }
        #endregion

        #region AuthCode_Test
        [PXDBString(512, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Auth Code - Test")]
        [PXDefault()]
        public virtual string AuthCode_Test { get; set; }
        public abstract class authCode_Test : PX.Data.BQL.BqlString.Field<authCode_Test> { }
        #endregion

        #region AuthCode_Prod
        [PXDBString(512, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Auth Code - Prod")]
        [PXDefault()]
        public virtual string AuthCode_Prod { get; set; }
        public abstract class authCode_Prod : PX.Data.BQL.BqlString.Field<authCode_Prod> { }
        #endregion

        #region ShipFrom_Name
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipping Name")]
        public virtual string ShipFrom_Name { get; set; }
        public abstract class shipFrom_Name : PX.Data.BQL.BqlString.Field<shipFrom_Name> { }
        #endregion

        #region ShipFrom_Address
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipping Address")]
        public virtual string ShipFrom_Address { get; set; }
        public abstract class shipFrom_Address : PX.Data.BQL.BqlString.Field<shipFrom_Address> { }
        #endregion

        #region ShipFrom_City
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipping City")]
        public virtual string ShipFrom_City { get; set; }
        public abstract class shipFrom_City : PX.Data.BQL.BqlString.Field<shipFrom_City> { }
        #endregion

        #region ShipFrom_State
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipping State")]
        public virtual string ShipFrom_State { get; set; }
        public abstract class shipFrom_State : PX.Data.BQL.BqlString.Field<shipFrom_State> { }
        #endregion

        #region ShipFrom_Country
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipping Country")]
        public virtual string ShipFrom_Country { get; set; }
        public abstract class shipFrom_Country : PX.Data.BQL.BqlString.Field<shipFrom_Country> { }
        #endregion

        #region ShipFrom_Zip
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipping Zip")]
        public virtual string ShipFrom_Zip { get; set; }
        public abstract class shipFrom_Zip : PX.Data.BQL.BqlString.Field<shipFrom_Zip> { }
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
    }
}
