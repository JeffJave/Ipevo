using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using APILibrary.Model;
using APILibrary.Model.Interface;
using Newtonsoft.Json;

namespace APILibrary
{
    public class APICaller
    {
        protected IAPIConfig config;

        public APICaller(IAPIConfig _config)
            => this.config = _config;

        public LumAPIResultModel CallApi()
        {
            return this.CallApi<object>(null);
        }

        public LumAPIResultModel CallApi<T>(T parameterObj) where T : class
        {
            if (this.config == null)
                throw new Exception("Config can not be null");

            using (HttpClient client = new HttpClient())
            {
                // Setting Authorization
                client.DefaultRequestHeaders.Add("Authorization", $"{this.config.AuthorizationToken}");

                // Setting Request
                HttpRequestMessage _request =
                    new HttpRequestMessage(this.config.RequestMethod, this.config.RequestUrl);

                // Setting Parameter
                if (parameterObj != null)
                    _request.Content = new StringContent(JsonConvert.SerializeObject(parameterObj), Encoding.UTF8,
                        "application/json");

                // Get Result
                HttpResponseMessage _response = client.SendAsync(_request).GetAwaiter().GetResult();

                // Return Result
                return new LumAPIResultModel()
                {
                    StatusCode = _response.StatusCode,
                    Content = _response.Content,
                    ContentResult = _response.Content.ReadAsStringAsync().Result
                };
            }
        }
    }
}
