using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ExternalLogisticsAPI.DAC;
using ExternalLogisticsAPI.Descripter;

namespace ExternalLogisticsAPI.Graph
{
    public class LUMAmzInterfaceAPIMaint : PXGraph<LUMAmzInterfaceAPIMaint>
    {
        #region Features
        public PXSave<LUMAmazonInterfaceAPI> Save;
        public PXCancel<LUMAmazonInterfaceAPI> Cancel;
        [PXImport(typeof(LUMAmazonInterfaceAPI))]
        public PXProcessing<LUMAmazonInterfaceAPI> AMZInterfaceAPI;
        #endregion

        #region Ctor
        public LUMAmzInterfaceAPIMaint()
        {
            AMZInterfaceAPI.Cache.AllowInsert = AMZInterfaceAPI.Cache.AllowUpdate = AMZInterfaceAPI.Cache.AllowDelete = true;

            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.orderType>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.orderNbr>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.sequenceNo>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.marketplace>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.data1>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.data2>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.write2Acumatica1>(AMZInterfaceAPI.Cache, null, true);
            PXUIFieldAttribute.SetEnabled<LUMAmazonInterfaceAPI.write2Acumatica2>(AMZInterfaceAPI.Cache, null, true);

            AMZInterfaceAPI.SetProcessDelegate(delegate (List<LUMAmazonInterfaceAPI> list)
            {
                CreateSO(list);
            });
        }
        #endregion

        #region Static Methods
        public static void CreateSO(List<LUMAmazonInterfaceAPI> list)
        {
            LUMAmzInterfaceAPIMaint graph = PXGraph.CreateInstance<LUMAmzInterfaceAPIMaint>();

            graph.CreateSO(graph, list);
        }

        /// <summary>
        /// Obtain Acumatica sales order type according to parameter givene by the AMZ middleware.
        /// </summary>
        /// <param name="amzType"></param>
        /// <returns></returns>
        public static string GetAcumaticaSOType(int? amzType)
        {
            string sOType = "SO";

            switch (amzType)
            {
                case AmazonOrderType.FBA_SO:
                case AmazonOrderType.FBA_OI_ShipInfo:
                case AmazonOrderType.FBA_OI_AmzFee:
                case AmazonOrderType.FBA_RMA_CM:
                case AmazonOrderType.FBA_RMA_Exch:
                case AmazonOrderType.FBA_RMA_RA_Ealier:
                case AmazonOrderType.FBA_RMA_RA_Later:
                    sOType = "FA";
                    break;
                case AmazonOrderType.FBM_ShipInfo:
                case AmazonOrderType.FBM_AmzFee:
                case AmazonOrderType.FBM_OI_ShipInfo:
                case AmazonOrderType.FBM_OI_AmzFee:
                    sOType = "FM";
                    break;
            }

            return sOType;
        }

        /// <summary>
        /// Obtain the Acumtica payment entry type according to the parameter given by the AMZ middleware.
        /// </summary>
        /// <param name="amzType"></param>
        /// <returns></returns>
        public static string GetAcumaticaPymtEntryType(string amzType)
        {
            switch (amzType)
            {
			    case "CODChargeback":
				    return "CODCHARGE";
			    case "FBAPerOrderFulfillmentFee":
				    return "FBAPERORD";
			    case "FBAPerUnitFulfillmentFee":
				    return "FBAPERUNIT";
			    case "FBAWeightBasedFee":
				    return "FBAWEIGHT";
			    case "FixedClosingFee":
				    return "FIXEDCLOSE";
			    case "GiftwrapChargeback":
				    return "GIFTWRAP";
			    case "RefundCommission":
				    return "REFUNDCOM";
			    case "RenewedProgramFee":
				    return "PROGRAMFEE";
			    case "SalesTaxCollectionFee":
				    return "SALESTAX";
			    case "ShippingChargeback":
				    return "SHIPPING";
			    case "ShippingHB":
				    return "SHIPPINGHB";
			    case "VariableClosingFee":
				    return "VARCLOSE";
                default:
				    return "COMMISSION";
            }
        }
        #endregion

        #region Methods
        public virtual void CreateSO(LUMAmzInterfaceAPIMaint graph, List<LUMAmazonInterfaceAPI> list)
        {
            if (list.Count < 0) { return; }

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                SOOrderEntry orderEntry = PXGraph.CreateInstance<SOOrderEntry>();

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Write2Acumatica1 == true) { continue; }

