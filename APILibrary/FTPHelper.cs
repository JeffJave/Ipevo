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

        /// <summary> Load File From FTP </summary>
        public Dictionary<string, string> LoadFileStringFromFTP(string _fileName = null)
        {
            var readResult = new Dictionary<string, string>();
            List<string> fileNames = new List<string>();
            if (string.IsNullOrEmpty(_fileName))
                fileNames = ListFTPAllFiles(this.config.FtpPath);
            else
                fileNames.Add(_fileName);

            foreach (var name in fileNames)
            {
                string url = $"ftp://{config.FtpHost}:{config.FtpPort}{config.FtpPath}{name}";
                var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                // setting Method
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                // setting upload type
                ftpRequest.UseBinary = true;
                // setting user & PW
                ftpRequest.Credentials = new NetworkCredential(config.FtpUser, config.FtpPass);
                // setting keepAlive
                ftpRequest.KeepAlive = false;
                // setting passive
                ftpRequest.UsePassive = true;
                // Get List FTP Directory
                using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream, true))
                        {
                            string[] allLines = reader.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            readResult.Add(name, allLines[1]);
                        }
                    }
                }
            }
            return readResult;
        }

        /// <summary> Get FTP Folder File name </summary>
        public List<string> ListFTPAllFiles(string _path)
        {
            var dir = new List<string>();
            string url = $"ftp://{config.FtpHost}:{config.FtpPort}{_path}";
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            // setting Method
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            // setting upload type
            ftpRequest.UseBinary = true;
            // setting user & PW
            ftpRequest.Credentials = new NetworkCredential(config.FtpUser, config.FtpPass);
            // setting keepAlive
            ftpRequest.KeepAlive = false;
            // setting passive
            ftpRequest.UsePassive = true;
            // Get List FTP Directory
            using (var response = (FtpWebResponse)ftpRequest.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, true))
                    {
                        while (!reader.EndOfStream)
                        {
                            dir.Add(reader.ReadLine());
                        }
                    }
                }
            }
            return dir;
        }

        /// <summary> Delete FTP File </summary>
        public bool DeleteFTPFile(string _fileName)
        {
            string url = $"ftp://{config.FtpHost}:{config.FtpPort}{config.FtpPath}{_fileName}";
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            // setting Method
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            // setting upload type
            ftpRequest.UseBinary = true;
            // setting user & PW
            ftpRequest.Credentials = new NetworkCredential(config.FtpUser, config.FtpPass);
            // setting keepAlive
            ftpRequest.KeepAlive = false;
            // setting passive
            ftpRequest.UsePassive = true;
            // Get List FTP Directory
            var response = (FtpWebResponse)ftpRequest.GetResponse();
            response.Close();
            return true;
        }
    }
}
