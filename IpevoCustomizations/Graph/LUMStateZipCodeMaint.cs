using PX.Data;
using IpevoCustomizations.DAC;

namespace IpevoCustomizations.Graph
{
    public class LUMStateZipCodeMaint : PXGraph<LUMStateZipCodeMaint>
    {
        public PXSave<LUMStateZipCode> Save;
        public PXCancel<LUMStateZipCode> Cancel;

        [PXImport(typeof(LUMStateZipCode))]
        public PXSelect<LUMStateZipCode> StateZipCode;
    }
}