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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Corporate_Cards
{
    /// <summary>
    /// These tests ensure that corporate cards can be added, edited and removed from employee records. Currently, these tests do not test the functionality of
    /// corporate cards throughout the product.
    /// </summary>
    [CodedUITest]
    public class CorporateCardsTests
    {
        public CorporateCardsTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.CorporateCardsUIMapClasses.CorporateCardsUIMap cCorpCardsMethods = new UIMaps.CorporateCardsUIMapClasses.CorporateCardsUIMap();


        [TestMethod]
        public void CorporateCardsSuccessfullyAddCorporateCardToEmployee()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);
            
            /// Navigate to search employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");
            
            /// Search for employee and edit the record
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();
            
            /// Go to the Corporate Cards page
            cCorpCardsMethods.SelectCorporateCardsPage();
            
            /// Select the Add Corporate Card link
            cCorpCardsMethods.ClickAddCorporateCardLink();
            
            /// Populate the fields on the modal and press save
            cCorpCardsMethods.SelectFirstCorporateCardFromList();
            cCorpCardsMethods.PopulateCardNumber();
            cCorpCardsMethods.SelectActiveTickbox();
            cCorpCardsMethods.PressCorporateCardSaveButton();
            
            /// Validate the Corporate card has been added
            cCorpCardsMethods.ValidateFirstCorporateCardExists();
            
            cCorpCardsMethods.PressEmployeeCancelButton();
            
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();
            cCorpCardsMethods.SelectCorporateCardsPage();
            cCorpCardsMethods.ValidateFirstCorporateCardExists();
        }


        [TestMethod]
        public void CorporateCardsUnsuccessfullyAddCorporateCardToEmployeeWhereDuplicatDetailsUsed()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to search employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for employee and edit the record
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();

            /// Go to the Corporate Cards page
            cCorpCardsMethods.SelectCorporateCardsPage();

            /// Select the Add Corporate Card link
            cCorpCardsMethods.ClickAddCorporateCardLink();

            /// Populate the fields on the modal and press save
            cCorpCardsMethods.SelectFirstCorporateCardFromList();
            cCorpCardsMethods.PopulateCardNumber();
            cCorpCardsMethods.SelectActiveTickbox();
            cCorpCardsMethods.PressCorporateCardSaveButton();

            /// Validate the Corporate card cannot be added
            cCorpCardsMethods.ValidateDuplicateCardMessage();
            cCorpCardsMethods.CancelFromDuplicateCardMessage();
        }


        [TestMethod]
        public void CorporateCardsSuccessfullyEditCorporateCardOnEmployee()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to search employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for employee and edit the record
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();

            /// Go to the Corporate Cards page
            cCorpCardsMethods.SelectCorporateCardsPage();

            /// Select the Edit Corporate Card icon
            cCorpCardsMethods.PressCorporateCardEditIcon();

            /// Populate the fields on the modal with new data and press save
            cCorpCardsMethods.SelectFirstCorporateCardFromListParams.UICardProviderComboBoxSelectedItem = "Premier Inn xls";
            cCorpCardsMethods.SelectFirstCorporateCardFromList();
            cCorpCardsMethods.PopulateCardNumberParams.UICardNumberEditText = "98765";
            cCorpCardsMethods.PopulateCardNumber();
            cCorpCardsMethods.UnselectActiveTickbox();
            cCorpCardsMethods.PressCorporateCardSaveButton();

            /// Validate the Corporate card has been edited
            cCorpCardsMethods.ValidateFirstCorporateCardIsEdited();
            cCorpCardsMethods.PressEmployeeCancelButton();
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();
            cCorpCardsMethods.SelectCorporateCardsPage();
            cCorpCardsMethods.ValidateFirstCorporateCardIsEdited();

            /// Reset Corporate Card for future tests
            cCorpCardsMethods.PressCorporateCardEditIcon();
            cCorpCardsMethods.SelectFirstCorporateCardFromListParams.UICardProviderComboBoxSelectedItem = "Barclaycard";
            cCorpCardsMethods.SelectFirstCorporateCardFromList();
            cCorpCardsMethods.PopulateCardNumberParams.UICardNumberEditText = "12345";
            cCorpCardsMethods.PopulateCardNumber();
            cCorpCardsMethods.SelectActiveTickbox();
            cCorpCardsMethods.PressCorporateCardSaveButton();
            cCorpCardsMethods.ValidateFirstCorporateCardExists();                        
        }


        [TestMethod]
        public void CorporateCardsSuccessfullyAddSecondCorporateCardToEmployee()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to search employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for employee and edit the record
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();

            /// Go to the Corporate Cards page
            cCorpCardsMethods.SelectCorporateCardsPage();

            /// Select the Add Corporate Card link
            cCorpCardsMethods.ClickAddCorporateCardLink();

            /// Populate the fields on the modal and press save
            cCorpCardsMethods.SelectSecondCorporateCardFromList();
            cCorpCardsMethods.PopulateCardNumberParams.UICardNumberEditText = "2468";
            cCorpCardsMethods.PopulateCardNumber();
            cCorpCardsMethods.SelectActiveTickbox();
            cCorpCardsMethods.PressCorporateCardSaveButton();

            /// Validate the Corporate card has been added
            cCorpCardsMethods.ValidateTwoCorporateCardsExist();
            cCorpCardsMethods.PressEmployeeCancelButton();
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();
            cCorpCardsMethods.SelectCorporateCardsPage();
            cCorpCardsMethods.ValidateTwoCorporateCardsExist();

            /// Remove the second Corporate Card
            cCorpCardsMethods.PressSecondCorporateCardDeleteIcon();
            cCorpCardsMethods.ValidateSecondCorporateCardDoesNotExist();
        }


        [TestMethod]
        public void CorporateCardsSuccessfullyDeleteCorporateCardToEmployee()
        {
            /// Logon
            cSharedMethods.Logon(ProductType.expenses, LogonType.administrator);

            /// Navigate to search employee page
            cSharedMethods.NavigateToPage(ProductType.expenses, "/shared/admin/selectemployee.aspx");

            /// Search for employee and edit the record
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();

            /// Go to the Corporate Cards page
            cCorpCardsMethods.SelectCorporateCardsPage();

            /// Select the Delete Corporate Card icon
            cCorpCardsMethods.PressCorporateCardDeleteIcon();

            /// Validate the Corporate card has been delete
            cCorpCardsMethods.ValidateFirstCorporateCardDoesNotExists();
            cCorpCardsMethods.PressEmployeeCancelButton();
            cCorpCardsMethods.SearchForEmployee();
            cCorpCardsMethods.ClickEmployeeEditIcon();
            cCorpCardsMethods.SelectCorporateCardsPage();
            cCorpCardsMethods.ValidateFirstCorporateCardDoesNotExists();
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
