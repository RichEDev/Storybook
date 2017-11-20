
namespace SpendManagement.AutomatedTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditAllowanceRateCoded : WebTest
    {

        public EditAllowanceRateCoded()
        {
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

            WebTestRequest request1 = new WebTestRequest("https://www2.sel-expenses.com/");
            request1.ThinkTime = 7;
            request1.ExpectedResponseUrl = "https://www2.sel-expenses.com/shared/logon.aspx?ReturnUrl=%2fdefault.aspx";
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request1.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request1;
            request1 = null;

            WebTestRequest request2 = new WebTestRequest("https://www2.sel-expenses.com/shared/logon.aspx");
            request2.ThinkTime = 12;
            request2.Method = "POST";
            request2.ExpectedResponseUrl = "https://www2.sel-expenses.com/home.aspx";
            request2.QueryStringParameters.Add("ReturnUrl", "%2fdefault.aspx", false, false);
            FormPostHttpBody request2Body = new FormPostHttpBody();
            request2Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request2Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request2Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request2Body.FormPostParameters.Add("ctl00$pageContents$txtCompanyID", "sel_test");
            request2Body.FormPostParameters.Add("ctl00$pageContents$txtUsername", "james");
            request2Body.FormPostParameters.Add("ctl00$pageContents$txtPassword", "Password1");
            request2Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request2Body.FormPostParameters.Add("ctl00$pageContents$btnLogon.x", "0");
            request2Body.FormPostParameters.Add("ctl00$pageContents$btnLogon.y", "0");
            request2.Body = request2Body;
            yield return request2;
            request2 = null;

            WebTestRequest request3 = new WebTestRequest("https://www2.sel-expenses.com/adminmenu.aspx");
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest("https://www2.sel-expenses.com/categorymenu.aspx");
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/adminallowances.aspx");
            request5.ThinkTime = 2;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "James Auto Test";
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

            string extracted, id;
            extracted = this.Context["NewID"].ToString();
            id = FindString.GetID(extracted, "(", ")", 1);

            WebTestRequest request6 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx");
            request6.ThinkTime = 3;
            request6.QueryStringParameters.Add("allowanceid", id, false, false);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "option ";
            extractionRule3.EndsWith = "Pound Sterling";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "CurrencyID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            string currencyid;
            extracted = this.Context["CurrencyID"].ToString();
            currencyid = FindString.GetID(extracted, "=", "\"", 2);

            WebTestRequest request7 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/saveAllowance");
            request7.ThinkTime = 8;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"allowanceID\":" + id + ",\"name\":\"James Auto Test EDITED\",\"description\":\"Automatically g" +
                "enerated allowance - EDITED\",\"numhours\":20,\"rate\":40,\"currencyid\":" + id + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/saveRate");
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"rateID\":0,\"allowanceID\":" + id + ",\"hours\":24,\"rate\":50}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/createRatesGrid");
            request9.ThinkTime = 4;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"contextKey\":" + id + "}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/saveAllowance");
            request10.ThinkTime = 6;
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"allowanceID\":" + id + ",\"name\":\"James Auto Test EDITED\",\"description\":\"Automatically g" +
                "enerated allowance - EDITED\",\"numhours\":20,\"rate\":40,\"currencyid\":" + id + "}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/saveRate");
            request11.Method = "POST";
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json; charset=utf-8";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = "{\"rateID\":0,\"allowanceID\":" + id + ",\"hours\":48,\"rate\":70}";
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/createRatesGrid");
            request12.ThinkTime = 3;
            request12.Method = "POST";
            StringHttpBody request12Body = new StringHttpBody();
            request12Body.ContentType = "application/json; charset=utf-8";
            request12Body.InsertByteOrderMark = false;
            request12Body.BodyString = "{\"contextKey\":" + id + "}";
            request12.Body = request12Body;
            yield return request12;
            request12 = null;

            WebTestRequest request13 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/aeallowance.aspx/saveAllowance");
            request13.Method = "POST";
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=utf-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"allowanceID\":" + id + ",\"name\":\"James Auto Test EDITED\",\"description\":\"Automatically g" +
                "enerated allowance - EDITED\",\"numhours\":20,\"rate\":40,\"currencyid\":" + currencyid + "}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            WebTestRequest request14 = new WebTestRequest("https://www2.sel-expenses.com/expenses/admin/adminallowances.aspx");
            request14.ThinkTime = 3;
            yield return request14;
            request14 = null;
        }
    }
}
