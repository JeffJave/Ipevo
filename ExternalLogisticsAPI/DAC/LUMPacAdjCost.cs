using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.IN;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("LUMPacAdjCost")]
    public class LUMPacAdjCost : IBqlTable
    {

        public static class FK
        {
            public class InventoryItem : PX.Objects.IN.InventoryItem.PK.ForeignKeyOf<LUMPacAdjCost>.By<inventoryID> { }
            public class Site : INSite.PK.ForeignKeyOf<LUMPacAdjCost>.By<siteid> { }
        }

        #region FinPeriodID
        [PXDBString(6, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Period")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region Finptdcogs
        [PXDBDecimal()]
        [PXUIField(DisplayName = "COGS(F)")]
        public virtual Decimal? Finptdcogs { get; set; }
        public abstract class finptdcogs : PX.Data.BQL.BqlDecimal.Field<finptdcogs> { }
        #endregion

        #region FinPtdQtySales
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Qty Sales(E)")]
        public virtual Decimal? FinPtdQtySales { get; set; }
        public abstract class finPtdQtySales : PX.Data.BQL.BqlDecimal.Field<finPtdQtySales> { }
        #endregion

        #region PACUnitCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PAC Unit Cost(=O/N)")]
        public virtual Decimal? PACUnitCost { get; set; }
        public abstract class pACUnitCost : PX.Data.BQL.BqlDecimal.Field<pACUnitCost> { }
        #endregion

        #region InventoryID
        [StockItem(IsKey = true)]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region Paccogs
        [PXDBDecimal()]
        [PXUIField(DisplayName = "PAC COGS (Q=PxE)")]
        public virtual Decimal? Paccogs { get; set; }
        public abstract class paccogs : PX.Data.BQL.BqlDecimal.Field<paccogs> { }
        #endregion

        #region Siteid
        [PX.Objects.IN.Site(DisplayName = "Warehouse ID", DescriptionField = typeof(INSite.descr),IsKey = true)]
        [PXUIField(DisplayName = "Warehouse")]
        [PXForeignReference(typeof(FK.Site))]
        public virtual int? Siteid { get; set; }
        public abstract class siteid : PX.Data.BQL.BqlInt.Field<siteid> { }
        #endregion

        #region Cogsadj
        [PXDBDecimal()]
        [PXUIField(DisplayName = "COGS ADJ Amount(COGS-PAC COGS)")]
        public virtual Decimal? Cogsadj { get; set; }
        public abstract class cogsadj : PX.Data.BQL.BqlDecimal.Field<cogsadj> { }
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