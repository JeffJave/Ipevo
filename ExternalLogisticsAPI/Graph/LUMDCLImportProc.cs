using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.Common.Extensions;
using PX.SM;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMDCLImportProc : PXGraph<LUMDCLImportProc>
    {
        [PXCacheName("Document")]
        public SelectFrom<LUMVendCntrlProcessOrder>.View Document;

        [PXHidden]
        public SelectFrom<LUMVendCntrlSetup>.View DCLSetup;

        public PXSave<LUMVendCntrlProcessOrder> Save;
        public PXCancel<LUMVendCntrlProcessOrder> Cancel;

        public PXAction<LUMVendCntrlProcessOrder> prepareImport;

        [PXButton]
        [PXUIField(DisplayName = "Prepare Import", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable PrepareImport(PXAdapter adapter)
        {
            var setup = this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault();
            DCL_Config config = new DCL_Config()
            {
                RequestMethod = HttpMethod.Get,
                RequestUrl = setup.SecureURL + "?received_from=2021-05-12",
                AuthType = "Basic",
                Token = "aWV2ODAxOlRyYW5zZmVyQDE="
            };

            APICaller caller = new APICaller(config);
            var result = caller.CallApi();
            if (result.StatusCode == HttpStatusCode.OK)
            {
                int count = 1;
                var _orders = JsonConvert.DeserializeObject<OrderResponse>(result.ContentResult);
                foreach (var orders in _orders.orders)
                {
                    var temp = this.Document.Insert((LUMVendCntrlProcessOrder) this.Document.Cache.CreateInstance());
                    temp.LineNumber = count++;
                    temp.OrderID = orders.order_number;
                    temp.CustomerID = setup.CustomerID?.ToString();
                    temp.OrderDate = DateTime.Parse(orders.ordered_date);
                    temp.OrderStatusID = orders.order_status.ToString();
                    temp.OrderAmount = orders.order_lines.FirstOrDefault()?.quantity;
                    temp.SalesTaxAmt = orders.order_lines.FirstOrDefault()?.price;
                    temp.LastUpdated = DateTime.Parse(orders.modified_at);
                }

                this.Save.Press();
            }

            return adapter.Get();
        }
    }
}
