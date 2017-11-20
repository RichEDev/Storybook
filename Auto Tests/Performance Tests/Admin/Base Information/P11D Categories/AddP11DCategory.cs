
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddP11DCategory : WebTest
    {

        public AddP11DCategory()
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

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM subcats WHERE subcat = '__Auto P11D Standard Item'");
            if (reader.HasRows == false)
            {
                // Add standard expense item if it does not exist
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddP11DExpenseItem(), false)) { yield return r; }
                reader.Close();
                reader = database.GetReader("SELECT * FROM subcats WHERE subcat = '__Auto P11D Standard Item'");
            }         
            reader.Read();
            string expenseItemID = reader.GetValue(reader.GetOrdinal("subcatid")).ToString();
            reader.Close();

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request3.ThinkTime = 1;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminp11d.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/aep11d.aspx");
            request6.ThinkTime = 9;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aep11d.aspx"));
            request7.ThinkTime = 2;
            request7.Method = "POST";
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/adminp11d.aspx");
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtpdcat", "__James Auto Test");
            request7Body.FormPostParameters.Add("subcat", expenseItemID);
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "33");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "14");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            // Extract values for expense Cat
            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminp11d.aspx"));
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "p11d";
            extractionRule4.EndsWith = "__James Auto Test";
            extractionRule4.IgnoreCase = false;
            extractionRule4.UseRegularExpression = false;
            extractionRule4.Required = true;
            extractionRule4.ExtractRandomMatch = false;
            extractionRule4.Index = 0;
            extractionRule4.HtmlDecode = false;
            extractionRule4.ContextParameterName = "";
            extractionRule4.ContextParameterName = "P11ID";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            request8.ThinkTime = 8;
            yield return request8;
            request8 = null;

            string P11ID = AutoTools.GetID(this.Context["P11ID"].ToString());

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/aep11d.aspx");
            validateRequest.QueryStringParameters.Add("action=2&pdcatid", P11ID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "value=\"" + expenseItemID + "\" checked></td></tr>";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);
            }
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
