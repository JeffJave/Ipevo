﻿using System.Collections.Generic;

namespace APILibrary.Model
{
    public class Amazon_Middleware
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Charge
        {
            public int type { get; set; }
            public string currency { get; set; }
            public double amount { get; set; }
        }

        public class Cop
        {
            public string currency { get; set; }
            public double amount { get; set; }
        }

        public class Fee
        {
            public int type { get; set; }
            public string name { get; set; }
            public string currency { get; set; }
            public double amount { get; set; }
        }

        public class Item
        {
            public int id { get; set; }
            public string shipment_date { get; set; }
            public string PostedDate { get; set; }
            public string kit_sku { get; set; }
            public string sku { get; set; }
            public int qty { get; set; }
            public string product { get; set; }
            public string partial_shipment_amazon_order_id { get; set; }
            public List<Charge> charge { get; set; }
            public double amount { get; set; }
            public double unit_price { get; set; }
            public List<Cop> cop { get; set; }
            public List<Fee> fee { get; set; }
            public string fulfillment_center_id { get; set; }
            public string carrier { get; set; }
            public string tracking_no { get; set; }
            public string currency { get; set; }
            public string country { get; set; }
            public string state { get; set; }
            public string order_id { get; set; }
            public string marketplace { get; set; }
            public int item_tax { get; set; }
        }

        public class Root
        {
            public int id { get; set; }
            public string merchant { get; set; }
            public string marketplace { get; set; }
            public string payments_date { get; set; }
            public string amazon_order_id { get; set; }
            public string buyer_tax_registration { get; set; }
            public string seller_tax_registration { get; set; }
            public bool export_outside_eu { get; set; }
            public bool overwritten { get; set; }
            public int status { get; set; }
            public object paymentReleaseDate { get; set; }
            public string buyer_name { get; set; }
            public string bill_address { get; set; }
            public string bill_city { get; set; }
            public string bill_state { get; set; }
            public string bill_postal_code { get; set; }
            public string bill_country { get; set; }
            public string buyer_email { get; set; }
            public string recipient_name { get; set; }
            public string ship_address { get; set; }
            public string ship_city { get; set; }
            public string ship_state { get; set; }
            public string ship_postal_code { get; set; }
            public string ship_country { get; set; }
            public List<Item> item { get; set; }
            public double itemSubtotal { get; set; }
            public double shipment { get; set; }
            public int giftWrap { get; set; }
            public double discount { get; set; }
            public int cashOnDelivery { get; set; }
            public double tax_rate { get; set; }
            public double gst_rate { get; set; }
            public double hst_rate { get; set; }
            public double qst_rate { get; set; }
            public double pst_rate { get; set; }
            public double tax { get; set; }
            public double gst { get; set; }
            public double hst { get; set; }
            public double qst { get; set; }
            public double pst { get; set; }
            public double salesProceeds { get; set; }
            public double marketplaceWithheldTax { get; set; }
            public int costOfPoints { get; set; }
            public double amazonFee { get; set; }
            public double accountBalance { get; set; }
        }

		public class Item2
		{
			public int id { get; set; }
			public int order_id { get; set; }
			public string partial_shipment_amazon_order_id { get; set; }
			public object shipment_date { get; set; }
			public string sku { get; set; }
			public string product { get; set; }
			public int qty { get; set; }
			public string carrier { get; set; }
			public string tracking_no { get; set; }
			public string currency { get; set; }
			public double unit_price { get; set; }
			public double amount { get; set; }
			public string fulfillment_center_id { get; set; }
			public object country { get; set; }
			public object state { get; set; }
			public string item_tax { get; set; }
			public List<Charge> charge { get; set; }
			public List<Cop> cop { get; set; }
			public List<Fee> fee { get; set; }
			public string marketplace { get; set; }
			public string PostedDate { get; set; }
			public string kit_sku { get; set; }
		}

		public class Root2
		{
			public int id { get; set; }
			public string merchant { get; set; }
			public string marketplace { get; set; }
			public string payments_date { get; set; }
			public string amazon_order_id { get; set; }
			public string buyer_tax_registration { get; set; }
			public string seller_tax_registration { get; set; }
			public bool export_outside_eu { get; set; }
			public bool overwritten { get; set; }
			public int status { get; set; }
			public string paymentReleaseDate { get; set; }
			public string buyer_name { get; set; }
			public string bill_address { get; set; }
			public string bill_city { get; set; }
			public string bill_state { get; set; }
			public string bill_postal_code { get; set; }
			public string bill_country { get; set; }
			public string buyer_email { get; set; }
			public string recipient_name { get; set; }
			public string ship_address { get; set; }
			public string ship_city { get; set; }
			public string ship_state { get; set; }
			public string ship_postal_code { get; set; }
			public string ship_country { get; set; }
			public List<Item2> item { get; set; }
			public double itemSubtotal { get; set; }
			public double shipment { get; set; }
			public double giftWrap { get; set; }
			public double discount { get; set; }
			public double cashOnDelivery { get; set; }
			public double tax_rate { get; set; }
			public double gst_rate { get; set; }
			public double hst_rate { get; set; }
			public double qst_rate { get; set; }
			public double pst_rate { get; set; }
			public double tax { get; set; }
			public double gst { get; set; }
			public double hst { get; set; }
			public double qst { get; set; }
			public double pst { get; set; }
			public double salesProceeds { get; set; }
			public double marketplaceWithheldTax { get; set; }
			public double costOfPoints { get; set; }
			public double amazonFee { get; set; }
			public double accountBalance { get; set; }
		}
	}
}
