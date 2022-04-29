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

namespace LumSplitVarianceCost.Graph
{
    public class STDCostVarianceSplitMaint : PXGraph<STDCostVarianceSplitMaint>
    {
        public PXFilter<STDCostVarianceSplitFilter> MasterFilter;
        public SelectFrom<LumSTDCostVarSplit>.Where<LumSTDCostVarSplit.finPeriodID.IsGreaterEqual<STDCostVarianceSplitFilter.fromFinPeriodID.FromCurrent>.
                                                And<LumSTDCostVarSplit.finPeriodID.IsLessEqual<STDCostVarianceSplitFilter.toFinPeriodID.FromCurrent>>>.OrderBy<Asc<LumSTDCostVarSplit.inventoryCD>>.View DetailsView;

        public SelectFrom<LumSTDCostVarSplitHistory>.Where<LumSTDCostVarSplitHistory.finPeriodID.IsEqual<STDCostVarianceSplitFilter.toFinPeriodID.FromCurrent>>.OrderBy<Asc<LumSTDCostVarSplitHistory.inventoryCD>>.View HistoryDetailsView;

        public STDCostVarianceSplitMaint()
        {
            DetailsView.AllowInsert = DetailsView.AllowDelete = false;
        }

        #region Delegate DataView
        public IEnumerable detailsView()
        {
            List<object> result = new List<object>();
            var fromPeriodID = ((STDCostVarianceSplitFilter)this.Caches[typeof(STDCostVarianceSplitFilter)].Current)?.FromFinPeriodID;
            var toPeriodID = ((STDCostVarianceSplitFilter)this.Caches[typeof(STDCostVarianceSplitFilter)].Current)?.ToFinPeriodID;
            if (fromPeriodID != null && toPeriodID != null)
            {
                var curLumSTDCostVarSplitHistory = SelectFrom<LumSTDCostVarSplitHistory>.Where<LumSTDCostVarSplitHistory.finPeriodID.IsEqual<@P.AsString>>.View.Select(this, toPeriodID);
                if (curLumSTDCostVarSplitHistory != null && curLumSTDCostVarSplitHistory.Count > 0)
                {
                    foreach (LumSTDCostVarSplitHistory data in curLumSTDCostVarSplitHistory)
                    {
                        LumSTDCostVarSplit lumSTDCostVarSplit = new LumSTDCostVarSplit();
                        lumSTDCostVarSplit.FinPeriodID = data.FinPeriodID;
                        lumSTDCostVarSplit.AccountID = data.AccountID;
                        lumSTDCostVarSplit.AccountCD = data.AccountCD;
                        lumSTDCostVarSplit.GLTranType = data.GLTranType;
                        lumSTDCostVarSplit.AccountDescription = data.AccountDescription;
                        lumSTDCostVarSplit.InventoryID = data.InventoryID;
                        lumSTDCostVarSplit.InventoryCD = data.InventoryCD;
                        lumSTDCostVarSplit.InventoryDescr = data.InventoryDescr;
                        lumSTDCostVarSplit.KitInventoryID = data.KitInventoryID;
                        lumSTDCostVarSplit.KitInventoryCD = data.KitInventoryCD;
                        lumSTDCostVarSplit.KitInventoryDescr = data.KitInventoryDescr;
                        lumSTDCostVarSplit.Qty = data.Qty;
                        lumSTDCostVarSplit.SplitQty = data.SplitQty;
                        lumSTDCostVarSplit.Split = data.Split;
                        lumSTDCostVarSplit.VarianceCost = data.VarianceCost;
                        lumSTDCostVarSplit.SplitCost = data.SplitCost;
                        lumSTDCostVarSplit.STDCost = data.Stdcost;
                        lumSTDCostVarSplit.INCost = data.Incost;
                        lumSTDCostVarSplit.NewSTDCost = data.Newstdcost;
                        lumSTDCostVarSplit.SubID = data.Subid;
                        result.Add(lumSTDCostVarSplit);
                    }
                    setButtonVisible();
                    buttonEnable(false);
                    return result;
                }


                var pars = new List<PXSPParameter>();
                PXSPParameter p0 = new PXSPInParameter("@P0", PXDbType.Char, fromPeriodID);
                PXSPParameter p1 = new PXSPInParameter("@P1", PXDbType.Char, toPeriodID);
                PXSPParameter companyID = new PXSPInParameter("@companyID", PXDbType.Int, PX.Data.Update.PXInstanceHelper.CurrentCompany);
                var varAcct = SelectFrom<Account>.Where<Account.accountID.IsEqual<@P.AsInt>>.View.Select(this, SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.VarAcctID).TopFirst?.AccountCD;
                if (varAcct == null) throw new PXException("Please set Variance Account in STD Cost Variance Account for Split");
                PXSPParameter stCostVarAcct = new PXSPInParameter("@StCostVarAcct", varAcct);
                var iNAcct = SelectFrom<Account>.Where<Account.accountID.IsEqual<@P.AsInt>>.View.Select(this, SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.InvtAcctID).TopFirst?.AccountCD;
                if (iNAcct == null) throw new PXException("Please set Account in STD Cost Variance Account for Split");
                PXSPParameter iNAccrualAcct = new PXSPInParameter("@INAccrualAcct", PXDbType.NVarChar, iNAcct);
                PXSPParameter keepKitRecordOnly = new PXSPInParameter("@KeepKitRecordOnly", PXDbType.Bit, 0);
                pars.Add(p0);
                pars.Add(p1);
                pars.Add(companyID);
                pars.Add(stCostVarAcct);
                pars.Add(iNAccrualAcct);
                pars.Add(keepKitRecordOnly);

                using (new PXConnectionScope())
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        PXDatabase.Execute("SP_GenerateLumSplitVarianceCost", pars.ToArray());
                        ts.Complete();
                    }
                }

