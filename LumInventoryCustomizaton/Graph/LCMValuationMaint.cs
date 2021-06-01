using System;
using System.Collections;
using System.Collections.Generic;
using LumInventoryCustomizaton.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.IN;

namespace LumInventoryCustomizaton.Graph
{
    public class LCMValuationMaint : PXGraph<LCMValuationMaint>
    {
        public PXFilter<LCMValuationFilter> MasterViewFilter;
        public SelectFrom<LCMValuation>.View DetailsView;

        public LCMValuationMaint()
        {
            DetailsView.AllowInsert = DetailsView.AllowUpdate = DetailsView.AllowDelete = false;
        }

        #region LCMValuation Filter
        [Serializable]
        [PXCacheName("LCM Valuation Filter")]
        public class LCMValuationFilter : IBqlTable
        {
            #region FinPeriodID
            [FinPeriodID()]
            [PXSelector(typeof(Search4<INItemCostHist.finPeriodID, Where<INItemCostHist.finPeriodID.IsEqual<INItemCostHist.finPeriodID>>, Aggregate<GroupBy<INItemCostHist.finPeriodID>>, OrderBy<Desc<INItemCostHist.finPeriodID>>>),
                        typeof(INItemCostHist.finPeriodID))]
            [PXUIField(DisplayName = "Period ID", Visibility = PXUIVisibility.SelectorVisible)]
            public virtual string FinPeriodID { get; set; }
            public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
            #endregion
        }
        #endregion
        
        protected virtual void LCMValuationFilter_FinPeriodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = ((DateTime)Accessinfo.BusinessDate).AddMonths(-1).ToString("MMyyyy");
            detailsView();
        }
        
        #region Delegate DataView
        public IEnumerable detailsView()
        {
            //var lastFinPeriod = DateTime.Now.AddMonths(-1).ToString("yyyyMM");

            var filter = MasterViewFilter.Current?.FinPeriodID;
            if (filter == null)
                filter = ((DateTime)Accessinfo.BusinessDate).AddMonths(-1).ToString("yyyyMM");

            PXSelectBase<LCMValuation> command = new SelectFrom<LCMValuation>.View.ReadOnly(this);

            var result = new PXDelegateResult();
            foreach (PXResult<LCMValuation> row in command.Select())
            {
                if (((LCMValuation)row).FinPeriodID == filter)
                    result.Add(row);
            }

            return result;
        }
        #endregion
    }
}