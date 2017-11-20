
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteTeam : WebTest
    {

        public DeleteTeam()
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
            request5.ThinkTime = 1;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 2;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminteams.aspx"));
            request7.ThinkTime = 3;
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
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "accountid =";
            extractionRule3.EndsWith = ";";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "accountID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request7;
            request7 = null;

            string teamID = AutoTools.GetID(this.Context["teamID"].ToString());
            string accountID = this.Context["accountID"].ToString();

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcTeams.asmx/DeleteTeam"));
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"teamID\":" + teamID + "}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/changePage"));
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = @"{""accountid"":" + accountID + @",""gridid"":""gridTeams"",""pageNumber"":0,""filter"":"""",""gridDetails"":[""gridTeams"",true,""aeteam.aspx?teamid={teamid}"",true,""javascript:DeleteTeam({teamid});"",true,20,true,false,""1422263f-882e-4b8d-bd10-b6a4e9267e61"",[true,""b60a43d6-1b0a-4f11-95ce-3a9d0c10f6ce"",[],0,"""",false,""1422263f-882e-4b8d-bd10-b6a4e9267e61"",[],0,"""",false,""cde72fb1-e31a-412e-87d7-83d841209a92"",[],0,""""],""fa495951-4d06-49ad-9f85-d67f9eff4a27"",false,"""","""",""Ascending"",""teamid"",true,[],[""Members"","""",""/shared/images/icons/users3.png"",""javascript:ShowTeamMembers({teamid});"",""View Team Members""],false,""CheckBox"",false,""cGrid""]}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminteams.aspx"));
            AutoTools.ValidateText("__Auto Team", request10, false, false);
            yield return request10;
            request10 = null;
        }
    }
}
