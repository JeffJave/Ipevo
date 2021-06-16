using APILibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary
{
    public class FTPHelper
    {
        protected IFTPConfig config;
        public FTPHelper(IFTPConfig _config)
            => this.config = _config;

        /// <summary> Upload File by FTP </summary>
        public bool UploadFileToFTP(byte[] data, string fileName)
        {
            string url = $"ftp://{config.FtpHost}:{config.FtpPort}{config.FtpPath}{fileName}";
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            // setting Method
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            // setting upload type
            ftpRequest.UseBinary = true;
            // setting user & PW
            ftpRequest.Credentials = new NetworkCredential(config.FtpUser, config.FtpPass);
            // setting keepAlive
            ftpRequest.KeepAlive = false;
            // setting passive
            ftpRequest.UsePassive = true;
            // setting requestStream
            var reqStream = ftpRequest.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();
            reqStream.Dispose();
            // Upload File
            var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            var IsSuccess = ftpResponse.StatusCode == FtpStatusCode.ClosingData;
            // Release
            ftpResponse.Close();
            ftpRequest.Abort();
            return IsSuccess;
        }

    }
}
