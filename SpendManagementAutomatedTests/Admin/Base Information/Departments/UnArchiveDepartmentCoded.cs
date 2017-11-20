﻿
namespace SpendManagement.AutomatedTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class UnArchiveDepartmentCoded : WebTest
    {

        public UnArchiveDepartmentCoded()
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

            WebTestRequest request1 = new WebTestRequest("http://www2.sel-expenses.com/");
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
            request2.ThinkTime = 3;
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
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/admindepartments.aspx");
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript:changeArchive";
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

            WebTestRequest request6 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/admindepartments.aspx/changeStatus");
            request6.ThinkTime = 2;
            request6.Method = "POST";
            StringHttpBody request6Body = new StringHttpBody();
            request6Body.ContentType = "application/json; charset=utf-8";
            request6Body.InsertByteOrderMark = false;
            request6Body.BodyString = "{\"accountid\":239,\"departmentid\":" + id + "}";
            request6.Body = request6Body;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest("https://www2.sel-expenses.com/shared/admin/admindepartments.aspx");
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "(" + id + ");\"><img title=\"Archive";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request7.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request7;
            request7 = null;
        }
    }
}
