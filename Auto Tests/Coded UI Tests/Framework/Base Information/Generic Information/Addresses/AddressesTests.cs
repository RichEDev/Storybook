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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Generic_Information.Addresses
{
    /// <summary>
    /// This class contains all of the tests for Address within Framework
    /// </summary>
    [CodedUITest]
    public class AddressesTests
    {
        public AddressesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.AddressesUIMapClasses.AddressesUIMap cAddresses = new UIMaps.AddressesUIMapClasses.AddressesUIMap();

        /// <summary>
        /// This test ensures a new address can be created within Framework
        /// </summary>
        [TestMethod]
        public void AddressesSuccessfullyCreateNewAddressWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
             
            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");
            
            /// Click New Address link
            cAddresses.ClickAddAddressLink();
            
            /// Enter address details
            cAddresses.EnterAddressDetails();

            /// Validate address details before save
            Playback.Wait(2000);
            cAddresses.ValidateAddAddressDetails();

            /// Press save button
            cAddresses.ClickAddressSaveButton();

            /// Search for address
            cAddresses.ValidateAddAddress();

            /// Validate the save
            cAddresses.ClickEditAddress();
            cAddresses.ValidateAddAddressDetails();
        }


        /// <summary>
        /// This test ensures duplicate addresses cannot be added within Framework
        /// </summary>
        [TestMethod]
        public void AddressesUnSuccessfullyCreateAddressWithDuplicateDetailsWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");
            
            /// Click Add Address link
            cAddresses.ClickAddAddressLink();
            
            /// Enter address details
            cAddresses.EnterAddressDetails();
            
            /// Press save
            cAddresses.ClickAddressSaveButton();
            
            /// Validate address is duplicate
            cAddresses.ValidateAddDuplicateAddress();
            
            /// Cancel duplication message
            cAddresses.UndoAddDuplicateAddress();
        }


        /// <summary>
        /// This test ensures an address can be edited within Framework
        /// </summary>
        [TestMethod]
        public void AddressesSuccessfullyEditAddressWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");
            
            /// Press the edit button and enter new details
            cAddresses.ClickEditAddress();
            cAddresses.EnterEditedAddressDetails();
            
            /// Validate details before save
            cAddresses.ValidateAddEditAddressDetails();
            
            /// Press save button
            cAddresses.ClickAddressSaveButton();
            
            /// Search for address validate
            cAddresses.ValidateEditAddress();
            
            /// Edit address and validate
            cAddresses.ClickEditAddress();
            cAddresses.ValidateAddEditAddressDetails();
        }


        /// <summary>
        /// This test ensures an address can be archived within Framework
        /// </summary>
        [TestMethod]
        public void AddressesSuccessfullyArchiveAddressWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");
            
            /// Archive address
            cAddresses.ArchiveAddress();
            
            /// Validate address is archived
            cAddresses.ValidateAddressIsArchived();
        }


        /// <summary>
        /// This test ensures an address can be unarchived within Framework
        /// </summary>
        [TestMethod]
        public void AddressesSuccessfullyUnArchiveAddressWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");

            /// Un-Archive address
            cAddresses.UnArchiveAddress();

            /// Validate address is un-archived
            cAddresses.ValidateAddressIsNotArchived();
        }


        /// <summary>
        /// This test ensures an address can be deleted within Framework
        /// </summary>
        [TestMethod]
        public void AddressesSuccessfullyDeleteAddressWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");

            /// Delete address
            cAddresses.DeleteAddress();

            /// Validate address no longer exists
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");
            cAddresses.ValidateAddressDoesNotExist();        
        }


        /// <summary>
        /// This test ensures a parent address can be added to an address within Framework
        /// </summary>
        [TestMethod]
        public void AddressesSuccessfullyAssignParentAddressWithinFramework()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to addresses page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/locationsearch.aspx");

            #region Add parent address

            /// Click New Address link
            cAddresses.ClickAddAddressLink();

            /// Enter address details
            cAddresses.EnterAddressDetailsParams.UINameEditText = "__Automated Parent Address";
            cAddresses.EnterAddressDetails();

            /// Validate address details before save
            Playback.Wait(2000);

            /// Press save button
            cAddresses.ClickAddressSaveButton();

            #endregion

            /// Search for address and edit
            cAddresses.ClickEditAddress();
            
            /// Assign parent and save
            cAddresses.EnterParentAddress();
            cAddresses.ClickAddressSaveButton();

            /// Search for address and edit
            cAddresses.ClickEditAddress();
            
            /// Validate save
            cAddresses.ValidateParentAddress();
            
            /// Un-assign parent address and save
            cAddresses.EnterParentAddressParams.UIParentaddressEditText = "";
            cAddresses.EnterParentAddress();
            cAddresses.ClickAddressSaveButton();
            
            /// Search for address and edit
            cAddresses.ClickEditAddress();
            
            /// Validate save
            cAddresses.ValidateParentAddressExpectedValues.UIParentaddressEditText = "";
            cAddresses.ValidateParentAddress();
            cAddresses.ClickAddressSaveButton();

            /// Delete parent address
            cAddresses.DeleteParentAddress();
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
