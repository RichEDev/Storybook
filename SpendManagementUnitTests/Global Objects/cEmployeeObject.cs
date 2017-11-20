using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;
using System.Collections;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cEmployeeObject
    {
        public static int CreateUTEmployee()
        {
            int employeeID;
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            cEmployee reqEmployee = GetUTEmployeeTemplateObject();

            employeeID = clsEmployees.saveEmployee(reqEmployee, new cDepCostItem[] { }, new List<int>(), null);

            return employeeID;
        }

        /// <summary>
        /// Create the employee deleagte object to be used for the unit tests where testing for a delegate is required
        /// </summary>
        /// <returns></returns>
        public static int CreateUTDelegateEmployee()
        {
            int employeeID;
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            cEmployee reqEmployee = GetUTEmployeeTemplateObject();

            employeeID = clsEmployees.saveEmployee(reqEmployee, new cDepCostItem[] { }, new List<int>(), null);
            cGlobalVariables.DelegateID = employeeID;

            return employeeID;
        }

        /// <summary>
        /// Creates an inactive ESR Assignment number for an employee with start and final active dates provided
        /// </summary>
        /// <param name="employeeid"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static cEmployee CreateUTEmployeeInactiveESRAssignmentNumber(int employeeid, DateTime startdate, DateTime? enddate)
        {
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = clsEmployees.GetEmployeeById(employeeid);
            Dictionary<int, cESRAssignment> esrAssignments = new Dictionary<int,cESRAssignment>();
            cESRAssignment ass = new cESRAssignment(123, 0, "1234567", startdate, enddate, ESRAssignmentStatus.ActiveAssignment, "", "", "", "", "", "", "", "", "", false, "", "", "", "", "", "", true, 35, "", 0, 0, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", null, false, null, null, null, null);
            esrAssignments.Add(123, ass);

            cDepCostItem[] lstCostCodes = clsEmployees.GetCostCodeBreakdown(emp.employeeid);

            cEmployee newemp = new cEmployee(emp.accountid, emp.employeeid, emp.username, emp.password, emp.PasswordMethod, emp.title, emp.firstname, emp.surname, emp.mileagetotal, emp.mileagetotaldate, emp.email, emp.currefnum, emp.curclaimno, emp.address1, emp.address2, emp.city, emp.county, emp.postcode, emp.payroll, emp.position, emp.telno, emp.creditor, emp.archived, emp.groupid, emp.lastchange, emp.fax, emp.homeemail, emp.extension, emp.pagerno, emp.mobileno, emp.linemanager, emp.advancegroup, emp.primarycountry, emp.primarycurrency, emp.verified, emp.active, emp.licenceexpiry, emp.licencelastchecked, emp.licencecheckedby, emp.licencenumber, emp.groupidcc, emp.groupidpc, emp.ninumber, emp.middlenames, emp.maidenname, emp.gender, emp.dateofbirth, emp.hiredate, emp.leavedate, emp.country, emp.customiseditems, emp.createdon, emp.createdby, emp.modifiedon, emp.modifiedby, emp.Name, emp.AccountNumber, emp.AccountType, emp.Sortcode, emp.Reference, emp.LocaleID, emp.NHSTrustID, emp.currentLogonCount, emp.logonRetryCount, emp.FirstLogon, emp.LicenceAttachID, cGlobalVariables.SubAccountID, emp.empCreationMethod);

            int newEmpID = 0;
            if (emp != null)
            {
                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                newEmpID = clsEmployees.saveEmployee(newemp, lstCostCodes, clsEmployees.GetEmailNotifications(employeeid), clsEmployees.GetUserDefinedFields(employeeid));
            }

            if (newEmpID > 0 && newEmpID == emp.employeeid)
            {
                cESRAssignments clsAssignments = new cESRAssignments(cGlobalVariables.AccountID, emp.employeeid);
                clsAssignments.saveESRAssignment(ass);
            }

            return emp;
        }

        public static cEmployee CreateUTEmployeeObject(cEmployee reqEmployee)
        {
            int employeeID;
            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            cDepCostItem[] lstCostCodes = clsEmployees.GetCostCodeBreakdown(reqEmployee.employeeid);
            List<int> lstEmailNotifications = clsEmployees.GetEmailNotifications(reqEmployee.employeeid);
            SortedList<int, object> lstUserDefinedFields = clsEmployees.GetUserDefinedFields(reqEmployee.employeeid);

            employeeID = clsEmployees.saveEmployee(reqEmployee, lstCostCodes, lstEmailNotifications, lstUserDefinedFields);

            cEmployee savedEmployee = null;
            if (employeeID > 0)
            {
                savedEmployee = clsEmployees.GetEmployeeById(employeeID);
            }

            return savedEmployee;
        }

        /// <summary>
        /// Create an employee global static object that has item roles associated
        /// </summary>
        /// <returns></returns>
        public static cEmployee CreateUTEmployeeWithItemRolesObject()
        {
            //Create and associate an item role to the employee
            List<int> lstItemRoles = new List<int>();
            cItemRole role = cItemRoleObject.CreateItemRole();
            lstItemRoles.Add(role.itemroleid);

            cEmployee reqEmployee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);

            reqEmployee._ItemRoles = lstItemRoles;

            return reqEmployee;
        }

        public static void UpdateEmployeePasswordDetails(PwdMethod pwdMethod, string planTextPassword, int employeeID)
        {
            string convertedPassword = string.Empty;

            switch (pwdMethod)
            {
                case PwdMethod.FWBasic:
                    convertedPassword = cPassword.Crypt(planTextPassword, "2");
                    break;
                case PwdMethod.Hash:
                case PwdMethod.MD5:
                    convertedPassword = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(planTextPassword, System.Web.Configuration.FormsAuthPasswordFormat.MD5.ToString());
                    break;
                case PwdMethod.RijndaelManaged:
                    cSecureData clsSecureData = new cSecureData();
                    convertedPassword = clsSecureData.Encrypt(planTextPassword);
                    break;
                case PwdMethod.SHA_Hash:
                    convertedPassword = cPassword.SHA_HashPassword(planTextPassword);
                    break;
                default:
                    throw new Exception("Unknown PwdMethod type");
            }


            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strSQL = "UPDATE employees SET passwordMethod=@pwdMethod, password=@pwd WHERE employeeID=@employeeID";
            db.sqlexecute.Parameters.AddWithValue("@pwdMethod", pwdMethod);
            db.sqlexecute.Parameters.AddWithValue("@pwd", convertedPassword);
            db.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            db.ExecuteSQL(strSQL);
        }

        public static cEmployee GetUTEmployeeTemplateObject(DateTime? licenceExpiryDate = null)
        {
            cEmployee employee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.ToShortDateString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);
            return employee;
        }

        public static cEmployee GetUTEmployeeTemplateWhereArchivedObject()
        {
            cEmployee employee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.ToShortDateString() + " @software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);
            return employee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeWhoIsActiveAndHasNoPasswordSetObject()
        {
            cEmployee employee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, true, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false,  DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);
            return employee;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeWhoIsInactiveAndHasPasswordSetObject()
        {
            cEmployee employee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);
            return employee;
        }

        /// <summary>
        /// This is used where comparisons need to be done for employees with the same username
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeTemplateObjectWithStaticUsername()
        {
            cEmployee employee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName", "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow, "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);
            return employee;
        }

        /// <summary>
        /// Employee whose password last chnaged date is 2 days old
        /// </summary>
        /// <returns></returns>
        public static cEmployee GetUTEmployeeTemplateWithPasswordLastChangedExpiredObject()
        {
            cEmployee employee = new cEmployee(cGlobalVariables.AccountID, 0, "UTUserName " + DateTime.UtcNow.ToString() + ":" + DateTime.UtcNow.Ticks, "A", PwdMethod.RijndaelManaged, "Auto", "Unit", "Tester", 0, null, "ut" + DateTime.Now.Ticks.ToString() + "@software-europe.co.uk", 0, 0, "1 UT Street", "", "Lincoln", "Lincs", "LN5 8SB", "12345678", "UTPosition", "01522881300", "UTCreditor", false, 0, DateTime.UtcNow.AddDays(-2), "01522881355", "uthome@software-europe.co.uk", "280", "01522000000", "07795000000", 0, 0, 0, 0, false, true, null, null, 0, "", 0, 0, "", "UTMIddle", "UTMaiden", "Male", DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "UK", false, DateTime.UtcNow, cGlobalVariables.EmployeeID, null, null, "UTName", "UTAccountNumber", "UTAccountType", "UTSortCode", "UTReference", null, null, 0, 0, false, 0, cGlobalVariables.SubAccountID, EmployeeCreationMethod.Manually);
            return employee;
        }

        public static void UpdateEmployeeBaseCurrency(int CurrencyID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "UPDATE employees SET primarycurrency = @currencyid WHERE employeeid = @employeeID ";

            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", cGlobalVariables.EmployeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", CurrencyID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Delete the delegate unit test employee from the database
        /// </summary>
        public static void DeleteDelegateUTEmployee()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            clsEmps.deleteEmployee(cGlobalVariables.DelegateID);
        }

        /// <summary>
        /// Delete the unit test employee from the database
        /// </summary>
        public static void DeleteUTEmployee(int ID, string username)
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            clsEmps.archiveEmployee(username, cGlobalVariables.AccountID);
            clsEmps.deleteEmployee(ID);
        }

        /// <summary>
        /// Create an unread broadcast message for the cGlobalVariables.EmployeeID
        /// </summary>
        public static void CreateUnreadBroadcastMessage()
        {
            cBroadcastMessages msgs = new cBroadcastMessages(cGlobalVariables.AccountID);
            
            //cGlobalVariables.BroadcastID = msgs.addBroadcastMessage("UT Test Broadcast Message", "This is a test message for use by unit tests.", DateTime.Now,DateTime.Now.AddDays(10), DateTime.Now.AddHours(1), true, broadcastLocation.HomePage,DateTime.Now, cGlobalVariables.EmployeeID);

            return;
        }

        /// <summary>
        /// Delete the broadcast message for the cGlobalVariables.EmployeeID
        /// </summary>
        public static void DeleteBroadcastMessage()
        {
            cBroadcastMessages msgs = new cBroadcastMessages(cGlobalVariables.AccountID);

            msgs.deleteBroadcastMessage(cGlobalVariables.BroadcastID);
            return;
        }

        /// <summary>
        /// properties that will be omitted when the compare assert method is used to compare objects in unit tests
        /// </summary>
        public static readonly List<string> lstOmittedProperties;

        /// <summary>
        /// The constructor creates a new instance of the omitted properties and adds them
        /// </summary>
        static cEmployeeObject()
        {
            lstOmittedProperties = new List<string>();

            lstOmittedProperties.Add("employeeid");
        }
    }
}
