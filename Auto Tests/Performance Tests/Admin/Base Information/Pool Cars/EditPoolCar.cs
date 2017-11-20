
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditPoolCar : WebTest
    {

        public EditPoolCar()
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

            // Add standard kilometre journey rate item if it does not exist
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM mileage_categories WHERE carsize = '__James Auto Test Standard Kilometre'");
            if (reader.HasRows == false)
            {
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddJourneyCatStandard(false), false)) { yield return r; }
                reader.Close();
                reader = database.GetReader("SELECT * FROM mileage_categories WHERE carsize = '__James Auto Test Standard Kilometre'");
            }
            reader.Read();
            string journeyrateID = reader.GetValue(reader.GetOrdinal("mileageid")).ToString();
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/poolcars.aspx"));
            request5.ThinkTime = 2;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "James</td>";
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

            string id = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aepoolcar.aspx"));
            request6.ThinkTime = 1;
            request6.QueryStringParameters.Add("action", "2", false, false);
            request6.QueryStringParameters.Add("carid", id, false, false);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/usercontrols/FileUploadIFrame.aspx"));
            request7.ThinkTime = 1;
            request7.QueryStringParameters.Add("tablename", "cars_attachments", false, false);
            request7.QueryStringParameters.Add("idfield", "id", false, false);
            request7.QueryStringParameters.Add("recordid", id, false, false);
            request7.QueryStringParameters.Add("mdlcancel", "ctl00_contentmain_aeCar_cmdCarDocAttachCancel", false, false);
            request7.QueryStringParameters.Add("multipleAttachments", "0", false, false);
            yield return request7;
            request7 = null;

            // Get the ID of another employee
            cDatabaseConnection dbEmployee = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader EmpReader = dbEmployee.GetReader("SELECT * FROM employees WHERE username = 'lynne'");
            EmpReader.Read();
            string employeeID = EmpReader.GetValue(EmpReader.GetOrdinal("employeeid")).ToString();
            EmpReader.Close();

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcCars.asmx/saveCar"));
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = @"{""carid"":" + id + @",""employeeid"":0,""startdate"":""06/06/2006"",""enddate"":""12/12/2012"",""make"":""James Edited"",""model"":""Auto Test Edited"",""registration"":""EDITED"",""active"":true,""cartypeid"":2,""startodometer"":10000,""fuelcard"":true,""endodometer"":90000,""taxexpiry"":""06/06/2006"",""taxlastchecked"":""06/06/2006"",""taxcheckedby"":" + employeeID + @",""mottestnumber"":""6666"",""motlastchecked"":""01/01/2001"",""motcheckedby"":" + employeeID + @",""motexpiry"":""02/02/2002"",""insurancenumber"":""7777"",""insuranceexpiry"":""03/03/2003"",""insurancelastchecked"":""04/04/2004"",""insurancecheckedby"":" + employeeID + @",""serviceexpiry"":""05/05/2005"",""servicelastchecked"":""06/06/2006"",""servicecheckedby"":" + employeeID + @",""defaultunit"":1,""enginesize"":""3500"",""mileagecats"":[" + journeyrateID + @"],""udfs"":[],""approved"":false,""exemptfromhometooffice"":true,""isAdmin"":true}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/poolcars.aspx"));
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "Auto Test Edited";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request11.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request11;
            request11 = null;
        }
    }
}
