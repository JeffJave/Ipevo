using System;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("vPACReturnPO")]
    public class vPACReturnPO : IBqlTable
    {
        #region POLine_ReceiptNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "POLine_Receipt Nbr")]
        public virtual string POLine_ReceiptNbr { get; set; }
        public abstract class pOLine_ReceiptNbr : PX.Data.BQL.BqlString.Field<pOLine_ReceiptNbr> { }
        #endregion

        #region POLine_LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "POLine_Line Nbr")]
        public virtual int? POLine_LineNbr { get; set; }
        public abstract class pOLine_LineNbr : PX.Data.BQL.BqlInt.Field<pOLine_LineNbr> { }
        #endregion

        #region InventoryID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uom")]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region ReceiptType
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Receipt Type")]
        public virtual string ReceiptType { get; set; }
        public abstract class receiptType : PX.Data.BQL.BqlString.Field<receiptType> { }
        #endregion

        #region ReceiptNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Receipt Nbr")]
        public virtual string ReceiptNbr { get; set; }
        public abstract class receiptNbr : PX.Data.BQL.BqlString.Field<receiptNbr> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Curyid
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Curyid")]
        public virtual string Curyid { get; set; }
        public abstract class curyid : PX.Data.BQL.BqlString.Field<curyid> { }
        #endregion

        #region POReceipt_CuryInfoID
        [PXDBLong()]
        [PXUIField(DisplayName = "POReceipt_ Cury Info ID")]
        public virtual long? POReceipt_CuryInfoID { get; set; }
        public abstract class pOReceipt_CuryInfoID : PX.Data.BQL.BqlLong.Field<pOReceipt_CuryInfoID> { }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt")]
        public virtual Decimal? Amt { get; set; }
        public abstract class amt : PX.Data.BQL.BqlDecimal.Field<amt> { }
        #endregion

        #region Qty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty")]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion
    }
}