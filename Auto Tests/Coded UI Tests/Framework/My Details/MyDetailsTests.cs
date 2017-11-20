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


namespace Auto_Tests.Coded_UI_Tests.Framework.My_Details
{
    /// <summary>
    /// Summary description for My Details Within Framework
    /// </summary>
    [CodedUITest]
    public class MyDetailsTests
    {
        public MyDetailsTests()
        {
        }


        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.MyDetailsUIMapClasses.MyDetailsUIMap cMyDetails = new UIMaps.MyDetailsUIMapClasses.MyDetailsUIMap();


        /// <summary>
        /// Associated Manual Test: 28991 - Successfully Verify Framework Page Attributes are Displayed and Non-Framework Attributes are Hidden.
        /// </summary>
        [TestMethod]
        public void MyDetailsSuccessfullyVerifyFrameworkPageAttributesAreDisplayedAndNonFrameworkAttributesAreHidden()
        {
            /// Logon to Framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the My Details page
            cSharedMethods.NavigateToPage(ProductType.framework, "/shared/information/mydetails.aspx");

            /// Validate all non-Framework fields are not shown
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UIPayrollNoEdit.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UINumberPersonalMilesCEdit.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UICreditorPurchaseLedgEdit.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UICurrentMileageEdit.Exists);
            //Assert.AreEqual(true, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UIPositionEdit.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UITbl_ESRAssignmentGriRow.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UICtl00contentmainccbdComboBox.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UICtl00contentmainccbdComboBox1.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UICtl00contentmainccbdComboBox2.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UICtl00_contentmain_ccTable.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UITbl_gridEmployeeCarsRow.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UINameEdit.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UIAccountNumberEdit.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UIDivExpensesDetailsPane.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UITbl_gridHomeAddresseRow.Exists);
            //Assert.AreEqual(false, cMyDetails.UIChangeMyDetailsWindoWindow.UIChangeMyDetailsDocument1.UITbl_gridWorkAddresseRow.Exists);                       
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
