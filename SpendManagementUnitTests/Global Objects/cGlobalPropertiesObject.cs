using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spend_Management;
using SpendManagementLibrary;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cGlobalPropertiesObject
    {
        public static void UpdateGlobalCurrency(int SubAccountID, int CurrencyID)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "UPDATE accountProperties SET stringValue = @currencyid WHERE stringKey = 'baseCurrency' and subAccountID = @subaccountid";

            expdata.sqlexecute.Parameters.AddWithValue("@currencyid", CurrencyID);
            expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", SubAccountID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Set the username format for the ESR outbound imports 
        /// </summary>
        /// <param name="format"></param>
        public static void UpdateUsernameFormat(string format)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "UPDATE accountProperties SET stringValue = @format WHERE stringKey = 'importUsernameFormat' and subAccountID = @subaccountid";

            expdata.sqlexecute.Parameters.AddWithValue("@format", format);
            expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", cGlobalVariables.SubAccountID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Set the home address format for the ESR outbound imports 
        /// </summary>
        /// <param name="format"></param>
        public static void UpdateHomeAddressFormat(string format)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(cGlobalVariables.AccountID));
            string strsql = "UPDATE accountProperties SET stringValue = @format WHERE stringKey = 'ImportHomeAddressFormat' and subAccountID = @subaccountid";

            expdata.sqlexecute.Parameters.AddWithValue("@format", format);
            expdata.sqlexecute.Parameters.AddWithValue("@subaccountid", cGlobalVariables.SubAccountID);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
    }
}
