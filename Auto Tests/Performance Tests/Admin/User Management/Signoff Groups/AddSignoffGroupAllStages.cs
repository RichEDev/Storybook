
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddSignoffGroupAllStages : WebTest
    {

        public AddSignoffGroupAllStages()
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

            #region Login and get to the signoff groups page

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request5.ThinkTime = 9;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request6.ThinkTime = 3;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/admingroups.aspx"));
            request7.ThinkTime = 3;
            yield return request7;
            request7 = null;

            #endregion

            #region Add the Signoff group

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx"));
            request8.ThinkTime = 17;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx"));
            request9.ThinkTime = 10;
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/aestage.aspx?groupid=3222");
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("accountid", this.Context["$HIDDEN1.accountid"].ToString());
            request9Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmenu$LinkButton1");
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtgroupname", "__new test");
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "new test");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9.Body = request9Body;
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            ExtractText extractionRule4 = new ExtractText();
            extractionRule4.StartsWith = "ame=\"";
            extractionRule4.EndsWith = "\" onc";
            extractionRule4.Index = 5;
            extractionRule4.IgnoreCase = false;
            extractionRule4.UseRegularExpression = false;
            extractionRule4.HtmlDecode = true;
            extractionRule4.Required = false;
            extractionRule4.ContextParameterName = "FormPostParam9.__EVENTTARGET";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request9;
            request9 = null;

            #endregion

            #region Extract the groupID



            #endregion

            #region Add first stage

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aestage.aspx"));
            request11.ThinkTime = 3;
            request11.Method = "POST";
            request11.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx?action=2&groupid=3222");
            request11.QueryStringParameters.Add("groupid", "3222", false, false);
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbsignofftype", "2");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmblist", "21417");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbinclude", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbinvolvement", "2");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbonholiday", "1");
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "40");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "19");
            request11.Body = request11Body;
            ExtractHiddenFields extractionRule6 = new ExtractHiddenFields();
            extractionRule6.Required = true;
            extractionRule6.HtmlDecode = true;
            extractionRule6.ContextParameterName = "1";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule6.Extract);
            yield return request11;
            request11 = null;

            #endregion
            

        }
    }
}
