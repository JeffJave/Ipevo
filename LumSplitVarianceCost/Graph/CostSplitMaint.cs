using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LumSplitVarianceCost.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;

namespace LumSplitVarianceCost.Graph
{
    public class CostSplitMaint : PXGraph<CostSplitMaint>
    {
        public PXFilter<CostSplitFilter> MasterFilter;
        public SelectFrom<v_LumCostSplit>.Where<v_LumCostSplit.finPeriodID.IsEqual<CostSplitFilter.finPeriodID.FromCurrent>>.OrderBy<Asc<v_LumCostSplit.inventoryCD>>.View DetailsView;
        public CostSplitMaint()
        {
            DetailsView.AllowInsert = DetailsView.AllowDelete = false;
        }

        #region Delegate DataView
        public IEnumerable detailsView()
        {
            List<object> result = new List<object>();
            var periodID = ((CostSplitFilter)this.Caches[typeof(CostSplitFilter)].Current)?.FinPeriodID;
            if (periodID != null)
            {
                var varAcct = SelectFrom<Account>.Where<Account.accountID.IsEqual<@P.AsInt>>.View.Select(this, SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.VarAcctID).TopFirst?.AccountCD;
                if (varAcct == null) throw new PXException("Please set Variance Account in STD Cost Variance Account for Split");

                foreach (v_LumCostSplit line in SelectFrom<v_LumCostSplit>.
                                                Where<v_LumCostSplit.finPeriodID.IsEqual<@P.AsString>.
                                                And<v_LumCostSplit.costDiffer.IsEqual<@P.AsInt>.
                                                And<v_LumCostSplit.accountID.IsNotEqual<@P.AsInt>>>>.
                                                OrderBy<Asc<v_LumCostSplit.inventoryCD>>.View.Select(this, periodID, 1, SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.VarAcctID))
                {
                    result.Add(line);
                }

                return result;
            }
            return result;
        }
        #endregion

