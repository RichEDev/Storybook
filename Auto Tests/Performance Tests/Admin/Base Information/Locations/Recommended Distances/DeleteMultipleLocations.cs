
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteMultipleLocations : WebTest
    {

        public DeleteMultipleLocations()
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx"));
            request5.ThinkTime = 1;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx"));
            request6.ThinkTime = 8;
            request6.Method = "POST";
            request6.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx?");
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtname", "");
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
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "45");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "15");
            request6.Body = request6Body;
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "javascript";
            extractionRule3.EndsWith = "__Auto Recommended Test 1";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "firstLocationID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "javascript";
            extractionRule4.EndsWith = "__Auto Recommended Test 2";
            extractionRule4.IgnoreCase = false;
            extractionRule4.UseRegularExpression = false;
            extractionRule4.Required = true;
            extractionRule4.ExtractRandomMatch = false;
            extractionRule4.Index = 0;
            extractionRule4.HtmlDecode = false;
            extractionRule4.ContextParameterName = "";
            extractionRule4.ContextParameterName = "secondLocationID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            ExtractText extractionRule5 = new ExtractText();
            extractionRule5.StartsWith = "var accountid = ";
            extractionRule5.EndsWith = ";";
            extractionRule5.IgnoreCase = false;
            extractionRule5.UseRegularExpression = false;
            extractionRule5.Required = true;
            extractionRule5.ExtractRandomMatch = false;
            extractionRule5.Index = 0;
            extractionRule5.HtmlDecode = false;
            extractionRule5.ContextParameterName = "";
            extractionRule5.ContextParameterName = "accountID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
            yield return request6;
            request6 = null;

            string firstLocationID = AutoTools.GetID(this.Context["firstLocationID"].ToString());
            string secondLocationID = AutoTools.GetID(this.Context["secondLocationID"].ToString());
            string accountID = this.Context["accountID"].ToString();

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx/deleteCompany"));
            request7.ThinkTime = 2;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"accountid\":" + accountID + ",\"companyid\":" + firstLocationID + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx/deleteCompany"));
            request8.ThinkTime = 4;
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"accountid\":" + accountID + ",\"companyid\":" + secondLocationID + "}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx"));
            request9.QueryStringParameters.Add("", "", false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__Auto Recommended Test 1";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = false;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "__Auto Recommended Test 2";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = false;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);
            }
            yield return request9;
            request9 = null;
        }
    }
}
