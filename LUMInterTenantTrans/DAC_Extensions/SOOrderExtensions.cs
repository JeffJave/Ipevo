using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR.CCPaymentProcessing.Common;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO.Attributes;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.SO
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
        #region UsrICPOCreated
        [PXDBBool]
        [PXUIField(DisplayName = "IC PO Created", Enabled = false)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrICPOCreated { get; set; }
        public abstract class usrICPOCreated : PX.Data.BQL.BqlBool.Field<usrICPOCreated> { }
        #endregion
    }
}