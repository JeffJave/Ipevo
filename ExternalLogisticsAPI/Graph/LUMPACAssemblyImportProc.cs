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
    public class LUMPACAssemblyImportProc : PXGraph<LUMPACAssemblyImportProc>
    {
        public PXCancel<PACFilter> Cancel;
        public PXFilter<PACFilter> Filter;

        public LUMPACAssemblyImportProc()
        {
            this.ImportPACList.SetProcessVisible(false);
            this.ImportPACList.SetProcessAllVisible(false);
        }

        [PXFilterable]
        public PXFilteredProcessing<LUMPacAssemblyAdjCost,
                                    PACFilter,
                                    Where<LUMPacAssemblyAdjCost.finPeriodID, Equal<Current<PACFilter.finPeriod>>>> ImportPACList;

        public PXAction<PACFilter> loadData;
        [PXButton]
        [PXUIField(DisplayName = "Load Data", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LoadData(PXAdapter adapter)
        {
            var filter = this.Filter.Current;
            var sourceData = SelectFrom<vPACAdjCostAssembly>
                             .InnerJoin<INComponentTran>.On<vPACAdjCostAssembly.inventoryID.IsEqual<INComponentTran.inventoryID>
                                        .And<vPACAdjCostAssembly.finPeriodID.IsEqual<INComponentTran.finPeriodID>>>
                             .InnerJoin<PX.Objects.IN.INKitRegister>.On<INComponentTran.docType.IsEqual<PX.Objects.IN.INKitRegister.docType>
                                        .And<INComponentTran.refNbr.IsEqual<PX.Objects.IN.INKitRegister.refNbr>>>
                             .Where<vPACAdjCostAssembly.finPeriodID.IsEqual<P.AsString>
                                        .And<vPACAdjCostAssembly.itemClassID.IsEqual<P.AsInt>>>
                             .View.Select(new PXGraph(), filter.FinPeriod, filter.ItemClassID).ToList();
            // Delete temp table data
            PXDatabase.Delete<LUMPacAssemblyAdjCost>();
            this.ImportPACList.Cache.Clear();

            foreach (var item in sourceData)
            {
                var data = this.ImportPACList.Insert((LUMPacAssemblyAdjCost)this.ImportPACList.Cache.CreateInstance());
                data.FinPeriodID = item.GetItem<vPACAdjCostAssembly>().FinPeriodID;
                data.FinPtdCostAssemblyOut = item.GetItem<vPACAdjCostAssembly>().FinPtdCostAssemblyOut;
                data.FinPtdQtyAssemblyOut = item.GetItem<vPACAdjCostAssembly>().FinPtdQtyAssemblyOut;
                data.PACUnitCost = item.GetItem<vPACAdjCostAssembly>().PACUnitCost;
                data.InventoryID = item.GetItem<vPACAdjCostAssembly>().InventoryID;
                data.ItemClassID = item.GetItem<vPACAdjCostAssembly>().ItemClassID;
                data.PACIssueCost = item.GetItem<vPACAdjCostAssembly>().PACIssueCost;
                data.Siteid = item.GetItem<vPACAdjCostAssembly>().Siteid;
                data.AssemblyAdjAmount = item.GetItem<vPACAdjCostAssembly>().AssemblyAdjAmount;
                data.ProductInventoryID = item.GetItem<INKitRegister>().KitInventoryID;
                data.ProductAdjAmount = data.AssemblyAdjAmount * -1;
            }
            this.Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<PACFilter> generateAdjustment;
        [PXButton]
        [PXUIField(DisplayName = "Generate Assembly Adjustment", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable GenerateAdjustment(PXAdapter adapter)
        {
            var graph = PXGraph.CreateInstance<INAdjustmentEntry>();
            try
            {
                var filter = this.Filter.Current;
                var impDatas = this.ImportPACList.Select().RowCast<LUMPacAssemblyAdjCost>().ToList();

                if (string.IsNullOrEmpty(filter.FinPeriod))
                    throw new PXException("Period can not be empty!!");
                if (!filter.ItemClassID.HasValue)
                    throw new PXException("ItemClass can not be empty!!");

                if (!impDatas.Any())
                    throw new PXException("No Data Found!!");

                // Create Adjustment
                decimal sum = 0;

                var doc = graph.adjustment.Insert((INRegister)graph.adjustment.Cache.CreateInstance());
                doc.FinPeriodID = filter.FinPeriod;
                doc.TranDesc = "PAC Assembly Adujstment";

                foreach (var row in impDatas)
                {
                    if (Math.Round((row.ProductAdjAmount ?? 0), 0) == 0)
                        continue;
                    var line = graph.transactions.Insert((INTran)graph.transactions.Cache.CreateInstance());
                    graph.transactions.SetValueExt<INTran.inventoryID>(line, row.ProductInventoryID);
                    graph.transactions.SetValueExt<INTran.siteID>(line, row.Siteid);
                    graph.transactions.SetValueExt<INTran.tranCost>(line, row.ProductAdjAmount);
                    graph.transactions.SetValueExt<INTran.reasonCode>(line, "PACADJ");
                    graph.transactions.SetValueExt<INTran.lotSerialNbr>(line, string.Empty);
                    sum += (row.ProductAdjAmount ?? 0);
                }
                doc.TotalCost = sum;
                graph.Save.Press();
                // Delete temp table data
                PXDatabase.Delete<LUMPacAssemblyAdjCost>();
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
