using IpevoCustomizations.DAC_Extensions;
using PX.Data;
using PX.SM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.GL
{
    public class JournalEntryExt : PXGraphExtension<JournalEntry>
    {
        public virtual void _(Events.RowSelected<Batch> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            if(e.Row != null)
            {
                if(e.Row.Status == BatchStatus.Unposted)
                { 
                    e.Row.GetExtension<BatchExtension>().UsrDisplayReviewer = Users.PK.Find(Base, e.Row.LastModifiedByID)?.FullName;
                }
                else if(e.Row.Status == BatchStatus.Posted)
                {
                    e.Row.GetExtension<BatchExtension>().UsrDisplayReviewer = Users.PK.Find(Base, e.Row.GetExtension<BatchExtension>().UsrReviewer)?.FullName;
                    e.Row.GetExtension<BatchExtension>().UsrDisplayPostedBy = Users.PK.Find(Base,e.Row.LastModifiedByID)?.FullName;
                }
                else
                    e.Row.GetExtension<BatchExtension>().UsrDisplayPostedBy = string.Empty;
            }
        }
    }
}
