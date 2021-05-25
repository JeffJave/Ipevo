using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.SO;

namespace PX.Objects.AR
{
    public class ARPaymentEntryExt : PXGraphExtension<ARPaymentEntry>
    {
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
		public virtual void _ (Events.CacheAttached<SOAdjust.adjdOrderNbr> e){}
    }
}
