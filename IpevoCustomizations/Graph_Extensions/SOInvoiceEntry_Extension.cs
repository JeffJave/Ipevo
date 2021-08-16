using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.TX;
using System;
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

        [PXOverride]
        public virtual IEnumerable EmailInvoice(PXAdapter adapter, [PXString] string notificationCD = null)
        {
            var tenantName = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst?.BranchCD?.Trim()?.ToUpper();
            string reportID = tenantName.Contains("US") ? "SO606405" :
                              tenantName.Contains("UK") ? "SO606415" :
                              tenantName.Contains("CA") ? "SO606410" :
                              tenantName.Contains("NL") ? "SO606420" : string.Empty;
            // Go Process Standard code
            if (string.IsNullOrEmpty(reportID))
            {
                adapter.Arguments.Add("notificationCD", notificationCD ?? "SO INVOICE");
                Base.notification.PressButton(adapter);
                foreach (ARInvoice item in adapter.Get().RowCast<ARInvoice>())
                    yield return item;
            }
            else
            {
                #region Insert Customer mail setting
                var doc = Base.Document.Current;
                if (doc == null)
                    throw new Exception("Document is null!!");
                var customerInfo = Customer.PK.Find(Base, doc.CustomerID);
                if (customerInfo == null)
                    throw new PXException("Can not find Customer!!");
                if (SelectFrom<NotificationSource>.Where<NotificationSource.refNoteID.IsEqual<@P.AsGuid>
                                                         .And<NotificationSource.reportID.IsEqual<@P.AsString>>>.View.Select(Base, customerInfo.NoteID, reportID).Count == 0)
                    CreateCRMailingSetting(reportID);
                #endregion
                // setting report parameter
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                List<Guid?> attachments = new List<Guid?>();

                // Get File
                foreach (ARTran item in Base.Transactions.Cache.Cached)
                {
                    var order = SOOrder.PK.Find(Base, item.SOOrderType, item.SOOrderNbr);
                    if (order == null)
                        continue;
                    var files = PXNoteAttribute.GetFileNotes(PXGraph.CreateInstance<SOOrderEntry>().Document.Cache, order);
                    foreach (var file in files)
                        attachments.Add(file);
                }
                Base.Activity.SendNotification(PX.Objects.AR.ARNotificationSource.Customer, "LUM INVOICE", Base.Accessinfo.BranchID, parameters, attachments);
                foreach (ARInvoice item in adapter.Get().RowCast<ARInvoice>())
                    yield return item;
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
                throw new PXReportRequiredException(parameters, "SO606405", "Report SO606405") { Mode = PXBaseRedirectException.WindowMode.New };
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
                throw new PXReportRequiredException(parameters, "SO606410", "Report SO606410") { Mode = PXBaseRedirectException.WindowMode.New };
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
                throw new PXReportRequiredException(parameters, "SO606415", "Report SO606415") { Mode = PXBaseRedirectException.WindowMode.New };
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
                throw new PXReportRequiredException(parameters, "SO606420", "Report SO606420") { Mode = PXBaseRedirectException.WindowMode.New };
            }
            return adapter.Get();
        }
        #endregion

        #region Cache Attached
        [PXRemoveBaseAttribute(typeof(SOInvoiceTaxAttribute))]
        [SOInvoiceTax2]
        protected void _(Events.CacheAttached<ARTran.taxCategoryID> e) { }
        #endregion

        #region Function

        public virtual void CreateCRMailingSetting(string lumRptID)
        {
            CustomerMaint maint = PXGraph.CreateInstance<CustomerMaint>();

            foreach (NotificationSetup setup in SelectFrom<NotificationSetup>.Where<NotificationSetup.reportID.IsEqual<P.AsString>>.View.Select(Base, lumRptID))
            {
                NotificationSource source = maint.NotificationSources.Cache.CreateInstance() as NotificationSource;

                source.SetupID = setup.SetupID;

                source = maint.NotificationSources.Insert(source);

                source.ReportID = setup.ReportID;
                source.Format = setup.Format;
                source.Active = setup.Active;
                source.RecipientsBehavior = setup.RecipientsBehavior;

                maint.NotificationSources.Update(source);
            }

            maint.CurrentCustomer.Current = Base.customer.Current;

            maint.Actions.PressSave();
        }

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
