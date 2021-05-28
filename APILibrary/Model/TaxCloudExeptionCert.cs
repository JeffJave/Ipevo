using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    public class TaxCloudExeptionCert
    {
        public class ExemptState
        {
            public string IdentificationNumber { get; set; }
            public string ReasonForExemption { get; set; }
            public string StateAbbr { get; set; }
        }

        public class PurchaserTaxID
        {
            public string IDNumber { get; set; }
            public string StateOfIssue { get; set; }
            public string TaxType { get; set; }
        }

        public class Detail
        {
            public string CreatedDate { get; set; }
            public List<ExemptState> ExemptStates { get; set; }
            public string PurchaserAddress1 { get; set; }
            public string PurchaserAddress2 { get; set; }
            public string PurchaserBusinessType { get; set; }
            public string PurchaserBusinessTypeOtherValue { get; set; }
            public string PurchaserCity { get; set; }
            public string PurchaserExemptionReason { get; set; }
            public string PurchaserExemptionReasonOtherValue { get; set; }
            public string PurchaserFirstName { get; set; }
            public string PurchaserLastName { get; set; }
            public string PurchaserState { get; set; }
            public PurchaserTaxID PurchaserTaxID { get; set; }
            public string PurchaserTitle { get; set; }
            public string PurchaserZip { get; set; }
        }

        public class ExemptCert
        {
            public string CertificateID { get; set; }
            public Detail Detail { get; set; }
        }

        public class Root
        {
            public string apiKey { get; set; }
            public string apiLoginID { get; set; }
            public string customerID { get; set; }
            public ExemptCert exemptCert { get; set; }
        }
    }
}
