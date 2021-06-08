using System;
using PX.Data;
using PX.Objects.CM;

namespace PX.Objects.SO
{
    public class SOOrderExt : PXCacheExtension<PX.Objects.SO.SOOrder>
    {
        // Jira IP-13
        #region UsrCurySubTot
        //[PXDecimal()]
        [PXCurrency(typeof(SOOrder.curyInfoID), typeof(SOOrderExt.usrCurySubTot))]
        [PXUIField(DisplayName = "Sub Total", IsReadOnly = true)]
        [PXFormula(typeof(Add<SOOrder.curyMiscTot, SOOrder.curyLineTotal>))]
        public virtual decimal? UsrCurySubTot { get; set; }
        public abstract class usrCurySubTot : PX.Data.BQL.BqlDecimal.Field<usrCurySubTot> { }
        #endregion
    }
}
