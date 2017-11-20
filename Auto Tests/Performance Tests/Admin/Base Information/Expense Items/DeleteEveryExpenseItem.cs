
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteEveryExpenseItem : WebTest
    {

        public DeleteEveryExpenseItem()
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

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request6.ThinkTime = 1;
            yield return request6;
            request6 = null;

            string accountID = string.Empty;
            string expenseItemID = string.Empty;            

            while (1 == 1)
            {
                WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
                ExtractText extractionRule2 = new ExtractText();
                extractionRule2.StartsWith = "var accountid = ";
                extractionRule2.EndsWith = ";";
                extractionRule2.IgnoreCase = false;
                extractionRule2.UseRegularExpression = false;
                extractionRule2.Required = false;
                extractionRule2.ExtractRandomMatch = false;
                extractionRule2.Index = 0;
                extractionRule2.HtmlDecode = false;
                extractionRule2.ContextParameterName = "";
                extractionRule2.ContextParameterName = "accountID";
                request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
                ExtractText extractionRule3 = new ExtractText();
                extractionRule3.StartsWith = "javascript";
                extractionRule3.EndsWith = "__Auto";
                extractionRule3.IgnoreCase = false;
                extractionRule3.UseRegularExpression = false;
                extractionRule3.Required = false;
                extractionRule3.ExtractRandomMatch = false;
                extractionRule3.Index = 0;
                extractionRule3.HtmlDecode = false;
                extractionRule3.ContextParameterName = "";
                extractionRule3.ContextParameterName = "expenseItemID";
                request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
                request7.ThinkTime = 3;
                yield return request7;
                request7 = null;

                try
                {
                    accountID = this.Context["accountID"].ToString();
                    expenseItemID = AutoTools.GetID(this.Context["expenseItemID"].ToString());
                }
                catch
                {
                    break;
                }

                WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx/deleteSubcat"));
                request8.Method = "POST";
                StringHttpBody request8Body = new StringHttpBody();
                request8Body.ContentType = "application/json; charset=utf-8";
                request8Body.InsertByteOrderMark = false;
                request8Body.BodyString = "{\"accountid\":" + accountID + ",\"subcatid\":" + expenseItemID + "}";
                request8.Body = request8Body;
                yield return request8;
                request8 = null;

                this.Context.Remove("expenseItemID");
                this.Context.Remove("accountID");
            }
        }
    }
}
