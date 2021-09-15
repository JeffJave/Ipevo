using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.SO;
using System.Collections;
using System.Collections.Generic;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Descripter;

namespace ExternalLogisticsAPI
{
    public class LUM3DCartImportProc : PXGraph<LUM3DCartImportProc>
    {
        #region Features        
        public PXCancel<SOOrderFilter> Cancel;
        public PXFilter<SOOrderFilter> Filter;
        public PXFilteredProcessing<LUM3DCartProcessOrder, SOOrderFilter, Where<LUM3DCartProcessOrder.processed, NotEqual<True>,
                                                                                And<LUM3DCartProcessOrder.orderDate, GreaterEqual<Current<SOOrderFilter.startDate>>,
                                                                                    And<LUM3DCartProcessOrder.orderDate, LessEqual<Current<SOOrderFilter.endDate>>>>>> ImportOrderList;
        #endregion

        #region Constructor
        public LUM3DCartImportProc()
        {
            this.Actions.Move(nameof(this.Cancel), nameof(this.PrepareRecords), true);

            SOOrderFilter currentFilter = this.Filter.Current;

            ImportOrderList.SetProcessCaption(PX.Objects.CR.Messages.Import);
            ImportOrderList.SetProcessAllCaption(PX.Objects.CR.Messages.Prepare + " & " + PX.Objects.CR.Messages.Import);
            ImportOrderList.SetProcessDelegate(
               delegate (List<LUM3DCartProcessOrder> list)
               {
                   ImportRecords(list, currentFilter);
               });
        }
        #endregion

        #region Overridden Properties
        // Add an overridden property for the graph IsDirty, keeping the page from trying to save changes.
        public override bool IsDirty => false;
        #endregion

        #region Action
        public PXAction<SOOrderFilter> PrepareRecords;
        [PXButton]
        [PXUIField(DisplayName = PX.Objects.CR.Messages.Prepare, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = true)]
        protected IEnumerable prepareRecords(PXAdapter adapter)
        {
            List<string> importedRecords = new List<string>();

            SOOrderFilter  curFilter = Filter.Current;
            LUM3DCartSetup curSetup = SelectFrom<LUM3DCartSetup>.View.SelectSingleBound(this, null);

            PXLongOperation.StartOperation(this, delegate ()
            {
                if (curFilter != null)
                {
                    ExternalAPIHelper.PrepareRecords(curSetup, curFilter.EndDate);
                }
            });

            return adapter.Get();
        }
        #endregion

        #region Static Methods
        public static void ImportRecords(List<LUM3DCartProcessOrder> list, SOOrderFilter currentFilter)
        {
            LUM3DCartImportProc graph = PXGraph.CreateInstance<LUM3DCartImportProc>();

            graph.ImportAllRecords(graph, list, currentFilter);
        }
        #endregion

        #region Methods
        public virtual void ImportAllRecords(LUM3DCartImportProc graph, List<LUM3DCartProcessOrder> list, SOOrderFilter curFilter)
        {
            if (list.Count < 0) { return; }

            LUM3DCartSetup curSetup = SelectFrom<LUM3DCartSetup>.View.SelectSingleBound(this, null);

            // Because I cannot get the number of records in the current cache, only use BQL select to get the number of records stored.
            int selectedCount = SelectFrom<LUM3DCartProcessOrder>.Where<LUM3DCartProcessOrder.processed.IsNotEqual<True>
                                                                        .And<LUM3DCartProcessOrder.orderDate.IsBetween<@P.AsDateTime, @P.AsDateTime>>>
                                                                 .View.Select(graph, curFilter.StartDate, curFilter.EndDate).Count;

            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                { 
                    if (curFilter != null)
                    {
                        if (selectedCount == list.Count)
                        {
                            graph.PrepareRecords.PressButton();
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            ExternalAPIHelper.ImportRecords(curSetup, list[i]);
                            UpdateProcessed(list[i]);
                        }
                    }

                    graph.Actions.PressSave();

                    ts.Complete();
                }
            });
        }

        public virtual void UpdateProcessed(LUM3DCartProcessOrder processOrder)
        {
            ImportOrderList.Cache.SetValue<LUM3DCartProcessOrder.processed>(processOrder, true);
            ImportOrderList.Update(processOrder);
        }
        #endregion
    }
    /*
    Rule 1: When user click Prepare, system call the 3DCart API to retrieve all orders with order status = NEW, and store into table 3DCartOrder.
    Rule 2: When user click Import, system call the 3DCart API basing on the orders users highlighted and create sales order in Acumatica.
    Rule 3: Once the sales order is created successfully, system call 3Dcart API to update the OrderStatusID in 3DCart, update 3DcartOrder.OrderStatusID, and update 3DcartOrder.Processed = True
    Rule 4: Highlight in yellow did not show on this screen
    Rule 5: Allow to batch run for the processes (Prepare ; Import ; Prepare & Import All)
    */
}