using ExternalLogisticsAPI.DAC;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
            Base.action.AddMenuAction(LumLOBMailPaperInvoice);
            LumLOBMailPaperInvoice.SetEnabled(false);

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.CountryID == "US" || curCoutry?.BaseCuryID == "USD")
            {
                LumLOBMailPaperInvoice.SetEnabled(true);
            }
        }

        #region Events
        protected virtual void ARInvoice_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ARInvoice aRInvoice = (ARInvoice)e.Row;
            if (aRInvoice == null) return;

            //show Send Papper Invoice or not
            bool needPapperInvoice = (SelectFrom<CSAnswers>.
                                      LeftJoin<Customer>.On<Customer.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<PapperInvoiceAttr>>>.
                                      Where<Customer.bAccountID.IsEqual<ARInvoice.customerID.FromCurrent>>.View.Select(Base).TopFirst?.Value) != null ? true : false;
            if (needPapperInvoice) LumLOBMailPaperInvoice.SetEnabled(true);
            else LumLOBMailPaperInvoice.SetEnabled(false);
        }
        #endregion

        #region Action
        public PXAction<ARInvoice> LumLOBMailPaperInvoice;
        [PXButton]
        [PXUIField(DisplayName = "Send Paper Invoice", Enabled = true, MapEnableRights = PXCacheRights.Select)]
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

                //Report Paramenters
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["RefNbr"] = Base.Document.Current.RefNbr;
                parameters["DocType"] = Base.Document.Current.DocType;

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
                    if (Directory.Exists(filePath)) Directory.Delete(filePath);
                    System.IO.File.WriteAllBytes(filePath, data);
                    fileArray[i] = filePath;
                }
                
                mergePDFFiles(fileArray, rootBasePath, fileBaseName);

                PXLongOperation.StartOperation(Base, async () =>
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

                            var response = await httpClient.SendAsync(request);

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                            }
                        }
                    }
                });
            }
            return adapter.Get();
        }
        #endregion

        #region Private Method
        /// <summary> 合併PDF檔(集合) </summary> 
        /// <param name="fileList">欲合併PDF檔之集合(一筆以上)</param>
        /// <param name="outMergeFile">合併後的檔名</param> 
        private void mergePDFFiles(string[] fileList, string rootPath, string outMergeFile)
        {
            outMergeFile = Path.Combine(rootPath, outMergeFile);
            FileStream fileStream = new FileStream(outMergeFile, FileMode.Create);
            PdfReader reader;
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, fileStream);
            document.Open();
            document.SetPageSize(PageSize.LETTER);
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage newPage;
            for (int i = 0; i < fileList.Length; i++)
            {
                reader = new PdfReader(Path.Combine(rootPath, fileList[i]));
                int iPageNum = reader.NumberOfPages;
                for (int j = 1; j <= iPageNum; j++)
                {
                    document.NewPage();
                    newPage = writer.GetImportedPage(reader, j);
                    cb.AddTemplate(newPage, 0, 0);
                }
            }
            document.Close();
            writer.Close();
            fileStream.Close();
        }
        #endregion
    }
}
