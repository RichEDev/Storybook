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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Invoice_Information.Invoice_Status
{
    /// <summary>
    /// Summary description for Invoice Status Types
    /// </summary>
    [CodedUITest]
    public class InvoiceStatusTests
    {
        public InvoiceStatusTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.InvoiceStatusUIMapClasses.InvoiceStatusUIMap cInvoiceStatus = new UIMaps.InvoiceStatusUIMapClasses.InvoiceStatusUIMap();


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusSuccessfullyCreateNewInvoiceStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            /// Ensure the cancel button can be used
            cInvoiceStatus.AddInvoiceStatusWithCancel();
            cInvoiceStatus.ValidateInvoiceStatusDoesNotExist();

            /// Add an invoice status
            cInvoiceStatus.AddInvoiceStatus();
            cInvoiceStatus.ValidateAddInvoice();
        }


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusUnSuccessfullyCreateNewInvoiceStatusWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            /// Add a duplicate invoice status
            cInvoiceStatus.AddInvoiceStatus();
            cInvoiceStatus.ValidateAddDuplicateInvoiceStatus();
        }


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusSuccessfullyEditInvoiceStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            /// Ensure the cancel button can be used
            cInvoiceStatus.EditInvoiceStatusWithCancel();
            cInvoiceStatus.ValidateAddInvoice();

            /// Edit an invoice status
            cInvoiceStatus.EditInvoiceStatus();
            cInvoiceStatus.ValidateEditInvoiceStatus();

            /// Reset the values
            cInvoiceStatus.EditInvoiceStatusResetValues();            
        }


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusSuccessfullyDeleteInvoiceStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            /// Ensure the cancel button can be used
            cInvoiceStatus.DeleteInvoiceStatusWithCancel();
            cInvoiceStatus.ValidateAddInvoice();

            /// Delete an invoice status
            cInvoiceStatus.DeleteInvoiceStatus();
            cInvoiceStatus.ValidateInvoiceStatusDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusSuccessfullyArchiveInvoiceStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            /// Ensure the cancel button can be used
            cInvoiceStatus.ArchiveInvoiceStatusWithCancel();
            cInvoiceStatus.ValidateInvoiceStatusIsNotArchived();

            /// Archive an invoice status
            cInvoiceStatus.ArchiveInvoiceStatus();
            cInvoiceStatus.ValidateInvoiceStatusIsArchived();
        }


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusSuccessfullyUnArchiveInvoiceStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            /// Ensure the cancel button can be used
            cInvoiceStatus.UnArchiveInvoiceStatusWithCancel();
            cInvoiceStatus.ValidateInvoiceStatusIsArchived();

            /// Un-archive an invoice status
            cInvoiceStatus.UnArchiveInvoiceStatus();
            cInvoiceStatus.ValidateInvoiceStatusIsNotArchived();
        }


        /// <summary>
        /// This test ensures that a new invoice status definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceStatusSuccessfullyValidateInvoiceStatusPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=131");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cInvoiceStatus.ValidateInvoiceStatusPageLayoutExpectedValues.UIJamesLloyd20SeptembePaneDisplayText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);

            /// Validate the page layout
            cInvoiceStatus.ValidateInvoiceStatusPageLayout();
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
