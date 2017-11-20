using Moq;
using SpendManagementLibrary.Cards;
using SpendManagementLibrary.Interfaces;
using Spend_Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Diagnostics;
using UnitTest2012Ultimate.DatabaseMock;

namespace UnitTest2012Ultimate
{       
    /// <summary>
    ///This is a test class for cCardStatementsTest and is intended
    ///to contain all cCardStatementsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class cCardStatementsTests
    {

        #region Additional test attributes

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        [TestCleanup()]
        public void MyTestCleanUp()
        {
            HelperMethods.ClearTestDelegateID();
        }

        #endregion

        private static Mock<IDBConnection> database;
        const string strsql =
            "SELECT statementid, name, statementdate, cardproviderid, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy FROM dbo.card_statements_base";

        private const string corpCardsSql =
            "select cardproviderid, claimants_settle_bill, createdon, createdby, modifiedon, modifiedby, allocateditem, blockcash, reconciled_by_admin, singleclaim, blockunmatched from dbo.corporate_cards";

        [TestInitialize]
        public void TestInitialize()
        {
            (new Utilities.DistributedCaching.Cache()).Delete(1, string.Empty, "cardstatements");

            var cardProvider = new cCardProvider(1, "visa", CorporateCardType.CreditCard, new DateTime(2013, 01, 01), 1, new DateTime(2013, 01, 01), 1);
            var corporateCard = new cCorporateCard(cardProvider, false, new DateTime(2013, 01, 01), 1, new DateTime(2013, 01, 01), 1, 1, false, false, false, false);
            var cardStatement = new cCardStatement(GlobalTestVariables.AccountId, cardProvider.cardproviderid, 1, "name", new DateTime(2013, 01, 25), new DateTime(2013, 02, 01), 0, new DateTime(2013, 10, 01), 1);
            var statementsReader = Reader.MockReaderDataFromClassData(strsql, new List<object> { cardStatement })
                               .AddAlias<cCardStatement>("cardproviderid", c => c.Corporatecard.cardprovider.cardproviderid);
            var corpCardsReader = Reader.MockReaderDataFromClassData(corpCardsSql, new List<object> { corporateCard })
                                        .AddAlias<cCorporateCard>("cardproviderid", c => c.cardprovider.cardproviderid)
                                        .AddAlias<cCorporateCard>("claimants_settle_bill", c => c.claimantsettlesbill)
                                        .AddAlias<cCorporateCard>("reconciled_by_admin", c => c.reconciledbyadministrator);
            database = Reader.NormalDatabase(new[] { statementsReader, corpCardsReader });
        }

        [TestMethod]
        public void cCardStatements_CachesList()
        {
            var cardStatements1 = new cCardStatements(GlobalTestVariables.AccountId, database.Object);
            Assert.IsTrue(cardStatements1.CreateDropDown().Length >= 1);
            database.Verify(d => d.GetReader(strsql, CommandType.Text), Times.AtMostOnce());

            var cardStatements2 = new cCardStatements(GlobalTestVariables.AccountId, database.Object);
            Assert.IsTrue(cardStatements2.CreateDropDown().Length >= 1);
            database.Verify(d => d.GetReader(strsql, CommandType.Text), Times.AtMostOnce());
        }

        /// <summary>
        ///A test for createStatementDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_createStatementDropDown()
        {
            int AccountID = GlobalTestVariables.AccountId;

            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            string cardnumber = "";
            cEmployeeCorporateCards empCards = new cEmployeeCorporateCards(AccountID);

            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");

                clsCardStatements = new cCardStatements(AccountID);

                cardnumber = cCardStatementObject.GetCardNumber(statement.statementid);

                clsCardStatements.allocateCard(statement.statementid, GlobalTestVariables.EmployeeId, cardnumber);

                ListItem[] lst = clsCardStatements.createStatementDropDown(GlobalTestVariables.EmployeeId);
                Assert.IsTrue(lst.Length > 0);
            }
            finally
            {
                if(statement != null)
                {
                    string sql = "select corporatecardid from employee_corporate_cards where cardnumber = @cardnumber";
                    DBConnection db = new DBConnection(cAccounts.getConnectionString(AccountID));
                    db.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
                    int cardID = db.getcount(sql);

                    empCards.DeleteCorporateCard(cardID);
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for CreateDropDown
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_CreateDropDown()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataForFlatFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Barclaycard Flat File.txt");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Barclaycard Flat File.txt");
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNotNull(import);
            Assert.IsTrue(import.filecontents.Count > 0);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData for a fixed width file
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataForFixedWidthFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("AMEX Monthly Text");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Amex Fixed Width.txt");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Amex Fixed Width.txt");
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNotNull(import);
            Assert.IsTrue(import.filecontents.Count > 0);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData for a fixed width file
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataForXLSFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("RBS Credit Card");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "RBS Monthly XLS.xls");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "RBS Monthly XLS.xls");
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNotNull(import);
            Assert.IsTrue(import.filecontents.Count > 0);
        }

        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_VerifyDatesInExcelImport()
        {
            var clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            var statementid = 0;
            try
            {
                cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("RBS Credit Card");
                var clsProviders = new CardProviders();
                cCardProvider provider = clsProviders.getProviderByName("RBS Credit Card");
                var clsCards = new CorporateCards(GlobalTestVariables.AccountId);
                cCorporateCard corporatecard = clsCards.GetCorporateCardById(provider.cardproviderid);
                string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
                byte[] fileData;
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "RBS Monthly XLS.xls");
            
                var statement = new cCardStatement(GlobalTestVariables.AccountId, corporatecard.cardprovider.cardproviderid, 0, "Unit Test Import", null, DateTime.Now.ToUniversalTime(), GlobalTestVariables.EmployeeId, null, null);
                cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);
                statementid = clsCardStatements.addStatement(statement);

                //Add the card transactions
                clsCardStatements.ImportCardTransactions(temp, statementid, statement.Corporatecard.cardprovider, fileData, import);
                var resultDataset = clsCardStatements.getTransactionGrid(statementid, 0, 0);

                Assert.IsNotNull(resultDataset);
                Assert.IsTrue(resultDataset.Tables.Count == 1);
                Assert.IsTrue(resultDataset.Tables[0].Rows[1]["transaction_date"].ToString() != string.Empty);

                Assert.IsNotNull(import);
                Assert.IsTrue(import.filecontents.Count > 0);
            }
            finally 
            {
                clsCardStatements.deleteStatement(statementid);
            }
        }


        /// <summary>
        ///A test for GetCardRecordTypeData with a null template
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataWithNullTemplate()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("AMEX Monthly Text");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Amex Fixed Width.txt");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Amex Fixed Width.txt");
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(null, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNull(import);
        }

        /// <summary>
        ///A test for GetCardRecordTypeData with an invalid file
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_GetCardRecordTypeDataWithInvalidFile()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Invalid File Data - Barclaycard.txt");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Invalid File Data - Barclaycard.txt");
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);

            Assert.IsNull(import);
        }

        /// <summary>
        ///A test for GetVCF4CompanyRecords
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_GetVCF4CompanyRecords()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard VCF4");
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "VCF4.txt");
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(sLocationForCardTestFiles + "VCF4.txt");
            }

            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardCompany], fileData);
            SortedList<string, string> lstComps = clsCardStatements.GetVCF4CompanyRecords(import, temp.RecordTypes[CardRecordType.CardCompany]);
            Assert.IsTrue(lstComps.Count > 0);
        }

        /// <summary>
        ///A test for ImportCardTransactions
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_addStatement()
        {
            cCardStatements clsCardStatements = null;
            int statementID = 0;
            try
            {
                cCardStatement expected = cCardStatementObject.GetBarclaycardCardStatement();
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_addStatementWithNullStatementDate()
        {
            cCardStatements clsCardStatements = null;
            int statementID = 0;
            try
            {
                cCardStatement expected = cCardStatementObject.GetBarclaycardCardStatementWithNullStatementDate();
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_addStatementAsDelegate()
        {
            HelperMethods.SetTestDelegateID();
            cCardStatements clsCardStatements = null;
            int statementID = 0;
            try
            {
                cCardStatement expected = cCardStatementObject.GetBarclaycardCardStatement();
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        ///A test for updateStatement
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_updateStatement()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatement expected = new cCardStatement(GlobalTestVariables.AccountId, statement.CorporateCardId, statement.statementid, "UpdatedName", new DateTime(2011, 01, 31), statement.createdon, statement.createdby, null, null);
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
                int statementID = clsCardStatements.updateStatement(expected);
                Assert.AreEqual(expected.statementid, statementID);
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_updateStatementWithNullStatementDate()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatement expected = new cCardStatement(GlobalTestVariables.AccountId, statement.CorporateCardId, statement.statementid, "UpdatedName", null, statement.createdon, statement.createdby, null, null);
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
                int statementID = clsCardStatements.updateStatement(expected);
                Assert.AreEqual(expected.statementid, statementID);
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_updateStatementAsADelegate()
        {
            HelperMethods.SetTestDelegateID();
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatement expected = new cCardStatement(GlobalTestVariables.AccountId, statement.CorporateCardId, statement.statementid, "UpdatedName", new DateTime(2011, 01, 31), statement.createdon, statement.createdby, null, null);

                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
                }
            }
        }

        /// <summary>
        ///A test for deleteStatement
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_deleteStatement()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_deleteStatementInvalidID()
        {
            cCardStatements clsCardStatements = null;
            
            clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            Assert.IsTrue(clsCardStatements.deleteStatement(-1));
        }

        /// <summary>
        ///A test for deleteStatement as a delegate
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_deleteStatementAsDelegate()
        {
            HelperMethods.SetTestDelegateID();
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        ///A test for allocateCard and getEmployeeTransactions. Both tested here as thay are both required to test this functionality
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_allocateCardAndGetEmployeeTransactions()
        {
            cCardStatements clsCardStatements = null; 
            cCardStatement statement = null;
            string cardnumber = "";
            cEmployeeCorporateCards empCards = new cEmployeeCorporateCards(GlobalTestVariables.AccountId);

            try
            
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
                cardnumber = cCardStatementObject.GetCardNumber(statement.statementid);
                clsCardStatements.allocateCard(statement.statementid, GlobalTestVariables.EmployeeId, cardnumber);
                //DataSet ds = clsCardStatements.getEmployeeTransactions(statement.statementid, GlobalTestVariables.EmployeeId);
               
                //Assert.IsTrue(ds.Tables[0].Rows.Count > 0);

            }
            finally
            {

                if (statement != null)
                {
                    try
                    {
                        const string sql = "select corporatecardid from employee_corporate_cards where cardnumber = @cardnumber";

                        DBConnection db = new DBConnection(cAccounts.getConnectionString(GlobalTestVariables.AccountId));
                        db.sqlexecute.Parameters.AddWithValue("@cardnumber", cardnumber);
                        int cardID = db.getcount(sql);
                        empCards.DeleteCorporateCard(cardID);
                    }
                    catch (Exception ex)
                    {

                    }
                    cCardStatementObject.DeleteCardStatement(statement.statementid);
                }
            }
        }

        /// <summary>
        ///A test for getInvalidCountries by importing a file that has invalid countries on
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_getInvalidCountries()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");

            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());

                cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("BarclayCard");
                //Need to import data to get some invalid countries back
                byte[] fileData;
                try
                {
                    fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Invalid Currencies and Countries - Barclaycard.txt");
                }
                catch
                {
                    // try D drive
                    fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Invalid Currencies and Countries - Barclaycard.txt");
                }

                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
                cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);     
                clsCardStatements.ImportCardTransactions(temp, statement.statementid, statement.Corporatecard.cardprovider, fileData, import);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_getInvalidCurrencies()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("BarclayCard");
                string sLocationForCardTestFiles = GlobalTestVariables.GetWebConfigStringValue("LocationForCardTestFiles");

                //Need to import data to get some invalid currencies back
                byte[] fileData;
                try
                {
                    fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Invalid Currencies and Countries - Barclaycard.txt");
                }
                catch
                {
                    // try D drive
                    fileData = File.ReadAllBytes(sLocationForCardTestFiles + "Invalid Currencies and Countries - Barclaycard.txt");
                }
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
                cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);
                clsCardStatements.ImportCardTransactions(temp, statement.statementid, statement.Corporatecard.cardprovider, fileData, import);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_getStatementById()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_getStatementByIdWithInvalidID()
        {
            cCardStatements clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
            
            cCardStatement actual = clsCardStatements.getStatementById(0);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for getTransactionById
        ///</summary>
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_getTransactionById()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");
                int transactionID = cCardStatementObject.GetTransactionID(statement.statementid);
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
        [TestCategory("Spend Management"), TestCategory("Card Statements"), TestMethod()]
        public void cCardStatements_getTransactionByIdInvalidID()
        {
            cCardStatements clsCardStatements = null;
            cCardStatement statement = null;
            try
            {
                statement = cCardStatementObject.CreateCardStatement(cCardStatementObject.GetBarclaycardCardStatement());
                cCardStatementObject.ImportCardTransactions(statement, "Barclaycard");
                clsCardStatements = new cCardStatements(GlobalTestVariables.AccountId);
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