                PXView select = new PXView(this, true, DetailsView.View.BqlSelect);
                int totalrow = 0;
                int startrow = PXView.StartRow;
                result = select.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startrow, PXView.MaximumRows, ref totalrow);
                PXView.StartRow = 0;
                return result;
            }
            return result;
        }
        #endregion

        protected void buttonEnable(bool switchBool)
        {
            bool turnswitchBool = switchBool == true ? false : true;

            saveAndFrozen.SetEnabled(switchBool);
            deleteAndRetrieve.SetEnabled(turnswitchBool);
            createSplitJournal.SetEnabled(turnswitchBool);
            setSTDPendingCost.SetEnabled(turnswitchBool);
        }

        protected void setButtonVisible()
        {
            deleteAndRetrieve.SetVisible(true);
            createSplitJournal.SetVisible(true);
            setSTDPendingCost.SetVisible(true);
        }

        private void insertLumSTDCostVarSplitHistory()
        {
            this.Actions.PressSave();
            var curLumSTDCostVarSplit = SelectFrom<LumSTDCostVarSplit>.View.Select(this);
            if (curLumSTDCostVarSplit.Count == 0) throw new PXException("There is no data.");

            var conditionPeriod = curLumSTDCostVarSplit.TopFirst.FinPeriodID;
            var curLumSTDCostVarSplitHistory = SelectFrom<LumSTDCostVarSplitHistory>.Where<LumSTDCostVarSplitHistory.finPeriodID.IsEqual<@P.AsString>>.View.Select(this, conditionPeriod);
            
            if (curLumSTDCostVarSplitHistory.TopFirst?.LineNbr == null)
            {
                foreach (LumSTDCostVarSplit line in curLumSTDCostVarSplit.OrderBy((x => ((LumSTDCostVarSplit)x).InventoryCD)))
                {
                    this.ProviderInsert<LumSTDCostVarSplitHistory>(
                        new PXDataFieldAssign("FinPeriodID", line.FinPeriodID),
                        new PXDataFieldAssign("AccountID", line.AccountID),
                        new PXDataFieldAssign("AccountCD", line.AccountCD),
                        new PXDataFieldAssign("AccountDescription", line.AccountDescription),
                        new PXDataFieldAssign("InventoryID", line.InventoryID),
                        new PXDataFieldAssign("InventoryCD", line.InventoryCD),
                        new PXDataFieldAssign("InventoryDescr", line.InventoryDescr),
                        new PXDataFieldAssign("Qty", line.Qty),
                        new PXDataFieldAssign("SplitQty", line.SplitQty),
                        new PXDataFieldAssign("Split", line.Split),
                        new PXDataFieldAssign("VarianceCost", line.VarianceCost),
                        new PXDataFieldAssign("SplitCost", line.SplitCost),
                        new PXDataFieldAssign("STDCost", line.STDCost),
                        new PXDataFieldAssign("INCost", line.INCost),
                        new PXDataFieldAssign("NewSTDCost", line.NewSTDCost),
                        new PXDataFieldAssign("SubID", line.SubID),
                        new PXDataFieldAssign("GLTranType", line.GLTranType),
                        new PXDataFieldAssign("KitInventoryID", line?.KitInventoryID),
                        new PXDataFieldAssign("KitInventoryCD", line?.KitInventoryCD),
                        new PXDataFieldAssign("KitInventoryDescr", line?.KitInventoryDescr)
                    );
                }
            }
        }

        private void deleteLumSTDCostVarSplitHisByFinPeriodID(string finPeriodID)
        {
            this.ProviderDelete<LumSTDCostVarSplitHistory>(
                new PXDataFieldRestrict("FinPeriodID", finPeriodID));
        }

        #region Actions

        public PXAction<STDCostVarianceSplitFilter> saveAndFrozen;
        [PXButton()]
        [PXUIField(DisplayName = "Save & Frozen", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = false)]
        protected virtual IEnumerable SaveAndFrozen(PXAdapter adapter)
        {
            var filterCurrnetCache = this.Caches[typeof(STDCostVarianceSplitFilter)].Current as STDCostVarianceSplitFilter;
            if (filterCurrnetCache.FromFinPeriodID == null || filterCurrnetCache.ToFinPeriodID == null) throw new PXException("Please enter period.");

            this.Actions.PressSave();

            insertLumSTDCostVarSplitHistory();
            setButtonVisible();
            buttonEnable(false);

            return adapter.Get();
        }

        public PXAction<STDCostVarianceSplitFilter> deleteAndRetrieve;
        [PXButton()]
        [PXUIField(DisplayName = "Delete & Retrieve", MapEnableRights = PXCacheRights.Delete, MapViewRights = PXCacheRights.Delete, Visible = false, Enabled = false)]
        protected virtual IEnumerable DeleteAndRetrieve(PXAdapter adapter)
        {
            //Pop Up Message
            WebDialogResult result = this.DetailsView.View.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("Delete it or no?"), MessageButtons.YesNo);
            //checking answer	
            if (result != WebDialogResult.Yes) return adapter.Get();

            //Delete History Data
            string finPeriodID = ((LumSTDCostVarSplit)this.Caches[typeof(LumSTDCostVarSplit)].Current)?.FinPeriodID;
            deleteLumSTDCostVarSplitHisByFinPeriodID(finPeriodID);

            buttonEnable(true);
            //refresh page
            Redirector.Refresh(System.Web.HttpContext.Current);

            return adapter.Get();
        }

        public PXAction<STDCostVarianceSplitFilter> createSplitJournal;
        [PXButton()]
        [PXUIField(DisplayName = "Create Split Journal", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false, Enabled = false)]
        protected virtual IEnumerable CreateSplitJournal(PXAdapter adapter)
        {
            //this.Actions.PressSave();
            var filterCurrnetCache = this.Caches[typeof(STDCostVarianceSplitFilter)].Current as STDCostVarianceSplitFilter;
            if (filterCurrnetCache.FromFinPeriodID == null || filterCurrnetCache.ToFinPeriodID == null) throw new PXException("Please enter dates.");
            var curLumSTDCostVarSplitHistory = SelectFrom<LumSTDCostVarSplitHistory>.Where<LumSTDCostVarSplitHistory.finPeriodID.IsEqual<@P.AsString>>.View.Select(this, filterCurrnetCache.ToFinPeriodID);

            if (curLumSTDCostVarSplitHistory.TopFirst?.LineNbr != null)
            {
                var varAcct = SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.VarAcctID;
                if (varAcct == null) throw new PXException("Please set Variance Account in STD Cost Variance Account for Split");
                var iNAcct = SelectFrom<Account>.Where<Account.accountID.IsEqual<@P.AsInt>>.View.Select(this, SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.InvtAcctID).TopFirst?.AccountCD;
                if (iNAcct == null) throw new PXException("Please set Account in STD Cost Variance Account for Split");
                var iNSubAcctID = SelectFrom<LumSTDCostVarSetup>.View.Select(this).TopFirst?.InvtSubID;
                if (iNSubAcctID == null) throw new PXException("Please set Sub Account in STD Cost Variance Account for Split");

                JournalEntry journalEntry = PXGraph.CreateInstance<JournalEntry>();
                Batch batch = journalEntry.BatchModule.Insert();
                DateTime firstDayInFinPeriod = DateTime.ParseExact(curLumSTDCostVarSplitHistory.TopFirst.FinPeriodID.ToString() + "01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                batch.DateEntered = firstDayInFinPeriod.AddMonths(1).AddDays(-firstDayInFinPeriod.AddMonths(1).Day);
                batch.Description = "標準成本差異分攤";
                journalEntry.BatchModule.Cache.RaiseFieldUpdated<Batch.dateEntered>(batch, null);
                journalEntry.Actions.PressSave();

                foreach (LumSTDCostVarSplitHistory line in curLumSTDCostVarSplitHistory.Where(x => ((LumSTDCostVarSplitHistory)x).SplitCost != 0m))
                {
                    if (line.AccountCD != iNAcct)
                    {
                        GLTran gLTran = new GLTran();
                        gLTran.AccountID = line.AccountID;
                        journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTran, null);
                        gLTran.SubID = line.Subid; //iNSubAcctID;
                        gLTran.InventoryID = line.InventoryID;
                        if (line.SplitCost < 0) gLTran.CuryCreditAmt = Math.Abs((decimal)line.SplitCost);
                        else gLTran.CuryDebitAmt = line.SplitCost;

                        gLTran = journalEntry.GLTranModuleBatNbr.Insert(gLTran);
                        journalEntry.Actions.PressSave();

                        //STD Cost Variance
                        GLTran gLTranVar = new GLTran();
                        gLTranVar.AccountID = varAcct;
                        journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTranVar, null);
                        gLTranVar.SubID = line.Subid; //iNSubAcctID;
                        gLTranVar.InventoryID = line.InventoryID;
                        if (line.SplitCost < 0) gLTranVar.CuryDebitAmt = Math.Abs((decimal)line.SplitCost);
                        else gLTranVar.CuryCreditAmt = line.SplitCost;

                        gLTranVar = journalEntry.GLTranModuleBatNbr.Insert(gLTranVar);
                        journalEntry.Actions.PressSave();
                    }

                    //KIT Assembly
                    if (line.KitInventoryID != null)
                    {
                        GLTran gLTran = new GLTran();
                        gLTran.AccountID = varAcct;
                        journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTran, null);
                        gLTran.SubID = line.Subid; //iNSubAcctID;
                        gLTran.InventoryID = line.KitInventoryID;
                        if (line.SplitCost < 0) gLTran.CuryCreditAmt = Math.Abs((decimal)line.SplitCost);
                        else gLTran.CuryDebitAmt = line.SplitCost;

                        gLTran = journalEntry.GLTranModuleBatNbr.Insert(gLTran);
                        journalEntry.Actions.PressSave();

                        //STD Cost Variance
                        GLTran gLTranVar = new GLTran();
                        gLTranVar.AccountID = varAcct;
                        journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTranVar, null);
                        gLTranVar.SubID = line.Subid; //iNSubAcctID;
                        gLTranVar.InventoryID = line.InventoryID;
                        if (line.SplitCost < 0) gLTranVar.CuryDebitAmt = Math.Abs((decimal)line.SplitCost);
                        else gLTranVar.CuryCreditAmt = line.SplitCost;

                        gLTranVar = journalEntry.GLTranModuleBatNbr.Insert(gLTranVar);
                        journalEntry.Actions.PressSave();
                    }
                }
            }
            else throw new PXException("Please press Save & Frozen first.");

            //Pop Up Message
            //this.DetailsView.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("Process Completed"), MessageButtons.OK);

            throw new PXException("Process Completed");

            /*
            var curLumSTDCostVarSplit = SelectFrom<LumSTDCostVarSplit>.View.Select(this);
            if (curLumSTDCostVarSplit.Count() == 0) throw new PXException("There is no data.");

            JournalEntry journalEntry = PXGraph.CreateInstance<JournalEntry>();
            Batch batch = journalEntry.BatchModule.Insert();
            DateTime firstDayInFinPeriod = DateTime.ParseExact(curLumSTDCostVarSplit.TopFirst.FinPeriodID.ToString() + "01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            batch.DateEntered = firstDayInFinPeriod.AddMonths(1).AddDays(-firstDayInFinPeriod.AddMonths(1).Day);
            journalEntry.BatchModule.Cache.RaiseFieldUpdated<Batch.dateEntered>(batch, null);
            journalEntry.Actions.PressSave();

            foreach (LumSTDCostVarSplit line in curLumSTDCostVarSplit)
            {
                if (line.AccountCD != iNAcct && line.SplitCost != 0m)
                {
                    GLTran gLTran = new GLTran();
                    gLTran.AccountID = line.AccountID;
                    journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTran, null);
                    gLTran.SubID = line.SubID; //iNSubAcctID;
                    gLTran.InventoryID = line.InventoryID;
                    if (line.SplitCost < 0) gLTran.CuryCreditAmt = Math.Abs((decimal)line.SplitCost);
                    else gLTran.CuryDebitAmt = line.SplitCost;

                    gLTran = journalEntry.GLTranModuleBatNbr.Insert(gLTran);
                    journalEntry.Actions.PressSave();

                    //STD Cost Variance
                    GLTran gLTranVar = new GLTran();
                    gLTranVar.AccountID = varAcct;
                    journalEntry.GLTranModuleBatNbr.Cache.RaiseFieldUpdated<GLTran.accountID>(gLTranVar, null);
                    gLTranVar.SubID = line.SubID; //iNSubAcctID;
                    gLTranVar.InventoryID = line.InventoryID;
                    if (line.SplitCost < 0) gLTranVar.CuryDebitAmt = Math.Abs((decimal)line.SplitCost);
                    else gLTranVar.CuryCreditAmt = line.SplitCost;

                    gLTranVar = journalEntry.GLTranModuleBatNbr.Insert(gLTranVar);
                    journalEntry.Actions.PressSave();
                }
            }
            */


            return adapter.Get();
        }

        public PXAction<STDCostVarianceSplitFilter> setSTDPendingCost;
        [PXButton()]
        [PXUIField(DisplayName = "Set STD Pending Cost", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false, Enabled = false)]
        protected virtual IEnumerable SetSTDPendingCost(PXAdapter adapter)
        {
            var filterCurrnetCache = this.Caches[typeof(STDCostVarianceSplitFilter)].Current as STDCostVarianceSplitFilter;
            if (filterCurrnetCache.FromFinPeriodID == null || filterCurrnetCache.ToFinPeriodID == null) throw new PXException("Please enter dates.");
            var curLumSTDCostVarSplitHistory = SelectFrom<LumSTDCostVarSplitHistory>.Where<LumSTDCostVarSplitHistory.finPeriodID.IsEqual<@P.AsString>>.View.Select(this, filterCurrnetCache.ToFinPeriodID);
            if (curLumSTDCostVarSplitHistory.Count == 0) throw new PXException("There is no data.");

            //Dictionary for Cost Process
            Dictionary<int?, string> dicInventory = new Dictionary<int?, string>();

            //First Adjustment - SplitCost / on hold Qty + STD Cost = New STD Cost, last day in ToPeriod 
            DateTime lastDayInFinPeriod = DateTime.ParseExact(curLumSTDCostVarSplitHistory.TopFirst.FinPeriodID.ToString() + "01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddMonths(1).AddDays(-1);
            foreach (LumSTDCostVarSplitHistory line in curLumSTDCostVarSplitHistory)
            {
                if (line.Newstdcost != null && line.SplitCost != 0m)
                {
                    if (!dicInventory.ContainsKey(line.InventoryID)) dicInventory.Add(line.InventoryID, line.InventoryCD.Trim());

                    //Current Qty On hold
                    var curItem = SelectFrom<INSiteStatus>.Where<INSiteStatus.inventoryID.IsEqual<@P.AsInt>>.View.Select(this, line.InventoryID).RowCast<INSiteStatus>().ToList();

                    InventoryItemMaint inventoryItemMaint = PXGraph.CreateInstance<InventoryItemMaint>();
                    InventoryItem inventoryItem = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(inventoryItemMaint, line.InventoryID);
                    inventoryItem.PendingStdCost = line.Stdcost + (line.SplitCost / curItem.Sum(x => x.QtyOnHand));
                    inventoryItem.PendingStdCostDate = lastDayInFinPeriod;
                    inventoryItemMaint.Item.Cache.Update(inventoryItem);
                    inventoryItemMaint.Actions.PressSave();
                }
            }
            setSTDPendingCost.SetEnabled(false);

            //Update flag in LUMSTDCostVarSetup table
            this.ProviderUpdate<LumSTDCostVarSetup>(new PXDataFieldAssign("EnableCreateAdjmOnLastDayInLastMonth", true), new PXDataFieldAssign("CreateAdjmTranDate", lastDayInFinPeriod));

            //Enter Update STD Cost Process
            INUpdateStdCost iNUpdateStdCost = PXGraph.CreateInstance<INUpdateStdCost>();
            iNUpdateStdCost.Filter.Current.PendingStdCostDate = lastDayInFinPeriod;

            //INUpdateStdCostProcess iNUpdateStdCostProcess = PXGraph.CreateInstance<INUpdateStdCostProcess>();
            foreach (var row in iNUpdateStdCost.INItemList.Select())
            {
                if (dicInventory.ContainsKey(((INUpdateStdCostRecord)row).InventoryID))
                {
                    INUpdateStdCostRecord record = row;
                    record.Selected = true;
                    iNUpdateStdCost.INItemList.Cache.Update(row);

                    INUpdateStdCostProcess iNUpdateStdCostProcess = PXGraph.CreateInstance<INUpdateStdCostProcess>();
                    iNUpdateStdCostProcess.UpdateStdCost(row);
                    iNUpdateStdCostProcess.ReleaseAdjustment();

                    dicInventory.Remove(((INUpdateStdCostRecord)row).InventoryID);
                    //INUpdateStdCost.UpdateStdCost(iNUpdateStdCostProcess, row);
                    //INUpdateStdCost.ReleaseAdjustment(iNUpdateStdCostProcess);
                    //iNUpdateStdCostProcess.UpdateStdCost(row);
                }
            }

            //Update flag in LUMSTDCostVarSetup table
            this.ProviderUpdate<LumSTDCostVarSetup>(new PXDataFieldAssign("EnableCreateAdjmOnLastDayInLastMonth", false));

            //var tt = iNUpdateStdCost.INItemList.Select();

            //INUpdateStdCostProcess iNUpdateStdCostProcess = PXGraph.CreateInstance<INUpdateStdCostProcess>();
            //iNUpdateStdCostProcess.UpdateStdCost(iNUpdateStdCost.INItemList.Select());
            //iNUpdateStdCostProcess.ReleaseAdjustment();

            //INUpdateStdCost.UpdateStdCost(iNUpdateStdCostProcess, iNUpdateStdCost.INItemList.Select());
            //INUpdateStdCost.ReleaseAdjustment(iNUpdateStdCostProcess);

            //Second Adjustment - New STD Cost, Fisrt day in the next ToPeriod
            DateTime firstDayInNextFinPeriod = DateTime.ParseExact(curLumSTDCostVarSplitHistory.TopFirst.FinPeriodID.ToString() + "01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddMonths(1);
            foreach (LumSTDCostVarSplitHistory line in curLumSTDCostVarSplitHistory)
            {
                if (line.Newstdcost != null && line.SplitCost != 0m)
                {
                    if (!dicInventory.ContainsKey(line.InventoryID)) dicInventory.Add(line.InventoryID, line.InventoryCD.Trim());

                    InventoryItemMaint inventoryItemMaint = PXGraph.CreateInstance<InventoryItemMaint>();
                    InventoryItem inventoryItem = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(inventoryItemMaint, line.InventoryID);
                    inventoryItem.PendingStdCost = line.Newstdcost;
                    inventoryItem.PendingStdCostDate = firstDayInNextFinPeriod;
                    inventoryItemMaint.Item.Cache.Update(inventoryItem);
                    inventoryItemMaint.Actions.PressSave();
                }
            }


            //Pop Up Message
            //this.DetailsView.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("Update Pending Cost Completed"), MessageButtons.OK);
            throw new PXException("Process Completed");

            return adapter.Get();
        }
        #endregion

        #region STDCostVarianceSplit Filter
        [Serializable]
        [PXCacheName("STD Cost Variance Split Filter")]
        public class STDCostVarianceSplitFilter : IBqlTable
        {
            #region FromFinPeriodID
            [FinPeriodID()]
            [PXSelector(typeof(Search4<INItemCostHist.finPeriodID, Where<INItemCostHist.finPeriodID.IsEqual<INItemCostHist.finPeriodID>>, Aggregate<GroupBy<INItemCostHist.finPeriodID>>, OrderBy<Desc<INItemCostHist.finPeriodID>>>),
                        typeof(INItemCostHist.finPeriodID))]
            [PXUIField(DisplayName = "From Period", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string FromFinPeriodID { get; set; }
            public abstract class fromFinPeriodID : PX.Data.BQL.BqlString.Field<fromFinPeriodID> { }
            #endregion

            #region ToFinPeriodID
            [FinPeriodID()]
            [PXSelector(typeof(Search4<INItemCostHist.finPeriodID, Where<INItemCostHist.finPeriodID.IsEqual<INItemCostHist.finPeriodID>>, Aggregate<GroupBy<INItemCostHist.finPeriodID>>, OrderBy<Desc<INItemCostHist.finPeriodID>>>),
                        typeof(INItemCostHist.finPeriodID))]
            [PXUIField(DisplayName = "To Period", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string ToFinPeriodID { get; set; }
            public abstract class toFinPeriodID : PX.Data.BQL.BqlString.Field<toFinPeriodID> { }
            #endregion
        }
        #endregion

    }
}