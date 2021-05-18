using System;
using ExternalLogisticsAPI.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;

namespace ExternalLogisticsAPI
{
    public class LUMSalesChanelSetup : PXGraph<LUMSalesChanelSetup>
    {
        public PXSave<LUM3DCartSetup> Save;
        public PXCancel<LUM3DCartSetup> Cancel;

        public SelectFrom<LUM3DCartSetup>.View ThreeDCart;
        public SelectFrom<LUMVendCntrlSetup>.View VendorCentral;
    }
}