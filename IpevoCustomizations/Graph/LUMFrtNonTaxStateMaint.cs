using PX.Data;
using IpevoCustomizations.DAC;

namespace IpevoCustomizations.Graph
{
    public class LUMFrtNonTaxStateMaint : PXGraph<LUMFrtNonTaxStateMaint>
    {
        public PXSave<LUMFreightNonTaxState> Save;
        public PXCancel<LUMFreightNonTaxState> Cancel;

        [PXImport(typeof(LUMFreightNonTaxState))]
        public PXSelect<LUMFreightNonTaxState> FreightNonTaxState;
    }
}