using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ExternalLogisticsAPI.DAC;

namespace ExternalLogisticsAPI.Descripter
{
    public class ThreeDCartHelper
    {
        public const string StockItemNonExists = "[{0}] Doesn't Exist In The System.";

        public static async Task<List<MyArray>> GetResponse(LUM3DCartSetup setup, string specifyLocation)
        {
            string responseData = null;
            var baseAddress = new Uri(setup.SecureURL);

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("secureurl", "");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("privatekey", "");

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("token", "");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", setup.AuthToken);

                using (var response = await httpClient.GetAsync(specifyLocation) )
                {
                    responseData = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<MyArray>>(responseData);
                }
            }
        }

        public static void PrepareRecords(LUM3DCartSetup curSetup)
        {
            LUM3DCartImportProc graph = PXGraph.CreateInstance<LUM3DCartImportProc>();

            LUM3DCartProcessOrder processOrder = graph.ImportOrderList.Current;

            if (processOrder == null)
            {
                try
                {
                    DeleteWrkTableRecs();
                    CreateProcessOrders(GetResponse(curSetup, "3dCartWebAPI/v2/Orders?datestart=05/13/2021 00:00:00&limit=1000&orderstatus=1").Result);
                }
                catch (Exception ex)
                {
                    PXProcessing.SetError<LUM3DCartProcessOrder>(ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get 3D Cart API response collection to insert data into custom table.
        /// </summary>
        public static void CreateProcessOrders(List<MyArray> arrays)
        {
            LUM3DCartImportProc graph = PXGraph.CreateInstance<LUM3DCartImportProc>();

            for (int i = 0; i < arrays.Count; i++)
            {
                LUM3DCartProcessOrder procOrder = new LUM3DCartProcessOrder()
                {
                    LineNumber = i + 1,
                    InvoiceNumber = arrays[i].InvoiceNumber,
                    OrderID = arrays[i].OrderID.ToString(),
                    CustomerID = arrays[i].CustomerID == 0 ? null : arrays[i].CustomerID.ToString(),
                    OrderDate = arrays[i].OrderDate,
                    OrderStatusID = arrays[i].OrderStatusID.ToString(),
                    OrderAmount = (decimal)arrays[i].OrderAmount,
                    SalesTaxAmt = (decimal)(arrays[i].SalesTax + arrays[i].SalesTax2),
                    LastUpdated = arrays[i].LastUpdate,
                    BillingEmailID = arrays[i].BillingEmail,
                    Processed = false,
                    ProcessID = GetProcessID(graph)
                };

                graph.ImportOrderList.Cache.Insert(procOrder);
            }

            graph.Actions.PressSave();
        }

        /// <summary>
        /// Create an sales order from external logistic.
        /// </summary>
        public static void ImportRecords(LUM3DCartSetup curSetup, LUM3DCartProcessOrder processOrder)
        {
            try
            {
                SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();

                SOOrder order = orderEntry.Document.Cache.CreateInstance() as SOOrder;

                order.OrderType        = curSetup.OrderType;
                order.CustomerID       = curSetup.CustomerID;
                order.CustomerOrderNbr = processOrder.OrderID;
                order.DocDate          = processOrder.OrderDate;

                order = orderEntry.Document.Insert(order);

                CreateOrderDetail(orderEntry, curSetup, order);

                orderEntry.Save.Press();
            }
            catch (PXException ex)
            {
                PXProcessing.SetError<LUM3DCartProcessOrder>(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Generate sales order line.
        /// </summary>
        public static void CreateOrderDetail(SOOrderEntry orderEntry, LUM3DCartSetup curSetup, SOOrder order)
        {
            try
            {
                List<MyArray> list = GetResponse(curSetup, "3dCartWebAPI/v2/Orders/" + order.CustomerOrderNbr).Result;

                //order.PaymentMethodID = GetAcuPymtMethod(orderEntry, list[0].BillingPaymentMethod);

                UpdateSOContactAddress(orderEntry, list, order);

                var itemList = list.Find(x => x.OrderItemList.Count > 0).OrderItemList;

                for (int i = 0; i < itemList.Count; i++)
                {
                    SOLine line = orderEntry.Transactions.Cache.CreateInstance() as SOLine;

                    line.InventoryID = GetAcuInventoryID(orderEntry, itemList[i].ItemID);
                    line.OrderQty = (decimal)itemList[i].ItemQuantity;
                    line.UnitPrice = (decimal)itemList[i].ItemUnitPrice;

                    orderEntry.Transactions.Insert(line);
                }
            }
            catch (PXException ex)
            {
                PXProcessing.SetError<LUM3DCartProcessOrder>(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Override billing, shipping contact & address.
        /// </summary>
        public static void UpdateSOContactAddress(SOOrderEntry orderEntry, List<MyArray> list, SOOrder order)
        {
            SOBillingContact billContact = orderEntry.Billing_Contact.Current;
            SOBillingAddress billAddress = orderEntry.Billing_Address.Current;

            order.ContactID = CreateSOContact(order.CustomerID, list[0]);

            billContact.OverrideContact = true;
            billContact.Attention       = list[0].BillingFirstName + "," + list[0].BillingLastName;
            billContact.FullName        = list[0].BillingCompany;
            billContact.Phone1          = list[0].BillingPhoneNumber;
            billContact.Email           = list[0].BillingEmail;

            orderEntry.Billing_Contact.Update(billContact);

            billAddress.OverrideAddress = true;
            billAddress.AddressLine1    = list[0].BillingAddress;
            billAddress.AddressLine2    = list[0].BillingAddress2;
            billAddress.City            = list[0].BillingCity;
            billAddress.CountryID       = list[0].BillingCountry;
            billAddress.PostalCode      = list[0].BillingZipCode;
            billAddress.State           = list[0].BillingState;

            orderEntry.Billing_Address.Update(billAddress);

            var shipList = list.Find(x => x.ShipmentList.Count > 0).ShipmentList;

            SOShippingContact shipContact = orderEntry.Shipping_Contact.Current;
            SOShippingAddress shipAddress = orderEntry.Shipping_Address.Current;

            for (int i = 0; i < shipList.Count; i++)
            {
                order.OrderDesc = string.Format("Tracking Nbr. : {0}", shipList[i].ShipmentTrackingCode);
                order.CuryPremiumFreightAmt = (decimal)shipList[i].ShipmentCost;

                shipContact.OverrideContact = true;
                shipContact.Attention       = shipList[i].ShipmentFirstName + "," + shipList[i].ShipmentLastName;
                shipContact.FullName        = shipList[i].ShipmentCompany;
                shipContact.Phone1          = shipList[i].ShipmentPhone;
                shipContact.Email           = shipList[i].ShipmentEmail;

                orderEntry.Shipping_Contact.Update(shipContact);

                shipAddress.OverrideAddress = true;
                shipAddress.AddressLine1    = shipList[i].ShipmentAddress;
                shipAddress.AddressLine2    = shipList[i].ShipmentAddress2;
                shipAddress.City            = shipList[i].ShipmentCity;
                shipAddress.CountryID       = shipList[i].ShipmentCountry;
                shipAddress.PostalCode      = shipList[i].ShipmentZipCode;
                shipAddress.State           = shipList[i].ShipmentState;

                orderEntry.Shipping_Address.Update(shipAddress);
            }
        }

        /// <summary>
        /// If external logistic email doesn't exist in Acumatica contact, then create it.
        /// </summary>
        public static int? CreateSOContact(int? sOCustomeID, MyArray myArray)
        {
            ContactMaint contactGraph = PXGraph.CreateInstance<ContactMaint>();

            Contact origContact = SelectFrom<Contact>.Where<Contact.eMail.IsEqual<@P.AsString>>.View.Select(contactGraph, myArray.BillingEmail);

            if (origContact == null)
            {
                Contact contact = contactGraph.ContactCurrent.Cache.CreateInstance() as Contact;

                contact.ContactType = ContactTypesAttribute.Person;
                contact.BAccountID  = sOCustomeID;
                contact.FirstName   = myArray.BillingFirstName;
                contact.LastName    = myArray.BillingLastName;
                contact.EMail       = myArray.BillingEmail;
                contact.Phone1      = myArray.BillingPhoneNumber;

                contactGraph.ContactCurrent.Insert(contact);

                Address address = contactGraph.AddressCurrent.Select();

                address.AddressLine1 = myArray.BillingAddress;
                address.AddressLine2 = myArray.BillingAddress2;
                address.City         = myArray.BillingCity;
                address.CountryID    = myArray.BillingCountry;
                address.PostalCode   = myArray.BillingZipCode;
                address.State        = myArray.BillingState;

                contactGraph.AddressCurrent.Update(address);

                contactGraph.Save.Press();
            }

            return contactGraph.ContactCurrent.Current?.ContactID ?? origContact.ContactID;
        }

        /// <summary>
        /// The process ID value is generated based on the number of executions.
        /// </summary>
        public static int? GetProcessID(PXGraph graph)
        {
            LUM3DCartProcessOrder objProcess = PXSelectGroupBy<LUM3DCartProcessOrder, Aggregate<Max<LUM3DCartProcessOrder.processID>>>.Select(graph);

            return objProcess != null && objProcess.ProcessID != null ? (objProcess.ProcessID + 1) : 1;
        }

        /// <summary>
        /// Get Acumatica inventory ID by item name.
        /// </summary>
        public static int? GetAcuInventoryID(PXGraph graph, string inventoryCD)
        {
            InventoryItem item = SelectFrom<InventoryItem>.Where<InventoryItem.inventoryCD.Contains<@P.AsString>>.View.Select(graph, inventoryCD);

            if (item != null)
            {
                return item.InventoryID;
            }
            else
            {
                throw new PXException(StockItemNonExists, inventoryCD);
            }
        }

        /// <summary>
        /// Get Acumatica payment method ID by payment description.
        /// </summary>
        public static string GetAcuPymtMethod(PXGraph graph, string paymDescr)
        {
            PaymentMethod paymMethod = SelectFrom<PaymentMethod>.Where<PaymentMethod.descr.Contains<@P.AsString>>.View.Select(graph, paymDescr);

            return paymMethod?.PaymentMethodID;
        }

        /// <summary>
        /// Delete specify table record by Processed is not true.
        /// </summary>
        protected static void DeleteWrkTableRecs()
        {
            PXDatabase.Delete<LUM3DCartProcessOrder>(new PXDataFieldRestrict<LUM3DCartProcessOrder.processed>(PXDbType.Bit, false));
        }

        protected static void Update3DCartOrderStatus()
        {

        }
    }

    #region Entity Classes
    public class ShipmentList
    {
        public int ShipmentID { get; set; }
        public DateTime ShipmentLastUpdate { get; set; }
        public int ShipmentBoxes { get; set; }
        public string ShipmentInternalComment { get; set; }
        public int ShipmentOrderStatus { get; set; }
        public string ShipmentAddress { get; set; }
        public string ShipmentAddress2 { get; set; }
        public string ShipmentAlias { get; set; }
        public string ShipmentCity { get; set; }
        public string ShipmentCompany { get; set; }
        public double ShipmentCost { get; set; }
        public string ShipmentCountry { get; set; }
        public string ShipmentEmail { get; set; }
        public string ShipmentFirstName { get; set; }
        public string ShipmentLastName { get; set; }
        public int ShipmentMethodID { get; set; }
        public string ShipmentMethodName { get; set; }
        public string ShipmentShippedDate { get; set; }
        public string ShipmentPhone { get; set; }
        public string ShipmentState { get; set; }
        public string ShipmentZipCode { get; set; }
        public double ShipmentTax { get; set; }
        public double ShipmentWeight { get; set; }
        public string ShipmentTrackingCode { get; set; }
        public string ShipmentUserID { get; set; }
        public int ShipmentNumber { get; set; }
        public int ShipmentAddressTypeID { get; set; }
    }

    public class OrderItemList
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public double NumberOfItems { get; set; }
        public int WishlistItemID { get; set; }
        public int CatalogID { get; set; }
        public int ItemIndexID { get; set; }
        public string ItemID { get; set; }
        public string ItemDescription { get; set; }
        public int ItemShipmentID { get; set; }
        public double ItemQuantity { get; set; }
        public int ItemWarehouseID { get; set; }
        public double ItemUnitPrice { get; set; }
        public double ItemWeight { get; set; }
        public double ItemOptionPrice { get; set; }
        public string ItemAdditionalField1 { get; set; }
        public string ItemAdditionalField2 { get; set; }
        public string ItemAdditionalField3 { get; set; }
        public string ItemPageAdded { get; set; }
        public string ItemAvailability { get; set; }
        public DateTime ItemDateAdded { get; set; }
        public double ItemUnitCost { get; set; }
        public double ItemUnitStock { get; set; }
        public string ItemOptions { get; set; }
        public string ItemCatalogIDOptions { get; set; }
        public string ItemSerial { get; set; }
        public string ItemImage1 { get; set; }
        public string ItemImage2 { get; set; }
        public string ItemImage3 { get; set; }
        public string ItemImage4 { get; set; }
        public string ItemWarehouseLocation { get; set; }
        public string ItemWarehouseBin { get; set; }
        public string ItemWarehouseAisle { get; set; }
        public string ItemWarehouseCustom { get; set; }
        public int RecurringOrderFrequency { get; set; }
    }

    public class TransactionList
    {
        public int TransactionIndexID { get; set; }
        public int OrderID { get; set; }
        public string TransactionID { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string TransactionType { get; set; }
        public string TransactionMethod { get; set; }
        public double TransactionAmount { get; set; }
        public string TransactionApproval { get; set; }
        public string TransactionReference { get; set; }
        public int TransactionGatewayID { get; set; }
        public string TransactionCVV2 { get; set; }
        public string TransactionAVS { get; set; }
        public string TransactionResponseText { get; set; }
        public string TransactionResponseCode { get; set; }
        public int TransactionCaptured { get; set; }
    }

    public class QuestionList
    {
        public int QuestionAnswerIndexID { get; set; }
        public int OrderID { get; set; }
        public int QuestionID { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionAnswer { get; set; }
        public string QuestionType { get; set; }
        public int QuestionCheckoutStep { get; set; }
        public int QuestionSorting { get; set; }
        public int QuestionDiscountGroup { get; set; }
    }

    public class MyArray
    {
        public int OrderID { get; set; }
        public int OrderStatusID { get; set; }
        public int BillingPaymentMethodID { get; set; }
        public string InvoiceNumberPrefix { get; set; }
        public int InvoiceNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string UserID { get; set; }
        public string SalesPerson { get; set; }
        public string ContinueURL { get; set; }
        public string AlternateOrderID { get; set; }
        public string OrderType { get; set; }
        public string PaymentTokenID { get; set; }
        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingCompany { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZipCode { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPhoneNumber { get; set; }
        public string BillingEmail { get; set; }
        public string BillingPaymentMethod { get; set; }
        public bool BillingOnLinePayment { get; set; }
        public List<ShipmentList> ShipmentList { get; set; }
        public List<OrderItemList> OrderItemList { get; set; }
        public List<object> PromotionList { get; set; }
        public double OrderDiscount { get; set; }
        public double OrderDiscountCoupon { get; set; }
        public double OrderDiscountPromotion { get; set; }
        public double SalesTax { get; set; }
        public double SalesTax2 { get; set; }
        public double SalesTax3 { get; set; }
        public double OrderAmount { get; set; }
        public double AffiliateCommission { get; set; }
        public List<TransactionList> TransactionList { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string CardName { get; set; }
        public string CardExpirationMonth { get; set; }
        public string CardExpirationYear { get; set; }
        public string CardIssueNumber { get; set; }
        public string CardStartMonth { get; set; }
        public string CardStartYear { get; set; }
        public string CardAddress { get; set; }
        public string CardVerification { get; set; }
        public List<object> OfflinePaymentFieldList { get; set; }
        public int RewardPoints { get; set; }
        public List<QuestionList> QuestionList { get; set; }
        public string Referer { get; set; }
        public string IP { get; set; }
        public string CustomerComments { get; set; }
        public string InternalComments { get; set; }
        public string ExternalComments { get; set; }
        public List<object> ExternalIdsList { get; set; }
    }

    public class Root
    {
        public List<MyArray> MyArray { get; set; }
    }
    #endregion
}
