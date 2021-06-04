using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APILibrary;
using APILibrary.Model;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Descripter;
using Newtonsoft.Json;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.Licensing;
using PX.Data.Update.ExchangeService;
using PX.Objects.AR;
using PX.Objects.Common;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;

namespace ExternalLogisticsAPI.Graph_Extensions
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {

        [PXHidden]
        public SelectFrom<LUMVendCntrlSetup>.View DCLSetup;

        public PXAction<SOOrder> createDCLShipment;
        public PXAction<SOOrder> lumCallDCLShipemnt;

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
                /*
                 *
                 * Send Data to DCL for Create Shipment(Implement)
                 *
                 */
                Base.Document.Cache.Update(order);
            }

            Base.Persist();
            return adapter.Get();
        }

        [PXButton]
        [PXUIField(DisplayName = "Call DCL for Shipment", Enabled = true, MapEnableRights = PXCacheRights.Select, Visible = false)]
        protected virtual IEnumerable LumCallDCLShipemnt(PXAdapter adapter, [PXDate] DateTime? shipDate, [PXInt] int? siteID, [SOOperation.List] string operation)
        {
            try
            {
                using (PXTransactionScope sc = new PXTransactionScope())
                {
                    Base.CreateShipmentIssue(adapter, shipDate, siteID);
                    var processResult = PXProcessing<SOOrder>.GetItemMessage();
                    if (processResult.ErrorLevel != PXErrorLevel.RowInfo)
                        return adapter.Get();
                    var _order = adapter.Get<SOOrder>().ToList()[0];

                    // Get DCL SO. Data(理論上資料一定存在)
                    var dclOrders = JsonConvert.DeserializeObject<OrderResponse>(
                        DCLHelper.CallDCLToGetSOByOrderNumbers(
                            this.DCLSetup.Select().RowCast<LUMVendCntrlSetup>().FirstOrDefault(), _order.CustomerRefNbr).ContentResult);

                    // Create SOShipment Graph
                    var graph = PXGraph.CreateInstance<SOShipmentEntry>();

                    // Find SOShipment
                    var _soOrderShipments =
                        FbqlSelect<SelectFromBase<SOOrderShipment, TypeArrayOf<IFbqlJoin>.Empty>.Where<BqlChainableConditionBase<TypeArrayOf<IBqlBinary>.FilledWith<And<Compare<SOOrderShipment.orderType, Equal<P.AsString>>>>>.And<BqlOperand<SOOrderShipment.orderNbr, IBqlString>.IsEqual<P.AsString>>>, SOOrderShipment>.View.Select(Base, _order.OrderType, _order.OrderNbr)
                            .RowCast<SOOrderShipment>();
                    foreach (var refItem in _soOrderShipments)
                    {
                        // Create new Adapter
                        var newAdapter = new PXAdapter(graph.Document) { Searches = new Object[] { refItem.ShipmentNbr } };
                        // Select Current Shipment
                        var _soShipment = newAdapter.Get<SOShipment>().ToList()[0];

                        try
                        {
                            if (dclOrders.orders == null)
                                _soShipment.ShipmentDesc = "Can not Mapping DCL Data";
                            else
                            {
                                // Get Carrier and TrackingNbr
                                var shippingCarrier = dclOrders.orders.FirstOrDefault().shipping_carrier;
                                var packagesInfo = dclOrders.orders.FirstOrDefault().shipments.SelectMany(x => x.packages);
                                _soShipment.ShipmentDesc = $"Carrier: {shippingCarrier}|" +
                                                           $"TrackingNbr: {string.Join("|", packagesInfo.Select(x => x.tracking_number))}";
                                if (_soShipment.ShipmentDesc.Length > 256)
                                    _soShipment.ShipmentDesc = _soShipment.ShipmentDesc.Substring(0, 255);
                            }
                        }
                        catch (Exception e)
                        {
                            _soShipment.ShipmentDesc = e.Message;
                        }

                        // Update Data
                        graph.Document.Update(_soShipment);

                        // Remove Hold
                        graph.releaseFromHold.PressButton(newAdapter);

                        // Confirm Shipment
                        graph.confirmShipmentAction.PressButton(newAdapter);
                    }
                    sc.Complete();
                }
            }
            catch (Exception e)
            {
                PXProcessing.SetError<SOOrder>(e.Message);
            }

            return adapter.Get();
        }

        #region Method

        /// <summary> Combine DCL Shipment MetaData(JSON) </summary>
        /// Shipping Carrier and Server Rule : soOrder.OrderWeight >= 150 -> UPS FREIGHT ;other -> UPS(Default or ShipVia)
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

            string shippingCarrier = string.Empty;
            string shippingService = string.Empty;

            if (soOrder.OrderWeight >= 150 || soOrder.ShipVia == "UPSFREIGHT")
            {
                shippingCarrier = "UPS FREIGHT";
                shippingService = "STANDARD";
            }
            else if(soOrder.ShipVia == "UPSGROUND")
            {
                shippingCarrier = "UPS";
                shippingService = "GROUND";
            }
            model.orders = new List<Order>()
            {
                new Order()
                {
                    order_number = soOrder.OrderNbr,
                    account_number = soOrder.ShipVia == "UPSGROUND" ?"19311" : "19310",
                    ordered_date = soOrder.OrderDate?.ToString("yyyy-MM-dd"),
                    po_number = soOrder.CustomerOrderNbr,
                    customer_number = GetCurrAcctCD(soOrder.CustomerID),
                    acknowledgement_email = "logistic@ipevo.com",
                    freight_account = "00500",
                    shipping_carrier = shippingCarrier,
                    shipping_service = shippingService,
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
                    custom_field1 = soOrder.OrderNbr,
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

    internal class LumShipmentDocView : PXView
    {
        List<object> _Records;
        internal LumShipmentDocView(PXGraph graph, BqlCommand command, List<object> records)
            : base(graph, true, command)
        {
            _Records = records;
        }
        public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
        {
            return _Records;
        }
    }
}
