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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Contract_Configuration_Options.Contract_Types
{
    /// <summary>
    /// Summary description for Contract Types
    /// </summary>
    [CodedUITest]
    public class ContractTypesTests
    {
        public ContractTypesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ContractTypesUIMapClasses.ContractTypesUIMap cContractTypes = new UIMaps.ContractTypesUIMapClasses.ContractTypesUIMap();


        /// <summary>
        /// This test ensures that a new contract type definition can be created
        /// </summary>
        [TestMethod]
        public void ContractTypesSuccessfullyCreateNewContractType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Ensure the cancel button can be used when adding
            cContractTypes.AddContractTypeWithCancel();
            cContractTypes.ValidateAddContractTypeWithCancel();

            /// Add a new contract type definition
            cContractTypes.AddContractType();
            cContractTypes.ValidateAddContractType();
        }


        /// <summary>
        /// This test ensures that duplicate contract type definitions cannot be created
        /// </summary>
        [TestMethod]
        public void ContractTypesUnsuccessfullyCreateNewContractTypeWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Ensure a contract type with duplicate details cannot be created
            cContractTypes.AddContractTypeWithDuplicateDetails();
            cContractTypes.ValidateAddContractTypeWithDuplicateDetails();
        }


        /// <summary>
        /// This test ensures that a contract type definition can be successfully edited
        /// </summary>
        [TestMethod]
        public void ContractTypesSuccessfullyEditContractType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Ensure the cancel button can be used when editing
            cContractTypes.EditContractTypeWithCancel();
            cContractTypes.ValidateAddContractType();

            /// Edit a contract type definition
            cContractTypes.EditContractType();
            cContractTypes.ValidateEditContractType();

            /// Reset the values for future tests
            cContractTypes.EditContractTypeResetValues();
        }


        /// <summary>
        /// This test ensures that a contract type definition can be successfully deleted
        /// </summary>
        [TestMethod]
        public void ContractTypesSuccessfullyDeleteContractType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Ensure the cancel button can be used when deleting
            cContractTypes.DeleteContractTypeWithCancel();
            cContractTypes.ValidateAddContractType();

            /// Delete a contract type definition
            cContractTypes.DeleteContractType();
            cContractTypes.ValidateAddContractTypeWithCancel();
        }


        /// <summary>
        /// This test ensures that a contract type definition can be successfully archived
        /// </summary>
        [TestMethod]
        public void ContractTypesSuccessfullyArchiveContractType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Ensure the cancel button can be used when archiving
            cContractTypes.ArchiveContractTypeWithCancel();
            cContractTypes.ValidateContractTypeIsNotArchived();

            /// Archive a contract type definition
            cContractTypes.ArchiveContractType();
            cContractTypes.ValidateContractTypeIsArchived();
        }


        /// <summary>
        /// This test ensures that a contract type definition can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void ContractTypesSuccessfullyUnarchiveContractType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Ensure the cancel button can be used when un-archiving
            cContractTypes.UnArchiveContractTypeWithCancel();
            cContractTypes.ValidateContractTypeIsArchived();

            /// Un-archive a contract type definition
            cContractTypes.UnArchiveContractType();
            cContractTypes.ValidateContractTypeIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the page layout is correct for contract types
        /// </summary>
        [TestMethod]
        public void ContractTypesSuccessfullyValidateContractTypesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to contract types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=110");

            /// Validate the page layout for contract types

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";
            
            cContractTypes.ValidateContractTypesPageLayoutExpectedValues.UIJamesLloyd111117SeptPaneDisplayText = 
                cGlobalVariables.AdministratorUserName(ProductType.framework);


            cContractTypes.ValidateContractTypesPageLayoutExpectedValues.UICompanyPolicyHelpSupPaneInnerText =
                "Tasks | Switch Sub Account | About | Exit Home : Administration : Base Information : Contract Configuration Options : Contract Types ";

            cContractTypes.ValidateContractTypesPageLayout();
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
