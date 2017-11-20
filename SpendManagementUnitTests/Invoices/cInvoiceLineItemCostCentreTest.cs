using SpendManagementLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cInvoiceLineItemCostCentreTest and is intended
    ///to contain all cInvoiceLineItemCostCentreTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cInvoiceLineItemCostCentreTest
    {


        private TestContext testContextInstance;

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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ProjectCode
        ///</summary>
        [TestMethod()]
        public void ProjectCodeTest()
        {
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre(); // TODO: Initialize to an appropriate value
            cProjectCode expected = null; // TODO: Initialize to an appropriate value
            cProjectCode actual;
            target.ProjectCode = expected;
            actual = target.ProjectCode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PercentSplit
        ///</summary>
        [TestMethod()]
        public void PercentSplitTest()
        {
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PercentSplit = expected;
            actual = target.PercentSplit;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InvoiceLineItemCostCentreId
        ///</summary>
        [TestMethod()]
        public void InvoiceLineItemCostCentreIdTest()
        {
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.InvoiceLineItemCostCentreId = expected;
            actual = target.InvoiceLineItemCostCentreId;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Department
        ///</summary>
        [TestMethod()]
        public void DepartmentTest()
        {
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre(); // TODO: Initialize to an appropriate value
            cDepartment expected = null; // TODO: Initialize to an appropriate value
            cDepartment actual;
            target.Department = expected;
            actual = target.Department;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CostCode
        ///</summary>
        [TestMethod()]
        public void CostCodeTest()
        {
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre(); // TODO: Initialize to an appropriate value
            cCostCode expected = null; // TODO: Initialize to an appropriate value
            cCostCode actual;
            target.CostCode = expected;
            actual = target.CostCode;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for cInvoiceLineItemCostCentre Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceLineItemCostCentreConstructorTest1()
        {
            Nullable<int> invoiceLineItemCostCentreId = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cDepartment department = null; // TODO: Initialize to an appropriate value
            cCostCode costcode = null; // TODO: Initialize to an appropriate value
            cProjectCode projectcode = null; // TODO: Initialize to an appropriate value
            Nullable<int> percentsplit = new Nullable<int>(); // TODO: Initialize to an appropriate value
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre(invoiceLineItemCostCentreId, department, costcode, projectcode, percentsplit);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for cInvoiceLineItemCostCentre Constructor
        ///</summary>
        [TestMethod()]
        public void cInvoiceLineItemCostCentreConstructorTest()
        {
            cInvoiceLineItemCostCentre target = new cInvoiceLineItemCostCentre();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
