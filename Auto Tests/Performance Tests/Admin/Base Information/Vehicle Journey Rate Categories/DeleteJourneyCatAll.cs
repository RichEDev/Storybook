
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteJourneyCatAll : WebTest
    {

        public DeleteJourneyCatAll()
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
            request3.RecordResult = false;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.RecordResult = false;
            yield return request4;
            request4 = null;

            string journeyCatID = "";

            while (journeyCatID != "finished")
            {

                WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminmileage.aspx"));
                request5.RecordResult = false;
                ExtractText extractionRule2 = new ExtractText();
                extractionRule2.StartsWith = "javascript";
                extractionRule2.EndsWith = "__Auto";
                extractionRule2.IgnoreCase = false;
                extractionRule2.UseRegularExpression = false;
                extractionRule2.Required = false;
                extractionRule2.ExtractRandomMatch = false;
                extractionRule2.Index = 0;
                extractionRule2.HtmlDecode = true;
                extractionRule2.ContextParameterName = "";
                extractionRule2.ContextParameterName = "NewID";
                request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
                yield return request5;
                request5 = null;


                WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminmileage.aspx/deleteMileageCategory"));

                try
                {
                    journeyCatID = AutoTools.GetID(this.Context["NewID"].ToString());
                    
                    request6.RecordResult = false;
                    request6.Method = "POST";
                    StringHttpBody request6Body = new StringHttpBody();
                    request6Body.ContentType = "application/json; charset=utf-8";
                    request6Body.InsertByteOrderMark = false;
                    request6Body.BodyString = "{\"mileageid\":" + journeyCatID + "}";
                    request6.Body = request6Body;                                                                                           
                }
                catch
                {
                    journeyCatID = "finished";
                }

                // To avoid the test incorrectly failing, only return the result if a VJR was found
                if (journeyCatID != "finished")
                {
                    yield return request6;
                    request6 = null;
                }

                this.Context.Remove("NewID");
            }
        }
    }
}
