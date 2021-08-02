using System;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("vRMAOrderInfo")]
    public class vRMAOrderInfo : IBqlTable
    {
        #region CompanyCD
        [PXDBString(128, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Company CD")]
        public virtual string CompanyCD { get; set; }
        public abstract class companyCD : PX.Data.BQL.BqlString.Field<companyCD> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Order Nbr")]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region OrderDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Desc")]
        public virtual string OrderDesc { get; set; }
        public abstract class orderDesc : PX.Data.BQL.BqlString.Field<orderDesc> { }
        #endregion

        #region CustomerOrderNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Customer Order Nbr")]
        public virtual string CustomerOrderNbr { get; set; }
        public abstract class customerOrderNbr : PX.Data.BQL.BqlString.Field<customerOrderNbr> { }
        #endregion

        #region CustomerID
        [PXDBString()]
        [PXUIField(DisplayName = "Customer ID")]
        public virtual string CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlString.Field<customerID> { }
        #endregion

        #region CustomerRefNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Customer Ref Nbr")]
        public virtual string CustomerRefNbr { get; set; }
        public abstract class customerRefNbr : PX.Data.BQL.BqlString.Field<customerRefNbr> { }
        #endregion

        #region ShipmentNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Shipment Nbr")]
        public virtual string ShipmentNbr { get; set; }
        public abstract class shipmentNbr : PX.Data.BQL.BqlString.Field<shipmentNbr> { }
        #endregion

        #region ShipDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Ship Date")]
        public virtual DateTime? ShipDate { get; set; }
        public abstract class shipDate : PX.Data.BQL.BqlDateTime.Field<shipDate> { }
        #endregion

        #region InventoryID
        [PXDBString(255, IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual string InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlString.Field<inventoryID> { }
        #endregion

        #region InventoryDescr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory Descr")]
        public virtual string InventoryDescr { get; set; }
        public abstract class inventoryDescr : PX.Data.BQL.BqlString.Field<inventoryDescr> { }
        #endregion

        #region OrderQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Order Qty")]
        public virtual Decimal? OrderQty { get; set; }
        public abstract class orderQty : PX.Data.BQL.BqlDecimal.Field<orderQty> { }
        #endregion

        #region TaxRate
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Rate")]
        public virtual Decimal? TaxRate { get; set; }
        public abstract class taxRate : PX.Data.BQL.BqlDecimal.Field<taxRate> { }
        #endregion

        #region CuryTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Tax Amt")]
        public virtual Decimal? CuryTaxAmt { get; set; }
        public abstract class curyTaxAmt : PX.Data.BQL.BqlDecimal.Field<curyTaxAmt> { }
        #endregion

        #region Siteid
        [PXDBInt()]
        [PXUIField(DisplayName = "Siteid")]
        public virtual int? Siteid { get; set; }
        public abstract class siteid : PX.Data.BQL.BqlInt.Field<siteid> { }
        #endregion

        #region SiteDescr
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Site Descr")]
        public virtual string SiteDescr { get; set; }
        public abstract class siteDescr : PX.Data.BQL.BqlString.Field<siteDescr> { }
        #endregion

        #region Email
        [PXDBString(255, InputMask = "")]
        [PXUIField(DisplayName = "Email")]
        public virtual string Email { get; set; }
        public abstract class email : PX.Data.BQL.BqlString.Field<email> { }
        #endregion
    }
}