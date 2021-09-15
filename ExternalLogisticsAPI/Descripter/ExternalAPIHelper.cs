﻿using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Graph;

namespace ExternalLogisticsAPI.Descripter
{
    public class ExternalAPIHelper
    {
        public const string StockItemNonExists = "[{0}] Doesn't Exist In The System.";

        public static async Task<List<MyArray>> GetResponse(LUM3DCartSetup setup, string specifyLocation, bool updateStatus = false)
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

                if (updateStatus == false)
                {
                    using (var response = await httpClient.GetAsync(specifyLocation))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return null;
                            //throw new JsonReaderException(response.ReasonPhrase);
                        }

                        responseData = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<List<MyArray>>(responseData);
                    }
                }
                else
                {
                    using (var content = new StringContent("\"OrderStatusID\":5", System.Text.Encoding.Default, "application/json"))
                    {
                        using (var response = await httpClient.PutAsync(specifyLocation, content))
                        {
                            responseData = await response.Content.ReadAsStringAsync();

                            return JsonConvert.DeserializeObject<List<MyArray>>(responseData);
                        }
                    }
                }    
            }
        }

        public static void PrepareRecords(LUM3DCartSetup curSetup, DateTime? endDate)
        {
            LUM3DCartImportProc graph = PXGraph.CreateInstance<LUM3DCartImportProc>();

            LUM3DCartProcessOrder processOrder = graph.ImportOrderList.Current;

            if (processOrder == null)
            {
                try
                {
                    DeleteWrkTableRecs();

                    var taskArr = GetResponse(curSetup, string.Format("3dCartWebAPI/v2/Orders?datestart={0}&limit={1}&orderstatus={2}", endDate.Value.AddDays(-7), 1000, ThreeDCartOrderStatus.New)).Result;

                    if (taskArr != null) { CreateProcessOrders(taskArr); }
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
                    LineNumber      = i + 1,
                    ProcessID       = GetProcessID(graph),
                    OrderID         = arrays[i].OrderID.ToString(),
                    InvoiceNumber   = arrays[i].InvoiceNumber,
                    OrderNbr        = string.Format("{0}{1}", arrays[i].InvoiceNumberPrefix, arrays[i].InvoiceNumber),
                    CustomerID      = arrays[i].CustomerID == 0 ? null : arrays[i].CustomerID.ToString(),
                    OrderDate       = arrays[i].OrderDate.Date,
                    OrderStatusID   = arrays[i].OrderStatusID.ToString(),
                    OrderAmount     = (decimal)arrays[i].OrderAmount,
                    SalesTaxAmt     = (decimal)(arrays[i].SalesTax + arrays[i].SalesTax2),
                    LastUpdated     = arrays[i].LastUpdate,
                    BillingEmailID  = arrays[i].BillingEmail,
                    Processed       = false,
                    BillingAddress  = arrays[i].BillingAddress,
                    ShipmentAddress = arrays[i].ShipmentList[0].ShipmentAddress,
                    OrderQty        = (decimal)arrays[i].OrderItemList.ToList().Sum(x => x.ItemQuantity)
                };

                if (LUM3DCartProcessOrder.UK.Find(graph, procOrder.OrderID, procOrder.InvoiceNumber) == null)
                {
                    graph.ImportOrderList.Cache.Insert(procOrder);
                }
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
                order.CustomerRefNbr   = processOrder.OrderNbr;
                order.OrderDesc        = $"Invoice No. : {processOrder.OrderNbr}";
                order.DocDate          = processOrder.OrderDate;

                order = orderEntry.Document.Insert(order);

                CreateOrderDetail(orderEntry, curSetup, order);

                orderEntry.Save.Press();

                CreatePaymentProcess(order);
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

                UpdateSOContactAddress(orderEntry, list, order);

                var itemList = list.Find(x => x.OrderItemList.Count > 0).OrderItemList;

                for (int i = 0; i < itemList.Count; i++)
                {
                    SOLine line = orderEntry.Transactions.Cache.CreateInstance() as SOLine;

                    line.InventoryID   = GetAcuInventoryID(orderEntry, itemList[i].ItemID);
                    line.OrderQty      = (decimal)itemList[i].ItemQuantity;
                    line.CuryUnitPrice = (decimal)itemList[i].ItemUnitPrice;

                    orderEntry.Transactions.Insert(line);
                }

                orderEntry.Taxes.Cache.SetValueExt<SOTaxTran.curyTaxAmt>(orderEntry.Taxes.Current, list[0].SalesTax + list[0].SalesTax2);
                orderEntry.CurrentDocument.SetValueExt<SOOrder.paymentMethodID>(order, GetAcuPymtMethod(orderEntry, order.CustomerID, list[0].BillingPaymentMethod));

                //string tranID = list.Find(x => x.TransactionList.Count > 0).TransactionList[0].TransactionID;
                //int    index  = tranID.IndexOf(':') + 2; // Because there is a space between the ':' of transaction ID.

                //order.CustomerRefNbr = tranID.Contains(PX.Objects.PO.Messages.Completed) ? tranID.Substring(index, tranID.Length - index) : null;

                // Refer to PX.AmazonIntegration project SetDocumentLevelDiscountandTaxData() to manully update SOOrder total amount fields.
                order.CuryTaxTotal    = orderEntry.Taxes.Current.CuryTaxAmt;
                order.CuryOrderTotal += order.CuryTaxTotal;
            }
            catch (PXException ex)
            {
                PXProcessing.SetError<LUM3DCartProcessOrder>(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Generate sales order line from AMZ interface.
        /// </summary>
        public static void CreateOrderDetail(SOOrderEntry orderEntry, object obj)
        {
            dynamic root = obj as APILibrary.Model.Amazon_Middleware.Root;

            if (root == null)
            {
                root = obj as APILibrary.Model.Amazon_Middleware.Root2;
            }

            for (int i = 0; i < root.item.Count; i++)
            {
                SOLine line = orderEntry.Transactions.Cache.CreateInstance() as SOLine;

                line.InventoryID   = InventoryItem.UK.Find(orderEntry, root.item[i].sku).InventoryID;
                line.OrderQty      = root.item[i].qty;
                line.CuryUnitPrice = (decimal)root.item[i].unit_price;

                SOLineExt lineExt = line.GetExtension<SOLineExt>();

                lineExt.UsrFulfillmentCenter = root.item[i].fulfillment_center_id;
                lineExt.UsrShipFromCountryID = root.item[i].country;
                lineExt.UsrShipFromState     = root.item[i].state;
                lineExt.UsrCarrier           = root.item[i].carrier;
                lineExt.UsrTrackingNbr       = root.item[i].tracking_no;

                for (int j = 0; j < root.item[i].charge.Count; j++)
                {
                    switch (root.item[i].charge[j].type)
                    {
                        case (int)AMZChargeType.Discount:
                            line.CuryDiscAmt = (line.CuryDiscAmt ?? 0m) + Math.Abs((decimal)root.item[i].charge[j].amount);
                            break;
                        case (int)AMZChargeType.Shipping_Tax:
                            lineExt.UsrFreightTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.Gift_Wrap_Tax:
                            lineExt.UsrGiftwrapTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.Item_Tax:
                            lineExt.UsrItemTaxAmt    = (decimal)root.item[i].charge[j].amount;
                            lineExt.UsrAmazWHTaxAmt  = -lineExt.UsrItemTaxAmt;
                            break;
                        case (int)AMZChargeType.GST_Shipping_Tax:
                            lineExt.UsrFrtGSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.GST_Gift_Wrap_Tax:
                            lineExt.UsrGWGSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.GST_Item_Tax:
                            lineExt.UsrItemGSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.HST_Shipping_Tax:
                            lineExt.UsrFrtHSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.HST_Gift_Wrap_Tax:
                            lineExt.UsrGWHSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.HST_Item_Tax:
                            lineExt.UsrItemHSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.QST_Shipping_Tax:
                            lineExt.UsrFrtQSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.QST_Gift_Wrap_Tax:
                            lineExt.UsrGWQSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.QST_Item_Tax:
                            lineExt.UsrItemQSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.PST_Shipping_Tax:
                            lineExt.UsrFrtPSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.PST_Gift_Wrap_Tax:
                            lineExt.UsrGWPSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                        case (int)AMZChargeType.PST_Item_Tax:
                            lineExt.UsrItemPSTTaxAmt = (decimal)root.item[i].charge[j].amount;
                            break;
                    }
                }

                orderEntry.Transactions.Insert(line);

                for (int k = 0; k < root.item[i].cop.Count; k++)
                {
                    ///<remarks> 
                    /// Because the standard SO Discount logic is calculated differently in RowInserted, RowUpdated and DiscountID_FieldUpdated events.
                    /// </remarks>
                    SOOrderDiscountDetail disDetail = orderEntry.DiscountDetails.Insert(orderEntry.DiscountDetails.Cache.CreateInstance() as SOOrderDiscountDetail);

                    disDetail.DiscountID = "COP";

                    orderEntry.DiscountDetails.Update(disDetail);

                    //orderEntry.DiscountDetails.Cache.SetValueExt<SOOrderDiscountDetail.curyDiscountAmt>(disDetail, Math.Abs((decimal)root.item[i].cop[k].amount));
                    disDetail.CuryDiscountAmt = Math.Abs((decimal)root.item[i].cop[k].amount);

                    orderEntry.DiscountDetails.Update(disDetail);
                }
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
                //order.OrderDesc = string.IsNullOrEmpty(shipList[i].ShipmentTrackingCode) ? null : string.Format("Tracking Nbr. : {0}", shipList[i].ShipmentTrackingCode);

                orderEntry.Document.Cache.SetValueExt<SOOrder.curyPremiumFreightAmt>(order, (decimal)shipList[i].ShipmentCost);

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
        /// Override billing, shipping contact & address from AMZ interface.
        /// </summary>
        public static void UpdateSOContactAddress(SOOrderEntry orderEntry, object obj)
        {
            dynamic root = obj as APILibrary.Model.Amazon_Middleware.Root;

            if (root == null)
            {
                root = obj as APILibrary.Model.Amazon_Middleware.Root2;
            }

            SOBillingContact billContact = orderEntry.Billing_Contact.Select();
            SOBillingAddress billAddress = orderEntry.Billing_Address.Select();

            billContact.OverrideContact = true;
            billContact.FullName        = root.buyer_name;
            billContact.Email           = root.buyer_email;

            orderEntry.Billing_Contact.Update(billContact);

            billAddress.OverrideAddress = true;
            billAddress.AddressLine1    = root.bill_address;
            billAddress.City            = root.bill_city;
            billAddress.CountryID       = string.IsNullOrEmpty(root.bill_country) ? billAddress.CountryID : root.bill_country;
            billAddress.PostalCode      = root.bill_postal_code;
            billAddress.State           = root.bill_state;

            orderEntry.Billing_Address.Update(billAddress);

            SOShippingContact shipContact = orderEntry.Shipping_Contact.Select();
            SOShippingAddress shipAddress = orderEntry.Shipping_Address.Select();

            shipContact.OverrideContact = true;
            shipContact.FullName        = root.recipient_name;

            orderEntry.Shipping_Contact.Update(shipContact);

            shipAddress.OverrideAddress = true;
            shipAddress.AddressLine1    = root.ship_address;
            shipAddress.City            = root.ship_city;
            shipAddress.CountryID       = root.ship_country;
            shipAddress.PostalCode      = root.ship_postal_code;
            shipAddress.State           = root.ship_state;

            orderEntry.Shipping_Address.Update(shipAddress);
        }

        /// <summary>
        /// If external logistic email doesn't exist in Acumatica contact, then create it.
        /// </summary>
        public static int? CreateSOContact(int? sOCustomeID, MyArray myArray)
        {
            ContactMaint contactGraph = PXGraph.CreateInstance<ContactMaint>();

            Contact origContact = SelectFrom<Contact>.Where<Contact.eMail.IsEqual<@P.AsString>
                                                            .And<Contact.bAccountID.IsEqual<@P.AsInt>>>.View.Select(contactGraph, myArray.BillingEmail, sOCustomeID);

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
        /// Manually create AR payment and related to specified sales order.
        /// </summary>
        public static void CreatePaymentProcess(SOOrder order)
        {
            ARPaymentEntry pymtEntry = PXGraph.CreateInstance<ARPaymentEntry>();

            ARPayment payment = new ARPayment()
            {
                DocType = ARPaymentType.Payment
            };

            payment = PXCache<ARPayment>.CreateCopy(pymtEntry.Document.Insert(payment));

            payment.CustomerID         = order.CustomerID;
            payment.CustomerLocationID = order.CustomerLocationID;
            payment.PaymentMethodID    = order.PaymentMethodID;
            payment.PMInstanceID       = order.PMInstanceID;
            payment.CuryOrigDocAmt     = 0m;
            payment.ExtRefNbr          = order.CustomerRefNbr ?? order.CustomerOrderNbr;
            payment.DocDesc            = order.OrderNbr;
            payment.AdjDate            = order.RequestDate;

            payment = pymtEntry.Document.Update(payment);

            SOAdjust adj = new SOAdjust()
            {
                AdjdOrderType = order.OrderType.Trim(),
                AdjdOrderNbr = order.OrderNbr.Trim()
            };
            pymtEntry.SOAdjustments.Insert(adj);

            if (payment.CuryOrigDocAmt == 0m)
            {
                payment.CuryOrigDocAmt = payment.CurySOApplAmt;
                pymtEntry.Document.Update(payment);
            }
            pymtEntry.Actions.PressSave();

            if (pymtEntry.Actions.Contains("Release"))
            {
                pymtEntry.releaseFromHold.Press();
                pymtEntry.release.Press();
            }
        }

        /// <summary>
        /// Manually create AR payment and related to specified sales order from AMZ interface.
        /// </summary>
        public static void CreatePaymentProcess(SOOrder order, object obj)
        {
            dynamic root = obj as APILibrary.Model.Amazon_Middleware.Root;

            bool hasAMZFee = false;

            if (root == null)
            {
                root = obj as APILibrary.Model.Amazon_Middleware.Root2;

                hasAMZFee = true;
            }

            ARPaymentEntry pymtEntry = PXGraph.CreateInstance<ARPaymentEntry>();

            ARPayment payment = new ARPayment()
            {
                DocType = ARPaymentType.Payment
            };

            payment = PXCache<ARPayment>.CreateCopy(pymtEntry.Document.Insert(payment));

            payment.CustomerID         = order.CustomerID;
            payment.CustomerLocationID = order.CustomerLocationID;
            payment.PaymentMethodID    = order.PaymentMethodID;
            payment.PMInstanceID       = order.PMInstanceID;
            payment.CuryOrigDocAmt     = 0m;
            payment.ExtRefNbr          = order.CustomerRefNbr ?? order.CustomerOrderNbr;
            payment.DocDesc            = order.OrderNbr;
            payment.AdjDate            = (hasAMZFee == true && root.paymentReleaseDate != null) ? DateTime.Parse(root.paymentReleaseDate) : order.OrderDate;
            payment.CashAccountID      = order.CashAccountID;

            payment = pymtEntry.Document.Update(payment);

            SOAdjust adj = new SOAdjust()
            {
                AdjdOrderType = order.OrderType.Trim(),
                AdjdOrderNbr  = order.OrderNbr.Trim()
            };

            pymtEntry.SOAdjustments.Insert(adj);

            for (int i = 0; i < root.item.Count; i++)
            {
                for (int j = 0; j < root.item[i].fee.Count; j++)
                {
                    if (root.item[i].fee[j].type == AmazonFeeType.Amz_WarehouseFee)
                    {
                        ARPaymentChargeTran chargeTran = pymtEntry.PaymentCharges.Cache.CreateInstance() as ARPaymentChargeTran;

                        chargeTran.EntryTypeID = LUMAmzInterfaceAPIMaint.GetAcumaticaPymtEntryType(root.item[i].fee[j].name);
                        chargeTran.CuryTranAmt = (decimal)Math.Abs(root.item[i].fee[j].amount);

                        pymtEntry.PaymentCharges.Insert(chargeTran);
                    }
                    else
                    {
                        pymtEntry.SOAdjustments.Current.CuryAdjgAmt += (decimal)root.item[i].fee[j].amount;
                    }
                }
            }

            if (payment.CuryOrigDocAmt == 0m)
            {
                pymtEntry.SOAdjustments.UpdateCurrent();

                payment.CuryOrigDocAmt = payment.CurySOApplAmt;

                pymtEntry.Document.Update(payment);
            }

            pymtEntry.releaseFromHold.Press();
            pymtEntry.Actions.PressSave();
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
        public static string GetAcuPymtMethod(PXGraph graph, int? customerID, string paymDescr)
        {
            CustomerPaymentMethodC paymMethod = SelectFrom<CustomerPaymentMethodC>.Where<CustomerPaymentMethodC.descr.Contains<@P.AsString>
                                                                                         .And<CustomerPaymentMethodC.bAccountID.IsEqual<@P.AsInt>>>.View.Select(graph, paymDescr, customerID);
            return paymMethod?.PaymentMethodID;
        }

        /// <summary>
        /// Delete specify table record by Processed is not true.
        /// </summary>
        protected static void DeleteWrkTableRecs()
        {
            PXDatabase.Delete<LUM3DCartProcessOrder>(new PXDataFieldRestrict<LUM3DCartProcessOrder.processed>(PXDbType.Bit, false));
        }

        /// <summary>
        /// Update 3D Cart specify order status to cancelled.
        /// </summary>
        /// <param name="curSetup"></param>
        /// <param name="orderID"></param>
        //public static void Update3DCartOrderStatus(LUM3DCartSetup curSetup, int orderID) => GetResponse(curSetup, string.Format("3dCartWebAPI/v2/Orders/{0}", orderID), true);
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