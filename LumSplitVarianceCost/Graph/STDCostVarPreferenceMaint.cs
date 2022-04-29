using System;
using LumSplitVarianceCost.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;

namespace LumSplitVarianceCost.Graph
{
    public class STDCostVarPreferenceMaint : PXGraph<STDCostVarPreferenceMaint>
    {

        public PXSave<LumSTDCostVarSetup> Save;
        public PXCancel<LumSTDCostVarSetup> Cancel;

        public SelectFrom<LumSTDCostVarSetup>.View lumSTDCostVarSetupView;

    }
}