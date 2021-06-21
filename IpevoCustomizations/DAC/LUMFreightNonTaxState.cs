using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using IpevoCustomizations.Graph;
using PX.Data.ReferentialIntegrity.Attributes;

namespace IpevoCustomizations.DAC
{
    [Serializable]
    [PXCacheName("Freight Non-Tax State")]
    [PXPrimaryGraph(typeof(LUMFrtNonTaxStateMaint))]
    public class LUMFreightNonTaxState : IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<LUMFreightNonTaxState>.By<stateID>
        {
            public static LUMFreightNonTaxState Find(PXGraph graph, string stateID) => FindBy(graph, stateID);
        }
        #endregion

        #region StateID
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "State ID")]
        [PXDefault()]
        [PXSelector(typeof(Search2<State.stateID, InnerJoin<Branch, On<Branch.countryID, Equal<State.countryID>>>,
                                                  Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>), 
                    DescriptionField = typeof(State.name), Filterable = true)]
        public virtual string StateID { get; set; }
        public abstract class stateID : PX.Data.BQL.BqlString.Field<stateID> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
    }
}