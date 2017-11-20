
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditBudgetHolders : WebTest
    {

        public EditBudgetHolders()
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
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 2;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "deleteBudgetHolder";
            extractionRule2.EndsWith = "__Auto Budget Holder";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "budgetHolderID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "deleteBudgetHolder";
            extractionRule3.EndsWith = "__Auto Budget Holder Two";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "secondBudgetHolderID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request7;
            request7 = null;

            string budgetHolderID = AutoTools.GetID(this.Context["budgetHolderID"].ToString(), ";", "<", 21);
            string secondBudgetHolderID = AutoTools.GetID(this.Context["secondBudgetHolderID"].ToString(), ";", "<", 21);

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request8.ThinkTime = 1;
            request8.QueryStringParameters.Add("budgetholderid", budgetHolderID, false, false);
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx");
            request9.QueryStringParameters.Add("budgetholderid", budgetHolderID, false, false);
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtlabel", "__Auto Budget Holder EDITED");
            request9Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_hdn_1", "lynne");
            request9Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_text_1", "lynne");
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated budget holder - EDITED");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "37");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "17");
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request10.ThinkTime = 1;
            request10.QueryStringParameters.Add("budgetholderid", secondBudgetHolderID, false, false);
            ExtractHiddenFields extractionRule5 = new ExtractHiddenFields();
            extractionRule5.Required = true;
            extractionRule5.HtmlDecode = true;
            extractionRule5.ContextParameterName = "1";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aebudget.aspx"));
            request11.Method = "POST";
            request11.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx");
            request11.QueryStringParameters.Add("budgetholderid", secondBudgetHolderID, false, false);
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtlabel", "__Auto Budget Holder Two EDITED");
            request11Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_hdn_1", "lynne");
            request11Body.FormPostParameters.Add("ctl00$contentmain$relUser$relationship_text_1", "lynne");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated second budget holder - EDITED");
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "37");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "17");
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            // Validation
            WebTestRequest request12 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/admin/adminbudget.aspx");
            AutoTools.ValidateText("__Auto Budget Holder EDITED", request12);
            AutoTools.ValidateText("__Auto Budget Holder Two EDITED", request12);
            AutoTools.ValidateText("Automatically generated budget holder - EDITED", request12);
            AutoTools.ValidateText("Automatically generated second budget holder - EDITED", request12);
            AutoTools.ValidateText("Hunt, Mrs Lynne", request12);
        }
    }
}
