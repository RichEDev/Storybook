
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddEmailSuffix : WebTest
    {

        public AddEmailSuffix()
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

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/tailoringmenu.aspx"));
            request6.ThinkTime = 1;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/emailsuffixes.aspx"));
            request7.ThinkTime = 1;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aeemailsuffix.aspx"));
            request8.ThinkTime = 6;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aeemailsuffix.aspx"));
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/emailsuffixes.aspx");
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtsuffix", "selauto.com");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "37");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "11");
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/emailsuffixes.aspx"));
            AutoTools.ValidateText("selauto.com</td>", request10);
            yield return request10;
            request10 = null;
        }
    }
}
