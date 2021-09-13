using ExternalLogisticsAPI.DAC;
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
    public class LUMPACUnitCostHistoryProc : PXGraph<LUMPACUnitCostHistoryProc>
    {
        public PXCancel<PACFilter> Cancel;
        public PXFilter<PACFilter> Filter;
        [PXFilterable]
        public PXFilteredProcessing<LUMPacUnitCostHistory, PACFilter, Where<LUMPacUnitCostHistory.finPeriodID, Equal<Current<PACFilter.finPeriod>>>> ImportHistoryList;
        public LUMPACUnitCostHistoryProc()
        {
            this.ImportHistoryList.SetProcessVisible(false);
            this.ImportHistoryList.SetProcessAllVisible(false);
        }

        public PXAction<PACFilter> loadData;
        [PXButton]
        [PXUIField(DisplayName = "Load Data and Save", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LoadData(PXAdapter adapter)
        {
            var filter = this.Filter.Current;
            if (string.IsNullOrEmpty(filter.FinPeriod))
                throw new Exception("FinPeriod can not be empty!!");

            // confirm data
            WebDialogResult result = this.ImportHistoryList.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("You may override exists data"),
                MessageButtons.OKCancel, MessageIcon.Warning, true);
            if (result != WebDialogResult.OK)
                return adapter.Get();
            // Delete temp table data
            PXDatabase.Delete<LUMPacUnitCostHistory>(
                          new PXDataFieldRestrict<LUMPacUnitCostHistory.finPeriodID>(filter.FinPeriod));
            this.ImportHistoryList.Cache.Clear();
            // combine Data
            var sourceData = SelectFrom<vPACUnitCost>.Where<vPACUnitCost.finPeriodID.IsEqual<P.AsString>>.View.Select(this, filter.FinPeriod);
            foreach (var item in sourceData.RowCast<vPACUnitCost>())
            {
                #region INItemCostHist
                var inItemCostHistData = SelectFrom<INItemCostHist>
                                             .Where<INItemCostHist.finPeriodID.IsEqual<P.AsString>
                                             .And<INItemCostHist.inventoryID.IsEqual<P.AsInt>>>
                                             .View.Select(this, item.FinPeriodID, item.InventoryID)
                                             .RowCast<INItemCostHist>()
                                             .GroupBy(x => new { x.InventoryID, x.FinPeriodID })
                                             .Select(x => new
                                             {
                                                 FinBegCost = x.Sum(y => y.FinBegCost),
                                                 FinBegQty = x.Sum(y => y.FinBegQty),
                                                 Finptdcogs = x.Sum(y => y.FinPtdCOGS),
                                                 FinPtdCOGSCredits = x.Sum(y => y.FinPtdCOGSCredits),
                                                 FinPtdCostAdjusted = x.Sum(y => y.FinPtdCostAdjusted),
                                                 FinPtdCostAssemblyIn = x.Sum(y => y.FinPtdCostAssemblyIn),
                                                 FinPtdCostAssemblyOut = x.Sum(y => y.FinPtdCostAssemblyOut),
                                                 FinPtdCostIssued = x.Sum(y => y.FinPtdCostIssued),
                                                 FinPtdCostReceived = x.Sum(y => y.FinPtdCostReceived),
                                                 FinPtdCostTransferIn = x.Sum(y => y.FinPtdCostTransferIn),
                                                 FinPtdCostTransferOut = x.Sum(y => y.FinPtdCostTransferOut),
                                                 FinPtdQtyAdjusted = x.Sum(y => y.FinPtdQtyAdjusted),
                                                 FinPtdQtyAssemblyIn = x.Sum(y => y.FinPtdQtyAssemblyIn),
                                                 FinPtdQtyAssemblyOut = x.Sum(y => y.FinPtdQtyAssemblyOut),
                                                 FinPtdQtyIssued = x.Sum(y => y.FinPtdQtyIssued),
                                                 FinPtdQtyReceived = x.Sum(y => y.FinPtdQtyReceived),
                                                 FinPtdQtySales = x.Sum(y => y.FinPtdQtySales),
                                                 FinPtdQtyTransferIn = x.Sum(y => y.FinPtdQtyTransferIn),
                                                 FinPtdQtyTransferOut = x.Sum(y => y.FinPtdQtyTransferOut),
                                                 FinYtdCost = x.Sum(y => y.FinYtdCost),
                                                 FinYtdQty = x.Sum(y => y.FinYtdQty),
                                                 FinPtdCreditMemos = x.Sum(y => y.FinPtdCreditMemos),
                                                 FinPtdQtyCreditMemos = x.Sum(y => y.FinPtdQtyCreditMemos)
                                             }).FirstOrDefault(); 
                #endregion
                var data = this.ImportHistoryList.Insert((LUMPacUnitCostHistory)this.ImportHistoryList.Cache.CreateInstance());
                data.FinPeriodID = item.FinPeriodID;
                data.InventoryID = item.InventoryID;
                data.PACUnitCost = item.PACUnitCost;
                data.ItemClassID = InventoryItem.PK.Find(this, item.InventoryID).ItemClassID;
                data.PACUnitCost = item.PACUnitCost;
                data.FinBegCost = inItemCostHistData.FinBegCost;
                data.FinBegQty = inItemCostHistData.FinBegQty;
                data.Finptdcogs = inItemCostHistData.Finptdcogs;
                data.FinPtdCOGSCredits = inItemCostHistData.FinPtdCOGSCredits;
                data.FinPtdCostAdjusted = inItemCostHistData.FinPtdCostAdjusted;
                data.FinPtdCostAssemblyIn = inItemCostHistData.FinPtdCostAssemblyIn;
                data.FinPtdCostAssemblyOut = inItemCostHistData.FinPtdCostAssemblyOut;
                data.FinPtdCostIssued = inItemCostHistData.FinPtdCostIssued;
                data.FinPtdCostReceived = inItemCostHistData.FinPtdCostReceived;
                data.FinPtdCostTransferIn = inItemCostHistData.FinPtdCostTransferIn;
                data.FinPtdCostTransferOut = inItemCostHistData.FinPtdCostTransferOut;
                data.FinPtdQtyAdjusted = inItemCostHistData.FinPtdQtyAdjusted;
                data.FinPtdQtyAssemblyIn = inItemCostHistData.FinPtdQtyAssemblyIn;
                data.FinPtdQtyAssemblyOut = inItemCostHistData.FinPtdQtyAssemblyOut;
                data.FinPtdQtyIssued = inItemCostHistData.FinPtdQtyIssued;
                data.FinPtdQtyReceived = inItemCostHistData.FinPtdQtyReceived;
                data.FinPtdQtySales = inItemCostHistData.FinPtdQtySales;
                data.FinPtdQtyTransferIn = inItemCostHistData.FinPtdQtyTransferIn;
                data.FinPtdQtyTransferOut = inItemCostHistData.FinPtdQtyTransferOut;
                data.FinYtdCost = inItemCostHistData.FinYtdCost;
                data.FinYtdQty = inItemCostHistData.FinYtdQty;
                data.FinPtdCreditMemos = inItemCostHistData.FinPtdCreditMemos;
                data.TotalCostIN = inItemCostHistData.FinBegCost + inItemCostHistData.FinPtdCOGSCredits + inItemCostHistData.FinPtdCostAdjusted + inItemCostHistData.FinPtdCostAssemblyIn + inItemCostHistData.FinPtdCostReceived + inItemCostHistData.FinPtdCreditMemos;
                data.TotalQtyIn = inItemCostHistData.FinPtdQtyReceived + inItemCostHistData.FinPtdQtyCreditMemos + inItemCostHistData.FinPtdQtyAssemblyIn + inItemCostHistData.FinBegQty + inItemCostHistData.FinPtdQtyAdjusted;
            }
            this.Actions.PressSave();
            return adapter.Get();
        }

    }
}
