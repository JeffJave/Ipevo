using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ExemptState
    {
        public string StateAbbr { get; set; }
        public string StateAbbreviation { get; set; }
        public string ReasonForExemption { get; set; }
        public string IdentificationNumber { get; set; }
    }

    public class PurchaserTaxID
    {
        public string TaxType { get; set; }
        public string IDNumber { get; set; }
        public object StateOfIssue { get; set; }
    }

    public class Detail
    {
        public List<ExemptState> ExemptStates { get; set; }
        public bool SinglePurchase { get; set; }
        public string SinglePurchaseOrderNumber { get; set; }
        public string PurchaserFirstName { get; set; }
        public string PurchaserLastName { get; set; }
        public object PurchaserTitle { get; set; }
        public string PurchaserAddress1 { get; set; }
        public string PurchaserAddress2 { get; set; }
        public string PurchaserCity { get; set; }
        public string PurchaserState { get; set; }
        public string PurchaserZip { get; set; }
        public PurchaserTaxID PurchaserTaxID { get; set; }
        public string PurchaserBusinessType { get; set; }
        public object PurchaserBusinessTypeOtherValue { get; set; }
        public string PurchaserExemptionReason { get; set; }
        public string PurchaserExemptionReasonValue { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ExemptCertificate
    {
        public string CertificateID { get; set; }
        public Detail Detail { get; set; }
    }

    public class Root
    {
        public List<ExemptCertificate> ExemptCertificates { get; set; }
        public int ResponseType { get; set; }
        public List<object> Messages { get; set; }
    }
}
