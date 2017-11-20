
namespace SpendManagementLibrary.BaseClasses
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary.Account;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    /// <summary>
    /// BankAcountsBase abstract class
    /// </summary>
    public abstract class BankAccountsBase
    {
        #region protected variables

        protected int EmployeeId;
        protected string AccountConnectionString;
        protected Dictionary<int, BankAccount> ListBankAccounts;

        #endregion

       /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="employeeId">employeeid</param>
        /// <param name="accountConnectionString">connection string</param>
        protected BankAccountsBase(int employeeId, string accountConnectionString)
        {
            EmployeeId = employeeId;
            AccountConnectionString = accountConnectionString;
        }

        /// <summary>
        /// Gets list of bank accounts for employee
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="employeeId">employeeId</param>
        /// <returns>return list of bank account</returns>
        protected Dictionary<int, BankAccount> GetBankAccountsToCache(int accountId,int employeeId)
        {
            var lstToCache = new Dictionary<int, BankAccount>();

            using (var databaseConnection = new DatabaseConnection(this.AccountConnectionString))
            {
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                using (var reader = databaseConnection.GetReader("GetBankAccount", CommandType.StoredProcedure))
                {
                    if (reader != null && reader.FieldCount > 0)
                    {
                        int bankAccountIdOrd = reader.GetOrdinal("BankAccountId");
                        int accountNameOrd = reader.GetOrdinal("AccountName");
                        int accountNumberOrd = reader.GetOrdinal("AccountNumber");
                        int accountTypeOrd = reader.GetOrdinal("AccountType");
                        int sortCodeOrd = reader.GetOrdinal("SortCode");
                        int referenceOrd = reader.GetOrdinal("Reference");
                        int currencyIdOrd = reader.GetOrdinal("CurrencyId");
                        int countryIdOrd = reader.GetOrdinal("CountryId");
                        int archivedOrd = reader.GetOrdinal("archived");

                        while (reader.Read())
                        {
                            int bankAccountId = reader.GetInt32(bankAccountIdOrd);
                            string accountName = (!reader.IsDBNull(accountNameOrd)) ? reader.GetString(accountNameOrd) : string.Empty;
                            string accountNumber = (!reader.IsDBNull(accountNumberOrd)) ? reader.GetString(accountNumberOrd) : string.Empty;
                            int accountType = reader.GetInt32(accountTypeOrd);
                            string sortCode = (!reader.IsDBNull(sortCodeOrd)) ? reader.GetString(sortCodeOrd) : string.Empty;
                            string reference = (!reader.IsDBNull(referenceOrd)) ? reader.GetString(referenceOrd) : string.Empty;
                            int currencyId = reader.GetInt32(currencyIdOrd);
                            int countryId = reader.GetInt32(countryIdOrd);
                            bool archived = reader.GetBoolean(archivedOrd);
                            var account = new BankAccount(bankAccountId, employeeId, accountName, accountNumber, accountType, sortCode, reference, currencyId, countryId, archived);
                            lstToCache.Add(bankAccountId, account);
                        }
                        reader.Close();
                    }
                }
            }
            return lstToCache;
        }

        /// <summary>
        /// Create drop down for filling bank account types
        /// </summary>
        /// <param name="dropDownList">dropdownlist</param>
        public static void AddAccountTypesToDropDownList(ref DropDownList dropDownList)
        {
            foreach (AccountTypes.AccountType value in Enum.GetValues(typeof(AccountTypes.AccountType)))
            {
                string text = Enum.GetName(typeof(AccountTypes.AccountType), value);

                switch (value)
                {
                    case AccountTypes.AccountType.None:
                        text = string.Format("[{0}]", text);
                        break;
                    default:
                        text = Utilities.StringManipulation.Spacing.AddSpaceBeforeCapitals(text);
                        break;
                }

                dropDownList.Items.Add(new ListItem(text, ((int)value).ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}
