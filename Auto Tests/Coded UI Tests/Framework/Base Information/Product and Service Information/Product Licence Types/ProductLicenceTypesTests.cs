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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Product_and_Service_Information.Product_Licence_Types
{
    /// <summary>
    /// Summary description for Product categories
    /// </summary>
    [CodedUITest]
    public class ProductLicenceTypesTests
    {
        public ProductLicenceTypesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.ProductLicenceTypesUIMapClasses.ProductLicenceTypesUIMap cLicenceTypes = new UIMaps.ProductLicenceTypesUIMapClasses.ProductLicenceTypesUIMap();

        /// <summary>
        /// This test ensures that a new product licence type can be created
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesSuccessfullyCreateNewProductLicenceType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            /// Ensure the cancel button can be used when adding
            cLicenceTypes.AddLicenceTypeWithCancel();
            cLicenceTypes.ValidateLicenceTypeDoesNotExist();

            /// Create a new product category
            cLicenceTypes.AddLicenceType();
            cLicenceTypes.ValidateAddLicenceType();

            cSharedMethods.NavigateToFrameworkHomePage();
        }


        /// <summary>
        /// This test ensures that a duplicate product licence type cannot be created
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesUnSuccessfullyCreateNewProductLicenceTypeWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            /// Add a duplicate product category
            cLicenceTypes.AddLicenceType();
            cLicenceTypes.ValidateAddDuplicateLicenceType();
            cLicenceTypes.CancelDuplicateMessage();
        }


        /// <summary>
        /// This test ensures that a product licence type can be edited
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesSuccessfullyEditProductLicenceType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            /// Ensure the cancel button can be used when editing
            cLicenceTypes.EditLicenceTypeWithCancel();
            cLicenceTypes.ValidateAddLicenceType();

            /// Edit a product category
            cLicenceTypes.EditLicenceType();
            cLicenceTypes.ValidateEditLicenceType();

            /// Reset the values for future tests
            cLicenceTypes.EditLicenceTypeResetValues();
        }


        /// <summary>
        /// This test ensures that a product licence type can be deleted
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesSuccessfullyDeleteProductLicenceType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            /// Ensure the cancel button can be used when 
            cLicenceTypes.DeleteLicenceTypeWithCancel();
            cLicenceTypes.ValidateAddLicenceType();

            /// Delete a product category
            cLicenceTypes.DeleteLicenceType();
            cLicenceTypes.ValidateLicenceTypeDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a product licence type can be archived
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesSuccessfullyArchiveProductLicenceType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            /// Ensure the cancel button can be used when archiving
            cLicenceTypes.ArchiveLicenceTypeWithCancel();
            cLicenceTypes.ValidateLicenceTypeIsNotArchived();

            /// Archive a product category
            cLicenceTypes.ArchiveLicenceType();
            cLicenceTypes.ValidateLicenceTypeIsArchived();
        }


        /// <summary>
        /// This test ensures that a product licence type can be un-archived
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesSuccessfullyUnArchiveProductLicenceType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            /// Ensure the cancel button can be used when un-archiving
            cLicenceTypes.UnArchiveLicenceTypeWithCancel();
            cLicenceTypes.ValidateLicenceTypeIsArchived();

            /// Unarchive a product category
            cLicenceTypes.UnArchiveLicenceType();
            cLicenceTypes.ValidateLicenceTypeIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the product licence type page layout is correct
        /// </summary>
        [TestMethod]
        public void ProductLicenceTypesSuccessfullyValidateProductLicenceTypesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the product licence types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=136");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cLicenceTypes.ValidateLicenceTypePageLayoutExpectedValues.UIMrJamesLloydActiveSuPaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate the page layout
            cLicenceTypes.ValidateLicenceTypePageLayoutExpectedValues.UIAboutSwitchSubAccounPaneDisplayText = "Switch Sub Account | About | Help & Support | Exit Home : Administration Menu : Base Information : Product Information : Product Licence Types ";
            cLicenceTypes.ValidateLicenceTypePageLayout();
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
