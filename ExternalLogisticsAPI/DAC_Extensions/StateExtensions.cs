using PX.Data;

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
    }
}