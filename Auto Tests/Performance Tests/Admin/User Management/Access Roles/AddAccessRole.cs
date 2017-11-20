
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddAccessRole : WebTest
    {

        public AddAccessRole()
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
            request6.ThinkTime = 4;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accessRoles.aspx"));
            request7.ThinkTime = 2;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAccessRole.aspx"));
            request8.ThinkTime = 70;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAccessRoles.asmx/SaveAccessRoleElementAccess"));
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"accessRoleID\":\"0\",\"accessRoleName\":\"__Auto Access Role\",\"description\":\"Automati" +
                "cally generated access role - do not edit\",\"roleAccessLevel\":\"3\",\"elementDetails" +
                "\":[null,[true,true,true,true],[true,true,true,true],[true,true,true,true],[true," +
                "true,true,true],null,[true,true,true,true],[true,true,true,true],[true,true,true" +
                ",true],[true,null,null,true],[true,true,true,true],[true,true,true,true],[true,t" +
                "rue,true,true],[true,true,true,true],[true,true,true,true],[true,true,true,true]" +
                ",[true,true,true,true],[true,true,true,true],null,[true,true,true,true],[true,tr" +
                "ue,true,true],[true,true,true,true],[true,true,true,true],[true,true,true,true]," +
                "[true,true,true,true],[true,true,true,true],[true,true,true,true],[true,true,tru" +
                "e,true],[true,true,true,true],[true,true,true,true],[true,true,true,true],[true," +
                "true,true,true],[true,true,true,true],[true,true,true,true],[true,true,true,true" +
                "],[true,true,true,true],[true,true,true,true],[true,true,true,true],[true,true,t" +
                "rue,true],[true,true,true,true],[true,true,true,true],[true,true,true,true],[tru" +
                "e,true,true,true],[true,true,true,true],null,[true,true,true,true],[true,true,tr" +
                "ue,true],[true,true,true,true],[true,true,true,true],[true,true,true,true],[true" +
                ",true,true,true],[true,true,true,true],null,null,null,null,[true,true,true,true]" +
                ",[true],[true,true,true,true],null,null,null,[true,true,true,true],null,null,nul" +
                "l,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,nul" +
                "l,[true,null,true],[true,null,true],[true,null,true],[true,true,true,true],[true" +
                ",true,true,true],[true,null,true],[true],null,[true,null,true],null,[true,true,t" +
                "rue,true],null,null,[true,true,true,true],[true,true,true,true],null,null,null,[" +
                "true],[true,true,true,true],[true,true,true,true],null,[true,true,true,true],[tr" +
                "ue,true,true,true],null,[true,true,true,true],null,[true,true,true,true],[true,t" +
                "rue,true,true],[true,true,true,true],[true,true,true,true],[true,true,true,true]" +
                ",[true,true,true,true],[true,true,true,true],null,null,[true,true,true,true],[tr" +
                "ue,true,true,true],[true,true,true,true],[true],[true],[true],[true],[true],[tru" +
                "e],null,[true],[true,true,true,true],[true,true,true,true],[true,true,true,true]" +
                ",[true,true,true,true],[true,true,true,true],[true,true,true,true]],\"customEntit" +
                "yDetails\":[null,[null,[[false,false,false,false]],[null,[false,false,false,false" +
                "]],[null,[false,false,false,false]]]],\"maximumClaimAmount\":\"500\",\"minimumClaimA" +
                "mount\":\"2\",\"canAdjustCostCodes\":true,\"canAdjustDepartment\":true,\"canAdjustProj" +
                "ectCodes\":true,\"lstReportableAccessRoles\":[]}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accessRoles.aspx"));
            yield return request10;
            request10 = null;

            // Validation

            WebTestRequest request11 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/accessRoles.aspx");
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "__Auto Access Role";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "accessRoleID";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request11;
            request11 = null;
            string accessRoleID = AutoTools.GetID(this.Context["accessRoleID"].ToString());

            WebTestRequest request12 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aeAccessRole.aspx");
            request12.QueryStringParameters.Add("accessRoleID", accessRoleID, false, false);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "head id";
            extractionRule3.EndsWith = "/body";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "wholePage";
            request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request12;
            request12 = null;

            string wholePage = this.Context["wholePage"].ToString();

            int checkedBoxes = AutoTools.FindAll(wholePage, "checked=\\\"checked\\\"");

            WebTestRequest request13 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aeAccessRole.aspx");
            request13.QueryStringParameters.Add("accessRoleID", accessRoleID, false, false);

            if (checkedBoxes == 330)
            {
                AutoTools.ValidateText("checked=\"checked\"", request13);
            }
            else
            {
                AutoTools.ValidateText("All of the checkboxes have not remained ticked - fail test", request13);
            }

            AutoTools.ValidateText("__Auto Access Role", request13);

            AutoTools.ValidateText("Automatically generated access role - do not edit", request13);

            AutoTools.ValidateText("500.00", request13);

            AutoTools.ValidateText("2.00", request13);

            AutoTools.ValidateText("checked\" /> All data<", request13);

            yield return request13;
            request13 = null;
        }
    }
}
