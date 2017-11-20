
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class GeneralOptionsUnselectChk : WebTest
    {

        public GeneralOptionsUnselectChk()
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
            System.Data.SqlClient.SqlDataReader reader = testdb.GetReader("SELECT * FROM accountProperties");

            // Check that all checkboxes can be UN-ticked
            while (reader.Read())
            {
                string formName = reader.GetValue(reader.GetOrdinal("formPostKey")).ToString();

                // Only test CHECKBOXES remain UN-ticked
                if (formName.Contains("chk") == true)
                {
                    // Change all of the values to UN-ticked and validate
                    foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit(formName, "", "chkoff"), false)) { yield return r; }
                }
            }

            // Run a test to check that all tick boxes have remained unticked? May be the case that they can stay unticked individually,
            // but not when other values are unticked aswell
            
            reader.Close();
        }
    }
}