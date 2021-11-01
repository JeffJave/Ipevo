using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.TX;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;
using eGUICustomizations.Graph_Release;
using static eGUICustomizations.Descriptor.TWNStringList;
using PX.Objects.CR;

namespace PX.Objects.AP
{
    public class APReleaseProcess_Extension : PXGraphExtension<APReleaseProcess>
    {
        #region Selects
        public SelectFrom<TWNGUITrans>
                          .Where<TWNGUITrans.orderNbr.IsEqual<APInvoice.refNbr.FromCurrent>>.View ViewGUITrans;

        public SelectFrom<TWNWHTTran>.Where<TWNWHTTran.docType.IsEqual<APRegister.docType.FromCurrent>
                                            .And<TWNWHTTran.refNbr.IsEqual<APRegister.refNbr.FromCurrent>>>.View WHTTranView;
        #endregion

        #region Delegate Funcation
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            baseMethod();

            APRegister doc = Base.APDocument.Current;

            if (TWNGUIValidation.ActivateTWGUI(Base) == true &&
                doc != null && 
                doc.Released == true &&
                doc.DocType.IsIn(APDocType.Invoice, APDocType.DebitAdj) )
            { 
                foreach (TWNManualGUIAPBill row in SelectFrom<TWNManualGUIAPBill>.Where<TWNManualGUIAPBill.docType.IsEqual<@P.AsString>
                                                                                        .And<TWNManualGUIAPBill.refNbr.IsEqual<@P.AsString>>>.View.Select(Base, doc.DocType, doc.RefNbr))
                {
                    // Avoid standard logic calling this method twice and inserting duplicate records into TWNGUITrans.
                    if (CountExistedRec(Base, row.GUINbr, row.VATInCode, doc.RefNbr) >= 1) { return; }

                    if (Tax.PK.Find(Base, row.TaxID).GetExtension<TaxExt>().UsrTWNGUI != true) { continue; }

                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNReleaseProcess rp = PXGraph.CreateInstance<TWNReleaseProcess>();

                        TWNGUITrans tWNGUITrans = rp.InitAndCheckOnAP(row.GUINbr, row.VATInCode);

                        Vendor vendor = Vendor.PK.Find(Base, row.VendorID);

                        rp.CreateGUITrans(new STWNGUITran()
                        {    
                            VATCode       = row.VATInCode,
                            GUINbr        = row.GUINbr,
                            GUIStatus     = TWNGUIStatus.Used,
                            BranchID      = Base.APTran_TranType_RefNbr.Current.BranchID,
                            GUIDirection  = TWNGUIDirection.Receipt,
                            GUIDate       = row.GUIDate,
                            GUIDecPeriod  = doc.DocDate,
                            GUITitle      = vendor?.AcctName,
                            TaxZoneID     = row.TaxZoneID,
                            TaxCategoryID = row.TaxCategoryID,
                            TaxID         = row.TaxID,
                            TaxNbr        = row.TaxNbr,
                            OurTaxNbr     = row.OurTaxNbr,
                            NetAmount     = row.NetAmt,
                            TaxAmount     = row.TaxAmt,
                            AcctCD        = vendor?.AcctCD,
                            AcctName      = vendor?.AcctName,
                            DeductionCode = row.Deduction,
                            Remark        = row.Remark,
                            BatchNbr      = doc.BatchNbr,
                            OrderNbr      = doc.RefNbr
                        });

                        if (tWNGUITrans != null)
                        {
                            if (tWNGUITrans.NetAmtRemain < row.NetAmt)
                            {
                                throw new PXException(TWMessages.RemainAmt);
                            }

                            ViewGUITrans.SetValueExt<TWNGUITrans.netAmtRemain>(tWNGUITrans, (tWNGUITrans.NetAmtRemain -= row.NetAmt));
                            ViewGUITrans.SetValueExt<TWNGUITrans.taxAmtRemain>(tWNGUITrans, (tWNGUITrans.TaxAmtRemain -= row.TaxAmt));

                            tWNGUITrans = ViewGUITrans.Update(tWNGUITrans);
                        }

                        // Manually Saving as base code will not call base graph persis.
                        ViewGUITrans.Cache.Persist(PXDBOperation.Insert);
                        ViewGUITrans.Cache.Persist(PXDBOperation.Update);

                        ts.Complete(Base);
                    }
                }

