
using System.Xml.Schema;

namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddEveryItem : WebTest
    {

        public AddEveryItem()
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
            request5.ThinkTime = 7;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request6.ThinkTime = 4;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
            request7.ThinkTime = 2;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx"));
            request8.ThinkTime = 72;
            yield return request8;
            request8 = null;

            string subcatID = string.Empty;

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM ExpenseItems");
            while (reader.Read())
            {

                // Create expense item

                WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx/saveSubcat"));
                request9.ThinkTime = 11;
                request9.Method = "POST";
                StringHttpBody request9Body = new StringHttpBody();
                request9Body.ContentType = "application/json; charset=utf-8";
                request9Body.InsertByteOrderMark = false;
                request9Body.BodyString = @"{""subcatid"":0,""subcat"":""" + reader.GetValue(0).ToString() + @"""," +
                        @"""categoryid"":319,""accountcode"":""" + reader.GetValue(2).ToString() + @"""," +
                        @"""description"":""" + reader.GetValue(3).ToString() + @""",""allowanceamount"":" + reader.GetValue(4).ToString() + @"," +
                        @"""addasnet"":" + reader.GetValue(5).ToString() + @",""mileageapp"":" + reader.GetValue(6).ToString() + @"," +
                        @"""staffapp"":" + reader.GetValue(7).ToString() + @",""othersapp"":" + reader.GetValue(8).ToString() + @"," +
                        @"""attendeesapp"":" + reader.GetValue(9).ToString() + @",""pmilesapp"":" + reader.GetValue(10).ToString() + @"," +
                        @"""bmilesapp"":" + reader.GetValue(11).ToString() + @",""tipapp"":" + reader.GetValue(12).ToString() + @"," +
                        @"""eventinhomeapp"":" + reader.GetValue(13).ToString() + @",""passengersapp"":" + reader.GetValue(14).ToString() + @"," +
                        @"""nopassengersapp"":" + reader.GetValue(15).ToString() + @",""splitentertainment"":" + reader.GetValue(16).ToString() + @"," +
                        @"""entertainmentid"":" + reader.GetValue(17).ToString() + @",""pdcatid"":207," +
                        @"""reimbursable"":" + reader.GetValue(19).ToString() + @",""nonights"":" + reader.GetValue(20).ToString() + @"," +
                        @"""hotelapp"":" + reader.GetValue(21).ToString() + @",""comment"":""" + reader.GetValue(22).ToString() + @"""," +
                        @"""attendeesmand"":" + reader.GetValue(23).ToString() + @",""nodirectorsapp"":" + reader.GetValue(24).ToString() + @"," +
                        @"""alternateaccountcode"":""" + reader.GetValue(25).ToString() + @""",""hotelmand"":" + reader.GetValue(26).ToString() + @"," +
                        @"""nopersonalguests"":" + reader.GetValue(27).ToString() + @",""noremoteworkers"":" + reader.GetValue(28).ToString() + @"," +
                        @"""splitpersonal"":" + reader.GetValue(29).ToString() + @",""splitremote"":" + reader.GetValue(30).ToString() + @"," +
                        @"""reasonapp"":" + reader.GetValue(31).ToString() + @",""otherdetails"":" + reader.GetValue(32).ToString() + @"," +
                        @"""personalid"":" + reader.GetValue(33).ToString() + @",""remoteid"":" + reader.GetValue(34).ToString() + @"," +
                        @"""noroomsapp"":" + reader.GetValue(35).ToString() + @",""vatnumber"":" + reader.GetValue(36).ToString() + @"," +
                        @"""vatnumbermand"":" + reader.GetValue(37).ToString() + @",""fromapp"":" + reader.GetValue(38).ToString() + @"," +
                        @"""toapp"":" + reader.GetValue(39).ToString() + @",""companyapp"":" + reader.GetValue(40).ToString() + @"," +
                        @"""shortsubcat"":" + reader.GetValue(41).ToString() + @",""receipt"":" + reader.GetValue(42).ToString() + @"," +
                        @"""calculation"":" + reader.GetValue(43).ToString() + @",""arrCountries"":[],""allowances"":[]," +
                        @"""associatedudfs"":[],""enableHomeToLocationMileage"":" + reader.GetValue(47).ToString() + @"," +
                        @"""hometolocationtype"":" + reader.GetValue(48).ToString() + @",""mileageCategory"":" + reader.GetValue(49).ToString() + @"," +
                        @"""isRelocationMileage"":" + reader.GetValue(50).ToString() + @",""reimbursableSubcatID"":" + reader.GetValue(51).ToString() + @"," +
                        @"""allowHeavyBulkyMileage"":" + reader.GetValue(52).ToString() + @",""udfs"":[]}";
                request9.Body = request9Body;
                yield return request9;
                request9 = null;

                // Extract the ID of the expense item

                WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminsubcats.aspx"));
                request10.ThinkTime = 10;
                ExtractText extractionRule2 = new ExtractText();
                extractionRule2.StartsWith = "javascript";
                extractionRule2.EndsWith = reader.GetValue(0).ToString();
                extractionRule2.IgnoreCase = false;
                extractionRule2.UseRegularExpression = false;
                extractionRule2.Required = true;
                extractionRule2.ExtractRandomMatch = false;
                extractionRule2.Index = 0;
                extractionRule2.HtmlDecode = false;
                extractionRule2.ContextParameterName = "";
                extractionRule2.ContextParameterName = "subcatID";
                request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
                yield return request10;
                request10 = null;

                subcatID = AutoTools.GetID(this.Context["subcatID"].ToString());

                // Add item role ID to the expense item

                WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aesubcat.aspx/saveRole"));
                request11.Method = "POST";
                StringHttpBody request11Body = new StringHttpBody();
                request11Body.ContentType = "application/json; charset=utf-8";
                request11Body.InsertByteOrderMark = false;
                request11Body.BodyString = "{\"subcatid\":" + subcatID + ",\"roleid\":\"4\",\"addtotemplate\":false,\"receiptmaximum\":0,\"maximum\":500}";
                request11.Body = request11Body;
                yield return request11;
                request11 = null;
            }
            reader.Close();
        }
    }
}
