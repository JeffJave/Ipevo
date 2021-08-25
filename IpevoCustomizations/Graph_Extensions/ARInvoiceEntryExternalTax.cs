using System;
using PX.Common;
using PX.CS.Contracts.Interfaces;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.Common;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.TaxProvider;

namespace PX.Objects.AR
{
    public class ARInvoiceEntryExternalTax_Extension : PXGraphExtension<ARInvoiceEntryExternalTax, ARInvoiceEntry>
    {
		public const string Country_US = "US";
		public const string TaxCloud   = "TAXCLOUD";

		#region Delegate Methods
		public delegate GetTaxRequest BuildGetTaxRequestDelegate(ARInvoice invoice);
		[PXOverride]
		public GetTaxRequest BuildGetTaxRequest(ARInvoice invoice, BuildGetTaxRequestDelegate baseMethod) => BuildCommitTaxRequestWithFrt(invoice);
		#endregion

		public virtual CommitTaxRequest BuildCommitTaxRequestWithFrt(ARInvoice invoice)
		{
			if (invoice == null) throw new PXArgumentException(nameof(invoice), ErrorMessages.ArgumentNullException);

			Customer cust = (Customer)Base.customer.View.SelectSingleBound(new object[] { invoice });
			CR.Location loc = (CR.Location)Base.location.View.SelectSingleBound(new object[] { invoice });

			CommitTaxRequest request = new CommitTaxRequest();
			request.CompanyCode = Base1.CompanyCodeFromBranch(invoice.TaxZoneID, invoice.BranchID);
			request.CurrencyCode = invoice.CuryID;
			request.CustomerCode = cust.AcctCD;
			request.TaxRegistrationID = loc?.TaxRegistrationID;
			IAddressBase fromAddress = Base1.GetFromAddress(invoice);
			IAddressBase toAddress = Base1.GetToAddress(invoice);

			if (fromAddress == null) { throw new PXException(Messages.FailedGetFrom); }

			if (toAddress == null) { throw new PXException(Messages.FailedGetTo); }

			request.OriginAddress	   = AddressConverter.ConvertTaxAddress(fromAddress);
			request.DestinationAddress = AddressConverter.ConvertTaxAddress(toAddress);
			request.DocCode			   = $"AR.{invoice.DocType}.{invoice.RefNbr}";
			request.DocDate			   = invoice.DocDate.GetValueOrDefault();
			request.LocationCode	   = GetExternalTaxProviderLocationCode<ARTran, ARTran.FK.Invoice.SameAsCurrent, ARTran.siteID>(invoice);
			request.CustomerUsageType  = invoice.AvalaraCustomerUsageType;

			if (!string.IsNullOrEmpty(invoice.ExternalTaxExemptionNumber))
			{
				request.ExemptionNo = invoice.ExternalTaxExemptionNumber;
			}

			request.DocType = Base1.GetTaxDocumentType(invoice);
			Sign sign = Base1.GetDocumentSign(invoice);

			PXSelectBase<ARTran> select = new PXSelectJoin<ARTran, LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ARTran.inventoryID>>,
																			LeftJoin<Account, On<Account.accountID, Equal<ARTran.accountID>>>>,
																   Where<ARTran.tranType, Equal<Current<ARInvoice.docType>>,
																         And<ARTran.refNbr, Equal<Current<ARInvoice.refNbr>>,
																	         And<Where<ARTran.lineType, NotEqual<SOLineType.discount>, Or<ARTran.lineType, IsNull>>>>>,
																   OrderBy<Asc<ARTran.tranType, Asc<ARTran.refNbr, Asc<ARTran.lineNbr>>>>>(Base);

			request.Discount = Base.Document.Current.CuryDiscTot.GetValueOrDefault();
			DateTime? taxDate = invoice.OrigDocDate;

			bool applyRetainage = Base.ARSetup.Current?.RetainTaxes != true && invoice.IsOriginalRetainageDocument();

			/// <summary>
			/// Add the following condition and logic per Jira [IP-23]
			/// </summary>>
			string taxCategory = (Base as SOInvoiceEntry)?.FreightDetails.Current?.TaxCategoryID;

			if (invoice.CuryFreightTot > 0 && GL.Branch.PK.Find(Base, Base.Accessinfo.BranchID).CountryID == Country_US && invoice.TaxZoneID == TaxCloud && !string.IsNullOrEmpty(taxCategory))
			{
				var line = new TaxCartItem();
				line.Index = short.MinValue;
				line.Quantity = 1;
				line.UOM = "EA";
				line.Amount = sign * invoice.CuryFreightTot.GetValueOrDefault();
				line.Description = PXMessages.LocalizeNoPrefix(SO.Messages.FreightDesc);
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = "N/A";
				line.Discounted = false;
				line.TaxCode = taxCategory;

				request.CartItems.Add(line);
			}

			foreach (PXResult<ARTran, InventoryItem, Account> res in select.View.SelectMultiBound(new object[] { invoice }))
			{
				ARTran tran = (ARTran)res;
				InventoryItem item = (InventoryItem)res;
				Account salesAccount = (Account)res;

				var line = new TaxCartItem();
				line.Index = tran.LineNbr ?? 0;
				line.Amount = sign * (tran.CuryTranAmt.GetValueOrDefault() + (applyRetainage ? tran.CuryRetainageAmt.GetValueOrDefault() : 0m));
				line.Description = tran.TranDesc;
				line.DestinationAddress = AddressConverter.ConvertTaxAddress(Base1.GetToAddress(invoice, tran));
				line.OriginAddress = AddressConverter.ConvertTaxAddress(Base1.GetFromAddress(invoice, tran));
				line.ItemCode = item.InventoryCD;
				line.Quantity = Math.Abs(tran.Qty.GetValueOrDefault());
				line.UOM = tran.UOM;
				line.Discounted = tran.LineType != SOLineType.Freight && request.Discount > 0;
				line.RevAcct = salesAccount.AccountCD;

				line.TaxCode = tran.TaxCategoryID;
				line.CustomerUsageType = tran.AvalaraCustomerUsageType;

				if (tran.OrigInvoiceDate != null)
					taxDate = tran.OrigInvoiceDate;

				request.CartItems.Add(line);
			}

			if (applyRetainage)
			{
				var line = new TaxCartItem();
				line.Index = invoice.LineCntr.GetValueOrDefault() + 1;
				line.Amount = Sign.Minus * sign * invoice.CuryLineRetainageTotal.GetValueOrDefault();
				line.Description = PXMessages.LocalizeFormatNoPrefix(AP.Messages.RetainageForTransactionDescription, GetLabel.For<ARDocType>(invoice.DocType), invoice.RefNbr);
				line.DestinationAddress = request.DestinationAddress;
				line.OriginAddress = request.OriginAddress;
				line.ItemCode = "Retainage";
				line.Discounted = false;
				line.NonTaxable = true;

				request.CartItems.Add(line);
			}

			if ((invoice.DocType == ARDocType.CreditMemo || invoice.DocType == ARDocType.CashReturn) && invoice.OrigDocDate != null)
			{
				request.TaxOverride.Reason = Messages.ReturnReason;
				request.TaxOverride.TaxDate = taxDate.Value;
				request.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
				sign = Sign.Minus;
			}

			return request;
		}

		protected string GetExternalTaxProviderLocationCode<TLine, TLineDocFK, TLineSiteID>(ARInvoice document)
			where TLine : class, IBqlTable, new()
			where TLineDocFK : IParameterizedForeignKeyBetween<TLine, ARInvoice>, new()
			where TLineSiteID : IBqlField
		{
			TLine lineWithSite = PXSelect<TLine, Where2<TLineDocFK, And<TLineSiteID, IsNotNull>>>.SelectSingleBound(Base, new[] { document });

			if (lineWithSite == null)
				return null;

			var site = PX.Objects.IN.INSite.PK.Find(Base, (int?)Base.Caches<TLine>().GetValue<TLineSiteID>(lineWithSite));
			return site?.SiteCD;
		}
	}
}
