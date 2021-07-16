using IpevoCustomizations.DAC;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpevoCustomizations.Graph
{
    public class LUMRestockNonTaxStateMaint : PXGraph<LUMRestockNonTaxStateMaint>
    {
        public PXSave<LUMRestockNonTaxState> Save;
        public PXCancel<LUMRestockNonTaxState> Cancel;

        [PXImport(typeof(LUMRestockNonTaxState))]
        public PXSelect<LUMRestockNonTaxState> RestockNonTaxState;
    }
}
