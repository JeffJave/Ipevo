using IpevoCustomizations.Descriptor;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpevoCustomizations.Graph_Extensions
{
    public class CADepositEntryExt : PXGraphExtension<CADepositEntry>
    {
        #region Override DAC
        [LUMGetAvailablePaymentsAttribute]
        [PXMergeAttributes(Method = MergeMethod.Append)]
        public virtual void _(Events.CacheAttached<CADepositDetail.origRefNbr> e) { }

        #endregion


        #region Event

        public virtual void _(Events.RowSelected<CADeposit> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            Base.DepositPayments.Cache.AllowInsert = Base.DepositPayments.Cache.AllowUpdate = true;
        }

        public virtual void _(Events.FieldUpdated<CADepositDetail.origRefNbr> e, PXFieldUpdated baseHandler)
        {
            var row = e.Row as CADepositDetail;
            // Check is Manual Insert New Record
            if (row == null || !string.IsNullOrEmpty(row.OrigDocType))
                return;
            baseHandler?.Invoke(e.Cache, e.Args);
            // Get ARInfomation
            var ARInfo = SelectFrom<ARPayment>.Where<ARPayment.refNbr.IsEqual<P.AsString>>.View.Select(Base, (string)e.NewValue).RowCast<ARPayment>().FirstOrDefault();
            // Setting filter
            Base.filter.Current.StartDate = ARInfo.DepositAfter;
            Base.filter.Current.EndDate = ARInfo.DepositAfter;
            Base.filter.Current.PaymentMethodID = null;
            //
            Base.AvailablePayments.Select().ToList();
            // Delete Manual insert record
            Base.DepositPayments.Delete(e.Row as CADepositDetail);

            // Get Availalbe Payment and set current record Selected
            Base.AvailablePayments.Cache.AllowInsert = Base.AvailablePayments.Cache.AllowUpdate = true;
            try
            {
                Base.AvailablePayments.Cache.Inserted.Cast<PaymentInfo>().Where(p => p.RefNbr == (string)e.NewValue).FirstOrDefault().Selected = true;
            }
            catch (NullReferenceException ex)
            {
                throw new Exception("Can not find data in Available PAYMENT!");
            }
            // Insert Data
            IEnumerable<PaymentInfo> toAdd = Base.AvailablePayments.Cache.Inserted.Cast<PaymentInfo>().Where(p => p.Selected == true);
            Base.AddPaymentInfoBatch(toAdd);
            foreach (PaymentInfo it in toAdd)
                it.Selected = false;
            Base.AvailablePayments.Cache.AllowInsert = Base.AvailablePayments.Cache.AllowUpdate = false;
            Base.AvailablePayments.Cache.Clear();
            Base.DepositPayments.View.RequestRefresh();
        }

        #endregion

        #region Method
        protected virtual CashAccountDeposit GetCashAccountDepositSettings(PaymentInfo payment)
        {
            CashAccountDeposit settings = null;
            if (payment.Module == PX.Objects.GL.BatchModule.AP || payment.Module == PX.Objects.GL.BatchModule.AR)
            {
                settings = PXSelect<CashAccountDeposit,
                            Where<CashAccountDeposit.accountID, Equal<Current<CADeposit.cashAccountID>>,
                                And<CashAccountDeposit.depositAcctID, Equal<Required<CashAccountDeposit.depositAcctID>>,
                                And<Where<CashAccountDeposit.paymentMethodID, Equal<PX.Objects.BQLConstants.EmptyString>,
                                       Or<CashAccountDeposit.paymentMethodID, Equal<Required<CashAccountDeposit.paymentMethodID>>>>>>>,
                            OrderBy<Desc<CashAccountDeposit.paymentMethodID>>>.Select(Base, payment.CashAccountID, payment.PaymentMethodID);
            }
            else if (payment.Module == PX.Objects.GL.BatchModule.CA)
            {
                settings = PXSelect<CashAccountDeposit,
                                        Where<CashAccountDeposit.accountID, Equal<Current<CADeposit.cashAccountID>>,
                                          And<CashAccountDeposit.depositAcctID, Equal<Required<CashAccountDeposit.depositAcctID>>,
                                          And<CashAccountDeposit.paymentMethodID, Equal<PX.Objects.BQLConstants.EmptyString>>>>,
                                        OrderBy<Desc<CashAccountDeposit.paymentMethodID>>>.Select(Base, payment.CashAccountID);

                if (settings == null)
                {
                    settings = new CashAccountDeposit();
                    settings.AccountID = Base.DocumentCurrent.Current.CashAccountID;
                }
            }

            return settings;
        }

        protected void Copy(CADepositDetail aDest, PaymentInfo aPayment)
        {
            //aDest.OrigModule = aPayment.Module;
            aDest.OrigDocType = aPayment.DocType;
            //aDest.OrigRefNbr = aPayment.RefNbr;
            aDest.OrigCuryID = aPayment.CuryID;
            aDest.OrigCuryInfoID = aPayment.CuryInfoID;
            aDest.OrigDrCr = aPayment.DrCr;
            aDest.CuryOrigAmt = aPayment.CuryOrigDocAmt;
            aDest.OrigAmt = aPayment.OrigDocAmt;
            aDest.AccountID = aPayment.CashAccountID;
            aDest.CuryTranAmt = aDest.CuryOrigAmtSigned; //Check Later - for the case when currencies are different
            aDest.TranAmt = aDest.OrigAmtSigned;
        }
        protected static void Copy(CADepositDetail aDest, CashAccountDeposit aSettings)
        {
            aDest.ChargeEntryTypeID = aSettings.ChargeEntryTypeID;
            aDest.PaymentMethodID = aSettings.PaymentMethodID;
        }

        #endregion

    }
}
