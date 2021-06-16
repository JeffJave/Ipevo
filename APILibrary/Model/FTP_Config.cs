using APILibrary.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    public class FTP_Config : IFTPConfig
    {
        public string FtpHost { get; set; }
        public string FtpUser { get; set; }
        public string FtpPass { get; set; }
        public string FtpPath { get; set; }
        public string FtpPort { get; set; }
    }
}
