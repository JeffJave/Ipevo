using System;
using System.Linq;
using System.Diagnostics;
using PX.TaxProvider;
using PX.Common;
using PX.CS.Contracts.Interfaces;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;

namespace PX.Objects.SO
{
    public class SOOrderEntryExternalTax_Extension : PXGraphExtension<SOOrderEntryExternalTax, SOOrderEntry>
    {
        #region Delegate Methods
        public delegate GetTaxRequest BuildGetTaxRequestDelegate(SOOrder order);
		[PXOverride]
		public GetTaxRequest BuildGetTaxRequest(SOOrder order, BuildGetTaxRequestDelegate baseMethod) =>
			BuildGetTaxRequestWithFRTCate<SOLine.curyLineAmt, SOLine.orderQty, SOOrder.curyDiscTot>(order, $"SO.{order.OrderType}.{order.OrderNbr}", nameof(BuildGetTaxRequest));

		public delegate GetTaxRequest BuildGetTaxRequestOpenDelegate(SOOrder order);
		[PXOverride]
		public GetTaxRequest BuildGetTaxRequestOpen(SOOrder order, BuildGetTaxRequestOpenDelegate baseMethod) =>
			BuildGetTaxRequestWithFRTCate<SOLine.curyOpenAmt, SOLine.openQty, SOOrder.curyOpenDiscTotal>(order, $"SO.{order.OrderType}.{order.OrderNbr}", nameof(BuildGetTaxRequestOpen));

		public delegate GetTaxRequest BuildGetTaxRequestUnbilledDelegate(SOOrder order);
		[PXOverride]
		public GetTaxRequest BuildGetTaxRequestUnbilled(SOOrder order, BuildGetTaxRequestUnbilledDelegate baseMethod) =>
			BuildGetTaxRequestWithFRTCate<SOLine.curyUnbilledAmt, SOLine.unbilledQty, SOOrder.curyUnbilledDiscTotal>(order, $"{order.OrderType}.{order.OrderNbr}.Open", nameof(BuildGetTaxRequestUnbilled));
		#endregion

		protected GetTaxRequest BuildGetTaxRequestWithFRTCate<TLineAmt, TLineQty, TDocDiscount>(SOOrder order, string docCode, string debugMethodName)
			where TLineAmt : IBqlField
			where TLineQty : IBqlField
			where TDocDiscount : IBqlField
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			Debug.Indent();

			if (order == null) { throw new PXArgumentException(nameof(order)); }

			Customer cust = (Customer)Base.customer.View.SelectSingleBound(new object[] { order });
			Location loc = (Location)Base.location.View.SelectSingleBound(new object[] { order });

			IAddressBase fromAddress = GetFromAddress(order);
			IAddressBase toAddress = GetToAddress(order);

			Debug.Print($"{DateTime.Now.TimeOfDay} Select Customer, Location, Addresses in {sw.ElapsedMilliseconds} millisec");

			if (fromAddress == null) { throw new PXException(Messages.FailedGetFromAddressSO); }
			if (toAddress == null) { throw new PXException(Messages.FailedGetToAddressSO); }

			GetTaxRequest request = new GetTaxRequest();

			request.CompanyCode = Base1.CompanyCodeFromBranch(order.TaxZoneID, order.BranchID);
			request.CurrencyCode = order.CuryID;
			request.CustomerCode = cust.AcctCD;
			request.TaxRegistrationID = loc?.TaxRegistrationID;
			request.OriginAddress = AddressConverter.ConvertTaxAddress(fromAddress);
			request.DestinationAddress = AddressConverter.ConvertTaxAddress(toAddress);
			request.DocCode = docCode;
			request.DocDate = order.OrderDate.GetValueOrDefault();
			request.LocationCode = GetExternalTaxProviderLocationCode<SOLine, SOLine.FK.Order.SameAsCurrent, SOLine.siteID>(order);

			Sign docSign = Sign.Plus;

			request.CustomerUsageType = order.AvalaraCustomerUsageType;

			if (!string.IsNullOrEmpty(order.ExternalTaxExemptionNumber))
			{
				request.ExemptionNo = order.ExternalTaxExemptionNumber;
			}

			SOOrderType orderType = (SOOrderType)Base.soordertype.View.SelectSingleBound(new object[] { order });

			if (orderType.DefaultOperation == SOOperation.Receipt)
			{
				request.DocType = TaxDocumentType.ReturnOrder;
				docSign = Sign.Minus;
			}
			else
			{
				request.DocType = TaxDocumentType.SalesOrder;
			}

