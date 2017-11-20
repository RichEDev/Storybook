
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddItemRole : WebTest
    {

        public AddItemRole()
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
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 2;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminitemroles.aspx"));
            request7.ThinkTime = 2;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aeitemrole.aspx"));
            request8.ThinkTime = 19;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "2";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request8;
            request8 = null;

            // Check that standard expense item exists, if not create
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT subcatid FROM subcats WHERE subcat = '__Auto Standard Item'");

            if (!reader.HasRows)
            {
                // Create standard expense item
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddExpenseItemStandard(), false)) { yield return r; }
            }

            reader.Close();

            reader = database.GetReader("SELECT subcatid FROM subcats WHERE subcat = '__Auto Standard Item'");
            reader.Read();
            string expenseItemID = reader.GetValue(0).ToString();
            reader.Close();


            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aeitemrole.aspx"));
            request9.Method = "POST";
            request9.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("ctl00$scriptman", "ctl00$contentmain$UpdatePanel2|ctl00$contentmain$lnksubcat" + expenseItemID);
            request9Body.FormPostParameters.Add("itemroleid", this.Context["$HIDDEN2.itemroleid"].ToString());
            request9Body.FormPostParameters.Add("accountid", this.Context["$HIDDEN2.accountid"].ToString());
            request9Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmain$lnksubcat" + expenseItemID);
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN2.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN2.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtrolename", "__Auto Item Role");
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated Item Role - Do not edit");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("__ASYNCPOST", "true");
            request9.Body = request9Body;
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "2";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aeitemrole.aspx"));
            request10.Method = "POST";
            request10.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/adminitemroles.aspx");
            FormPostHttpBody request10Body = new FormPostHttpBody();
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtrolename", "__Auto Item Role");
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated Item Role - Do not edit");
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtmaxwithout" + expenseItemID, "0.00");
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtmaxwith" + expenseItemID, "0.00");
            request10Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request10Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request10Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request10Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN2.__VIEWSTATE"].ToString());
            request10Body.FormPostParameters.Add("__AjaxControlToolkitCalendarCssLoaded", this.Context["$HIDDEN2.__AjaxControlToolkitCalendarCssLoaded"].ToString());
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "33");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "14");
            request10.Body = request10Body;
            yield return request10;
            request10 = null;
        }
    }
}
