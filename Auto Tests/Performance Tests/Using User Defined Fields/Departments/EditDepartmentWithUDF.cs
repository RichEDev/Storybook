
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditDepartmentWithUDF : WebTest
    {

        public EditDepartmentWithUDF()
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
            request3.ThinkTime = 1;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/categorymenu.aspx");
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/admindepartments.aspx");
            
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript:changeArchive";
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

            string departmentID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aedepartment.aspx");
            request6.ThinkTime = 8;
            request6.QueryStringParameters.Add("departmentid", departmentID, false, false);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aedepartment.aspx");
            request7.Method = "POST";
            request7.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/shared/admin/admindepartments.aspx";
            request7.QueryStringParameters.Add("departmentid", departmentID, false, false);
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtdepartment", "__James Auto Test EDITED");
            request7Body.FormPostParameters.Add("ctl00$contentmain$vcDepartment_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$vcDepartment_ClientState"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated department - EDITED");
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "15");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "17");
            InsertUDFvalues.AddAllUDFs(request7Body, "AutoDepartment", true);
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aedepartment.aspx");
            validateRequest.QueryStringParameters.Add("departmentid", departmentID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated department - EDITED";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                // Validate UDFs
                cDatabaseConnection dbvalidate = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
                System.Data.SqlClient.SqlDataReader readerVal = dbvalidate.GetReader("SELECT * FROM udfValidation");
                string validationText = "";
                while (readerVal.Read())
                {
                    validationText = readerVal.GetValue(1).ToString();

                    if (validationText.Contains("Text"))
                    {
                        validationText = "EDITED AutoDepartment" + validationText;
                    }
                    validateRequest.ValidateResponse += InsertUDFvalues.ValidateAllUDFs(validateRequest, validationText);
                }
                readerVal.Close();
            }
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
