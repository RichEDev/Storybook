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


namespace Auto_Tests.Coded_UI_Tests.Framework.Base_Information.Contract_Configuration_Options.Inflator_Metrics
{
    /// <summary>
    /// Summary description for Inflator Metrics
    /// </summary>
    [CodedUITest]
    public class InflatorMetricsTests
    {
        public InflatorMetricsTests()
        {
        }

        UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap cSharedMethods = new UIMaps.SharedMethodsUIMapClasses.SharedMethodsUIMap();
        UIMaps.InflatorMetricsUIMapClasses.InflatorMetricsUIMap cInflatorMetrics = new UIMaps.InflatorMetricsUIMapClasses.InflatorMetricsUIMap();

        /// <summary>
        /// This test ensures that a new inflator metric definition can be successfully created
        /// </summary>
        [TestMethod]
        public void InflatorMetricsSuccessfullyCreateNewInflatorMetric()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Ensure the cancel button can be used
            cInflatorMetrics.AddInflatorMetricWithCancelParams.UIInflatorMetricDescriEditText = "CodedUI Inflator";
            cInflatorMetrics.AddInflatorMetricWithCancel();
            cInflatorMetrics.ValidateInflatorMetricDoesNotExist();

            /// Add an inflator metric
            cInflatorMetrics.AddInflatorMetric();
            cInflatorMetrics.ValidateAddInflatorMetric();
        }


        /// <summary>
        /// This test ensures that a duplicate inflator metric definition cannot be created
        /// </summary>
        [TestMethod]
        public void InflatorMetricsUnSuccessfullyCreateNewInflatorMetricWithDuplicateDetails()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Add a duplicate inflator metric
            cInflatorMetrics.AddInflatorMetric();
            cInflatorMetrics.ValidateDuplicateInflatorMetric();
        }


        /// <summary>
        /// This test ensures that an inflator metric definition can be successfully edited
        /// </summary>
        [TestMethod]
        public void InflatorMetricsSuccessfullyEditInflatorMetric()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Ensure the cancel button can be used
            cInflatorMetrics.EditInflatorMetricWithCancel();
            cInflatorMetrics.ValidateAddInflatorMetric();

            /// Edit an inflator metric
            cInflatorMetrics.EditInflatorMetric();
            cInflatorMetrics.ValidateEditInflatorMetric();

            /// Reset the values for future use
            cInflatorMetrics.EditInflatorMetricResetValues();
        }


        /// <summary>
        /// This test ensures that an inflator metric definition can be successfully deleted
        /// </summary>
        [TestMethod]
        public void InflatorMetricsSuccessfullyDeleteInflatorMetric()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Ensure the cancel button can be used
            cInflatorMetrics.DeleteInflatorMetricWithCancel();
            cInflatorMetrics.ValidateAddInflatorMetric();

            /// Delete an inflator metric
            cInflatorMetrics.DeleteInflatorMetric();
            cInflatorMetrics.ValidateInflatorMetricDoesNotExist();
        }


        /// <summary>
        /// This test ensures that an inflator metric definition can be successfully archived
        /// </summary>
        [TestMethod]
        public void InflatorMetricsSuccessfullyArchiveInflatorMetric()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Ensure the cancel button can be used
            cInflatorMetrics.ArchiveInflatorMetricWithCancel();
            cInflatorMetrics.ValidateInflatorMetricIsNotArchived();

            /// Archive an inflator metric
            cInflatorMetrics.ArchiveInflatorMetric();
            cInflatorMetrics.ValidateInflatorMetricIsArchived();
        }


        /// <summary>
        /// This test ensures that an inflator metric definition can be successfully un-archived
        /// </summary>
        [TestMethod]
        public void InflatorMetricsSuccessfullyUnArchiveInflatorMetric()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Ensure the cancel button can be used
            cInflatorMetrics.UnArchiveInflatorMetricWithCancel();
            cInflatorMetrics.ValidateInflatorMetricIsArchived();

            /// Un-archive an inflator metric
            cInflatorMetrics.UnArchiveInflatorMetric();
            cInflatorMetrics.ValidateInflatorMetricIsNotArchived();
        }


        /// <summary>
        /// This test ensures that the inflator metrics page layout is correct
        /// </summary>
        [TestMethod]
        public void InflatorMetricsSuccessfullyValidateInflatorMetricsPageLayout()
        {
            /// Logon to framework
            cSharedMethods.Logon(ProductType.framework, LogonType.administrator);

            /// Navigate to the inflator metrics page
            cSharedMethods.NavigateToPage(ProductType.framework, "/contracts/admin/basedefinitions.aspx?bdt=112");

            /// Validate the page layout for contract types

            string sTodaysDate = DateTime.Now.Day.ToString() + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() + " ";

            cInflatorMetrics.ValidateInflatorMetricsPageLayoutExpectedValues.UIJamesLloyd21SeptembePaneDisplayText =
                cGlobalVariables.AdministratorUserName(ProductType.framework);

            cInflatorMetrics.ValidateInflatorMetricsPageLayout();
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