			PXSelectBase<SOLine> select = new PXSelectJoin<SOLine, LeftJoin<InventoryItem, On<SOLine.FK.InventoryItem>,
																			LeftJoin<Account, On<Account.accountID, Equal<SOLine.salesAcctID>>>>,
																   Where<SOLine.orderType, Equal<Current<SOOrder.orderType>>,
																		 And<SOLine.orderNbr, Equal<Current<SOOrder.orderNbr>>>>,
																   OrderBy<Asc<SOLine.orderType,
																			   Asc<SOLine.orderNbr,
																				   Asc<SOLine.lineNbr>>>>>(Base);

			PXCache documentCache = Base.Caches[typeof(SOOrder)];

			request.Discount = (documentCache.GetValue<TDocDiscount>(order) as decimal?) ?? 0m;

			Stopwatch sw2 = new Stopwatch();
			sw2.Start();
			
									     ///<remarks> Add additional condition to skip the standard calculation always including the freight amount per YJ's request.</remarks>
			if (order.CuryFreightTot > 0 && !string.IsNullOrEmpty(order.FreightTaxCategoryID))
			{
				var line = new TaxCartItem();
				line.Index = short.MinValue;
				line.Quantity = 1;
				line.UOM = "EA";
				line.Amount = docSign * order.CuryFreightTot.GetValueOrDefault();
				line.Description = PXMessages.LocalizeNoPrefix(Messages.FreightDesc);
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = "N/A";
				line.Discounted = false;
				line.TaxCode = order.FreightTaxCategoryID;

				request.CartItems.Add(line);
			}

			PXCache lineCache = Base.Caches[typeof(SOLine)];

			int		totalLine   = select.Select().RowCast<SOLine>().Count();
			decimal totalAmount = 0m;
			foreach (PXResult<SOLine, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { order }))
			{
				SOLine tran = (SOLine)res;
				Account salesAccount = (Account)res;
				InventoryItem item = (InventoryItem)res;

				bool lineIsDiscounted = request.Discount > 0m && ((tran.DocumentDiscountRate ?? 1m) != 1m || (tran.GroupDiscountRate ?? 1m) != 1m);

				var line = new TaxCartItem();
				line.Index = tran.LineNbr ?? 0;

				decimal lineAmount  = (lineCache.GetValue<TLineAmt>(tran) as decimal?) ?? 0m;
				decimal lineQty		= (lineCache.GetValue<TLineQty>(tran) as decimal?) ?? 0m;
				
				line.Description = tran.TranDesc;
				line.DestinationAddress = AddressConverter.ConvertTaxAddress(Base1.GetToAddress(order, tran));
				line.OriginAddress = AddressConverter.ConvertTaxAddress(Base1.GetFromAddress(order, tran));
				line.ItemCode = item.InventoryCD;
				line.Quantity = Math.Abs(lineQty);
				line.UOM = tran.UOM;
				line.Discounted = lineIsDiscounted;
				line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;
				line.CustomerUsageType = tran.AvalaraCustomerUsageType;

				if (tran.Operation == SOOperation.Receipt && tran.InvoiceDate != null)
				{
					line.TaxOverride.Reason = Messages.ReturnReason;
					line.TaxOverride.TaxDate = tran.InvoiceDate.Value;
					line.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
				}

				///<remarks> According to Jira [Ip-42] to do the special logic. </remarks>
				if (order.OrderType == "RA" && order.CuryOrderTotal > 0)
				{
					totalAmount  += orderType.DefaultOperation != tran.Operation ? Sign.Minus * docSign * lineAmount : docSign * lineAmount;
					line.Amount	  = totalAmount;
					line.Quantity = 1;

					if (tran.SortOrder != totalLine)  { continue; }
				}
				else
				{
					line.Amount = orderType.DefaultOperation != tran.Operation ? Sign.Minus * docSign * lineAmount : docSign * lineAmount;
				}

				request.CartItems.Add(line);
			}

			sw2.Stop();
			Debug.Print($"{DateTime.Now.TimeOfDay} Select detail lines in {sw2.ElapsedMilliseconds} millisec.");

			Debug.Unindent();
			sw.Stop();
			Debug.Print($"{DateTime.Now.TimeOfDay} {debugMethodName}() in {sw.ElapsedMilliseconds} millisec.");

