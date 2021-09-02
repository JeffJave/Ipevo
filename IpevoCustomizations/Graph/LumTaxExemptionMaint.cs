using IpevoCustomizations.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.SM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpevoCustomizations.Graph
{
    public class LumTaxExemptionMaint : PXGraph<LumTaxExemptionMaint>
    {
        public PXCancel<LumTaxExemptionCcertificate> Cancel;

        public SelectFrom<LumTaxExemptionCcertificate>
              .LeftJoin<LumTaxExemptionCcertificateLog>.On<LumTaxExemptionCcertificate.customerID.IsEqual<LumTaxExemptionCcertificateLog.customerID>
                          .And<LumTaxExemptionCcertificate.filePath.IsEqual<LumTaxExemptionCcertificateLog.filePath>>>.View UploadFileList;

        [PXHidden]
        public SelectFrom<LumTaxExemptionCcertificateLog>.View uploadLog;

        #region Action

        public PXAction<LumTaxExemptionCcertificate> lumLoadFile;
        [PXButton]
        [PXUIField(DisplayName = "LOAD FILE", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LumLoadFile(PXAdapter adapter)
        {
            var dirPath = @"C:\Storage\TaxExemptionCcertificate\";
            //var dirPath = @"D:\uploadTest\";
            var driInfo = new System.IO.DirectoryInfo(dirPath);
            System.IO.FileInfo[] Files = driInfo.GetFiles("*.pdf"); //Getting Text files
            var customerData = SelectFrom<Customer>.View.Select(this).RowCast<Customer>().ToList();
            // Delete temp table data
            PXDatabase.Delete<LumTaxExemptionCcertificate>();
            this.UploadFileList.Cache.Clear();
            foreach (var item in Files)
            {
                if (!customerData.Any(x => x.AcctCD.ToUpper().Trim() == item.Name.ToUpper().Split('.')[0]))
                    continue;
                var model = this.UploadFileList.Insert((LumTaxExemptionCcertificate)this.UploadFileList.Cache.CreateInstance());
                model.FilePath = item.FullName;
                model.CustomerID = item.Name.Split('.')[0];
            }
            this.Actions.PressSave();
            return adapter.Get();
        }

        public PXAction<LumTaxExemptionCcertificate> lumUploadAllFile;
        [PXButton]
        [PXUIField(DisplayName = "UPLOAD ALL FILE", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable LumUploadAllFile(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate ()
            {
                // Upload Graph
                UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                //var datas = this.UploadFileList.Select().RowCast<LumTaxExemptionCcertificate>().ToList();
                foreach (var item in this.UploadFileList.Select())
                {
                    var impData = item.GetItem<LumTaxExemptionCcertificate>();
                    var logData = item.GetItem<LumTaxExemptionCcertificateLog>();
                    try
                    {
                        if ((logData.IsProcess ?? false))
                            continue;
                        // Get current Customer Data
                        var customerData = SelectFrom<Customer>.Where<Customer.acctCD.IsEqual<P.AsString>>.View.Select(this, impData.CustomerID).RowCast<Customer>().FirstOrDefault();
                        if (customerData == null)
                            throw new Exception("Can not find Customer Data");
                        byte[] filebyte = System.IO.File.ReadAllBytes(impData.FilePath);
                        // Create SM.FileInfo
                        var fileName = $"{impData.CustomerID}.pdf";
                        FileInfo fi = new FileInfo(fileName, null, filebyte);
                        // upload file to Attachment
                        upload.SaveFile(fi);
                        // Create Customer Graph
                        var graph = PXGraph.CreateInstance<CustomerMaint>();
                        graph.BAccount.Current = customerData;
                        PXNoteAttribute.SetFileNotes(graph.BAccount.Cache, graph.BAccount.Current, fi.UID.Value);
                        graph.Save.Press();
                        InsertImpLog(impData, true, null);
                    }
                    catch (Exception ex)
                    {
                        InsertImpLog(impData, false, ex.Message);
                    }
                }
                this.Actions.PressSave();
            });
            return adapter.Get();
        }
        #endregion

        /// <summary> Insert Log Data </summary>
        public void InsertImpLog(LumTaxExemptionCcertificate impData, bool success, string errMsg)
        {
            // Insert Or Updated
            var IsUpdated = this.uploadLog
                .Select()
                .RowCast<LumTaxExemptionCcertificateLog>()
                .Any(x => x.FilePath == impData.FilePath && x.CustomerID == impData.CustomerID);
            var model =
                this.uploadLog
                    .Select()
                    .RowCast<LumTaxExemptionCcertificateLog>()
                    .FirstOrDefault(x => x.FilePath == impData.FilePath && x.CustomerID == impData.CustomerID) ?? this.uploadLog.Insert((LumTaxExemptionCcertificateLog)this.uploadLog.Cache.CreateInstance());
            model.CustomerID = impData.CustomerID;
            model.FilePath = impData.FilePath;
            model.IsProcess = success;
            model.ErrorMsg = success ? null : errMsg;
            if (IsUpdated)
                this.uploadLog.Cache.Update(model);
        }

    }
}
