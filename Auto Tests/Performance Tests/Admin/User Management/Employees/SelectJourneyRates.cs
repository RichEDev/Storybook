
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class SelectJourneyRates : WebTest
    {

        public SelectJourneyRates()
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "var employeeid = ";
            extractionRule2.EndsWith = "\nvar appPath";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "employeeID";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            string employeeID = this.Context["employeeID"].ToString();

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 1;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request7.ThinkTime = 4;
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request7;
            request7 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeemployee.aspx"));            
            request9.QueryStringParameters.Add("employeeid", employeeID, false, false);
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "javascript";
            extractionRule4.EndsWith = "Nissan";
            extractionRule4.IgnoreCase = false;
            extractionRule4.UseRegularExpression = false;
            extractionRule4.Required = true;
            extractionRule4.ExtractRandomMatch = false;
            extractionRule4.Index = 0;
            extractionRule4.HtmlDecode = false;
            extractionRule4.ContextParameterName = "";
            extractionRule4.ContextParameterName = "carID";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request9;
            request9 = null;
           
            string carID = AutoTools.GetID(this.Context["carID"].ToString());

            // Extract all of the Auto VJRs from the database
            string journeyCats = "";
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("select mileageid, carsize from mileage_categories where carsize like '__Auto%'");

            while (reader.Read())
            {
                journeyCats = journeyCats + "," + reader.GetValue(0).ToString();
            }
            
            reader.Close();

            journeyCats = journeyCats.Remove(0, 1);            

            WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcCars.asmx/saveCar"));
            request14.Method = "POST";
            StringHttpBody request14Body = new StringHttpBody();
            request14Body.ContentType = "application/json; charset=utf-8";
            request14Body.InsertByteOrderMark = false;
            request14Body.BodyString = @"{""carid"":" + carID + @",""employeeid"":" + employeeID + @",""startdate"":""2009/01/01"",""enddate"":null,""make"":""Nissan"",""model"":""350z"",""registration"":""NZ09 666"",""active"":true,""cartypeid"":1,""startodometer"":0,""fuelcard"":true,""endodometer"":0,""taxexpiry"":null,""taxlastchecked"":null,""taxcheckedby"":0,""mottestnumber"":"""",""motlastchecked"":null,""motcheckedby"":0,""motexpiry"":null,""insurancenumber"":"""",""insuranceexpiry"":null,""insurancelastchecked"":null,""insurancecheckedby"":0,""serviceexpiry"":null,""servicelastchecked"":null,""servicecheckedby"":0,""defaultunit"":0,""enginesize"":""3500"",""mileagecats"":[" + journeyCats + @"],""udfs"":[],""approved"":true,""exemptfromhometooffice"":false,""isAdmin"":true}";
            request14.Body = request14Body;
            yield return request14;
            request14 = null;

            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            yield return request17;
            request17 = null;
        }
    }
}
