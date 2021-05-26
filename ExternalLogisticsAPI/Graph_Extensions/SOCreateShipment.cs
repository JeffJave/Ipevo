using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.SO
{
    public class SOCreateShipmentExt : PXGraphExtension<SOCreateShipment>
    {
        public delegate void AddCommonFiltersDelegate(SOOrderFilter filter, PXSelectBase<SOOrder> cmd);

        [PXOverride]
        public void AddCommonFilters(SOOrderFilter filter, PXSelectBase<SOOrder> cmd, AddCommonFiltersDelegate baseMethod)
        {
            baseMethod.Invoke(filter, cmd);
            if (filter.Action.Contains("DCL"))
            {
                cmd.WhereAnd<Where<SOOrder.status, Equal<SOOrderStatus.open>>>();
                cmd.WhereAnd<Where<SOOrderExt.usrDCLShipmentCreated, Equal<False>,
                    Or<SOOrderExt.usrDCLShipmentCreated, IsNull>>>();
            }
        }

        protected virtual PXSelectBase<SOOrder> MYCreateShipment(SOOrderFilter filter)
        {
            return new PXSelectJoin<SOOrder, InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>,
                    LeftJoin<Carrier, On<SOOrder.shipVia, Equal<Carrier.carrierID>>,
                        LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>>,
                Where<SOOrderExt.usrDCLShipmentCreated, Equal<False>,
                    And<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>(Base);
        }
    }
}
