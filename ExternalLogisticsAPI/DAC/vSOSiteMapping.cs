using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI
{
    [Serializable]
    [PXCacheName("vSOSiteMapping")]
    public class vSOSiteMapping : IBqlTable
    {
        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Order Nbr")]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region Siteid
        [PXDBInt()]
        [PXUIField(DisplayName = "Siteid")]
        public virtual int? Siteid { get; set; }
        public abstract class siteid : PX.Data.BQL.BqlInt.Field<siteid> { }
        #endregion
    }
}