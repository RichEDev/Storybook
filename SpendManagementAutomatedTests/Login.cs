using System;
using System.Collections.Generic;
using SpendManagement.AutomatedTests.Runtime;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

namespace SpendManagement.AutomatedTests
{
    /// <summary>
    /// 
    /// </summary>
    public class Login : DynamicWebTest
    {
        /// <summary>
        /// 
        /// </summary>
        public Login()
        {
            this.Context.Add("Expenses Test Server", "https://sm.sel-expenses.com");
            this.Context.Add("Expenses Account ID", "211");
            this.Context.Add("Company Name", "Lewis");
            this.Context.Add("Username", "Lewis");
            this.Context.Add("Password", "potato");

            this.PreAuthenticate = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Medium))
            {
                ValidateResponseUrl responseValidator = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(responseValidator.Validate);
            }

            WebTestRequest request1 = new WebTestRequest((this.Context["Expenses Test Server"].ToString() + "/logon.aspx"));
            request1.ExpectedHttpStatusCode = 200;
            request1.ExpectedResponseUrl = (this.Context["Expenses Test Server"].ToString() + "/logon.aspx");

            yield return request1;

            WebTestRequest request2 = new WebTestRequest((this.Context["Expenses Test Server"].ToString() + "/logon.aspx"));
            request2.Method = "POST";
            request2.ExpectedHttpStatusCode = 200;
            request2.ExpectedResponseUrl = (this.Context["Expenses Test Server"].ToString() + "/home.aspx");

            FormPostHttpBody request2Body = new FormPostHttpBody();
            request2Body.FormPostParameters.Add("ctl00$pageContents$txtCompanyID", this.Context["Company Name"].ToString());
            request2Body.FormPostParameters.Add("ctl00$pageContents$txtUsername", this.Context["Username"].ToString());
            request2Body.FormPostParameters.Add("ctl00$pageContents$txtPassword", this.Context["Password"].ToString());
            request2Body.FormPostParameters.Add("ctl00$pageContents$btnLogon.x", "0");
            request2Body.FormPostParameters.Add("ctl00$pageContents$btnLogon.y", "0");
            request2.Body = request2Body;

            WebTestRequest request2Dependent1 = new WebTestRequest((this.Context["Expenses Test Server"].ToString() + "/home.aspx"));
            request2Dependent1.ExpectedHttpStatusCode = 200;
            request2Dependent1.ExpectedResponseUrl = (this.Context["Expenses Test Server"].ToString() + "/home.aspx");
            request2.DependentRequests.Add(request2Dependent1);

            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText findValidator = new ValidationRuleFindText();
                findValidator.FindText = "The details you have entered are incorrect";
                findValidator.IgnoreCase = true;
                findValidator.UseRegularExpression = false;
                findValidator.PassIfTextFound = false;

                request2.ValidateResponse += new EventHandler<ValidationEventArgs>(findValidator.Validate);
            }

            yield return request2;
        }
    }
}
