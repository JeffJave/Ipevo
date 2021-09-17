using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.SO
{
    public class SOInvoiceShipmentExt : PXGraphExtension<SOInvoiceShipment>
    {
        public delegate void ApplyShipmentFiltersDelegate(PXSelectBase<SOShipment> shCmd, SOShipmentFilter filter);

        [PXOverride]
        public virtual void ApplyShipmentFilters(PXSelectBase<SOShipment> shCmd, SOShipmentFilter filter, ApplyShipmentFiltersDelegate baseMthod)
        {
            baseMthod?.Invoke(shCmd, filter);
            switch (filter.Action)
            {
                // Yusen NL
                case "SO302000$lumGererateYUSENNLFile":
                    shCmd.WhereAnd<Where<INSite.siteCD.IsEqual<YusenAttr>>>();
                    shCmd.WhereAnd<Where<SOShipment.status.IsEqual<SOShipmentStatus.open>>>();
                    break;
                // Yusen CA
                case "SO302000$lumGenerateYUSENCAFile":
                    shCmd.WhereAnd<Where<INSite.siteCD.IsEqual<YusenAttr>>>();
                    shCmd.WhereAnd<Where<SOShipment.status.IsEqual<SOShipmentStatus.open>>>();
                    break;
                // P3PL UK
                case "SO302000$lumGenerate3PLUKFile":
                    shCmd.WhereAnd<Where<INSite.siteCD.IsEqual<P3PLAttr>>>();
                    shCmd.WhereAnd<Where<SOShipment.status.IsEqual<SOShipmentStatus.open>>>();
                    break;
            }
        }
    }

    public class YusenAttr : PX.Data.BQL.BqlString.Constant<YusenAttr>
    {
        public YusenAttr() : base("YUSEN") { }
    }

    public class P3PLAttr : PX.Data.BQL.BqlString.Constant<P3PLAttr>
    {
        public P3PLAttr() : base("P3PL") { }
    }
}
