
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddDepartmentWithUDF : WebTest
    {

        public AddDepartmentWithUDF()
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

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aedepartment.aspx"));
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request3.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aedepartment.aspx"));
            request4.Method = "POST";
            request4.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/admindepartments.aspx");
            FormPostHttpBody request4Body = new FormPostHttpBody();
            request4Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request4Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request4Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request4Body.FormPostParameters.Add("ctl00$contentmain$txtdepartment", "__James Auto Test With UDFs");
            request4Body.FormPostParameters.Add("ctl00$contentmain$vcDepartment_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$vcDepartment_ClientState"].ToString());
            request4Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated department - With User Defined Fields");
            request4Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request4Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "35");
            request4Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "21");
            InsertUDFvalues.AddAllUDFs(request4Body, "AutoDepartment", false);
            request4.Body = request4Body;
            yield return request4;
            request4 = null;

            // Connect to database to get ID after the department has been created
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM departments WHERE department LIKE '__James Auto Test%'");
            reader.Read();
            string departmentID = reader.GetValue(reader.GetOrdinal("departmentid")).ToString();
            reader.Close();

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aedepartment.aspx");
            validateRequest.QueryStringParameters.Add("departmentid", departmentID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test With UDFs";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated department - With User Defined Fields";
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
                    validationText = readerVal.GetValue(0).ToString();

                    if (validationText.Contains("Text"))
                    {
                        validationText = "Testing AutoDepartment" + validationText;
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
