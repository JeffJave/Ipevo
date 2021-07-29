using PX.Data;
using PX.Data.BQL;
using PX.Objects.AR;
using PX.Objects.CS;
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
            baseMthod?.Invoke(aRCmd, filter);
            switch (filter.Action)
            {
                // LOB API
                case "SO302000$sendLOBPaperInvoice":
                    aRCmd.Join<LeftJoin<Customer, On<Customer.bAccountID, Equal<ARInvoice.customerID>>>>();
                    aRCmd.Join<LeftJoin<CSAnswers, On<CSAnswers.refNoteID, Equal<Customer.noteID>, And<CSAnswers.attributeID, Equal<PapperInvoiceAttr>>>>>();
                    aRCmd.WhereAnd<Where<ARInvoiceExt.usrLOBSent, Equal<False>,
                        Or<ARInvoiceExt.usrLOBSent, IsNull>>>();
                    break;
            }
        }
        
    }

    public class PapperInvoiceAttr : BqlString.Constant<PapperInvoiceAttr>
    {
        public PapperInvoiceAttr() : base("PAPERINV") { }
    }
}
