using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI.DAC
{
    public class DCLFilter : IBqlTable
    {
        [PXDate]
        [PXUIField(DisplayName = "Received from")]
        public virtual DateTime? Received_from { get; set; }

        [PXDate]
        [PXUIField(DisplayName = "Received to")]
        public virtual DateTime? Received_to { get; set; }

        [PXString]
        [PXDefault("AMZ")]
        [PXStringList(new string[] { "AMZ" }, new string[] { "AMZ" })]
        [PXUIField(DisplayName = "Customer number")]
        public virtual String Customer_number { get; set; }
    }
}
