using PX.Data;
using PX.Objects.CA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLogisticsAPI.Graph_Extensions
{
    public  class CADepositEntryExt :  PXGraphExtension<CADepositEntry>
    {
        public virtual void _(Events.RowSelected<CADeposit> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache,e.Args);
            Base.DepositPayments.Cache.AllowInsert = true;
        }
    }
}
