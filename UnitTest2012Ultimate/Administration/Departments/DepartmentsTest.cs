using SpendManagementLibrary;
using Spend_Management;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate
{
    /// <summary>
    /// Tests the cDepartments class
    /// </summary>
    [TestClass]
    public class DepartmentsTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion

        #region deleteDepartment

        /// <summary>
        /// check that a department that isn't assigned to any entities can be deleted by a non-delegate user
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Departments")]
        public void DeleteDepartment_UserIsNotDelegate()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            cDepartments departments = new cDepartments(currentUser.AccountID);
            cDepartment department = new cDepartment(0, "Test Department",
                                                     "For test Department_DeleteDepartment_UserIsNotDelegate", false,
                                                     DateTime.Now, currentUser.EmployeeID, null,
                                                     null, new SortedList<int, object>());
            int departmentId = departments.SaveDepartment(department);

            Assert.AreEqual(0, departments.DeleteDepartment(departmentId, currentUser.EmployeeID));

        }

        /// <summary>
        /// check that a department that isn't assigned to any entities can be deleted by a delegate user
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Departments")]
        public void DeleteDepartment_UserIsDelegate()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUserDelegateMock().Object;
            HelperMethods.SetTestDelegateID();
            cDepartments departments = new cDepartments(currentUser.AccountID);
            cDepartment department = new cDepartment(0, "Test Department",
                                                     "For test Department_DeleteDepartment_UserIsDelegate", false,
                                                     DateTime.Now, currentUser.Delegate.EmployeeID, null, null, new SortedList<int, object>());
            int departmentId = departments.SaveDepartment(department);

            Assert.AreEqual(0, departments.DeleteDepartment(departmentId, currentUser.EmployeeID));

        }

        #endregion

    }
}
