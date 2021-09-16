using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        public LumAPIResultModel GetAuthToken(string _id, string pwd)
        {
            using (HttpClient client = new HttpClient())
            {
                // Setting Request
                HttpRequestMessage _request =
                new HttpRequestMessage(this.config.RequestMethod, this.config.RequestUrl);
                _request.Content = new StringContent(JsonConvert.SerializeObject(new { email = _id, password = pwd }), Encoding.UTF8,
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

        public LumAPIResultModel CallApi<T>(T parameterObj, string userName, string pwd, string token) where T : class
        {
            using (HttpClient client = new HttpClient())
            {
                // Setting Header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Setting Request
                HttpRequestMessage _request =
                    new HttpRequestMessage(this.config.RequestMethod, this.config.RequestUrl);
                //_request.Headers.Add("Content-Type", "application/json");
                _request.Headers.Add("UserName", userName);
                _request.Headers.Add("Password", pwd);
                _request.Headers.Add("AccessLicenseNumber", token);

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

    public static class APIHelper
    {
        public static string CombineQueryString<T>(string _url, T param)
        {
            var properties = from p in param.GetType().GetProperties()
                             where p.GetValue(param, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(param, null).ToString());
            return $"{_url}?{string.Join("&", properties.ToArray())}";
        }

        public static string GetJsonString<T>(T _obj) where T : class
        {
            return JsonConvert.SerializeObject(_obj);
        }

        public static T GetObjectFromString<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
