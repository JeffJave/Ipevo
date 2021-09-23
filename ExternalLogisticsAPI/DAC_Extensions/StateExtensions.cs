using PX.Data;
using System;

namespace PX.Objects.CS
{
    public class StateExt : PXCacheExtension<PX.Objects.CS.State>
    {
        #region UsrIsAMZWithheldTax
        [PXDBBool]
        [PXUIField(DisplayName = "AMZ Withheld Tax")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsAMZWithheldTax { get; set; }
        public abstract class usrIsAMZWithheldTax : PX.Data.BQL.BqlBool.Field<usrIsAMZWithheldTax> { }
        #endregion

        #region UsrFreightFactor
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "1.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public decimal? UsrFreightFactor { get; set; }
        public abstract class usrFreightFactor : PX.Data.BQL.BqlDecimal.Field<usrFreightFactor> { }
        #endregion
    }
}