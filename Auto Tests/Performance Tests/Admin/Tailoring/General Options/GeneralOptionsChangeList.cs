
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class GeneralOptionsChangeList : WebTest
    {

        public GeneralOptionsChangeList()
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
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/tailoringmenu.aspx"));
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accountOptions.aspx"));
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;
            
            // Connect to database
            cDatabaseConnection testdb = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            // Only select list fields
            System.Data.SqlClient.SqlDataReader reader = testdb.GetReader("SELECT * FROM accountProperties " +
            "WHERE (formPostKey LIKE '%ddl%' OR formPostKey LIKE '%cmb%') AND formPostKey NOT LIKE '%chk%'");            

            // Check that all drop down lists can be changed
            while (reader.Read())
            {
                string formName = reader.GetValue(reader.GetOrdinal("formPostKey")).ToString();

                WebTestRequest requestList = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/accountOptions.aspx");
                ExtractText extractionRuleList = new ExtractText();
                extractionRuleList.StartsWith = "ctl00$contentmain$" + formName;
                extractionRuleList.EndsWith = "</select>";
                extractionRuleList.IgnoreCase = false;
                extractionRuleList.UseRegularExpression = false;
                extractionRuleList.Required = true;
                extractionRuleList.ExtractRandomMatch = false;
                extractionRuleList.Index = 0;
                extractionRuleList.HtmlDecode = false;
                extractionRuleList.ContextParameterName = "";
                extractionRuleList.ContextParameterName = "listValues";
                requestList.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRuleList.Extract);
                yield return requestList;
                requestList = null;
                // Get the ID of the item in the ddl that is to be selected
                string newValue = AutoTools.GetListValue(this.Context["listValues"].ToString());

                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit(formName, newValue, "list"), false)) { yield return r; }
            }
            
            reader.Close();
        }
    }
}