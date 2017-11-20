
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddExpenseItemMileage : WebTest
    {

        public AddExpenseItemMileage()
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

            // Check if the expense item already exists in the database
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("select * from subcats where subcat = '__AutoMileageItem'");
            bool alreadyExists = reader.HasRows;
            reader.Close();
            if (alreadyExists == false)
            {


                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

                WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx"));
                // Extract the Expense Category ID
                ExtractText extractionRule2 = new ExtractText();
                extractionRule2.StartsWith = "option value";
                extractionRule2.EndsWith = "Travel</option>";
                extractionRule2.Required = true;
                extractionRule2.HtmlDecode = false;
                extractionRule2.ContextParameterName = "";
                extractionRule2.ContextParameterName = "categoryID";
                request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
                // Extract the Item Role ID
                ExtractText extractionRule3 = new ExtractText();
                extractionRule3.StartsWith = "option value";
                extractionRule3.EndsWith = "Claimant</option>";
                extractionRule3.Required = true;
                extractionRule3.HtmlDecode = false;
                extractionRule3.ContextParameterName = "";
                extractionRule3.ContextParameterName = "itemroleID";
                request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);

                // Check database for P11D cat
                System.Data.SqlClient.SqlDataReader pdcatCheck = database.GetReader("select * from pdcats where pdname = '__James Auto Test'");
                if (pdcatCheck.HasRows == false)
                {
                    // Create P11D cat if it does not exist
                    foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddP11DCategory(), false)) { yield return r; }
                }
                pdcatCheck.Close();

                // Extract P11D Cat ID
                ExtractText extractionRule4 = new ExtractText();
                extractionRule4.StartsWith = "javascript";
                extractionRule4.EndsWith = "__James Auto Test";
                extractionRule4.Required = true;
                extractionRule4.HtmlDecode = false;
                extractionRule4.ContextParameterName = "";
                extractionRule4.ContextParameterName = "pdcatID";
                request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);                

                yield return request6;
                request6 = null;

                string categoryID = AutoTools.GetID(this.Context["categoryID"].ToString(), "=", "\"", 2);
                string itemroleID = AutoTools.GetID(this.Context["itemroleID"].ToString(), "=", "\"", 2);
                string pdcatID = AutoTools.GetID(this.Context["pdcatID"].ToString(), "=", "\"", 2);

                WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx/saveSubcat"));
                request7.ThinkTime = 5;
                request7.Method = "POST";
                StringHttpBody request7Body = new StringHttpBody();
                request7Body.ContentType = "application/json; charset=utf-8";
                request7Body.InsertByteOrderMark = false;
                request7Body.BodyString = @"{""subcatid"":0,""subcat"":""__AutoMileageItem"",""categoryid"":" + categoryID + @",""accountcode"":""Auto123"",""description"":""Automatically generated Pence Per Mile expense item"",""allowanceamount"":0,""addasnet"":false,""mileageapp"":true,""staffapp"":false,""othersapp"":false,""attendeesapp"":false,""pmilesapp"":false,""bmilesapp"":false,""tipapp"":false,""eventinhomeapp"":false,""passengersapp"":true,""nopassengersapp"":true,""splitentertainment"":false,""entertainmentid"":0,""pdcatid"":" + pdcatID + @",""reimbursable"":false,""nonights"":false,""hotelapp"":false,""comment"":""Automatically generated Pence Per Mile expense item"",""attendeesmand"":false,""nodirectorsapp"":false,""alternateaccountcode"":"""",""hotelmand"":false,""nopersonalguests"":false,""noremoteworkers"":false,""splitpersonal"":false,""splitremote"":false,""reasonapp"":false,""otherdetails"":false,""personalid"":0,""remoteid"":0,""noroomsapp"":false,""vatnumber"":false,""vatnumbermand"":false,""fromapp"":false,""toapp"":false,""companyapp"":false,""shortsubcat"":""AutoMileage"",""receipt"":false,""calculation"":3,""arrCountries"":[],""allowances"":[],""associatedudfs"":[],""enableHomeToLocationMileage"":false,""hometolocationtype"":0,""mileageCategory"":null,""isRelocationMileage"":false,""reimbursableSubcatID"":null,""allowHeavyBulkyMileage"":true,""udfs"":[]}";
                request7.Body = request7Body;
                yield return request7;
                request7 = null;

                // Extract the subcatID
                WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
                ExtractText extractionRule5 = new ExtractText();
                extractionRule5.StartsWith = "javascript";
                extractionRule5.EndsWith = "__AutoMileageItem</td><";
                extractionRule5.Required = true;
                extractionRule5.HtmlDecode = false;
                extractionRule5.ContextParameterName = "";
                extractionRule5.ContextParameterName = "subcatID";
                request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
                yield return request8;
                request8 = null;

                string subcatID = AutoTools.GetID(this.Context["subcatID"].ToString());

                WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx/saveRole"));
                request9.Method = "POST";
                StringHttpBody request9Body = new StringHttpBody();
                request9Body.ContentType = "application/json; charset=utf-8";
                request9Body.InsertByteOrderMark = false;
                request9Body.BodyString = "{\"subcatid\":" + subcatID + ",\"roleid\":\"" + itemroleID + "\",\"addtotemplate\":false,\"receiptmaximum\":0,\"maximum\":0}";
                request9.Body = request9Body;
                yield return request9;
                request9 = null;

                WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
                yield return request11;
                request11 = null;

                // Add validation
            }

        }
    }
}
