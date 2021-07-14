using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("vPACAdjCost")]
    public class vPACAdjCost : IBqlTable
    {
        #region FinPeriodID
        [PXDBString(6, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Fin Period ID")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region Finptdcogs
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Finptdcogs")]
        public virtual Decimal? Finptdcogs { get; set; }
        public abstract class finptdcogs : PX.Data.BQL.BqlDecimal.Field<finptdcogs> { }
        #endregion

        #region FinPtdQtySales
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Fin Ptd Qty Sales")]
        public virtual Decimal? FinPtdQtySales { get; set; }
        public abstract class finPtdQtySales : PX.Data.BQL.BqlDecimal.Field<finPtdQtySales> { }
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

        #region Paccogs
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Paccogs")]
        public virtual Decimal? Paccogs { get; set; }
        public abstract class paccogs : PX.Data.BQL.BqlDecimal.Field<paccogs> { }
        #endregion

        #region Siteid
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Siteid")]
        public virtual int? Siteid { get; set; }
        public abstract class siteid : PX.Data.BQL.BqlInt.Field<siteid> { }
        #endregion

        #region Cogsadj
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cogsadj")]
        public virtual Decimal? Cogsadj { get; set; }
        public abstract class cogsadj : PX.Data.BQL.BqlDecimal.Field<cogsadj> { }
        #endregion
    }
}