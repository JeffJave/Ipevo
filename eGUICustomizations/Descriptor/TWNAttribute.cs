using System;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.TX;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Descriptor
{
    #region ChineseAmountAttribute
    /// <summary>
    /// Create custom attribute that convert number to Chinese word.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ChineseAmountAttribute : PXStringAttribute, IPXFieldSelectingSubscriber
    {
        void IPXFieldSelectingSubscriber.FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            ARInvoice invoice = sender.Graph.Caches[typeof(ARInvoice)].Current as ARInvoice;

            if (invoice != null)
            {
                e.ReturnValue = ARInvoiceEntry_Extension.AmtInChinese((int)invoice.CuryDocBal);
            }
        }
    }
    #endregion

    #region ARGUINbrAutoNumAttribute
    public class ARGUINbrAutoNumAttribute : PX.Objects.CS.AutoNumberAttribute
    {
        public ARGUINbrAutoNumAttribute(Type doctypeField, Type dateField) : base(doctypeField, dateField) { }

        public ARGUINbrAutoNumAttribute(Type doctypeField, Type dateField, string[] doctypeValues, Type[] setupFields) : 
                                        base(doctypeField, dateField, doctypeValues, setupFields) { }

        public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            if (e.Operation == PXDBOperation.Delete) { return; }

            string vATOutCode = string.Empty;

            if (this.BqlTable.Name == nameof(DAC.TWNManualGUIAR))
            {
                var row = (TWNManualGUIAR)e.Row;

                vATOutCode = row.VatOutCode;
            }
            else
            {
                var row = (ARRegister)e.Row;

                vATOutCode = PXCache<ARRegister>.GetExtension<ARRegisterExt>(row).UsrVATOutCode;
            }

            if (vATOutCode != TWGUIFormatCode.vATOutCode33 && vATOutCode != TWGUIFormatCode.vATOutCode34 && vATOutCode != null)
            {
                base.RowPersisting(sender, e);
            }
            
            sender.SetValue(e.Row, _FieldName, (string)sender.GetValue(e.Row, _FieldName));
        }
    }
    #endregion

    #region GUINumberAttribute
    public class GUINumberAttribute : PXDBStringAttribute, IPXFieldVerifyingSubscriber
    {
        public GUINumberAttribute(int length) : base(length) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (string.IsNullOrEmpty((string)e.NewValue) ||
                TWNGUIValidation.ActivateTWGUI(new PXGraph()) == false) { return; }

            object obj = null;
            string vATCode = null;

            switch (this.BqlTable.Name)
            {
                case nameof(ARRegister):
                    obj = sender.GetValueExt<ARRegisterExt.usrVATOutCode>(e.Row);
                    break;
                case nameof(TWNGUITrans):
                    obj = sender.GetValueExt<TWNGUITrans.gUIFormatcode>(e.Row);
                    break;
                case nameof(TWNManualGUIAP):
                    obj = sender.GetValueExt<TWNManualGUIAP.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAR):
                    obj = sender.GetValueExt<TWNManualGUIAR.vatOutCode>(e.Row);
                    break;
                case nameof(TWNManualGUIBank):
                    obj = sender.GetValueExt<TWNManualGUIBank.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIExpense):
                    obj = sender.GetValueExt<TWNManualGUIExpense.vATInCode>(e.Row);
                    break;
                case nameof(TWNManualGUIAPBill):
                    obj = sender.GetValueExt<TWNManualGUIAPBill.vATInCode>(e.Row);
                    break;
            }

            vATCode = obj is null ? string.Empty : obj.ToString();

            if ((!vATCode.IsIn(TWGUIFormatCode.vATOutCode33, TWGUIFormatCode.vATOutCode34) ||
                vATCode.IsIn(TWGUIFormatCode.vATInCode21, TWGUIFormatCode.vATInCode23, TWGUIFormatCode.vATInCode25)) &&
                e.NewValue.ToString().Length < 10)
            {
                throw new PXSetPropertyException(TWMessages.GUINbrMini, PXErrorLevel.Error);
            }

            new TWNGUIValidation().CheckGUINbrExisted(sender.Graph, (string)e.NewValue, vATCode);
        }
    }
    #endregion

    #region TaxNbrVerifyAttribute
    public class TaxNbrVerifyAttribute : PXDBStringAttribute, IPXFieldVerifyingSubscriber
    {
        public TaxNbrVerifyAttribute(int length) : base(length) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null) { return; }

            TWNGUIValidation validation = new TWNGUIValidation();

            validation.CheckTabNbr(e.NewValue.ToString());

            if (validation.errorOccurred == true)
            {
                throw new PXSetPropertyException(validation.errorMessage, (PXErrorLevel)validation.errorLevel);
            }

        }
    }
    #endregion

    #region TWNetAmountAttribute
    public class TWNetAmountAttribute : PXDBDecimalAttribute, IPXFieldVerifyingSubscriber
    {
        public TWNetAmountAttribute(int percision) : base(percision) { }

        public void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue < 0)
            {
                // Throwing an exception to cancel assignment of the new value to the field
                throw new PXSetPropertyException(TWMessages.NetAmtNegError);
            }
        }
    }
    #endregion

    #region TWTaxAmountAttribute
    public class TWTaxAmountAttribute : PXDBDecimalAttribute, IPXFieldVerifyingSubscriber
    {
        protected Type _NetAmt;

        public TWTaxAmountAttribute(int percision) : base(percision) { } 

        public TWTaxAmountAttribute(int percision, Type netAmt) : base(percision) 
        {
            _NetAmt = netAmt;
        }

        public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue < 0)
            {
                // Throwing an exception to cancel assignment of the new value to the field
                throw new PXSetPropertyException(TWMessages.TaxAmtNegError);
            }

            new TWNGUIValidation().CheckTaxAmount((decimal)sender.GetValue(e.Row, _NetAmt.Name), (decimal)e.NewValue);
        }
    }
    #endregion

    #region TWTaxAmountCalcAttribute
    public class TWTaxAmountCalcAttribute : TWNetAmountAttribute, IPXFieldUpdatedSubscriber
    {
        protected Type _TaxID;
        protected Type _NetAmt;
        protected Type _TaxAmt;

        public TWTaxAmountCalcAttribute(int percision, Type taxID, Type netAmt, Type taxAmt) : base(percision) 
        {
            _TaxID  = taxID;
            _NetAmt = netAmt;
            _TaxAmt = taxAmt;
        }

        public virtual void FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            string  taxID  = (string)sender.GetValue(e.Row, _TaxID.Name);
            decimal netAmt = (decimal)sender.GetValue(e.Row, _NetAmt.Name);

            foreach (TaxRev taxRev in SelectFrom<TaxRev>.Where<TaxRev.taxID.IsEqual<@P.AsString>
                                                               .And<TaxRev.taxType.IsEqual<@P.AsString>>>.View.Select(sender.Graph, taxID, "P")) // P = Group type (Input)
            {
                decimal taxAmt = Math.Round(netAmt * (taxRev.TaxRate.Value / taxRev.NonDeductibleTaxRate.Value), 0);

                sender.SetValue(e.Row, _TaxAmt.Name, taxAmt);
            }
        }
    }
    #endregion
}