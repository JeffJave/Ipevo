using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.SO
{
    public class SOShipmentEntry_Extension : PXGraphExtension<SOShipmentEntry>
    {
        private const string _QTYINCARTON = "QTYPERCTN";
        public class QtyPerCartonAttr : BqlString.Constant<QtyPerCartonAttr>
        {
            public QtyPerCartonAttr() : base("QTYPERCTN") { }
        }

        public class QtyCartonPerPalletByAirAttr : BqlString.Constant<QtyCartonPerPalletByAirAttr>
        {
            public QtyCartonPerPalletByAirAttr() : base("CTNPPLTAIR") { }
        }

        public class QtyCartonPerPalletBySeaAttr : BqlString.Constant<QtyCartonPerPalletBySeaAttr>
        {
            public QtyCartonPerPalletBySeaAttr() : base("CTNPPLTSEA") { }
        }

        public delegate void PersistDelegate();

        public override void Initialize()
        {
            base.Initialize();
            PackingList.SetVisible(false);

            var curCoutry = (PXSelect<Branch>.Select(Base, PX.Data.Update.PXInstanceHelper.CurrentCompany)).TopFirst;
            if (curCoutry?.CountryID == "TW" || curCoutry?.BaseCuryID == "TWD")
            {
                PackingList.SetVisible(true);
                Base.report.AddMenuAction(PackingList);
            }
        }

        # region Override Persist Event
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            var needUpdatePackedQty = Base.Packages.Cache.Dirty.RowCast<SOPackageDetailEx>().Count() > 0;

            if (needUpdatePackedQty)
            {
                var _packagesFromDB = SelectFrom<SOPackageDetail>.Where<SOPackageDetail.shipmentNbr.IsEqual<SOPackageDetail.shipmentNbr.FromCurrent>>.View.Select(Base);

                // Except Delete row
                var _shipLines = Base.Transactions.Cache.Cached.RowCast<SOShipLine>();
                _shipLines = _shipLines.Except((IEnumerable<SOShipLine>)Base.Transactions.Cache.Deleted);

                // Recalculate PackedQty
                foreach (var item in _shipLines)
                {
                    item.PackedQty = _packagesFromDB.Where(x => ((SOPackageDetailEx)x).GetExtension<SOPackageDetailExt>().UsrShipmentSplitLineNbr == item.LineNbr).Sum(x => ((SOPackageDetailEx)x).Qty);
                    Base.Caches[typeof(SOShipLine)].Update(item);
                }
            }

            baseMethod();
        }
        #endregion

        #region Action
        public PXAction<SOShipment> PackingList;
        [PXButton]
        [PXUIField(DisplayName = "Print Packing List", Enabled = true, MapEnableRights = PXCacheRights.Select)]
        protected virtual IEnumerable packingList(PXAdapter adapter)
        {
            if (Base.Document.Current != null)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["ShipmentNbr"] = Base.Document.Current.ShipmentNbr;
                parameters["PackageCountNo"] = Convert.ToString(Base.Document.Current.PackageCount);
                parameters["PackageCountEn"] = Number2English(Convert.ToDecimal(Base.Document.Current.PackageCount));
                throw new PXReportRequiredException(parameters, "LM642005", "Report LM642005");
            }
            return adapter.Get();
        }
        #endregion

        #region Auto Packaging Button Click Event
        public PXAction<SOShipLine> autoPackaging;
        [PXButton]
        [PXUIField(DisplayName = "Auto Packaging", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable AutoPackaging(PXAdapter adapter)
        {
            decimal parseResult = 0;
            var _maxCartno = GetMaxPalletNbr();

            SOShipLine _line = (SOShipLine)Base.Transactions.Cache.Current;
            var _CartonsPerPallet = decimal.TryParse(_line.GetExtension<SOShipLineExt>().UsrCartonsPerPallet, out parseResult) ? parseResult : 0;
            var _QtyPerCarton = decimal.TryParse(SelectFrom<CSAnswers>.
                                LeftJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<QtyPerCartonAttr>>>.
                                Where<InventoryItem.inventoryID.IsEqual<SOShipLine.inventoryID.FromCurrent>>.View.Select(Base).TopFirst?.Value, out parseResult) ? parseResult : 0;

            if (_line.ShippedQty < (_CartonsPerPallet * _QtyPerCarton))
                throw new PXException("Shipped Qty should greater or equal to Cartons Per Pallet.");

            if (_line.GetExtension<SOShipLineExt>().UsrRemainingQty < (_CartonsPerPallet * _QtyPerCarton))
                throw new PXException("Remaining Qty should greater or equal to Cartons Per Pallet.");

            var NumberOfPackages = (int)Math.Floor((decimal)(_line.GetExtension<SOShipLineExt>().UsrRemainingQty / (_CartonsPerPallet * _QtyPerCarton)));

            PXLongOperation.StartOperation(Base, () =>
            {
                for (int i = 0; i < NumberOfPackages; i++)
                {
                    Base.Packages.Insert((SOPackageDetailEx)Base.Packages.Cache.CreateInstance());
                    SOPackageDetailEx _package = Base.Packages.Cache.Dirty.RowCast<SOPackageDetailEx>().ElementAt(i);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.shipmentNbr>(_package, _line.ShipmentNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetailEx.customRefNbr2>(_package, (++_maxCartno).ToString().PadLeft(3, '0'));
                    Base.Packages.Cache.SetValueExt<SOPackageDetailExt.usrShipmentSplitLineNbr>(_package, _line.LineNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetailExt.usrCartonQty>(_package, (int)_CartonsPerPallet);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.qty>(_package, _CartonsPerPallet * _QtyPerCarton);
                }
                Base.Save.Press();
            });

            return adapter.Get();
        }
        #endregion

        #region Maunal Packaging Button Click Event
        public PXAction<SOShipLine> manualPackaging;
        [PXButton]
        [PXUIField(DisplayName = "Manual Packaging", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable ManualPackaging(PXAdapter adapter)
        {
            int parseResult = 0;
            var _shipLines = Base.Transactions.Cache.Cached.RowCast<SOShipLine>();

            if (_shipLines.Where(x => x.GetExtension<SOShipLineExt>().UsrRemainingQty < x.GetExtension<SOShipLineExt>().UsrPackingQty).Count() > 0)
                throw new PXException("Packing Qty cannot exceed Remaining Qty.");
            
            PXLongOperation.StartOperation(Base, () =>
            {
                int pointer = 0;
                var _maxCartonNbr = GetMaxPalletNbr() + 1;
                foreach (var _shipline in _shipLines.Where(x => x.GetExtension<SOShipLineExt>().UsrPackingQty > 0))
                {
                    var _CartonsPerPallet = Int32.TryParse(((SOShipLine)Base.Transactions.Cache.Current).GetExtension<SOShipLineExt>().UsrCartonsPerPallet, out parseResult) ? parseResult : 0;
                    var _QtyPerCarton = Int32.TryParse(SelectFrom<CSAnswers>.
                                        LeftJoin<InventoryItem>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>.And<CSAnswers.attributeID.IsEqual<QtyPerCartonAttr>>>.
                                        Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>.View.Select(Base, _shipline.InventoryID).TopFirst?.Value, out parseResult) ? parseResult : 0;
                    Base.Packages.Insert((SOPackageDetailEx)Base.Packages.Cache.CreateInstance());
                    SOPackageDetailEx _package = Base.Packages.Cache.Dirty.RowCast<SOPackageDetailEx>().ElementAt(pointer++);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.shipmentNbr>(_package, _shipline.ShipmentNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetailEx.customRefNbr2>(_package, _maxCartonNbr.ToString().PadLeft(3, '0'));
                    Base.Packages.Cache.SetValueExt<SOPackageDetailExt.usrShipmentSplitLineNbr>(_package, _shipline.LineNbr);
                    Base.Packages.Cache.SetValueExt<SOPackageDetail.qty>(_package, _shipline.GetExtension<SOShipLineExt>().UsrPackingQty);
                    Base.Packages.Cache.SetValueExt<SOPackageDetailExt.usrCartonQty>(_package, (int)Math.Ceiling((decimal)(_shipline.GetExtension<SOShipLineExt>().UsrPackingQty / _QtyPerCarton)) > _CartonsPerPallet ? _CartonsPerPallet : (int)Math.Ceiling((decimal)(_shipline.GetExtension<SOShipLineExt>().UsrPackingQty / _QtyPerCarton)));
                }
                Base.Save.Press();
            });

            return adapter.Get();
        }
        #endregion

        #region SOPackageDetailEx_BoxID Defaulting
        protected void _(Events.FieldDefaulting<SOPackageDetailEx.boxID> e)
        {
            var row = (SOPackageDetailEx)e.Row;
            var boxsInfo = GetBoxsInfo(row.InventoryID);
            e.NewValue = string.IsNullOrEmpty(boxsInfo.stockItemBox) ? boxsInfo.sBoxID : boxsInfo.stockItemBox;
        }
        #endregion

        #region SOPackageDetailExt_usrShipmentSplitLineNbr Updated Event
        protected virtual void _(Events.FieldUpdated<SOPackageDetailExt.usrShipmentSplitLineNbr> e)
        {
            if (e.NewValue == null)
                return;
            var _shipLine = Base.Transactions.Cache.Cached.RowCast<SOShipLine>().Where(x => x.LineNbr == (int?)e.NewValue).SingleOrDefault();
            var _shipLineSplit = new PXGraph().Select<SOShipLineSplit>().Where(x => x.ShipmentNbr == _shipLine.ShipmentNbr && x.LineNbr == _shipLine.LineNbr);
            var boxsInfo = GetBoxsInfo(_shipLine.InventoryID);
            e.Cache.SetValueExt<SOPackageDetailEx.boxID>(e.Row, string.IsNullOrEmpty(boxsInfo.stockItemBox) ? boxsInfo.sBoxID : boxsInfo.stockItemBox);
            e.Cache.SetValueExt<SOPackageDetail.inventoryID>(e.Row, _shipLine.InventoryID);
        }
        #endregion

        #region Verify Packing Qty
        protected void _(Events.FieldVerifying<SOShipLineExt.usrPackingQty> e)
        {
            if ((decimal?)e.NewValue < 0)
                throw new PXSetPropertyException("Packing Qty must br greater than 0");
            if ((decimal?)e.NewValue > ((SOShipLine)e.Row).GetExtension<SOShipLineExt>().UsrRemainingQty)
                throw new PXSetPropertyException("Packing Qty cannot exceed Remaining Qty");
        }
        #endregion

        #region SOPackageDetailExt UsrMeasurement Updated Event
        protected virtual void _(Events.FieldUpdated<SOPackageDetailExt.usrLength> e)
        {
            if ((SOPackageDetailEx)e.Row == null) return;
            SOPackageDetailExt _line = ((SOPackageDetail)Base.Packages.Cache.Current).GetExtension<SOPackageDetailExt>();
            e.Cache.SetValueExt<SOPackageDetailExt.usrMeasurement>(e.Row, _line.UsrLength * _line.UsrWidth * _line.UsrHeight / 1000);
        }

        protected virtual void _(Events.FieldUpdated<SOPackageDetailExt.usrWidth> e)
        {
            if ((SOPackageDetailEx)e.Row == null) return;
            SOPackageDetailExt _line = ((SOPackageDetail)Base.Packages.Cache.Current).GetExtension<SOPackageDetailExt>();
            e.Cache.SetValueExt<SOPackageDetailExt.usrMeasurement>(e.Row, _line.UsrLength * _line.UsrWidth * _line.UsrHeight / 1000);
        }

        protected virtual void _(Events.FieldUpdated<SOPackageDetailExt.usrHeight> e)
        {
            if ((SOPackageDetailEx)e.Row == null) return;
            SOPackageDetailExt _line = ((SOPackageDetail)Base.Packages.Cache.Current).GetExtension<SOPackageDetailExt>();
            e.Cache.SetValueExt<SOPackageDetailExt.usrMeasurement>(e.Row, _line.UsrLength * _line.UsrWidth * _line.UsrHeight / 1000);
        }
        #endregion
        
        #region Other Methods

        #region Get Max Pallet Nbr
        public int GetMaxPalletNbr()
        {
            int result;
            var _PackageDetail_MaxPalletNbr = SelectFrom<SOPackageDetail>.Where<SOPackageDetail.shipmentNbr.IsEqual<@P.AsString>>.OrderBy<Desc<SOPackageDetail.createdDateTime>>.View.Select(Base, (Base.Caches<SOShipment>().Current as SOShipment)?.ShipmentNbr).TopFirst?.CustomRefNbr2;
            try
            {
                return Int32.TryParse(_PackageDetail_MaxPalletNbr, out result) ? result : 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region Get StockItem Info
        public PXResult<InventoryItem, CSAnswers> GetStockInfo(int InventoryID, string AttributeID)
        {
            return (PXResult<InventoryItem, CSAnswers>)SelectFrom<InventoryItem>
                                                      .LeftJoin<CSAnswers>.On<InventoryItem.noteID.IsEqual<CSAnswers.refNoteID>
                                                           .And<CSAnswers.attributeID.IsEqual<@P.AsString>>>
                                                      .Where<InventoryItem.inventoryID.IsEqual<@P.AsInt>>
                                                      .View.Select(Base, AttributeID, InventoryID);
        }
        #endregion

        #region Get sBoxID and stockItemBox
        public (string sBoxID, string stockItemBox) GetBoxsInfo(int? InventoryID)
        {
            var _sboxID = new PXGraph().Select<CSBox>().FirstOrDefault()?.BoxID;
            var _stockItemBoxID = new PXGraph().Select<INItemBoxEx>().Where(x => x.InventoryID == InventoryID).FirstOrDefault()?.BoxID;
            return (_sboxID, _stockItemBoxID);
        }
        #endregion

        #region Number2English mothod
        public string Number2English(decimal num)
        {
            string nu = num.ToString("#00.00");
            string dollars = "";
            string cents = "";
            string tp = "";
            string[] temp;
            string[] tx = { "", "THOUSAND", "MILLION", "BILLION", "TRILLION" };

            if (decimal.Parse(nu) == 0) return "ZERO";
            else if (decimal.Parse(nu) <= 0) return "ERROR!! ";
            else
            { //處理小數點(通常是兩位)
                temp = nu.Split('.');
                string strx = temp[1].ToString();

                string cent = GetEnglish(strx);
                if (!cent.Equals("")) cents = cent + "CENTS";
            }

            //處理整數部分
            //先將資料格式化，只取出整數
            decimal x = Math.Truncate(decimal.Parse(nu));
            //格式化整數部分
            temp = x.ToString("#,0").Split(',');
            //利用整數,號檢查千、萬、百萬....
            int j = temp.Length - 1;

            for (int i = 0; i < temp.Length; i++)
            {
                tp = tp + GetEnglish(temp[i]);
                if (tp != "")
                {
                    tp = tp + tx[j] + " ";
                }
                else
                {
                    tp = tp + " ";
                }

                j = j - 1;
            }
            if (x == 0 && cents != "") // 如果整數部位= 0 ，且有小數
            {
                dollars = "ZERO AND" + cents + " ONLY.";
            }
            else
            {
                if (cents == "")
                {
                    dollars = tp + "DOLLARS ONLY.";
                }
                else
                {
                    dollars = tp + "DOLLARS AND" + cents + " ONLY.";
                }
            }
            return dollars.Replace("DOLLARS", "").Replace("ONLY.", "").Replace("  ", " ");
        }
        private string GetEnglish(string nu)
        {
            string x = "";
            string str1;
            string str2;
            string[] tr = { "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
            string[] ty = { "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

            //處理百位數
            str1 = tr[int.Parse(nu) / 100] + " HUNDRED"; //EX  當315除以100 用於int 會取出3 ..對應到字串陣列，就是 Three
            if (str1.Equals(" HUNDRED")) str1 = ""; //如果結果是空值，表示沒有百分位
            //處理十位數
            int temp = int.Parse(nu) % 100;   //  當315 除100 會剩餘 15 

            if (temp < 20)
            {
                str2 = tr[temp]; //取字串陣列 
            }
            else
            {
                str2 = ty[(temp / 10)].ToString();  //十位數  10/20/30的數量確認 

                if (str2.Equals(""))
                {
                    str2 = tr[(temp % 10)];
                }
                else
                {
                    str2 = str2 + "-" + tr[(temp % 10)];  //十位數組成
                }

            }
            if (str1 == "" && str2 == "")
            {
                x = "";
            }
            else
            {
                x = str1 + " " + str2 + " ";
            }

            return x;
        }
        #endregion

        #endregion
    }
}
