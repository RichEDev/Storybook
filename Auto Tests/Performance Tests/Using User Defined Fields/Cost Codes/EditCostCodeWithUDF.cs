
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditCostCodeWithUDF : WebTest
    {

        public EditCostCodeWithUDF()
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

            WebTestRequest request3 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/adminmenu.aspx");
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/categorymenu.aspx");
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/admincostcodes.aspx");
            request5.ThinkTime = 6;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "__James Auto Test";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "NewID";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            string costcodeID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aecostcode.aspx");
            request6.ThinkTime = 10;
            request6.QueryStringParameters.Add("costcodeid", costcodeID, false, false);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aecostcode.aspx");
            request7.ThinkTime = 3;
            request7.Method = "POST";
            request7.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/shared/admin/admincostcodes.aspx";
            request7.QueryStringParameters.Add("costcodeid", costcodeID, false, false);
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtcostcode", "__James Auto Test EDITED");
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated cost code - EDITED");
            InsertUDFvalues.AddAllUDFs(request7Body, "AutoCostCode", true);
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "26");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "17");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aecostcode.aspx");
            request9.QueryStringParameters.Add("costcodeid", costcodeID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);                

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated cost code - EDITED";
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
                    validationText = reader.GetValue(1).ToString();

                    if (validationText.Contains("Text"))
                    {
                        validationText = "EDITED AutoCostCode" + validationText;
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
