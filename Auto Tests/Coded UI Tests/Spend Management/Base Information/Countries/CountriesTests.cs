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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Base_Information.Countries
{
    /// <summary>
    /// Tests for Countries
    /// </summary>
    [CodedUITest]
    public class CountriesTests
    {
        public CountriesTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.CountriesUIMapClasses.CountriesUIMap cCountriesMethods = new UIMaps.CountriesUIMapClasses.CountriesUIMap();


        /// <summary>
        /// 29837 -  Successfully Cancel Adding a Country
        /// </summary>
        [TestMethod]
        public void CountriesSuccessfullyCancelAddingCountry()
        {           
            /// Logon to Framework and navigate to the Countries administration page
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");

            /// Select new country from list
            cCountriesMethods.SelectNewCountry();
            cCountriesMethods.EnterCountryNameParams.UICountryComboBoxSelectedItem = "Brazil";
            cCountriesMethods.EnterCountryName();

            /// Press the cancel button
            cCountriesMethods.SelectCancelCountryDetails();

            /// Ensure the country has not been created
            if (cCountriesMethods.UICountriesWindowsInteWindow.UICountriesDocument1.UITbl_gridCountriesTable3.UIItemCustom.InnerText.Contains("Brazil"))
            {
                Assert.Fail();
            }
        }


        /// <summary>
        /// 29834 -  Successfully Add New Country
        /// </summary>
        [TestMethod]
        public void CountriesSuccessfullyAddNewCountry()
        {           
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the Countries page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");

            /// Select country form the list and save
            cCountriesMethods.SelectNewCountry();
            cCountriesMethods.EnterCountryNameParams.UICountryComboBoxSelectedItem = "Brazil";
            cCountriesMethods.EnterCountryName();
            cCountriesMethods.SaveCountryDetails();

            /// Search for the country
            cCountriesMethods.SearchForCountryParams.UICountryEditText = "Brazil";
            try
            {
                /// Perform search if the page is paginated
                cCountriesMethods.SearchForCountry();
            }
            catch
            {
                /// Perform search if the pag is not paginated
                cCountriesMethods.EditCountryWhenNotPaginated();
            }

            /// Check that the country record has been created successfully.
            cCountriesMethods.ValidateThatCountryExistsExpectedValues.UICountryComboBoxSelectedItem = "Brazil";
            cCountriesMethods.AssertCountryAlpah2Value();
            cCountriesMethods.AssertCountryAlpha3Value();
            cCountriesMethods.AssertCountryNumeric3Value();
            cCountriesMethods.ValidateThatCountryExists();

            /// Check that the 'Add Country' list no-longer contains 'sCountry'.
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/aecountry.aspx");

            string sCountriesComboBox = cCountriesMethods.UICountryNewWindowsIntWindow.UICountryNewDocument.UICountryComboBox1.InnerText.ToString();

            /// If the 'Country' combobox contains the test country then fail the test.
            if (sCountriesComboBox.Contains("Brazil"))
            {
                Assert.Fail();
            }
        }
                 

        /// <summary>
        /// 29835 -  Successfully Delete Country
        /// </summary>
        [TestMethod]
        public void CountriesSuccessfullyDeleteCountry()
        {
            /// Logon to Framework and navigate to the Countries administration page
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");

            /// Search for the country
            cCountriesMethods.SearchForCountryParams.UICountryEditText = "Brazil";
            try
            {
                /// Perform search if the page is paginated
                cCountriesMethods.SearchForCountry();
            }
            catch { }

            /// Press the delete icon and cancel
            cCountriesMethods.SelectDeleteCountry();
            cCountriesMethods.SelectCancelDeletion();

            /// Press the delete icon and confirm
            cCountriesMethods.SelectDeleteCountry();
            cCountriesMethods.SelectConfirmDeletion();


            /// Search for the country
            cCountriesMethods.SearchForCountryParams.UICountryEditText = "Brazil";
            try
            {
                /// Perform search if the page is paginated
                cCountriesMethods.SearchForCountry();
            }
            catch { }

            /// Check that the country record has been deleted successfully.
            if (cCountriesMethods.UICountriesWindowsInteWindow.UICountriesDocument1.UITbl_gridCountriesTable3.UIItemCustom.InnerText.Contains("Brazil"))
            {
                Assert.Fail();
            }
        }


        [TestMethod]
        public void CountriesSuccessfullyArchiveCountry()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            
            /// Navigate to the countries page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");
            
            /// Click the archive icon
            cCountriesMethods.PressArchiveCountryIcon();
            
            /// Validate the country has been archived
            cCountriesMethods.ValidateCountryIsArchived();        
        }


        [TestMethod]
        public void CountriesSuccessfullyUnarchiveCountry()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the countries page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");

            /// Click the un-archive icon
            cCountriesMethods.PressUnarchiveCountryIcon();

            /// Validate the country has been archived
            cCountriesMethods.ValidateCountryIsNotArchived();   
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
