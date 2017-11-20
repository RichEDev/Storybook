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


namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.Userdefined_Groupings
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class UserDefinedGroupingsTests
    {
        public UserDefinedGroupingsTests()
        {
        }
        private string groupingsURL = "/shared/admin/userdefinedFieldGroupings.aspx";
        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.UserDefinedGroupingsUIMapClasses.UserDefinedGroupingsUIMap cUDFG = new UIMaps.UserDefinedGroupingsUIMapClasses.UserDefinedGroupingsUIMap();


        /// <summary>
        /// This test ensures that a new UserDefinedGrouping can be successfully created
        /// </summary>
        [TestMethod]
        public void UserDefinedGroupingsSuccessfullyCreateNew()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            /// Navigate to the UserDefinedGrouping page
            cSharedMethods.NavigateToPage(ProductType.framework, groupingsURL);
            cUDFG.ClickCreateNewLink();
            cUDFG.GroupNameField();
            cUDFG.SelectProductArea();
            cUDFG.ClickSaveButton();
            /// Assert the add
        }


        /// <summary>
        /// This test ensures that a duplicate UserDefinedGrouping cannot be created
        /// </summary>
        [TestMethod]
        public void UserDefinedGroupingsUnSuccessfullyCreateNewWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            /// Navigate to the UserDefinedGrouping page
            cSharedMethods.NavigateToPage(ProductType.framework, groupingsURL);
            cUDFG.ClickCreateNewLink();
            cUDFG.GroupNameField();
            cUDFG.SelectProductArea();
            cUDFG.ClickSaveButton();
            /// Assert the duplicate dialog
            cUDFG.AssertDuplicateDialog();
            /// Close the dialog
            cUDFG.ClickDuplicateDialogOK();
        }


        /// <summary>
        /// This test ensures that a UserDefinedGrouping can be successfully edited
        /// </summary>
        [TestMethod]
        public void UserDefinedGroupingsSuccessfullyEdit()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            /// Navigate to the UserDefinedGrouping page
            cSharedMethods.NavigateToPage(ProductType.framework, groupingsURL);
            /// Click edit on the top group
            cUDFG.ClickEditButton();
            /// Edit the group name
            cUDFG.EditForm2();

            /// Click edit on the top group
            cUDFG.ClickEditButton();

            cUDFG.AssertEditedFormExpectedValues.UIGroupNameEditText = "__GROUPNAME__";
            /// Assert it has changed
            cUDFG.AssertEditedForm();
            /// Assert the product area is disabled
            cUDFG.AssertProductAreaDisabled();
            /// Save the record
            cUDFG.ClickSaveButtonAfterEdit();
            /// Re-edit the form
            cUDFG.ClickEditButton();
            /// Assert it was saved correctly
            cUDFG.AssertEditedFormExpectedValues.UIGroupNameEditText = "__GROUPNAME__";
            cUDFG.AssertEditedForm();
            /// Click the cancel button
            cUDFG.ClickCloseButton();
        }


        /// <summary>   
        /// This test ensures that a UserDefinedGrouping can be successfully deleted
        /// </summary>
        [TestMethod]
        public void UserDefinedGroupingsSuccessfullyDelete()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);
            /// Navigate to the UserDefinedGrouping page
            cSharedMethods.NavigateToPage(ProductType.framework, groupingsURL);
            cUDFG.ClickDeleteButton();

            cUDFG.AssertGroupDeleted();
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
