
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class UnArchiveLocation : WebTest
    {

        public UnArchiveLocation()
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

            WebTestRequest request3 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/adminmenu.aspx");
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/categorymenu.aspx");
            request4.ThinkTime = 7;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request5.ThinkTime = 9;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request6.ThinkTime = 6;
            request6.Method = "POST";
            request6.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx?";
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtname", "__James Auto Test");
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
            extractionRule3.EndsWith = "\">__James Auto Test";
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
            id = AutoTools.GetID(extracted);

            WebTestRequest request30 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request30.ThinkTime = 4;
            ExtractText extractionRule30 = new ExtractText();
            extractionRule30.StartsWith = "var accountid = ";
            extractionRule30.EndsWith = ";";
            extractionRule30.IgnoreCase = false;
            extractionRule30.UseRegularExpression = false;
            extractionRule30.Required = true;
            extractionRule30.ExtractRandomMatch = false;
            extractionRule30.Index = 0;
            extractionRule30.HtmlDecode = false;
            extractionRule30.ContextParameterName = "";
            extractionRule30.ContextParameterName = "accountID";
            request30.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule30.Extract);
            yield return request30;
            request30 = null;

            string accountid = this.Context["accountID"].ToString();

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx/changeStatus");
            request7.ThinkTime = 3;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"accountid\":" + accountid + ",\"companyid\":" + id + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request8.ThinkTime = 6;
            request8.Method = "POST";
            request8.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx?";
            FormPostHttpBody request8Body = new FormPostHttpBody();
            request8Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request8Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request8Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtname", "__James Auto Test");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtcode", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmbshowfrom", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmbshowto", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmbiscompany", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtparentcompany", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline1", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline2", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtcity", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtcounty", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtpostcode", "");
            request8Body.FormPostParameters.Add("ctl00$contentmain$txtcountry", "");
            request8Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "0");
            request8Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "0");
            request8.Body = request8Body;
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
