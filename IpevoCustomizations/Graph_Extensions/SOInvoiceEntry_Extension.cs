using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.TX;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.SO
{
	public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry>
	{
        public override void Initialize()
        {
            base.Initialize();
            ExportInvoice.SetVisible(false);
            ExportInvoiceUS.SetVisible(false);

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.CountryID == "TW" || curCoutry?.BaseCuryID == "TWD")
            {
                ExportInvoice.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoice);
            }
            else if (curCoutry?.CountryID == "US" || curCoutry?.BaseCuryID == "USD")
            {
                ExportInvoiceUS.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoiceUS);
            }
        }

        #region Action
        public PXAction<ARInvoice> ExportInvoice;
        [PXButton]
        [PXUIField(DisplayName = "Print Export Invoice", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable exportInvoice(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM606400", "Report LM606400");
            }
            return adapter.Get();
        }

        public PXAction<ARInvoice> ExportInvoiceUS;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice - US", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable exportInvoiceUS(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM606405", "Report LM606405") { Mode = PXBaseRedirectException.WindowMode.New };
            }
            return adapter.Get();
        }
        #endregion

        #region Cache Attached
        [PXRemoveBaseAttribute(typeof(SOInvoiceTaxAttribute))]
        [SOInvoiceTax2]
        protected void _(Events.CacheAttached<ARTran.taxCategoryID> e) { }
        #endregion
    }

    #region Inherited class
    public class SOInvoiceTax2Attribute : SOInvoiceTaxAttribute
    {
        protected override TaxDetail CalculateTaxSum(PXCache sender, object taxrow, object row)
        {
            TaxRev taxrev = PXResult.Unwrap<TaxRev>(taxrow);
            Tax tax = PXResult.Unwrap<Tax>(taxrow);
            
            bool propagateCustomRate = false;
            var origTaxRate = taxrev.TaxRate;

            if (taxrev.TaxID != null && tax != null)
            {
                TaxExt taxExt = tax.GetExtension<TaxExt>();

                if (taxExt.UsrAMPropagateTaxAmt == true)
                {
                    SOTaxTran soTax = PXResult<SOTaxTran>.Current;

                    if (soTax != null && taxrev.TaxID == soTax.TaxID && soTax.CuryTaxableAmt.GetValueOrDefault() > 0)
                    {
                        var taxRate = soTax.CuryTaxAmt / soTax.CuryTaxableAmt * 100;
                        if (taxRate != origTaxRate && taxRate > 0)
                        {
                            taxrev.TaxRate = taxRate;
                            propagateCustomRate = true;
                        }
                    }
                }
            }

            TaxDetail result = base.CalculateTaxSum(sender, taxrow, row);

            if (result != null && propagateCustomRate)
            {
                result.TaxRate = origTaxRate;
                taxrev.TaxRate = origTaxRate;
            }
            return result;
        }
    }
    #endregion
}
