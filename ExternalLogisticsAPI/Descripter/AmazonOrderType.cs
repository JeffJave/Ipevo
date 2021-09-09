using PX.Data;

namespace ExternalLogisticsAPI.Descripter
{
    public class AmazonOrderType
    {
        public const int FBA_SO = 1;
        public const int FBM_ShipInfo = 2;
        public const int FBM_AmzFee = 3;
        public const int FBA_OI_ShipInfo = 4;
        public const int FBA_OI_AmzFee = 5;
        public const int FBM_OI_ShipInfo = 6;
        public const int FBM_OI_AmzFee = 7;
        public const int FBA_RMA_CM = 8;
        public const int FBA_RMA_Exch = 9;
        public const int FBA_RMA_RA = 10;
        public const int RestockingFee = 11;
        public const int Reimbursement = 12;
        public const int Rev_Reimbursement = 13;

        public static readonly int[] Values = new int[]
        {
            FBA_SO, FBM_ShipInfo, FBM_AmzFee, FBA_OI_ShipInfo, FBA_OI_AmzFee, FBM_OI_ShipInfo, FBM_OI_AmzFee,
            FBA_RMA_CM, FBA_RMA_Exch, FBA_RMA_RA, RestockingFee, Reimbursement, Rev_Reimbursement
        };
        public static readonly string[] Labels = new string[]
        {
            "FBA 銷單", "FBM 銷單僅出貨資訊", "FBM 銷單含Amz Fee", "FBA Open Invoice 銷單僅出貨資訊", "FBA Open Invoice 銷單含Amz Fee",
            "FBM Open Invoice 銷單僅出貨資訊", "FBM Open Invoice 銷單含Amz Fee", "FBA RMA CM", "FBA RMA RAExchange",
            "FBA RMA RA", "Restocking Fee", "Reimbursement(全為正項)", "Reverse Reimbursement(全為負項)"
        };

        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute() : base(AmazonOrderType.Values, AmazonOrderType.Labels) { }
        }
    }
}
