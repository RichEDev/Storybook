
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class SearchingForAnEmployee : WebTest
    {

        public SearchingForAnEmployee()
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

            #region Get to selectemployee.aspx page

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx"));
            request5.ThinkTime = 3;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request6.ThinkTime = 17;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            #endregion

            #region Create cDatabaseConnection

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());

            #endregion

            #region Test searching by surname

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request7.ThinkTime = 4;
            request7.Method = "POST";
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=lloyd&roleid=0&groupid=&costcodeid=0&departmentid=0&username=");
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "lloyd");
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "44");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "20");
            AutoTools.ValidateText("<td class=\"row[0-9]\">Mr</td><td class=\"row[0-9]\">James</td><td class=\"row[0-9]\">Lloyd</td>", request7, true, true);
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            #endregion

            #region Test searching by username

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request9.ThinkTime = 5;
            request9.Method = "POST";
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=&costcodeid=0&departmentid=0&username=james");
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request9Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "james");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "36");
            request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "16");
            AutoTools.ValidateText("<td class=\"row[0-9]\">Mr</td><td class=\"row[0-9]\">James</td><td class=\"row[0-9]\">Lloyd</td>", request9, true, true);
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            #endregion

            #region Test searching by Signoff group

            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT TOP 1 groupid, firstname, surname, title FROM employees WHERE groupid IS NOT NULL");
            reader.Read();
            string groupID = reader.GetValue(0).ToString();
            string firstName = reader.GetValue(1).ToString();
            string surname = reader.GetValue(2).ToString();
            string title = reader.GetValue(3).ToString();
            reader.Close();

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request11.ThinkTime = 2;
            request11.Method = "POST";
            request11.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=" + groupID + "&costcodeid=0&departmentid=0&username=");
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request11Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", groupID);
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "26");
            request11Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "24");
            AutoTools.ValidateText("<td class=\"row[0-9]\">" + title + "</td><td class=\"row[0-9]\">" + firstName + 
                                    "</td><td class=\"row[0-9]\">" + surname + "</td>", request11, true, true); ;
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            #endregion

            #region Test searching by Role

            reader = database.GetReader("SELECT TOP 1 roleid, firstname, surname, title FROM employees WHERE roleid IS NOT NULL ORDER BY username");
            reader.Read();
            string roleID = reader.GetValue(0).ToString();
            firstName = reader.GetValue(1).ToString();
            surname = reader.GetValue(2).ToString();
            title = reader.GetValue(3).ToString();
            reader.Close();

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request13.ThinkTime = 3;
            request13.Method = "POST";
            request13.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=" + roleID + "&groupid=&costcodeid=0&departmentid=0&username=");
            FormPostHttpBody request13Body = new FormPostHttpBody();
            request13Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request13Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request13Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", roleID);
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request13Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "24");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "17");
            AutoTools.ValidateText("<td class=\"row[0-9]\">" + title + "</td><td class=\"row[0-9]\">" + firstName +
                        "</td><td class=\"row[0-9]\">" + surname + "</td>", request13, true, true);
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            #endregion

            #region Test searching by Department

            reader = database.GetReader("SELECT TOP 1 employee_costcodes.departmentid, firstname, surname, title FROM employee_costcodes " +
                                        "INNER JOIN employees ON employees.employeeid = employee_costcodes.employeeid WHERE " +
                                        "employee_costcodes.departmentid IS NOT NULL");
            reader.Read();
            string departmentID = reader.GetValue(0).ToString();
            firstName = reader.GetValue(1).ToString();
            surname = reader.GetValue(2).ToString();
            title = reader.GetValue(3).ToString();
            reader.Close();

            WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request15.ThinkTime = 3;
            request15.Method = "POST";
            request15.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=&costcodeid=0&departmentid=" + departmentID + "&username=");
            FormPostHttpBody request15Body = new FormPostHttpBody();
            request15Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request15Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request15Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request15Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request15Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "");
            request15Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request15Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request15Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", departmentID);
            request15Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request15Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request15Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "47");
            request15Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "22");
            AutoTools.ValidateText("<td class=\"row[0-9]\">" + title + "</td><td class=\"row[0-9]\">" + firstName +
            "</td><td class=\"row[0-9]\">" + surname + "</td>", request15, true, true);
            request15.Body = request15Body;
            yield return request15;
            request15 = null;

            #endregion

            #region Test searching by Costcode

            reader = database.GetReader("SELECT TOP 1 employee_costcodes.costcodeid, firstname, surname, title FROM employee_costcodes " +
                            "INNER JOIN employees ON employees.employeeid = employee_costcodes.employeeid WHERE " +
                            "employee_costcodes.costcodeid IS NOT NULL AND employee_costcodes.costcodeid NOT LIKE 241");
            reader.Read();
            string costcodeID = reader.GetValue(0).ToString();
            firstName = reader.GetValue(1).ToString();
            surname = reader.GetValue(2).ToString();
            title = reader.GetValue(3).ToString();
            reader.Close();

            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request17.ThinkTime = 3;
            request17.Method = "POST";
            request17.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=&costcodeid=" + costcodeID + "&departmentid=0&username=");
            FormPostHttpBody request17Body = new FormPostHttpBody();
            request17Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request17Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request17Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request17Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request17Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", costcodeID);
            request17Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "34");
            request17Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "13");
            AutoTools.ValidateText("<td class=\"row[0-9]\">" + title + "</td><td class=\"row[0-9]\">" + firstName +
            "</td><td class=\"row[0-9]\">" + surname + "</td>", request17, true, true);
            request17.Body = request17Body;
            yield return request17;
            request17 = null;

            #endregion

            #region Test searching by Username and Signoff group

            reader = database.GetReader("SELECT TOP 1 groupid, username, firstname, surname, title FROM employees WHERE groupid IS NOT NULL");
            reader.Read();
            groupID = reader.GetValue(0).ToString();
            string userName = reader.GetValue(1).ToString();
            firstName = reader.GetValue(2).ToString();
            surname = reader.GetValue(3).ToString();
            title = reader.GetValue(4).ToString();
            reader.Close();
            
            WebTestRequest request19 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request19.ThinkTime = 4;
            request19.Method = "POST";
            request19.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx?surname=&roleid=0&groupid=" + groupID + "&costcodeid=0&departmentid=0&username=" + userName);
            FormPostHttpBody request19Body = new FormPostHttpBody();
            request19Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request19Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request19Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request19Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request19Body.FormPostParameters.Add("ctl00$contentmain$txtusername", userName);
            request19Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", groupID);
            request19Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request19Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request19Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request19Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request19Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "57");
            request19Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "14");
            AutoTools.ValidateText("<td class=\"row[0-9]\">" + title + "</td><td class=\"row[0-9]\">" + firstName +
            "</td><td class=\"row[0-9]\">" + surname + "</td>", request19, true, true);
            request19.Body = request19Body;
            yield return request19;
            request19 = null;

            #endregion

            #region Test pressing the Cancel button

            WebTestRequest request21 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/selectemployee.aspx"));
            request21.Method = "POST";
            request21.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/usermanagementmenu.aspx");
            FormPostHttpBody request21Body = new FormPostHttpBody();
            request21Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request21Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request21Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request21Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "");
            request21Body.FormPostParameters.Add("ctl00$contentmain$txtusername", "");
            request21Body.FormPostParameters.Add("ctl00$contentmain$cmbgroups", "");
            request21Body.FormPostParameters.Add("ctl00$contentmain$cmbroles", "0");
            request21Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartments", "0");
            request21Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcodes", "0");
            request21Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request21Body.FormPostParameters.Add("ctl00$contentmain$cmdClose.x", "30");
            request21Body.FormPostParameters.Add("ctl00$contentmain$cmdClose.y", "17");
            request21.Body = request21Body;
            yield return request21;
            request21 = null;

            #endregion

        }
    }
}
