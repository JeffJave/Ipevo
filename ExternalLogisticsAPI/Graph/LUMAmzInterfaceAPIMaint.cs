using PX.Data;
using ExternalLogisticsAPI.DAC;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMAmzInterfaceAPIMaint : PXGraph<LUMAmzInterfaceAPIMaint>
    {
        public PXSave<LUMAmazonInterfaceAPI> Save;
        public PXCancel<LUMAmazonInterfaceAPI> Cancel;
        public PXProcessing<LUMAmazonInterfaceAPI> AMZInterfaceAPI; 

        public LUMAmzInterfaceAPIMaint()
        {
            AMZInterfaceAPI.Cache.AllowInsert = AMZInterfaceAPI.Cache.AllowUpdate = AMZInterfaceAPI.Cache.AllowDelete = true;
            
            //PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.orderType>(AMZInterfaceAPI.Cache, null, true);
            //PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.orderNbr>(AMZInterfaceAPI.Cache, null, true);
            //PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.sequenceNo>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.marketplace>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.data1>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.data2>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.write2Acumatica1>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.write2Acumatica2>(AMZInterfaceAPI.Cache, null, true);
        }
    }
}