using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    public class MiddleWareSingInModel
    {
        public bool Status { get; set; }
        public TokenResult data { get; set; }
        public string Message { get; set; }
    }

    public class TokenResult
    {
        public string Jwt { get; set; }
        public int Expiration_ts { get; set; }
    }
}
