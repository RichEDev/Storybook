
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class Logon : WebTest
    {

        public Logon(UserType userType)
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.Context.Add("Company", AutoTools.UserLevel(userType)[0]);
            this.Context.Add("Username", AutoTools.UserLevel(userType)[1]);
            this.Context.Add("Password", AutoTools.UserLevel(userType)[2]);
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

            WebTestRequest request2 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request2.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request2.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request2.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request2;
            request2 = null;

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request3.Method = "POST";
            request3.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/home.aspx");
            FormPostHttpBody request3Body = new FormPostHttpBody();
            request3Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request3Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request3Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request3Body.FormPostParameters.Add("txtCompanyID", this.Context["Company"].ToString());
            request3Body.FormPostParameters.Add("txtUsername", this.Context["Username"].ToString());
            request3Body.FormPostParameters.Add("txtPassword", this.Context["Password"].ToString());
            request3Body.FormPostParameters.Add("btnLogon.x", "32");
            request3Body.FormPostParameters.Add("btnLogon.y", "17");
            request3.Body = request3Body;
            this.Context.Remove("Company");
            this.Context.Remove("Username");
            this.Context.Remove("Password");
            yield return request3;
            request3 = null;            
        }
    }
}
