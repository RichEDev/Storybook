
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddCostCodeWithUDFs : WebTest
    {

        public AddCostCodeWithUDFs()
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
            request4.ThinkTime = 7;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincostcodes.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecostcode.aspx"));
            request6.ThinkTime = 77;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecostcode.aspx"));
            request7.Method = "POST";
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/admincostcodes.aspx");
            FormPostHttpBody request7Body = new FormPostHttpBody();

            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtcostcode", "__James Auto Test");
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated cost code - with User Defined Fields");
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "35");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "8");

            // Call code to insert all UDFs            
            InsertUDFvalues.AddAllUDFs(request7Body, "AutoCostCode", false);
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/admincostcodes.aspx");
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "javascript";
            extractionRule3.EndsWith = "__James Auto Test";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "NewID";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request8;
            request8 = null;

            string costcodeID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aecostcode.aspx");
            request9.QueryStringParameters.Add("costcodeid", costcodeID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated cost code - with User Defined Fields";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                // UDF Validation

                string validationText = "";
                cDatabaseConnection datasource = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
                System.Data.SqlClient.SqlDataReader reader = datasource.GetReader("SELECT * FROM udfValidation");

                while (reader.Read())
                {
                    validationText = reader.GetValue(0).ToString();

                    if (validationText.Contains("Text")) 
                    { 
                        validationText = "Testing AutoCostCode" + validationText; 
                    }
                    request9.ValidateResponse += InsertUDFvalues.ValidateAllUDFs(request9, validationText);
                }
                reader.Close();
            }
            yield return request9;
            request9 = null;            
        }
    }
}
