﻿using System;
using PX.Data;
using ExternalLogisticsAPI.Descripter;

namespace ExternalLogisticsAPI.DAC
{
    [Serializable]
    [PXCacheName("3DCart Process Order")]
    public class LUM3DCartProcessOrder : IBqlTable
    {
        #region Selected
        [PXBool()]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region LineNumber
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Line Number")]
        public virtual int? LineNumber { get; set; }
        public abstract class lineNumber : PX.Data.BQL.BqlInt.Field<lineNumber> { }
        #endregion

        #region InvoiceNumber
        [PXDBInt()]
        [PXUIField(DisplayName = "Invoice Number")]
        public virtual int? InvoiceNumber { get; set; }
        public abstract class invoiceNumber : PX.Data.BQL.BqlInt.Field<invoiceNumber> { }
        #endregion

        #region OrderID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order ID")]
        public virtual string OrderID { get; set; }
        public abstract class orderID : PX.Data.BQL.BqlString.Field<orderID> { }
        #endregion

        #region CustomerID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Customer ID")]
        public virtual string CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlString.Field<customerID> { }
        #endregion

        #region OrderDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Order Date")]
        public virtual DateTime? OrderDate { get; set; }
        public abstract class orderDate : PX.Data.BQL.BqlDateTime.Field<orderDate> { }
        #endregion

        #region OrderStatusID
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Status ID")]
        [ThreeDCartOrderStatus.List]
        public virtual string OrderStatusID { get; set; }
        public abstract class orderStatusID : PX.Data.BQL.BqlString.Field<orderStatusID> { }
        #endregion

        #region OrderAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Order Amount")]
        public virtual Decimal? OrderAmount { get; set; }
        public abstract class orderAmount : PX.Data.BQL.BqlDecimal.Field<orderAmount> { }
        #endregion

        #region SalesTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sales Tax Amt")]
        public virtual Decimal? SalesTaxAmt { get; set; }
        public abstract class salesTaxAmt : PX.Data.BQL.BqlDecimal.Field<salesTaxAmt> { }
        #endregion

        #region LastUpdated
        [PXDBDate()]
        [PXUIField(DisplayName = "Last Updated")]
        public virtual DateTime? LastUpdated { get; set; }
        public abstract class lastUpdated : PX.Data.BQL.BqlDateTime.Field<lastUpdated> { }
        #endregion

        #region BillingEmailID
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Billing Email ID")]
        public virtual string BillingEmailID { get; set; }
        public abstract class billingEmailID : PX.Data.BQL.BqlString.Field<billingEmailID> { }
        #endregion

        #region Processed
        [PXDBBool()]
        [PXUIField(DisplayName = "Processed")]
        public virtual bool? Processed { get; set; }
        public abstract class processed : PX.Data.BQL.BqlBool.Field<processed> { }
        #endregion

        #region ProcessID
        [PXDBInt()]
        [PXUIField(DisplayName = "Process ID")]
        public virtual int? ProcessID { get; set; }
        public abstract class processID : PX.Data.BQL.BqlInt.Field<processID> { }
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
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}