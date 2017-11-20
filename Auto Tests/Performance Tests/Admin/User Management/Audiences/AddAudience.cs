
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddAudience : WebTest
    {

        public AddAudience()
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

            // Delete any audiences that may already exist
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM audiences WHERE audienceName LIKE '__Auto Audience%'");
            if (reader.HasRows)
            {
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.DeleteAudience(), false)) { yield return r; }
            }
            reader.Close();

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAudiences.asmx/SaveAudience"));
            request4.Method = "POST";
            StringHttpBody request4Body = new StringHttpBody();
            request4Body.ContentType = "application/json; charset=utf-8";
            request4Body.InsertByteOrderMark = false;
            request4Body.BodyString = "{\"audienceID\":\"0\",\"audienceName\":\"__Auto Audience\",\"audienceDescription\":\"Automat" +
                "ically generated audience - do not edit\"}";
            request4.Body = request4Body;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request5.ThinkTime = 3;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 1;
            yield return request6;
            request6 = null;

            // extract the audience ID from the request below
            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminAudiences.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "__Auto Audience";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "audienceID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            request7.ThinkTime = 1;
            yield return request7;
            request7 = null;

            string audienceID = AutoTools.GetID(this.Context["audienceID"].ToString());

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAudience.aspx"));
            request8.ThinkTime = 3;
            request8.QueryStringParameters.Add("audienceid", audienceID, false, false);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAudience.aspx/CreateEmployeesModalGrid"));
            request9.ThinkTime = 5;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"contextKey\":\"xResetModalGridx\"}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            // Need to change SQL so that the Add and Edit tests select different parameters
            reader = database.GetReader("SELECT employeeid FROM employees WHERE username = 'james' " +
                "UNION SELECT TOP 1 budgetholderid FROM budgetholders WHERE budgetholder NOT LIKE '__Auto Budget Holder%' " +
                "UNION SELECT TOP 1 teamid FROM teams WHERE teamname NOT LIKE '__Auto Team%'");
            reader.Read();
            string budgetHolderID = reader.GetValue(0).ToString();
            reader.Read();
            string teamID = reader.GetValue(0).ToString();
            reader.Read();
            string employeeID = reader.GetValue(0).ToString();            
            reader.Close();

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAudience.aspx/CreateEmployeesModalGrid"));
            request10.ThinkTime = 2;
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"contextKey\":\"james\"}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAudiences.asmx/SaveAudienceEmployees"));
            request11.Method = "POST";
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json; charset=utf-8";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = "{\"audienceID\":\"" + audienceID + "\",\"employeesList\":[" + employeeID + "]}";
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAudience.aspx/CreateEmployeesGrid"));
            request12.ThinkTime = 1;
            request12.Method = "POST";
            StringHttpBody request12Body = new StringHttpBody();
            request12Body.ContentType = "application/json; charset=utf-8";
            request12Body.InsertByteOrderMark = false;
            request12Body.BodyString = "{\"contextKey\":" + audienceID + "}";
            request12.Body = request12Body;
            yield return request12;
            request12 = null;

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAudiences.asmx/SaveAudienceBudgetHolders"));
            request13.Method = "POST";
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=utf-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"audienceID\":\"" + audienceID + "\",\"budgetHoldersList\":[" + budgetHolderID + "]}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAudiences.asmx/SaveAudienceTeams"));
            request14.Method = "POST";
            StringHttpBody request14Body = new StringHttpBody();
            request14Body.ContentType = "application/json; charset=utf-8";
            request14Body.InsertByteOrderMark = false;
            request14Body.BodyString = "{\"audienceID\":\"" + audienceID + "\",\"teamsList\":[" + teamID + "]}";
            request14.Body = request14Body;
            yield return request14;
            request14 = null;

            // Validation
            WebTestRequest request16 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAudience.aspx"));
            request16.QueryStringParameters.Add("audienceid", audienceID, false, false);
            AutoTools.ValidateText("value=\"__Auto Audience\"", request16);
            AutoTools.ValidateText("class=\"row([1-2])\">james</td><td class=\"row([1-2])\">Mr</td><td class=\"row([1-2])\">James</td><td class=\"row([1-2])\">Lloyd</td>", request16, true, true);
            
            reader = database.GetReader("SELECT budgetholder, description FROM budgetholders WHERE budgetholderid = " + budgetHolderID);
            reader.Read();
            AutoTools.ValidateText(reader.GetValue(0).ToString(), request16);
            AutoTools.ValidateText(reader.GetValue(1).ToString(), request16);
            reader.Close();

            reader = database.GetReader("SELECT teamname, description FROM teams WHERE teamid = " + teamID);
            reader.Read();
            AutoTools.ValidateText(reader.GetValue(0).ToString(), request16);
            AutoTools.ValidateText(reader.GetValue(1).ToString(), request16);
            reader.Close();
            yield return request16;
            request16 = null;
        }
    }
}
