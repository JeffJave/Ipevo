using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model
{
    [Serializable]
    public class OrderRequest
    {
        public string status { get; set; }
        public string order_numbers { get; set; }
        public string received_from { get; set; }
        public string received_to { get; set; }
        public string shipped_from { get; set; }
        public string shipped_to { get; set; }
        public string modified_from { get; set; }
        public string modified_to { get; set; }
        public string fields { get; set; }
        public int page { get; set; } = 1;
        public int page_size { get; set; } = 100;
        public string filter { get; set; }
        public bool extended_date { get; set; } = false;
    }

    public class OrderResponse
    {
        public int error_code { get; set; }
        public string error_message { get; set; }
        public ResOrders[] orders { get; set; }
    }

    public class ResOrders
    {
        public int order_status { get; set; }
        public string order_number { get; set; }
        public string location { get; set; }
        public int order_stage { get; set; }
        public string stage_description { get; set; }
        public bool is_cancelled { get; set; }
        public bool is_back_order { get; set; }
        public string account_number { get; set; }
        public string order_type { get; set; }
        public string system_id { get; set; }
        public string ordered_date { get; set; }
        public string received_date { get; set; }
        public string po_number { get; set; }
        public string customer_number { get; set; }
        public string freight_account { get; set; }
        public string consignee_number { get; set; }
        public string shipping_carrier { get; set; }
        public string shipping_service { get; set; }
        public ResOrdersShippingAddr shipping_address { get; set; }
        public ResOrdersBillingAddr billing_address { get; set; }
        public int international_code { get; set; }
        public decimal order_subtotal { get; set; }
        public decimal shipping_handling { get; set; }
        public decimal sales_tax { get; set; }
        public decimal international_handling { get; set; }
        public decimal total_due { get; set; }
        public decimal amount_paid { get; set; }
        public decimal net_due_currency { get; set; }
        public decimal balance_due_us { get; set; }
        public decimal international_declared_value { get; set; }
        public decimal insurance { get; set; }
        public string payment_type { get; set; }
        public string terms { get; set; }
        public string fob { get; set; }
        public int packing_list_type { get; set; }
        public string packing_list_comments { get; set; }
        public string shipping_instructions { get; set; }
        public string custom_field1 { get; set; }
        public string custom_field2 { get; set; }
        public string custom_field3 { get; set; }
        public string custom_field4 { get; set; }
        public string custom_field5 { get; set; }

        public ResOrderLine[] order_lines { get; set; }
        public ResShipments[] shipments { get; set; }
        public string modified_at { get; set; }
    }

    public class ResOrdersShippingAddr
    {
        public string company { get; set; }
        public string attention { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class ResOrdersBillingAddr
    {
        public string company { get; set; }
        public string attention { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class ResOrderLine
    {
        public int line_number { get; set; }
        public string item_number { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public bool is_back_order { get; set; }
        public decimal price { get; set; }
        public string do_not_ship_before { get; set; }
        public string ship_by { get; set; }
        public string comments { get; set; }
        public string custom_field1 { get; set; }
        public string custom_field2 { get; set; }
        public string custom_field3 { get; set; }
        public string custom_field4 { get; set; }
        public string custom_field5 { get; set; }
        public List<ResOrdersBundleLine> bundle_lines { get; set; }
    }

    public class ResOrdersBundleLine
    {
        public int line_number { get; set; }
        public string item_number { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public bool is_back_order { get; set; }
        public decimal price { get; set; }
        public string do_not_ship_before { get; set; }
        public string ship_by { get; set; }
        public string comments { get; set; }
        public string custom_field1 { get; set; }
        public string custom_field2 { get; set; }
        public string custom_field3 { get; set; }
        public string custom_field4 { get; set; }
        public string custom_field5 { get; set; }
        public List<ResOrdersBundleLine> bundle_lines { get; set; }
    }

    public class ResShipments
    {
        public int ship_id { get; set; }
        public string ship_date { get; set; }
        public string shipping_carrier { get; set; }
        public string shipping_service { get; set; }
        public string freight_account { get; set; }
        public decimal total_weight { get; set; }
        public decimal total_charge { get; set; }
        public string reference1 { get; set; }
        public string reference2 { get; set; }
        public string reference3 { get; set; }
        public string reference4 { get; set; }
        public string rs_tracking_number { get; set; }
        public ResOrdersPackages[] packages { get; set; }
        public ResOrdersShippedLine[] shipped_lines { get; set; }
        public ResOrdersBillingAddr shipping_address { get; set; }
    }

    public class ResOrdersPackages
    {
        public string carton_id { get; set; }
        public int carton_count_number { get; set; }
        public decimal weight { get; set; }
        public decimal freight { get; set; }
        public string dimension { get; set; }
        public string tracking_number { get; set; }
        public string delivery_date { get; set; }
        public string delivery_status { get; set; }
        public string asn { get; set; }
        public List<ResOrdersShippedItem> shipped_items { get; set; }
    }

    public class ResOrdersShippedItem
    {
        public string item_number { get; set; }
        public string description { get; set; }
        public int line_number { get; set; }
        public string custom_field3 { get; set; }
        public int quantity { get; set; }
        public string[] serial_numbers { get; set; }
        public string lot_numbers { get; set; }
    }

    public class ResOrdersShippedLine
    {
        public string item_number { get; set; }
        public string description { get; set; }
        public int line_number { get; set; }
        public string custom_field3 { get; set; }
        public int quantity { get; set; }
        public string[] serial_numbers { get; set; }
        public string lot_numbers { get; set; }
        public List<ResOrdersBundleLine> bundle_lines { get; set; }
    }
}
