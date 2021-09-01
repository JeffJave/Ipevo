using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

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
                        Filter = $"Customer_number eq {filter.Customer_number}"
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

        /// <summary> Call DCL To Create Shipment </summary>
        public static LumAPIResultModel CallDCLToCreateShipment(LUMVendCntrlSetup setup, DCLShipmentRequestEntity metadataShipemt)
        {
            var config = new DCL_Config()
            {
                RequestMethod = HttpMethod.Post,
                RequestUrl = setup.SecureURLbatches,
                AuthType = setup.AuthType,
                Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{setup.ClientID}:{setup.ClientSecret}"))
            };
            var caller = new APICaller(config);
            return caller.CallApi(metadataShipemt);
        }

        /// <summary> Get Correct InventoryCD  </summary>
        public static string GetCorrectInventoryCD(int? _inventoryID)
        {
            var itemData = PX.Objects.IN.InventoryItem.PK.Find(new PX.Data.PXGraph(), _inventoryID);
            var xrefData = SelectFrom<PX.Objects.IN.INItemXRef>
                           .Where<PX.Objects.IN.INItemXRef.inventoryID.IsEqual<P.AsInt>>
                           .View.Select(new PX.Data.PXGraph(), _inventoryID).RowCast<PX.Objects.IN.INItemXRef>().FirstOrDefault();
            if (xrefData != null && xrefData.AlternateType == "GLBL" & !string.IsNullOrEmpty(xrefData.AlternateID))
                return xrefData.AlternateID;
            else
                return itemData.InventoryCD;
        }

    }
}
