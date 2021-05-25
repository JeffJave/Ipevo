using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APILibrary.Model.Interface;

namespace APILibrary.Model
{
    public class DCL_Config : IAPIConfig
    {
        /// <summary> API Method </summary>
        public HttpMethod RequestMethod { get; set; }

        /// <summary> Target API EndPoint </summary>
        public string RequestUrl { get; set; }

        /// <summary> Authorization Type </summary>
        public string AuthType { get; set; }

        /// <summary> Authorization secret key </summary>
        public string Token { get; set; }

        /// <summary> Combine Authorization Token </summary>
        public string AuthorizationToken
        {
            get => $"{this.AuthType} {this.Token}";
        }
    }
}
