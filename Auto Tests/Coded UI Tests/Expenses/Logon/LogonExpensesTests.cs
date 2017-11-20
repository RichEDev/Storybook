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


namespace Auto_Tests.Coded_UI_Tests.Expenses.Logon
{
    /// <summary>
    /// Summary description for LogonExpensesTests
    /// </summary>
    [CodedUITest]
    public class LogonExpensesTests
    {
        public LogonExpensesTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.LogonUIMapClasses.LogonUIMap cLogonMethods = new UIMaps.LogonUIMapClasses.LogonUIMap();


        [TestMethod]
        public void LogonExpensesSuccessfullyValidateTheLogonPageLayout()
        {
            //Open web browser and navigate to framwork logon page.
            cSharedMethods.StartIE(ProductType.expenses);

            //Validate Expenses logon page.
            cLogonMethods.ValidateSpendManagementLogonPageLayout();
            cLogonMethods.ValidateExpensesOnlyAttributesLogonPageLayout();

            //Validate Expesnes Forgotten Details apge.
            cLogonMethods.NavigateLogonToForgottenDetails();
            cLogonMethods.ValidateSpendManagementForgottenDetailsPageLayout();
            cLogonMethods.ValidateExpensesOnlyAttributesForgottenDetailsPageLayout();

            cLogonMethods.CancelForgottenDetails();
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
