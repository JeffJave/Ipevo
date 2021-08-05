using System.Collections.Generic;
using PX.Data;
using PX.Objects.AR;
using eGUICustomizations.Descriptor;

namespace PX.Objects.SO
{
    public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry>
    {
        public const string GUIReportID = "TW601000";

        #region Override Methods
        public override void Initialize()
        {
            base.Initialize();

            Base.report.AddMenuAction(printGUIInvoice);
        }
        #endregion

        #region Actions
        public PXAction<ARInvoice> printGUIInvoice;
        [PXButton()]
        [PXUIField(DisplayName = "Print GUI Invoice", MapEnableRights = PXCacheRights.Select)]
        protected virtual void PrintGUIInvoice()
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    [nameof(ARInvoice.DocType)] = Base.Document.Current.DocType,
                    [nameof(ARInvoice.RefNbr)] = Base.Document.Current.RefNbr
                };

                throw new PXReportRequiredException(parameters, GUIReportID, GUIReportID);
            }
        }
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<SOInvoice> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            Base.report.SetVisible(nameof(PrintGUIInvoice), TWNGUIValidation.ActivateTWGUI(e.Cache.Graph));

            printGUIInvoice.SetEnabled(TWNGUIValidation.ActivateTWGUI(e.Cache.Graph) && !string.IsNullOrEmpty(Base.Document.Current.GetExtension<ARRegisterExt>()?.UsrGUINbr) );
        }
        #endregion
    }
}