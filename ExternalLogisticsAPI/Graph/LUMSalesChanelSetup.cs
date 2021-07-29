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
        public SelectFrom<LUMYusenCASetup>.View CAYusen;
        public SelectFrom<LUM3PLUKSetup>.View UKP3PL;
        public SelectFrom<LUMMiddleWareSetup>.View Middleware;
        public SelectFrom<LUMLobAPISetup>.View LobAPI;

        /// <summary> Events.RowPersisting LUMVendCntrlSetup (Generate Access Token) </summary>
        public virtual void _(Events.RowPersisting<LUMVendCntrlSetup> e)
        {
            var row = e.Row;
            row.AuthToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{row.ClientID}:{row.ClientSecret}"));
        }

    }
}