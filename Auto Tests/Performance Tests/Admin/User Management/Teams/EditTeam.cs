
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditTeam : WebTest
    {

        public EditTeam()
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

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminteams.aspx"));
            request6.ThinkTime = 1;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "__Auto Team";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "teamID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "accountid = ";
            extractionRule3.EndsWith = ";";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "accountID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            string teamID = AutoTools.GetID(this.Context["teamID"].ToString());
            string accountID = this.Context["accountID"].ToString();

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeteam.aspx"));
            request7.ThinkTime = 4;
            request7.QueryStringParameters.Add("teamid", teamID, false, false);
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "javascript";
            extractionRule4.EndsWith = "row[1-2]\">james";
            extractionRule4.IgnoreCase = false;
            extractionRule4.UseRegularExpression = true;
            extractionRule4.Required = true;
            extractionRule4.ExtractRandomMatch = false;
            extractionRule4.Index = 0;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "";
            extractionRule4.ContextParameterName = "jamesEmployeeID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            ExtractText extractionRule5 = new ExtractText();
            extractionRule5.StartsWith = "javascript";
            extractionRule5.EndsWith = "row[1-2]\">lynne";
            extractionRule5.IgnoreCase = false;
            extractionRule5.UseRegularExpression = true;
            extractionRule5.Required = true;
            extractionRule5.ExtractRandomMatch = false;
            extractionRule5.Index = 0;
            extractionRule5.HtmlDecode = true;
            extractionRule5.ContextParameterName = "";
            extractionRule5.ContextParameterName = "lynneEmployeeID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
            ExtractText extractionRule6 = new ExtractText();
            extractionRule6.StartsWith = "option";
            extractionRule6.EndsWith = "Lynne</option>";
            extractionRule6.IgnoreCase = false;
            extractionRule6.UseRegularExpression = true;
            extractionRule6.Required = true;
            extractionRule6.ExtractRandomMatch = false;
            extractionRule6.Index = 0;
            extractionRule6.HtmlDecode = true;
            extractionRule6.ContextParameterName = "";
            extractionRule6.ContextParameterName = "lynneTeamLeaderID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule6.Extract);
            yield return request7;
            request7 = null;

            string jamesEmployeeID = AutoTools.GetID(this.Context["jamesEmployeeID"].ToString());
            string lynneEmployeeID = AutoTools.GetID(this.Context["lynneEmployeeID"].ToString());
            string lynneTeamLeaderID = AutoTools.GetID(this.Context["lynneTeamLeaderID"].ToString(), "=", "\"", 2);

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/DeleteTeamEmployee"));
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"teamEmpID\":" + jamesEmployeeID + "}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/changePage"));
            request9.ThinkTime = 6;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = @"{""accountid"":" + accountID + @",""gridid"":""gridTeamMembers"",""pageNumber"":0,""filter"":"""",""gridDetails"":[""gridTeamMembers"",false,"""",true,""javascript:DeleteTeamEmployee({teamempid});"",true,20,true,false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[true,""4a17eac4-0634-44c0-a5b8-fc065921cae4"",[],0,"""",true,""6b1ef8d5-12ee-4e4a-9815-142aa739f508"",[],0,"""",true,""6dd9864e-df3a-4503-8999-5ad4b65b6c07"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,""""],""d99f21d9-64fa-4a4b-8db6-8f2a6df3b857"",false,"""","""",""Ascending"",""teamempid"",true,[""6b1ef8d5-12ee-4e4a-9815-142aa739f508"",1,[""188""],[]],[],false,""CheckBox"",false,""cGrid""]}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/SaveTeam"));
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"teamID\":" + teamID + ",\"teamName\":\"__Auto Team EDITED\",\"teamDescription\":\"Automatically generated" +
                " team - EDITED\",\"teamLeaderID\":" + lynneTeamLeaderID + "}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminteams.aspx"));
            request11.ThinkTime = 4;
            AutoTools.ValidateText("__Auto Team EDITED", request11);
            AutoTools.ValidateText("Automatically generated team - EDITED", request11);
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeteam.aspx"));
            request12.QueryStringParameters.Add("teamid", teamID, false, false);
            AutoTools.ValidateText("__Auto Team EDITED", request12);
            AutoTools.ValidateText("Automatically generated team - EDITED", request12);
            AutoTools.ValidateText("value=\"" + lynneTeamLeaderID + "\">Hunt, Mrs Lynne</option>", request12);
            AutoTools.ValidateText("lynne</td><td class=\"row[1-2]\">Mrs</td><td class=\"row[1-2]\">Lynne</td><td class=\"row[1-2]\">Hunt</td", request12, true, true);
            AutoTools.ValidateText("james</td><td class=\"row[1-2]\">Mr</td><td class=\"row[1-2]\">James</td><td class=\"row[1-2]\">Lloyd</td", request12, true, false);
            yield return request12;
            request12 = null;
        }
    }
}
