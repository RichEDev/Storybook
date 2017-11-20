
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    
    public class GeneralOptionsEdit : WebTest
    {
        public GeneralOptionsEdit(string changeItem, string changeValue, string itemType)
        {                        
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
            this.Context.Add("changeItem", changeItem);
            this.Context.Add("changeValue", changeValue);
            this.Context.Add("itemType", itemType);
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accountOptions.aspx"));
            request5.ThinkTime = 6;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accountOptions.aspx"));
            request6.Method = "POST";
            request6.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/tailoringmenu.aspx");
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("ctl00_contentmain_tabsGeneralOptions_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true,true,true,true]}");
            request6Body.FormPostParameters.Add("ctl00_contentmain_tabsNewExpenses_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true,true,true,true,true]}");
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());

            // Create a connection to the current test database
            cDatabaseConnection testdb = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = testdb.GetReader("SELECT * FROM accountProperties");
            
            while (reader.Read())
            {
                string formName = reader.GetValue(reader.GetOrdinal("formPostKey")).ToString();
                string formValue = reader.GetValue(reader.GetOrdinal("stringValue")).ToString();
                           
                // Only post values that contain a formPostKey in the database and it is not the value to be updated
                if (formName != "" && formName != this.Context["changeItem"].ToString())
                {
                    // To unselect a checkbox, "" should be used instead of "0"
                    if (formName.Contains("chk") == true && formValue == "0")
                    {
                        formValue = "";
                    }

                    // Radio buttons on the general tab cannot be set using "0"
                    if (formName.Contains("tabGeneral$odometerentry") == true && formValue == "0") 
                    { 
                        formValue = "optodologin"; 
                    }
                    if (formName.Contains("tabGeneral$locations") == true && formValue == "0") 
                    { 
                        formValue = "optlocationdd"; 
                    }

                    // The e-mail source cannot be set using "1"
                    if (formName.Contains("source") == true && formValue == "1") 
                    { 
                        formValue = "optserver"; 
                    }

                    request6Body.FormPostParameters.Add("ctl00$contentmain$" + formName, formValue);
                }

            }
            reader.Close();

            // Set the properties that are to be changed below
            request6Body.FormPostParameters.Add("ctl00$contentmain$" + this.Context["changeItem"].ToString(), this.Context["changeValue"].ToString());

            request6Body.FormPostParameters.Add("ctl00$contentmain$tabsNewExpenses$tabmileage$hdnAllowMileage", "1");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtDisplayAs", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$hdnAddScreenCode", this.Context["$HIDDEN1.ctl00$contentmain$hdnAddScreenCode"].ToString());
            request6Body.FormPostParameters.Add("ctl00$contentmain$hdnAddScreenFieldID", this.Context["$HIDDEN1.ctl00$contentmain$hdnAddScreenFieldID"].ToString());
            request6Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request6Body.FormPostParameters.Add("ctl00$contentmain$btnSave.x", "26");
            request6Body.FormPostParameters.Add("ctl00$contentmain$btnSave.y", "5");
            request6.Body = request6Body;
            yield return request6;
            request6 = null;


            // Validate that the changes have been saved properly
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsValidate(this.Context["changeItem"].ToString(),
                this.Context["changeValue"].ToString(), this.Context["itemType"].ToString()), false)) { yield return r; }         
        }
    }
}
