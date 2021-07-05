using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    public class ARPaymentEntryExt : PXGraphExtension<ARPaymentEntry>
    {
        #region Cache Attached Events
        [PXMergeAttributes(Method = MergeMethod.Replace)]
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Order Nbr.")]
		[PXParent(typeof(PX.Objects.SO.SOAdjust.FK.Order))]
		[PXUnboundFormula(typeof(Switch<Case<Where<SOAdjust.curyAdjdAmt, Greater<decimal0>>, int1>, int0>), typeof(SumCalc<SOOrder.paymentCntr>))]
		[PXRestrictor(typeof(Where<SOOrder.status, NotEqual<SOOrderStatus.cancelled>, And<SOOrder.status, NotEqual<SOOrderStatus.pendingApproval>,
			And<SOOrder.status, NotEqual<SOOrderStatus.voided>>>>), PX.Objects.SO.Messages.DontApprovedDocumentsCannotBeSelected)]
		[PXRestrictor(typeof(Where<SOOrder.hasLegacyCCTran, NotEqual<True>>),
            PX.Objects.SO.Messages.CantProcessSOBecauseItHasLegacyCCTran, typeof(SOOrder.orderType), typeof(SOOrder.orderNbr))]
		[PXSelector(typeof(Search2<SOOrder.orderNbr,
			InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>,
			InnerJoin<Terms, On<Terms.termsID, Equal<SOOrder.termsID>>>>,
			Where2<Where<SOOrder.customerID, Equal<Current<SOAdjust.customerID>>, Or<SOOrder.customerID, In2<Search<Customer.bAccountID, Where<Customer.consolidatingBAccountID, Equal<Current<SOAdjust.customerID>>>>>>>,
			  And<SOOrder.orderType, Equal<Optional<SOAdjust.adjdOrderType>>,
			  And<SOOrder.openDoc, Equal<boolTrue>,
			  And2<Where<SOOrderType.aRDocType, Equal<ARDocType.invoice>, Or<SOOrderType.aRDocType, Equal<ARDocType.debitMemo>>>,
			  And<SOOrder.orderDate, LessEqual<Current<ARPayment.adjDate>>,
			  And<Terms.installmentType, NotEqual<TermsInstallmentType.multiple>>>>>>>>),
				typeof(SOOrder.orderNbr),
				typeof(SOOrder.customerOrderNbr),
				typeof(SOOrder.orderDate),
				typeof(SOOrder.finPeriodID),
				typeof(SOOrder.customerLocationID),
				typeof(SOOrder.curyID),
				typeof(SOOrder.curyOrderTotal),
				typeof(SOOrder.curyOpenOrderTotal),
				typeof(SOOrder.status),
				typeof(SOOrder.dueDate),
				typeof(SOOrder.invoiceNbr),
				typeof(SOOrder.orderDesc),
				Filterable = true)]
		public virtual void _ (Events.CacheAttached<SOAdjust.adjdOrderNbr> e) {}
        #endregion

        #region Staic Methods
		/// <summary>
		/// Get the first record of invoice nbr. from bank diposit.
		/// </summary>
		/// <param name="adjgDocType"></param>
		/// <param name="adjgRefNbr"></param>
		/// <returns></returns>
		public static string GetSOInvoiceNbr(string adjgDocType, string adjgRefNbr)
        {
			return SelectFrom<ARInvoice>.InnerJoin<ARAdjust>.On<ARAdjust.adjdDocType.IsEqual<ARInvoice.docType>
																.And<ARAdjust.adjdRefNbr.IsEqual<ARInvoice.refNbr>>>
										.Where<ARAdjust.adjgDocType.IsEqual<@P.AsString>
												.And<ARAdjust.adjgRefNbr.IsEqual<@P.AsString>>>.View.Select(new PXGraph(), adjgDocType, adjgRefNbr).TopFirst?.InvoiceNbr;
        }
        #endregion
    }
}
