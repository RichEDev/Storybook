
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddAllowance : WebTest
    {
        public AddAllowance()
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

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminallowances.aspx");
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx");
            request6.ThinkTime = 45;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "option ";
            extractionRule2.EndsWith = "Pound Sterling";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "CurrencyID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            string currencyID = AutoTools.GetID(this.Context["CurrencyID"].ToString(), "=", "\"", 2);

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveAllowance");
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"allowanceID\":0,\"name\":\"__James Auto Test\",\"description\":\"Automatically generated " +
                "allowance - do not edit\",\"numhours\":10,\"rate\":20,\"currencyid\":" + currencyID + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            // Connect to database to get the ID of the created Allowance - needed to add a rate
            cDatabaseConnection testdb = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = testdb.GetReader("SELECT * FROM allowances WHERE allowance = '__James Auto Test'");           
            reader.Read();                      
            string allowanceID = reader.GetValue(reader.GetOrdinal("allowanceid")).ToString();
            reader.Close();

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveRate");
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"rateID\":0,\"allowanceID\":" + allowanceID + ",\"hours\":12,\"rate\":25}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminallowances.aspx");
            request9.ThinkTime = 5;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx");
            request10.QueryStringParameters.Add("allowanceid", allowanceID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                AutoTools.ValidateText("__James Auto Test", request10);

                AutoTools.ValidateText("option selected=\"selected\" value=\"" + currencyID + "\">Pound Sterling", request10);

                AutoTools.ValidateText("Automatically generated allowance - do not edit", request10);

                AutoTools.ValidateText("input name=\"ctl00$contentmain$txtnighthours\" type=\"text\" value=\"10\"", request10);

                AutoTools.ValidateText("input name=\"ctl00$contentmain$txtnightrate\" type=\"text\" value=\"20.00", request10);

                AutoTools.ValidateText(">12</td><td class=\"row([1-2])\" align=\"right\">25.00", request10, true, true);
            }
            yield return request10;
            request10 = null;
        }
    }
}
