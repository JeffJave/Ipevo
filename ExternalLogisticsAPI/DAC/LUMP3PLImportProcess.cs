using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("LUMP3PLImportProcess")]
    public class LUMP3PLImportProcess : IBqlTable
    {
        #region Selected
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region WarehouseOrder
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Warehouse Order")]
        public virtual string WarehouseOrder { get; set; }
        public abstract class warehouseOrder : PX.Data.BQL.BqlString.Field<warehouseOrder> { }
        #endregion

        #region CustomerOrderRef
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Customer Order Ref")]
        public virtual string CustomerOrderRef { get; set; }
        public abstract class customerOrderRef : PX.Data.BQL.BqlString.Field<customerOrderRef> { }
        #endregion

        #region OrderStatus
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Status")]
        public virtual string OrderStatus { get; set; }
        public abstract class orderStatus : PX.Data.BQL.BqlString.Field<orderStatus> { }
        #endregion

        #region UnitsSent
        [PXDBInt()]
        [PXUIField(DisplayName = "Units Sent")]
        public virtual int? UnitsSent { get; set; }
        public abstract class unitsSent : PX.Data.BQL.BqlInt.Field<unitsSent> { }
        #endregion

        #region Carrier
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Carrier")]
        public virtual string Carrier { get; set; }
        public abstract class carrier : PX.Data.BQL.BqlString.Field<carrier> { }
        #endregion

        #region TrackingNumber
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tracking Number")]
        public virtual string TrackingNumber { get; set; }
        public abstract class trackingNumber : PX.Data.BQL.BqlString.Field<trackingNumber> { }
        #endregion

        #region FreightCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Freight Cost")]
        public virtual Decimal? FreightCost { get; set; }
        public abstract class freightCost : PX.Data.BQL.BqlDecimal.Field<freightCost> { }
        #endregion

        #region FreightCurrency
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Freight Currency")]
        public virtual string FreightCurrency { get; set; }
        public abstract class freightCurrency : PX.Data.BQL.BqlString.Field<freightCurrency> { }
        #endregion

        #region FtpFileName
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "FTP File Name")]
        public virtual string FtpFileName { get; set; }
        public abstract class ftpFileName : PX.Data.BQL.BqlString.Field<ftpFileName> { }
        #endregion

        #region Noteid
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
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}
