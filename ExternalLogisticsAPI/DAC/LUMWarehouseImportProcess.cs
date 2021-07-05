using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("LUMWarehouseImportProcess")]
    public class LUMWarehouseImportProcess : IBqlTable
    {

        #region Selected
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region Erporder
        [PXDBString(100, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ERPORDER")]
        public virtual string Erporder { get; set; }
        public abstract class erporder : PX.Data.BQL.BqlString.Field<erporder> { }
        #endregion

        #region ShipmentID
        [PXDBString(100, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "SHIPMENTID")]
        public virtual string ShipmentID { get; set; }
        public abstract class shipmentID : PX.Data.BQL.BqlString.Field<shipmentID> { }
        #endregion

        #region Pronumalpha
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "TRACKING Nbr")]
        public virtual string TrackingNbr { get; set; }
        public abstract class trackingNbr : PX.Data.BQL.BqlString.Field<trackingNbr> { }
        #endregion

        #region Carrier
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CARRIER")]
        public virtual string Carrier { get; set; }
        public abstract class carrier : PX.Data.BQL.BqlString.Field<carrier> { }
        #endregion

        #region ShipmentDae
        [PXDBDate]
        [PXUIField(DisplayName = "Shipment Date")]
        public virtual DateTime? ShipmentDate { get; set; }
        public abstract class shipmentDate : PX.Data.BQL.BqlDateTime.Field<shipmentDate> { }
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
