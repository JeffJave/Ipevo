using PX.Data;

namespace PX.Objects.TX
{
    public class TaxExt : PXCacheExtension<PX.Objects.TX.Tax>
    {
        #region UsrAMPropagateTaxAmt
        [PXDBBool]
        [PXUIField(DisplayName = "Propagate Manually Set Tax Amount From SO to Invoices")]
        public virtual bool? UsrAMPropagateTaxAmt { get; set; }
        public abstract class usrAMPropagateTaxAmt : PX.Data.BQL.BqlBool.Field<usrAMPropagateTaxAmt> { }
        #endregion
    }
}