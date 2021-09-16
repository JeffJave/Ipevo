using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLogisticsAPI.DAC_Extensions
{
    public class StateExtension : PXCacheExtension<State>
    {
        #region UsrFreightFactor
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "1.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public decimal? UsrFreightFactor { get; set; }
        public abstract class usrFreightFactor : PX.Data.BQL.BqlDecimal.Field<usrFreightFactor> { }
        #endregion
    }
}
