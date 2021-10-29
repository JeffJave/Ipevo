using PX.Data;
using PX.SM;
using System.Collections;
using IpevoCustomizations.DAC_Extensions;

namespace PX.Objects.GL
{
    public class JournalEntryExt : PXGraphExtension<JournalEntry>
    {
        #region Delegate Data View
        public IEnumerable gLTranModuleBatNbr()
        {
            PXView select = new PXView(Base, true, Base.GLTranModuleBatNbr.View.BqlSelect);

            int totalrow = 0;
            int startrow = PXView.StartRow;

            select.WhereAnd<Where<GLTran.curyDebitAmt, Greater<CS.decimal0>, Or<GLTran.curyCreditAmt, Greater<CS.decimal0>>>>();

            return select.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startrow, PXView.MaximumRows, ref totalrow);
        }
        #endregion

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
