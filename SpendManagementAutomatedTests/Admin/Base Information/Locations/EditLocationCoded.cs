
namespace SpendManagement.AutomatedTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditLocationCoded : WebTest
    {

        public EditLocationCoded()
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

            WebTestRequest request8 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/aelocation.aspx");
            request8.ThinkTime = 11;
            request8.QueryStringParameters.Add("companyid", id, false, false);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest("https://www2.sel-expenses.com/shared/webServices/svcAutoComplete.asmx/getAddress");
            request9.ThinkTime = 6;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"country\":\"United Kingdom\",\"postcode\":\"LN6 321\"}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/aelocation.aspx/saveLocation");
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = @"{""locationID"":" + id + @",""location"":""James Auto Test EDITED"",""code"":""JAT"",""comment"":""Automatically generated location - EDITED"",""iscompany"":false,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""321"",""addressline2"":""321"",""city"":""321"",""county"":""321"",""postcode"":""LN6 321"",""country"":""United Kingdom"",""udfs"":[]}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/locationsearch.aspx");
            yield return request11;
            request11 = null;
        }
    }
}
