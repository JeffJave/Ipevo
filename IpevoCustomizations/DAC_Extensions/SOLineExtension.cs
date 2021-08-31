using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class SOLineExtension : PXCacheExtension<SOLine>
    {
        #region UsrMSRPDiscountRate
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "MSRP Discount Rate")]
        public virtual decimal? UsrMSRPDiscountRate { get; set; }
        public abstract class usrMSRPDiscountRate : PX.Data.BQL.BqlDecimal.Field<usrMSRPDiscountRate> { }
        #endregion
    }
}
