using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Update;
using PX.Objects.CR;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LUMInterTenantTrans.Graph
{
    public class LMICCustomerMaint : PXGraph<LMICCustomerMaint>
    {
        public PXSave<LMICCustomer> Save;
        public PXCancel<LMICCustomer> Cancel;

        public SelectFrom<LMICCustomer>.View DetailsView;

        #region Event Handlers

        #region Auto-fill LoginName
        protected void LMICCustomer_TenantID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var row = (LMICCustomer)e.Row;
            if (row == null || row.TenantID == null) return;

            foreach (UPCompany info in PXCompanyHelper.SelectCompanies())
            {
                if (info.CompanyID == row.TenantID)
                {
                    row.LoginName = info.LoginName;
                    break;
                }
            }
        }
        #endregion

        #region Auto-fill CustomerName
        protected void LMICCustomer_CustomerID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var row = (LMICCustomer)e.Row;
            if (row == null || row.CustomerID == null) return;

            var currCustomer = PXSelect<BAccountR, Where<BAccountR.bAccountID, Equal<Required<BAccountR.bAccountID>>>>.Select(this, row.CustomerID);
            row.CustomerName = currCustomer.TopFirst.AcctName;
        }
        #endregion

        #endregion
    }
}
