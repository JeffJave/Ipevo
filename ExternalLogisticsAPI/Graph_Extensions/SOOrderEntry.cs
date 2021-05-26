using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Licensing;
using PX.Objects.AR;
using PX.Objects.IN;
using PX.Objects.SO;

namespace ExternalLogisticsAPI.Graph_Extensions
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {
        public PXAction<SOOrder> createDCLShipment;

        [PXButton]
        [PXUIField(DisplayName = "Create Shipment in DCL", Enabled = true, MapEnableRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable CreateDCLShipment(PXAdapter adapter)
        {
            List<SOOrder> list = adapter.Get<SOOrder>().ToList();

            var adapterSlice = (adapter.MassProcess, adapter.QuickProcessFlow, adapter.AllowRedirect);
            for (int i = 0; i < list.Count; i++)
            {
                var order = list[i];
                if (adapterSlice.MassProcess)
                    PXProcessing<SOOrder>.SetCurrentItem(order);

                DCLShipment model = new DCLShipment();
                CombineDLCShipmentEntity(model, order);
                PXNoteAttribute.SetNote(Base.Document.Cache, order, APIHelper.GetJsonString(model));
                order.GetExtension<SOOrderExt>().UsrDCLShipmentCreated = true;
                Base.Document.Cache.Update(order);
            }

            Base.Persist();
            return adapter.Get();
        }

        #region Method

        public void CombineDLCShipmentEntity(DCLShipment model, SOOrder soOrder)
        {
            if (model == null)
                return;

            var soLine = SelectFrom<SOLine>.Where<SOLine.orderNbr.IsEqual<P.AsString>
                .And<SOLine.orderType.IsEqual<P.AsString>>>
                .View
                .Select(Base, soOrder.OrderNbr, soOrder.OrderType)
                .RowCast<SOLine>()
                .ToList();

            var shippingContact = SOShippingContact.PK.Find(Base, soOrder.CustomerID) ?? new SOShippingContact();
            var shippingAddress = SOShippingAddress.PK.Find(Base, soOrder.ShipAddressID) ?? new SOShippingAddress();
            var billingContact = SOBillingContact.PK.Find(Base, soOrder.ContactID) ?? new SOBillingContact();
            var billingAddress = SOBillingAddress.PK.Find(Base, soOrder.BillAddressID) ?? new SOBillingAddress();
            model.allow_partial = true;
            model.location = "LA";

            List<APILibrary.Model.Line> dclLines = soLine.Any() ? new List<APILibrary.Model.Line>() : null;
            int count = 1;
            foreach (var item in soLine)
            {
                var inventory = InventoryItem.PK.Find(Base, item.InventoryID);
                APILibrary.Model.Line dclItem = new APILibrary.Model.Line()
                {
                    line_number = count++,
                    item_number = inventory.InventoryCD == "5-884-4-01-02" ? "5-884-4-01-00" : inventory.InventoryCD,
                    description = inventory.Descr,
                    quantity = (int)item?.OrderQty,
                    price = (double?)item.UnitPrice
                };
                dclLines.Add(dclItem);
            }

            model.orders = new List<Order>()
            {
                new Order()
                {
                    order_number = soOrder.OrderNbr,
                    account_number = soOrder.ShipVia == "UPSGROUND" ?"19311" : "19310",
                    ordered_date = soOrder.OrderDate?.ToString("yyyy-MM-dd"),
                    po_number = "",
                    customer_number = GetCurrAcctCD(soOrder.CustomerID),
                    acknowledgement_email = "logistic@ipevo.com",
                    freight_account = "00500",
                    shipping_carrier = soOrder.ShipVia?.Substring(0,3),
                    shipping_service = soOrder.ShipVia?.Substring(3),
                    system_id = "P",
                    shipping_address =  new ShippingAddress()
                    {
                        company = shippingContact.FullName?.Length > 40 ? shippingContact.FullName.Substring(0,40) : shippingContact.FullName,
                        attention = shippingContact.Attention?.Length > 40 ? shippingContact.Attention.Substring(0,40) : shippingContact.FullName,
                        address1 = shippingAddress.AddressLine1,
                        address2 = shippingAddress.AddressLine2,
                        phone = shippingContact.Phone1,
                        email = shippingContact.Email,
                        state_province = shippingAddress.State,
                        postal_code = shippingAddress.PostalCode,
                        country_code = "US"
                    },
                    billing_address = new BillingAddress()
                    {
                        company = billingContact.FullName?.Length>40 ?  billingContact.FullName?.Substring(0,40) : billingContact.FullName,
                        attention = billingContact.Attention?.Length>40 ?  billingContact.Attention?.Substring(0,40) : billingContact.Attention,
                        address1 = billingAddress.AddressLine1,
                        address2 = billingAddress.AddressLine2,
                        phone = billingContact.Phone1,
                        email = billingContact.Email,
                        state_province = billingAddress.State,
                        postal_code = billingAddress.PostalCode,
                        country_code = "US"
                    },
                    international_code = 0,
                    order_subtotal = (double?)soOrder.OrderTotal,
                    shipping_handling = (double?)soOrder.FreightAmt,
                    sales_tax = (double?)soOrder.TaxTotal,
                    international_handling = 0,
                    total_due = (double?)soOrder.LineTotal,
                    amount_paid = 0,
                    net_due_currency = 0,
                    balance_due_us = 0,
                    international_declared_value = 0,
                    insurance = 0,
                    payment_type = string.Empty,
                    terms = string.Empty,
                    fob = string.Empty,
                    custom_field1 = soOrder.CustomerOrderNbr,
                    packing_list_type = 0,
                    packing_list_comments = string.Empty,
                    shipping_instructions = $@"1.Please note that use the plastic wrapper to secure all cartons onto the pallets for this order.\r\n
2.Please contact {shippingContact.FullName}@{shippingContact.Phone1} for delivery.",
                    lines = dclLines
                }
            };
        }

        /// <summary> Get BAAccount CD </summary>
        public string GetCurrAcctCD(int? baccountId)
        {
            return SelectFrom<PX.Objects.CR.BAccount>.Where<PX.Objects.CR.BAccount.bAccountID.IsEqual<P.AsInt>>
                .View.SelectSingleBound(Base, null, baccountId)
                .RowCast<PX.Objects.CR.BAccount>().FirstOrDefault()?.AcctCD;
        }

        #endregion
    }
}
