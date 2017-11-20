
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddSignoffGroup : WebTest
    {

        public AddSignoffGroup()
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
            request6.ThinkTime = 1;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/admingroups.aspx"));
            request7.ThinkTime = 1;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx"));
            request8.ThinkTime = 16;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx"));
            request9.ThinkTime = 2;
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/aestage.aspx");
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("accountid", this.Context["$HIDDEN1.accountid"].ToString());
            request9Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmenu$LinkButton1");
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtgroupname", "__Auto Signoff Group");
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated signoff group - do not edit");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9.Body = request9Body;
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request9;
            request9 = null;

            // Extract the ID of signoff group
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT groupid FROM groups WHERE groupname" +
                                    " = '__Auto Signoff Group' UNION SELECT employeeid FROM employees WHERE username = 'james'");
            reader.Read();
            string groupID = reader.GetValue(0).ToString();
            reader.Read();
            string employeeID = reader.GetValue(0).ToString();
            reader.Close();

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aestage.aspx"));
            request10.ThinkTime = 11;
            request10.Method = "POST";
            request10.QueryStringParameters.Add("groupid", groupID, false, false);
            FormPostHttpBody request10Body = new FormPostHttpBody();
            request10Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmain$cmbsignofftype");
            request10Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request10Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request10Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbsignofftype", "2");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmblist", "16");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbinclude", "1");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbinvolvement", "1");
            request10Body.FormPostParameters.Add("ctl00$contentmain$cmbonholiday", "1");
            request10Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request10.Body = request10Body;
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/admin/aestage.aspx"));
            request11.ThinkTime = 5;
            request11.Method = "POST";
            request11.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/admin/aegroup.aspx");
            request11.QueryStringParameters.Add("groupid", groupID, false, false);
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbsignofftype", "2");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmblist", employeeID);
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbinclude", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbinvolvement", "2");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbonholiday", "1");
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "50");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "6");
            request11.Body = request11Body;
            ExtractHiddenFields extractionRule5 = new ExtractHiddenFields();
            extractionRule5.Required = true;
            extractionRule5.HtmlDecode = true;
            extractionRule5.ContextParameterName = "1";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
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
            request12Body.FormPostParameters.Add("ctl00$contentmain$txtgroupname", "__Auto Signoff Group");
            request12Body.FormPostParameters.Add("ctl00$contentmain$txtdescription", "Automatically generated signoff group - do not edit");            
            request12Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "39");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "20");
            request12.Body = request12Body;
            yield return request12;
            request12 = null;


            // Validation




        }
    }
}
