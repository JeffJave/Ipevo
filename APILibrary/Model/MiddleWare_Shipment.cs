using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    public class MiddleWare_Shipment
    {
        public string merchant { get; set; }
        public string amazon_order_id { get; set; }
        public string shipment_date { get; set; }
        public string carrier { get; set; }
        public string tracking_number { get; set; }
        public string shipping_method { get; set; }
    }
}
