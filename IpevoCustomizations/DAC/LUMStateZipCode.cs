using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;

namespace IpevoCustomizations.DAC
{
    [Serializable]
    [PXCacheName("State And Zip Code")]
    [PXPrimaryGraph(typeof(IpevoCustomizations.Graph.LUMStateZipCodeMaint))]
    public class LUMStateZipCode : IBqlTable
    {
        #region CountryID
        [PXDBString(2, IsKey = true, IsUnicode = true, InputMask = ">??")]
        [PXUIField(DisplayName = "Country")]
        [Country()]
        [PXDefault(typeof(Search<Branch.countryID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string CountryID { get; set; }
        public abstract class countryID : PX.Data.BQL.BqlString.Field<countryID> { }
        #endregion

        #region State
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "State")]
        [PXSelector(typeof(Search<State.stateID, Where<State.countryID, Equal<Current<countryID>>>>), 
                    DescriptionField = typeof(State.name), ValidateValue = false)]
        public virtual string State { get; set; }
        public abstract class state : PX.Data.BQL.BqlString.Field<state> { }
        #endregion

        #region ZipCode
        [PXDBString(20, IsKey = true)]
        [PXUIField(DisplayName = "Zip Code")]
        public virtual string ZipCode { get; set; }
        public abstract class zipCode : PX.Data.BQL.BqlString.Field<zipCode> { }
        #endregion

        #region CountyName
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "County Name")]
        public virtual string CountyName { get; set; }
        public abstract class countyName : PX.Data.BQL.BqlString.Field<countyName> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    }
}