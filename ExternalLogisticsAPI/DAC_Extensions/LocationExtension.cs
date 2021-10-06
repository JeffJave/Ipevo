using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.CR.Standalone
{
    public class LocationExt : PXCacheExtension<Location>
    {
        #region UsrVATIsValid
        [PXDBBool]
        [PXUIField(DisplayName = "Validated", Enabled = false)]
        public bool? UsrVATIsValid { get; set; }
        public abstract class usrVATIsValid : PX.Data.BQL.BqlBool.Field<usrVATIsValid> { }
        #endregion
    }
}
