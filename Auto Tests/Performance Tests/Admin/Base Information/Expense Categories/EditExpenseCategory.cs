
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditExpenseCategory : WebTest
    {

        public EditExpenseCategory()
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

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/admincategories.aspx");
            request5.ThinkTime = 3;
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

            string catID = AutoTools.GetID(this.Context["NewID"].ToString());

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/aecategory.aspx");
            request6.ThinkTime = 8;
            request6.QueryStringParameters.Add("action", "2", false, false);
            request6.QueryStringParameters.Add("categoryid", catID, false, false);
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/aecategory.aspx");
            request7.ThinkTime = 2;
            request7.Method = "POST";
            request7.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/admin/admincategories.aspx";
            request7.QueryStringParameters.Add("action", "2", false, false);
            request7.QueryStringParameters.Add("categoryid", catID, false, false);
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtcategory", "__James Auto Test EDITED");
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated expense category EDITED");
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "35");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "11");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/admincategories.aspx");
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = false;
                validationRule2.PassIfTextFound = true;
                request8.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);

                ValidationRuleFindText validationRule3 = new ValidationRuleFindText();
                validationRule3.FindText = "Automatically generated expense category EDITED";
                validationRule3.IgnoreCase = false;
                validationRule3.UseRegularExpression = false;
                validationRule3.PassIfTextFound = true;
                request8.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule3.Validate);
            }
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/aecategory.aspx");
            request9.QueryStringParameters.Add("action", "2");
            request9.QueryStringParameters.Add("categoryid", catID);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule4 = new ValidationRuleFindText();
                validationRule4.FindText = "__James Auto Test EDITED";
                validationRule4.IgnoreCase = false;
                validationRule4.UseRegularExpression = false;
                validationRule4.PassIfTextFound = true;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule4.Validate);

                ValidationRuleFindText validationRule5 = new ValidationRuleFindText();
                validationRule5.FindText = "Automatically generated expense category EDITED";
                validationRule5.IgnoreCase = false;
                validationRule5.UseRegularExpression = false;
                validationRule5.PassIfTextFound = true;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule5.Validate);
            }
            yield return request9;
            request9 = null;
        }
    }
}
