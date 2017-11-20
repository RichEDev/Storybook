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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Relationship_Information.Supplier_Status
{
    /// <summary>
    /// Summary description for Supplier Status Tests
    /// </summary>
    [CodedUITest]
    public class SupplierStatusTests
    {
        public SupplierStatusTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.SupplierStatusUIMapClasses.SupplierStatusUIMap cSupplierStatus = new UIMaps.SupplierStatusUIMapClasses.SupplierStatusUIMap();


        /// <summary>
        /// This test ensures that a new Supplier Status definition can be successfully added
        /// </summary>
        [TestMethod]
        public void SupplierStatusSuccessfullyCreateNewSupplierStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");

            /// Add supplier status with description only
            cSupplierStatus.AddSupplierStatusWithDescriptionOnly();
            cSupplierStatus.ValidateAddSupplierStatusWithDescriptionOnly();
            cSupplierStatus.DeleteSupplierStatus();

            /// Add supplier status with description and sequence only 
            cSupplierStatus.AddSupplierStatusWithDescriptionAndSequenceOnly();
            cSupplierStatus.ValidateAddSupplierStatusWithDescriptionAndSequenceOnly();
            cSupplierStatus.DeleteSupplierStatus();

            /// Add supplier status with all information
            cSupplierStatus.AddSupplierStatusWithAllInformation();
            cSupplierStatus.ValidateAddSupplierStatusWithAllInformation();            
        }


        /// <summary>
        /// This test ensures that duplicate Supplier Status definitions cannot be created
        /// </summary>
        [TestMethod]
        public void SupplierStatusUnsuccessfullyCreateNewSupplierStatusWithDuplicateDetails()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");

            /// Ensure you cannot add a duplicate supplier status
            cSupplierStatus.AddDuplicateSupplierStatus();
            cSupplierStatus.ValidateAddDuplicateSupplierStatus();
        }


        /// <summary>
        /// This test ensures that a Supplier Status definition can be edited
        /// </summary>
        [TestMethod]
        public void SupplierStatusSuccessfullyEditSupplierStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");

            /// Ensure the cancel button can be used when editing
            cSupplierStatus.EditSupplierStatusAndCancel();

            /// Edit the supplier status description
            cSupplierStatus.EditSupplierStatus();
            cSupplierStatus.ValidateEditSupplierStatusDescription();

            /// Edit the supplier status sequence and permit
            cSupplierStatus.EditSupplierStatusSequenceAndPermit();
            cSupplierStatus.ValidateEditSupplierStatusSequenceAndPermit();

            /// Reset values for future use
            cSupplierStatus.EditSupplierStatusResetValues();                                                                    
        }


        /// <summary>
        /// This test ensures that a Supplier Status definition can be deleted
        /// </summary>
        [TestMethod]
        public void SupplierStatusSuccessfullyDeleteSupplierStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");

            /// Ensure the cancel button works when deleting
            cSupplierStatus.DeleteSupplierStatusAndCancel();
            cSupplierStatus.ValidateDeleteSupplierStatusExpectedValues.UITbl_bdGrid_e8cde388_RowExists = true;
            cSupplierStatus.ValidateDeleteSupplierStatus();

            /// Delete a supplier status
            cSupplierStatus.DeleteSupplierStatus();
            cSupplierStatus.ValidateDeleteSupplierStatus();
        }


        /// <summary>
        /// This test ensures that a Supplier Status definition can be archived
        /// </summary>
        [TestMethod]
        public void SupplierStatusSuccessfullyArchiveSupplierStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");

            /// Ensure the cancel button can be used when archiving
            cSupplierStatus.ArchiveSupplierStatusAndCancel();
            cSupplierStatus.ValidateUnArchiveSupplierStatus();

            /// Archive a supplier status
            cSupplierStatus.ArchiveSupplierStatus();
            cSupplierStatus.ValidateArchiveSupplierStatus();
        }


        /// <summary>
        /// This test ensures that a Supplier Status definition can be un-archived
        /// </summary>
        [TestMethod]
        public void SupplierStatusSuccessfullyUnArchiveSupplierStatus()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");

            /// Ensure the cancel button can be used when un-archiving
            cSupplierStatus.UnArchiveSupplierStatusAndCancel();
            cSupplierStatus.ValidateArchiveSupplierStatus();

            /// Un-archive a supplier status
            cSupplierStatus.UnArchiveSupplierStatus();
            cSupplierStatus.ValidateUnArchiveSupplierStatus();
        }


        /// <summary>
        /// This test ensures that
        /// </summary>
        [TestMethod]
        public void SupplierStatusSuccessfullyValidateSupplierStatusPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Supplier Status page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=55");


            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cSupplierStatus.ValidateSupplierStatusPageLayoutExpectedValues.UILynneHunt16SeptemberPaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate the page layout
            cSupplierStatus.ValidateSupplierStatusPageLayout();
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
