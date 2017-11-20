
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class GeneralOptionsChangeRbtn : WebTest
    {

        public GeneralOptionsChangeRbtn()
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

            // Check that all txt fields can be changed
            while (reader.Read())
            {
                string formName = reader.GetValue(reader.GetOrdinal("formPostKey")).ToString();
                string formValue = reader.GetValue(reader.GetOrdinal("stringValue")).ToString();
                string changeValue = "";

                //*************************************************************************************************
                // IF NEW RADIO BUTTONS ARE ADDED, ADDITIONAL CLAUSES NEED TO BE ADDED HERE AND TO THE VALIDATION
                //*************************************************************************************************

                // Radio buttons on the general tab cannot be set using "0"                  
                if (formName.Contains("tabGeneral$odometerentry") == true)
                {
                    if (formValue == "0")
                    {
                        changeValue = "1";
                    }
                    else
                    {
                        changeValue = "optodologin";
                    }                        
                }
                else if (formName.Contains("tabGeneral$locations") == true)
                {                    
                    if (formValue == "0")
                    {
                        changeValue = "1";
                    }
                    else
                    {
                        changeValue = "optlocationdd";
                    }
                } else if (formName.Contains("source") == true)
                {
                    // The e-mail source cannot be set using "1"
                    if (formValue == "0")
                    {
                        changeValue = "optserver";
                    }
                    else
                    {
                        changeValue = "0";
                    }
                }

                if (changeValue != "")
                {
                    foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit(formName, changeValue, "rbtn"), false)) { yield return r; }
                }
            }
            
            reader.Close();
        }
    }
}