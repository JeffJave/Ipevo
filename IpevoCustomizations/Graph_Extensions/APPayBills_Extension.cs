using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;
using PX.Objects.Common.Extensions;
using PX.Objects.Common.Utility;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AP
{
	public class APPayBills_Extension : PXGraphExtension<APPayBills>
	{
		private Dictionary<object, object> _copies = new Dictionary<object, object>();
		public class VendorCrossRateAttr : BqlString.Constant<VendorCrossRateAttr>
		{
			public VendorCrossRateAttr() : base("CROSSRATE") { }
		}
		
		protected virtual void APAdjust_CuryAdjgAmt_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e, PXFieldDefaulting del)
		{
			if (del != null)
			{
				del(sender, e);
			}

			APAdjust apRow = e.Row as APAdjust;
			var aPInvoiceCurData = SelectFrom<APInvoice>.View.Select(Base).RowCast<APInvoice>().ToList().FirstOrDefault(x => x.DocType == apRow.AdjdDocType && x.RefNbr == apRow.AdjdRefNbr);
			var aPPaymentVendorCrossRateAttr = SelectFrom<CSAnswers>.
													LeftJoin<BAccountR>.On<CSAnswers.refNoteID.IsEqual<BAccountR.noteID>>.
													LeftJoin<APAdjust>.On<BAccountR.bAccountID.IsEqual<APAdjust.vendorID>>.
													Where<APAdjust.vendorID.IsEqual<@P.AsInt>.And<CSAnswers.attributeID.IsEqual<VendorCrossRateAttr>>>.
													View.Select(Base, apRow.VendorID).TopFirst?.Value;

			if (aPPaymentVendorCrossRateAttr == "1")
			{
				var curyInfo = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aPInvoiceCurData?.CuryInfoID).TopFirst;
				var curyInfoCuryRate = curyInfo?.CuryMultDiv == "M" ? curyInfo?.CuryRate : curyInfo?.RecipRate;
				var aPInvoiceCuryOrigDocAmt = aPInvoiceCurData?.CuryOrigDocAmt;
				e.NewValue = Math.Round((decimal)(aPInvoiceCuryOrigDocAmt * curyInfoCuryRate));
			}
		}
		
	}
}
