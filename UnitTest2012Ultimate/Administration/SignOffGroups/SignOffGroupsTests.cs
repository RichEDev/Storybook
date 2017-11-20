using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using SpendManagementLibrary.Employees;
using Spend_Management;

namespace UnitTest2012Ultimate.Administration.SignOffGroups
{
    [TestClass]
    public class SignOffGroupsTests
    {
        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        ///A test for saving a sign off group
        ///</summary>
        [TestMethod]
        public void SignOffGroups_SaveNewGroup()
        {
            int accountId = GlobalTestVariables.AccountId;
            cGroups groups = new cGroups(accountId);
            int outcome = 0;
            ICurrentUser currentUser = Moqs.CurrentUser();
            try
            {
                string groupName = "UT " + DateTime.UtcNow.Ticks + " Test Group";
                string groupDescription = "UT " + DateTime.UtcNow.Ticks + " Test Desc";
                outcome = groups.SaveGroup(0, groupName, groupDescription, true, currentUser, 0);

                Assert.IsTrue(outcome > 0);
            }
            finally
            {
                TearDown(outcome, groups, currentUser);
            }

        }

        /// <summary>
        ///A test for saving a sign off group with existing group name
        ///</summary>
        [TestMethod]
        public void SignOffGroups_SaveNewGroup_UsingSignOffGroupName()
        {
            int accountId = GlobalTestVariables.AccountId;
            cGroups groups = new cGroups(accountId);
            int outcome = 0;
            ICurrentUser currentUser = Moqs.CurrentUser();
            try
            {
                string groupName = "UT " + DateTime.UtcNow.Ticks + " Test Group";
                string groupDescription = "UT " + DateTime.UtcNow.Ticks + " Test Desc";
                outcome = groups.SaveGroup(0, groupName, groupDescription, true, currentUser, 0);

                Assert.IsTrue(outcome > 0);

                outcome = groups.SaveGroup(0, groupName, groupDescription, true, currentUser, 0);             
                Assert.AreEqual(outcome, -1);

            }
            finally
            {
                TearDown(outcome, groups, currentUser);
            }

        }

        /// <summary>
        ///A test for editing a sign off group
        ///</summary>
        [TestMethod]
        public void SignOffGroups_EditExistingGroup()
        {
            int accountId = GlobalTestVariables.AccountId;
            cGroups groups = new cGroups(accountId);
            ICurrentUser currentUser = Moqs.CurrentUser();
            int outcome = 0;
            int outcome2 = 0;
            try
            {
                string groupName = "UT " + DateTime.UtcNow.Ticks + " Test Group";
                string groupDescription = "UT " + DateTime.UtcNow.Ticks + " Test Desc";
                string groupName2 = "UT " + DateTime.UtcNow.Ticks + " Test Group Updated";

                outcome = groups.SaveGroup(0, groupName, groupDescription, true, currentUser, 0);
                Assert.IsTrue(outcome > 0);
                outcome2 = groups.SaveGroup(outcome, groupName2, groupDescription, true, currentUser, 0);
             
                Assert.AreEqual(outcome, outcome2);

            }
            finally
            {
                TearDown(outcome, groups, currentUser);
            }
        }

        /// <summary>
        ///A test for deleting a sign off group 
        ///</summary>
        [TestMethod]
        public void SignOffGroups_DeleteExistingGroup()
        {
            int accountId = GlobalTestVariables.AccountId;
            cGroups groups = new cGroups(accountId);
            ICurrentUser currentUser = Moqs.CurrentUser();
            int outcome = 0;
            int outcome2 = 0;
            try
            {
                string groupName = "UT " + DateTime.UtcNow.Ticks + " Test Group";
                string groupDescription = "UT " + DateTime.UtcNow.Ticks + " Test Desc";

                outcome = groups.SaveGroup(0, groupName, groupDescription, true, currentUser, 0);
                Assert.IsTrue(outcome > 0);

                outcome2 = groups.DeleteGroup(outcome, currentUser);
                Assert.AreEqual(outcome, outcome2);

                cGroup group = groups.GetGroupById(outcome);
                Assert.IsNull(group);
            }
            finally
            {
                TearDown(outcome, groups, currentUser);
            }
        }


        /// <summary>
        ///A test for deleting a sign off group where the sign off group is assigned to an employee
        ///</summary>
        [TestMethod]
        public void SignOffGroups_DeleteExistingGroup_AssignedToEmployee()
        {
            int accountId = GlobalTestVariables.AccountId;
            cGroups groups = new cGroups(accountId);
            ICurrentUser currentUser = Moqs.CurrentUser();
            int outcome = 0;
            int outcome2 = 0;
            Employee employee = new Employee();
            try
            {
                string groupName = "UT " + DateTime.UtcNow.Ticks + " Test Group";
                string groupDescription = "UT " + DateTime.UtcNow.Ticks + " Test Desc";

                outcome = groups.SaveGroup(0, groupName, groupDescription, true, currentUser, 0);
                Assert.IsTrue(outcome > 0);
         
                employee = cEmployeeObject.GetUTEmployeeTemplateObject();
                employee.Save(Moqs.CurrentUser());
                cExpenseItemObject.UpdateEmployeeGroup(employee, outcome);
                outcome2 = groups.DeleteGroup(outcome, currentUser);
                Assert.AreEqual(-1, outcome2);
            }
            finally
            {
                employee.Save(currentUser);
                employee.Delete(currentUser);
                TearDown(outcome, groups, currentUser);
            }
        }

        private void TearDown(int groupId, cGroups groups, ICurrentUser currentUser)
        {
            if (groupId >= -1)
            {
                groups.DeleteGroup(groupId, currentUser);
            }
        }
    }
}
