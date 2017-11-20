﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace Auto_Tests.Coded_UI_Tests.Expenses.Add_Expense
{
    /// <summary>
    /// Summary description for CodedUITest2
    /// </summary>
    [CodedUITest]
    public class CodedUITest2
    {
        public CodedUITest2()
        {
        }

        [DataSourceAttribute("Microsoft.VisualStudio.TestTools.DataSource.TestCase", "http://lithium:8080/tfs/software europe;Spend Management", "35526", DataAccessMethod.Sequential), TestMethod]
        public void CodedUITestMethod1()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            this.UIMap.LogonsuccessfullyasAdministrator();
            this.UIMap.NavigatetoAdminBaseInformationCostCodes();
            this.UIMap.ClickAddCostCode();
            this.UIMap.PopulatetheCostCodefieldwithCostCodeParams.UICostCodeEditText = TestContext.DataRow["CostCode"].ToString();
            this.UIMap.PopulatetheCostCodefieldwithCostCode();
            this.UIMap.PopulatretheDescriptionfieldwithDescriptionParams.UIDescriptionEditText = TestContext.DataRow["Description"].ToString();
            this.UIMap.PopulatretheDescriptionfieldwithDescription();
            this.UIMap.PopulatetheCostCodeOwnerwithCostCodeOwnerParams.UICostCodeOwnerEditText = TestContext.DataRow["Cost"].ToString();
            this.UIMap.PopulatetheCostCodeOwnerwithCostCodeOwner();
            this.UIMap.PopulatefieldswithnewanduniquedataandpressSave();
        }

        [DataSourceAttribute("System.Data.SqlClient",
            "Data Source=company.software-europe.co.uk;Initial Catalog=AutoTestingDataSources;User ID=spenduser;Password=P3ngu1ns"
            , "costcodes", DataAccessMethod.Sequential),
        TestMethod]
        public void CodedUITestMethod2()
        {
            this.UIMap.ClickAddCostCode();
            this.UIMap.PopulatetheCostCodefieldwithCostCodeParams.UICostCodeEditText = TestContext.DataRow["CostCode"].ToString();
            this.UIMap.PopulatetheCostCodefieldwithCostCode();
            this.UIMap.PopulatretheDescriptionfieldwithDescriptionParams.UIDescriptionEditText = TestContext.DataRow["Description"].ToString();
            this.UIMap.PopulatretheDescriptionfieldwithDescription();
            //this.UIMap.PopulatetheCostCodeOwnerwithCostCodeOwnerParams.UICostCodeOwnerEditText = TestContext.DataRow["Cost"].ToString();
            //this.UIMap.PopulatetheCostCodeOwnerwithCostCodeOwner();
            this.UIMap.PopulatefieldswithnewanduniquedataandpressSave();
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

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
