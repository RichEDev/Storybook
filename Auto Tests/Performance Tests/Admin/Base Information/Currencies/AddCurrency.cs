
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddCurrency : WebTest
    {

        public AddCurrency()
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

            // Needs to be more thorough?? Add when static, monthly, date ranged??

            // Set the exchange rate to static before the test is ran
            WebTestRequest request2 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincurrencies.aspx/saveCurrencyType"));            
            request2.Method = "POST";
            StringHttpBody request2Body = new StringHttpBody();
            request2Body.ContentType = "application/json; charset=utf-8";
            request2Body.InsertByteOrderMark = false;
            request2Body.BodyString = "{\"currencyType\":2}";
            request2.Body = request2Body;
            yield return request2;
            request2 = null;

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 10;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincurrencies.aspx"));
            request5.ThinkTime = 3;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request6.QueryStringParameters.Add("currencyType", "2", false, false);
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "<option value";
            extractionRule3.EndsWith = "Bahamian Dollar";
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

            string currencyID = AutoTools.GetID(this.Context["NewID"].ToString(), "=", "\"", 2);

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx/getCurrencyDetails"));
            request8.ThinkTime = 6;
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"globalCurrencyID\":\"" + currencyID + "\"}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/admincurrencies.aspx");
            request9.QueryStringParameters.Add("currencyType", "2", false, false);
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbCurrency", currencyID);
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbPositiveFormat", "1");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbNegativeFormat", "1");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "29");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "14");
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/admincurrencies.aspx");
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "Bahamian Dollar<\\/td><td class=\"row([1-2])\">BSD<\\/td><td class=\"row([1-2])\">44";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = true;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
