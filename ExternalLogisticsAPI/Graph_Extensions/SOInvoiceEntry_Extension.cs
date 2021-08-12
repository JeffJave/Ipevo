using ExternalLogisticsAPI.DAC;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Reports;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Reports.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PX.Objects.SO
{
    public class SOInvoiceEntry_Extension : PXGraphExtension<SOInvoiceEntry>
    {
        public class PapperInvoiceAttr : BqlString.Constant<PapperInvoiceAttr>
        {
            public PapperInvoiceAttr() : base("PAPERINV") { }
        }

        public class InvoicePapperPageAttr : BqlString.Constant<InvoicePapperPageAttr>
        {
            public InvoicePapperPageAttr() : base("INVPRTPAGE") { }
        }
        public override void Initialize()
        {
            base.Initialize();

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.CountryID == "US" || curCoutry?.BaseCuryID == "USD")
            {
                LumLOBMailPaperInvoice.SetVisible(true);
                Base.action.AddMenuAction(LumLOBMailPaperInvoice);
            }
        }

        #region Events
        protected virtual void ARInvoice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARInvoice aRInvoice = (ARInvoice)e.Row;
            if (aRInvoice == null) return;

            //show Send Papper Invoice or not
            //Type = INV and Attribute clicked
            bool needPapperInvoice = (SelectFrom<CSAnswers>.
                                      LeftJoin<Customer>.On<Customer.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<PapperInvoiceAttr>>>.
                                      Where<Customer.bAccountID.IsEqual<ARInvoice.customerID.FromCurrent>>.View.Select(Base).TopFirst?.Value) != null ? true : false;
            if (aRInvoice.GetExtension<ARInvoiceExt>()?.UsrLOBSent == true) LumLOBMailPaperInvoice.SetEnabled(false);
            else if (needPapperInvoice && aRInvoice.DocType == "INV") LumLOBMailPaperInvoice.SetEnabled(true);
            else LumLOBMailPaperInvoice.SetEnabled(false);
        }
        #endregion

        #region Action
        public PXAction<ARInvoice> LumLOBMailPaperInvoice;
        [PXButton]
        [PXUIField(DisplayName = "Send Paper Invoice", Enabled = false, Visible = false, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable lumlOBMailPaperInvoice(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                //Get API Setting from Sales Chanel Preferences LOB tab
                var requestBaseInfo = SelectFrom<LUMLobAPISetup>.View.Select(Base).TopFirst;
                if (requestBaseInfo?.IsProd == null || requestBaseInfo?.Lobapiurl == null || requestBaseInfo?.AuthCode_Test == null || requestBaseInfo?.AuthCode_Prod == null)
                {
                    throw new PXException("Please set some base information in Sales Chanel Preferences.");
                }

                //Check it sent or not
                if (Base.Document.Current.GetExtension<ARInvoiceExt>().UsrLOBSent == true)
                {
                    throw new PXException("It is already sent to LOB paper invoice.");
                }

                //Report Paramenters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                parameters["DocType"] = Base.Document.Current.DocType;
                parameters["ToLobAPI"] = "1"; //Controll the ClickToPay icoon

                //Report Processing
                PX.Reports.Controls.Report _report = PXReportTools.LoadReport("LM606405", null);
                PXReportTools.InitReportParameters(_report, parameters, PXSettingProvider.Instance.Default);
                ReportNode reportNode = ReportProcessor.ProcessReport(_report);

                //Generation PDF
                byte[] data = PX.Reports.Mail.Message.GenerateReport(reportNode, ReportProcessor.FilterPdf).First();

                //Set PDF Path
                var fileBaseName = $"Invoice Report {Base.Document.Current.RefNbr}.pdf";
                var rootBasePath = System.Web.HttpContext.Current.Server.MapPath("~/PDF4LobTemp/");

                //Create Directory if PDF4Lob is not exist
                if (!Directory.Exists(rootBasePath)) Directory.CreateDirectory(rootBasePath);

                //Get Billing Info
                PXResult<ARInvoice, ARAddress, ARContact> billingInfo = (PXResult<ARInvoice, ARAddress, ARContact>)SelectFrom<ARInvoice>.
                                    LeftJoin<ARAddress>.On<ARAddress.addressID.IsEqual<ARInvoice.billAddressID.FromCurrent>>.
                                    LeftJoin<ARContact>.On<ARContact.contactID.IsEqual<ARInvoice.billContactID.FromCurrent>>.
                                    Where<ARInvoice.refNbr.IsEqual<ARInvoice.refNbr.FromCurrent>.And<ARInvoice.docType.IsEqual<ARInvoice.docType.FromCurrent>>>.
                                    View.Select(Base);

                //Recreate content
                int printTimesDefault = 1;
                int printTimes = int.TryParse(SelectFrom<CSAnswers>.
                                              LeftJoin<Customer>.On<Customer.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<InvoicePapperPageAttr>>>.
                                              Where<Customer.bAccountID.IsEqual<ARInvoice.customerID.FromCurrent>>.View.Select(Base).TopFirst?.Value, out printTimes) ? printTimes : printTimesDefault;
                
                string[] fileArray = new string[printTimes];
                for (int i = 0; i < printTimes; i++)
                {
                    var fileName = $"Invoice Report {Base.Document.Current.RefNbr}_{i}.pdf";
                    var filePath = Path.Combine(rootBasePath, fileName);
                    //Delete Orig File
                    if (File.Exists(filePath)) File.Delete(filePath);
                    System.IO.File.WriteAllBytes(filePath, data);
                    fileArray[i] = filePath;
                }

                string outputFilePath = Path.Combine(rootBasePath, fileBaseName);
                //Check file
                if (File.Exists(outputFilePath)) File.Delete(Path.Combine(outputFilePath));
                MergeMultiplePDFIntoSinglePDF(outputFilePath, fileArray);

                PXLongOperation.StartOperation(Base, () =>
                {
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.lob.com/v1/letters"))
                        {
                            //Setting Authoriztion Information - Default is Test Env.
                            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(requestBaseInfo?.AuthCode_Test + ":"));
                            if (requestBaseInfo?.IsProd == "P") base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(requestBaseInfo?.AuthCode_Prod + ":"));

                            request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                            var multipartContent = new MultipartFormDataContent();
                            multipartContent.Add(new StringContent($"Invoice - {Base.Document.Current.RefNbr}"), "description");
                            multipartContent.Add(new StringContent("ACCOUNTS PAYABLE"), "to[name]");
                            multipartContent.Add(new StringContent($"{((ARContact)billingInfo).FullName}"), "to[company]");
                            multipartContent.Add(new StringContent($"{((ARAddress)billingInfo).AddressLine1}"), "to[address_line1]");
                            multipartContent.Add(new StringContent($"{((ARAddress)billingInfo).City}"), "to[address_city]");
                            multipartContent.Add(new StringContent($"{((ARAddress)billingInfo).State}"), "to[address_state]");
                            multipartContent.Add(new StringContent($"{((ARAddress)billingInfo).CountryID}"), "to[address_country]");
                            multipartContent.Add(new StringContent($"{((ARAddress)billingInfo).PostalCode}"), "to[address_zip]");
                            multipartContent.Add(new StringContent(requestBaseInfo?.ShipFrom_Name != null ? requestBaseInfo?.ShipFrom_Name : "IPEVO"), "from[name]");
                            multipartContent.Add(new StringContent(requestBaseInfo?.ShipFrom_Address != null ? requestBaseInfo?.ShipFrom_Address : "4000 Pimlico Dr, Suite 114-119"), "from[address_line1]");
                            multipartContent.Add(new StringContent(requestBaseInfo?.ShipFrom_City != null ? requestBaseInfo?.ShipFrom_City : "Pleasanton"), "from[address_city]");
                            multipartContent.Add(new StringContent(requestBaseInfo?.ShipFrom_State != null ? requestBaseInfo?.ShipFrom_State : "CA"), "from[address_state]");
                            multipartContent.Add(new StringContent(requestBaseInfo?.ShipFrom_Country != null ? requestBaseInfo?.ShipFrom_Country : "US"), "from[address_country]");
                            multipartContent.Add(new StringContent(requestBaseInfo?.ShipFrom_Zip != null ? requestBaseInfo?.ShipFrom_Zip : "94588"), "from[address_zip]");
                            multipartContent.Add(new ByteArrayContent(File.ReadAllBytes($"{rootBasePath}{fileBaseName}")), "file", Path.GetFileName($"{rootBasePath}{fileBaseName}"));
                            multipartContent.Add(new StringContent("true"), "color");
                            multipartContent.Add(new StringContent("false"), "double_sided");
                            multipartContent.Add(new StringContent("insert_blank_page"), "address_placement");
                            request.Content = multipartContent;

                            var response = httpClient.SendAsync(request).GetAwaiter().GetResult();

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                ARInvoice aRInvoice = Base.Document.Current;
                                aRInvoice.GetExtension<ARInvoiceExt>().UsrLOBSent = true;
                                Base.Document.Cache.Update(aRInvoice);
                                //enable button
                                LumLOBMailPaperInvoice.SetEnabled(false);
                                Base.Save.PressButton();

                                //Delete separate files
                                foreach (var file in fileArray) if (File.Exists(file)) File.Delete(file);
                                //Delete combine file
                                if (File.Exists(outputFilePath)) File.Delete(Path.Combine(outputFilePath));
                            }
                        }
                    }
                });
            }
            return adapter.Get();
        }
        #endregion

        #region Private Method
        private static void MergeMultiplePDFIntoSinglePDF(string outputFilePath, string[] pdfFiles)
        {
            var size = PageSizeConverter.ToSize(PdfSharp.PageSize.Letter);
            PdfDocument outputPDFDocument = new PdfDocument();
            foreach (string pdfFile in pdfFiles)
            {
                PdfDocument inputPDFDocument = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import);
                outputPDFDocument.Version = inputPDFDocument.Version;
                foreach (PdfPage page in inputPDFDocument.Pages)
                {
                    page.Width = size.Width;
                    page.Height = size.Height;
                    outputPDFDocument.AddPage(page);
                }
            }
            outputPDFDocument.Save(outputFilePath);
        }
        #endregion
    }
}
