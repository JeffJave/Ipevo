using PX.Data;
using ExternalLogisticsAPI.Descripter;

namespace PX.Objects.SO
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
        #region UsrDCLShipmentCreated

        [PXDBBool]
        [PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Shipment in DCL")]
        public bool? UsrDCLShipmentCreated { get;set; }
        public abstract class usrDCLShipmentCreated : 
            PX.Data.BQL.BqlBool.Field<usrDCLShipmentCreated>{}

        #endregion

        #region UsrSendToMiddleware

        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Send to Middle Ware",Enabled = false)]
        public bool? UsrSendToMiddleware { get; set; }
        public abstract class usrSendToMiddleware :
            PX.Data.BQL.BqlBool.Field<usrSendToMiddleware>{ }

        #endregion

        #region UsrAPIOrderType
        [PXDBInt]
        [PXUIField(DisplayName = "API Order Type", IsReadOnly = true)]
        [AmazonOrderType.List]
        public virtual int? UsrAPIOrderType { get; set; }
        public abstract class usrAPIOrderType : PX.Data.BQL.BqlInt.Field<usrAPIOrderType> { }
        #endregion
    }
}
