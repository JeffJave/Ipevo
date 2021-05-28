using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using APILibrary.Model;
using Newtonsoft.Json;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;
using PX.Objects.TX;

namespace PX.Objects.AR
{
    public class CustomerMaint_Extension : PXGraphExtension<CustomerMaint>
    {
        public const string taxProviderID = "TAXCLOUD";
        public const string taxCloudID    = "apiLoginID";
        public const string taxCloudKey   = "apiKey";
        public const string taxCloudCust  = "customerID";

        #region Event Handlers
        protected void _(Events.RowSelected<Customer> e, PXRowSelected baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);

            getExemptCert.SetEnabled((Base.BaseLocations.Current?.GetExtension<PX.Objects.CR.Standalone.LocationExt>().UsrTaxExemptCust ?? false) == true);
        }
        #endregion

        #region Action
        public PXAction<Location> getExemptCert;
		[PXUIField(DisplayName = "Get Exempt Certificate", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXButton()]
		public virtual IEnumerable GetExemptCert(PXAdapter adapter)
		{
			var jsonResult = CallApiAsync();
            var covtResult = JsonConvert.DeserializeObject<Root>(jsonResult.Result);

            if (covtResult.ExemptCertificates.Count <= 0 || string.IsNullOrEmpty(covtResult.ExemptCertificates[0].CertificateID) )
            {
                Base.BaseLocations.Ask(Base.BaseLocations.Current, "Tax Cloud Exemption Certificate", "The Customer Certificate ID Doesn't Exist.", MessageButtons.OK);
            }
            else
            {
                Base.BaseLocations.SetValueExt<Location.cAvalaraExemptionNumber>(Base.BaseLocations.Current, covtResult.ExemptCertificates[0].CertificateID);
                Base.BaseLocations.UpdateCurrent();
                Base.Save.Press();
            }

            return adapter.Get();
		}
        #endregion

        #region Method
        public async System.Threading.Tasks.Task<string> CallApiAsync()
        {
            var data = new Dictionary<string, object>();

            foreach (TaxPluginDetail pluginDetail in SelectFrom<TaxPluginDetail>.Where<TaxPluginDetail.taxPluginID.IsEqual<@P.AsString>
                                                                                       .And<TaxPluginDetail.settingID.Contains<@P.AsString>>>.View.Select(Base, taxProviderID, DR.DRScheduleDocumentType.Bill))
            {
                if (pluginDetail.SettingID.Contains("ID"))
                {
                    data.Add(taxCloudID, pluginDetail.Value);
                }
                else if (pluginDetail.SettingID.Contains("KEY"))
                {
                    data.Add(taxCloudKey, pluginDetail.Value);
                } 
            }

            data.Add(taxCloudCust, Base.CurrentCustomer.Current.AcctCD.Trim());

            HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://api.taxcloud.net/1.0/TaxCloud/GetExemptCertificates");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content).ConfigureAwait(false);

            return response.IsSuccessStatusCode ? response.Content.ReadAsStringAsync().Result : string.Empty;
        }
        #endregion
    }
}