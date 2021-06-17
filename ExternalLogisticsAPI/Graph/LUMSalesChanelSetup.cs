using System;
using System.Text;
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
        public SelectFrom<LUMYusenNLSetup>.View NLYusen;
        public SelectFrom<LUMYusenCASetup>.View CAYusen;
        public SelectFrom<LUM3PLUKSetup>.View UKP3PL;

        /// <summary> Events.RowPersisting LUMVendCntrlSetup (Generate Access Token) </summary>
        public virtual void _(Events.RowPersisting<LUMVendCntrlSetup> e)
        {
            var row = e.Row;
            row.AuthToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{row.ClientID}:{row.ClientSecret}"));
        }

    }
}