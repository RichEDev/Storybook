
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteUDF : WebTest
    {

        public DeleteUDF()
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

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request3.ThinkTime = 2;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/tailoringmenu.aspx"));
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            bool finished = false;
            string udfID = "";

            while (finished == false)
            {
                this.Context.Remove("newID");
                
                WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminuserdefined.aspx")); 
                ExtractText extractionRule2 = new ExtractText();
                extractionRule2.StartsWith = "javascript";
                extractionRule2.EndsWith = "Auto";
                extractionRule2.IgnoreCase = false;
                extractionRule2.UseRegularExpression = false;
                extractionRule2.Required = false;
                extractionRule2.ExtractRandomMatch = false;
                extractionRule2.Index = 0;
                extractionRule2.HtmlDecode = true;
                extractionRule2.ContextParameterName = "";
                extractionRule2.ContextParameterName = "newID";
                request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
                yield return request5;
                request5 = null;

                try
                {
                    udfID = AutoTools.GetID(this.Context["newID"].ToString());
                }
                catch (WebTestException)
                {
                    finished = true;
                }
                                            
                if (finished != true)
                {                   
                    WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminuserdefined.aspx/deleteUserDefined"));
                    request6.ThinkTime = 3;
                    request6.Method = "POST";
                    StringHttpBody request6Body = new StringHttpBody();
                    request6Body.ContentType = "application/json; charset=utf-8";
                    request6Body.InsertByteOrderMark = false;
                    request6Body.BodyString = "{\"userdefineid\":" + udfID + "}";
                    request6.Body = request6Body;
                    yield return request6;
                    request6 = null;
                }
            }
        }
    }
}
