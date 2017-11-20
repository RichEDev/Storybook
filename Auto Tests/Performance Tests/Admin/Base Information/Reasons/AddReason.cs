
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddReason : WebTest
    {

        public AddReason()
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

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request3.ThinkTime = 1;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminreasons.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aereason.aspx"));
            request6.ThinkTime = 23;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/webservices/svcReasons.asmx/SaveReason"));
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"ReasonID\":0,\"ReasonName\":\"__James Auto Test\",\"Description\":\"Automatically " +
            "generated reason - do not edit\",\"CodeWithVAT\":\"JL06\",\"CodeWithoutVAT\":\"JL11\"}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminreasons.aspx"));
            yield return request8;
            request8 = null;

            // Connect to database to get ID after the department has been created
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM reasons WHERE reason LIKE '__James Auto Test%'");
            reader.Read();
            string reasonID = reader.GetValue(reader.GetOrdinal("reasonid")).ToString();
            reader.Close();

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aereason.aspx");
            validateRequest.QueryStringParameters.Add("reasonid", reasonID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated reason - do not edit";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                ValidationRuleFindText validationRule4 = new ValidationRuleFindText();
                validationRule4.FindText = "JL06";
                validationRule4.IgnoreCase = false;
                validationRule4.UseRegularExpression = false;
                validationRule4.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule4.Validate);

                ValidationRuleFindText validationRule5 = new ValidationRuleFindText();
                validationRule5.FindText = "JL11";
                validationRule5.IgnoreCase = false;
                validationRule5.UseRegularExpression = false;
                validationRule5.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule5.Validate);
            }
            yield return validateRequest;
            validateRequest = null;
        }
    }
}
