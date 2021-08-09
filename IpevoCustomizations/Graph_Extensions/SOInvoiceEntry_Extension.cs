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
            ExportInvoiceUK.SetVisible(false);
            ExportInvoiceCA.SetVisible(false);
            ExportInvoiceNL.SetVisible(false);

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.BranchCD.Trim() == "IPEVOTW" || curCoutry?.BaseCuryID == "TWD")
            {
                ExportInvoice.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoice);
            }
            else if (curCoutry?.BranchCD.Trim() == "IPEVOUS" || curCoutry?.BaseCuryID == "USD")
            {
                ExportInvoiceUS.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoiceUS);
            }
            else if (curCoutry?.BranchCD.Trim() == "IPEVOUK" || curCoutry?.BaseCuryID == "GBP")
            {
                ExportInvoiceUK.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoiceUK);
            }
            else if (curCoutry?.BranchCD.Trim() == "IPEVOCA" || curCoutry?.BaseCuryID == "CAD")
            {
                ExportInvoiceCA.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoiceCA);
            }
            else if (curCoutry?.BranchCD.Trim() == "IPEVONL" || curCoutry?.BaseCuryID == "NLG") //EUR
            {
                ExportInvoiceNL.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoiceNL);
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

        public PXAction<ARInvoice> ExportInvoiceCA;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice - CA", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable exportInvoiceCA(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM606410", "Report LM606410") { Mode = PXBaseRedirectException.WindowMode.New };
            }
            return adapter.Get();
        }

        public PXAction<ARInvoice> ExportInvoiceUK;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice - UK", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable exportInvoiceUK(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM606415", "Report LM606415") { Mode = PXBaseRedirectException.WindowMode.New };
            }
            return adapter.Get();
        }

        public PXAction<ARInvoice> ExportInvoiceNL;
        [PXButton]
        [PXUIField(DisplayName = "Print Invoice - NL", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable exportInvoiceNL(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                throw new PXReportRequiredException(parameters, "LM606420", "Report LM606420") { Mode = PXBaseRedirectException.WindowMode.New };
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
