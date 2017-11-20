
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class CheckRecommendedDistance : WebTest
    {

        public CheckRecommendedDistance()
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

            // Get carID
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader(@"SELECT carid FROM cars 
                                                        INNER JOIN employees ON cars.employeeid = employees.employeeid 
                                                        WHERE employees.username = 'james' AND model = '350z'");
            string carID = "";
            if (reader.HasRows)
            {
                reader.Read();
                carID = reader.GetValue(0).ToString();
            }
            reader.Close();            


            string expectedValue = ",44";

            for(int x = 1; x < 3; x++)
            {
                // Set the calculation to either shortest or quickest (1 or 2)
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit("tabsNewExpenses$tabmileage$cmbmileagecalculationtype", x.ToString(), "list"), false)) { yield return r; }

                WebTestRequest request13 = new WebTestRequest((AutoTools.ServerToUse() + "/shared/webServices/svcAutocomplete.asmx/getUsualMileage"));
                request13.Method = "POST";
                StringHttpBody request13Body = new StringHttpBody();
                request13Body.ContentType = "application/json; charset=utf-8";
                request13Body.InsertByteOrderMark = false;
                request13Body.BodyString = "{\"from\":\"__Auto Recommended Test 1\",\"to\":\"__Auto Recommended Test 2\",\"fromnameid\"" +
                    ":\"ctl00_contentmain_txtfrom0_0\",\"sCarid\":\"" + carID + "\",\"date\":\"2010/02/02\"}";
                request13.Body = request13Body;
                if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                {
                    ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                    validationRule2.FindText = expectedValue;
                    validationRule2.IgnoreCase = false;
                    validationRule2.UseRegularExpression = false;
                    validationRule2.PassIfTextFound = true;
                    request13.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                }
                yield return request13;
                request13 = null;

                expectedValue = ",45.2";
            }
        }
    }
}
