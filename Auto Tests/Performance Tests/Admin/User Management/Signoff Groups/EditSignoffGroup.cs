
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditSignoffGroup : WebTest
    {

        public EditSignoffGroup()
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



            // ===========================================================
            // Test needs updating to test all different types of signoff
            // ===========================================================




            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request5.ThinkTime = 24;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 3;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/admingroups.aspx"));
            request7.ThinkTime = 2;
            ExtractText extractionRule1 = new ExtractText();
            extractionRule1.StartsWith = "javascript";
            extractionRule1.EndsWith = "__Auto Signoff Group";
            extractionRule1.IgnoreCase = false;
            extractionRule1.UseRegularExpression = false;
            extractionRule1.Required = true;
            extractionRule1.ExtractRandomMatch = false;
            extractionRule1.Index = 0;
            extractionRule1.HtmlDecode = false;
            extractionRule1.ContextParameterName = "";
            extractionRule1.ContextParameterName = "groupID";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request7;
            request7 = null;

            string groupID = AutoTools.GetID(this.Context["groupID"].ToString());

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx"));
            request8.ThinkTime = 16;
            request8.QueryStringParameters.Add("action", "2", false, false);
            request8.QueryStringParameters.Add("groupid", groupID, false, false);
            ExtractText extractionRule1b = new ExtractText();
            extractionRule1b.StartsWith = "javascript:deleteStage(" + groupID + ",";
            extractionRule1b.EndsWith = ");&quot";
            extractionRule1b.IgnoreCase = false;
            extractionRule1b.UseRegularExpression = false;
            extractionRule1b.Required = true;
            extractionRule1b.ExtractRandomMatch = false;
            extractionRule1b.Index = 0;
            extractionRule1b.HtmlDecode = false;
            extractionRule1b.ContextParameterName = "";
            extractionRule1b.ContextParameterName = "signoffID";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1b.Extract);
            yield return request8;
            request8 = null;

            string signoffID = this.Context["signoffID"].ToString();

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aestage.aspx"));
            request9.ThinkTime = 2;
            request9.QueryStringParameters.Add("action", "2", false, false);
            request9.QueryStringParameters.Add("groupid", groupID, false, false);
            request9.QueryStringParameters.Add("signoffid", signoffID, false, false);
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aestage.aspx"));
            request10.ThinkTime = 4;
            request10.Method = "POST";
            request10.QueryStringParameters.Add("action", "2", false, false);
            request10.QueryStringParameters.Add("groupid", groupID, false, false);
            request10.QueryStringParameters.Add("signoffid", signoffID, false, false);
            FormPostHttpBody request10Body = new FormPostHttpBody();
            request10Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmain$cmbsignofftype");
            request10Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request10Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request10Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbsignofftype", "1");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmblist", "21417");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbinclude", "1");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbinvolvement", "2");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbonholiday", "1");
            request10Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request10.Body = request10Body;
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aestage.aspx"));
            request11.ThinkTime = 4;
            request11.Method = "POST";
            request11.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx?action=2&groupid=" + groupID);
            request11.QueryStringParameters.Add("action", "2", false, false);
            request11.QueryStringParameters.Add("groupid", groupID, false, false);
            request11.QueryStringParameters.Add("signoffid", signoffID, false, false);
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbsignofftype", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmblist", "16");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbinclude", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbinvolvement", "2");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbonholiday", "1");
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "41");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "9");
            request11.Body = request11Body;
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx"));
            request12.Method = "POST";
            request12.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/admingroups.aspx");
            request12.QueryStringParameters.Add("action", "2", false, false);
            request12.QueryStringParameters.Add("groupid", groupID, false, false);
            FormPostHttpBody request12Body = new FormPostHttpBody();
            request12Body.FormPostParameters.Add("accountid", this.Context["$HIDDEN1.accountid"].ToString());
            request12Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request12Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request12Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request12Body.FormPostParameters.Add("ctl00$contentmain$txtgroupname", "__Auto Signoff Group EDITED");
            request12Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated signoff group - EDITED");
            request12Body.FormPostParameters.Add("ctl00xcontentmainxgridstages", "%3CDisplayLayout%3E%3CStateChanges%3E%3C/StateChanges%3E%3C/DisplayLayout%3E");
            request12Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "33");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "16");
            request12.Body = request12Body;
            yield return request12;
            request12 = null;
        }
    }
}
