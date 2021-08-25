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

        #region CustomerName
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Customer Name")]
        public virtual string CustomerName { get; set; }
        public abstract class customerName : PX.Data.BQL.BqlString.Field<customerName> { }
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

        #region TaxZoneID
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "TaxZoneID")]
        public virtual string TaxZoneID { get; set; }
        public abstract class taxZoneID : PX.Data.BQL.BqlString.Field<taxZoneID> { }
        #endregion

        #region CuryTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Tax Amt")]
        public virtual Decimal? CuryTaxAmt { get; set; }
        public abstract class curyTaxAmt : PX.Data.BQL.BqlDecimal.Field<curyTaxAmt> { }
        #endregion

        #region TaxRate
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Rate")]
        public virtual Decimal? TaxRate { get; set; }
        public abstract class taxRate : PX.Data.BQL.BqlDecimal.Field<taxRate> { }
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

        #region BillCompany
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Company")]
        public virtual string BillCompany { get; set; }
        public abstract class billCompany : PX.Data.BQL.BqlString.Field<billCompany> { }
        #endregion

        #region BillAttention
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Attention")]
        public virtual string BillAttention { get; set; }
        public abstract class billAttention : PX.Data.BQL.BqlString.Field<billAttention> { }
        #endregion

        #region BillPhone
        [PXDBString(50, InputMask = "")]
        [PXUIField(DisplayName = "Bill Phone")]
        public virtual string BillPhone { get; set; }
        public abstract class billPhone : PX.Data.BQL.BqlString.Field<billPhone> { }
        #endregion

        #region BillEmail
        [PXDBString(255, InputMask = "")]
        [PXUIField(DisplayName = "Bill Email")]
        public virtual string BillEmail { get; set; }
        public abstract class billEmail : PX.Data.BQL.BqlString.Field<billEmail> { }
        #endregion

        #region BillAddr1
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Addr1")]
        public virtual string BillAddr1 { get; set; }
        public abstract class billAddr1 : PX.Data.BQL.BqlString.Field<billAddr1> { }
        #endregion

        #region BillAddr2
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Addr2")]
        public virtual string BillAddr2 { get; set; }
        public abstract class billAddr2 : PX.Data.BQL.BqlString.Field<billAddr2> { }
        #endregion

        #region BillCity
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill City")]
        public virtual string BillCity { get; set; }
        public abstract class billCity : PX.Data.BQL.BqlString.Field<billCity> { }
        #endregion

        #region BillCountry
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill Country")]
        public virtual string BillCountry { get; set; }
        public abstract class billCountry : PX.Data.BQL.BqlString.Field<billCountry> { }
        #endregion

        #region BillState
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bill State")]
        public virtual string BillState { get; set; }
        public abstract class billState : PX.Data.BQL.BqlString.Field<billState> { }
        #endregion

        #region BillPostalCode
        [PXDBString(20, InputMask = "")]
        [PXUIField(DisplayName = "Bill Postal Code")]
        public virtual string BillPostalCode { get; set; }
        public abstract class billPostalCode : PX.Data.BQL.BqlString.Field<billPostalCode> { }
        #endregion

        #region ShipCompany
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship Company")]
        public virtual string ShipCompany { get; set; }
        public abstract class shipCompany : PX.Data.BQL.BqlString.Field<shipCompany> { }
        #endregion

        #region ShipAttention
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship Attention")]
        public virtual string ShipAttention { get; set; }
        public abstract class shipAttention : PX.Data.BQL.BqlString.Field<shipAttention> { }
        #endregion

        #region ShipPhone
        [PXDBString(50, InputMask = "")]
        [PXUIField(DisplayName = "Ship Phone")]
        public virtual string ShipPhone { get; set; }
        public abstract class shipPhone : PX.Data.BQL.BqlString.Field<shipPhone> { }
        #endregion

        #region ShipEmail
        [PXDBString(255, InputMask = "")]
        [PXUIField(DisplayName = "Ship Email")]
        public virtual string ShipEmail { get; set; }
        public abstract class shipEmail : PX.Data.BQL.BqlString.Field<shipEmail> { }
        #endregion

        #region ShipAddr1
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship Addr1")]
        public virtual string ShipAddr1 { get; set; }
        public abstract class shipAddr1 : PX.Data.BQL.BqlString.Field<shipAddr1> { }
        #endregion

        #region ShipAddr2
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship Addr2")]
        public virtual string ShipAddr2 { get; set; }
        public abstract class shipAddr2 : PX.Data.BQL.BqlString.Field<shipAddr2> { }
        #endregion

        #region ShipCity
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship City")]
        public virtual string ShipCity { get; set; }
        public abstract class shipCity : PX.Data.BQL.BqlString.Field<shipCity> { }
        #endregion

        #region ShipCountryID
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship Country ID")]
        public virtual string ShipCountryID { get; set; }
        public abstract class shipCountryID : PX.Data.BQL.BqlString.Field<shipCountryID> { }
        #endregion

        #region ShipState
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ship State")]
        public virtual string ShipState { get; set; }
        public abstract class shipState : PX.Data.BQL.BqlString.Field<shipState> { }
        #endregion

        #region ShipPostalCode
        [PXDBString(20, InputMask = "")]
        [PXUIField(DisplayName = "Ship Postal Code")]
        public virtual string ShipPostalCode { get; set; }
        public abstract class shipPostalCode : PX.Data.BQL.BqlString.Field<shipPostalCode> { }
        #endregion

        #region InvoiceNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invoice Nbr")]
        public virtual string InvoiceNbr { get; set; }
        public abstract class invoiceNbr : PX.Data.BQL.BqlString.Field<invoiceNbr> { }
        #endregion

        #region CuryID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CuryID")]
        public virtual string CuryID { get; set; }
        public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
        #endregion
    }
}