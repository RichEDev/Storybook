using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SpendManagementLibrary;
using SpendManagementUnitTests.Global_Objects;
using System.Collections.Generic;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCardTemplatesTest and is intended
    ///to contain all cCardTemplatesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCardTemplatesTest
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
        ///A test for getTemplate for Allstar fuel
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForAllstarFuelCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetAllstarFuelCardTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("Allstar Fuel Card");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 4);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 32);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 4);
        }

        /// <summary>
        ///A test for getTemplate for AMEX Daily Text
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForAMEXDailyText()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetAMEXDailyTextTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("AMEX Daily Text");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 79);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for AMEX Monthly Text
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForAMEXMonthlyText()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetAMEXMonthlyTextTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("AMEX Monthly Text");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 20);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for AMEX Monthly XLS
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForAMEXMonthlyXLS()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetAMEXMonthlyXLSTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("AMEX Monthly XLS");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 20);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for Barclaycard Enhanced
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForBarclaycardEnhanced()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetBarclaycardEnhancedTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("Barclaycard Enhanced");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 28);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for Barclaycard VCF4
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForBarclaycardVCF4()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetBarclaycardVCF4Template();
            cCardTemplate actual = clsCardTemplates.getTemplate("Barclaycard VCF4");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 76);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);

            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardCompany));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardCompany].HeaderFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardCompany].FooterFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardCompany].Fields.Count == 2);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardCompany].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for Barclaycard
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForBarclaycard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetBarclaycardTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("Barclaycard");
            
            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 1);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 14);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for Diners Card
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForDinersCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetDinersCardTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("Diners Card");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 10);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for Fuel Card xls
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForFuelCardXLS()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);
            cCardTemplate expected = cCardTemplateObject.GetFuelCardXLSTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("Fuel Card xls");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 7);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 14);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for MBNA Credit Card
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForMBNACreditCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetMBNACreditCardTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("MBNA Credit Card");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 9);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for Premier Inn XLS
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForPremierInnXLS()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetPremierInnXLSTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("Premier Inn XLS");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 3);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 16);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for RBS Credit Card
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForRBSCreditCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetRBSCreditCardTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("RBS Credit Card");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 4);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 16);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }

        /// <summary>
        ///A test for getTemplate for RBS Purchase Card
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardTemplates_getTemplateForRBSPurchaseCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(cGlobalVariables.AccountID);

            cCardTemplate expected = cCardTemplateObject.GetRBSPurchaseCardTemplate();
            cCardTemplate actual = clsCardTemplates.getTemplate("RBS Purchase Card");

            List<string> lstOmittedProperties = new List<string>();
            lstOmittedProperties.Add("RecordTypes");
            cCompareAssert.AreEqual(expected, actual, lstOmittedProperties);
            Assert.IsNotNull(actual.RecordTypes);
            Assert.IsTrue(actual.RecordTypes.ContainsKey(CardRecordType.CardTransaction));
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].HeaderFields.Count == 4);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].FooterFields.Count == 0);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].Fields.Count == 33);
            Assert.IsTrue(actual.RecordTypes[CardRecordType.CardTransaction].ExcludeValues.Count == 0);
        }
    }
}
