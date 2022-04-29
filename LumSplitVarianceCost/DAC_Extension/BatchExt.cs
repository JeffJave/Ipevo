using PX.Common;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL.Attributes;
using PX.Objects.GL.DAC;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.GL;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.GL
{
    public class BatchExt : PXCacheExtension<PX.Objects.GL.Batch>
    {
        #region UsrSplited
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Splited")]
        public virtual bool? UsrSplited { get; set; }
        public abstract class usrSplited : PX.Data.BQL.BqlBool.Field<usrSplited> { }
        #endregion
    }
}