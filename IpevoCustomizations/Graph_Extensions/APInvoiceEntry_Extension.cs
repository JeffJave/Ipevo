using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AP
{
    public class APInvoiceEntryExt : PXGraphExtension<APInvoiceEntry>
    {
        [InjectDependency]
        private ICurrentUserInformationProvider _currentUserInformationProvider { get; set; }

        public void _(Events.RowSelected<APInvoice> e, PXRowSelected baseMethod)
        {
            if (!CheckAcessRoleByWP(e.Row.EmployeeID) && e.Row != null && e.Row.EmployeeID.HasValue)
                throw new PXException("You don't have right to read this data.");
            baseMethod?.Invoke(e.Cache, e.Args);
        }

        /// <summary> Check AccessRole </summary>
        public virtual bool CheckAcessRoleByWP(int? _employeeid)
        {
            var IsAdmin = SelectFrom<UsersInRoles>
                .Where<UsersInRoles.username.IsEqual<P.AsString>>
                .View.Select(new PXGraph(),_currentUserInformationProvider.GetUserName()).RowCast<UsersInRoles>()
                .Where(x => x.Rolename.Contains("Administrator")).Any();
            var userEmployeeInfo = SelectFrom<PX.Objects.CR.CREmployee>
                             .Where<PX.Objects.CR.CREmployee.userID.IsEqual<AccessInfo.userID.FromCurrent>>
                             .View.Select(Base).RowCast<PX.Objects.CR.CREmployee>().FirstOrDefault();
            var isOwner = _employeeid == userEmployeeInfo.DefContactID || _employeeid == userEmployeeInfo.OwnerID;
            return IsAdmin || isOwner;
        }

    }
}
