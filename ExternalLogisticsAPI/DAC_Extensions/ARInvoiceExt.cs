using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects.AR
{
    public class ARInvoiceExt : PXCacheExtension<ARInvoice>
    {
        #region UsrLOBSent
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Send to LOB Paper Invoice", Enabled = false)]
        public bool? UsrLOBSent { get; set; }
        public abstract class usrLOBSent : PX.Data.BQL.BqlBool.Field<usrLOBSent> { }
        #endregion
    }
}
