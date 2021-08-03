using PX.Data;
using PX.Data.BQL;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class SOReleaseInvoiceExt : PXGraphExtension<SOReleaseInvoice>
    {
        public delegate void ApplyAdditionalFiltersDelegate(PXSelectBase<ARInvoice> aRCmd, SOInvoiceFilter filter);

        [PXOverride]
        public virtual void ApplyAdditionalFilters(PXSelectBase<ARInvoice> aRCmd, SOInvoiceFilter filter, ApplyAdditionalFiltersDelegate baseMthod)
        {
            if (filter.Action == "SO303000$lumlOBMailPaperInvoice")
            {
                // LOB API
                //case "SO303000$lumlOBMailPaperInvoice":
                aRCmd.WhereAnd<Where<ARInvoice.origModule.IsEqual<BatchModule.moduleSO>>>();
                aRCmd.WhereAnd<Where<WorkflowAction.IsEnabled<ARInvoice, SOInvoiceFilter.action>>>();
                aRCmd.Join<InnerJoin<Customer, On<ARInvoice.FK.Customer>>>();
                aRCmd.WhereAnd<Where<Match<Customer, AccessInfo.userName.FromCurrent>>>();
                aRCmd.WhereAnd<Where<ARInvoice.docDate.IsLessEqual<SOInvoiceFilter.endDate.FromCurrent>>>();

                if (filter.StartDate != null)
                    aRCmd.WhereAnd<Where<ARInvoice.docDate.IsGreaterEqual<SOInvoiceFilter.startDate.FromCurrent>>>();
                if (filter.CustomerID != null)
                    aRCmd.WhereAnd<Where<ARInvoice.customerID.IsEqual<SOInvoiceFilter.customerID.FromCurrent>>>();

                
                aRCmd.Join<InnerJoinSingleTable<CSAnswers, On<CSAnswers.refNoteID, Equal<Customer.noteID>, And<CSAnswers.attributeID, Equal<PapperInvoiceAttr>>>>>();
                aRCmd.WhereAnd<Where<ARInvoice.docType, Equal<INVDocType>>>();
                aRCmd.WhereAnd<Where<ARInvoiceExt.usrLOBSent, Equal<False>, Or<ARInvoiceExt.usrLOBSent, IsNull>>>();
            }
            else
            {
                baseMthod?.Invoke(aRCmd, filter);
            }
        }
        
    }

    public class INVDocType : BqlString.Constant<INVDocType>
    {
        public INVDocType() : base("INV") { }
    }

    public class PapperInvoiceAttr : BqlString.Constant<PapperInvoiceAttr>
    {
        public PapperInvoiceAttr() : base("PAPERINV") { }
    }
}
