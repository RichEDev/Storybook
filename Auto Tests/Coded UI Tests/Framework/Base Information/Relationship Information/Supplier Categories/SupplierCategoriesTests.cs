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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Relationship_Information.Supplier_Categories
{
    /// <summary>
    /// This is the class containing all of the Supplier Categories
    /// </summary>
    [CodedUITest]
    public class SupplierCategoriesTests
    {
        public SupplierCategoriesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.SupplierCategoriesUIMapClasses.SupplierCategoriesUIMap cSupplierCategories = new UIMaps.SupplierCategoriesUIMapClasses.SupplierCategoriesUIMap();

        /// <summary>
        /// This test ensures a new supplier category can be successfully created
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesSuccessfullyCreateNewSupplierCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            /// Validate that the supplier category does not already exist
            cSupplierCategories.ValidateDeleteSupplierCategory();

            /// Add a new supplier category
            cSupplierCategories.AddSupplierCategory();
            cSupplierCategories.ValidateAddSupplierCategory();            
        }


        /// <summary>
        /// This test ensures a supplier category cannot be duplicated
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesUnsuccessfullyCreateNewSupplierWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator); ;

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            /// Add a duplicate Supplier Category
            cSupplierCategories.AddDuplicateSupplierCategory();
            cSupplierCategories.ValidateAddDuplicateSupplierCategory();
        }


        /// <summary>
        /// This test ensures a supplier category can be successfully edited
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesSuccessfullyEditSupplierCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            /// Check the cancel button can be used when editing
            cSupplierCategories.EditSupplierCategoryAndCancel();
            cSupplierCategories.ValidateAddSupplierCategory();

            /// Edit a supplier category
            cSupplierCategories.EditSupplierCategory();
            cSupplierCategories.ValidateEditSupplierCategory();

            /// Reset the supplier category for future tests
            cSupplierCategories.EditSupplierCategoryReset();
            cSupplierCategories.ValidateAddSupplierCategory();
        }


        /// <summary>
        /// This test ensures a supplier category can be successfully deleted
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesSuccessfullyDeleteSupplierCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            /// Check the cancel button can be used when deleting
            cSupplierCategories.DeleteSupplierCategoryAndCancel();
            cSupplierCategories.ValidateDeleteSupplierCategoryAndCancel();

            /// Delete a supplier category
            cSupplierCategories.DeleteSupplierCategory();
            cSupplierCategories.ValidateDeleteSupplierCategory();
        }


        /// <summary>
        /// This test ensures a supplier category can be successfully archived
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesSuccessfullyArchiveSupplierCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            /// Check the cancel button can be used when archiving
            cSupplierCategories.ArchiveSupplierCategoryAndCancel();
            cSupplierCategories.ValidateArchiveSupplierCategoryAndCancel();

            /// Archive a supplier category
            cSupplierCategories.ArchiveSupplierCategory();
            cSupplierCategories.ValidateArchiveSupplierCategory();
        }


        /// <summary>
        /// This test ensures a supplier category can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesSuccessfullyUnArchiveSupplierCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            /// Check the cancel button can be used when un-archiving
            cSupplierCategories.UnArchiveSupplierCategoryAndCancel();
            cSupplierCategories.ValidateUnArchiveSupplierCategoryAndCancel();

            /// Un-archive a supplier category
            cSupplierCategories.UnArchiveSupplierCategory();
            cSupplierCategories.ValidateUnArchiveSupplierCategory();
        }


        /// <summary>
        /// This test ensures that the page layout for supplier categories is correct
        /// </summary>
        [TestMethod]
        public void SupplierCategoriesValidateSupplierCategoriesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the supplier category page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=53");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cSupplierCategories.ValidateSupplierCategoriesPageLayoutExpectedValues.UILynneHunt16SeptemberPaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Valdiate the page layout
            cSupplierCategories.ValidateSupplierCategoriesPageLayout();
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
