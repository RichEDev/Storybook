
namespace SpendManagement.AutomatedTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class UnArchiveLocationCoded : WebTest
    {

        public UnArchiveLocationCoded()
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
            request1.ThinkTime = 9;
            request1.ExpectedResponseUrl = "https://www2.sel-expenses.com/shared/logon.aspx?ReturnUrl=%2fdefault.aspx";
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request1.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request1;
            request1 = null;

            WebTestRequest request2 = new WebTestRequest("https://www2.sel-expenses.com/shared/logon.aspx");
            request2.ThinkTime = 20;
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
            request4.ThinkTime = 7;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/locationsearch.aspx");
            request5.ThinkTime = 9;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/locationsearch.aspx");
            request6.ThinkTime = 6;
            request6.Method = "POST";
            request6.ExpectedResponseUrl = "https://www2.sel-expenses.com/shared/admin/adminlocations.aspx?name=james+auto+te" +
                "st&";
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtname", "james auto test");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcode", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmbshowfrom", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmbshowto", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmbiscompany", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtparentcompany", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline1", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline2", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcity", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcounty", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtpostcode", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcountry", "");
            request6Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "0");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "0");
            request6.Body = request6Body;
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "javascript";
            extractionRule3.EndsWith = "James Auto Test";
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

            string extracted, id;
            extracted = this.Context["NewID"].ToString();
            id = FindString.GetID(extracted);

            WebTestRequest request7 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/adminlocations.aspx/changeStatus");
            request7.ThinkTime = 3;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"accountid\":239,\"companyid\":" + id + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/adminlocations.aspx");
            request8.QueryStringParameters.Add("name", "james+auto+test", false, false);
            request8.QueryStringParameters.Add("", "", false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "changeArchiveStatus(" + id + ");\"><img title=\"Archive\"";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request8.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request8;
            request8 = null;
        }
    }
}
