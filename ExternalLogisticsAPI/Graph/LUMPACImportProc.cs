using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMPACImportProc : PXGraph<LUMPACImportProc>
    {
        public PXCancel<PACFilter> Cancel;
        public PXFilter<PACFilter> Filter;

        public LUMPACImportProc()
        {
            this.ImportPACList.SetProcessVisible(false);
            this.ImportPACList.SetProcessAllVisible(false);
        }

        [PXFilterable]
        public PXFilteredProcessing<LUMPacCOGSAdjCost, PACFilter, Where<LUMPacCOGSAdjCost.finPeriodID, Equal<Current<PACFilter.finPeriod>>>> ImportPACList;

        public PXAction<PACFilter> loadData;
        [PXButton]
        [PXUIField(DisplayName = "Load Data", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LoadData(PXAdapter adapter)
        {
            var filter = this.Filter.Current;
            var sourceData = SelectFrom<vPACAdjCostCOGS>
                             .Where<vPACAdjCostCOGS.finPeriodID.IsEqual<P.AsString>>.View.Select(this, filter.FinPeriod).RowCast<vPACAdjCostCOGS>().ToList();

            var histData = SelectFrom<LUMPacUnitCostHistory>
                          .Where<LUMPacUnitCostHistory.finPeriodID.IsEqual<P.AsString>>
                          .View.Select(this, filter.FinPeriod).RowCast<LUMPacUnitCostHistory>().ToList();

            if (filter.ItemClassID.HasValue)
                sourceData = sourceData.Where(x => x.ItemClassID == filter.ItemClassID.Value).ToList();
            if (histData.Count == 0)
                throw new PXException("Data is not found in PAC Unit Cost History, please process ‘Save PAC Unit Cost History’ again");

            // Delete temp table data
            PXDatabase.Delete<LUMPacCOGSAdjCost>();
            this.ImportPACList.Cache.Clear();

            foreach (var item in sourceData)
            {
                var data = this.ImportPACList.Insert((LUMPacCOGSAdjCost)this.ImportPACList.Cache.CreateInstance());
                data.FinPeriodID = item.FinPeriodID;
                data.FinPtdCostIssued = item.FinPtdCostIssued;
                data.FinPtdQtyIssued = item.FinPtdQtyIssued;
                data.PACUnitCost = item.PACUnitCost;
                data.InventoryID = item.InventoryID;
                data.ItemClassID = item.ItemClassID;
                data.PACIssueCost = item.PACIssueCost;
                data.Siteid = item.Siteid;
                data.IssueAdjAmount = item.IssueAdjAmount;
                data.ReasonCode = item.ReasonCode;
                data.RefNbr = item.RefNbr;
                data.LineNbr = item.LineNbr;
                data.SOOrderNbr = item.SOOrderNbr;
                data.Selected = true;
            }
            this.Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<PACFilter> generateAdjustment;
        [PXButton]
        [PXUIField(DisplayName = "Generate COGS Adjustment", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable GenerateAdjustment(PXAdapter adapter)
        {
            var graph = PXGraph.CreateInstance<INAdjustmentEntry>();
            try
            {
                var filter = this.Filter.Current;
                var impDatas = this.ImportPACList.Select().RowCast<LUMPacCOGSAdjCost>().ToList();

                if (string.IsNullOrEmpty(filter.FinPeriod))
                    throw new PXException("Period can not be empty!!");

                if (!impDatas.Any())
                    throw new PXException("No Data Found!!");

                // Create Adjustment
                decimal sum = 0;

                var doc = graph.adjustment.Insert((INRegister)graph.adjustment.Cache.CreateInstance());
                doc.FinPeriodID = filter.FinPeriod;
                doc.TranDesc = "PAC COGS Adujstment";

                foreach (var row in impDatas.Where(x => x.Selected ?? true))
                {
                    if (Math.Round((row.IssueAdjAmount ?? 0), 0) == 0)
                        continue;
                    var line = graph.transactions.Insert((INTran)graph.transactions.Cache.CreateInstance());
                    graph.transactions.SetValueExt<INTran.inventoryID>(line, row.InventoryID);
                    graph.transactions.SetValueExt<INTran.siteID>(line, row.Siteid);
                    graph.transactions.SetValueExt<INTran.tranCost>(line, row.IssueAdjAmount);
                    graph.transactions.SetValueExt<INTran.reasonCode>(line, string.IsNullOrEmpty(row.ReasonCode) ? "PACADJ" : row.ReasonCode + "A");
                    graph.transactions.SetValueExt<INTran.lotSerialNbr>(line, string.Empty);
                    graph.transactions.SetValueExt<INTran.tranDesc>(line, row.SOOrderNbr);
                    sum += (row.IssueAdjAmount ?? 0);
                }
                doc.TotalCost = sum;
                graph.Save.Press();
                // Delete temp table data
                PXDatabase.Delete<LUMPacCOGSAdjCost>();
                this.ImportPACList.Cache.Clear();
            }
            catch (Exception ex)
            {
                throw new PXException(ex.Message);
            }
            throw new PXRedirectRequiredException(graph, "");
        }

    }

    [Serializable]
    public class PACFilter : IBqlTable
    {
        [PXDBString]
        [PXUIField(DisplayName = "Period", Required = true)]
        public virtual string FinPeriod { get; set; }
        public abstract class finPeriod : PX.Data.BQL.BqlString.Field<finPeriod> { }

        [PXDBInt]
        [PXUIField(DisplayName = "Item Class")]
        [PXDimensionSelector(INItemClass.Dimension, typeof(Search<INItemClass.itemClassID>), typeof(INItemClass.itemClassCD), DescriptionField = typeof(INItemClass.descr), CacheGlobal = true)]
        public virtual int? ItemClassID { get; set; }
        public abstract class itemClassID : PX.Data.BQL.BqlInt.Field<itemClassID> { }

    }
}
