
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditCurrency : WebTest
    {

        public EditCurrency()
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
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincurrencies.aspx"));
            request5.ThinkTime = 3;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript:changeArchive";
            extractionRule2.EndsWith = "Bahamian Dollar";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "NewID";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            string currencyID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request6.ThinkTime = 3;
            request6.QueryStringParameters.Add("currencyid", currencyID, false, false);
            request6.QueryStringParameters.Add("currencyType", "2", false, false);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            // Need to make sure the exchange rate is set to Monthly. >> Create Edit tests for Static and Date Range.

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/aeexchangerate.aspx");
            request7.ThinkTime = 9;
            request7.Method = "POST";
            request7.QueryStringParameters.Add("currencyid", currencyID, false, false);
            request7.QueryStringParameters.Add("currencyType", "2", false, false);
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmenu$cmdAddExchange");
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbPositiveFormat", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbNegativeFormat", "1");
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeexchangerate.aspx"));
            request8.ThinkTime = 8;
            request8.QueryStringParameters.Add("currencyid", currencyID, false, false);
            request8.QueryStringParameters.Add("currencyType", "2", false, false);
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "Euro";
            extractionRule4.EndsWith = ",";
            extractionRule4.IgnoreCase = false;
            extractionRule4.UseRegularExpression = false;
            extractionRule4.Required = true;
            extractionRule4.ExtractRandomMatch = false;
            extractionRule4.Index = 0;
            extractionRule4.HtmlDecode = false;
            extractionRule4.ContextParameterName = "";
            extractionRule4.ContextParameterName = "exchangeID";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request8;
            request8 = null;

            string exchange = AutoTools.GetID(this.Context["exchangeID"].ToString(), "e", "'", 1);

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeexchangerate.aspx/saveCurrencyMonth"));
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = @"{""currencyid"":" + currencyID + @",""currencymonthid"":""0"",""exchangerates"":[[" + exchange + @",""10""]],""month"":""1"",""year"":""2012""}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request10.ThinkTime = 2;
            request10.QueryStringParameters.Add("currencyid", currencyID, false, false);
            request10.QueryStringParameters.Add("currencyType", "2", false, false);
            ExtractHiddenFields extractionRule5 = new ExtractHiddenFields();
            extractionRule5.Required = true;
            extractionRule5.HtmlDecode = true;
            extractionRule5.ContextParameterName = "1";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request11.Method = "POST";
            request11.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/admincurrencies.aspx");
            request11.QueryStringParameters.Add("currencyid", currencyID, false, false);
            request11.QueryStringParameters.Add("currencyType", "2", false, false);
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbPositiveFormat", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbNegativeFormat", "1");
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "29");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "13");
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            // Extract currencymonthid
            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecurrency.aspx"));
            request12.QueryStringParameters.Add("currencyid", currencyID, false, false);
            request12.QueryStringParameters.Add("currencyType", "2", false, false);
            ExtractText extractionRule6 = new ExtractText();
            extractionRule6.StartsWith = "tbl_gridMonthlyCurrencies";
            extractionRule6.EndsWith = "January";
            extractionRule6.IgnoreCase = true;
            extractionRule6.UseRegularExpression = false;
            extractionRule6.Required = true;
            extractionRule6.ExtractRandomMatch = false;
            extractionRule6.Index = 0;
            extractionRule6.HtmlDecode = false;
            extractionRule6.ContextParameterName = "";
            extractionRule6.ContextParameterName = "currencymonthID";
            request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule6.Extract);
            yield return request12;
            request12 = null;

            string currencyMonthID = AutoTools.GetID(this.Context["currencymonthID"].ToString(), ",", ")", 1);
            
            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aeexchangerate.aspx");
            validateRequest.QueryStringParameters.Add("currencyid", currencyID, false, false);
            validateRequest.QueryStringParameters.Add("currencymonthid", currencyMonthID, false, false);
            validateRequest.QueryStringParameters.Add("currencyType", "2", false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "txtYear\" type=\"text\" value=\"2012";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;                
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "id=\"exchangerate" + exchange + "\" value=\"10";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);
            }
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
