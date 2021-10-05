using PX.Data;

namespace ExternalLogisticsAPI.Descripter
{
    public class AmazonFeeType
    {
        public const int Amz_WithholdingTax = 1;
        public const int Amz_WarehouseFee = 2;
        public const int Amz_Commission = 3;

        public static readonly int[] Values = new int[]
        {
            Amz_WithholdingTax, Amz_WarehouseFee, Amz_Commission
        };

        public static readonly string[] Labels = new string[]
        {
            "Amazon代徵代繳US州稅", "Amazon/Warehouse Fee", "Aamzon Commission"
        };

        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute() : base(AmazonOrderType.Values, AmazonOrderType.Labels) { }
        }
    }
}
