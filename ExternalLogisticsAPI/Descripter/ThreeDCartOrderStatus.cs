using System;
using PX.Data;

namespace ExternalLogisticsAPI.Descripter
{
    /* 3D Cart order status
     * 1:New 2:Processing 3:Partial 4:Shipped 5:Cancel 6:Hold 7: Not Completedv11:Unpaid 
    */
    public class ThreeDCartOrderStatus
    {
        public const int New = 1;
        public const int Processing = 2;
        public const int Partial = 3;
        public const int Shipped = 4;
        public const int Cancel = 5;
        public const int Hold = 6;
        public const int NotCompleted = 7;
        public const int Updated = 11;

        public static readonly string[] Values = new string[]
        {
            "1", "2", "3", "4", "5", "6", "7", "11"
        };
        public static readonly string[] Labels = new string[]
        {
            "New", "Processing", "Partial", "Shipped", "Cancel", "Hold", "Not Completed", "Updated"
        };

        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(ThreeDCartOrderStatus.Values, ThreeDCartOrderStatus.Labels) { }
        }
    }
}
