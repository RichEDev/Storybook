
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddPoolCar : WebTest
    {

        public AddPoolCar()
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

            // Add standard journey rate item if it does not exist
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM mileage_categories WHERE carsize = '__James Auto Test Standard'");
            if (reader.HasRows == false)
            {                
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddJourneyCatStandard(true), false)) { yield return r; }
                reader.Close();
                reader = database.GetReader("SELECT * FROM mileage_categories WHERE carsize = '__James Auto Test Standard'");
            }
            reader.Read();
            string journeyCatID = reader.GetValue(reader.GetOrdinal("mileageid")).ToString();
            reader.Close();

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }                      

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/poolcars.aspx"));
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aepoolcar.aspx"));
            request5.ThinkTime = 1;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/usercontrols/FileUploadIFrame.aspx"));
            request6.ThinkTime = 21;
            request6.QueryStringParameters.Add("tablename", "cars_attachments", false, false);
            request6.QueryStringParameters.Add("idfield", "id", false, false);
            request6.QueryStringParameters.Add("recordid", "0", false, false);
            request6.QueryStringParameters.Add("mdlcancel", "ctl00_contentmain_aeCar_cmdCarDocAttachCancel", false, false);
            request6.QueryStringParameters.Add("multipleAttachments", "0", false, false);
            yield return request6;
            request6 = null;

            cDatabaseConnection dbEmployee = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader readerEmployee = dbEmployee.GetReader("SELECT * FROM employees WHERE username = 'james'");
            readerEmployee.Read();
            string employeeID = readerEmployee.GetValue(readerEmployee.GetOrdinal("employeeid")).ToString();
            readerEmployee.Close();

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcCars.asmx/saveCar"));
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = @"{""carid"":0,""employeeid"":0,""startdate"":""01/01/2009"",""enddate"":""01/01/2013"",""make"":""James"",""model"":""Auto Test"",""registration"":""JAT 123"",""active"":true,""cartypeid"":1,""startodometer"":5000,""fuelcard"":true,""endodometer"":0,""taxexpiry"":""01/01/2012"",""taxlastchecked"":""01/01/2010"",""taxcheckedby"":""" + employeeID + @""",""mottestnumber"":""1234"",""motlastchecked"":""01/01/2010"",""motcheckedby"":""" + employeeID + @""",""motexpiry"":""01/01/2012"",""insurancenumber"":""5678"",""insuranceexpiry"":""01/01/2012"",""insurancelastchecked"":""01/01/2010"",""insurancecheckedby"":""" + employeeID + @""",""serviceexpiry"":""01/01/2012"",""servicelastchecked"":""01/01/2010"",""servicecheckedby"":""" + employeeID + @""",""defaultunit"":0,""enginesize"":""1200"",""mileagecats"":[" + journeyCatID + @"],""udfs"":[],""approved"":true,""exemptfromhometooffice"":false,""isAdmin"":true}";
            // NEED NEW UDF CODE FOR STRINGHTTPBODY
            request7.Body = request7Body;
            yield return request7;
            request7 = null;





            // Cannot add validation for any pages that run client-side javascript!!!
        }
    }
}
