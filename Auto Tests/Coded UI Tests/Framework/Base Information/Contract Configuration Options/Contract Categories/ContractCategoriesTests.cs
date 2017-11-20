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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Contract_Configuration_Options.Contract_Categories
{
    /// <summary>
    /// Summary description for Contract Categories
    /// </summary>
    [CodedUITest]
    public class ContractCategoriesTests
    {
        public ContractCategoriesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ContractCategoriesUIMapClasses.ContractCategoriesUIMap cContractCategories = new UIMaps.ContractCategoriesUIMapClasses.ContractCategoriesUIMap();

        
        /// <summary>
        /// This test ensures that a new contract category definition can be created
        /// </summary>
        [TestMethod]
        public void ContractCategoriesSuccessfullyCreateNewContractCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            /// Ensure the cancel button can be used
            cContractCategories.AddContractCategoryWithCancel();
            cContractCategories.ValidateContractCategoryDoesNotExist();

            /// Add a contract category definition
            cContractCategories.AddContractCategory();
            cContractCategories.ValidateAddContractCategory();
        }


        /// <summary>
        /// This test ensures that duplicate contract category definitions cannot be created
        /// </summary>
        [TestMethod]
        public void ContractCategoriesUnSuccessfullyCreateNewContractCategoryWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            /// Attempt to add a duplicate contract category
            cContractCategories.AddContractCategory();
            cContractCategories.ValidateAddContractCategoryWithDuplicateDetails();
        }


        /// <summary>
        /// This test ensures that a contract category can be successfully edited
        /// </summary>
        [TestMethod]
        public void ContractCategoriesSuccessfullyEditContractCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            /// Ensure the cancel button can be used
            cContractCategories.EditContractCategoryWithCancel();
            cContractCategories.ValidateAddContractCategory();

            /// Edit a contract category
            cContractCategories.EditContractCategory();
            cContractCategories.ValidateEditContractCategory();

            /// Reset values for future tests
            cContractCategories.EditContractCategoryResetValues();
        }


        /// <summary>
        /// This test ensures that a contract category can be successfully deleted
        /// </summary>
        [TestMethod]
        public void ContractCategoriesSuccessfullyDeleteContractCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            /// Ensure the cancel button can be used when deleting
            cContractCategories.DeleteContractCategoryWithCancel();
            cContractCategories.ValidateAddContractCategory();

            /// Delete contract category
            cContractCategories.DeleteContractCategory();
            cContractCategories.ValidateContractCategoryDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a contract category can be archived
        /// </summary>
        [TestMethod]
        public void ContractCategoriesSuccessfullyArchiveContractCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            /// Ensure the cancel button can be used when archiving
            cContractCategories.ArchiveContractCategoryWithCancel();
            cContractCategories.ValidateContractCategoryIsNotArchived();

            /// Archive a contract category
            cContractCategories.ArchiveContractCategory();
            cContractCategories.ValidateContractCategoryIsArchived();
        }


        /// <summary>
        /// This test ensures that a contract category can be un-archived
        /// </summary>
        [TestMethod]
        public void ContractCategoriesSuccessfullyUnarchiveContractCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            /// Ensure the cancel button can be used when un-archiving
            cContractCategories.UnArchiveContractCategoryWithCancel();
            cContractCategories.ValidateContractCategoryIsArchived();

            /// Un-archive a contract category
            cContractCategories.UnArchiveContractCategory();
            cContractCategories.ValidateContractCategoryIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the page layout is correct for Contract Categories
        /// </summary>
        [TestMethod]
        public void ContractCategoriesSuccessfullyValidateContractCategoriesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the contract categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=109");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cContractCategories.ValidateContractCategoryPageLayoutExpectedValues.UIJamesLloyd111117SeptPaneDisplayText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);

            /// Valdiate the page layout
            cContractCategories.ValidateContractCategoryPageLayout();
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
