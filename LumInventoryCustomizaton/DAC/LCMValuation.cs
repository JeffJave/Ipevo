using System;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.IN;

namespace LumInventoryCustomizaton.DAC
{
  [Serializable]
  [PXCacheName("LCMValuation")]
  public class LCMValuation : IBqlTable
  {
    #region CostSiteID
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Cost Site ID")]
    public virtual int? CostSiteID { get; set; }
    public abstract class costSiteID : PX.Data.BQL.BqlInt.Field<costSiteID> { }
    #endregion

    #region Sitecd
    [PXDBString(30, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Site ID")]
    public virtual string Sitecd { get; set; }
    public abstract class sitecd : PX.Data.BQL.BqlString.Field<sitecd> { }
    #endregion

    #region InventoryID
    [Inventory(Filterable = true, DirtyRead = true, Enabled = false, DisplayName = "Inventory ID")]
    public virtual int? InventoryID { get; set; }
    public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
    #endregion

    #region FinPeriodID
    [PXDBString(6, IsFixed = true, InputMask = "")]
    [PXUIField(DisplayName = "Fin Period ID")]
    public virtual string FinPeriodID { get; set; }
    public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
    #endregion

    #region FinYtdCost
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Fin Ytd Cost")]
    public virtual Decimal? FinYtdCost { get; set; }
    public abstract class finYtdCost : PX.Data.BQL.BqlDecimal.Field<finYtdCost> { }
    #endregion

    #region FinYtdQty
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Fin Ytd Qty")]
    public virtual Decimal? FinYtdQty { get; set; }
    public abstract class finYtdQty : PX.Data.BQL.BqlDecimal.Field<finYtdQty> { }
    #endregion

    #region UnitCost
    [PXDBDecimal(4)]
    [PXUIField(DisplayName = "Unit Cost")]
    public virtual Decimal? UnitCost { get; set; }
    public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
    #endregion

    #region LastSalesPrice
    [PXDBDecimal(4)]
    [PXUIField(DisplayName = "Last Sales Price")]
    public virtual Decimal? LastSalesPrice { get; set; }
    public abstract class lastSalesPrice : PX.Data.BQL.BqlDecimal.Field<lastSalesPrice> { }
    #endregion

    #region LastSalesDate
    [PXDBDate()]
    [PXUIField(DisplayName = "Last Sales Date")]
    public virtual DateTime? LastSalesDate { get; set; }
    public abstract class lastSalesDate : PX.Data.BQL.BqlDateTime.Field<lastSalesDate> { }
    #endregion

    #region NetRealizableValue
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Net Realizable Value")]
    public virtual Decimal? NetRealizableValue { get; set; }
    public abstract class netRealizableValue : PX.Data.BQL.BqlDecimal.Field<netRealizableValue> { }
    #endregion

    #region IsValuationLoss
    [PXDBInt()]
    [PXUIField(DisplayName = "Is Valuation Loss")]
    public virtual int? IsValuationLoss { get; set; }
    public abstract class isValuationLoss : PX.Data.BQL.BqlInt.Field<isValuationLoss> { }
    #endregion

    #region ValuationLoss
    [PXDBDecimal()]
    [PXUIField(DisplayName = "Valuation Loss")]
    public virtual Decimal? ValuationLoss { get; set; }
    public abstract class valuationLoss : PX.Data.BQL.BqlDecimal.Field<valuationLoss> { }
    #endregion

    #region LastPurchasePrice
    [PXDBDecimal(4)]
    [PXUIField(DisplayName = "Last Purchase Price")]
    public virtual Decimal? LastPurchasePrice { get; set; }
    public abstract class lastPurchasePrice : PX.Data.BQL.BqlDecimal.Field<lastPurchasePrice> { }
        #endregion

    #region LastPurchaseDate
    [PXDBDate()]
    [PXUIField(DisplayName = "Last Purchase Date")]
    public virtual DateTime? LastPurchaseDate { get; set; }
    public abstract class lastPurchaseDate : PX.Data.BQL.BqlDateTime.Field<lastPurchaseDate> { }
    #endregion
  }
}