			return request;
		}


		#region Standard Protected/Private level Methods
		protected IAddressBase ValidAddressFrom<TFieldSource>(IAddressBase address)
            where TFieldSource : IBqlField
        {
            if (!IsEmptyAddress(address)) return address;
            throw new PXException(PickAddressError<TFieldSource>(address));
        }

		protected virtual IAddressBase GetFromAddress(SOOrder order)
		{
			var branch = PXSelectJoin<Branch, InnerJoin<BAccountR, On<BAccountR.bAccountID, Equal<Branch.bAccountID>>,
											  InnerJoin<Address, On<Address.addressID, Equal<BAccountR.defAddressID>>>>,
											  Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, order.BranchID)
																										.RowCast<Address>()
																										.FirstOrDefault()
																										.With(ValidAddressFrom<BAccountR.defAddressID>);

			return branch;
		}

		protected virtual IAddressBase GetToAddress(SOOrder order)
        {
            if (order.WillCall == true)
                return GetFromAddress(order);
            else
                return ((SOShippingAddress)PXSelect<SOShippingAddress, Where<SOShippingAddress.addressID, Equal<Required<SOOrder.shipAddressID>>>>.Select(Base, order.ShipAddressID)).With(ValidAddressFrom<SOOrder.shipAddressID>);
        }

		protected string GetExternalTaxProviderLocationCode<TLine, TLineDocFK, TLineSiteID>(SOOrder document)
			where TLine : class, IBqlTable, new()
			where TLineDocFK : IParameterizedForeignKeyBetween<TLine, SOOrder>, new()
			where TLineSiteID : IBqlField
		{
			TLine lineWithSite = PXSelect<TLine, Where2<TLineDocFK, And<TLineSiteID, IsNotNull>>>.SelectSingleBound(Base, new[] { document });

			if (lineWithSite == null)
				return null;

			var site = PX.Objects.IN.INSite.PK.Find(Base, (int?)Base.Caches<TLine>().GetValue<TLineSiteID>(lineWithSite));
			return site?.SiteCD;
		}

		protected string PickAddressError<TFieldSource>(IAddressBase address)
			where TFieldSource : IBqlField
		{
			if (typeof(TFieldSource) == typeof(SOOrder.shipAddressID))
				return PXSelect<SOOrder, Where<SOOrder.shipAddressID, Equal<Required<Address.addressID>>>>
					.SelectWindowed(Base, 0, 1, ((SOAddress)address).AddressID).First().GetItem<SOOrder>()
					.With(e => PXMessages.LocalizeFormat(AR.Messages.AvalaraAddressSourceError, EntityHelper.GetFriendlyEntityName<SOOrder>(), new EntityHelper(Base).GetRowID(e)));

			if (typeof(TFieldSource) == typeof(Vendor.defLocationID))
				return PXSelectReadonly2<Vendor, InnerJoin<Location, On<Location.locationID, Equal<Vendor.defLocationID>>>, Where<Location.defAddressID, Equal<Required<Address.addressID>>>>
					.SelectWindowed(Base, 0, 1, ((Address)address).AddressID).First().GetItem<Vendor>()
					.With(e => PXMessages.LocalizeFormat(AR.Messages.AvalaraAddressSourceError, EntityHelper.GetFriendlyEntityName<Vendor>(), new EntityHelper(Base).GetRowID(e)));

			if (typeof(TFieldSource) == typeof(INSite.addressID))
				return PXSelectReadonly<INSite, Where<INSite.addressID, Equal<Required<Address.addressID>>>>
					.SelectWindowed(Base, 0, 1, ((Address)address).AddressID).First().GetItem<INSite>()
					.With(e => PXMessages.LocalizeFormat(AR.Messages.AvalaraAddressSourceError, EntityHelper.GetFriendlyEntityName<INSite>(), new EntityHelper(Base).GetRowID(e)));

			if (typeof(TFieldSource) == typeof(BAccountR.defAddressID))
				return PXSelectReadonly<BAccountR, Where<BAccountR.defAddressID, Equal<Required<Address.addressID>>>>
					.SelectWindowed(Base, 0, 1, ((Address)address).AddressID).First().GetItem<BAccountR>()
					.With(e => PXMessages.LocalizeFormat(AR.Messages.AvalaraAddressSourceError, EntityHelper.GetFriendlyEntityName<BAccountR>(), new EntityHelper(Base).GetRowID(e)));

			throw new ArgumentOutOfRangeException("Unknown address source used");
		}
		#endregion

		#region Static Method
		public static bool IsEmptyAddress(IAddressBase address) => string.IsNullOrEmpty(address?.PostalCode);
		#endregion
	}
}
