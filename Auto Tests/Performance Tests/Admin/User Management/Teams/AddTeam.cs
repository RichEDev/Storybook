
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddTeam : WebTest
    {

        public AddTeam()
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
            request5.ThinkTime = 4;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 4;
            yield return request6;
            request6 = null;           

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminteams.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "var accountid = ";
            extractionRule2.EndsWith = ";";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "accountID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request7;
            request7 = null;

            string accountID = this.Context["accountID"].ToString();

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeteam.aspx"));
            request8.ThinkTime = 16;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/SaveTeam"));
            request9.ThinkTime = 5;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"teamID\":0,\"teamName\":\"__Auto Team\",\"teamDescription\":\"Automatically generated t" +
                "eam - do not edit\",\"teamLeaderID\":0}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT teamid FROM teams WHERE teamname = '__Auto Team' UNION " +
                "SELECT employeeid FROM employees WHERE username = 'james' UNION SELECT employeeid FROM employees WHERE username = 'lynne'");
            reader.Read();
            string teamID = reader.GetValue(0).ToString();
            reader.Read();
            string employeeID = reader.GetValue(0).ToString();
            reader.Read();
            string secondEmployeeID = reader.GetValue(0).ToString();
            reader.Close();

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/filterGrid"));
            request10.ThinkTime = 8;
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = @"{""accountid"":" + accountID + @",""gridid"":""gridEmployees"",""filter"":""james"",""gridDetails"":[""gridEmployees"",false,"""",false,"""",true,10,true,false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",false,"""","""",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[],""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",2,[""1""],[]],[],true,""CheckBox"",false,""cGrid""]}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/filterGrid"));
            request11.ThinkTime = 2;
            request11.Method = "POST";
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json; charset=utf-8";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = @"{""accountid"":" + accountID + @",""gridid"":""gridEmployees"",""filter"":""james"",""gridDetails"":[""gridEmployees"",false,"""",false,"""",true,10,true,false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",false,"""","""",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[],""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",2,[""1""],[]],[],true,""CheckBox"",false,""cGrid""]}";
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/SaveTeamEmps"));
            request12.Method = "POST";
            StringHttpBody request12Body = new StringHttpBody();
            request12Body.ContentType = "application/json; charset=utf-8";
            request12Body.InsertByteOrderMark = false;
            request12Body.BodyString = "{\"teamID\":" + teamID + ",\"employeeID\":[" + employeeID + "]}";
            request12.Body = request12Body;
            yield return request12;
            request12 = null;

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/CreateTeamEmpsGrid"));
            request13.ThinkTime = 8;
            request13.Method = "POST";
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=utf-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"teamID\":" + teamID + "}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/SaveTeamEmps"));
            request15.Method = "POST";
            StringHttpBody request15Body = new StringHttpBody();
            request15Body.ContentType = "application/json; charset=utf-8";
            request15Body.InsertByteOrderMark = false;
            request15Body.BodyString = "{\"teamID\":" + teamID + ",\"employeeID\":[" + secondEmployeeID + "]}";
            request15.Body = request15Body;
            yield return request15;
            request15 = null;

            WebTestRequest request16 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/CreateTeamEmpsGrid"));
            request16.ThinkTime = 6;
            request16.Method = "POST";
            StringHttpBody request16Body = new StringHttpBody();
            request16Body.ContentType = "application/json; charset=utf-8";
            request16Body.InsertByteOrderMark = false;
            request16Body.BodyString = "{\"teamID\":" + teamID + "}";
            request16.Body = request16Body;
            yield return request16;
            request16 = null;

            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/SaveTeam"));
            request17.Method = "POST";
            StringHttpBody request17Body = new StringHttpBody();
            request17Body.ContentType = "application/json; charset=utf-8";
            request17Body.InsertByteOrderMark = false;
            request17Body.BodyString = "{\"teamID\":" + teamID + ",\"teamName\":\"__Auto Team\",\"teamDescription\":\"Automatically generated" +
                " team - do not edit\",\"teamLeaderID\":" + secondEmployeeID + "}";
            request17.Body = request17Body;
            yield return request17;
            request17 = null;

            // Validation

            WebTestRequest request18 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminteams.aspx"));
            AutoTools.ValidateText("__Auto Team", request18);
            AutoTools.ValidateText("Automatically generated team - do not edit", request18);
            yield return request18;
            request18 = null;

            WebTestRequest request19 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeteam.aspx"));
            request19.QueryStringParameters.Add("teamid", teamID, false, false);
            AutoTools.ValidateText("__Auto Team", request19);
            AutoTools.ValidateText("Automatically generated team - do not edit", request19);
            AutoTools.ValidateText("<option selected=\"selected\" value=\"" + secondEmployeeID + "\">Lloyd, Mr James</option>", request19);

            yield return request19;
            request19 = null;
        }
    }
}
