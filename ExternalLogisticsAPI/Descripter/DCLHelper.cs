using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;

namespace ExternalLogisticsAPI.Descripter
{
    public static class DCLHelper
    {
        /// <summary> Call DCL To Get SO. By Filter </summary>
        public static LumAPIResultModel CallDCLToGetSOByFilter(LUMVendCntrlSetup setup, DCLFilter filter)
        {
            var config = new DCL_Config()
            {
                RequestMethod = HttpMethod.Get,
                RequestUrl = APIHelper.CombineQueryString(
                    setup.SecureURL,
                    new
                    {
                        Received_from = filter.Received_from?.ToString("yyyy-MM-dd"),
                        Received_to = filter.Received_to?.ToString("yyyy-MM-dd"),
                        Filter = $"Customer_number eq {filter.Customer_number}, order_stage eq 60"
                    }),
                AuthType = setup.AuthType,
                Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{setup.ClientID}:{setup.ClientSecret}"))
            };
            var caller = new APICaller(config);
            return caller.CallApi();
        }

        /// <summary> Call DCL To Get SO. By OrderNumbers </summary>
        public static LumAPIResultModel CallDCLToGetSOByOrderNumbers(LUMVendCntrlSetup setup, string _orderNumbers)
        {
            // If No _orderNumbers then query NoN
            _orderNumbers = string.IsNullOrEmpty(_orderNumbers) ? "NoNeedToQuery" : _orderNumbers;
            var config = new DCL_Config()
            {
                RequestMethod = HttpMethod.Get,
                RequestUrl = APIHelper.CombineQueryString(
                    setup.SecureURL,
                    new
                    {
                        order_numbers = _orderNumbers
                    }),
                AuthType = setup.AuthType,
                Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{setup.ClientID}:{setup.ClientSecret}"))
            };
            var caller = new APICaller(config);
            return caller.CallApi();
        }

    }
}
