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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Contract_Configuration_Options.Term_Types
{
    /// <summary>
    /// Summary description for Term Types
    /// </summary>
    [CodedUITest]
    public class TermTypesTests
    {
        public TermTypesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.TermTypesUIMapClasses.TermTypesUIMap cTermTypes = new UIMaps.TermTypesUIMapClasses.TermTypesUIMap();


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesSuccessfullyCreateNewTermType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            /// Ensure the cancel button can be used
            cTermTypes.AddTermTypeWithCancel();
            cTermTypes.ValidateTermTypeDoesNotExist();

            /// Add a term type
            cTermTypes.AddTermType();
            cTermTypes.ValidateAddTermType();
        }


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesUnSuccessfullyCreateNewTermTypeWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            /// Add a duplicate term type
            cTermTypes.AddTermType();
            cTermTypes.ValidateAddDuplicateTermType();
        }


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesSuccessfullyEditTermType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            /// Ensure the cancel button can be used
            cTermTypes.EditTermTypeWithCancel();
            cTermTypes.ValidateAddTermType();

            /// Edit a term type
            cTermTypes.EditTermType();
            cTermTypes.ValidateEditTermType();

            /// Reset the values for future use
            cTermTypes.EditTermTypeResetValues();
        }


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesSuccessfullyDeleteTermType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            /// Ensure the cancel button can be used
            cTermTypes.DeleteTermTypeWithCancel();
            cTermTypes.ValidateAddTermType();

            /// Delete a term type
            cTermTypes.DeleteTermType();
            cTermTypes.ValidateTermTypeDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesSuccessfullyArchiveTermType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            /// Ensure the cancel button can be used
            cTermTypes.ArchiveTermTypeWithCancel();
            cTermTypes.ValidateTermTypeIsNotArchived();

            /// Archive a term type
            cTermTypes.ArchiveTermType();
            cTermTypes.ValidateTermTypeIsArchived();
        }


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesSuccessfullyUnArchiveTermType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            /// Ensure the cancel button can be used
            cTermTypes.UnArchiveTermTypeWithCancel();
            cTermTypes.ValidateTermTypeIsArchived();

            /// Un-archive a term type
            cTermTypes.UnArchiveTermType();
            cTermTypes.ValidateTermTypeIsNotArchived();
        }


        /// <summary>
        /// This test ensures that a term type can be successfully
        /// </summary>
        [TestMethod]
        public void TermTypesSuccessfullyValidatePageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the term types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=113");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cTermTypes.ValidateTermTypesPageLayoutExpectedValues.UIJamesLloyd20SeptembePaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate the page layout
            cTermTypes.ValidateTermTypesPageLayout();
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
