using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spend_Management;
using SpendManagementLibrary;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UnitTest2012Ultimate
{
    [TestClass]
    public class cCardTemplatesTests
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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion


        /// <summary>
        ///A test for getTemplate for Allstar fuel
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForAllstarFuelCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("Allstar Fuel Card", 2, 0, ImportType.FlatFile, ",", 0, new SortedList<CardRecordType, cCardRecordType>(), "\"");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForAMEXDailyText()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("AMEX Daily Text", 0, 0, ImportType.FixedWidth, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForAMEXMonthlyText()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("AMEX Monthly Text", 0, 1, ImportType.FixedWidth, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForAMEXMonthlyXLS()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("AMEX Monthly XLS", 2, 1, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForBarclaycardEnhanced()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("Barclaycard Enhanced", 2, 2, ImportType.Excel, "", 1, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForBarclaycardVCF4()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("Barclaycard VCF4", 0, 0, ImportType.FlatFile, "\\t", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForBarclaycard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("Barclaycard", 1, 1, ImportType.FlatFile, "{", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForDinersCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("Diners Card", 4, 2, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForFuelCardXLS()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);
            cCardTemplate expected = new cCardTemplate("Fuel Card xls", 3, 2, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForMBNACreditCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("MBNA Credit Card", 1, 1, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForPremierInnXLS()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("Premier Inn XLS", 2, 1, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForRBSCreditCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("RBS Credit Card", 2, 0, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
        [TestCategory("Spend Management"), TestCategory("Card Templates"), TestMethod()]
        public void cCardTemplates_getTemplateForRBSPurchaseCard()
        {
            cCardTemplates clsCardTemplates = new cCardTemplates(GlobalTestVariables.AccountId);

            cCardTemplate expected = new cCardTemplate("RBS Purchase Card", 2, 0, ImportType.Excel, "", 0, new SortedList<CardRecordType, cCardRecordType>(), "");
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
