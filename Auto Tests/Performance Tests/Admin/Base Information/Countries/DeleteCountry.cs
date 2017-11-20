
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class DeleteCountry : WebTest
    {

        public DeleteCountry()
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

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx"));
            request5.ThinkTime = 7;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "javascript";
            extractionRule2.EndsWith = "American Samoa";
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

            string extracted, id;
            extracted = this.Context["NewID"].ToString();
            id = AutoTools.GetID(extracted);

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx/deleteCountry"));
            request6.ThinkTime = 1;
            request6.Method = "POST";
            StringHttpBody request6Body = new StringHttpBody();
            request6Body.ContentType = "application/json; charset=utf-8";
            request6Body.InsertByteOrderMark = false;
            request6Body.BodyString = "{\"countryid\":" + id  + "}";
            request6.Body = request6Body;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/admincountries.aspx"));
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "American Samoa";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = false;
                request7.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request7;
            request7 = null;
        }
    }
}
