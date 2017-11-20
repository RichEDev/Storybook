
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

    public class EditAllowance : WebTest
    {

        public EditAllowance()
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

            WebTestRequest request3 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/adminmenu.aspx");
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/categorymenu.aspx");
            request4.ThinkTime = 3;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminallowances.aspx");
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

            string allowanceID = AutoTools.GetID(this.Context["NewID"].ToString());            

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx");
            request6.ThinkTime = 18;
            request6.QueryStringParameters.Add("allowanceid", allowanceID, false, false);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "option ";
            extractionRule3.EndsWith = "US Dollar";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "CurrencyID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            string currencyID = AutoTools.GetID(this.Context["CurrencyID"].ToString(), "=", "\"", 2);           

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveAllowance");
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"allowanceID\":" + allowanceID + ",\"name\":\"__James Auto Test EDITED\",\"description\":\"Automatically ge" +
                "nerated allowance - EDITED\",\"numhours\":50,\"rate\":50,\"currencyid\":" + currencyID + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminallowances.aspx");
            request8.ThinkTime = 2;
            yield return request8;
            request8 = null;

            WebTestRequest request10 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx");
            request10.QueryStringParameters.Add("allowanceid", allowanceID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "option selected=\"selected\" value=\"" + currencyID + "\">US Dollar";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                ValidationRuleFindText validationRule4 = new ValidationRuleFindText();
                validationRule4.FindText = "Automatically generated allowance - EDITED";
                validationRule4.IgnoreCase = false;
                validationRule4.UseRegularExpression = false;
                validationRule4.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule4.Validate);

                ValidationRuleFindText validationRule5 = new ValidationRuleFindText();
                validationRule5.FindText = "input name=\"ctl00$contentmain$txtnighthours\" type=\"text\" value=\"50\"";
                validationRule5.IgnoreCase = false;
                validationRule5.UseRegularExpression = false;
                validationRule5.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule5.Validate);

                ValidationRuleFindText validationRule6 = new ValidationRuleFindText();
                validationRule6.FindText = "input name=\"ctl00$contentmain$txtnightrate\" type=\"text\" value=\"50.00";
                validationRule6.IgnoreCase = false;
                validationRule6.UseRegularExpression = false;
                validationRule6.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule6.Validate);

                ValidationRuleFindText validationRule7 = new ValidationRuleFindText();
                validationRule7.FindText = ">12</td><td class=\"row([1-2])\" align=\"right\">25.00";
                validationRule7.IgnoreCase = false;
                validationRule7.UseRegularExpression = true;
                validationRule7.PassIfTextFound = true;
                request10.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule7.Validate);
            }
            yield return request10;
            request10 = null;
        }
    }
}
