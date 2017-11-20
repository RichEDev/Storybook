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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Contract_Configuration_Options.Sales_Tax
{
    /// <summary>
    /// Summary description for Sales Tax tests
    /// </summary>
    [CodedUITest]
    public class SalesTaxTests
    {
        public SalesTaxTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.SalesTaxUIMapClasses.SalesTaxUIMap cSalesTax = new UIMaps.SalesTaxUIMapClasses.SalesTaxUIMap();


        /// <summary>
        /// This test ensures that a new sales tax definition can be successfully created
        /// </summary>
        [TestMethod]
        public void SalesTaxSuccessfullyCreateNewSalesTax()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            /// Ensure the cancel button can be used
            cSalesTax.AddSalesTaxWithCancel();
            cSalesTax.ValidateSalesTaxDoesNotExist();

            /// Add a sales tax
            cSalesTax.AddSalesTax();
            cSalesTax.ValidateAddSalesTax();
        }


        /// <summary>
        /// This test ensures that a duplicate sales tax definition cannot be created
        /// </summary>
        [TestMethod]
        public void SalesTaxUnSuccessfullyCreateNewSalesTaxWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            /// Add a duplicate sales tax
            cSalesTax.AddSalesTax();
            cSalesTax.ValidateAddDuplicateSalesTax();
        }


        /// <summary>
        /// This test ensures that a sales tax definition can be successfully edited
        /// </summary>
        [TestMethod]
        public void SalesTaxSuccessfullyEditSalesTax()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            /// Ensure the cancel button can be used
            cSalesTax.EditSalesTaxWithCancel();
            cSalesTax.ValidateAddSalesTax();

            /// Edit a sales tax
            cSalesTax.EditSalesTax();
            cSalesTax.ValidateEditSalesTax();

            /// Reset the values for future use
            cSalesTax.EditSalesTaxResetValues();
        }


        /// <summary>
        /// This test ensures that a sales tax definition can be successfully deleted
        /// </summary>
        [TestMethod]
        public void SalesTaxSuccessfullyDeleteSalesTax()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            /// Ensure the cancel button can be used
            cSalesTax.DeleteSalesTaxWithCancel();
            cSalesTax.ValidateAddSalesTax();

            /// Delete a sales tax
            cSalesTax.DeleteSalesTax();
            cSalesTax.ValidateSalesTaxDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a sales tax definition can be successfully archived
        /// </summary>
        [TestMethod]
        public void SalesTaxSuccessfullyArchiveSalesTax()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            /// Ensure the cancel button can be used
            cSalesTax.ArchiveSalesTaxWithCancel();
            cSalesTax.ValidateSalesTaxIsNotArchived();

            /// Archive a sales tax
            cSalesTax.ArchiveSalesTax();
            cSalesTax.ValidateSalesTaxIsArchived();
        }


        /// <summary>
        /// This test ensures that a sales tax definition can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void SalesTaxSuccessfullyUnArchiveSalesTax()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            /// Ensure the cancel button can be used
            cSalesTax.UnArchiveSalesTaxWithCancel();
            cSalesTax.ValidateSalesTaxIsArchived();

            /// Un-archive a sales tax
            cSalesTax.UnArchiveSalesTax();
            cSalesTax.ValidateSalesTaxIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the page layout for sales tax is correct
        /// </summary>
        [TestMethod]
        public void SalesTaxSuccessfullyValidateSalesTaxPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the sales tax page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=114");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cSalesTax.ValidateSalesTaxPageLayoutExpectedValues.UIJamesLloyd20SeptembePaneDisplayText = 
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate the page layout
            cSalesTax.ValidateSalesTaxPageLayout();
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
