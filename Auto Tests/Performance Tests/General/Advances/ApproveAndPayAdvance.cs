
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class ApproveAndPayAdvance : WebTest
    {

        public ApproveAndPayAdvance()
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminfloats.aspx"));                      

            request5.ThinkTime = 10;
            yield return request5;
            request5 = null;
            

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminfloats.aspx"));
            request6.ThinkTime = 8;
            request6.QueryStringParameters.Add("action", "4", false, false);
            request6.QueryStringParameters.Add("floatid", "696", false, false);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminfloats.aspx"));
            request7.QueryStringParameters.Add("action", "5", false, false);
            request7.QueryStringParameters.Add("floatid", "696", false, false);
            yield return request7;
            request7 = null;
        }
    }
}
