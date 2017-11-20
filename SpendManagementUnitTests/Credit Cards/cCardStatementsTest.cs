using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Data;
using SpendManagementUnitTests.Global_Objects;
using System.Configuration;
using System.IO;

namespace SpendManagementUnitTests
{
    
    
    /// <summary>
    ///This is a test class for cCardStatementsTest and is intended
    ///to contain all cCardStatementsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCardStatementsTest
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
        ///A test for createStatementDropDown
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_createStatementDropDown()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            string cardnumber = "";
            cEmployeeCorporateCards empCards = new cEmployeeCorporateCards(cGlobalVariables.AccountID);

            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");

                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);

                cardnumber = cCardStatementObject.GetCardNumber(statement.statementid);

                clsCardStatements.allocateCard(statement.statementid, cGlobalVariables.EmployeeID, cardnumber);

                ListItem[] lst = clsCardStatements.createStatementDropDown(cGlobalVariables.EmployeeID);
                Assert.IsTrue(lst.Length > 0);
            }
            finally
            {
                if(statement != null)
                {
                    string sql = "select corporatecardid from employee_corporate_cards where cardnumber = @cardnumber";
                    DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                    db.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
                    int cardID = db.getcount(sql);

                    empCards.DeleteCorporateCard(cardID);
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for CacheList
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_CacheList()
        {
            cCardStatement statement = null;
            try
            {
                System.Web.Caching.Cache Cache = System.Web.HttpRuntime.Cache;
                Cache.Remove("cardstatements" + cGlobalVariables.AccountID);
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                SortedList<int, cCardStatement> expected = (SortedList<int, cCardStatement>)Cache["cardstatements" + cGlobalVariables.AccountID];

                Assert.IsNotNull(expected);
                Assert.IsTrue(expected.Count > 0);
                Cache.Remove("cardstatements" + cGlobalVariables.AccountID);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
            
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_CreateDropDown()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                ListItem[] lst = clsCardStatements.CreateDropDown();
                Assert.IsTrue(lst.Length > 0);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for GetCardRecordTypeData for a flat file
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataForFlatFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "Barclaycard Flat File.txt"));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "Barclaycard Flat File.txt"));
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNotNull(import);
            Assert.IsTrue(import.filecontents.Count > 0);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData for a fixed width file
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataForFixedWidthFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("AMEX Monthly Text");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "Amex Fixed Width.txt"));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "Amex Fixed Width.txt"));
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNotNull(import);
            Assert.IsTrue(import.filecontents.Count > 0);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData for a fixed width file
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataForXLSFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("RBS Credit Card");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "RBS Monthly XLS.xls"));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "RBS Monthly XLS.xls"));
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNotNull(import);
            Assert.IsTrue(import.filecontents.Count > 0);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData with a null template
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataWithNullTemplate()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("AMEX Monthly Text");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "Amex Fixed Width.txt"));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "Amex Fixed Width.txt"));
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(null, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNull(import);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData with an invalid file
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataWithInvalidFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "Invalid File Data - Barclaycard.txt"));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "Invalid File Data - Barclaycard.txt"));
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNull(import);
        }

        /// <summary>
        ///A test for GetVCF4CompanyRecords
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_GetVCF4CompanyRecords()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard VCF4");

            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "VCF4.txt"));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "VCF4.txt"));
            }

            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardCompany], fileData);
            SortedList<string, string> lstComps = clsCardStatements.GetVCF4CompanyRecords(import, temp.RecordTypes[CardRecordType.CardCompany]);
            Assert.IsTrue(lstComps.Count > 0);
        }

        /// <summary>
        ///A test for ImportCardTransactions
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_ImportCardTransactions()
        {
            cCardStatement statement = null;
            try
            {
                string provider = "Barclaycard";
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardStatementObject.ImportCardTransactions(statement, provider);
                CardImportSuccess impSuccess = cCardStatementObject.GetStatementTransactionImportSuccess(statement.statementid, provider); ;

                CheckImportSuccess(impSuccess);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for ImportCardTransactions for VCF4 File Format
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_ImportCardTransactionsForVCF4FileFormat()
        {
            cCardStatement statement = null;
            try
            {
                string provider = "Barclaycard VCF4";
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardVCF4CardStatement());

                cCardStatementObject.ImportVCF4CardCompanies(statement);
                cCardStatementObject.ImportCardTransactions(statement, provider);
                CardImportSuccess impSuccess = cCardStatementObject.GetStatementTransactionImportSuccess(statement.statementid, provider); ;
                
                CheckImportSuccess(impSuccess);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                    cCardStatementObject.DeleteVCF4CardCompanies();
                }
            }
        }

        /// <summary>
        ///A test for ImportCardTransactions for a File Format with rows to exclude
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_ImportCardTransactionsForFileWithExclusionRows()
        {
            cCardStatement statement = null;
            try
            {
                string provider = "Allstar Fuel Card";
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardStatementObject.ImportCardTransactions(statement, provider);
                CardImportSuccess impSuccess = cCardStatementObject.GetStatementTransactionImportSuccess(statement.statementid, provider);
                CheckImportSuccess(impSuccess);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        /// Check the success of a transaction import in the unit tests
        /// </summary>
        /// <param name="impSuccess"></param>
        public void CheckImportSuccess(CardImportSuccess impSuccess)
        {
            switch (impSuccess)
            {
                case CardImportSuccess.Success:
                    Assert.IsTrue(true);
                    break;
                case CardImportSuccess.Fail:
                    Assert.Fail();
                    break;
                case CardImportSuccess.TransactionCountsDontMatch:
                    Assert.Fail("Failed due to transaction counts not matching");
                    break;
                default:
                    break;

            }
        }

        /// <summary>
        ///A test for addStatement
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_addStatement()
        {
            cCardStatements clsCardStatements = null;
            int statementID = 0;
            try
            {
                cCardStatement expected = cCardStatementObject.GetBarclaycardCardStatement();
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                statementID = clsCardStatements.addStatement(expected);
                cCardStatement actual = clsCardStatements.getStatementById(statementID); 
                Assert.IsNotNull(actual);
                cCompareAssert.AreEqual(expected, actual, cCardStatementObject.lstOmittedProperties);
            }
            finally
            {
                if (statementID > 0)
                {
                    cCardStatementObject.DeleteCardStatement(statementID);
                }
            }
        }

        /// <summary>
        ///A test for addStatement with a statement date of null
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_addStatementWithNullStatementDate()
        {
            cCardStatements clsCardStatements = null;
            int statementID = 0;
            try
            {
                cCardStatement expected = cCardStatementObject.GetBarclaycardCardStatementWithNullStatementDate();
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                statementID = clsCardStatements.addStatement(expected);
                cCardStatement actual = clsCardStatements.getStatementById(statementID);
                Assert.IsNotNull(actual);
                cCompareAssert.AreEqual(expected, actual, cCardStatementObject.lstOmittedProperties);
            }
            finally
            {
                if (statementID > 0)
                {
                    cCardStatementObject.DeleteCardStatement(statementID);
                }
            }
        }

        /// <summary>
        ///A test for addStatement as a delegate
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_addStatementAsDelegate()
        {
            cCardStatements clsCardStatements = null;
            int statementID = 0;
            try
            {
                //Set the delegate for the current user
                cEmployeeObject.CreateUTDelegateEmployee();
                System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;

                cCardStatement expected = cCardStatementObject.GetBarclaycardCardStatement();
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                statementID = clsCardStatements.addStatement(expected);
                cCardStatement actual = clsCardStatements.getStatementById(statementID);
                Assert.IsNotNull(actual);
                cCompareAssert.AreEqual(expected, actual, cCardStatementObject.lstOmittedProperties);
            }
            finally
            {
                if (statementID > 0)
                {
                    cCardStatementObject.DeleteCardStatement(statementID);
                    System.Web.HttpContext.Current.Session["myid"] = null;
                }
            }
        }

        /// <summary>
        ///A test for updateStatement
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_updateStatement()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatement expected = new cCardStatement(statement.statementid, "UpdatedName", new DateTime(2011, 01, 31), statement.corporatecard, statement.createdon, statement.createdby, null, null);
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                int statementID = clsCardStatements.updateStatement(expected);
                Assert.AreEqual(expected.statementid, statementID);
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cCardStatement actual = clsCardStatements.getStatementById(statementID);
                Assert.IsNotNull(statement);
                cCompareAssert.AreEqual(expected, actual, cCardStatementObject.lstOmittedProperties);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for updateStatement with a null statement date
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_updateStatementWithNullStatementDate()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatement expected = new cCardStatement(statement.statementid, "UpdatedName", null, statement.corporatecard, statement.createdon, statement.createdby, null, null);
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                int statementID = clsCardStatements.updateStatement(expected);
                Assert.AreEqual(expected.statementid, statementID);
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cCardStatement actual = clsCardStatements.getStatementById(statementID);
                Assert.IsNotNull(statement);
                cCompareAssert.AreEqual(expected, actual, cCardStatementObject.lstOmittedProperties);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for updateStatement as a delegate
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_updateStatementAsADelegate()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                cEmployeeObject.CreateUTDelegateEmployee();
                System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatement expected = new cCardStatement(statement.statementid, "UpdatedName", new DateTime(2011, 01, 31), statement.corporatecard, statement.createdon, statement.createdby, null, null);

                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                int statementID = clsCardStatements.updateStatement(expected);
                Assert.AreEqual(expected.statementid, statementID);
                cCardStatement actual = clsCardStatements.getStatementById(statementID);
                Assert.IsNotNull(statement);
                cCompareAssert.AreEqual(expected, actual, cCardStatementObject.lstOmittedProperties);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                    System.Web.HttpContext.Current.Session["myid"] = null;
                }
            }
        }

        /// <summary>
        ///A test for deleteStatement
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_deleteStatement()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                clsCardStatements.deleteStatement(statement.statementid);
                statement = clsCardStatements.getStatementById(statement.statementid);
                Assert.IsNull(statement);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for deleteStatement with an invalid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_deleteStatementInvalidID()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                clsCardStatements.deleteStatement(statement.statementid);
                cCardStatement tempStatement = clsCardStatements.getStatementById(0);
                Assert.IsNotNull(statement);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for deleteStatement as a delegate
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_deleteStatementAsDelegate()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                cEmployeeObject.CreateUTDelegateEmployee();
                System.Web.HttpContext.Current.Session["myid"] = cGlobalVariables.DelegateID;
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                clsCardStatements.deleteStatement(statement.statementid);
                statement = clsCardStatements.getStatementById(statement.statementid);
                Assert.IsNull(statement);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                    System.Web.HttpContext.Current.Session["myid"] = null;
                }
            }
        }

        /// <summary>
        ///A test for allocateCard and getEmployeeTransactions. Both tested here as thay are both required to test this functionality
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_allocateCardAndGetEmployeeTransactions()
        {
            cCardStatements clsCardStatements = null; 
            cCardStatement statement = null;
            string cardnumber = "";
            cEmployeeCorporateCards empCards = new cEmployeeCorporateCards(cGlobalVariables.AccountID);

            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");

                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            
                cardnumber = cCardStatementObject.GetCardNumber(statement.statementid);
                
                clsCardStatements.allocateCard(statement.statementid, cGlobalVariables.EmployeeID, cardnumber);

                DataSet ds = clsCardStatements.getEmployeeTransactions(statement.statementid, cGlobalVariables.EmployeeID);

                Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
            }
            finally
            {
                if (statement != null)
                {
                    string sql = "select corporatecardid from employee_corporate_cards where cardnumber = @cardnumber";
                    DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
                    db.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
                    int cardID = db.getcount(sql);

                    empCards.DeleteCorporateCard(cardID);
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for getInvalidCountries by importing a file that has invalid countries on
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_getInvalidCountries()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("BarclayCard");
                //Need to import data to get some invalid countries back
                byte[] fileData;
                try
                {
                    fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "Invalid Currencies and Countries - Barclaycard.txt"));
                }
                catch
                {
                    // try D drive
                    fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "Invalid Currencies and Countries - Barclaycard.txt"));
                }

                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);     
                clsCardStatements.ImportCardTransactions(temp, statement.statementid, statement.corporatecard.cardprovider, fileData, import);
                SortedList<int, string> invalidCountries = clsCardStatements.getInvalidCountries();

                Assert.IsTrue(invalidCountries.Count > 0);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for getInvalidCurrencies by importing a file that has invalid currencies on
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_getInvalidCurrencies()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("BarclayCard");

                //Need to import data to get some invalid currencies back
                byte[] fileData;
                try
                {
                    fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + "Invalid Currencies and Countries - Barclaycard.txt"));
                }
                catch
                {
                    // try D drive
                    fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + "Invalid Currencies and Countries - Barclaycard.txt"));
                }
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);
                clsCardStatements.ImportCardTransactions(temp, statement.statementid, statement.corporatecard.cardprovider, fileData, import);
                SortedList<int, string> invalidCurrencies = clsCardStatements.getInvalidCurrencies();

                Assert.IsTrue(invalidCurrencies.Count > 0);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for getStatementById
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_getStatementById()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cCardStatement actual = clsCardStatements.getStatementById(statement.statementid);
                Assert.IsNotNull(actual);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for getStatementById with an invalid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_getStatementByIdWithInvalidID()
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            
            cCardStatement actual = clsCardStatements.getStatementById(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for getTransactionById
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_getTransactionById()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");
                int transactionID = cCardStatementObject.GetTransactionID(statement.statementid);
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cCardTransaction trans = clsCardStatements.getTransactionById(transactionID);
                Assert.IsNotNull(trans);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for getTransactionById with an invalid ID
        ///</summary>
        [TestCategory("Continuous Integration"), TestMethod()]
        public void cCardStatements_getTransactionByIdInvalidID()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");
                clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
                cCardTransaction trans = clsCardStatements.getTransactionById(0);
                Assert.IsNull(trans);
            }
            finally
            {
                if (statement != null)
                {
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }
    }
}
