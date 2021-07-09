using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.SO;

namespace PX.Objects.AP
{
    public class APPaymentEntryExt : PXGraphExtension<APPaymentEntry>
    {
        public class VendorCrossRateAttr : BqlString.Constant<VendorCrossRateAttr>
        {
            public VendorCrossRateAttr() : base("CROSSRATE") { }
        }

        public IEnumerable Adjustments()
        {
            var newAdjustments = SelectFrom<APAdjust>.
                                    LeftJoin<APInvoice>.On<APInvoice.docType.IsEqual<APAdjust.adjdDocType>.And<APInvoice.refNbr.IsEqual<APAdjust.adjdRefNbr>>>.
                                    LeftJoin<APTran>.On<APInvoice.paymentsByLinesAllowed.IsEqual<True>.And<APTran.tranType.IsEqual<APAdjust.adjdDocType>.And<APTran.refNbr.IsEqual<APAdjust.adjdRefNbr>.And<APTran.lineNbr.IsEqual<APAdjust.adjdLineNbr>>>>>.
                                    Where<APAdjust.adjgDocType.IsEqual<APPayment.docType.FromCurrent>.And<APAdjust.adjgRefNbr.IsEqual<APPayment.refNbr.FromCurrent>.And<APAdjust.released.IsNotEqual<True>>>>.
                                    View.Select(Base);

            var row = Base.Document.Current;
            if (row == null) return newAdjustments;

            var aPPaymentVendorCrossRateAttr = SelectFrom<CSAnswers>.
                                                LeftJoin<BAccountR>.On<CSAnswers.refNoteID.IsEqual<BAccountR.noteID>>.
                                                LeftJoin<APPayment>.On<BAccountR.bAccountID.IsEqual<APPayment.vendorID>>.
                                                Where<APPayment.refNbr.IsEqual<@P.AsString>.And<APPayment.docType.IsEqual<@P.AsString>>.And<CSAnswers.attributeID.IsEqual<VendorCrossRateAttr>>>.
                                                View.Select(Base, row.RefNbr, row.DocType).TopFirst?.Value;
            foreach (PXResult<APAdjust, APInvoice, APTran> adjustment in newAdjustments)
            {
                APAdjust aPAdjust = adjustment;
                APInvoice aPInvoice = adjustment;

                if (row.CuryID != aPInvoice.CuryID && aPPaymentVendorCrossRateAttr == "1" && Convert.ToDecimal(aPAdjust.AdjdCuryRate) != 1.00m && aPInvoice.CuryInfoID != null)
                {
                    var curyInfo = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aPInvoice.CuryInfoID).TopFirst;
                    var curyInfoCuryRate = curyInfo?.CuryMultDiv == "M" ? curyInfo?.CuryRate : curyInfo?.RecipRate;
                    aPAdjust.AdjdCuryRate = curyInfoCuryRate == null ? aPAdjust.AdjdCuryRate : curyInfoCuryRate;
                }
            }
            return newAdjustments;
        }

        protected void _(Events.FieldUpdated<APAdjust.adjdRefNbr> e)
        {
            if (e.NewValue != null)
            {
                var row = Base.Document.Current;
                var aPPaymentVendorCrossRateAttr = SelectFrom<CSAnswers>.
                                                    LeftJoin<BAccountR>.On<CSAnswers.refNoteID.IsEqual<BAccountR.noteID>>.
                                                    LeftJoin<APPayment>.On<BAccountR.bAccountID.IsEqual<APPayment.vendorID>>.
                                                    Where<APPayment.vendorID.IsEqual<@P.AsInt>.And<CSAnswers.attributeID.IsEqual<VendorCrossRateAttr>>>.
                                                    View.Select(Base, row.VendorID).TopFirst?.Value;
                var apRow = e.Row as APAdjust;
                var aPInvoiceCurData = SelectFrom<APInvoice>.View.Select(Base).RowCast<APInvoice>().ToList().FirstOrDefault(x => x.DocType == apRow.AdjdDocType && x.RefNbr == apRow.AdjdRefNbr);
                if (row.CuryID != aPInvoiceCurData?.CuryID && aPPaymentVendorCrossRateAttr == "1" && Convert.ToDecimal(apRow.AdjdCuryRate) != 1.00m && aPInvoiceCurData?.CuryInfoID != null)
                {
                    var curyInfo = SelectFrom<CurrencyInfo>.Where<CurrencyInfo.curyInfoID.IsEqual<@P.AsInt>>.View.Select(Base, aPInvoiceCurData?.CuryInfoID).TopFirst;
                    var curyInfoCuryRate = curyInfo?.CuryMultDiv == "M" ? curyInfo?.CuryRate : curyInfo?.RecipRate;
                    e.Cache.SetValueExt<APAdjust.adjdCuryRate>(e.Row, curyInfoCuryRate == null ? apRow.AdjdCuryRate : curyInfoCuryRate);
                }
            }
        }
    }
}
