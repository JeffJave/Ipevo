using PX.Data;
using System;

namespace PX.Objects.CR.Standalone
{
    public class LocationExt : PXCacheExtension<PX.Objects.CR.Standalone.Location>
    {
        #region UsrTaxExemptCust
        [PXDBBool()]
        [PXUIField(DisplayName = "Tax Exempt Customer")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrTaxExemptCust { get; set; }
        public abstract class usrTaxExemptCust : PX.Data.BQL.BqlBool.Field<usrTaxExemptCust> { }
        #endregion
    }
}