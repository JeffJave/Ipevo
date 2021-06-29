using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace PX.Objects.AP
{
    public class APInvoiceEntry_Extension : PXGraphExtension<APInvoiceEntry>
    {
        #region Event Handlers
        public bool activateGUI = TWNGUIValidation.ActivateTWGUI(new PXGraph());

        protected void _(Events.RowPersisting<APInvoice> e, PXRowPersisting InvokeBaseHandler)
        {
            InvokeBaseHandler?.Invoke(e.Cache, e.Args);

            if (activateGUI.Equals(false) || e.Row.Status.Equals(APDocStatus.Open)) { return; }

            APRegisterExt regisExt = PXCache<APRegister>.GetExtension<APRegisterExt>(e.Row);

            TWNGUIValidation tWNGUIValidation = new TWNGUIValidation();

            if (regisExt.UsrVATInCode == TWGUIFormatCode.vATInCode21 ||
                regisExt.UsrVATInCode == TWGUIFormatCode.vATInCode22 ||
                regisExt.UsrVATInCode == TWGUIFormatCode.vATInCode25)
            {
                tWNGUIValidation.CheckGUINbrExisted(Base, regisExt.UsrGUINbr, regisExt.UsrVATInCode);
            }
            else
            {
                tWNGUIValidation.CheckCorrespondingInv(Base, regisExt.UsrGUINbr, regisExt.UsrVATInCode);

                if (tWNGUIValidation.errorOccurred.Equals(true))
                {
                    e.Cache.RaiseExceptionHandling<APRegisterExt.usrGUINbr>(e.Row, regisExt.UsrGUINbr, new PXSetPropertyException(tWNGUIValidation.errorMessage, PXErrorLevel.Error));
                }
            }
        }

        protected void _(Events.RowInserting<APInvoice> e)
        {
            if (e.Row == null || Base.vendor.Current == null || activateGUI.Equals(false)) { return; }

            APRegisterExt regisExt = PXCache<APRegister>.GetExtension<APRegisterExt>(e.Row);

            string vATIncode = regisExt.UsrVATInCode ?? string.Empty;

            if (string.IsNullOrEmpty(vATIncode))
            {
                CSAnswers cSAnswers = SelectCSAnswers(Base, Base.vendor.Current.NoteID);

                vATIncode = (e.Row.IsRetainageDocument == true || cSAnswers == null) ? null : cSAnswers.Value;
            }

            regisExt.UsrVATInCode = e.Row.DocType.Equals(APDocType.DebitAdj, System.StringComparison.CurrentCulture) && 
                                    !string.IsNullOrEmpty(vATIncode) ? TWGUIFormatCode.vATInCode23 /*(int.Parse(vATIncode) + 2).ToString()*/ : vATIncode;
        }

        protected void _(Events.RowSelected<APRegister> e)
        {
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrDeduction>(e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrGUIDate>  (e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrGUINbr>    (e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrOurTaxNbr> (e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrTaxNbr>    (e.Cache, null, activateGUI);
            PXUIFieldAttribute.SetVisible<APRegisterExt.usrVATInCode>(e.Cache, null, activateGUI);
        }

        protected void _(Events.FieldUpdated<APInvoice.vendorID> e)
        {
            var row    = e.Row as APInvoice;
            var vendor = Base.vendor.Current;

            if (vendor == null || activateGUI == false) { return; }

            switch (row.DocType)
            {
                case APDocType.DebitAdj:
                    PXCache<APRegister>.GetExtension<APRegisterExt>(row).UsrVATInCode = TWGUIFormatCode.vATInCode23;
                    break;

                case APDocType.Invoice:
                    CSAnswers cSAnswers = SelectCSAnswers(Base, vendor.NoteID);

                    PXCache<APRegister>.GetExtension<APRegisterExt>(row).UsrVATInCode = cSAnswers?.Value;
                    break;
            }
        }
        #endregion

        #region Static Method
        public static CSAnswers SelectCSAnswers(PXGraph graph, System.Guid? noteID)
        {
            return SelectFrom<CSAnswers>.Where<CSAnswers.refNoteID.IsEqual<@P.AsGuid>
                                               .And<CSAnswers.attributeID.IsEqual<APRegisterExt.VATINFRMTNameAtt>>>
                                        .View.Select(graph, noteID);
        }
        #endregion
    }
}