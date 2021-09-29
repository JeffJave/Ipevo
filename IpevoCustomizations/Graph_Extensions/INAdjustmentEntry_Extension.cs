using PX.Data;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpevoCustomizations.Graph_Extensions
{
    public class INAdjustmentEntry_Extension : PXGraphExtension<INAdjustmentEntry>
    {
        #region Override
        public override void Initialize()
        {
            base.Initialize();
            this.action.AddMenuAction(lumReverseAdjustment);
            this.action.SetEnabled("LumReverseAdjustment", false);
            this.action.MenuAutoOpen = true;
        }
        #endregion

        #region Action
        public PXAction<INRegister> action;
        public PXAction<INRegister> lumReverseAdjustment;

        [PXButton(SpecialType = PXSpecialButtonType.ActionsFolder)]
        [PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        protected void Action() { }

        [PXButton]
        [PXUIField(DisplayName = "Reverse Adjustment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Enabled = false)]
        public virtual IEnumerable LumReverseAdjustment(PXAdapter adapter)
        {
            var graph = PXGraph.CreateInstance<INAdjustmentEntry>();
            // Setting Document
            var newDoc = graph.adjustment.Insert((INRegister)graph.adjustment.Cache.CreateInstance());
            newDoc.Hold = true;
            newDoc.ExtRefNbr = Base.adjustment.Current.RefNbr;
            newDoc.TranDesc = Base.adjustment.Current.TranDesc + "(REVERSED)";
            newDoc.TranDate = Base.adjustment.Current.TranDate;
            newDoc.FinPeriodID = Base.adjustment.Current.FinPeriodID;
            newDoc.TranPeriodID = Base.adjustment.Current.TranPeriodID;
            newDoc.DocType = Base.adjustment.Current.DocType;
            newDoc.BranchID = Base.adjustment.Current.BranchID;
            newDoc.TotalQty = Base.adjustment.Current.TotalQty;
            newDoc.TotalCost = Base.adjustment.Current.TotalCost * -1;
            graph.adjustment.Insert(newDoc);
            // Setting Line
            foreach (var item in Base.transactions.Select().RowCast<INTran>())
            {
                var newTrans = graph.transactions.Insert((INTran)graph.transactions.Cache.CreateInstance());
                var oldLineNbr = newTrans.LineNbr;
                PXCache<INTran>.RestoreCopy(newTrans, item);
                newTrans.RefNbr = graph.adjustment.Current.RefNbr;
                newTrans.OrigTranCost *= -1;
                newTrans.TranCost *= -1; ;
                newTrans.NoteID = PXNoteAttribute.GetNoteID<INTran.noteID>(graph.transactions.Cache, graph.transactions.Cache.CreateInstance());
                newTrans.LineNbr = oldLineNbr;
                graph.transactions.Cache.Insert(newTrans);
            }
            graph.Actions.PressSave();
            throw new PXRedirectRequiredException(graph, true, "Reverse Adjustment");
        }
        #endregion

        #region Events

        public virtual void _(Events.RowSelected<INRegister> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            if (e.Row.Released ?? false)
                this.action.SetEnabled("LumReverseAdjustment", true);
        }

        #endregion
    }
}
