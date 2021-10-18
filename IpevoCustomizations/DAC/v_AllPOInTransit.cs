using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.PO;

namespace IpevoCustomizations.DAC
{
    [Serializable]
    [PXCacheName("v_AllPOInTransit")]
    public class v_AllPOInTransit : IBqlTable
    {
        #region TenantName
        [PXDBString(128, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tenant Name")]
        public virtual string TenantName { get; set; }
        public abstract class tenantName : PX.Data.BQL.BqlString.Field<tenantName> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr")]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [POOrderStatus.ListAttribute]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region OrderDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Order Date")]
        public virtual DateTime? OrderDate { get; set; }
        public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }
        #endregion

        #region AcctName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Name")]
        public virtual string AcctName { get; set; }
        public abstract class acctName : PX.Data.BQL.BqlString.Field<acctName> { }
        #endregion

        #region OrderDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Desc")]
        public virtual string OrderDesc { get; set; }
        public abstract class orderDesc : PX.Data.BQL.BqlString.Field<orderDesc> { }
        #endregion

        #region VendorRefNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "TW SO Nbr")]
        public virtual string VendorRefNbr { get; set; }
        public abstract class vendorRefNbr : PX.Data.BQL.BqlString.Field<vendorRefNbr> { }
        #endregion

        #region Curyid
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Currency")]
        public virtual string Curyid { get; set; }
        public abstract class curyid : PX.Data.BQL.BqlString.Field<curyid> { }
        #endregion

        #region InventoryCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region Descr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Descr")]
        public virtual string Descr { get; set; }
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "UOM")]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region OrderQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Order Qty")]
        public virtual Decimal? OrderQty { get; set; }
        public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
        #endregion

        #region OpenQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Open Qty")]
        public virtual Decimal? OpenQty { get; set; }
        public abstract class openQty : PX.Data.BQL.BqlDecimal.Field<openQty> { }
        #endregion

        #region BranchCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Branch")]
        public virtual string BranchCD { get; set; }
        public abstract class branchCD : PX.Data.BQL.BqlString.Field<branchCD> { }
        #endregion

        #region CuryUnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Unit Price")]
        public virtual Decimal? CuryUnitCost { get; set; }
        public abstract class curyUnitCost : PX.Data.BQL.BqlDecimal.Field<curyUnitCost> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
    }
}