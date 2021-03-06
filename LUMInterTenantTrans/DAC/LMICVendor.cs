using System;
using System.Collections;
using PX.Data;
using PX.Data.Update;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.SM;

namespace LUMInterTenantTrans
{
    [Serializable]
    [PXCacheName("LMICVendor")]
    public class LMICVendor : IBqlTable
    {
        #region BranchID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch ID")]
        [PXSelector(typeof(Search<Branch.branchID>), SubstituteKey = typeof(Branch.branchCD))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region VendorID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Vendor ID")]
        [PXDimensionSelectorAttribute("VENDOR", typeof(Search<VendorR.bAccountID,
                                                        Where<VendorR.type, Equal<BAccountType.vendorType>,
                                                        And<VendorR.vStatus, Equal<CustomerStatus.active>>>>),
                                                typeof(VendorR.acctCD),
                                                new Type[] { typeof(VendorR.bAccountID), typeof(VendorR.acctCD), typeof(VendorR.acctName) },
                                      IsDirty = true)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorid : PX.Data.BQL.BqlInt.Field<vendorid> { }
        #endregion

        #region VendorName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Name", Enabled = false)]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region TenantID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Tenant ID")]
        [CompanySelector()]
        public virtual int? TenantID { get; set; }
        public abstract class tenantID : PX.Data.BQL.BqlInt.Field<tenantID> { }
        #endregion

        #region LoginName
        [PXDBString(128, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Login Name", Enabled = false)]
        public virtual string LoginName { get; set; }
        public abstract class loginName : PX.Data.BQL.BqlString.Field<loginName> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NeuralCompanySelectorAttribute
        public class CompanySelectorAttribute : PXCustomSelectorAttribute
        {
            public CompanySelectorAttribute()
                : base(typeof(UPCompany.companyID))
            {
                DescriptionField = typeof(UPCompany.loginName);
            }
            protected virtual IEnumerable GetRecords()
            {
                PXCache cache = _Graph.Caches[typeof(UPCompany)];
                Int32 current = PX.Data.Update.PXInstanceHelper.CurrentCompany;
                foreach (UPCompany info in PXCompanyHelper.SelectCompanies(PXCompanySelectOptions.Visible))
                {
                    if (current != info.CompanyID) yield return info;
                }
            }
            public override void DescriptionFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, string alias)
            {
                if (e.Row == null || (sender.GetValue(e.Row, _FieldOrdinal) == null)) base.DescriptionFieldSelecting(sender, e, alias);
                else
                {
                    UPCompany item = null;
                    Object value = sender.GetValue(e.Row, _FieldOrdinal);
                    Int32 key = (Int32)value;
                    foreach (UPCompany info in PXCompanyHelper.SelectCompanies())
                    {
                        if (info.CompanyID == key)
                        {
                            item = info;
                            break;
                        }
                    }
                    if (item != null) e.ReturnValue = sender.Graph.Caches[_Type].GetValue(item, _DescriptionField.Name);
                }
            }
        }
        #endregion
    }
}