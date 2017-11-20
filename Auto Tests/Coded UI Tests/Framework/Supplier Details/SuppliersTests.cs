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
using System.Threading;


namespace Auto_Tests.Coded_UI_Tests.Framework.Supplier_Details
{
    /// <summary>
    /// Suppliers Tests
    /// </summary>
    [CodedUITest]
    public class SuppliersTests
    {
        public SuppliersTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.SuppliersUIMapClasses.SuppliersUIMap cSuppliersMethods = new UIMaps.SuppliersUIMapClasses.SuppliersUIMap();


        [TestMethod]
        public void SuppliersSuccessfullyCreateSupplier()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            
            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");
            
            /// Click on New Supplier link
            cSuppliersMethods.ClickNewSupplierLink();
            
            /// Populate General Details section
            cSuppliersMethods.PopulateSupplierGeneralDetails();
            
            /// Populate Supplier Contact Details section
            cSuppliersMethods.PopulateSupplierContactDetails();
            
            /// Populate Supplier Address Details section
            cSuppliersMethods.PopulateSupplierAddressDetails();

            /// Press save
            cSuppliersMethods.PressSupplierSaveButton();
            
            /// Validate the save on the Suppliers grid
            cSuppliersMethods.ValidateSupplierExists();
            
            /// Press the edit icon for the newly created supplier
            cSuppliersMethods.PressEditSupplierIcon();
            
            /// Validate the supplier details
            cSuppliersMethods.ValidateSupplierGeneralDetailsSection();
            cSuppliersMethods.ValidateSupplierContactDetailsSection();
            cSuppliersMethods.ValidateSupplierAddressDetailsSection();
        }


        [TestMethod]
        public void SuppliersUnsuccessfullyCreateSupplierWithDuplicateDetails()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click on New Supplier link
            cSuppliersMethods.ClickNewSupplierLink();

            /// Populate General Details section
            cSuppliersMethods.PopulateSupplierGeneralDetails();

            /// Press save
            cSuppliersMethods.PressSupplierSaveButton();

            /// Validate the duplicate supplier error message
            cSuppliersMethods.ValidateDuplicateSupplierMessage();
            cSuppliersMethods.PressDuplicateSupplierMessageClose();

        }


        [TestMethod]
        public void SuppliersSuccessfullyAddAttachmentToSupplier()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the edit supplier icon
            cSuppliersMethods.PressEditSupplierIcon();
            
            /// Click the Add attachment icon and click the Add attachment link
            cSuppliersMethods.ClickAttachIcon();
            cSuppliersMethods.ClickAddAttachmentLink();
            
            /// Click browse and select the standard test file
            cSuppliersMethods.ClickBrowseButton();
            cSuppliersMethods.EnterFileNameAndEnterParams.UIFilenameComboBoxEditableItem = @"c:\Test.txt";
            cSuppliersMethods.EnterFileNameAndEnter();
            cSuppliersMethods.EnterAttachmentDescriptionAndSave();
            cSuppliersMethods.ValidateAttachmentExists();
            cSuppliersMethods.PressAttachmentCloseButton();
            cSuppliersMethods.ValidateGeneralDetailsAttachmentList();            
        }


        [TestMethod]
        public void SuppliersSuccessfullyDeleteAttachmentFromSupplier()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the edit supplier icon
            cSuppliersMethods.PressEditSupplierIcon();

            /// Click the attachment icon and press delete
            cSuppliersMethods.ClickAttachIcon();
            cSuppliersMethods.PressAttachmentDeleteIcon();

            /// Validate
            cSuppliersMethods.ValidateNoAttachmentsExist();
        }


        [TestMethod]
        public void SuppliersSuccessfullyEditSupplier()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the Edit supplier icon
            cSuppliersMethods.PressEditSupplierIcon();
            
            /// Populate General Details with new, unique data
            cSuppliersMethods.PopulateSupplierGeneralDetailsWithEditedData();
            
            /// Populate Contact Details with new, unique data
            cSuppliersMethods.PopulateSupplierContactDetailsWithEditedData();
            
            /// Populate Address Details with new, unique data
            cSuppliersMethods.PopulateSupplierAddressDetailsWithEditedData();
            
            /// Press the save button
            cSuppliersMethods.PressEditedSupplierSaveButton();
            
            /// Validate the supplier details have been updated on the Supplier grid
            cSuppliersMethods.ValidateEditedSupplierExists();
            
            /// Press the edit supplier icon and validate the changes
            cSuppliersMethods.PressEditSupplierIcon();
            cSuppliersMethods.ValidateSupplierGeneralDetailsSectionEdited();
            cSuppliersMethods.ValidateSupplierContactDetailsSectionEdited();
            cSuppliersMethods.ValidateSupplierAddressDetailsSectionEdited();
        }


        [TestMethod]
        public void SuppliersSuccessfullyDeleteSupplier()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the Delete supplier icon
            cSuppliersMethods.PressDeleteSupplierIcon();
            
            /// Validate confirmation message and press OK
            cSuppliersMethods.ValidateDeleteSupplierMessage();
            cSuppliersMethods.PressDeleteSupplierMessageOK();
            
            /// Validate the supplier not longer exists
            cSuppliersMethods.ValidateSupplierDoesNotExist();
        }


        [TestMethod]
        public void SuppliersSuccessfullyValidateSupplierWebLink()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the Supplier hyperlink icon
            cSuppliersMethods.PressHyperlinkSupplierIcon();

            /// Validate hyperlink warning message and press OK
            cSuppliersMethods.ValidateSupplierHyperlinkMessage();
            cSuppliersMethods.PressHyperlinkSupplierOK();
            
            /// Validate the website is displayed and close
            cSuppliersMethods.ValidateSupplierHyperlinkWindow();
            cSuppliersMethods.CloseSupplierHyperlinkWindow();
        }


        [TestMethod]
        public void SuppliersSuccessfullyValidateSupplierContractPage()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click on Supplier edit icon
            cSuppliersMethods.PressEditSupplierIcon();

            /// Click the Contracts link
            cSuppliersMethods.ClickContractsLink();
    
            /// Click the Add Contract link
            cSuppliersMethods.ClickAddContractLink();

            /// Validate the Contract page that is shown correctly
            cSuppliersMethods.ValidateSupplierContractPage();
        }

        [TestMethod]
        public void UnsuccessfullyAddSupplierWhereMissingManatoryFields()
        {
            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click on New Supplier link
            cSuppliersMethods.ClickNewSupplierLink();

            // Press save
            cSuppliersMethods.PressSupplierSaveButton();
            /// Validate the duplicate supplier error message
            cSuppliersMethods.ValidateDuplicateSupplierMessageExpectedValues.UIArecordalreadyexistsPaneDisplayText = "Supplier Name field is mandatory\r\nStatus is a mandatory value, please choose an option from the list.\r\nSupplier Category is a mandatory value, please choose an option from the list.";
            cSuppliersMethods.ValidateDuplicateSupplierMessage();
            cSuppliersMethods.PressDuplicateSupplierMessageClose();

            //Assert message gone.
            cSuppliersMethods.AssertSaveErrorModalMessageClose();
            
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
