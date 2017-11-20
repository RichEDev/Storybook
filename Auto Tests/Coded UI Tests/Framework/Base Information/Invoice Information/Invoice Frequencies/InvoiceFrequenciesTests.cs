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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Invoice_Information.Invoice_Frequencies
{
    /// <summary>
    /// Summary description for Invoice Frequencies
    /// </summary>
    [CodedUITest]
    public class InvoiceFrequenciesTests
    {
        public InvoiceFrequenciesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.InvoiceFrequenciesUIMapClasses.InvoiceFrequenciesUIMap cInvoiceFrequencies = new UIMaps.InvoiceFrequenciesUIMapClasses.InvoiceFrequenciesUIMap();


        /// <summary>
        /// This test ensures that a new invoice frequency definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesSuccessfullyCreateNewInvoiceFrequency()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");

            /// Ensure the cancel button can be used
            cInvoiceFrequencies.AddInvoiceFrequencyWithCancel();
            cInvoiceFrequencies.ValidateInvoiceFrequencyDoesNotExist();


            cInvoiceFrequencies.AddInvoiceFrequencyDescriptionOnly();
            cInvoiceFrequencies.ValidateAddInvoiceFrequencyDescriptionOnly();


            cInvoiceFrequencies.DeleteInvoiceFrequenc();


            cInvoiceFrequencies.AddInvoiceFrequencyAllInformation();
            cInvoiceFrequencies.ValidateAddInvoiceFrequencyAllInformation();
        }


        /// <summary>
        /// This test ensures that a duplicate invoice frequency definition cannnot be created
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesUnSuccessfullyCreateNewInvoiceFrequencyWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");


            cInvoiceFrequencies.AddInvoiceFrequencyAllInformation();
            cInvoiceFrequencies.ValidateAddDuplicateInvoiceFrequency();
        }


        /// <summary>
        /// This test ensures that an invoice frequency definition can be successfully edited
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesSuccessfullyEditInvoiceFrequency()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");

            /// Ensure the cancel button can be used
            cInvoiceFrequencies.EditInvoiceFrequencyWithCancel();
            cInvoiceFrequencies.ValidateAddInvoiceFrequencyAllInformation();


            cInvoiceFrequencies.EditInvoiceFrequency();
            cInvoiceFrequencies.ValidateEditInvoiceFrequency();


            cInvoiceFrequencies.EditInvoiceFrequencyResetValues();
        }


        /// <summary>
        /// This test ensures that an invoice frequency definition can be successfully deleted
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesSuccessfullyDeleteInvoiceFrequency()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");

            /// Ensure the cancel button can be used
            cInvoiceFrequencies.DeleteInvoiceFrequencyWithCancel();
            cInvoiceFrequencies.ValidateAddInvoiceFrequencyAllInformation();


            cInvoiceFrequencies.DeleteInvoiceFrequenc();
            cInvoiceFrequencies.ValidateInvoiceFrequencyDoesNotExist();
        }


        /// <summary>
        /// This test ensures that an invoice frequency definition can be successfully archied
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesSuccessfullyArchiveInvoiceFrequency()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");

            /// Ensure the cancel button can be used
            cInvoiceFrequencies.ArchiveInvoiceFrequencyWithCancel();
            cInvoiceFrequencies.ValidateInvoiceFrequencyIsNotArchived();


            cInvoiceFrequencies.ArchiveInvoiceFrequency();
            cInvoiceFrequencies.ValidateInvoiceFrequencyIsArchived();
        }


        /// <summary>
        /// This test ensures that an invoice frequency definition can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesSuccessfullyUnArchiveInvoiceFrequency()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");

            /// Ensure the cancel button can be used
            cInvoiceFrequencies.UnArchiveInvoiceFrequencyWithCancel();
            cInvoiceFrequencies.ValidateInvoiceFrequencyIsArchived();


            cInvoiceFrequencies.UnArchiveInvoiceFrequency();
            cInvoiceFrequencies.ValidateInvoiceFrequencyIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the page layout for invoice frequencies is correct
        /// </summary>
        [TestMethod]
        public void InvoiceFrequenciesSuccessfullyValidateInvoiceFrequenciesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the invoice frequency page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=130");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cInvoiceFrequencies.ValidateInvoiceFrequenciesPageLayoutExpectedValues.UIJamesLloyd20SeptembePaneDisplayText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);

            cInvoiceFrequencies.ValidateInvoiceFrequenciesPageLayout();
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