        #region Actions
        public PXAction<v_LumCostSplit> createSplitJournal;
        [PXButton()]
        [PXUIField(DisplayName = "Create Split Journal", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected virtual IEnumerable CreateSplitJournal(PXAdapter adapter)
        {
            var filterCurrnetCache = this.Caches[typeof(CostSplitFilter)].Current as CostSplitFilter;
            if (filterCurrnetCache.FinPeriodID == null) throw new PXException("Please enter dates.");
            //if (this.DetailsView.Select() == null || this.DetailsView.Select().Count(x => ((v_LumCostSplit)x).UsrSplited != true) == 0) throw new PXException("There is no data.");
            if (this.DetailsView.Select().Count(x => ((v_LumCostSplit)x).UsrSplited != true) == 0) throw new PXException("There is no data.");


            var varAcct = SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.VarAcctID;
            if (varAcct == null) throw new PXException("Please set Variance Account in STD Cost Variance Account for Split");
            var iNSubAcctID = SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.InvtSubID;
            if (iNSubAcctID == null) throw new PXException("Please set Sub Account in STD Cost Variance Account for Split");

            JournalEntry journalEntry = PXGraph.CreateInstance<JournalEntry>();
            Batch batch = journalEntry.BatchModule.Insert();
            DateTime firstDayInFinPeriod = DateTime.ParseExact(this.DetailsView.Select().TopFirst.FinPeriodID.ToString() + "01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            batch.DateEntered = firstDayInFinPeriod.AddMonths(1).AddDays(-firstDayInFinPeriod.AddMonths(1).Day);
            batch.Description = "當月單位成本差異重估";
            journalEntry.BatchModule.Cache.RaiseFieldUpdated<Batch.dateEntered>(batch, null);
            journalEntry.Actions.PressSave();

            //foreach (LumCostSplit line in curLumCostSplit)
            foreach (v_LumCostSplit line in this.DetailsView.Select())
            {
                if (line.UsrSplited != true)
                {
                    GLTran gLTran = new GLTran();
                    gLTran.AccountID = line.AccountID;
                    journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTran, null);
                    gLTran.SubID = line.SubID; //iNSubAcctID;
                    gLTran.InventoryID = line.InventoryID;
                    if (line.Qty < 0)
                    {
                        if (line.STDCostVariance < 0) gLTran.CuryCreditAmt = Math.Abs((decimal)line.STDCostVariance);
                        else gLTran.CuryDebitAmt = line.STDCostVariance;
                    }
                    else
                    {
                        if (line.STDCostVariance < 0) gLTran.CuryDebitAmt = Math.Abs((decimal)line.STDCostVariance);
                        else gLTran.CuryCreditAmt = line.STDCostVariance;
                    }
                    
                    gLTran = journalEntry.GLTranModuleBatNbr.Insert(gLTran);
                    journalEntry.Actions.PressSave();

                    //STD Cost Variance
                    GLTran gLTranVar = new GLTran();
                    gLTranVar.AccountID = varAcct;
                    journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTranVar, null);
                    gLTranVar.SubID = line.SubID; //iNSubAcctID;
                    gLTranVar.InventoryID = line.InventoryID;
                    if (line.Qty < 0)
                    {
                        if (line.STDCostVariance < 0) gLTranVar.CuryDebitAmt = Math.Abs((decimal)line.STDCostVariance);
                        else gLTranVar.CuryCreditAmt = line.STDCostVariance;
                    }
                    else
                    {
                        if (line.STDCostVariance < 0) gLTranVar.CuryCreditAmt = Math.Abs((decimal)line.STDCostVariance);
                        else gLTranVar.CuryDebitAmt = line.STDCostVariance;
                    }
                    
                    gLTranVar = journalEntry.GLTranModuleBatNbr.Insert(gLTranVar);
                    journalEntry.Actions.PressSave();

                    line.UsrSplited = true;
                    this.DetailsView.Update(line);
                    //this.Actions.PressSave();

                    //Update Batch UsrSplited
                    this.ProviderUpdate<Batch>(
                            new PXDataFieldAssign("UsrSplited", true),
                            new PXDataFieldRestrict("BatchNbr", line.BatchNbr),
                            new PXDataFieldRestrict("Module", line.Module));
                }
            }

            //Pop Up Message
            //this.DetailsView.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("Process Completed"), MessageButtons.OK);

            throw new PXException("Process Completed");

            //WebDialogResult result = this.DetailsView.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("Process Completed"), MessageButtons.OK);
            //if (result != WebDialogResult.OK) Redirector.Refresh(System.Web.HttpContext.Current); //refresh page

            return adapter.Get();
        }


        public PXAction<CostSplitFilter> viewDetails;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXEditDetailButton]
        public virtual IEnumerable ViewDetails(PXAdapter adapter)
        {
            if (this.DetailsView.Current != null && this.MasterFilter.Current != null)
            {
                v_LumCostSplit res = this.DetailsView.Current;
                CostSplitFilter currentFilter = this.MasterFilter.Current;

                AccountByPeriodEnq graph = PXGraph.CreateInstance<AccountByPeriodEnq>();
                AccountByPeriodFilter filter = graph.Filter.Current;
                filter.FinPeriodID = currentFilter.FinPeriodID;
                filter.EndPeriodID = currentFilter.FinPeriodID;

                filter.AccountID = res.AccountID;
                graph.Filter.Update(filter);
                filter = graph.Filter.Select();
                throw new PXRedirectRequiredException(graph, "Account Details");
            }
            return MasterFilter.Select();
        }

        public PXAction<CostSplitFilter> viewBatch;
        [PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(OnClosingPopup = PXSpecialButtonType.Refresh)]
        public virtual IEnumerable ViewBatch(PXAdapter adapter)
        {
            GLTranR tran = new GLTranR();
            tran.BatchNbr = this.DetailsView.Current.BatchNbr;
            tran.Module = this.DetailsView.Current.Module;

            if (tran != null)
            {
                Batch batch = JournalEntry.FindBatch(this, tran);

                if (batch != null)
                {
                    JournalEntry.RedirectToBatch(batch);
                }
            }

            return MasterFilter.Select();
        }
        #endregion

        #region CostSplit Filter
        [Serializable]
        [PXCacheName("Cost Split Filter")]
        public class CostSplitFilter : IBqlTable
        {
            #region FinPeriodID
            [FinPeriodID()]
            [PXSelector(typeof(Search4<INItemCostHist.finPeriodID, Where<INItemCostHist.finPeriodID.IsEqual<INItemCostHist.finPeriodID>>, Aggregate<GroupBy<INItemCostHist.finPeriodID>>, OrderBy<Desc<INItemCostHist.finPeriodID>>>),
                        typeof(INItemCostHist.finPeriodID))]
            [PXUIField(DisplayName = "FinPeriod", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string FinPeriodID { get; set; }
            public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
            #endregion
        }
        #endregion

    }
}