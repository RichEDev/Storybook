
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddCountry : WebTest
    {

        public AddCountry()
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

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request3.ThinkTime = 1;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx"));
            request6.ThinkTime = 3;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "option value";
            extractionRule3.EndsWith = "American Samoa";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "NewID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            string countryID = AutoTools.GetID(this.Context["NewID"].ToString(), "=", "\"", 2);

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx/getCountryCode"));
            request7.ThinkTime = 11;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"globalCountryID\":\"" + countryID + "\"}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx"));
            request8.Method = "POST";
            request8.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx");
            FormPostHttpBody request8Body = new FormPostHttpBody();
            request8Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request8Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request8Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmbcountry", countryID);
            request8Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "40");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "23");
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx");
            AutoTools.ValidateText("American Samoa", validateRequest);
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
