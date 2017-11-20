using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using SpendManagementUnitTests.Global_Objects;
using Spend_Management;
using System.Configuration;
using System.IO;

namespace SpendManagementUnitTests
{
    public class cCardStatementObject
    {
        /// <summary>
        /// Get a template card statement object
        /// </summary>
        /// <returns></returns>
        public static cCardStatement GetBarclaycardCardStatement()
        {
            cCardStatement statement = new cCardStatement(0, "TestStatement" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), new DateTime(2011, 01 , 01), cCorporateCardObject.CreateCorporateCard("Barclaycard"), DateTime.UtcNow, 0, null, null);
            return statement;
        }

        /// <summary>
        /// Get a template card statement object
        /// </summary>
        /// <returns></returns>
        public static cCardStatement GetBarclaycardVCF4CardStatement()
        {
            cCardStatement statement = new cCardStatement(0, "TestStatement" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), new DateTime(2011, 01, 01), cCorporateCardObject.CreateCorporateCard("Barclaycard VCF4"), DateTime.UtcNow, 0, null, null);
            return statement;
        }

        /// <summary>
        /// Get a template card statement object with null statement date
        /// </summary>
        /// <returns></returns>
        public static cCardStatement GetBarclaycardCardStatementWithNullStatementDate()
        {
            cCardStatement statement = new cCardStatement(0, "TestStatement" + DateTime.UtcNow.ToString() + DateTime.UtcNow.Ticks.ToString(), null, cCorporateCardObject.CreateCorporateCard("Barclaycard"), DateTime.UtcNow, 0, null, null);
            return statement;
        }

        /// <summary>
        /// Create and save the card statement to the database
        /// </summary>
        /// <returns></returns>
        public static cCardStatement CreateCardStatement(cCardStatement tmpStatement)
        {
            cCardStatements clsStatements = new cCardStatements(cGlobalVariables.AccountID);

            int tempID = clsStatements.addStatement(tmpStatement);
            cCardStatement statement = clsStatements.getStatementById(tempID);
            return statement;
        }

        /// <summary>
        /// Delete the card statement from the database
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteCardStatement(int ID)
        {
            cCardStatements clsStatements = new cCardStatements(cGlobalVariables.AccountID);
            clsStatements.deleteStatement(ID);
            cCorporateCardObject.DeleteCorporateCard(cGlobalVariables.CorporateCardID);
        }

        /// <summary>
        /// Method to check the success of imported transactions making sure they are added to the card transactions table and the additional table
        /// </summary>
        /// <param name="statementID"></param>
        /// <returns></returns>
        public static CardImportSuccess GetStatementTransactionImportSuccess(int statementID, string provider)
        {
            CardImportSuccess impSuccess = CardImportSuccess.Success;
            int cardTransactionCount = 0;
            int cardTransactionAdditionalCount = 0;
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "SELECT count(transactionid) from card_transactions WHERE statementid = @statementID";
            db.sqlexecute.Parameters.AddWithValue("@statementID", statementID);

            cardTransactionCount = db.getcount(strsql);

            strsql = string.Empty;

            switch (provider)
            {
                case "Allstar Fuel Card":
                    strsql = "SELECT count(ctAdditional.transactionid) from card_transactions_allstar ctAdditional";
                    break;
                case "Barclaycard":
                    strsql = "SELECT count(ctAdditional.transactionid) from card_transactions_barclaycard ctAdditional";
                    break;
                case "Barclaycard VCF4":
                    strsql = "SELECT count(ctAdditional.transactionid) from card_transactions_vcf4 ctAdditional"; 
                    break;
                default:
                    break;
            }

            if (strsql != string.Empty)
            {
                strsql += " inner join card_transactions ct on ct.transactionid = ctAdditional.transactionid where ct.statementid = @statementID";

                cardTransactionAdditionalCount = db.getcount(strsql);

                if (cardTransactionCount == 0 || cardTransactionAdditionalCount == 0)
                {
                    impSuccess = CardImportSuccess.Fail;
                }
                else if (cardTransactionCount != cardTransactionAdditionalCount)
                {
                    impSuccess = CardImportSuccess.TransactionCountsDontMatch;
                }
            }
            else
            {
                impSuccess = CardImportSuccess.Fail;
            }

            db.sqlexecute.Parameters.Clear();
            return impSuccess;
        }

        public static void ImportCardTransactions(cCardStatement statement, string provider)
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate(provider); ;
            string transactionDataFileName = string.Empty;

            switch (provider)
            {
                case "Allstar Fuel Card":
                    transactionDataFileName = "Allstar Exclude Rows.txt";
                    break;
                case "Barclaycard":
                    transactionDataFileName = "Barclaycard Flat File.txt";
                    break;
                case "Barclaycard VCF4":
                    transactionDataFileName = "VCF4.txt";
                    break;
                default:
                    break;
            }

            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "C").Replace("<BRANCH_NAME>", "xmas2010") + transactionDataFileName));
            }
            catch
            {
                // try D drive
                fileData = File.ReadAllBytes(System.IO.Path.Combine(cGlobalVariables.CreditCardTestFilesPath.Replace("<DRIVE_LETTER>", "D").Replace("<BRANCH_NAME>", "Main") + transactionDataFileName));
            }
            cImport import = clsCardStatements.GetCardRecordTypeData(temp, temp.RecordTypes[CardRecordType.CardTransaction], fileData);
            clsCardStatements.ImportCardTransactions(temp, statement.statementid, statement.corporatecard.cardprovider, fileData, import);
        }

        /// <summary>
        /// Add the card compaies to tbe database
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="provider"></param>
        public static void ImportVCF4CardCompanies(cCardStatement statement)
        {
            cCardStatements clsCardStatements = new cCardStatements(cGlobalVariables.AccountID);
            cCardTemplate temp = cCardTemplateObject.GetProviderTemplate("Barclaycard VCF4"); ;

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

            SortedList<string, string> lstCompanies = clsCardStatements.GetVCF4CompanyRecords(import, temp.RecordTypes[CardRecordType.CardCompany]);

            cCardCompanies clsCardComps = new cCardCompanies(cGlobalVariables.AccountID);

            foreach (KeyValuePair<string, string> kvp in lstCompanies)
            {
                clsCardComps.SaveCardCompany(new cCardCompany(0, kvp.Value, kvp.Key, true, null, null, null, null));
            }
        }

        /// <summary>
        /// Delete any card companies from the database
        /// </summary>
        public static void DeleteVCF4CardCompanies()
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            db.ExecuteSQL("DELETE FROM cardCompanies");
        }

        /// <summary>
        /// Get a card number from a transaction for an imported statement
        /// </summary>
        /// <param name="statementID"></param>
        /// <returns></returns>
        public static string GetCardNumber(int statementID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "SELECT TOP 1 card_number from card_transactions WHERE statementid = @statementID";
            db.sqlexecute.Parameters.AddWithValue("@statementID", statementID);

            string cardNumber = db.getStringValue(strsql);

            db.sqlexecute.Parameters.Clear();

            return cardNumber;
        }

        /// <summary>
        /// Get a Transaction ID from a transaction for an imported statement
        /// </summary>
        /// <param name="statementID"></param>
        /// <returns></returns>
        public static int GetTransactionID(int statementID)
        {
            DBConnection db = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "SELECT TOP 1 transactionid from card_transactions WHERE statementid = @statementID";
            db.sqlexecute.Parameters.AddWithValue("@statementID", statementID);

            int transactionID = db.getIntSum(strsql);

            db.sqlexecute.Parameters.Clear();

            return transactionID;
        }

        /// <summary>
        /// List of omitted properties for the compare assert
        /// </summary>
        public static List<string> lstOmittedProperties
        {
            get
            {
                List<string> OmittedProperties = new List<string>();

                OmittedProperties.Add("statementid");
                OmittedProperties.Add("createdon");
                OmittedProperties.Add("createdby");
                OmittedProperties.Add("modifiedon");
                OmittedProperties.Add("modifiedby");

                return OmittedProperties;
            }
        }


    }

    /// <summary>
    /// Enumerable type to determine if the imported transactions are adding and to ensure if there is an additional table that the amount of transactions is the same as the card transactions table
    /// </summary>
    public enum CardImportSuccess
    {
        /// <summary>
        /// Successfully imported and the counts are the same
        /// </summary>
        Success = 0,

        /// <summary>
        /// One or both counts are 0 meaning there was a failure in the import 
        /// </summary>
        Fail = 1,

        /// <summary>
        /// Both counts have a value but they do not match
        /// </summary>
        TransactionCountsDontMatch =2
    }
}
