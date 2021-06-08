using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.SO
{
    public class SOShipLineExt : PXCacheExtension<SOShipLine>
    {
        #region Constant parameters - AIR and SEA
        public class ShipViaByAir : PX.Data.BQL.BqlString.Constant<ShipViaByAir>
        {
            public ShipViaByAir() : base("AIR") { }
        }

        public class ShipViaBySea : PX.Data.BQL.BqlString.Constant<ShipViaBySea>
        {
            public ShipViaBySea() : base("SEA") { }
        }
        #endregion

        #region Constant parameters - CTNPPLTAIR and CTNPPLTSEA
        public class ShipViaByAirAttr : PX.Data.BQL.BqlString.Constant<ShipViaByAirAttr>
        {
            public ShipViaByAirAttr() : base("CTNPPLTAIR") { }
        }

        public class ShipViaBySeaAttr : PX.Data.BQL.BqlString.Constant<ShipViaBySeaAttr>
        {
            public ShipViaBySeaAttr() : base("CTNPPLTSEA") { }
        }
        #endregion

        [PXString]
        [PXUIField(DisplayName = "Sea Pallet", Visible = false)]
        [PXDBScalar(typeof(SelectFrom<CSAnswers>.
                          LeftJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<ShipViaBySeaAttr>>>.
                          Where<InventoryItem.inventoryID.IsEqual<SOShipLine.inventoryID>>.
                          SearchFor<CSAnswers.value>))]
        public virtual string UsrPalletQtyBySea { get; set; }
        public abstract class usrPalletQtyBySea : BqlString.Field<usrPalletQtyBySea> { }

        [PXString]
        [PXUIField(DisplayName = "Air Pallet", Visible = false)]
        [PXDBScalar(typeof(SelectFrom<CSAnswers>.
                          LeftJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<ShipViaByAirAttr>>>.
                          Where<InventoryItem.inventoryID.IsEqual<SOShipLine.inventoryID>>.
                          SearchFor<CSAnswers.value>))]
        public virtual string UsrPalletQtyByAir { get; set; }
        public abstract class usrPalletQtyByAir : BqlString.Field<usrPalletQtyByAir> { }

        [PXDecimal]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Packing Qty")]
        public virtual decimal? UsrPackingQty { get; set; }
        public abstract class usrPackingQty : BqlDecimal.Field<usrPackingQty> { }

        [PXDecimal]
        [PXUIField(DisplayName = "Remaining Qty", Enabled = false)]
        [PXFormula(typeof(Sub<SOShipLine.shippedQty, SOShipLine.packedQty>))]
        public virtual decimal? UsrRemainingQty { get; set; }
        public abstract class usrRemainingQty : BqlDecimal.Field<usrRemainingQty> { }

        [PXString]
        [PXUIField(DisplayName = "Cartons Per Pallet", Enabled = false)]
        [PXFormula(typeof(Switch<
                          Case<Where<SOShipment.shipVia.FromCurrent.IsEqual<ShipViaByAir>>, usrPalletQtyByAir,
                          Case<Where<SOShipment.shipVia.FromCurrent.IsEqual<ShipViaBySea>>, usrPalletQtyBySea>>>))]
        public virtual string UsrCartonsPerPallet { get; set; }
        public abstract class usrCartonsPerPallet : BqlString.Field<usrCartonsPerPallet> { }
    }
}
