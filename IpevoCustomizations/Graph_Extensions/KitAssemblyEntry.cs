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
                // 固定四捨五入到小數2位數
                decimalPlace = 2;
                var itemInfo = SelectFrom<INItemCost>.Where<INItemCost.inventoryID.IsEqual<P.AsInt>>.View.Select(Base, docRow.KitInventoryID).RowCast<INItemCost>().FirstOrDefault();
                // Order by UnitCost find Max Price
                var trans = Base.Components.Select().RowCast<INComponentTran>().ToList().OrderBy(x => x.UnitCost);
                if (decimalPlace != null && itemInfo != null && trans != null)
                {
                    // 組件成本
                    var itemCost = (decimal)(itemInfo?.AvgCost * docRow?.Qty);
                    var totalComponentsCost = trans.Sum(x => x?.UnitCost * x?.Qty);
                    var adjCost = itemCost - totalComponentsCost;
                    decimal alreadyAjdCost = 0;
                    for (int i = 0; i < trans.Count() - 1; i++)
                    {
                        decimal newValue = Math.Round((decimal)(trans.ElementAt(i).UnitCost.Value + (trans.ElementAt(i).UnitCost.Value * trans.ElementAt(i).Qty.Value / totalComponentsCost * adjCost / docRow?.Qty.Value)), (int)decimalPlace);
                        alreadyAjdCost += newValue * trans.ElementAt(i).Qty.Value;
                        Base.Components.SetValueExt<INComponentTran.unitCost>(trans.ElementAt(i), (decimal)newValue);
                    }

                    //if (Math.Round(calcResult * (double)trans.ElementAt(lastIdx)?.Qty, (int)decimalPlace) != result)
                    //    throw new PXException("The disassembly will create unbalance inventory value, please modify the unit cost manually");
                    Base.Components.SetValueExt<INComponentTran.unitCost>(trans.LastOrDefault(), Math.Round((decimal)((itemCost - alreadyAjdCost) / docRow.Qty), (int)decimalPlace));
                }
            }
            baseHandler();
        }
    }
}
