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


namespace Auto_Tests.Coded_UI_Tests.Framework.Supplier_Details.Supplier_Contacts
{
    /// <summary>
    /// Supplier Contacts tests
    /// </summary>
    [CodedUITest]
    public class SupplierContactsTests
    {
        public SupplierContactsTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.SuppliersUIMapClasses.SuppliersUIMap cSuppliersMethods = new UIMaps.SuppliersUIMapClasses.SuppliersUIMap();


        [TestMethod]
        public void SupplierContactsSuccessfullyCreateSupplierContact()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the Edit supplier icon
            cSuppliersMethods.PressEditSupplierIcon();

            /// Click the Contacts link and select New Contact
            cSuppliersMethods.ClickContactsLink();
            cSuppliersMethods.ClickNewContactLink();

            /// Populate the Contact Details section
            cSuppliersMethods.PopulateSupplierContactContactDetails();

            /// Populate the Business Address section
            cSuppliersMethods.PopulateSupplierContactBusinessAddress();

            /// Populate the Personal Address section
            cSuppliersMethods.PopulateSupplierContactPersonalAddressParams.UIPostCodeEdit1Text = "P P CODE";
            cSuppliersMethods.PopulateSupplierContactPersonalAddress();

            /// Press the Save button and validate
            cSuppliersMethods.PressSupplierContactSaveButton();
            cSuppliersMethods.ValidateSupplierContactExists();

            /// Press the edit icon and validate
            cSuppliersMethods.PressSupplierContactEditIcon();
            cSuppliersMethods.ValidateSupplierContactContactDetailsSection();
            cSuppliersMethods.ValidateSupplierContactBusinessAddress();
            cSuppliersMethods.ValidateSupplierContactPersonalAddressExpectedValues.UIPostCodeEdit1Text = "P P CODE";
            cSuppliersMethods.ValidateSupplierContactPersonalAddress();
            cSuppliersMethods.PressSupplierContactCancelButton();

            cSharedMethods.NavigateToFrameworkHomePage();
        }


        [TestMethod]
        public void SupplierContactsSuccessfullyEditSupplierContact()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the Edit supplier icon
            cSuppliersMethods.PressEditSupplierIcon();

            /// Click the Contacts link and click the edit supplier contact icon
            cSuppliersMethods.ClickContactsLink();
            cSuppliersMethods.PressSupplierContactEditIcon();
            
            /// Populate the Supplier contact Contact Details section with new data
            cSuppliersMethods.PopulateSupplierContactContactEditedDetails();
            
            /// Populate the Supplier contact Business Address section with new data
            cSuppliersMethods.PopulateSupplierContactBusinessAddressEditedParams.UIPostCodeEditText = "EDITED PC";
            cSuppliersMethods.PopulateSupplierContactBusinessAddressEdited();
            
            /// Populate the Supplier contact Personal Address section with new data
            cSuppliersMethods.PopulateSupplierContactPersonalAddressEdited();
            
            /// Press save and validate
            cSuppliersMethods.PressEditedSupplierContactSaveButton();
            cSuppliersMethods.ValidateSupplierContactEdited();
            
            /// Press the supplier contact edit icon and validate
            cSuppliersMethods.PressSupplierContactEditIcon();
            cSuppliersMethods.ValidateSupplierContactContactDetailsSectionEdited();
            cSuppliersMethods.ValidateSupplierContactBusinessAddressEditedExpectedValues.UIPostCodeEditText = "EDITED PC";
            cSuppliersMethods.ValidateSupplierContactBusinessAddressEdited();
            cSuppliersMethods.ValidateSupplierContactPersonalAddressEduted();
            cSuppliersMethods.PressSupplierContactCancelButton();

            cSharedMethods.NavigateToFrameworkHomePage();
        }


        [TestMethod]
        public void SupplierContactsSuccessfullyDeleteSupplierContact()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to Suppliers page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/suppliers.aspx");

            /// Click the Edit supplier icon
            cSuppliersMethods.PressEditSupplierIcon();

            /// Click the Contacts link and the supplier contact delete icon
            cSuppliersMethods.ClickContactsLink();
            cSuppliersMethods.PressDeleteSupplierIcon();
            
            /// Validate the deletion message and press ok
            cSuppliersMethods.PressDeleteSupplierMessageOK();
            
            /// Validate the supplier contact has been deleted
            cSuppliersMethods.ValidateSupplierDoesNotExist();
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
