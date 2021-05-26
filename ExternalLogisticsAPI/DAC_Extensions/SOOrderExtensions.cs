using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects.SO
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
        #region UsrDCLShipmentCreated

        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Shipment in DCL")]
        public bool? UsrDCLShipmentCreated { get;set; }
        public abstract class usrDCLShipmentCreated : PX.Data.BQL.BqlBool.Field<usrDCLShipmentCreated>{}

        #endregion
    }
}
