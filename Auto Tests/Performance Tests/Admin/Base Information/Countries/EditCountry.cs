
using System.Xml.Schema;

namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditCountry : WebTest
    {

        public EditCountry()
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
            request3.ThinkTime = 2;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx"));
            request5.ThinkTime = 2;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript:changeArchive";
            extractionRule2.EndsWith = "American Samoa";
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

            string countryID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx"));
            request6.ThinkTime = 9;
            request6.QueryStringParameters.Add("countryid", countryID, false, false);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);            
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx"));
            request7.ThinkTime = 12;
            request7.QueryStringParameters.Add("countryid", countryID, false, false);
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx/saveCountrySubcat"));
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"countryid\":" + countryID + ",\"countrysubcatid\":0,\"subcatid\":\"" + expenseItemID + "\",\"vatRate\":\"17.5\",\"claimab" +
                "lePercentage\":\"10\"}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx/CreateGrid"));
            request9.ThinkTime = 2;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"contextKey\":" + countryID + "}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx"));
            request10.Method = "POST";
            request10.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx");
            request10.QueryStringParameters.Add("countryid", countryID, false, false);
            FormPostHttpBody request10Body = new FormPostHttpBody();
            request10Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request10Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request10Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbSubcat", expenseItemID);
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtVatRate", "17.5");
            request10Body.FormPostParameters.Add("ctl00$contentmain$txtClaimablePercentage", "10");
            request10Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "0");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "24");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "16");
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aecountry.aspx");
            validateRequest.QueryStringParameters.Add("countryid", countryID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__Auto P11D Standard Item', " + expenseItemID + ", 17.5, 10";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
