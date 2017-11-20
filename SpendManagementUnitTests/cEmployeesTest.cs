using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using System;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using Infragistics.WebUI.UltraWebGrid;
using System.Data;
using System.Collections;
using SpendManagementUnitTests.Global_Objects;
using System.Data.SqlClient;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cEmployeesTest and is intended
    ///to contain all cEmployeesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cEmployeesTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion



        /// <summary>
        ///A test for verifyAccount
        ///</summary>
        [TestMethod()]
        public void cEmployees_verifyAccount()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                clsEmps.verifyAccount(emp.employeeid);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                string strSQL = "SELECT verified FROM employees WHERE employeeid = @employeeID";

                bool verified = false;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        verified = reader.GetBoolean(0);
                    }
                    reader.Close();
                }

                db.sqlexecute.Parameters.Clear();

                Assert.IsTrue(verified);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for UpdatePasswordMethod
        ///This is on the logon page so the unit test will test from the database as there wont be an employee object available outside
        ///of logging on
        ///</summary>
        [TestMethod()]
        public void cEmployees_UpdatePasswordMethod()
        {           
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                cSecureData clsSecureData = new cSecureData();
                string password = "newUnitTest";

                clsEmps.UpdatePasswordMethod(emp.employeeid, clsSecureData.Encrypt(password), PwdMethod.RijndaelManaged);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                string strSQL = "SELECT password FROM employees WHERE employeeid = @employeeID";

                string newPassword = db.getStringValue(strSQL);
                db.sqlexecute.Parameters.Clear();

                Assert.AreEqual(password, clsSecureData.Decrypt(newPassword));
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
           
        }

        /// <summary>
        ///A test for updateMyDetails
        ///</summary>
        [TestMethod()]
        public void cEmployees_updateMyDetails()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                clsEmps.updateMyDetails(emp.employeeid, "Dr", "Changed", "Changed", "987654321", "changed@changed.com", "changed@changed.com", "987", "001", "987654321");
                emp = clsEmps.GetEmployeeById(emp.employeeid);
                Assert.AreEqual("Dr", emp.title);
                Assert.AreEqual("Changed", emp.firstname);
                Assert.AreEqual("Changed", emp.surname);
                Assert.AreEqual("987654321", emp.telno);
                Assert.AreEqual("changed@changed.com", emp.email);
                Assert.AreEqual("changed@changed.com", emp.homeemail);
                Assert.AreEqual("987", emp.pagerno);
                Assert.AreEqual("001", emp.extension);
                Assert.AreEqual("987654321", emp.mobileno);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }
        
        /// <summary>
        ///A test for saveEmployee
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cEmployeesTest_saveEmployee_newEmployee()
        {
            int accountid = cGlobalVariables.AccountID;
            cEmployees target = new cEmployees(accountid);
            cEmployee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            int actual = 0;

            try
            {
                actual = target.saveEmployee(employee, new cDepCostItem[] { }, new List<int>(), null);
                Assert.IsTrue(actual > 0);
            }
            finally
            {
                target.archiveEmployee(employee.username, accountid);
                target.deleteEmployee(actual);
            }
        }

        /// <summary>
        ///A test for saveEmployee
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cEmployeesTest_saveEmployee_existingEmployee()
        {
            int accountid = cGlobalVariables.AccountID;
            cEmployees target = new cEmployees(accountid);
            cEmployee employee = cEmployeeObject.GetUTEmployeeTemplateObject();
            int actual = 0;
            
            try
            {
                actual = target.saveEmployee(employee, new cDepCostItem[] { }, new List<int>(), null);
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString());

                int editing;
                editing = target.saveEmployee(target.GetEmployeeById(actual), new cDepCostItem[] { }, new List<int>(), null);
                Assert.AreEqual(actual, editing, "No employee id returned from editing save - return code " + editing.ToString());
            }
            finally
            {
                target.archiveEmployee(employee.username, accountid);
                target.deleteEmployee(actual);
            }
        }


        /// <summary>
        ///A test for saveEmployee
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cEmployeesTest_saveEmployee_duplicateEmployee()
        {
            int accountid = cGlobalVariables.AccountID;
            cEmployees target = new cEmployees(accountid);
            cEmployee employee = cEmployeeObject.GetUTEmployeeTemplateObject();

            int actual = 0;
            
            try
            {
                actual = target.saveEmployee(employee, new cDepCostItem[] { }, new List<int>(), null);
                Assert.IsTrue(actual > 0, "No employee id returned from save - return code " + actual.ToString());

                int duplicate;
                duplicate = target.saveEmployee(employee, new cDepCostItem[] { }, new List<int>(), null);
                Assert.IsTrue(duplicate == -1, "SaveEmployee duplicate actual Return Value = " + duplicate.ToString());
            }
            finally
            {
                target.archiveEmployee(employee.username, accountid);
                target.deleteEmployee(actual);
            }
        }
      
        /// <summary>
        ///A test for RemovePasswordKey
        ///</summary>
        [TestMethod()]
        public void cEmployees_RemovePasswordKey()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            
            string uKey = clsEmps.AddPasswordKey(cGlobalVariables.EmployeeID, cEmployees.PasswordKeyType.NewEmployee, DateTime.Now.AddDays(1));
            clsEmps.RemovePasswordKey(uKey);

            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            db.AddWithValue("@uniqueKey", uKey, 50);
            string strSQL = "SELECT COUNT(uniqueKey) FROM employeePasswordKeys WHERE uniqueKey=@uniqueKey";

            int count = db.getcount(strSQL);
            db.sqlexecute.Parameters.Clear();
            Assert.IsTrue(count == 0);

        }

        /// <summary>
        ///A test for getEmployeeidByUsername with a valid username
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByUsernameWithValidUsername()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                int empID = clsEmps.getEmployeeidByUsername(cGlobalVariables.AccountID, emp.username);
                Assert.AreEqual(emp.employeeid, empID);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for getEmployeeidByUsername with an invalid username
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByUsernameWithInvalidUsername()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                int empID = clsEmps.getEmployeeidByUsername(cGlobalVariables.AccountID, "utUnique" + DateTime.Now.Ticks.ToString());
                Assert.IsTrue(empID == 0);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for getEmployeeidByEmailAddress with a valid email address of an existing employee
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByEmailAddressWithValidEmail()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                int empID = clsEmps.getEmployeeidByEmailAddress(cGlobalVariables.AccountID, emp.email);
                Assert.AreEqual(emp.employeeid, empID);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for getEmployeeidByEmailAddress with an invalid email address of an existing employee
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByEmailAddressWithInvalidEmail()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            int empID = clsEmps.getEmployeeidByEmailAddress(cGlobalVariables.AccountID, "ut" + DateTime.Now.Ticks + "@ut.com");
            Assert.AreEqual(0, empID);
        }

        /// <summary>
        ///A test for getEmployeeidByEmailAddress with a blank email address of an existing employee
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByEmailAddressWithABlankEmail()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            int empID = clsEmps.getEmployeeidByEmailAddress(cGlobalVariables.AccountID, "");
            Assert.AreEqual(0, empID);
        }

        /// <summary>
        ///A test for getEmployeeidByEmail with a valid email address of an existing employee
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByEmailWithValidEmail()
        {
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                CurrentUser curUser = cEmployees.getEmployeeidByEmail(emp.email);
                Assert.AreEqual(emp.employeeid, curUser.EmployeeID);
                Assert.AreEqual(cGlobalVariables.AccountID, curUser.AccountID);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for getEmployeeidByEmail with an invalid email address of an existing employee
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByEmailWithInvalidEmail()
        {
            CurrentUser curUser = cEmployees.getEmployeeidByEmail("ut" + DateTime.Now.Ticks + "@ut.com");
            Assert.AreEqual(0, curUser.EmployeeID);
            Assert.AreEqual(0, curUser.AccountID);
        }

        /// <summary>
        ///A test for getEmployeeidByEmail with a blank email address of an existing employee
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByEmailWithABlankEmail()
        {
            CurrentUser curUser = cEmployees.getEmployeeidByEmail("");
            Assert.AreEqual(0, curUser.EmployeeID);
            Assert.AreEqual(0, curUser.AccountID);
        }

        /// <summary>
        ///A test for getEmployeeidByAssignment where assignment exists
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByAssignmentWithValidAssignment()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, emp.payroll + "-1");
                Assert.AreEqual(emp.employeeid, empID);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for getEmployeeidByAssignment where assignment doe not exist
        ///</summary>
        [TestMethod()]
        public void cEmployees_getEmployeeidByAssignmentWithInvalidAssignment()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                int empID = clsEmps.getEmployeeidByAssignment(cGlobalVariables.AccountID, "utUnique" + DateTime.Now.Ticks.ToString());
                Assert.IsTrue(empID == 0);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        // USERNAME USES TICKS SO AT PRESENT IS NEVER EQUAL - NOT TAKING TICKS OUT AS MAY NEED TO BE UNIQUE ELSEWHERE.

        ///// <summary>
        /////A test for GetEmployeeById
        /////</summary>
        //[TestMethod()]
        //public void cEmployeesTest_GetEmployeeById_withAllValuesSet()
        //{
        //    System.Diagnostics.Debugger.Break();
        //    int accountid = cGlobalVariables.AccountID;
        //    int employeeid = cEmployeeObject.CreateUTEmployee();
        //    cEmployees target = new cEmployees(accountid);
        //    cEmployee actual = null;
        //    cEmployee expected;

        //    try
        //    {
        //        expected = cEmployeeObject.GetUTEmployeeTemplateObject();
        //        actual = target.GetEmployeeById(employeeid);

        //        cCompareAssert.AreEqual(expected, actual, cEmployeeObject.lstOmittedProperties);
        //        Assert.AreEqual(actual.employeeid, employeeid);
        //    }
        //    finally
        //    {
        //        target.archiveEmployee(actual.username, accountid);
        //        target.deleteEmployee(employeeid);
        //    }
        //}
                
        /// <summary>
        ///A test for getCount with a valid Account ID
        ///</summary>
        [TestMethod()]
        public void cEmployees_getCountWithValidAccountID()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                //Need to ensure the count in cache is null
                System.Web.Caching.Cache Cache = (System.Web.Caching.Cache)System.Web.HttpRuntime.Cache;
                Cache.Remove("employeecount" + cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                int count = clsEmps.getCount(cGlobalVariables.AccountID);
                Assert.IsTrue(count > 0);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for getCount with a valid Account ID
        ///</summary>
        [TestMethod()]
        public void cEmployees_getCountWithAnInvalidAccountID()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                
                int count = clsEmps.getCount(-1);
                Assert.IsTrue(count == 0);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for deleteEmployee
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cEmployeesTest_deleteEmployee_noConstraints()
        {
            int accountid = cGlobalVariables.AccountID;
            cEmployees target = new cEmployees(accountid);
            cEmployee employee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
            int expected = 0;
            int actual;
            actual = target.deleteEmployee(employee.employeeid);

            Assert.IsTrue(actual == 4); // should get return value that says employee not archived

            target.archiveEmployee(employee.username, accountid);
            actual = target.deleteEmployee(employee.employeeid);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for deleteEmployee
        ///</summary>
        [TestMethod()]
        public void deleteEmployee_constraints()
        {
            int accountid = cGlobalVariables.AccountID;
            cEmployees target = new cEmployees(accountid);
            cEmployee employee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

            try
            {
                //case 1:
                //    alert('This employee cannot be deleted as they are assigned to one or more Signoff Groups.');
                //    break;
                cGroup signoff = cGroupObject.CreateObject();
                cStage stage = cStageObject.FromTemplateWithType2(cGlobalVariables.EmployeeID);
                cGroups signoffs = new cGroups(cGlobalVariables.AccountID);
                signoffs.addStage(signoff.groupid, stage.signofftype, employee.employeeid, (int)stage.include, stage.amount, stage.notify, stage.onholiday, stage.holidaytype, stage.holidayid, stage.includeid, stage.claimantmail, stage.singlesignoff, stage.sendmail, stage.displaydeclaration, cGlobalVariables.EmployeeID, stage.signoffid, false);
                //case 2:
                //    alert('This employee cannot be deleted as they have one or more advances allocated to them.');
                //    break;
                
                cFloat advance = cFloatObject.CreateObject();
                //case 3:
                //    alert('This employee is currently set as a budget holder.');
                //    break;
                cBudgetHolder budgetHolder = cBudgetHolderObject.CreateObject();
                //case 4:
                //    alert('You must archive an employee before it can be deleted.');
                //    break;
                //case 5:
                //    alert('This employee cannot be deleted as they are the owner of one or more contracts.');
                //    break;
                //case 6:
                //    alert('This employee cannot be deleted as they are on one or more contract audiences as an individual.');
                //    break;
                //case 7:
                //    alert('This employee cannot be deleted as they are on one or more attachment audiences as an individual.');
                //    break;
                //case 8:
                //    alert('This employee cannot be deleted as they are on one or more contract notification lists.');
                //    break;
                //case 9:
                //    alert('This employee cannot be deleted as they are the leader of one or more teams.');
                //    break;
                //case 10:
                //    alert('This employee cannot be deleted as it would leave one or more empty teams.');
                //    break;
                target.archiveEmployee(employee.username, accountid);

                int expected = 1;
                int actual;
                actual = target.deleteEmployee(employee.employeeid);

                Assert.AreEqual(expected, actual); // should get return value that says employee not archived

                //actual = target.deleteEmployee(employee.employeeid);

                //Assert.AreEqual(expected, actual);
            }
            catch
            {

            }
            finally
            {
                cEmployeeObject.DeleteUTEmployee(employee.employeeid, employee.username);
            }
        }


        /// <summary>
        ///A test for checkExpiry where the password expiry is turned on and the employee password is set to expire
        ///</summary>
        [TestMethod()]
        public void cEmployees_checkExpiryPasswordExpiresOnAndPasswordExpires()
        {
            cEmployee emp = null;
            Dictionary<string, string> properties = new Dictionary<string, string>();
            cAccountSubAccounts clsProps = new cAccountSubAccounts(cGlobalVariables.AccountID);

            try
            {
                //Set the properties for the test so that the password will expire
                
                properties.Add("pwdExpires", "1");
                properties.Add("pwdExpiryDays", "-2");
                
                clsProps.SaveProperties(cGlobalVariables.SubAccountID, properties, cGlobalVariables.EmployeeID, null);
                
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateWithPasswordLastChangedExpiredObject());
                
                bool expired = clsEmps.checkExpiry(emp.employeeid);
                Assert.IsTrue(expired);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);

                    properties.Clear();
                    properties.Add("pwdExpires", "0");
                    properties.Add("pwdExpiryDays", "0");

                    clsProps.SaveProperties(cGlobalVariables.SubAccountID, properties, cGlobalVariables.EmployeeID, null);
                }
            }
        }

        /// <summary>
        ///A test for checkExpiry where the password expiry is turned on and the employee password date is not set to expire
        ///</summary>
        [TestMethod()]
        public void cEmployees_checkExpiryPasswordExpiresOnAndPasswordNotExpires()
        {
            cEmployee emp = null;
            Dictionary<string, string> properties = new Dictionary<string, string>();
            cAccountSubAccounts clsProps = new cAccountSubAccounts(cGlobalVariables.AccountID);            
            
            try
            {
                //Set the properties for the test so that the password will check for expiry but the date will not expire

                properties.Add("pwdExpires", "1");
                properties.Add("pwdExpiryDays", "5");

                clsProps.SaveProperties(cGlobalVariables.SubAccountID, properties, cGlobalVariables.EmployeeID, null);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateWithPasswordLastChangedExpiredObject());

                bool expired = clsEmps.checkExpiry(emp.employeeid);
                Assert.IsFalse(expired);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                    properties.Clear();
                    properties.Add("pwdExpires", "0");
                    properties.Add("pwdExpiryDays", "0");

                    clsProps.SaveProperties(cGlobalVariables.SubAccountID, properties, cGlobalVariables.EmployeeID, null);
                }
            }
        }

        /// <summary>
        ///A test for checkExpiry where the password expiry is turned off
        ///</summary>
        [TestMethod()]
        public void cEmployees_checkExpiryPasswordExpiresOff()
        {
            cEmployee emp = null;
            Dictionary<string, string> properties = new Dictionary<string, string>();
            cAccountSubAccounts clsProps = new cAccountSubAccounts(cGlobalVariables.AccountID);

            try
            {
                //Set the properties for the test so that the password will expire
                
                properties.Add("pwdExpires", "0");

                clsProps.SaveProperties(cGlobalVariables.SubAccountID, properties, cGlobalVariables.EmployeeID, null);

                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateWithPasswordLastChangedExpiredObject());

                bool expired = clsEmps.checkExpiry(emp.employeeid);
                Assert.IsFalse(expired);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for checkEmailIsUnique with user email that already exists
        ///</summary>
        [TestMethod()]
        public void cEmployees_checkEmailIsUniqueWithUserEmailThatAlreadyExists()
        {
            cEmployee emp = null;
            cEmployee emp2 = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                // create 2nd employee that should have different name but same email address.
                emp2 = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                bool isUnique = cEmployees.checkEmailIsUnique(emp.email);
                Assert.IsFalse(isUnique);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                    cEmployeeObject.DeleteUTEmployee(emp2.employeeid, emp2.username);
                }
            }
        }

        /// <summary>
        ///A test for checkEmailIsUnique with user email that does not already exists
        ///</summary>
        [TestMethod()]
        public void cEmployees_checkEmailIsUniqueWithUserEmailThatDoesNotAlreadyExists()
        {
            bool isUnique = cEmployees.checkEmailIsUnique("ut" + DateTime.Now.Ticks + "@ut.com");
            Assert.IsTrue(isUnique);
        }

        /// <summary>
        ///A test for GetEmployeeRetryCount
        ///</summary>
        [TestMethod()]
        public void cEmployees_GetEmployeeRetryCount()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                int retryCount = -1;
                retryCount = clsEmps.getCount(cGlobalVariables.AccountID);
                Assert.IsTrue(retryCount > -1);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for changeStatus with archive set to true
        ///</summary>
        [TestMethod()]
        public void cEmployees_changeStatusWithArchivedTrue()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                clsEmps.changeStatus(emp.employeeid, true);

                emp = clsEmps.GetEmployeeById(emp.employeeid);

                Assert.IsTrue(emp.archived);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }

        }

        /// <summary>
        ///A test for changeStatus with archive set to false
        ///</summary>
        [TestMethod()]
        public void cEmployees_changeStatusWithArchivedFalse()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                //By default new employees are set to be not archived so to make sure the tset is valid the employee needs to be set to archived first
                clsEmps.changeStatus(emp.employeeid, true);
                emp = clsEmps.GetEmployeeById(emp.employeeid);
                Assert.IsTrue(emp.archived);

                clsEmps.changeStatus(emp.employeeid, false);
                emp = clsEmps.GetEmployeeById(emp.employeeid);
                Assert.IsFalse(emp.archived);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }

        }

        /// <summary>
        ///A test for changePassword with check old password on and the old password incorrect
        ///</summary>
        [TestMethod()]
        public void cEmployees_changePasswordCheckOldPasswordTrueOldPasswordIncorrect()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                byte returnCode = clsEmps.changePassword(cGlobalVariables.AccountID, emp.employeeid, "IncorrectPassword", "NewPassword", true, 0);
                Assert.AreEqual(1, returnCode);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for changePassword with check old password on and the old password correct
        ///</summary>
        [TestMethod()]
        public void cEmployees_changePasswordCheckOldPasswordTrueOldPasswordCorrect()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                byte returnCode = clsEmps.changePassword(cGlobalVariables.AccountID, emp.employeeid, emp.password, "NewPassword", true, 0);
                Assert.AreEqual(0, returnCode);

                #region Get the new password and compare

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                cSecureData clsSecureData = new cSecureData();

                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                string strSQL = "SELECT password FROM employees WHERE employeeid = @employeeID";

                string newPassword = string.Empty;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        newPassword = reader.GetString(0);
                    }
                    reader.Close();
                }

                #endregion

                Assert.AreEqual("NewPassword", clsSecureData.Decrypt(newPassword));

                db.sqlexecute.Parameters.Clear();
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for changePassword with check old password off
        ///</summary>
        [TestMethod()]
        public void cEmployees_changePasswordCheckOldPasswordFalse()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                byte returnCode = clsEmps.changePassword(cGlobalVariables.AccountID, emp.employeeid, emp.password, "NewPassword", false, 0);
                Assert.AreEqual(0, returnCode);

                #region Get the new password and compare

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                cSecureData clsSecureData = new cSecureData();

                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                string strSQL = "SELECT password FROM employees WHERE employeeid = @employeeID";

                string newPassword = string.Empty;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        newPassword = reader.GetString(0);
                    }
                    reader.Close();
                }

                #endregion
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for changePassword with check new password incorrect as this means the password policy is not being met
        ///</summary>
        [TestMethod()]
        public void cEmployees_changePasswordWithPolicyIncorrect()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            cEmployee emp = null;

            try
            {
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                byte returnCode = clsEmps.changePassword(cGlobalVariables.AccountID, emp.employeeid, emp.password, "NewPassword", false, 2);
                Assert.AreEqual(2, returnCode);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for archiveEmployee
        ///This is on the logon page so the unit test will test from the database as there wont be an employee object available outside
        ///of logging on
        ///</summary>
        [TestMethod()]
        public void cEmployee_archiveEmployee()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                clsEmps.archiveEmployee(emp.username, cGlobalVariables.AccountID);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                string strSQL = "SELECT archived FROM employees WHERE employeeid = @employeeID";

                bool archived = false;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        archived = reader.GetBoolean(0);
                    }
                    reader.Close();
                }

                db.sqlexecute.Parameters.Clear();

                Assert.IsTrue(archived);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for alreadyExists where the employee exists and action is 0
        ///</summary>
        [TestMethod()]
        public void cEmployees_alreadyExistsEmployeeExistsActionZero()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                bool exists = clsEmps.alreadyExists(emp.username,0, emp.employeeid, cGlobalVariables.AccountID);
                Assert.IsTrue(exists);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for alreadyExists where the employee does not exist and action is 0
        ///</summary>
        [TestMethod()]
        public void cEmployees_alreadyExistsEmployeeNotExistsActionZero()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);

            bool exists = clsEmps.alreadyExists("utUnique" + DateTime.Now.Ticks.ToString(), 0, 0, cGlobalVariables.AccountID);
            Assert.IsFalse(exists);
        }

        /// <summary>
        ///A test for alreadyExists where the employee does not exist and action is greater than 0
        ///</summary>
        [TestMethod()]
        public void cEmployees_alreadyExistsEmployeeNotExistsActionGreaterThanZero()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);

            bool exists = clsEmps.alreadyExists("utUnique" + DateTime.Now.Ticks.ToString(), 1, 0, cGlobalVariables.AccountID);
            Assert.IsFalse(exists);
        }


        /// <summary>
        ///A test for AddPasswordKey with send date set
        ///</summary>
        [TestMethod()]
        public void cEmployees_AddPasswordKeyWithSendDateSet()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            string uKey = string.Empty;

            try
            {
                uKey = clsEmps.AddPasswordKey(cGlobalVariables.EmployeeID, cEmployees.PasswordKeyType.NewEmployee, DateTime.Now.AddDays(1));
                
                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                db.AddWithValue("@uniqueKey", uKey, 50);
                string strSQL = "SELECT COUNT(uniqueKey) FROM employeePasswordKeys WHERE uniqueKey=@uniqueKey";

                int count = db.getcount(strSQL);
                db.sqlexecute.Parameters.Clear();
                Assert.IsTrue(count > 0);
            }
            finally
            {
                clsEmps.RemovePasswordKey(uKey);
            }
        }

        /// <summary>
        ///A test for AddPasswordKey with send date not set
        ///</summary>
        [TestMethod()]
        public void cEmployees_AddPasswordKeyWithSendDateNotSet()
        {
            cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
            string uKey = string.Empty;

            try
            {
                uKey = clsEmps.AddPasswordKey(cGlobalVariables.EmployeeID, cEmployees.PasswordKeyType.NewEmployee, null);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                db.AddWithValue("@uniqueKey", uKey, 50);
                string strSQL = "SELECT COUNT(uniqueKey) FROM employeePasswordKeys WHERE uniqueKey=@uniqueKey";

                int count = db.getcount(strSQL);
                db.sqlexecute.Parameters.Clear();
                Assert.IsTrue(count > 0);
            }
            finally
            {
                clsEmps.RemovePasswordKey(uKey);
            }
        }

        /// <summary>
        ///A test for activateAccount
        ///</summary>
        [TestMethod()]
        public void cEmployees_activateAccount()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));

                //Need to set the employee as inactive as they are active by default
                string strSQL = "update employees set active = 0 where employeeid = @employeeid";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                db.ExecuteSQL(strSQL);
                db.sqlexecute.Parameters.Clear();

                clsEmps.activateAccount(emp.employeeid);

                //Check the employee is active
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                strSQL = "SELECT active FROM employees WHERE employeeid = @employeeID";

                bool active = false;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                    }
                    reader.Close();
                }

                db.sqlexecute.Parameters.Clear();

                Assert.IsTrue(active);
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for activateAccount and ESR Assignment with valid start and end date
        ///</summary>
        [TestMethod()]
        public void cEmployees_activateAccountWithValidESRAssignment()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                emp = cEmployeeObject.CreateUTEmployeeInactiveESRAssignmentNumber(emp.employeeid, DateTime.Now, null);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));

                //Need to set the employee as inactive as they are active by default
                string strSQL = "update employees set active = 0 where employeeid = @employeeid";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                db.ExecuteSQL(strSQL);
                db.sqlexecute.Parameters.Clear();

                clsEmps.activateAccount(emp.employeeid);

                //Check the employee is active
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                strSQL = "SELECT active FROM employees WHERE employeeid = @employeeID";

                bool active = false;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                    }
                    reader.Close();
                }

                db.sqlexecute.Parameters.Clear();

                Assert.IsTrue(active);

                // check ALL associated esr assignment numbers are activated
                db.sqlexecute.Parameters.Clear();
                strSQL = "select active from esr_assignments where employeeid = @employeeID";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                
                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                        if (!active)
                        {
                            Assert.Fail("ESR Assignment number for employee failed to activate in cEmployees_activateAccountWithValidESRAssignment()");
                            break;
                        }
                    }
                    reader.Close();
                }
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for activateAccount with ESR Assignment number with invalid end
        ///</summary>
        [TestMethod()]
        public void cEmployees_activateAccountWithInvalidEndDateESRAssignment()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                emp = cEmployeeObject.CreateUTEmployeeInactiveESRAssignmentNumber(emp.employeeid, DateTime.Now.AddYears(-5), DateTime.Now);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));

                //Need to set the employee as inactive as they are active by default
                string strSQL = "update employees set active = 0 where employeeid = @employeeid";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                db.ExecuteSQL(strSQL);
                db.sqlexecute.Parameters.Clear();

                clsEmps.activateAccount(emp.employeeid);

                //Check the employee is active
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                strSQL = "SELECT active FROM employees WHERE employeeid = @employeeID";

                bool active = false;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                    }
                    reader.Close();
                }

                db.sqlexecute.Parameters.Clear();

                Assert.IsTrue(active);

                // check ALL associated esr assignment numbers are activated
                db.sqlexecute.Parameters.Clear();
                strSQL = "select active from esr_assignments where employeeid = @employeeID";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                        if (active)
                        {
                            Assert.Fail("ESR Assignment number for employee activated incorrectly in cEmployees_activateAccountWithInvalidEndDateESRAssignment()");
                            break;
                        }
                    }
                    reader.Close();
                }
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }

        /// <summary>
        ///A test for activateAccount with ESR Assignment number with invalid start date
        ///</summary>
        [TestMethod()]
        public void cEmployees_activateAccountWithInvalidStartDateESRAssignment()
        {
            cEmployee emp = null;

            try
            {
                cEmployees clsEmps = new cEmployees(cGlobalVariables.AccountID);
                emp = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());
                emp = cEmployeeObject.CreateUTEmployeeInactiveESRAssignmentNumber(emp.employeeid, DateTime.Now.AddDays(1), null);

                DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));

                //Need to set the employee as inactive as they are active by default
                string strSQL = "update employees set active = 0 where employeeid = @employeeid";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                db.ExecuteSQL(strSQL);
                db.sqlexecute.Parameters.Clear();

                clsEmps.activateAccount(emp.employeeid);

                //Check the employee is active
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);
                strSQL = "SELECT active FROM employees WHERE employeeid = @employeeID";

                bool active = false;

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                    }
                    reader.Close();
                }

                db.sqlexecute.Parameters.Clear();

                Assert.IsTrue(active);

                // check ALL associated esr assignment numbers are activated
                db.sqlexecute.Parameters.Clear();
                strSQL = "select active from esr_assignments where employeeid = @employeeID";
                db.sqlexecute.Parameters.AddWithValue("@employeeID", emp.employeeid);

                using (SqlDataReader reader = db.GetReader(strSQL))
                {
                    while (reader.Read())
                    {
                        active = reader.GetBoolean(0);
                        if (active)
                        {
                            Assert.Fail("ESR Assignment number for employee activated incorrectly in cEmployees_activateAccountWithInvalidStartDateESRAssignment()");
                            break;
                        }
                    }
                    reader.Close();
                }
            }
            finally
            {
                if (emp != null)
                {
                    cEmployeeObject.DeleteUTEmployee(emp.employeeid, emp.username);
                }
            }
        }
    }
}
