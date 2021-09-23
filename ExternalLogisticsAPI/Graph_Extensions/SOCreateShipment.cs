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

        public delegate PXSelectBase<SOOrder> GetSelectCommandDelegate(SOOrderFilter filter);
        [PXOverride]
        public virtual PXSelectBase<SOOrder> GetSelectCommand(SOOrderFilter filter, GetSelectCommandDelegate baseMethod)
        {
            PXSelectBase<SOOrder> cmd;
            switch (filter.Action)
            {
                case "SO301000$lumCreateShipmentforFBA":
                    cmd = BuildCommandCreateShipment(filter);
                    break;
                case "SO301000$lumPrepareInvoiceforAmazon":
                    cmd = BuildCommandPrepareInvoice();
                    break;
                default:
                    cmd = baseMethod(filter);
                    break;
            }
            return cmd;
        }

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
                    cmd.WhereAnd<Where<SOOrder.orderType, NotEqual<SotypeVCAttr>>>();
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
                    cmd.Join<InnerJoin<vSOSiteMapping, On<SOOrder.orderType, Equal<vSOSiteMapping.orderType>, And<SOOrder.orderNbr, Equal<vSOSiteMapping.orderNbr>>>>>();
                    cmd.Join<InnerJoin<INSite, On<vSOSiteMapping.siteid, Equal<INSite.siteID>>>>();
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<INSite.siteCD.IsEqual<YusenAttr>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrSendToMiddleware, Equal<False>,
                        Or<SOOrderExt.usrSendToMiddleware, IsNull>>>();
                    break;
                case "SO301000$lumGererateYUSENNLFile":
                    cmd.Join<InnerJoin<vSOSiteMapping, On<SOOrder.orderType, Equal<vSOSiteMapping.orderType>, And<SOOrder.orderNbr, Equal<vSOSiteMapping.orderNbr>>>>>();
                    cmd.Join<InnerJoin<INSite, On<vSOSiteMapping.siteid, Equal<INSite.siteID>>>>();
                    cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                    cmd.WhereAnd<Where<INSite.siteCD.IsEqual<YusenAttr>>>();
                    cmd.WhereAnd<Where<SOOrderExt.usrSendToMiddleware, Equal<False>,
                        Or<SOOrderExt.usrSendToMiddleware, IsNull>>>();
                    break;
                case "SO301000$lumPrepareInvoiceforAmazon":
                    //cmd.WhereAnd<Where<SOShipment.status.IsEqual<SOShipmentStatus.confirmed>>>();
                    break;
            }
        }

        #endregion

        #region Method

        protected virtual PXSelectBase<SOOrder> BuildCommandCreateShipment(SOOrderFilter filter)
        {
            PXSelectBase<SOOrder> cmd = new PXSelectJoinGroupBy<SOOrder,
                InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>,
                LeftJoin<Carrier, On<SOOrder.shipVia, Equal<Carrier.carrierID>>,
                InnerJoin<SOShipmentPlan,
                    On<SOOrder.orderType, Equal<SOShipmentPlan.orderType>,
                        And<SOOrder.orderNbr, Equal<SOShipmentPlan.orderNbr>>>,
                InnerJoin<INSite, On<INSite.siteID, Equal<SOShipmentPlan.siteID>>,
                LeftJoin<SOOrderShipment,
                    On<SOOrderShipment.orderType, Equal<SOShipmentPlan.orderType>,
                        And<SOOrderShipment.orderNbr, Equal<SOShipmentPlan.orderNbr>,
                        And<SOOrderShipment.siteID, Equal<SOShipmentPlan.siteID>,
                        And<SOOrderShipment.confirmed, Equal<boolFalse>>>>>,
                LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>>>>>,
                Where<SOShipmentPlan.inclQtySOBackOrdered, Equal<short0>, And<SOOrderShipment.shipmentNbr, IsNull,
                    And2<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>,
                    And<Match<INSite, Current<AccessInfo.userName>>>>>>,
                Aggregate<
                    GroupBy<SOOrder.orderType,
                    GroupBy<SOOrder.orderNbr,
                    GroupBy<SOOrder.approved>>>>>(Base);

            if (filter.SiteID != null)
                cmd.WhereAnd<Where<SOShipmentPlan.siteID, Equal<Current<SOOrderFilter.siteID>>>>();

            if (filter.DateSel == "S")
            {
                if (filter.EndDate != null)
                    cmd.WhereAnd<Where<SOShipmentPlan.planDate, LessEqual<Current<SOOrderFilter.endDate>>>>();

                if (filter.StartDate != null)
                {
                    cmd.WhereAnd<Where<SOShipmentPlan.planDate, GreaterEqual<Current<SOOrderFilter.startDate>>>>();
                }

                filter.DateSel = string.Empty;
            }

            return cmd;
        }

        protected virtual PXSelectBase<SOOrder> BuildCommandPrepareInvoice()
        {
            var cmd =
                new PXSelectJoinGroupBy<SOOrder,
                        InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>, And<SOOrderType.aRDocType, NotEqual<ARDocType.noUpdate>>>,
                        LeftJoin<Carrier, On<SOOrder.shipVia, Equal<Carrier.carrierID>>,
                        LeftJoin<SOOrderShipment, On<SOOrderShipment.orderType, Equal<SOOrder.orderType>, And<SOOrderShipment.orderNbr, Equal<SOOrder.orderNbr>>>,
                        LeftJoinSingleTable<ARInvoice, On<ARInvoice.docType, Equal<SOOrderShipment.invoiceType>, And<ARInvoice.refNbr, Equal<SOOrderShipment.invoiceNbr>>>,
                        LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>>>>,
                    Where<SOOrder.hold, Equal<boolFalse>, And<SOOrder.cancelled, Equal<boolFalse>,
                        And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>>,
                    Aggregate<
                        GroupBy<SOOrder.orderType,
                        GroupBy<SOOrder.orderNbr,
                        GroupBy<SOOrder.approved>>>>>(Base);

            if (PXAccess.FeatureInstalled<FeaturesSet.inventory>())
            {
                cmd.WhereAnd<
                    Where<Sub<Sub<Sub<SOOrder.shipmentCntr,
                                                                    SOOrder.openShipmentCntr>,
                                                                    SOOrder.billedCntr>,
                                                                    SOOrder.releasedCntr>, Greater<short0>,
                        Or2<Where<SOOrder.orderQty, Equal<decimal0>,
                                    And<SOOrder.curyUnbilledMiscTot, Greater<decimal0>>>,
                        Or<Where<SOOrderType.requireShipping, Equal<boolFalse>, And<ARInvoice.refNbr, IsNull>>>>>>();
            }
            else
            {
                cmd.WhereAnd<
                    Where<SOOrder.curyUnbilledMiscTot, Greater<decimal0>, And<SOOrderShipment.shipmentNbr, IsNull,
                        Or<Where<SOOrderType.requireShipping, Equal<boolFalse>, And<ARInvoice.refNbr, IsNull>>>>>>();
            }

            return cmd;
        }

        #endregion

        public class SotypeVCAttr : PX.Data.BQL.BqlString.Constant<SotypeVCAttr>
        {
            public SotypeVCAttr() : base("VC") { }
        }
    }
}
