
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddFixedAllowanceExpense : WebTest
    {

        public AddFixedAllowanceExpense()
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

            // Get the accountID and the employeeID
            WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/home.aspx");
            ExtractText accountExtraction = new ExtractText();
            accountExtraction.StartsWith = "var accountid = ";
            accountExtraction.EndsWith = ";";
            accountExtraction.Required = true;
            accountExtraction.ContextParameterName = "";
            accountExtraction.ContextParameterName = "accountID";
            request4.ExtractValues += new EventHandler<ExtractionEventArgs>(accountExtraction.Extract);
            ExtractText employeeExtraction = new ExtractText();
            employeeExtraction.StartsWith = "var employeeid = ";
            employeeExtraction.EndsWith = ";";
            employeeExtraction.Required = true;
            employeeExtraction.ContextParameterName = "";
            employeeExtraction.ContextParameterName = "employeeID";
            request4.ExtractValues += new EventHandler<ExtractionEventArgs>(employeeExtraction.Extract);
            yield return request4;
            request4 = null;
            string accountID = this.Context["accountID"].ToString();
            string employeeID = this.Context["employeeID"].ToString();


            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx"));
            request5.ThinkTime = 1;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx"));
            request11.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/expenses/claimViewer.aspx";
            request11.Method = "POST";
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmain$cmdok");
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());

            // Use the extracted employeeID and accountID
            request11Body.FormPostParameters.Add("employeeid", employeeID);
            request11Body.FormPostParameters.Add("accountid", accountID);
            request11Body.FormPostParameters.Add("ctl00$contentleft$chkitems$0", "on");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtdate", "25/02/2010");
            request11Body.FormPostParameters.Add("ctl00$contentmain$_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$_ClientState"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$valcalltxtdate_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$valcalltxtdate_ClientState"].ToString());

            // Need companyID, reasonID, countryID and currencyID
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtcompanyid", "23675");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtcompany", "Microsoft");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbreason", "356");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbcountry", "78");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbcurrency", "2883");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtexchangerate", "");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtotherdetails", "Testing 06111986");

            // Add UDF values if Auto UDFs have been added
            cDatabaseConnection udfData = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader udfCount = udfData.GetReader("SELECT COUNT(*) FROM userdefined WHERE attribute_name LIKE 'Auto%'");
            udfCount.Read();
            bool validateUDFs = false;
            if (udfCount.GetValue(0).ToString() == "136")
            {
                validateUDFs = true;
                InsertUDFvalues.AddAllUDFs(request11Body, "AutoExpenseItem", false);
            }
            udfCount.Close();

            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartment0", "139");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcode0", "245");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbprojectcode0", "26");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtpercentage0", "100");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtrows", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtcostcodetotal", "100");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtnumitems", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtexpenseid0", "");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtadvance0", "");

            cDatabaseConnection subcatDB = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader subcatReader = subcatDB.GetReader("SELECT subcatid FROM subcats WHERE subcat = '__Auto Fixed Allowance Item'");
            subcatReader.Read();
            string subcatID = subcatReader.GetValue(0).ToString();
            subcatReader.Close();

            // The bit that's actually different:
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtsubcatid0", subcatID);
            request11Body.FormPostParameters.Add("ctl00$contentmain$chkallowance0", "on");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtallowances0", "1");
            // Connect to database and check for UDF called 'Details'
            cDatabaseConnection detailsDB = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader detailsReader = detailsDB.GetReader(@"SELECT userdefined.userdefineid FROM subcats_userdefined 
                                                INNER JOIN userdefined ON userdefined.userdefineid = subcats_userdefined.userdefineid
                                                WHERE subcatid = " + subcatID + " AND userdefined.attribute_name LIKE 'Details%'");
            string udfDetails = "";
            if (detailsReader.HasRows)
            {
                detailsReader.Read();
                udfDetails = detailsReader.GetValue(0).ToString();                
                request11Body.FormPostParameters.Add("ctl00$contentmain$txtudf" + udfDetails + "0", "Testing fixed allowance");
            }   
            detailsReader.Close();
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            // =====================================
            // Validation

            WebTestRequest request12 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/claimsummary.aspx");
            request12.QueryStringParameters.Add("claimtype", "1");
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "claimid=";
            extractionRule3.EndsWith = "&quot";
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "claimID";
            request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request12;
            request12 = null;

            string claimID = this.Context["claimID"].ToString();

            cDatabaseConnection valDatabase = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader valReader = valDatabase.GetReader(@"SELECT expenseid FROM savedexpenses WHERE 
                                                            claimid = " + claimID + " AND primaryitem = '1' ORDER BY expenseid DESC");
            valReader.Read();
            string expenseID = valReader.GetValue(0).ToString();
            valReader.Close();

            WebTestRequest request14 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/aeexpense.aspx");
            request14.QueryStringParameters.Add("action", "2");
            request14.QueryStringParameters.Add("claimid", claimID);
            request14.QueryStringParameters.Add("expenseid", expenseID);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                List<string> expenseVal = new List<string>();
                expenseVal.Add("value=\"25/02/2010\"");
                expenseVal.Add("value=\"Microsoft\"");
                // At some point, the following 'value's should be parameterised
                expenseVal.Add("<option selected=\"selected\" value=\"356\">Employee Entertainment</option>");
                expenseVal.Add("<option selected=\"selected\" value=\"2883\">Pound Sterling</option>");
                expenseVal.Add("Testing 06111986");
                expenseVal.Add("contentmain_chkallowance0\" type=\"checkbox\"");
                expenseVal.Add("contentmain$txtallowances0\" type=\"text\" value=\"1\"");
                if (udfDetails != "")
                {
                    expenseVal.Add("contentmain$txtudf" + udfDetails + "0\" type=\"text\" value=\"Testing fixed allowance\"");
                }
                for (int x = 0; x < expenseVal.Count; x++)
                {
                    ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                    validationRule2.FindText = expenseVal[x];
                    request14.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                }

                // UDF Validation
                if (validateUDFs == true)
                {
                    string validationText = "";
                    cDatabaseConnection datasource = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
                    System.Data.SqlClient.SqlDataReader reader = datasource.GetReader("SELECT * FROM udfValidation");

                    while (reader.Read())
                    {
                        validationText = reader.GetValue(0).ToString();

                        if (validationText.Contains("Text"))
                        {
                            validationText = "Testing AutoExpenseItem" + validationText;
                        }
                        request14.ValidateResponse += InsertUDFvalues.ValidateAllUDFs(request14, validationText);
                    }
                    reader.Close();
                }
            }
            yield return request14;
            request14 = null;
        }
    }
}