                // Triggering after save events.
                ViewGUITrans.Cache.Persisted(false);

                TWNWHT tWNWHT = SelectFrom<TWNWHT>.Where<TWNWHT.docType.IsEqual<@P.AsString>.And<TWNWHT.refNbr.IsEqual<@P.AsString>>>.View.Select(Base, doc.DocType, doc.RefNbr);

                if (doc.DocType == APDocType.Invoice && tWNWHT != null)
                {
                    using (PXTransactionScope ts = new PXTransactionScope())
                    {
                        TWNWHTTran wHTTran = new TWNWHTTran() 
                        {
                            DocType = doc.DocType,
                            RefNbr  = doc.RefNbr,
                        };

                        wHTTran = WHTTranView.Cache.Insert(wHTTran) as TWNWHTTran;

                        wHTTran.BatchNbr   = doc.BatchNbr;
                        wHTTran.TranDate   = doc.DocDate;
                        wHTTran.PaymDate   = Base.APPayment_DocType_RefNbr.Current?.DocDate;
                        wHTTran.PaymRef    = Base.APPayment_DocType_RefNbr.Current?.ExtRefNbr;
                        wHTTran.PersonalID = tWNWHT.PersonalID;
                        wHTTran.PropertyID = tWNWHT.PropertyID;
                        wHTTran.TypeOfIn   = tWNWHT.TypeOfIn;
                        wHTTran.WHTFmtCode = tWNWHT.WHTFmtCode;
                        wHTTran.WHTFmtSub  = tWNWHT.WHTFmtSub;
                        wHTTran.PayeeName  = (string)PXSelectorAttribute.GetField(Base.APDocument.Cache, doc, nameof(doc.VendorID), doc.VendorID, nameof(Vendor.acctName));
                        wHTTran.PayeeAddr  = SelectFrom<Address>.InnerJoin<Vendor>.On<Vendor.bAccountID.IsEqual<Address.bAccountID>
                                                                                     .And<Vendor.defAddressID.IsEqual<Address.addressID>>>
                                                               .Where<Vendor.bAccountID.IsEqual<@P.AsInt>>.View.ReadOnly.SelectSingleBound(Base, null, doc.VendorID).TopFirst?.AddressLine1;
                        wHTTran.WHTTaxPct  = tWNWHT.WHTTaxPct;
                        wHTTran.WHTAmt     = 0;//row.CuryLineAmt * -1;
                        wHTTran.NetAmt     = doc.CuryOrigDocAmt;
                        wHTTran.SecNHIPct  = tWNWHT.SecNHIPct;
                        wHTTran.SecNHICode = tWNWHT.SecNHICode;
                        wHTTran.SecNHIAmt  = 0;//row.CuryLineAmt * -1;

                        WHTTranView.Cache.Update(wHTTran);

                        ts.Complete(Base);
                    }
                }

                baseMethod();
            }
        }
        #endregion

        #region Static Methods
        public static int CountExistedRec(PXGraph graph, string gUINbr, string gUIFmtCode, string refNbr)
        {
            return SelectFrom<TWNGUITrans>.Where<TWNGUITrans.gUINbr.IsEqual<@P.AsString>
                                                 .And<TWNGUITrans.gUIFormatcode.IsEqual<@P.AsString>>
                                                      .And<TWNGUITrans.orderNbr.IsEqual<@P.AsString>>>
                                          .View.ReadOnly.Select(graph, gUINbr, gUIFmtCode, refNbr).Count;
        }

        public static TaxTran SelectTaxTran(PXGraph graph, string tranType, string refNbr, string module)
        {
            return SelectFrom<TaxTran>.Where<TaxTran.tranType.IsEqual<@P.AsString>
                                             .And<TaxTran.refNbr.IsEqual<@P.AsString>>
                                                  .And<TaxTran.module.IsEqual<@P.AsString>>>
                                      .View.ReadOnly.Select(graph, tranType, refNbr, module);
        }
        #endregion
    }
}