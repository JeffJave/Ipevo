using System;
using LumSplitVarianceCost.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;

namespace PX.Objects.IN
{
    public class INUpdateStdCostProcess_Extension : PXGraphExtension<INUpdateStdCostProcess>
    {
        public delegate INRegister AddToAdjustmentDelegate(INCostStatus layer, decimal? tranCost);
        [PXOverride]
        public virtual INRegister AddToAdjustment(INCostStatus layer, decimal? tranCost, AddToAdjustmentDelegate baseMethod)
        {
            INRegister register = baseMethod(layer, tranCost);

            if (tranCost != 0m)
            {
                bool EnableCreateAdjmOnLastDayInLastMonth = SelectFrom<LumSTDCostVarSetup>.View.Select(Base).TopFirst?.EnableCreateAdjmOnLastDayInLastMonth == true ? true : false;
                if (EnableCreateAdjmOnLastDayInLastMonth)
                {
                    //Base.je.adjustment.SetValueExt<INRegister.tranDate>(register, Base.je.Accessinfo.BusinessDate.Value.AddDays(-Base.je.Accessinfo.BusinessDate.Value.Day));
                    Base.je.adjustment.SetValueExt<INRegister.tranDate>(register, SelectFrom<LumSTDCostVarSetup>.View.Select(Base).TopFirst?.CreateAdjmTranDate);
                    Base.je.adjustment.UpdateCurrent();

                    Base.je.transactions.SetValueExt<INTran.tranDate>(Base.je.transactions.Current, Base.je.adjustment.Current.TranDate);
                    Base.je.transactions.UpdateCurrent();

                    //Update flag in LUMSTDCostVarSetup table
                    //Base.ProviderUpdate<LumSTDCostVarSetup>(new PXDataFieldAssign("EnableCreateAdjmOnLastDayInLastMonth", false));
                }
            }
            return register;
        }
    }
}
