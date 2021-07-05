using PX.Data;

namespace PX.Objects.CA
{
    public class PaymentInfoExt : PXCacheExtension<PX.Objects.CA.PaymentInfo>
    {
        #region UsrInvoiceNbr
        [PXString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Invoice Nbr.", Enabled = false)]
        public virtual string UsrInvoiceNbr
        {
            get => AR.ARPaymentEntryExt.GetSOInvoiceNbr(Base.DocType, Base.RefNbr);
            set { }
        }
        //public abstract class usrInvoiceNbr : PX.Data.BQL.BqlString.Field<usrInvoiceNbr> { }
        #endregion
    }
}