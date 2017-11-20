
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditAllowanceRate : WebTest
    {

        public EditAllowanceRate()
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
            request4.ThinkTime = 2;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminallowances.aspx");
            request5.ThinkTime = 2;
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
            request6.ThinkTime = 3;
            request6.QueryStringParameters.Add("allowanceid", allowanceID, false, false);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "option ";
            extractionRule3.EndsWith = "Pound Sterling";
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
            request7.ThinkTime = 8;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"allowanceID\":" + allowanceID + ",\"name\":\"__James Auto Test EDITED\",\"description\":\"Automatically g" +
                "enerated allowance - EDITED\",\"numhours\":20,\"rate\":40,\"currencyid\":" + allowanceID + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveRate");
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = "{\"rateID\":0,\"allowanceID\":" + allowanceID + ",\"hours\":24,\"rate\":50}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/createRatesGrid");
            request9.ThinkTime = 4;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"contextKey\":" + allowanceID + "}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveAllowance");
            request10.ThinkTime = 6;
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"allowanceID\":" + allowanceID + ",\"name\":\"__James Auto Test EDITED\",\"description\":\"Automatically g" +
                "enerated allowance - EDITED\",\"numhours\":20,\"rate\":40,\"currencyid\":" + allowanceID + "}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveRate");
            request11.Method = "POST";
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json; charset=utf-8";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = "{\"rateID\":0,\"allowanceID\":" + allowanceID + ",\"hours\":48,\"rate\":70}";
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/createRatesGrid");
            request12.ThinkTime = 3;
            request12.Method = "POST";
            StringHttpBody request12Body = new StringHttpBody();
            request12Body.ContentType = "application/json; charset=utf-8";
            request12Body.InsertByteOrderMark = false;
            request12Body.BodyString = "{\"contextKey\":" + allowanceID + "}";
            request12.Body = request12Body;
            yield return request12;
            request12 = null;

            WebTestRequest request13 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx/saveAllowance");
            request13.Method = "POST";
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=utf-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"allowanceID\":" + allowanceID + ",\"name\":\"__James Auto Test EDITED\",\"description\":\"Automatically g" +
                "enerated allowance - EDITED\",\"numhours\":20,\"rate\":40,\"currencyid\":" + currencyID + "}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            WebTestRequest request14 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminallowances.aspx");
            request14.ThinkTime = 3;
            yield return request14;
            request14 = null;

            WebTestRequest request15 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/aeallowance.aspx");
            request15.QueryStringParameters.Add("allowanceid", allowanceID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {

                // THESE NEED UPDATING


                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "option selected=\"selected\" value=\"" + currencyID + "\">Pound Sterling";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);

                ValidationRuleFindText validationRule4 = new ValidationRuleFindText();
                validationRule4.FindText = "Automatically generated allowance - EDITED";
                validationRule4.IgnoreCase = false;
                validationRule4.UseRegularExpression = false;
                validationRule4.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule4.Validate);

                ValidationRuleFindText validationRule5 = new ValidationRuleFindText();
                validationRule5.FindText = "input name=\"ctl00$contentmain$txtnighthours\" type=\"text\" value=\"20\"";
                validationRule5.IgnoreCase = false;
                validationRule5.UseRegularExpression = false;
                validationRule5.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule5.Validate);

                ValidationRuleFindText validationRule6 = new ValidationRuleFindText();
                validationRule6.FindText = "input name=\"ctl00$contentmain$txtnightrate\" type=\"text\" value=\"40.00";
                validationRule6.IgnoreCase = false;
                validationRule6.UseRegularExpression = false;
                validationRule6.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule6.Validate);

                ValidationRuleFindText validationRule7 = new ValidationRuleFindText();
                validationRule7.UseRegularExpression = true;
                validationRule7.FindText = ">12</td><td class=\"row([1-2])\" align=\"right\">25.00";
                validationRule7.IgnoreCase = false;
                validationRule7.UseRegularExpression = true;
                validationRule7.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule7.Validate);

                ValidationRuleFindText validationRule8 = new ValidationRuleFindText();
                validationRule8.UseRegularExpression = true;
                validationRule8.FindText = ">24</td><td class=\"row([1-2])\" align=\"right\">50.00";
                validationRule8.IgnoreCase = false;
                validationRule8.UseRegularExpression = true;
                validationRule8.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule8.Validate);

                ValidationRuleFindText validationRule9 = new ValidationRuleFindText();
                validationRule9.UseRegularExpression = true;
                validationRule9.FindText = ">48</td><td class=\"row([1-2])\" align=\"right\">70.00";
                validationRule9.IgnoreCase = false;
                validationRule9.UseRegularExpression = true;
                validationRule9.PassIfTextFound = true;
                request15.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule9.Validate);
            }
            yield return request15;
            request15 = null;



        }
    }
}
