using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Descripter;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO
{
    public class SOCreateShipmentExt : PXGraphExtension<SOCreateShipment>
    {
        [PXHidden]
        public SelectFrom<LUMVendCntrlSetup>.View DCLSetup;

        #region Override Mehtod

        public delegate IEnumerable OrdersDeletage();
        public delegate void AddCommonFiltersDelegate(SOOrderFilter filter, PXSelectBase<SOOrder> cmd);

        [PXOverride]
        public virtual void AddCommonFilters(SOOrderFilter filter, PXSelectBase<SOOrder> cmd, AddCommonFiltersDelegate baseMethod)
        {
            baseMethod.Invoke(filter, cmd);
            switch (filter.Action)
            {
                case "SO301000$createDCLShipment":
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrDCLShipmentCreated, Equal<False>,
                        Or<SOOrderExt.usrDCLShipmentCreated, IsNull>>>();
                    break;
                case "SO301000$lumCallDCLShipemnt":
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrDCLShipmentCreated, Equal<True>>>();
                    break;
                case "SO301000$lumGenerate3PLUKFile":
                    cmd.Join<InnerJoin<vSOSiteMapping, On<SOOrder.orderType, Equal<vSOSiteMapping.orderType>, And<SOOrder.orderNbr, Equal<vSOSiteMapping.orderNbr>>>>>();
                    cmd.Join<InnerJoin<INSite, On<vSOSiteMapping.siteid, Equal<INSite.siteID>>>>();
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<INSite.siteCD.IsEqual<P3PLAttr>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrSendToMiddleware, Equal<False>,
                        Or<SOOrderExt.usrSendToMiddleware, IsNull>>>();
                    break;
                case "SO301000$lumGenerateYUSENCAFile":
                    cmd.Join<InnerJoin<vSOSiteMapping, On<SOOrder.orderType, Equal<vSOSiteMapping.orderType>,And<SOOrder.orderNbr,Equal<vSOSiteMapping.orderNbr>>>>>();
                    cmd.Join<InnerJoin<INSite, On<vSOSiteMapping.siteid, Equal<INSite.siteID>>>>();
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<INSite.siteCD.IsEqual<YusenAttr>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrSendToMiddleware, Equal<False>,
                        Or<SOOrderExt.usrSendToMiddleware, IsNull>>>();
                    break;
                case "SO301000$lumGererateYUSENNLFile":
                    cmd.Join<InnerJoin<vSOSiteMapping, On<SOOrder.orderType, Equal<vSOSiteMapping.orderType>, And<SOOrder.orderNbr, Equal<vSOSiteMapping.orderNbr>>>>>();
                    cmd.Join<InnerJoin<INSite,On<vSOSiteMapping.siteid, Equal<INSite.siteID>>>>();
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<INSite.siteCD.IsEqual<YusenAttr>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrSendToMiddleware, Equal<False>, 
                        Or<SOOrderExt.usrSendToMiddleware, IsNull>>>();
                    break;
            }
        }

        [PXOverride]
        public virtual IEnumerable orders(OrdersDeletage baseMethod)
        {
            var filterResult = baseMethod.Invoke();
            switch (Base.Filter.Current.Action)
            {
                // Filter Data By DCL order_number
                case "SO301000$lumCallDCLShipemnt":
                    List<string> lstOrderNumbers = new List<string>();
                    // Combine Query String
                    foreach (SOOrder item in filterResult)
                    {
                        if (!string.IsNullOrEmpty(item.CustomerRefNbr))
                            lstOrderNumbers.Add(item.CustomerRefNbr);
                    }

                    // Get DCL SO. Data
                    var dclOrders = JsonConvert.DeserializeObject<OrderResponse>(
                        DCLHelper.CallDCLToGetSOByOrderNumbers(
                            this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(),
                            string.Join(",", lstOrderNumbers.ToArray())).ContentResult);

                    if (dclOrders.orders != null)
                    {
                        foreach (SOOrder item in filterResult)
                        {
                            /*
                             *
                             *
                             *Debug mode  DCL.order_number = SO.CustomerRefNbr
                             *Release mode DCL.order_nu,ber = SO.orderNumber
                             *
                             *
                             *
                             */
                            if (dclOrders.orders.Any(x =>
                                x.order_number == item.CustomerRefNbr &&
                                !string.IsNullOrEmpty(x.shipping_carrier) &&
                                x.shipments.Any(s =>
                                    s.packages.Any(p => !string.IsNullOrEmpty(p.tracking_number)))))
                                yield return item;
                        }
                    }
                    break;
                default:
                    foreach (var item in filterResult)
                        yield return item;
                    break;
            }
        }

        #endregion


        #region Method

        #endregion
    }
}
