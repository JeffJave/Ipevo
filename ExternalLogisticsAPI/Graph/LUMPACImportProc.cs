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
        public PXFilteredProcessing<LUMPacAdjCost, PACFilter, Where<LUMPacAdjCost.finPeriodID, Equal<Current<PACFilter.finPeriod>>>> ImportPACList;

        public PXAction<PACFilter> loadData;
        [PXButton]
        [PXUIField(DisplayName = "Load Data", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LoadData(PXAdapter adapter)
        {
            var filter = this.Filter.Current;
            var sourceData = SelectFrom<vPACAdjCost>.Where<vPACAdjCost.finPeriodID.IsEqual<P.AsString>>.View.Select(this, filter.FinPeriod).RowCast<vPACAdjCost>().ToList().Where(x => x.FinPeriodID == filter.FinPeriod);

            // Delete temp table data
            PXDatabase.Delete<LUMPacAdjCost>();
            this.ImportPACList.Cache.Clear();

            foreach (var item in sourceData)
            {
                var data = this.ImportPACList.Insert((LUMPacAdjCost)this.ImportPACList.Cache.CreateInstance());
                data.FinPeriodID = item.FinPeriodID;
                data.Finptdcogs = item.Finptdcogs;
                data.FinPtdQtySales = item.FinPtdQtySales;
                data.PACUnitCost = item.PACUnitCost;
                data.InventoryID = item.InventoryID;
                data.Paccogs = item.Paccogs;
                data.Siteid = item.Siteid;
                data.Cogsadj = item.Cogsadj;
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
                var impDatas = this.ImportPACList.Select().RowCast<LUMPacAdjCost>().ToList();

                if (string.IsNullOrEmpty(filter.FinPeriod))
                    throw new PXException("Period can not be empty!!");

                if (!impDatas.Any())
                    throw new PXException("No Data Found!!");

                // Create Adjustment
                decimal sum = 0;

                var doc = graph.adjustment.Insert((INRegister)graph.adjustment.Cache.CreateInstance());
                doc.FinPeriodID = filter.FinPeriod;
                doc.TranDesc = "PAC COGS Adujstment";

                foreach (var row in impDatas)
                {
                    if (Math.Round((row.Cogsadj ?? 0), 0) == 0)
                        continue;
                    var line = graph.transactions.Insert((INTran)graph.transactions.Cache.CreateInstance());
                    graph.transactions.SetValueExt<INTran.inventoryID>(line, row.InventoryID);
                    graph.transactions.SetValueExt<INTran.siteID>(line, row.Siteid);
                    graph.transactions.SetValueExt<INTran.tranCost>(line, row.Cogsadj);
                    graph.transactions.SetValueExt<INTran.reasonCode>(line, "PACADJ");
                    graph.transactions.SetValueExt<INTran.lotSerialNbr>(line, string.Empty);
                    sum += (row.Cogsadj ?? 0);
                }
                doc.TotalCost = sum;
                graph.Save.Press();
                // Delete temp table data
                PXDatabase.Delete<LUMPacAdjCost>();
                this.ImportPACList.Cache.Clear();
            }
            catch (Exception ex)
            {
                throw new PXException(ex.Message);
            }
            throw new PXRedirectRequiredException(graph, "");
            return adapter.Get();
        }

    }

    [Serializable]
    public class PACFilter : IBqlTable
    {
        [PXDBString]
        [PXUIField(DisplayName = "Period", Required = true)]
        public virtual string FinPeriod { get; set; }
        public abstract class finPeriod : PX.Data.BQL.BqlString.Field<finPeriod> { }
    }
}
