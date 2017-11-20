
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddBudgetHolders : WebTest
    {

        public AddBudgetHolders()
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

            // Delete any Budget Holder that may have been left in the system from other automated tests
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM budgetholders WHERE " +
                                                                    "budgetholder like '__Auto Budget Holder%'");
            if (reader.HasRows)
            {
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.DeleteBudgetHolders(), false)) { yield return r; }
            }
            reader.Close();

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 2;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx"));
            request7.ThinkTime = 2;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request8.ThinkTime = 11;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx");
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtlabel", "__Auto Budget Holder");
            request9Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_hdn_1", "james");
            request9Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_text_1", "james");
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated budget holder - do not edit");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "37");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "8");
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request10.Method = "POST";
            request10.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx");
            FormPostHttpBody request10Body = new FormPostHttpBody();
            request10Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request10Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request10Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtlabel", "__Auto Budget Holder Two");
            request10Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_hdn_1", "james");
            request10Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_text_1", "james");
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated second budget holder - do not edit");
            request10Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "37");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "8");
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            // Validation
            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx"));
            AutoTools.ValidateText("__Auto Budget Holder", request11);
            AutoTools.ValidateText("__Auto Budget Holder Two", request11);
            AutoTools.ValidateText("Automatically generated budget holder - do not edit", request11);
            AutoTools.ValidateText("Automatically generated second budget holder - do not edit", request11);
            AutoTools.ValidateText("Lloyd, Mr James", request11);
            yield return request11;
            request11 = null;
        }
    }
}
