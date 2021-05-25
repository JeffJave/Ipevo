using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    public class LumAPIResultModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpContent Content { get; set; }
        public string ContentResult { get; set; }

    }
}
