using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("LUMPacUnitCostHistory")]
    public class LUMPacUnitCostHistory : IBqlTable
    {
        #region InventoryID
        [StockItem(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region ItemClassID
        [PXDBInt()]
        [PXUIField(DisplayName = "Item Class")]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
        #endregion

        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region PACUnitCost
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "PACUnitCost")]
        public virtual Decimal? PACUnitCost { get; set; }
        public abstract class pACUnitCost : PX.Data.BQL.BqlDecimal.Field<pACUnitCost> { }
        #endregion

        #region FinBegCost
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Beg. Cost (C1)")]
        public virtual Decimal? FinBegCost { get; set; }
        public abstract class finBegCost : PX.Data.BQL.BqlDecimal.Field<finBegCost> { }
        #endregion

        #region FinBegQty
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Beg. Qty (Q1)")]
        public virtual Decimal? FinBegQty { get; set; }
        public abstract class finBegQty : PX.Data.BQL.BqlDecimal.Field<finBegQty> { }
        #endregion

        #region Finptdcogs
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost of Sales  (F)")]
        public virtual Decimal? Finptdcogs { get; set; }
        public abstract class finptdcogs : PX.Data.BQL.BqlDecimal.Field<finptdcogs> { }
        #endregion

        #region FinPtdCOGSCredits
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "RMA Cost of Sales (C4)")]
        public virtual Decimal? FinPtdCOGSCredits { get; set; }
        public abstract class finPtdCOGSCredits : PX.Data.BQL.BqlDecimal.Field<finPtdCOGSCredits> { }
        #endregion

        #region FinPtdCostAdjusted
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost ADJ (C5)")]
        public virtual Decimal? FinPtdCostAdjusted { get; set; }
        public abstract class finPtdCostAdjusted : PX.Data.BQL.BqlDecimal.Field<finPtdCostAdjusted> { }
        #endregion

        #region FinPtdCostAssemblyIn
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost Assembly In (C3)")]
        public virtual Decimal? FinPtdCostAssemblyIn { get; set; }
        public abstract class finPtdCostAssemblyIn : PX.Data.BQL.BqlDecimal.Field<finPtdCostAssemblyIn> { }
        #endregion

        #region FinPtdCostAssemblyOut
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost Assembly Out")]
        public virtual Decimal? FinPtdCostAssemblyOut { get; set; }
        public abstract class finPtdCostAssemblyOut : PX.Data.BQL.BqlDecimal.Field<finPtdCostAssemblyOut> { }
        #endregion

        #region FinPtdCostIssued
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost Issued")]
        public virtual Decimal? FinPtdCostIssued { get; set; }
        public abstract class finPtdCostIssued : PX.Data.BQL.BqlDecimal.Field<finPtdCostIssued> { }
        #endregion

        #region FinPtdCostReceived
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost Received (C2)")]
        public virtual Decimal? FinPtdCostReceived { get; set; }
        public abstract class finPtdCostReceived : PX.Data.BQL.BqlDecimal.Field<finPtdCostReceived> { }
        #endregion

        #region FinPtdCostTransferIn
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost Transfer In")]
        public virtual Decimal? FinPtdCostTransferIn { get; set; }
        public abstract class finPtdCostTransferIn : PX.Data.BQL.BqlDecimal.Field<finPtdCostTransferIn> { }
        #endregion

        #region FinPtdCostTransferOut
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Cost Transfer Out")]
        public virtual Decimal? FinPtdCostTransferOut { get; set; }
        public abstract class finPtdCostTransferOut : PX.Data.BQL.BqlDecimal.Field<finPtdCostTransferOut> { }
        #endregion

        #region FinPtdQtyAdjusted
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY ADJ (Q5)")]
        public virtual Decimal? FinPtdQtyAdjusted { get; set; }
        public abstract class finPtdQtyAdjusted : PX.Data.BQL.BqlDecimal.Field<finPtdQtyAdjusted> { }
        #endregion

        #region FinPtdQtyAssemblyIn
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Assembly In (Q3)")]
        public virtual Decimal? FinPtdQtyAssemblyIn { get; set; }
        public abstract class finPtdQtyAssemblyIn : PX.Data.BQL.BqlDecimal.Field<finPtdQtyAssemblyIn> { }
        #endregion

        #region FinPtdQtyAssemblyOut
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Assembly Out")]
        public virtual Decimal? FinPtdQtyAssemblyOut { get; set; }
        public abstract class finPtdQtyAssemblyOut : PX.Data.BQL.BqlDecimal.Field<finPtdQtyAssemblyOut> { }
        #endregion

        #region FinPtdQtyIssued
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Issued")]
        public virtual Decimal? FinPtdQtyIssued { get; set; }
        public abstract class finPtdQtyIssued : PX.Data.BQL.BqlDecimal.Field<finPtdQtyIssued> { }
        #endregion

        #region FinPtdQtyReceived
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Received (Q2)")]
        public virtual Decimal? FinPtdQtyReceived { get; set; }
        public abstract class finPtdQtyReceived : PX.Data.BQL.BqlDecimal.Field<finPtdQtyReceived> { }
        #endregion

        #region FinPtdQtySales
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Sales")]
        public virtual Decimal? FinPtdQtySales { get; set; }
        public abstract class finPtdQtySales : PX.Data.BQL.BqlDecimal.Field<finPtdQtySales> { }
        #endregion

        #region FinPtdQtyTransferIn
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Transfer In")]
        public virtual Decimal? FinPtdQtyTransferIn { get; set; }
        public abstract class finPtdQtyTransferIn : PX.Data.BQL.BqlDecimal.Field<finPtdQtyTransferIn> { }
        #endregion

        #region FinPtdQtyTransferOut
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "QTY Transfer Out")]
        public virtual Decimal? FinPtdQtyTransferOut { get; set; }
        public abstract class finPtdQtyTransferOut : PX.Data.BQL.BqlDecimal.Field<finPtdQtyTransferOut> { }
        #endregion

        #region FinYtdCost
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Ending Cost")]
        public virtual Decimal? FinYtdCost { get; set; }
        public abstract class finYtdCost : PX.Data.BQL.BqlDecimal.Field<finYtdCost> { }
        #endregion

        #region FinYtdQty
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Ending QTY")]
        public virtual Decimal? FinYtdQty { get; set; }
        public abstract class finYtdQty : PX.Data.BQL.BqlDecimal.Field<finYtdQty> { }
        #endregion

        #region FinPtdCreditMemos
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Credit Memos")]
        public virtual Decimal? FinPtdCreditMemos { get; set; }
        public abstract class finPtdCreditMemos : PX.Data.BQL.BqlDecimal.Field<finPtdCreditMemos> { }
        #endregion

        #region TotalCostIN
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Total Cost IN")]
        public virtual Decimal? TotalCostIN { get; set; }
        public abstract class totalCostIN : PX.Data.BQL.BqlDecimal.Field<totalCostIN> { }
        #endregion

        #region TotalQtyIn
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Total Qty In")]
        public virtual Decimal? TotalQtyIn { get; set; }
        public abstract class totalQtyIn : PX.Data.BQL.BqlDecimal.Field<totalQtyIn> { }
        #endregion

    }
}
