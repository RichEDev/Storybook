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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Contract_Configuration_Options.Contract_Status
{
    /// <summary>
    /// Summary description for Contract Status'
    /// </summary>
    [CodedUITest]
    public class ContractStatusTests
    {
        public ContractStatusTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ContractStatusUIMapClasses.ContractStatusUIMap cContractStatus = new UIMaps.ContractStatusUIMapClasses.ContractStatusUIMap();

        /// <summary>
        /// This test ensures that a new contract status can be successfully created
        /// </summary>
        [TestMethod]
        public void ContractStatusSuccessfullyCreateNewContractStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            /// Ensure the cancel button can be used
            cContractStatus.AddContractStatusWithCancel();
            cContractStatus.ValidateContractStatusDoesNotExist();

            /// Add a contract status
            cContractStatus.AddContractStatus();
            cContractStatus.ValidateAddContractStatus();
        }


        /// <summary>
        /// This test ensures that a contract status cannot be duplicated
        /// </summary>
        [TestMethod]
        public void ContractStatusUnSuccessfullyCreateNewContractStatusWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            /// Add a duplicate contract status
            cContractStatus.AddContractStatus();
            cContractStatus.ValidateAddDuplicateContractStatus();
        }


        /// <summary>
        /// This test ensures that a contract status can be successfully edited
        /// </summary>
        [TestMethod]
        public void ContractStatusSuccessfullyEditContractStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            /// Ensure the cancel button can be used
            cContractStatus.EditContractStatusWithCancel();
            cContractStatus.ValidateAddContractStatus();

            /// Edit a contract status
            cContractStatus.EditContractStatus();
            cContractStatus.ValidateEditContractStatus();

            /// Reset the values for future use
            cContractStatus.EditContractStatusResetValues();
        }


        /// <summary>
        /// This test ensures that a contract status can be successfully deleted
        /// </summary>
        [TestMethod]
        public void ContractStatusSuccessfullyDeleteContractStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            /// Ensure the cancel button can be used
            cContractStatus.DeleteContractStatusWithCancel();
            cContractStatus.ValidateAddContractStatus();

            /// Delete a contract status
            cContractStatus.DeleteContractStatus();
            cContractStatus.ValidateContractStatusDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a contract status can be successfully archived
        /// </summary>
        [TestMethod]
        public void ContractStatusSuccessfullyArchiveContractStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            /// Ensure the cancel button can be used
            cContractStatus.ArchiveContractStatusWithCancel();
            cContractStatus.ValidateContractStatusIsNotArchived();

            /// Archive a contract status
            cContractStatus.ArchiveContractStatus();
            cContractStatus.ValidateContractStatusIsArchived();
        }


        /// <summary>
        /// This test ensures that a contract status can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void ContractStatusSuccessfullyUnArchiveContractStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            /// Ensure the cancel button can be used
            cContractStatus.UnArchiveContractStatusWithCancel();
            cContractStatus.ValidateContractStatusIsArchived();

            /// Unarchive a contract status
            cContractStatus.UnArchiveContractStatus();
            cContractStatus.ValidateContractStatusIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the page layout for contract status' is correct
        /// </summary>
        [TestMethod]
        public void ContractStatusSuccessfullyValidateContractStatusPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=111");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cContractStatus.ValidateContractStatusPageLayoutExpectedValues.UIJamesLloyd21SeptembePaneDisplayText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);

            /// Validate the page layout
            cContractStatus.ValidateContractStatusPageLayout();
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

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void NavigateToHomePage()
        //{
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
