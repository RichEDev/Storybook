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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Product_and_Service_Information.Product_Categories
{
    /// <summary>
    /// Summary description for Product categories
    /// </summary>
    [CodedUITest]
    public class ProductCategoriesTests
    {
        public ProductCategoriesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ProductCategoriesUIMapClasses.ProductCategoriesUIMap cProductCategories = new UIMaps.ProductCategoriesUIMapClasses.ProductCategoriesUIMap();

        /// <summary>
        /// This test ensures that a new product category can be created
        /// </summary>
        [TestMethod]
        public void ProductCategoriesSuccessfullyCreateNewProductCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            /// Ensure the cancel button can be used when adding
            cProductCategories.AddProductCategoryWithCancel();
            cProductCategories.ValidateProductCategoryDoesNotExist();

            /// Create a new product category
            cProductCategories.AddProductCategory();
            cProductCategories.ValidateAddProductCategory();
        }


        /// <summary>
        /// This test ensures that a duplicate product category cannot be created
        /// </summary>
        [TestMethod]
        public void ProductCategoriesUnSuccessfullyCreateNewProductCategoryWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            /// Add a duplicate product category
            cProductCategories.AddProductCategory();
            cProductCategories.ValidateAddDuplicateProductCategory();
        }


        /// <summary>
        /// This test ensures that a product cateogry can be edited
        /// </summary>
        [TestMethod]
        public void ProductCategoriesSuccessfullyEditProductCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            /// Ensure the cancel button can be used when editing
            cProductCategories.EditProductCategoryWithCancel();
            cProductCategories.ValidateAddProductCategory();

            /// Edit a product category
            cProductCategories.EditProductCategory();
            cProductCategories.ValidateEditProductCategory();

            /// Reset the values for future tests
            cProductCategories.EditProductCategoryResetValues();
        }


        /// <summary>
        /// This test ensures that a product category can be deleted
        /// </summary>
        [TestMethod]
        public void ProductCategoriesSuccessfullyDeleteProductCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            /// Ensure the cancel button can be used when 
            cProductCategories.DeleteProductCategoryWithCancel();
            cProductCategories.ValidateAddProductCategory();

            /// Delete a product category
            cProductCategories.DeleteProductCategory();
            cProductCategories.ValidateProductCategoryDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a product category can be archived
        /// </summary>
        [TestMethod]
        public void ProductCategoriesSuccessfullyArchiveProductCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            /// Ensure the cancel button can be used when archiving
            cProductCategories.ArchiveProductCategoryWithCancel();
            cProductCategories.ValidateProductCategoryIsNotArchived();

            /// Archive a product category
            cProductCategories.ArchiveProductCategory();
            cProductCategories.ValidateProductCategoryIsArchived();
        }


        /// <summary>
        /// This test ensures that a product category can be un-archived
        /// </summary>
        [TestMethod]
        public void ProductCategoriesSuccessfullyUnArchiveProductCategory()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            /// Ensure the cancel button can be used when un-archiving
            cProductCategories.UnArchiveProductCategoryWithCancel();
            cProductCategories.ValidateProductCategoryIsArchived();

            /// Unarchive a product category
            cProductCategories.UnArchiveProductCategory();
            cProductCategories.ValidateProductCategoryIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the product categories page layout is correct
        /// </summary>
        [TestMethod]
        public void ProductCategoriesSuccessfullyValidateProductCategoriesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product categories page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=60");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cProductCategories.ValidateProductCategoryPageLayotExpectedValues.UIJamesLloyd111117SeptPaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate the page layout
            cProductCategories.ValidateProductCategoryPageLayot();
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
