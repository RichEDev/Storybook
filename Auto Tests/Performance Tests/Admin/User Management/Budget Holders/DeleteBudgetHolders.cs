
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteBudgetHolders : WebTest
    {

        public DeleteBudgetHolders()
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request5.ThinkTime = 1;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 5;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "deleteBudgetHolder";
            extractionRule2.EndsWith = "__Auto Budget Holder";
            extractionRule2.IgnoreCase = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "budgetHolderID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "deleteBudgetHolder";
            extractionRule3.EndsWith = "__Auto Budget Holder Two";
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "secondBudgetHolderID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "var accountid = ";
            extractionRule4.EndsWith = ";";
            extractionRule4.ContextParameterName = "";
            extractionRule4.ContextParameterName = "accountID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request7;
            request7 = null;

            string budgetHolderID = AutoTools.GetID(this.Context["budgetHolderID"].ToString(), ";", "<", 21);
            string secondBudgetHolderID = AutoTools.GetID(this.Context["secondBudgetHolderID"].ToString(), ";", "<", 21);
            string accountID = this.Context["accountID"].ToString();

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx/deleteBudgetHolder"));
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"accountid\":" + accountID + ",\"budgetholderid\":" + budgetHolderID + "}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx/deleteBudgetHolder"));
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"accountid\":" + accountID + ",\"budgetholderid\":" + secondBudgetHolderID + "}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx");
            AutoTools.ValidateText("__Auto Budget Holder", request10, false, false);          
            yield return request10;
            request10 = null;
        }
    }
}