                    try
                    {
                        bool noAMZFee  = list[i].OrderType.IsIn(AmazonOrderType.FBM_ShipInfo, AmazonOrderType.FBA_OI_ShipInfo, AmazonOrderType.FBM_OI_ShipInfo);
                        bool hasAMZFee = list[i].OrderType.IsIn(AmazonOrderType.FBM_AmzFee, AmazonOrderType.FBA_OI_AmzFee, AmazonOrderType.FBM_OI_AmzFee);

                        dynamic root = null;

                        if (noAMZFee == true)
                        {
                            root = JsonConvert.DeserializeObject<APILibrary.Model.Amazon_Middleware.Root2>(list[i].Data1);
                        }
                        else
                        {
                            root = JsonConvert.DeserializeObject<APILibrary.Model.Amazon_Middleware.Root>(list[i].Data1);
                        }

                        SOOrder order = orderEntry.Document.Cache.CreateInstance() as SOOrder;

                        if (hasAMZFee == false)
                        {
                            order.OrderType = GetAcumaticaSOType(list[i].OrderType);

                            order = orderEntry.Document.Insert(order);

                            order.CustomerID       = Customer.UK.Find(orderEntry, "SELLERCENTRAL").BAccountID;
                            order.OrderDate        = DateTime.Parse(root.payments_date);
                            order.RequestDate      = string.IsNullOrEmpty(root.item[0]?.shipment_date) ? order.RequestDate : DateTime.Parse(root.item[0]?.shipment_date);
                            order.CustomerOrderNbr = root.amazon_order_id;

                            orderEntry.Document.Cache.SetValue<SOOrderExt.usrAPIOrderType>(order, list[i].OrderType);

                            orderEntry.Document.Update(order);

                            ExternalAPIHelper.UpdateSOContactAddress(orderEntry, root);
                            ExternalAPIHelper.CreateOrderDetail(orderEntry, root);
                        }
                        else
                        {
                            orderEntry.Document.Current = SelectFrom<SOOrder>.Where<SOOrder.customerOrderNbr.IsEqual<@P.AsString>>.View.SelectSingleBound(orderEntry, null, root.amazon_order_id);
                            order = orderEntry.Document.Current;

                            foreach (SOLine line in orderEntry.Transactions.Select())
                            {
                                line.GetExtension<SOLineExt>().UsrCarrier     = root.item[line.SortOrder.Value - 1].carrier;
                                line.GetExtension<SOLineExt>().UsrTrackingNbr = root.item[line.SortOrder.Value - 1].tracking_no;

                                orderEntry.Transactions.Update(line);
                            }
                        }

                        string country  = root.item[0]?.country;
                        string state    = root.item[0]?.state;
                        string currency = root.item[0]?.currency;

                        if ((country == "US" && State.PK.Find(this, country, state)?.GetExtension<StateExt>().UsrIsAMZWithheldTax == true) ||
                            (country == "CA" && root.marketplaceWithheldTax != 0) )
                        {
                            order.OverrideTaxZone = true;
                            order.TaxZoneID = "AMAZONCA";
                        }

                        if (currency != order.CuryID && currency != "CDN")
                        {
                            orderEntry.Document.Cache.SetValueExt<SOOrder.curyID>(order, currency);
                        }

                        ///<remarks> 
                        ///Becuase the standard tax calculation logic write in SOOrder_RowUpdate event which means I must use the following approach.
                        order.CuryPremiumFreightAmt = (decimal)root.shipment;
                        orderEntry.Document.Update(order);
                        ///</remarks>

                        order.CuryTaxTotal   = UpdateSOTaxAmount(orderEntry, root);
                        order.CuryOrderTotal = hasAMZFee == false ? (order.CuryOrderTotal + order.CuryTaxTotal) : order.CuryOrderTotal;

                        UpdateSOUserDefineFields(orderEntry.Document.Cache, root);

                        orderEntry.Save.Press();

                        CurrencyInfo curyInfo = CurrencyInfo.PK.Find(this, order.CuryInfoID);

                        if (curyInfo.CuryRate != 1 && country == "UK")
                        {
                            orderEntry.Document.Cache.SetValueExt<SOOrder.cashAccountID>(order, SelectFrom<CashAccount>.Where<CashAccount.cashAccountCD.IsEqual<@P.AsString>
                                                                                                                              .And<CashAccount.curyID.IsEqual<@P.AsString>>>.View
                                                                                                .SelectSingleBound(this, null, (currency == "EUR") ? "EURAMAZON" : "GBPAMAZON", currency).TopFirst?.CashAccountID);
                            orderEntry.Document.Cache.MarkUpdated(order);
                            orderEntry.Save.Press();
                        }

                        if (noAMZFee == false || hasAMZFee == true)
                        {
                            ExternalAPIHelper.CreatePaymentProcess(order, root, curyInfo.CuryMultDiv == CuryMultDivType.Div ? curyInfo.RecipRate : curyInfo.CuryRate);
                        }

                        graph.AMZInterfaceAPI.Cache.SetValue<LUMAmazonInterfaceAPI.write2Acumatica1>(list[i], true);
                        graph.AMZInterfaceAPI.Update(list[i]);
                        graph.Save.Press();
                    }
                    catch (Exception ex)
                    {
                        PXProcessing.SetError<LUMAmazonInterfaceAPI>(ex.Message);
                        throw;
                    }
                }

