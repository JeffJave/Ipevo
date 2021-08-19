using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects.SO
{
    public class SOShipmentExt : PXCacheExtension<SOShipment>
    {
        #region UsrSendToWareHouse

        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Send to WareHouse", Enabled = false)]
        public bool? UsrSendToWareHouse { get; set; }
        public abstract class usrSendToWareHouse :
            PX.Data.BQL.BqlBool.Field<usrSendToWareHouse>
        { }

        #endregion

        #region UsrTrackingNbr

        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "TrackingNbr.")]
        public string UsrTrackingNbr { get; set; }
        public abstract class usrTrackingNbr : PX.Data.BQL.BqlString.Field<usrTrackingNbr> { }

        #endregion

        #region UsrCarrier

        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier.")]
        public string UsrCarrier { get; set; }
        public abstract class usrCarrier : PX.Data.BQL.BqlString.Field<usrCarrier> { }

        #endregion

    }
}
