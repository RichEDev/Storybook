using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;


    public class AddProjectCodeWithUDF : WebTest
    {

        public AddProjectCodeWithUDF()
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
                this.ValidateResponse += new EventHandler<Microsoft.VisualStudio.TestTools.WebTesting.ValidationEventArgs>(validationRule1.Validate);
            }

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            // Check that all of the Auto UDFs are in place before running the test
            if (AutoTools.CheckAllUDFs() == false)
            {
                // Delete any existing Auto UDFs
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.DeleteUDF(), false)) { yield return r; }
                // Create new Auto UDFs
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddUDF(), false)) { yield return r; }
            }

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request3.ThinkTime = 2;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 9;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminprojectcodes.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeprojectcode.aspx"));
            request6.ThinkTime = 32;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeprojectcode.aspx"));
            request7.Method = "POST";
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminprojectcodes.aspx");
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtprojectcode", "__James Auto Test With UDFs");
            request7Body.FormPostParameters.Add("ctl00$contentmain$vcprojectcode_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$vcprojectcode_ClientState"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated Project Code with UDFs - Do not edit");
            InsertUDFvalues.AddAllUDFs(request7Body, "AutoProjectCode", false);
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "32");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "10");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            // Extract Project Code ID

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/adminprojectcodes.aspx");                        
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "javascript:changeArchive";
            extractionRule3.EndsWith = "__James Auto Test";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "projectID";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule5 = new ValidationRuleFindText();
                validationRule5.FindText = "__James Auto Test With UDFs";
                validationRule5.IgnoreCase = false;
                validationRule5.UseRegularExpression = false;
                validationRule5.PassIfTextFound = true;
                request8.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule5.Validate);

                ValidationRuleFindText validationRule6 = new ValidationRuleFindText();
                validationRule6.FindText = "Automatically generated Project Code with UDFs - Do not edit";
                validationRule6.IgnoreCase = false;
                validationRule6.UseRegularExpression = false;
                validationRule6.PassIfTextFound = true;
                request8.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule6.Validate);
            }

            yield return request8;
            request8 = null;

            string projectID = AutoTools.GetID(this.Context["projectID"].ToString());

            WebTestRequest request10 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aeprojectcode.aspx");
            request10.QueryStringParameters.Add("projectcodeid", projectID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test With UDFs";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated Project Code with UDFs - Do not edit";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                ValidationRuleFindText validationRule4 = new ValidationRuleFindText();
                validationRule4.FindText = "name=\"ctl00$contentmain$chkrechargeable\" /></span>";
                // for ticked - 
                //validationRule4.FindText = "ctl00$contentmain$chkrechargeable\" checked=\"checked\" /></span>";
                validationRule4.IgnoreCase = false;
                validationRule4.UseRegularExpression = false;
                validationRule4.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule4.Validate);

                // UDF Validation

                string validationText = "";
                cDatabaseConnection datasource = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
                System.Data.SqlClient.SqlDataReader reader = datasource.GetReader("SELECT * FROM udfValidation");

                while (reader.Read())
                {
                    validationText = reader.GetValue(0).ToString();

                    if (validationText.Contains("Text"))
                    {
                        validationText = "Testing AutoProjectCode" + validationText;
                    }
                    request10.ValidateResponse += InsertUDFvalues.ValidateAllUDFs(request10, validationText);
                }
                reader.Close();
            }
            yield return request10;
            request10 = null;
        }
    }
}
