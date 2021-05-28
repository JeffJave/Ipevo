using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL;
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

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.CountryID == "TW" || curCoutry?.BaseCuryID == "TWD")
            {
                ExportInvoice.SetVisible(true);
                Base.report.AddMenuAction(ExportInvoice);
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
        #endregion
    }
}
