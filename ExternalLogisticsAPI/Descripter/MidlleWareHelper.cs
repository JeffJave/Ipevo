using APILibrary;
using APILibrary.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLogisticsAPI.Descripter
{
    public static class MiddleWareHelper
    {
        public static LumAPIResultModel GetToken(LUMMiddleWareSetup setup)
        {
            var config = new MiddleWare_Config()
            {
                RequestMethod = HttpMethod.Post,
                RequestUrl = setup.SecureURL_login
            };
            var caller = new APICaller(config);
            return caller.GetAuthToken(setup.ClientID, setup.ClientSecret);
        }

        public static LumAPIResultModel CallMiddleWareToUpdateFBM(LUMMiddleWareSetup setup, MiddleWare_Shipment metadataShipemt)
        {
            // Get Middle ware Token
            var tokenResult = MiddleWareHelper.GetToken(setup);
            if (tokenResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception($"Can not Get Midlle Ware Token, status code:{tokenResult.StatusCode} ");
            setup.AuthToken = JsonConvert.DeserializeObject<MiddleWareSingInModel>(tokenResult.ContentResult).data.Jwt;

            var config = new MiddleWare_Config()
            {
                RequestMethod = HttpMethod.Post,
                RequestUrl = setup.SecureURL_fbm,
                Token = setup.AuthToken
            };
            var caller = new APICaller(config);
            return caller.CallApi(metadataShipemt);
        }

    }
}
