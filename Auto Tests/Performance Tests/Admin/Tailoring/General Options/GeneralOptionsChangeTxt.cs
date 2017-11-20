
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class GeneralOptionsChangeTxt : WebTest
    {

        public GeneralOptionsChangeTxt()
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

                // Need to look at password settings - can't change the min/max values if 'any length' is selected

                // Only test TXT values
                if (formName.Contains("txt") == true)
                {
                    // if the field is password expires, don't set a small number as it will cause an error logging in
                    if (formName == "txtexpires")
                    {
                        if (formValue == "999")
                        {
                            changeValue = "998";
                        }
                        else
                        {
                            changeValue = "999";
                        }
                    // if the field is odometer reading day, number must be 0 < x < 28
                    } else if (formName == "tabsGeneralOptions$tabGeneral$txtodometerday")
                    {
                        if (formValue == "9")
                        {
                            changeValue = "10";
                        }
                        else
                        {
                            changeValue = "9";
                        }

                    } else
                    {
                        // Set the new txt value to 66. If 66 is currently being used, set 77
                        if (formValue == "66")
                        {
                            changeValue = "77";
                        }
                        else
                        {
                            changeValue = "66";
                        }
                    }                        
                    
                    foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit(formName, changeValue, "txt"), false)) { yield return r; }
                }
            }
            
            reader.Close();
        }
    }
}