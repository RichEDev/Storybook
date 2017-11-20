
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditAccessRole : WebTest
    {

        public EditAccessRole()
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

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accessRoles.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "deleteAccessRole";
            extractionRule2.EndsWith = "__Auto Access Role";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "accessRoleID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request7;
            request7 = null;

            string accessRoleID = AutoTools.GetID(this.Context["accessRoleID"].ToString());

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeAccessRole.aspx"));
            request8.ThinkTime = 106;
            request8.QueryStringParameters.Add("accessRoleID", accessRoleID, false, false);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAccessRoles.asmx/SaveAccessRoleElementAccess"));
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"accessRoleID\":\"" + accessRoleID + "\",\"accessRoleName\":\"__Auto Access Role EDITED\",\"description\"" +
                ":\"Automatically generated access role - Edited\",\"roleAccessLevel\":\"1\",\"elementDe" +
                "tails\":[null,[false,false,false,false],[false,false,false,false],[false,false,fa" +
                "lse,false],[false,false,false,false],null,[false,false,false,false],[false,false" +
                ",false,false],[false,false,false,false],[false,null,null,false],[false,false,fal" +
                "se,false],[false,false,false,false],[false,false,false,false],[false,false,false" +
                ",false],[false,false,false,false],[false,false,false,false],[false,false,false,f" +
                "alse],[false,false,false,false],null,[false,false,false,false],[false,false,fals" +
                "e,false],[false,false,false,false],[false,false,false,false],[false,false,false," +
                "false],[false,false,false,false],[false,false,false,false],[false,false,false,fa" +
                "lse],[false,false,false,false],[false,false,false,false],[false,false,false,fals" +
                "e],[false,false,false,false],[false,false,false,false],[false,false,false,false]" +
                ",[false,false,false,false],[false,false,false,false],[false,false,false,false],[" +
                "false,false,false,false],[false,false,false,false],[false,false,false,false],[fa" +
                "lse,false,false,false],[false,false,false,false],[false,false,false,false],[fals" +
                "e,false,false,false],[false,false,false,false],null,[false,false,false,false],[f" +
                "alse,false,false,false],[false,false,false,false],[false,false,false,false],[fal" +
                "se,false,false,false],[false,false,false,false],[false,false,false,false],null,n" +
                "ull,null,null,[false,false,false,false],[false],[false,false,false,false],null,n" +
                "ull,null,[false,false,false,false],null,null,null,null,null,null,null,null,null," +
                "null,null,null,null,null,null,null,null,null,null,[false,null,false],[false,null" +
                ",false],[false,null,false],[false,false,false,false],[false,false,false,false],[" +
                "false,null,false],[false],null,[false,null,false],null,[false,false,false,false]" +
                ",null,null,[false,false,false,false],[false,false,false,false],null,null,null,[f" +
                "alse],[false,false,false,false],[false,false,false,false],null,[false,false,fals" +
                "e,false],[false,false,false,false],null,[false,false,false,false],null,[false,fa" +
                "lse,false,false],[false,false,false,false],[false,false,false,false],[false,fals" +
                "e,false,false],[false,false,false,false],[false,false,false,false],[false,false," +
                "false,false],null,null,[false,false,false,false],[false,false,false,false],[fals" +
                "e,false,false,false],[false],[false],[false],[false],[false],[false],null,[false" +
                "],[false,false,false,false],[false,false,false,false],[false,false,false,false]," +
                "[false,false,false,false],[false,false,false,false],[false,false,false,false]],\"" +
                "customEntityDetails\":[null,[null,[[false,false,false,false]],[null,[false,false," +
                "false,false]],[null,[false,false,false,false]]]],\"maximumClaimAmount\":\"750.00\",\"" +
                "minimumClaimAmount\":\"5.00\",\"canAdjustCostCodes\":false,\"canAdjustDepartment\":fals" +
                "e,\"canAdjustProjectCodes\":false,\"lstReportableAccessRoles\":[]}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/accessRoles.aspx"));
            yield return request10;
            request10 = null;

            // Validation

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

            if (checkedBoxes == 1)
            {
                AutoTools.ValidateText("checked=\"checked\"", request13);
            }
            else
            {
                AutoTools.ValidateText("All of the checkboxes have not remained ticked - fail test", request13);
            }

            AutoTools.ValidateText("__Auto Access Role EDITED", request13);
            AutoTools.ValidateText("Automatically generated access role - Edited", request13);
            AutoTools.ValidateText("750.00", request13);
            AutoTools.ValidateText("5.00", request13);
            AutoTools.ValidateText("checked\" /> Data from employees they approve<", request13);
        }
    }
}
