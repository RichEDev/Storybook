
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditReason : WebTest
    {

        public EditReason()
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
            request3.ThinkTime = 5;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminreasons.aspx"));
            request5.ThinkTime = 4;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "__James Auto Test";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "NewID";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            string reasonID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aereason.aspx"));
            request6.ThinkTime = 23;
            request6.QueryStringParameters.Add("reasonid", reasonID, false, false);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/webservices/svcReasons.asmx/SaveReason"));
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"ReasonID\":" + reasonID + ",\"ReasonName\":\"__James Auto Test EDITED\",\"Description\":\"Automatical" +
                "ly generated reason - EDITED\",\"CodeWithVAT\":\"Edited01\",\"CodeWithoutVAT\":\"Edited02\"}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminreasons.aspx"));
            request8.ThinkTime = 3;
            yield return request8;
            request8 = null;

            WebTestRequest validateRequest = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aereason.aspx");
            validateRequest.QueryStringParameters.Add("reasonid", reasonID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated reason - EDITED";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                ValidationRuleFindText validationRule4 = new ValidationRuleFindText();
                validationRule4.FindText = "Edited01";
                validationRule4.IgnoreCase = false;
                validationRule4.UseRegularExpression = false;
                validationRule4.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule4.Validate);

                ValidationRuleFindText validationRule5 = new ValidationRuleFindText();
                validationRule5.FindText = "Edited02";
                validationRule5.IgnoreCase = false;
                validationRule5.UseRegularExpression = false;
                validationRule5.PassIfTextFound = true;
                validateRequest.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule5.Validate);
            }
            yield return validateRequest;
            validateRequest = null;        
        }
    }
}
