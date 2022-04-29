using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using System.Collections;
using System.Linq;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects;
using PX.Objects.IN;
using LumSplitVarianceCost.DAC;

namespace PX.Objects.IN
{
    public class INUpdateStdCost_Extension : PXGraphExtension<INUpdateStdCost>
    {
        #region Event Handlers
        protected virtual void INSiteFilter_PendingStdCostDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var row = (INSiteFilter)e.Row;
            AccessInfo curAccessInfo = Base.Caches[typeof(AccessInfo)].Current as AccessInfo;

            bool EnableCreateAdjmOnLastDayInLastMonth = SelectFrom<LumSTDCostVarSetup>.View.Select(Base).TopFirst?.EnableCreateAdjmOnLastDayInLastMonth == true ? true : false;

            if (row.PendingStdCostDate != curAccessInfo.BusinessDate && !EnableCreateAdjmOnLastDayInLastMonth)
            {
                //Pop Up Message
                WebDialogResult result = Base.Filter.Ask(ActionsMessages.Warning, PXMessages.LocalizeFormatNoPrefix("重要提醒 : 請先設定畫面上方 Business Date 為標準成本生效日"), MessageButtons.OK);
            }
        }
        #endregion
    }
}