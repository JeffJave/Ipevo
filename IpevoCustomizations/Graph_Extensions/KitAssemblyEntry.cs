using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.PXAccess;

namespace PX.Objects.IN
{
    public class KitAssemblyEntryExt : PXGraphExtension<KitAssemblyEntry>
    {
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseHandler)
        {
            var docRow = Base.Document.Current;
            if (docRow != null && docRow.DocType == "D")
            {
                var decimalPlace = SelectFrom<Organization>
                               .InnerJoin<CurrencyList>.On<Organization.baseCuryID.IsEqual<CurrencyList.curyID>>
                               .View.Select(Base).RowCast<CurrencyList>().FirstOrDefault()?.DecimalPlaces;
                var itemInfo = SelectFrom<INItemCost>.Where<INItemCost.inventoryID.IsEqual<P.AsInt>>.View.Select(Base, docRow.KitInventoryID).RowCast<INItemCost>().FirstOrDefault();
                // Order by UnitCost find Max Price
                var trans = Base.Components.Select().RowCast<INComponentTran>().ToList().OrderBy(x => x.UnitCost);
                if (decimalPlace != null && itemInfo != null && trans != null)
                {
                    var lastIdx = trans.Count() - 1;
                    var result = Math.Round((double)(itemInfo?.AvgCost * docRow?.Qty), (int)decimalPlace);
                    for (int i = 0; i < trans.Count() - 1; i++)
                        result -= (double)Math.Round((double)(trans.ElementAt(i)?.UnitCost * trans.ElementAt(i)?.Qty), (int)decimalPlace);
                    // Reset Unit Price
                    var calcResult = Math.Round(result / (double)trans.ElementAt(lastIdx)?.Qty, 4);
                    if(Math.Round(calcResult * (double)trans.ElementAt(lastIdx)?.Qty, (int)decimalPlace) != result)
                        throw new PXException("The disassembly will create unbalance inventory value, please modify the unit cost manually");
                    Base.Components.SetValueExt<INComponentTran.unitCost>(trans.ElementAt(lastIdx), (decimal)calcResult);
                }
            }
            baseHandler();
        }
    }
}
