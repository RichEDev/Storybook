using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Relationship_Information.Financial_Status
{
    /// <summary>
    /// Summary description for Financial Tests
    /// </summary>
    [CodedUITest]
    public class FinancialStatusTests
    {
        public FinancialStatusTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.FinancialStatusUIMapClasses.FinancialStatusUIMap cFinancialStatus = new UIMaps.FinancialStatusUIMapClasses.FinancialStatusUIMap();


        /// <summary>
        /// This test ensures that a new financial status definition can be created
        /// </summary>
        [TestMethod]
        public void FinancialStatusSuccessfullyCreateNewFinancialStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");

            /// Ensure the cancel button can be used when adding a financial status
            cFinancialStatus.AddFinancialStatusWithCancel();
            cFinancialStatus.ValidateAddFinancialStatusWithCancel();

            /// Add a financial status definition
            cFinancialStatus.AddFinancialStatus();
            cFinancialStatus.ValidateAddFinancialStatus();
        }


        /// <summary>
        /// This test ensures that duplicate financial status definitions cannot be created
        /// </summary>
        [TestMethod]
        public void FinancialStatusUnsuccessfullyCreateNewFinancialStatusWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");

            /// Ensure a duplicate cannot be added           
            cFinancialStatus.AddDuplicateFinancialStatus();
            cFinancialStatus.ValidateAddDuplicateFinancialStatus();
        }


        /// <summary>
        /// This test ensures that a financial status can be edited
        /// </summary>
        [TestMethod]
        public void FinancialStatusSuccessfullyEditFinancialStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");

            /// Ensure the cancel button can be used when editing
            cFinancialStatus.EditFinancialStatusWithCancel();
            cFinancialStatus.ValidateAddFinancialStatus();

            /// Edit a financial status
            cFinancialStatus.EditFinancialStatus();
            cFinancialStatus.ValidateEditFinancialStatus();
        }


        /// <summary>
        /// This test ensures that a financial status can be deleted
        /// </summary>
        [TestMethod]
        public void FinancialStatusSuccessfullyDeleteFinancialStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");

            /// Ensure the cancel button can be used
            cFinancialStatus.DeleteFinancialStatusWithCancel();
            cFinancialStatus.ValidateAddFinancialStatus();

            /// Delete a fincial status
            cFinancialStatus.DeleteFinancialStatus();
            cFinancialStatus.ValidateAddFinancialStatusWithCancel();           
        }


        /// <summary>
        /// This test ensures that a financial status can be archived
        /// </summary>
        [TestMethod]
        public void FinancialStatusArchiveFinancialStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");

            /// Ensure the cancel button can be used
            cFinancialStatus.ArchiveFinancialStatusWithCancel();
            cFinancialStatus.ValidateAddFinancialStatus();

            // Archive a financial status
            cFinancialStatus.ArchiveFinancialStatus();
            cFinancialStatus.ValidateArchiveFinancialStatus();
        }


        /// <summary>
        /// This test ensures that a financial status can be un-archived
        /// </summary>
        [TestMethod]
        public void FinancialStatusUnArchiveFinancialStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");

            /// Ensure the cancel button can be used
            cFinancialStatus.UnArchiveFinancialStatusWithCancel();
            cFinancialStatus.ValidateArchiveFinancialStatus();

            /// Un-archive a financial status
            cFinancialStatus.UnArchiveFinancialStatus();
            cFinancialStatus.ValidateUnArchiveFinancialStatus();
        }


        /// <summary>
        /// This test ensures that the page layout is correct for Financial Details
        /// </summary>
        [TestMethod]
        public void FinancialStatusSuccessfullyValidatePageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Financial Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=129");
            
            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cFinancialStatus.ValidateFinancialStatusPageLayoutExpectedValues.UILynneHunt16SeptemberPaneDisplayText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);

            /// Validate the page layout
            cFinancialStatus.ValidateFinancialStatusPageLayout();
        }


        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        #endregion

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
        private TestContext testContextInstance;
    }
}
