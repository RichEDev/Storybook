
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class SelectingAnEmployee : WebTest
    {

        public SelectingAnEmployee()
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            #region Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidationRuleResponseTimeGoal validationRule2 = new ValidationRuleResponseTimeGoal();
                validationRule2.Tolerance = 0D;
                this.ValidateResponseOnPageComplete += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            #endregion

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            #region Get to selectemployee.aspx

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request5.ThinkTime = 6;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request6.ThinkTime = 8;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            #endregion


            #region Test selecting an employee to edit

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request7.ThinkTime = 17;
            AutoTools.ExtractText("employeeid = ", ";", "employeeID", request7);
            AutoTools.ExtractText("accountid = ", ";", "accountID", request7);
            yield return request7;
            request7 = null;

            string employeeID = this.Context["employeeID"].ToString();
            string accountID = this.Context["accountID"].ToString();

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeemployee.aspx"));
            request8.QueryStringParameters.Add("employeeid", employeeID, false, false);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/usercontrols/FileUploadIFrame.aspx"));
            request9.QueryStringParameters.Add("tablename", "cars_attachments", false, false);
            request9.QueryStringParameters.Add("idfield", "id", false, false);
            request9.QueryStringParameters.Add("recordid", "0", false, false);
            request9.QueryStringParameters.Add("mdlcancel", "ctl00_contentmain_aeCar_cmdCarDocAttachCancel", false, false);
            request9.QueryStringParameters.Add("multipleAttachments", "0", false, false);
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/usercontrols/FileUploadIFrame.aspx"));
            request10.ThinkTime = 6;
            request10.QueryStringParameters.Add("tablename", "employee_attachments", false, false);
            request10.QueryStringParameters.Add("idfield", "id", false, false);
            request10.QueryStringParameters.Add("recordid", employeeID, false, false);
            request10.QueryStringParameters.Add("mdlcancel", "", false, false);
            request10.QueryStringParameters.Add("multipleAttachments", "0", false, false);
            yield return request10;
            request10 = null;

            #endregion


            #region Test archiving / unarchiving of employees

            #region Search for username 'james'

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request11.ThinkTime = 3;
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request12.ThinkTime = 6;
            request12.Method = "POST";
            request12.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=&costcodeid=0&departmentid=0&username=james");
            FormPostHttpBody request12Body = new FormPostHttpBody();
            request12Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request12Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request12Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request12Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request12Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "james");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request12Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "0");
            request12Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "0");
            request12.Body = request12Body;
            yield return request12;
            request12 = null;

            #endregion

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx/changeStatus"));
            request13.ThinkTime = 5;
            request13.Method = "POST";
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=utf-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"employeeid\":" + employeeID + "}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            #region Validate that 'james' has been archived

            WebTestRequest request13a = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request13a.ThinkTime = 6;
            request13a.Method = "POST";
            request13a.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=&costcodeid=0&departmentid=0&username=james");
            FormPostHttpBody request13aBody = new FormPostHttpBody();
            request13aBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request13aBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request13aBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request13aBody.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$txtusername", "james");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request13aBody.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "0");
            request13aBody.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "0");
            // Validate that the employee has been made 'Archived'
            AutoTools.ValidateText("javascript:changeArchiveStatus(" + employeeID + ");\"><img title=\"Un-Archive\"", request13a);
            request13a.Body = request13aBody;
            yield return request13a;
            request13a = null;

            #endregion

            WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx/changeStatus"));
            request14.ThinkTime = 9;
            request14.Method = "POST";
            StringHttpBody request14Body = new StringHttpBody();
            request14Body.ContentType = "application/json; charset=utf-8";
            request14Body.InsertByteOrderMark = false;
            request14Body.BodyString = "{\"employeeid\":" + employeeID + "}";
            request14.Body = request14Body;
            yield return request14;
            request14 = null;

            #region Validate that 'james' has been unarchived

            WebTestRequest request14a = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request14a.ThinkTime = 6;
            request14a.Method = "POST";
            request14a.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=&costcodeid=0&departmentid=0&username=james");
            FormPostHttpBody request14aBody = new FormPostHttpBody();
            request14aBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request14aBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request14aBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request14aBody.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$txtusername", "james");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request14aBody.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "0");
            request14aBody.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "0");
            // Validate that the employee has been made 'Un-Archived'
            AutoTools.ValidateText("javascript:changeArchiveStatus(" + employeeID + ");\"><img title=\"Archive", request14a);
            request14a.Body = request14aBody;
            yield return request14a;
            request14a = null;

            #endregion

            #endregion


            #region Test deleting an employee that is not archived
            
            WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx/deleteEmployee"));
            request15.ThinkTime = 10;
            request15.Method = "POST";
            StringHttpBody request15Body = new StringHttpBody();
            request15Body.ContentType = "application/json; charset=utf-8";
            request15Body.InsertByteOrderMark = false;
            request15Body.BodyString = "{\"accountid\":" + accountID + ",\"employeeid\":" + employeeID + "}";
            request15.Body = request15Body;
            yield return request15;
            request15 = null;

            #endregion


            #region Test the change password page can be loaded correctly

            WebTestRequest request16 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/changepassword.aspx"));
            request16.ThinkTime = 3;
            request16.QueryStringParameters.Add("returnto", "1", false, false);
            request16.QueryStringParameters.Add("employeeid", employeeID, false, false);
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request16.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request16;
            request16 = null;

            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/changepassword.aspx"));
            request17.ThinkTime = 5;
            request17.Method = "POST";
            request17.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=&groupid=&costcodeid=&departmentid=");
            request17.QueryStringParameters.Add("returnto", "1", false, false);
            request17.QueryStringParameters.Add("employeeid", employeeID, false, false);
            FormPostHttpBody request17Body = new FormPostHttpBody();
            request17Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request17Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request17Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request17Body.FormPostParameters.Add("ctl00$contentmain$txtnew", "");
            request17Body.FormPostParameters.Add("ctl00$contentmain$txtrenew", "");
            request17Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmdcancel.x", "42");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmdcancel.y", "5");
            request17.Body = request17Body;
            yield return request17;
            request17 = null;

            #endregion


            #region Test the reset password functionality does not error

            WebTestRequest request20 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx/ResetPassword"));
            request20.ThinkTime = 12;
            request20.Method = "POST";
            StringHttpBody request20Body = new StringHttpBody();
            request20Body.ContentType = "application/json; charset=utf-8";
            request20Body.InsertByteOrderMark = false;
            request20Body.BodyString = "{\"employeeID\":" + employeeID + "}";
            request20.Body = request20Body;
            yield return request20;
            request20 = null;

            #endregion


            // Grid needs to be tested with a few pages worth of employees being displayed.

            #region Test sorting of the grid


            WebTestRequest request21 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request21.ThinkTime = 2;
            request21.Method = "POST";
            StringHttpBody request21Body = new StringHttpBody();
            request21Body.ContentType = "application/json; charset=utf-8";
            request21Body.InsertByteOrderMark = false;
            request21Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""1c45b860-ddaa-47da-9eec-981f59cce795"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request21.Body = request21Body;
            yield return request21;
            request21 = null;

            WebTestRequest request22 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request22.ThinkTime = 1;
            request22.Method = "POST";
            StringHttpBody request22Body = new StringHttpBody();
            request22Body.ContentType = "application/json; charset=utf-8";
            request22Body.InsertByteOrderMark = false;
            request22Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""28471060-247d-461c-abf6-234bcb4698aa"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Descending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request22.Body = request22Body;
            yield return request22;
            request22 = null;

            WebTestRequest request23 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request23.ThinkTime = 1;
            request23.Method = "POST";
            StringHttpBody request23Body = new StringHttpBody();
            request23Body.ContentType = "application/json; charset=utf-8";
            request23Body.InsertByteOrderMark = false;
            request23Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""28471060-247d-461c-abf6-234bcb4698aa"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""28471060-247d-461c-abf6-234bcb4698aa"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request23.Body = request23Body;
            yield return request23;
            request23 = null;

            WebTestRequest request24 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request24.ThinkTime = 1;
            request24.Method = "POST";
            StringHttpBody request24Body = new StringHttpBody();
            request24Body.ContentType = "application/json; charset=utf-8";
            request24Body.InsertByteOrderMark = false;
            request24Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""6614acad-0a43-4e30-90ec-84de0792b1d6"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""28471060-247d-461c-abf6-234bcb4698aa"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Descending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request24.Body = request24Body;
            yield return request24;
            request24 = null;

            WebTestRequest request25 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request25.ThinkTime = 1;
            request25.Method = "POST";
            StringHttpBody request25Body = new StringHttpBody();
            request25Body.ContentType = "application/json; charset=utf-8";
            request25Body.InsertByteOrderMark = false;
            request25Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""6614acad-0a43-4e30-90ec-84de0792b1d6"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request25.Body = request25Body;
            yield return request25;
            request25 = null;

            WebTestRequest request26 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request26.ThinkTime = 1;
            request26.Method = "POST";
            StringHttpBody request26Body = new StringHttpBody();
            request26Body.ContentType = "application/json; charset=utf-8";
            request26Body.InsertByteOrderMark = false;
            request26Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""9d70d151-5905-4a67-944f-1ad6d22cd931"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Descending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request26.Body = request26Body;
            yield return request26;
            request26 = null;

            WebTestRequest request27 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request27.ThinkTime = 1;
            request27.Method = "POST";
            StringHttpBody request27Body = new StringHttpBody();
            request27Body.ContentType = "application/json; charset=utf-8";
            request27Body.InsertByteOrderMark = false;
            request27Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""9d70d151-5905-4a67-944f-1ad6d22cd931"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request27.Body = request27Body;
            yield return request27;
            request27 = null;

            WebTestRequest request28 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request28.Method = "POST";
            StringHttpBody request28Body = new StringHttpBody();
            request28Body.ContentType = "application/json; charset=utf-8";
            request28Body.InsertByteOrderMark = false;
            request28Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Descending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request28.Body = request28Body;
            yield return request28;
            request28 = null;

            WebTestRequest request29 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/sortGrid"));
            request29.Method = "POST";
            StringHttpBody request29Body = new StringHttpBody();
            request29Body.ContentType = "application/json; charset=utf-8";
            request29Body.InsertByteOrderMark = false;
            request29Body.BodyString = @"{""accountid"":258,""gridid"":""gridEmployees"",""newsortcolumnid"":""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",""filter"":"""",""gridDetails"":[""gridEmployees"",true,""aeemployee.aspx?employeeid={employeeid}"",true,""javascript:deleteEmployee({employeeid});"",true,20,true,false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[true,""eda990e3-6b7e-4c26-8d38-ad1d77fb2fbf"",[],0,"""",false,""1c45b860-ddaa-47da-9eec-981f59cce795"",[],0,"""",true,""3a6a93f0-9b30-4cc2-afc4-33ec108fa77a"",[],0,"""",false,""28471060-247d-461c-abf6-234bcb4698aa"",[],0,"""",false,""6614acad-0a43-4e30-90ec-84de0792b1d6"",[],0,"""",false,""9d70d151-5905-4a67-944f-1ad6d22cd931"",[],0,"""",false,""ae818689-4b20-40a4-b5ba-3f1ab8b523bb"",[],0,""""],""618db425-f430-4660-9525-ebab444ed754"",true,""javascript:changeArchiveStatus({employeeid});"",""archived"",""Ascending"",""employeeid"",true,[""1c45b860-ddaa-47da-9eec-981f59cce795"",7,[""james%""],[],""1c45b860-ddaa-47da-9eec-981f59cce795"",46,[""admin%""],[]],[""Change Password"","""",""../images/icons/replace2.gif"",""/shared/changepassword.aspx?returnto=1&employeeid={employeeid}"","""",""Reset Password"","""",""../images/icons/redo.png"",""javascript:sendPasswordLink({employeeid});"",""""],false,""CheckBox"",false,""cGrid""],""is_static"":0}";
            request29.Body = request29Body;
            yield return request29;
            request29 = null;

            #endregion
        }
    }
}
