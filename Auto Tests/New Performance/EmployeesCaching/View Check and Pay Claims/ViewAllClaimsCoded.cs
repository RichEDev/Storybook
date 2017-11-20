﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class ViewAllClaimsCoded : WebTest
    {

        public ViewAllClaimsCoded()
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
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidationRuleResponseTimeGoal validationRule2 = new ValidationRuleResponseTimeGoal();
                validationRule2.Tolerance = 0D;
                this.ValidateResponseOnPageComplete += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }

            WebTestRequest request1 = new WebTestRequest("http://localhost/main/expenses/");
            request1.ParseDependentRequests = false;
            yield return request1;
            request1 = null;

            WebTestRequest request2 = new WebTestRequest("http://localhost/main/expenses/logon.aspx");
            request2.ParseDependentRequests = false;
            request2.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request2.ExpectedResponseUrl = "http://localhost/main/expenses/shared/logon.aspx";
            WebTestRequest request2Dependent1 = new WebTestRequest("http://localhost/main/expenses/shared/");
            request2Dependent1.ThinkTime = 11;
            request2Dependent1.ExpectedResponseUrl = "http://localhost/main/expenses/shared/logon.aspx?ReturnUrl=%2fshared%2f";
            request2.DependentRequests.Add(request2Dependent1);
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request2.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request2;
            request2 = null;

            WebTestRequest request3 = new WebTestRequest("http://localhost/main/expenses/shared/logon.aspx");
            request3.ParseDependentRequests = false;
            request3.ThinkTime = 21;
            request3.Method = "POST";
            request3.ExpectedResponseUrl = "http://localhost/main/expenses/home.aspx";
            FormPostHttpBody request3Body = new FormPostHttpBody();
            request3Body.FormPostParameters.Add("tsm_HiddenField", ";;AjaxControlToolkit, Version=4.1.7.123, Culture=neutral, PublicKeyToken=28f01b" +
                    "0e84b6d53e:en-GB:dcb189ca-d78f-4a6b-930e-c3028c568dd1:de1feab2:f9cec9bc:a67c2700" +
                    ":f2c8e708:720a52bf:589eaa30:698129cf:59fb9c6f");
            request3Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request3Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request3Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request3Body.FormPostParameters.Add("txtCompanyID", "loadtest");
            request3Body.FormPostParameters.Add("txtUsername", "dylan");
            request3Body.FormPostParameters.Add("txtPassword", "Password1");
            request3Body.FormPostParameters.Add("btnLogon.x", "0");
            request3Body.FormPostParameters.Add("btnLogon.y", "0");
            request3.Body = request3Body;
            yield return request3;
            request3 = null;

            cDatabaseConnection database = new cDatabaseConnection("Data Source=JAMESLT\\SQLEXPRESS;Initial Catalog=AutoTestingDataSources;Persist Security Info=True;User ID=spenduser;Password=P3ngu1ns");

            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM loadTestClaims");

            WebTestRequest request4;

            while (reader.Read())
            {
                request4 = new WebTestRequest("http://localhost/main/expenses/aeexpense.aspx");
                request4.QueryStringParameters.Add("returnto", "3", false, false);
                request4.QueryStringParameters.Add("claimid", reader.GetValue(0).ToString(), false, false);
                request4.QueryStringParameters.Add("employeeid", reader.GetValue(1).ToString(), false, false);
                request4.QueryStringParameters.Add("stage", "1", false, false);
                request4.QueryStringParameters.Add("action", "2", false, false);
                request4.QueryStringParameters.Add("expenseid", reader.GetValue(2).ToString(), false, false);
                request4.QueryStringParameters.Add("", "", false, false);
                yield return request4;
                request4 = null;
            }

            reader.Close();
            
        }
    }
}
