using System;
using PX.Data;

namespace ExternalLogisticsAPI
{
    public class LUM3DCartImportProc : PXGraph<LUM3DCartImportProc>
    {

        public PXSave<MasterTable> Save;
        public PXCancel<MasterTable> Cancel;


        public PXFilter<MasterTable> MasterView;
        public PXFilter<DetailsTable> DetailsView;

        [Serializable]
        public class MasterTable : IBqlTable
        {

        }

        [Serializable]
        public class DetailsTable : IBqlTable
        {

        }
    }
}