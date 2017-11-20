
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddP11DExpenseItem : WebTest
    {

        public AddP11DExpenseItem()
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
            request3.ThinkTime = 5;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
            request5.ThinkTime = 1;
            yield return request5;
            request5 = null;

            // Extract values for expense Cat
            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx"));            
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "Expense Category</label>";
            extractionRule2.EndsWith = "P11d Category</label>";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "ExpenseCatID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);                       
            request6.ThinkTime = 8;
            yield return request6;
            request6 = null;

            string expenseCatID = AutoTools.GetListValue(this.Context["ExpenseCatID"].ToString());

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx/saveSubcat"));
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = @"{""subcatid"":0,""subcat"":""__Auto P11D Standard Item"",""categoryid"":" + 
                expenseCatID + @",""accountcode"":""123"",""description"":""Automatically generated Standard " +
                @"expense item for P11D Cats"",""allowanceamount"":0,""addasnet"":false,""mileageapp"":false,""staffapp"":" +
                @"false,""othersapp"":false,""attendeesapp"":false,""pmilesapp"":false,""bmilesapp"":false,""tipapp"":false,""" +
                @"eventinhomeapp"":false,""passengersapp"":false,""nopassengersapp"":false,""splitentertainment"":false,""" +
                @"entertainmentid"":0,""pdcatid"":0,""reimbursable"":false,""nonights"":false,""hotelapp"":false,""" +
                @"comment"":""Auto generated Standard expense item "",""attendeesmand"":false,""nodirectorsapp"":false,""" + 
                @"alternateaccountcode"":"""",""hotelmand"":false,""nopersonalguests"":false,""noremoteworkers"":false,""" +
                @"splitpersonal"":false,""splitremote"":false,""reasonapp"":false,""otherdetails"":false,""personalid"":0,""" +
                @"remoteid"":0,""noroomsapp"":false,""vatnumber"":false,""vatnumbermand"":false,""fromapp"":false,""toapp"":false,""" + 
                @"companyapp"":false,""shortsubcat"":""Auto Standard"",""receipt"":false,""calculation"":1,""arrCountries"":[],""" + 
                @"allowances"":[],""associatedudfs"":[],""enableHomeToLocationMileage"":false,""hometolocationtype"":0,""" +
                @"mileageCategory"":null,""isRelocationMileage"":false,""reimbursableSubcatID"":null,""allowHeavyBulkyMileage"":false,""" +
                @"udfs"":[]}";

            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
            yield return request8;
            request8 = null;
        }
    }
}
