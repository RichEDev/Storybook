using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System;
using System.Collections.Generic;
using SpendManagementLibrary;
using System.Web.UI.WebControls;
using SpendManagementUnitTests.Global_Objects;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cTeamsTest and is intended
    ///to contain all cTeamsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cTeamsTest
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
        ///A test for cTeams Constructor
        ///</summary>
        [TestMethod()]
        public void cTeams_cTeamsConstructor_accountAndSubAccount()
        {
            cTeam newTeam = cTeamObject.CreateObject();
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            Assert.IsNotNull(Cache[target.cacheKey]);

            Cache.Remove(target.cacheKey);

            target = new cTeams(accountid, subaccountid);

            Assert.IsNotNull(Cache[target.cacheKey]);

            // clearup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for cTeams Constructor
        ///</summary>
        [TestMethod()]
        public void cTeams_cTeamsConstructor_accountOnly()
        {
            cTeam newTeam = cTeamObject.CreateObject();
            System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;

            int accountid = cGlobalVariables.AccountID;
            cTeams target = new cTeams(accountid);

            Assert.IsNotNull(Cache[target.cacheKey]);

            Cache.Remove(target.cacheKey);

            target = new cTeams(accountid);

            Assert.IsNotNull(Cache[target.cacheKey]);
        }

        ///// <summary>
        /////A test for CacheList
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cTeams_CacheList_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cTeams_Accessor target = new cTeams_Accessor(param0); // TODO: Initialize to an appropriate value
        //    SortedList<int, cTeam> expected = null; // TODO: Initialize to an appropriate value
        //    SortedList<int, cTeam> actual;
        //    actual = target.CacheList();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestMethod()]
        public void cTeams_CreateDropDown_validTeamSelected()
        {
            cTeam team = cTeamObject.CreateObject();
            int teamid = team.teamid;

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            ListItem expected = new ListItem(team.teamname, team.teamid.ToString());
            ListItem[] actual;
            actual = target.CreateDropDown(teamid);

            Assert.IsNotNull(actual);

            bool selected = false;
            bool contains = false;

            foreach (ListItem li in actual)
            {
                if (li.Text == expected.Text && li.Value == expected.Value)
                {
                    contains = true;
                    if (li.Selected == true)
                    {
                        selected = true;
                    }
                    break;
                }
                
            }

            Assert.IsTrue(selected);
            Assert.IsTrue(contains);

            // cleanup only
            target.DeleteTeam(teamid);
        }

        /// <summary>
        ///A test for DeleteTeam
        ///</summary>
        [TestMethod()]
        public void cTeams_DeleteTeam_validID()
        {
            int teamID = cTeamObject.CreateID();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            int expected = 0;
            int actual;
            actual = target.DeleteTeam(teamID);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteTeam
        ///</summary>
        [TestMethod()]
        public void cTeams_DeleteTeam_invalid()
        {
            int teamID = cTeamObject.CreateID();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            int expected = 0;
            int actual;
            actual = target.DeleteTeam(-3);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteTeamEmp
        ///</summary>
        [TestMethod()]
        public void cTeams_DeleteTeamEmp_validEmployee()
        {
            cTeam newTeam = cTeamObject.CreateObject();
            SortedList<int, int> teamEmps = cTeamObject.GetTeamEmps(newTeam.teamid);

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int teamEmpID = -1;
            if (teamEmps.Count > 0)
            {
                foreach (KeyValuePair<int, int> kvp in teamEmps)
                {
                    if (kvp.Value != cGlobalVariables.EmployeeID)
                    {
                        teamEmpID = kvp.Key;
                        break;
                    }
                }
                if (teamEmpID == -1)
                {
                    Assert.Fail("Expected more than one employee in team employees, not including the global variable employeeid");
                }
            }

            target.DeleteTeamEmp(teamEmpID);

            SortedList<int, int> teamEmpsAfter = cTeamObject.GetTeamEmps(newTeam.teamid);

            Assert.IsTrue(teamEmpsAfter.Count == teamEmps.Count - 1);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for GetCombinedEmployeeListItems
        ///</summary>
        [TestMethod()]
        public void cTeams_GetCombinedEmployeeListItems_templateEmployeeAndTeam()
        {
            cTeam team = cTeamObject.CreateObject();
            int teamid = team.teamid;

            cEmployees clsEmployees = new cEmployees(cGlobalVariables.AccountID);
            cEmployee employee = clsEmployees.GetEmployeeById(cGlobalVariables.EmployeeID);

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            bool addNoneSelection = false;
            bool sorted = false;

            ListItem[] actual;
            actual = target.GetCombinedEmployeeListItems(addNoneSelection, sorted);

            Assert.IsNotNull(actual);

            bool noneEntry = false;
            bool containsEmp = false;
            bool containsTeam = false;

            foreach (ListItem li in actual)
            {
                if (li.Text == "[None]" && li.Value == "0")
                {
                    noneEntry = true;
                }

                if (li.Text.Contains(employee.firstname) && li.Value == employee.employeeid.ToString())
                {
                    containsEmp = true;
                }

                if (li.Text.Contains("*") && li.Text.Contains(team.teamname))
                {
                    containsTeam = true;
                }
            }

            Assert.IsFalse(noneEntry);
            Assert.IsTrue(containsEmp);
            Assert.IsTrue(containsTeam);

            noneEntry = false;
            containsEmp = false;
            containsTeam = false;

            foreach (ListItem li in actual)
            {
                if (li.Text == "[None]" && li.Value == "0")
                {
                    noneEntry = true;
                }

                if (li.Text.Contains(employee.firstname) && li.Value == employee.employeeid.ToString())
                {
                    containsEmp = true;
                }

                if (li.Text.Contains("*") && li.Text.Contains(team.teamname))
                {
                    containsTeam = true;
                }
            }

            Assert.IsFalse(noneEntry);
            Assert.IsTrue(containsEmp);
            Assert.IsTrue(containsTeam);

            // cleanup only
            target.DeleteTeam(teamid);
        }

        /// <summary>
        ///A test for GetMemberOfTeams
        ///</summary>
        [TestMethod()]
        public void cTeams_GetMemberOfTeams_templateEmployee()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            int employeeid = cGlobalVariables.EmployeeID;

            SortedList<int, cTeam> actual;
            actual = target.GetMemberOfTeams(employeeid);

            Assert.IsNotNull(actual);

            Assert.IsTrue(actual.ContainsKey(newTeam.teamid));
            cCompareAssert.AreEqual(actual[newTeam.teamid], newTeam);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for GetTeamById
        ///</summary>
        [TestMethod()]
        public void cTeams_GetTeamById_templateTeam()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            int teamid = newTeam.teamid;
            cTeam expected = newTeam;
            cTeam actual;
            actual = target.GetTeamById(teamid);
            cCompareAssert.AreEqual(expected, actual);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for GetTeamsListControlItems
        ///</summary>
        [TestMethod()]
        public void cTeams_GetTeamsListControlItems_all()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            bool addNoneSelection = false;
            bool sorted = false;
            ListItem expected = new ListItem(newTeam.teamname, "TEAM_" + newTeam.teamid.ToString());
            ListItem[] fullList = target.GetTeamsListControlItems(addNoneSelection, sorted);
            ListItem actual = null;
            for (int i = 0; i < fullList.Length; i++)
            {
                if (fullList[i].Text.Contains("*") && fullList[i].Text.Contains(newTeam.teamname))
                {
                    actual = fullList[i];
                }
            }

            actual.Text = actual.Text.Replace("*", "");
            Assert.AreEqual(expected, actual);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        ///// <summary>
        /////A test for InitialiseData
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cTeams_InitialiseData_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cTeams_Accessor target = new cTeams_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool resetCache = false; // TODO: Initialize to an appropriate value
        //    target.InitialiseData(resetCache);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for SaveTeam
        ///</summary>
        [TestMethod()]
        public void cTeams_SaveTeam_validTeam()
        {
            cTeam template = cTeamObject.FromValidTemplate();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int teamID = template.teamid;
            string teamname = template.teamname;
            string description = template.description;
            int leaderemployeeid = cGlobalVariables.EmployeeID;
            int EmployeeID = cGlobalVariables.EmployeeID;
            int actual = 0;
            actual = target.SaveTeam(teamID, teamname, description, leaderemployeeid, EmployeeID);

            Assert.IsTrue(actual > 0);

            // cleanup
            actual = target.DeleteTeam(actual);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        ///A test for SaveTeam
        ///</summary>
        [TestMethod()]
        public void cTeams_SaveTeam_validTeamNoLeaderID()
        {
            cTeam template = cTeamObject.FromValidTemplate();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int teamID = template.teamid;
            string teamname = template.teamname;
            string description = template.description;
            int leaderemployeeid = 0;
            int EmployeeID = cGlobalVariables.EmployeeID;
            int actual = 0;
            actual = target.SaveTeam(teamID, teamname, description, leaderemployeeid, EmployeeID);

            Assert.IsTrue(actual > 0);

            // cleanup
            actual = target.DeleteTeam(actual);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        ///A test for SaveTeam
        ///</summary>
        [TestMethod()]
        public void cTeams_SaveTeam_validTeamDuplicate()
        {
            cTeam template = cTeamObject.FromValidTemplate();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int teamID = template.teamid;
            string teamname = template.teamname;
            string description = template.description;
            int leaderemployeeid = cGlobalVariables.EmployeeID;
            int EmployeeID = cGlobalVariables.EmployeeID;
            int actual = 0;
            actual = target.SaveTeam(teamID, teamname, description, leaderemployeeid, EmployeeID);

            Assert.IsTrue(actual > 0);

            actual = target.SaveTeam(teamID, teamname, description, leaderemployeeid, EmployeeID);

            Assert.IsTrue(actual == -1);

            // cleanup
            actual = target.DeleteTeam(actual);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        ///A test for addTeamMember
        ///</summary>
        [TestMethod()]
        public void cTeams_addTeamMember_validNewMember()
        {
            cTeam newTeam = cTeamObject.CreateObject();
            cEmployee newEmployee = cEmployeeObject.CreateUTEmployeeObject(cEmployeeObject.GetUTEmployeeTemplateObject());

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int employeeid = newEmployee.employeeid;
            int teamid = newTeam.teamid;

            int actual;
            actual = target.addTeamMember(employeeid, teamid);
            Assert.IsTrue(actual == 0);

            cTeam alteredTeam = target.GetTeamById(newTeam.teamid);
            Assert.IsTrue(alteredTeam.teammembers.Count == newTeam.teammembers.Count + 1);

            // cleanup
            target.DeleteTeamEmp(actual);
            actual = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(actual == 0);
            cEmployeeObject.DeleteUTEmployee(newEmployee.employeeid, newEmployee.username);
        }

        /// <summary>
        ///A test for addTeamMember
        ///</summary>
        [TestMethod()]
        public void cTeams_addTeamMember_duplicateNewMember()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int employeeid = cGlobalVariables.EmployeeID;
            int teamid = newTeam.teamid;

            int memberCount = newTeam.teammembers.Count;

            int actual;
            actual = target.addTeamMember(employeeid, teamid);
            Assert.IsTrue(actual == 0);

            cTeam expected = target.GetTeamById(newTeam.teamid);
            Assert.IsTrue(memberCount == expected.teammembers.Count);

            // cleanup
            actual = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(actual == 0);
        }

        ///// <summary>
        /////A test for alreadyExists
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cTeams_alreadyExists_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cTeams_Accessor target = new cTeams_Accessor(param0); // TODO: Initialize to an appropriate value
        //    int action = 0; // TODO: Initialize to an appropriate value
        //    int teamid = 0; // TODO: Initialize to an appropriate value
        //    string teamname = string.Empty; // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.alreadyExists(action, teamid, teamname);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for deleteTeamMembers
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[UrlToTest("http://localhost/iteration2/sm")]
        //[DeploymentItem("Spend Management.dll")]
        //public void cTeams_deleteTeamMembers_()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    cTeams_Accessor target = new cTeams_Accessor(param0); // TODO: Initialize to an appropriate value
        //    int teamid = 0; // TODO: Initialize to an appropriate value
        //    target.deleteTeamMembers(teamid);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for getTeamByName
        ///</summary>
        [TestMethod()]
        public void cTeams_getTeamByName_templateName()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            string team = newTeam.teamname;
            cTeam actual;
            actual = target.getTeamByName(team);

            cCompareAssert.AreEqual(newTeam, actual);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for getTeamMemberGrid
        ///</summary>
        [TestMethod()]
        public void cTeams_getTeamMemberGrid_templateTeam()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int teamid = newTeam.teamid;
            string actual;
            actual = target.getTeamMemberGrid(teamid);
            Assert.IsTrue(actual.Length > 0);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for getTeamMembers
        ///</summary>
        [TestMethod()]
        public void cTeams_getTeamMembers_fromValidTemplate()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            SortedList<int, List<int>> actual;
            actual = target.getTeamMembers();

            Assert.IsTrue(actual.ContainsKey(newTeam.teamid));

            int count = 0;
            foreach (int i in actual[newTeam.teamid])
            {
                if (newTeam.teammembers.Contains(i))
                {
                    count++;
                }
            }

            Assert.IsTrue(count == newTeam.teammembers.Count);

            // cleanup
            int deleted = target.DeleteTeam(newTeam.teamid);
            Assert.IsTrue(deleted == 0);
        }

        /// <summary>
        ///A test for getTeamidByName
        ///</summary>
        [TestMethod()]
        public void cTeams_getTeamidByName_fromTemplateName()
        {
            cTeam newTeam = cTeamObject.CreateObject();

            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            string name = newTeam.teamname;
            int expected = newTeam.teamid;
            int actual;
            actual = target.getTeamidByName(name);
            Assert.AreEqual(expected, actual);

            // cleanup
            actual = target.DeleteTeam(actual);
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        ///A test for AccountID
        ///</summary>
        [TestMethod()]
        public void cTeams_AccountID_validAccountID()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);

            int actual;
            actual = target.AccountID;
            Assert.AreEqual(accountid, actual);
        }

        /// <summary>
        ///A test for SubAccountId
        ///</summary>
        [TestMethod()]
        public void cTeams_SubAccountId_accountOnlyConstructor()
        {
            int accountid = cGlobalVariables.AccountID;
            cTeams target = new cTeams(accountid);
            Nullable<int> actual;
            actual = target.SubAccountId;

            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for SubAccountId
        ///</summary>
        [TestMethod()]
        public void cTeams_SubAccountId_accountAndSubAccountConstructor()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid, subaccountid);
            Nullable<int> actual;
            actual = target.SubAccountId;

            Assert.IsTrue(actual.HasValue);
            Assert.AreEqual(subaccountid, actual.Value);
        }

        /// <summary>
        ///A test for cacheKey
        ///</summary>
        [TestMethod()]
        public void cTeams_cacheKey_bothConstructors()
        {
            int accountid = cGlobalVariables.AccountID;
            int subaccountid = cGlobalVariables.SubAccountID;
            cTeams target = new cTeams(accountid);
            string actual;
            actual = target.cacheKey;

            Assert.AreEqual(actual, "teams" + accountid.ToString());

            target = new cTeams(accountid, subaccountid);
            actual = target.cacheKey;

            Assert.AreEqual(actual, "teams" + accountid.ToString() + "_" + subaccountid.ToString());
        }
    }
}
