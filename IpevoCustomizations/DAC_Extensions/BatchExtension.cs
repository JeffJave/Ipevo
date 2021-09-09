using PX.Data;
using PX.Objects.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpevoCustomizations.DAC_Extensions
{
    public class BatchExtension : PXCacheExtension<Batch>
    {
        #region UsrReviewer
        [PXDBGuid]
        public virtual Guid? UsrReviewer { get; set; }
        public abstract class usrReviewer : PX.Data.BQL.BqlGuid.Field<usrReviewer> { }
        #endregion

        #region UsrReviewOn
        [PXDBDate]
        public virtual DateTime? UsrReviewOn { get; set; }
        public abstract class usrReviewOn : PX.Data.BQL.BqlDateTime.Field<usrReviewOn> { }
        #endregion

        #region UsrDisplayReviewer
        [PXUser]
        //[PXFormula(typeof(switch<Case<Where<Batch.status.FromCurrent.IsEqual<BatchStatus.unposted>>,BatchExtension.us>>))]
        [PXUIField(DisplayName = "Last Modifiy by", Enabled = false)]
        public virtual string UsrDisplayReviewer { get; set; }
        public abstract class usrDisplayReviewer : PX.Data.BQL.BqlString.Field<usrDisplayReviewer> { }
        #endregion

        #region UsrDisplayReviewer
        [PXString]
        [PXUIField(DisplayName = "Posted By", Enabled = false)]
        public virtual string UsrDisplayPostedBy { get; set; }
        public abstract class ssrDisplayPostedBy : PX.Data.BQL.BqlString.Field<ssrDisplayPostedBy> { }
        #endregion

    }
}
