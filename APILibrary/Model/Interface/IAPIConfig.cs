using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model.Interface
{
    public interface IAPIConfig
    {
        HttpMethod RequestMethod { get; set; }
        string RequestUrl { get; set; }
        string AuthType { get; set; }
        string Token { get;set; }
        string AuthorizationToken { get; }
    }
}
