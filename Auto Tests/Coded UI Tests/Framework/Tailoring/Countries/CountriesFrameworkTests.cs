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
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Countries
{
    /// <summary>
    /// Summary description for CountriesFrameworkTests
    /// </summary>
    [CodedUITest]
    public class CountriesFrameworkTests
    {
        public CountriesFrameworkTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.CountriesUIMapClasses.CountriesUIMap cCountriesMethods = new UIMaps.CountriesUIMapClasses.CountriesUIMap();


        /// <summary>
        /// 29838 -  Countries: Unsuccessfully Delete Country Where Country Is In Use.
        /// </summary>
        [TestMethod]
        public void CountriesUnsuccessfullyDeleteCountryWhereCountryIsUsedForSupplierAddress()
        {            
            /// Logon and navigate to the Countries base information page
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");
                                        
            /// Attempt to delete a country which is being used for a supplier address
            cCountriesMethods.DeleteCountry();

            /// Assert the error message
            cCountriesMethods.AssertDeletionMessageUsedOnSupplier();

            /// Close the resulting message box
            cCountriesMethods.PressUnsuccessfulDeleteClose();
        }


        /// <summary>
        /// 29839 -  Successfully Archive a Country Where Country Has Been Used For Supplier Address.
        /// Note that this test now ensures that a country CAN be archived, even when it has been used
        /// for a supplier address. Previously, if a country was used it could not be archived.
        /// </summary>
        [TestMethod]
        public void CountriesSuccessfullyArchiveCountryWhereCountryHasBeenUsedForSupplierAddress()
        {
            /// Logon and Navigate to the Countries base information page
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");

            /// Archive the country
            cCountriesMethods.ArchiveCountryInUse();

            /// Validate that the country is archived
            cCountriesMethods.ValidateCountryIsArchived();
        }


        /// <summary>
        /// This test is basically a clean-up for CountriesSuccessfullyArchiveCountryWhereCountryHasBeenUsedForSupplierAddress
        /// </summary>
        [TestMethod]
        public void CountriesSuccessfullyUnArchiveCountryWhereCountryHasBeenUsedForSupplierAddress()
        {
            /// Logon and Navigate to the Countries base information page
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/admin/admincountries.aspx");

            /// Archive the country
            cCountriesMethods.PressUnarchiveCountryUnitedKingdom();

            /// Validate that the country is archived
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
