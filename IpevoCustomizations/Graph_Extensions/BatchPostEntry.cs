using IpevoCustomizations.DAC_Extensions;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.GL
{
    public class BatchPostEntry : PXGraphExtension<BatchPost>
    {
        public override void Initialize()
        {
            base.Initialize();
            Base.BatchList.SetProcessDelegate<PostGraph>(
                delegate (PostGraph pg, Batch batch)
                {
                    var oldLastModifier = batch.LastModifiedByID;
                    var oldLastModifiedTime = batch.LastModifiedDateTime;
                    batch.GetExtension<BatchExtension>().UsrReviewer = oldLastModifier;
                    batch.GetExtension<BatchExtension>().UsrReviewOn = oldLastModifiedTime;
                    pg.Clear();
                    pg.PostBatchProc(batch);
                    //PXDatabase.Update<Batch>(new PXDataFieldAssign<BatchExtension.usrReviewer>(oldLastModifier),
                    //                         new PXDataFieldRestrict<Batch.batchNbr>(batch.BatchNbr));
                    //PXDatabase.Update<Batch>(new PXDataFieldAssign<BatchExtension.usrReviewOn>(oldLastModifiedTime),
                    //                         new PXDataFieldRestrict<Batch.batchNbr>(batch.BatchNbr));
                });
        }
    }
}