                ts.Complete();
            }
        }

        protected virtual void UpdateSOUserDefineFields(PXCache cache, object obj)
        {
            dynamic root = obj as APILibrary.Model.Amazon_Middleware.Root;

            if (root == null)
            {
                root = obj as APILibrary.Model.Amazon_Middleware.Root2;
            }

            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "TAXRATE", root.tax_rate);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "GSTRATE", root.gst_rate);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "HSTRATE", root.hst_rate);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "PSTRATE", root.pst_rate);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "QSTRATE", root.qst_rate);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "BUYERTAXID", root.buyer_tax_registration);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "ORITAXABLE", root.seller_tax_registration);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "MKTPLACE", root.marketplace);
            cache.SetValueExt(cache.Current, PX.Objects.CS.Messages.Attribute + "PAYMENTREL", root.paymentReleaseDate);
        }

        protected virtual decimal? UpdateSOTaxAmount(SOOrderEntry orderEntry, object obj)
        {
            dynamic root = obj as APILibrary.Model.Amazon_Middleware.Root;

            if (root == null)
            {
                root = obj as APILibrary.Model.Amazon_Middleware.Root2;
            }

            decimal? totalTax = 0m;

            if (root.item[0].country == "CA")
            {
                foreach (SOTaxTran row in orderEntry.Taxes.Cache.Inserted)
                {
                    switch (row.TaxID)
                    {
                        case "AMAZONGST":
                            orderEntry.Taxes.Cache.SetValueExt<SOTaxTran.curyTaxAmt>(row, root.gst);
                            break;
                        case "AMAZONHST":
                            orderEntry.Taxes.Cache.SetValueExt<SOTaxTran.curyTaxAmt>(row, root.hst);
                            break;
                        case "AMAZONPST":
                            orderEntry.Taxes.Cache.SetValueExt<SOTaxTran.curyTaxAmt>(row, root.pst);
                            break;
                        case "AMAZONQST":
                            orderEntry.Taxes.Cache.SetValueExt<SOTaxTran.curyTaxAmt>(row, root.qst);
                            break;
                    }
                }
                totalTax = (decimal)(root.gst + root.hst + root.pst + root.qst);
            }
            else
            {
                orderEntry.Taxes.Cache.SetValueExt<SOTaxTran.curyTaxAmt>(orderEntry.Taxes.Current, root.tax);

                totalTax = (decimal)root.tax;
            }

            return totalTax;
        }
        #endregion
    }

    #region enum
    public enum AMZChargeType
    {                                       
        Shipping = 1, 
        GiftWrap = 2, 
        Discount = 3, 
        COD = 4, // Cash_On_Delivery
        Shipping_Tax_Discount = 5, // (Open Invoice無finance data, 無稅報前，使用出貨報表代替時才會用到)
        Shipping_Tax = 11, 
        Gift_Wrap_Tax = 12, 
        Item_Tax = 13,
        GST_Shipping_Tax = 21, 
        GST_Gift_Wrap_Tax = 22, 
        GST_Item_Tax = 23,
        HST_Shipping_Tax = 31,
        HST_Gift_Wrap_Tax = 32,
        HST_Item_Tax = 33,
        QST_Shipping_Tax = 41,
        QST_Gift_Wrap_Tax = 42,
        QST_Item_Tax = 43,
        PST_Shipping_Tax = 51,
        PST_Gift_Wrap_Tax = 52,
        PST_Item_Tax = 53
    }
    #endregion
}