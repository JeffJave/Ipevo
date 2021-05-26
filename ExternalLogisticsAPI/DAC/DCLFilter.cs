﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    public class DCLFilter : IBqlTable
    {
        [PXDBDate]
        [PXDefault("2021/5/12")]
        [PXUIField(DisplayName = "Received from")]
        public virtual DateTime? Received_from { get; set; }
        public abstract class received_from : PX.Data.BQL.BqlString.Field<received_from> { }

        [PXDBDate]
        [PXDefault("2021/5/12")]
        [PXUIField(DisplayName = "Received to")]
        public virtual DateTime? Received_to { get; set; }
        public abstract class received_to : PX.Data.BQL.BqlString.Field<received_to> { }

        [PXDBString]
        [PXDefault("AMZ")]
        [PXStringList(new string[] { "AMZ" }, new string[] { "AMZ" })]
        [PXUIField(DisplayName = "Customer number")]
        public virtual String Customer_number { get; set; }
        public abstract class customer_number : PX.Data.BQL.BqlString.Field<customer_number> { }
    }
}
