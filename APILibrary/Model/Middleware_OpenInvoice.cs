using System.Collections.Generic;

namespace APILibrary.Model
{
    public class Item
    {
        public string sku { get; set; }
        public int qty { get; set; }
        public double whTax { get; set; }
    }

    public class Root
    {
        public string description { get; set; }
        public List<Item> item { get; set; }
        public double paymentAmount { get; set; }
        public double taxAmount { get; set; }
    }
}
