using APILibrary;
using APILibrary.Model;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLogisticsAPI.Descripter
{
    public static class UPSHelper
    {
        public static LumAPIResultModel GetUPSFreightResult(List<CarrierPluginDetail> setup, APILibrary.Model.UPS.Request.FreightRateRequestRoot freightRateRequest)
        {
            var config = new DCL_Config()
            {
                RequestMethod = HttpMethod.Post,
                RequestUrl = "https://onlinetools.ups.com/ship/v1/freight/rating/ground",
            };
            var caller = new APICaller(config);
            return caller.CallApi(freightRateRequest, 
                                  setup.FirstOrDefault(x => x.DetailID.ToUpper() == "LOGIN").Value,
                                  setup.FirstOrDefault(x => x.DetailID.ToUpper() == "PASSWORD").Value,
                                  setup.FirstOrDefault(x => x.DetailID.ToUpper() == "ACCESSNUM").Value);
        }
    }
}
