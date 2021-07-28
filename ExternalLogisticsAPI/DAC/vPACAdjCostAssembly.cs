using System;
using PX.Data;
using PX.Objects.IN;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("vPACAdjCostAssembly")]
    public class vPACAdjCostAssembly : IBqlTable
    {
        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "",IsKey = true)]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region ItemClassID
        [PXDBInt()]
        [PXUIField(DisplayName = "Item Class ID")]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }
        #endregion

        #region FinPtdCostAssemblyOut
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Fin Ptd Cost Assembly Out")]
        public virtual Decimal? FinPtdCostAssemblyOut { get; set; }
        public abstract class finPtdCostAssemblyOut : PX.Data.BQL.BqlDecimal.Field<finPtdCostAssemblyOut> { }
        #endregion

        #region FinPtdQtyAssemblyOut
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Fin Ptd Qty Assembly Out")]
        public virtual Decimal? FinPtdQtyAssemblyOut { get; set; }
        public abstract class finPtdQtyAssemblyOut : PX.Data.BQL.BqlDecimal.Field<finPtdQtyAssemblyOut> { }
        #endregion

        #region PACUnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PACUnit Cost")]
        public virtual Decimal? PACUnitCost { get; set; }
        public abstract class pACUnitCost : PX.Data.BQL.BqlDecimal.Field<pACUnitCost> { }
        #endregion

        #region InventoryID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region PACIssueCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PAC Assembly Issue Cost")]
        public virtual Decimal? PACIssueCost { get; set; }
        public abstract class pACIssueCost : PX.Data.BQL.BqlDecimal.Field<pACIssueCost> { }
        #endregion

        #region Siteid
        [PXDBInt()]
        [PXUIField(DisplayName = "Siteid")]
        public virtual int? Siteid { get; set; }
        public abstract class siteid : PX.Data.BQL.BqlInt.Field<siteid> { }
        #endregion

        #region AssembleAdjAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Assemble Adj Amount")]
        public virtual Decimal? AssemblyAdjAmount { get; set; }
        public abstract class assembleAdjAmount : PX.Data.BQL.BqlDecimal.Field<assembleAdjAmount> { }
        #endregion
    }
}