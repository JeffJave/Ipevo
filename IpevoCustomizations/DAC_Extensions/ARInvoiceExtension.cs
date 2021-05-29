using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AR
{
    public class ARInvoiceExtension : PXCacheExtension<ARInvoice>
    {
        #region UsrSwitchFromNumToEN
        [PXString]
        [PXUIField(DisplayName = "UsrSwitchFromNumToEN", Visible = false, IsReadOnly = true)]
        public virtual string UsrSwitchFromNumToEN
        {
            [PXDependsOnFields(typeof(ARInvoice.curyLineTotal))]
            get { return Number2English(Convert.ToDecimal(Base.CuryLineTotal)); }
            set { }
        }
        public abstract class usrSwitchFromNumToEN : PX.Data.BQL.BqlString.Field<usrSwitchFromNumToEN> { }
        #endregion
        public string Number2English(decimal num)
        {
            string nu = num.ToString("#00.00");
            string dollars = "";
            string cents = "";
            string tp = "";
            string[] temp;
            string[] tx = { "", "THOUSAND,", "MILLION,", "BILLION,", "TRILLION," };
            
            if (decimal.Parse(nu) == 0) return "ZERO DOLLARS";
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
                dollars = "ZERO DOLLARS AND" + cents + " ONLY";
            }
            else
            {
                if (cents == "")
                {
                    dollars = tp + "DOLLARS ONLY";
                }
                else
                {
                    dollars = tp + "DOLLARS AND" + cents + " ONLY";
                }
            }
            return dollars;
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
    }
}
