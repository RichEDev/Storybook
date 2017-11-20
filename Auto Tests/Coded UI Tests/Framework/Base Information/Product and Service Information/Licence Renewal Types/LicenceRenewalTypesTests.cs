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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Product_and_Service_Information.Licence_Renewal_Types
{
    /// <summary>
    /// Summary description for Licence Renewal Types
    /// </summary>
    [CodedUITest]
    public class LicenceRenewalTypesTests
    {
        public LicenceRenewalTypesTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.LicenceRenewalTypeUIMapClasses.LicenceRenewalTypeUIMap cLicenceRenewal = new UIMaps.LicenceRenewalTypeUIMapClasses.LicenceRenewalTypeUIMap();


        /// <summary>
        /// This test ensures that a new licence renewal type definition can be successfully created
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeSuccessfullyCreateNewLicenceRenewalType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            /// Ensure the edit button can be used
            cLicenceRenewal.AddLicenceRenewalWithCancel();
            cLicenceRenewal.ValidateLicenceRenewalDoesNotExist();

            /// Add a licence renewal type
            cLicenceRenewal.AddLicenceRenewal();
            cLicenceRenewal.ValidateAddLicenceRenewal();
        }


        /// <summary>
        /// This test ensures that a duplicate licence renewal type definition cannnot be created
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeUnSuccessfullyCreateNewLicenceRenewalTypeWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            /// Add a duplicate licence renewal type
            cLicenceRenewal.AddLicenceRenewal();
            cLicenceRenewal.ValidateAddDuplicateLicenceRenewal();
        }


        /// <summary>
        /// This test ensures that a licence renewal type definition can be successfully edited
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeSuccessfullyEditLicenceRenewalType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            /// Ensure the edit button can be used
            cLicenceRenewal.EditLicenceRenewalWithCancel();
            cLicenceRenewal.ValidateAddLicenceRenewal();

            /// Edit a licence renewal type
            cLicenceRenewal.EditLicenceRenewal();
            cLicenceRenewal.ValidateEditLicenceRenewal();

            /// Reset the values for future use
            cLicenceRenewal.EditLicenceRenewalResetValues();
        }


        /// <summary>
        /// This test ensures that a licence renewal type definition can be successfully deleted
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeSuccessfullyDeleteLicenceRenewalType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            /// Ensure the edit button can be used
            cLicenceRenewal.DeleteLicenceRenewalWithCancel();
            cLicenceRenewal.ValidateAddLicenceRenewal();

            /// Delete a licence renewal type
            cLicenceRenewal.DeleteLicenceRenewal();
            cLicenceRenewal.ValidateLicenceRenewalDoesNotExist();
        }


        /// <summary>
        /// This test ensures that a licence renewal type definition can be successfully archived
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeSuccessfullyArchiveLicenceRenewalType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            /// Ensure the edit button can be used
            cLicenceRenewal.ArchiveLicenceRenewalWithCancel();
            cLicenceRenewal.ValidateLicenceRenewalIsNotArchived();

            /// Archive a licence renewal type
            cLicenceRenewal.ArchiveLicenceRenewal();
            cLicenceRenewal.ValidateLicenceRenewalIsArchived();
        }


        /// <summary>
        /// This test ensures that a licence renewal type definition can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeSuccessfullyUnArchiveLicenceRenewalType()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            /// Ensure the edit button can be used
            cLicenceRenewal.UnArchiveLicenceRenewalWithCancel();
            cLicenceRenewal.ValidateLicenceRenewalIsArchived();

            /// Un-Archive a licence renewal type
            cLicenceRenewal.UnArchiveLicenceRenewal();
            cLicenceRenewal.ValidateLicenceRenewalIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the page layout is correct for licence renewal types
        /// </summary>
        [TestMethod]
        public void LicenceRenewalTypeSuccessfullyValidateLicenceRenewalTypesPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the licence renewal types page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=115");

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cLicenceRenewal.ValidateLicenceRenewalPageLayoutExpectedValues.UIJamesLloyd20SeptembePaneDisplayText =
                cGlobalVariables.EntireUsername(ProductType.framework, LogonType.administrator);

            /// Validate the page layout
            cLicenceRenewal.ValidateLicenceRenewalPageLayout();
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
