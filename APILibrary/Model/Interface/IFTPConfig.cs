using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model.Interface
{
    public interface IFTPConfig
    {
        string FtpHost { get;set;}
        string FtpUser { get;set;}
        string FtpPass { get;set;}
        string FtpPath { get;set;}
        string FtpPort { get;set;}
    }
}
