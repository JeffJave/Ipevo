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
    public class LUMPACIssueImportProc : PXGraph<LUMPACIssueImportProc>
    {
        public PXCancel<PACFilter> Cancel;
        public PXFilter<PACFilter> Filter;

        public LUMPACIssueImportProc()
        {
            this.ImportPACList.SetProcessVisible(false);
            this.ImportPACList.SetProcessAllVisible(false);
        }

        [PXFilterable]
        public PXFilteredProcessing<LumPacIssueAdjCost, PACFilter, Where<LumPacIssueAdjCost.finPeriodID, Equal<Current<PACFilter.finPeriod>>>> ImportPACList;

        public PXAction<PACFilter> loadData;
        [PXButton]
        [PXUIField(DisplayName = "Load Data", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LoadData(PXAdapter adapter)
        {
            var filter = this.Filter.Current;
            var sourceData = SelectFrom<vPACAdjCostIssue>
                             .Where<vPACAdjCostIssue.finPeriodID.IsEqual<P.AsString>>.View.Select(this, filter.FinPeriod).RowCast<vPACAdjCostIssue>().ToList();

            if(filter.ItemClassID.HasValue)
                sourceData = sourceData.Where(x => x.ItemClassID == filter.ItemClassID.Value).ToList();

            // Delete temp table data
            PXDatabase.Delete<LumPacIssueAdjCost>();
            this.ImportPACList.Cache.Clear();

            foreach (var item in sourceData)
            {
                var data = this.ImportPACList.Insert((LumPacIssueAdjCost)this.ImportPACList.Cache.CreateInstance());
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
            }
            this.Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<PACFilter> generateAdjustment;
        [PXButton]
        [PXUIField(DisplayName = "Generate Issue Adjustment", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable GenerateAdjustment(PXAdapter adapter)
        {
            var graph = PXGraph.CreateInstance<INAdjustmentEntry>();
            try
            {
                var filter = this.Filter.Current;
                var impDatas = this.ImportPACList.Select().RowCast<LumPacIssueAdjCost>().ToList();

                if (string.IsNullOrEmpty(filter.FinPeriod))
                    throw new PXException("Period can not be empty!!");

                if (!impDatas.Any())
                    throw new PXException("No Data Found!!");

                // Create Adjustment
                decimal sum = 0;

                var doc = graph.adjustment.Insert((INRegister)graph.adjustment.Cache.CreateInstance());
                doc.FinPeriodID = filter.FinPeriod;
                doc.TranDesc = "PAC Issue Adujstment";

                foreach (var row in impDatas.Where(x => x.Selected ?? false))
                {
                    if (Math.Round((row.IssueAdjAmount ?? 0), 0) == 0)
                        continue;
                    var line = graph.transactions.Insert((INTran)graph.transactions.Cache.CreateInstance());
                    graph.transactions.SetValueExt<INTran.inventoryID>(line, row.InventoryID);
                    graph.transactions.SetValueExt<INTran.siteID>(line, row.Siteid);
                    graph.transactions.SetValueExt<INTran.tranCost>(line, row.IssueAdjAmount);
                    graph.transactions.SetValueExt<INTran.reasonCode>(line, string.IsNullOrEmpty(row.ReasonCode) ? string.Empty : row.ReasonCode + "A");
                    graph.transactions.SetValueExt<INTran.lotSerialNbr>(line, string.Empty);
                    sum += (row.IssueAdjAmount ?? 0);
                }
                doc.TotalCost = sum;
                graph.Save.Press();
                // Delete temp table data
                PXDatabase.Delete<LumPacIssueAdjCost>();
                this.ImportPACList.Cache.Clear();
            }
            catch (Exception ex)
            {
                throw new PXException(ex.Message);
            }
            throw new PXRedirectRequiredException(graph, "");
        }

    }
}
