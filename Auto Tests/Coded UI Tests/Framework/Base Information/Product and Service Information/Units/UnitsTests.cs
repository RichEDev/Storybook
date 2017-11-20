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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Product_and_Service_Information.Units
{
    /// <summary>
    /// Summary description for Units
    /// </summary>
    [CodedUITest]
    public class UnitsTests
    {
        public UnitsTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.UnitsUIMapClasses.UnitsUIMap cUnits = new UIMaps.UnitsUIMapClasses.UnitsUIMap();        


        /// <summary>
        /// This test ensures that a new Unit definition can be successfully created
        /// </summary>
        [TestMethod]
        public void UnitsSuccessfullyCreateNewUnit()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");

            /// Ensure the cancel button can be used when adding a unit
            cUnits.AddUnitWithCancel();
            cUnits.ValidateUnitDoesNotExist();

            /// Add a unit
            cUnits.AddUnit();
            cUnits.ValidateAddUnit();
        }


        /// <summary>
        /// This test ensures that a duplicate Unit definition cannot be created
        /// </summary>
        [TestMethod]
        public void UnitsUnsuccessfullyCreateNewUnitWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");            

            /// Attempt to add a duplicate unit
            cUnits.AddUnit();
            cUnits.ValidateAddUnitWithDuplicateDetails();
        }


        /// <summary>
        /// This test ensures that a unit definition can be successfully edited
        /// </summary>
        [TestMethod]
        public void UnitsSuccessfullyEditUnit()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");

            /// Ensure the cancel button can be used when editing
            cUnits.EditUnitWithCancel();
            cUnits.ValidateAddUnit();

            /// Edit a unit
            cUnits.EditUnit();
            cUnits.ValidateEditUnit();

            /// Reset the values for future use
            cUnits.EditUnitResetValues();
        }


        /// <summary>
        /// This test ensures that a unit definition can be successfully deleted
        /// </summary>
        [TestMethod]
        public void UnitsSuccessfullyDeleteUnit()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");

            /// Ensure the cancel button can be used when deleting
            cUnits.DeleteUnitWithCancel();
            cUnits.ValidateAddUnit();

            /// Delete a unit
            cUnits.DeleteUnit();
            cUnits.ValidateUnitDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a unit definition can be successfully archived
        /// </summary>
        [TestMethod]
        public void UnitsSuccessfullyArchiveUnit()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");

            /// Ensure the cancel button can be used when archiving
            cUnits.ArchiveUnitWithCancel();
            cUnits.ValidateUnitIsNotArchived();

            /// Archive a unit
            cUnits.ArchiveUnit();
            cUnits.ValidateUnitIsArchived();
        }


        /// <summary>
        /// This test ensures that a unit definition can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void UnitsSuccessfullyUnArchiveUnit()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");

            /// Ensure the cancel button can be used when un-archiving
            cUnits.UnArchiveUnitWithCancel();
            cUnits.ValidateUnitIsArchived();

            /// Un-Archive a unit
            cUnits.UnArchiveUnit();
            cUnits.ValidateUnitIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the units page layout is correct
        /// </summary>
        [TestMethod]
        public void UnitsSuccessfullyValidateUnitsPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the units page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=118");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cUnits.ValidateUnitsPageLayoutExpectedValues.UIJamesLloyd111117SeptPaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate page layout
            cUnits.ValidateUnitsPageLayout();
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
