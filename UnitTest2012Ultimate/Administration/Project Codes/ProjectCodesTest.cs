using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary;
using Spend_Management;

namespace UnitTest2012Ultimate
{
    /// <summary>
    /// Tests the cProjectCodes class
    /// </summary>
    [TestClass]
    public class ProjectCodesTest
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

        #region deleteProjectCode
        
        /// <summary>
        /// Check to make sure that a project code that is not assigned to any entities can be deleted by a user who is not a delegate
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("ProjectCodes")]
        public void DeleteProjectCode_UserIsNotDelegate()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUser();
            cProjectCodes projectCodes = new cProjectCodes(currentUser.AccountID);

            cProjectCode projectCode = new cProjectCode(0, "Test ProjectCode",
                                                        "For test ProjectCodesTest_DeleteProjectCode_UserIsNotDelegate",
                                                        false, false, DateTime.Now, currentUser.EmployeeID, null, null,
                                                        new SortedList<int, object>());

            int projectCodeId = projectCodes.saveProjectCode(projectCode);

            Assert.AreEqual(0, projectCodes.deleteProjectCode(projectCodeId, currentUser.EmployeeID));
        }

        /// <summary>
        /// Check to make sure that a project code that is not assigned to any entities can be deleted by a user who is a delegate
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("ProjectCodes")]
        public void DeleteProjectCode_UserIsDelegate()
        {
            ICurrentUserBase currentUser = Moqs.CurrentUserDelegateMock().Object;
            HelperMethods.SetTestDelegateID();

            cProjectCodes projectCodes = new cProjectCodes(currentUser.AccountID);
            cProjectCode projectCode = new cProjectCode(0, "Test ProjectCode",
                                                        "For test ProjectCodesTest_DeleteProjectCode_UserIsDelegate",
                                                        false, false, DateTime.Now, currentUser.Delegate.EmployeeID, null, null,
                                                        new SortedList<int, object>());

            int projectCodeId = projectCodes.saveProjectCode(projectCode);

            Assert.AreEqual(0, projectCodes.deleteProjectCode(projectCodeId, currentUser.Delegate.EmployeeID));
        }

        #endregion

    }
}
