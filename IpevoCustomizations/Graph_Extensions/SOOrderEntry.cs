using System;
using PX.Data;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
    {
        // Jira IP-13
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXFormula(typeof(Add<SOOrder.curyMiscTot, SOOrder.curyLineTotal>))]
        protected void _(Events.CacheAttached<SOOrder.curyLineTotal> e) { }
    }
}
