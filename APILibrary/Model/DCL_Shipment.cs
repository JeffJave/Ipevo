using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class DCLShipment
    {
        public bool allow_partial { get; set; }
        public string location { get; set; }
        public List<Order> orders { get; set; }
    }

    public class ShippingAddress
    {
        public string company { get; set; }
        public string attention { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }

    public class Line
    {
        public int line_number { get; set; }
        public string item_number { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public double? price { get; set; }
        public string do_not_ship_before { get; set; }
        public string ship_by { get; set; }
        public string comments { get; set; }
        public string custom_field2 { get; set; }
    }

    public class BillingAddress
    {
        public string company { get; set; }
        public string attention { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public string email { get; set; }
    }

    public class Order
    {
        public string order_number { get; set; }
        public string account_number { get; set; }
        public string ordered_date { get; set; }
        public string freight_account { get; set; }
        public string shipping_carrier { get; set; }
        public string shipping_service { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public List<Line> lines { get; set; }
        public int? order_status { get; set; }
        public string po_number { get; set; }
        public string customer_number { get; set; }
        public string acknowledgement_email { get; set; }
        public string system_id { get; set; }
        public BillingAddress billing_address { get; set; }
        public int? international_code { get; set; }
        public double? order_subtotal { get; set; }
        public double? shipping_handling { get; set; }
        public double? sales_tax { get; set; }
        public double? international_handling { get; set; }
        public double? total_due { get; set; }
        public double? amount_paid { get; set; }
        public double? net_due_currency { get; set; }
        public double? balance_due_us { get; set; }
        public double? international_declared_value { get; set; }
        public double? insurance { get; set; }
        public string payment_type { get; set; }
        public string terms { get; set; }
        public string fob { get; set; }
        public int? packing_list_type { get; set; }
        public string packing_list_comments { get; set; }
        public string shipping_instructions { get; set; }
        public string custom_field1 { get; set; }
    }

}
