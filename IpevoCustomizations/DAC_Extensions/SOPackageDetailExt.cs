using PX.Common;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;

namespace PX.Objects.SO
{
    public class SOPackageDetailExt : PXCacheExtension<SOPackageDetail>
    {
        public class DenominatorForMeasurement : PX.Data.BQL.BqlInt.Constant<DenominatorForMeasurement>
        {
            public DenominatorForMeasurement() : base(1000) { }
        }

        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Pallet Nbr")]
        public virtual string CustomRefNbr2 { get; set; }
        public abstract class customRefNbr2 : BqlString.Field<customRefNbr2> { }

        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Qty", Enabled = true)]
        public virtual decimal? Qty { get; set; }
        public abstract class qty : BqlDecimal.Field<qty> { }
        
        [PXDBString(15, IsUnicode = true)]
        [PXDefault]
        [PXSelector(typeof(Search<CSBox.boxID>))]
        [PXUIField(DisplayName = "Box ID")]
        public virtual string BoxID { get; set; }
        public abstract class boxID : BqlString.Field<boxID> { }


        #region UsrCartonQty
        [PXDBInt]
        [PXUIField(DisplayName = "Carton Qty")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrCartonQty { get; set; }
        public abstract class usrCartonQty : BqlInt.Field<usrCartonQty> { }
        #endregion

        #region UsrLength
        [PXDBDecimal]
        [PXUIField(DisplayName = "Length(CM)")]
        [PXDefault(TypeCode.Decimal, "100.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrLength { get; set; }
        public abstract class usrLength : BqlDecimal.Field<usrLength> { }
        #endregion

        #region UsrWidth
        [PXDBDecimal]
        [PXUIField(DisplayName = "Width(CM)")]
        [PXDefault(TypeCode.Decimal, "120.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrWidth { get; set; }
        public abstract class usrWidth : BqlDecimal.Field<usrWidth> { }
        #endregion

        #region UsrHeight
        [PXDBDecimal]
        [PXUIField(DisplayName = "Height(CM)")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrHeight { get; set; }
        public abstract class usrHeight : BqlDecimal.Field<usrHeight> { }
        #endregion

        #region UsrMeasurement
        [PXDBDecimal]
        [PXUIField(DisplayName = "Measurement(CBM)")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Div<Mult<Mult<usrLength, usrWidth>, usrHeight>, DenominatorForMeasurement>))]
        public virtual decimal? UsrMeasurement { get; set; }
        public abstract class usrMeasurement : BqlDecimal.Field<usrMeasurement> { }
        #endregion

        #region UsrShipmentSplitLineNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Shipment Split Line Nbr")]
        [PXSelector(typeof(Search<SOShipLine.lineNbr,
                           Where<SOShipLine.shipmentNbr, Equal<Current<SOPackageDetail.shipmentNbr>>, 
                           And<SOShipLine.packedQty, NotEqual<SOShipLine.shippedQty>>>>),
                    typeof(SOShipLine.origOrderType),
                    typeof(SOShipLine.origOrderNbr),
                    typeof(SOShipLine.inventoryID),
                    typeof(SOShipLine.shippedQty),
                    typeof(SOShipLine.packedQty),
                    typeof(SOShipLine.uOM))]
        public virtual int? UsrShipmentSplitLineNbr { get; set; }
        public abstract class usrShipmentSplitLineNbr : BqlInt.Field<usrShipmentSplitLineNbr> { }
        #endregion
    }
}