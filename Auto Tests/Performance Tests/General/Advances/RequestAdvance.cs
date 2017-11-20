
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class RequestAdvance : WebTest
    {

        public RequestAdvance()
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Claimant), false)) { yield return r; }

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/information/myadvances.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/information/aeadvancereq.aspx"));
            request6.ThinkTime = 5;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/information/aeadvancereq.aspx"));
            request7.ThinkTime = 17;
            request7.Method = "POST";
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/information/myadvances.aspx");
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtname", "Auto advance " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtamount", "100");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbcurrencies", "2883");
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtreason", "Requesting advance to test advances are working");
            request7Body.FormPostParameters.Add("ctl00$contentmain$dtrequiredby", DateTime.Today.AddMonths(1).ToString("dd/MM/yyyy"));
            request7Body.FormPostParameters.Add("ctl00$contentmain$mskrequiredby_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$mskrequiredby_ClientState"].ToString());
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "19");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "23");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/process.aspx"));
            request8.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            request8.QueryStringParameters.Add("process", "1", false, false);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request8;
            request8 = null;

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminfloats.aspx"));            
            AutoTools.ValidateText("Requesting advance to test advances are working", request11);
            AutoTools.ValidateText("£100.00", request11);
            yield return request11;
            request11 = null;
        }
    }
}
