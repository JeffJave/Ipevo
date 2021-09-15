using System;
using PX.Data;
using PX.Objects.CR;

namespace PX.Objects.SO
{
    public class SOLineExt : PXCacheExtension<PX.Objects.SO.SOLine>
    {
        #region UsrFulfillmentCenter
        [PXDBString(6, IsUnicode = true)]
        [PXUIField(DisplayName = "Fulfillment Center")]
        public virtual string UsrFulfillmentCenter { get; set; }
        public abstract class usrFulfillmentCenter : PX.Data.BQL.BqlString.Field<usrFulfillmentCenter> { }
        #endregion

        #region UsrShipFromCountryID
        [PXDBString(2, IsUnicode = true)]
        [Country]
        [PXUIField(DisplayName = "Ship From Country")]
        public virtual string UsrShipFromCountryID { get; set; }
        public abstract class usrShipFromCountryID : PX.Data.BQL.BqlString.Field<usrShipFromCountryID> { }
        #endregion

        #region UsrShipFromState
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Ship From State")]
        public virtual string UsrShipFromState { get; set; }
        public abstract class usrShipFromState : PX.Data.BQL.BqlString.Field<usrShipFromState> { }
        #endregion

        #region UsrItemTaxAmt
        [PXDBDecimal(2)]
        //[PXDBCurrency(typeof(SOLine.curyInfoID), typeof(SOLine.openAmt), BaseCalc = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Item Tax")]
        public virtual decimal? UsrItemTaxAmt { get; set; }
        public abstract class usrItemTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrItemTaxAmt> { }
        #endregion

        #region UsrGiftwrapTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Giftwrap Tax")]
        public virtual decimal? UsrGiftwrapTaxAmt { get; set; }
        public abstract class usrGiftwrapTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrGiftwrapTaxAmt> { }
        #endregion

        #region UsrFreightTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight Tax")]
        public virtual decimal? UsrFreightTaxAmt { get; set; }
        public abstract class usrFreightTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrFreightTaxAmt> { }
        #endregion

        #region UsrAmazWHTaxAmt
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Amazon WH Tax")]
        public virtual decimal? UsrAmazWHTaxAmt { get; set; }
        public abstract class usrAmazWHTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrAmazWHTaxAmt> { }
        #endregion

        #region UsrItemGSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Item GST Tax")]
        public virtual decimal? UsrItemGSTTaxAmt { get; set; }
        public abstract class usrItemGSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrItemGSTTaxAmt> { }
        #endregion

        #region UsrItemPSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Item PST Tax")]
        public virtual decimal? UsrItemPSTTaxAmt { get; set; }
        public abstract class usrItemPSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrItemPSTTaxAmt> { }
        #endregion

        #region UsrItemQSTTaxAmt
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Item QST Tax")]
        public virtual decimal? UsrItemQSTTaxAmt { get; set; }
        public abstract class usrItemQSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrItemQSTTaxAmt> { }
        #endregion

        #region UsrItemHSTTaxAmt
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Item HST Tax")]
        public virtual decimal? UsrItemHSTTaxAmt { get; set; }
        public abstract class usrItemHSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrItemHSTTaxAmt> { }
        #endregion

        #region UsrGWGSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "GiftWrap GST Tax")]
        public virtual decimal? UsrGWGSTTaxAmt { get; set; }
        public abstract class usrGWGSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrGWGSTTaxAmt> { }
        #endregion

        #region UsrGWPSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "GiftWrap PST Tax")]
        public virtual decimal? UsrGWPSTTaxAmt { get; set; }
        public abstract class usrGWPSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrGWPSTTaxAmt> { }
        #endregion

        #region UsrGWQSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "GiftWrap QST Tax")]
        public virtual decimal? UsrGWQSTTaxAmt { get; set; }
        public abstract class usrGWQSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrGWQSTTaxAmt> { }
        #endregion

        #region UsrGWHSTTaxAmt
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "GiftWrap HST Tax")]
        public virtual decimal? UsrGWHSTTaxAmt { get; set; }
        public abstract class usrGWHSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrGWHSTTaxAmt> { }
        #endregion

        #region UsrFrtGSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight GST Tax")]
        public virtual decimal? UsrFrtGSTTaxAmt { get; set; }
        public abstract class usrFrtGSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrFrtGSTTaxAmt> { }
        #endregion

        #region UsrFrtPSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight PST Tax")]
        public virtual decimal? UsrFrtPSTTaxAmt { get; set; }
        public abstract class usrFrtPSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrFrtPSTTaxAmt> { }
        #endregion

        #region UsrFrtQSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight QST Tax")]
        public virtual decimal? UsrFrtQSTTaxAmt { get; set; }
        public abstract class usrFrtQSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrFrtQSTTaxAmt> { }
        #endregion

        #region UsrFrtHSTTaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Freight HST Tax")]
        public virtual decimal? UsrFrtHSTTaxAmt { get; set; }
        public abstract class usrFrtHSTTaxAmt : PX.Data.BQL.BqlDecimal.Field<usrFrtHSTTaxAmt> { }
        #endregion

        #region UsrCarrier
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Carrier")]
        public virtual string UsrCarrier { get; set; }
        public abstract class usrCarrier : PX.Data.BQL.BqlString.Field<usrCarrier> { }
        #endregion

        #region UsrTrackingNbr
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Tacking Nbr.")]
        public virtual string UsrTrackingNbr { get; set; }
        public abstract class usrTrackingNbr : PX.Data.BQL.BqlString.Field<usrTrackingNbr> { }
        #endregion
    }
}