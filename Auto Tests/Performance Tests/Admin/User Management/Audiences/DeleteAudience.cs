
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteAudience : WebTest
    {

        public DeleteAudience()
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
            request5.ThinkTime = 3;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 1;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminAudiences.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "__Auto Audience";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "audienceID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request7;
            request7 = null;

            string audienceID = AutoTools.GetID(this.Context["audienceID"].ToString());

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAudiences.asmx/DeleteAudience"));
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"audienceID\":" + audienceID + "}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminAudiences.aspx"));
            AutoTools.ValidateText("__Auto Audience", request9, false, false);
            AutoTools.ValidateText("Automatically generated audience", request9, false, false);
            yield return request9;
            request9 = null;
        }
    }
}
