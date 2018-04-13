namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Account;
    using SpendManagementLibrary.BaseClasses;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// BankAccounts holds basic functionality for save and retrieve
    /// </summary>
    public class BankAccounts : BankAccountsBase
    {

        private readonly Utilities.DistributedCaching.Cache cache = new Utilities.DistributedCaching.Cache();
        protected int AccountId;
        //private Cache WebCache = HttpRuntime.Cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankAccounts"/> class.
        /// </summary>
        /// <param name="accountId">account dd</param>
        /// <param name="employeeId">employee id</param>
        public BankAccounts(int accountId, int employeeId)
            : base(
                employeeId, cAccounts.getConnectionString(accountId))
        {
            AccountId = accountId;
            this.GetBankAccounts(accountId,employeeId);
        }

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="BankAccounts"/> class.
        /// </summary>
        /// <param name="accountId">The accountId</param>
        public BankAccounts(int accountId)
        {
            this.AccountId = accountId;
        }

        /// <summary>
        /// Gets all the <see cref="BankAccount"/>s in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="BankAccount"/>s.
        /// </returns>
        public List<BankAccount> GetAll()
        {
            var bankAccounts = new List<BankAccount>();

            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                using (var reader = connection.GetReader("GetBankAccounts", CommandType.StoredProcedure))
                {
                    connection.sqlexecute.Parameters.Clear();
                    bankAccounts = ProcessReader(reader);
                }
            }

            return bankAccounts;
        }

        /// <summary>
        /// Gets a bank account by its Id
        /// </summary>
        /// <param name="bankAccountId">bank account id</param>
        /// <returns>details of bank account for particular bankaccount id</returns>
        public BankAccount GetBankAccountById(int bankAccountId)
        {
            BankAccount account = (from x in ListBankAccounts.Values
                where x.BankAccountId == bankAccountId
                select x).FirstOrDefault();

            return account;
        }

        /// <summary>
        /// Gets a bank account by it's Id.
        /// </summary>
        /// <param name="id">Bank account Id.</param>
        /// <returns>A <see cref="BankAccount"/>.</returns>
        public BankAccount GetById(int id)
        {
            BankAccount bankAccount;

            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.AddWithValue("@id", id);

                using (var reader = connection.GetReader("GetBankAccountById", CommandType.StoredProcedure))
                {
                    connection.sqlexecute.Parameters.Clear();

                    bankAccount = ProcessReader(reader).FirstOrDefault();
                }
            }

            return bankAccount;
        }

        /// <summary>
        /// Create drop down for billing bank details
        /// </summary>
        /// <param name="employeeId">employeeId</param>
        /// <returns>Returns list of ListItem having bankaccount name and bankaccount id </returns>
        public List<ListItem> CreateDropDown(int employeeId)
        {
            List<BankAccount> accountList = (from x in ListBankAccounts.Values
                                             where x.EmployeeId == employeeId && x.Archived==false
                                             orderby x.AccountName 
                                             select x).ToList();
            var items = new List<ListItem> {new ListItem("[None]", "0")};

            for (int index = 0; index < accountList.Count; index++)
            {
                var item = accountList[index];
                items.Add(new ListItem(item.AccountName, Convert.ToString(item.BankAccountId)));
            }

            return items;
        }

        /// <summary>
        /// Returns list of bank accounts by employee
        /// </summary>
        /// <param name="employeeId">employeeId</param>
        /// <returns>return list of bank account</returns>
        public List<BankAccount> GetAccountAsListByEmployeeId(int employeeId)
        {
            List<BankAccount> accountList = (from x in ListBankAccounts.Values
                where x.EmployeeId == employeeId
                select x).ToList();
            return accountList;
        }

        /// <summary>
        /// Returns list of bank accounts by employee
        /// </summary>
        /// <param name="employeeId">employeeId</param>
        /// <returns>return list of bank account</returns>
        public List<BankAccount> GetActiveAccountByEmployeeId(int employeeId)
        {
            List<BankAccount> accountList = (from x in ListBankAccounts.Values
                                             where x.EmployeeId == employeeId && x.Archived==false
                                             select x).ToList();
            return accountList;
        }

        /// <summary>
        /// Gets a list of bank accounts for a particular employee
        /// </summary>
        /// <param name="employeeId">Employee Id to retrieve devices for</param>
        /// <returns>Dictionary collection of Bank Accounts class objects</returns>
        public Dictionary<int, BankAccount> GetBankAccountByEmployeeId(int employeeId)
        {
            Dictionary<int, BankAccount> accounts = (from x in ListBankAccounts.Values
                                                     where x.EmployeeId == employeeId
                                                     select x).ToDictionary(x => x.BankAccountId);
            return accounts;
        }

        /// <summary>
        /// Save changes to an existing or new bank account
        /// </summary>
        /// <param name="reqBankAcccount">Bank Account class object</param>
        /// <param name="reqEmployeeId">Employee performing the save</param>
        /// <returns></returns>
        public int SaveBankAccount(BankAccount reqBankAcccount, int reqEmployeeId)
        {
// ReSharper disable once CSharpWarnings::CS0618
            var  reqDbCon = new DBConnection(AccountConnectionString);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@BankAccountId", reqBankAcccount.BankAccountId );
            reqDbCon.sqlexecute.Parameters.AddWithValue("@EmployeeId", reqBankAcccount.EmployeeId );
            reqDbCon.sqlexecute.Parameters.AddWithValue("@AccountName", reqBankAcccount.AccountName );
            reqDbCon.sqlexecute.Parameters.AddWithValue("@AccountNumber", reqBankAcccount.AccountNumber );
            reqDbCon.sqlexecute.Parameters.AddWithValue("@AccountType", reqBankAcccount.AccountType );
            reqDbCon.sqlexecute.Parameters.AddWithValue("@SortCode", reqBankAcccount.SortCode );
            reqDbCon.sqlexecute.Parameters.AddWithValue("@Reference", reqBankAcccount.Reference);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@CurrencyId", reqBankAcccount.CurrencyId);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@requestorEmployeeId", reqEmployeeId);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@CountryId", reqBankAcccount.CountryId);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@SwiftCode", reqBankAcccount.SwiftCode);
            reqDbCon.sqlexecute.Parameters.AddWithValue("@Iban", reqBankAcccount.Iban);

            reqDbCon.sqlexecute.Parameters.Add("@newBankAccountID", SqlDbType.Int);
            reqDbCon.sqlexecute.Parameters["@newBankAccountID"].Direction = ParameterDirection.ReturnValue;
            reqDbCon.ExecuteProc("dbo.saveBankAccount");

            var bankAccountId = (int)reqDbCon.sqlexecute.Parameters["@newBankAccountID"].Value;

            reqDbCon.sqlexecute.Parameters.Clear();

            return bankAccountId;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the bank accounts for particular employee
        /// </summary>
        /// <param name="employeeId">employeeid</param>
        private void GetBankAccounts(int accountId,int employeeId)
        {
                ListBankAccounts = GetBankAccountsToCache(accountId,employeeId);
        }
        
    /// <summary>
    /// Replace currencyid with currency in gridview column
    /// </summary>
    /// <param name="grid">The grid</param>
    /// <param name="accountId">accountId</param>
    /// <param name="subAccountId">subAccountId</param>
    public void AddCurrency(ref cGridNew grid, int accountId,int subAccountId)
        {
            var clscurrencies = new cCurrencies(accountId, subAccountId);
            List<ListItem> lstCurrency = clscurrencies.CreateDropDown();

            foreach (ListItem lstItem in lstCurrency)
            {
                ((cFieldColumn)grid.getColumnByName("CurrencyId")).addValueListItem(Convert.ToInt32(lstItem.Value), lstItem.Text);
            }

            SetHeader(ref grid);
        }

    /// <summary>
    /// Sets header text of grid
    /// </summary>
    /// <param name="grid">grid</param>
    public void SetHeader(ref cGridNew grid)
    {
        grid.getColumnByName("AccountName").HeaderText = "Account Name";
        grid.getColumnByName("AccountNumber").HeaderText = "Account Number";
        grid.getColumnByName("AccountType").HeaderText = "Account Type";
        grid.getColumnByName("CurrencyId").HeaderText = "Currency";
        grid.getColumnByName("SortCode").HeaderText = "Sort Code";
    }

    /// <summary>
    /// Provide proper type of the account by accounttypeid in the grid
    /// </summary>
    /// <param name="grid">grid</param>
    public void AddAccountType(ref cGridNew grid)
        {
           foreach (AccountTypes.AccountType value in Enum.GetValues(typeof(AccountTypes.AccountType)))
            {
                string text = Enum.GetName(typeof(AccountTypes.AccountType), value);
                ((cFieldColumn)grid.getColumnByName("AccountType")).addValueListItem(Convert.ToInt32(value), Utilities.StringManipulation.Spacing.AddSpaceBeforeCapitals(text));
            }
        }


        /// <summary>
        /// Delete BankAccount from bank account grid
        /// </summary>
        /// <param name="bankaccountid"></param>
        /// <param name="employeeid"></param>
        /// <returns></returns>
        public int DeleteBankAccount(int bankAccountId, int employeeId)
        {
            int returnCode = 0;
            returnCode = CheckBankAccountCanArchivedOrDeleted(bankAccountId, employeeId);
            if (returnCode != -1)
            {
                var expdata = new DBConnection(cAccounts.getConnectionString(this.AccountId));
                expdata.sqlexecute.Parameters.AddWithValue("@bankAccountId", bankAccountId);
                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeId", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateId", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateId", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.Add("@returnCode", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                expdata.ExecuteProc("DeleteBankAccount");
                returnCode = (int)expdata.sqlexecute.Parameters["@returnCode"].Value;

                expdata.sqlexecute.Parameters.Clear();

                if (returnCode == 0)
                {
                    ListBankAccounts?.Remove(bankAccountId);
                }
            }
            return returnCode;
        }

        /// <summary>
        /// Archive/un-Archive the Bank account
        /// </summary>
        /// <param name="bankaccountid"></param>
        /// <param name="archive"></param>
        public int ChangeStatus(int bankAccountId, bool archive,int employeeId)
        {
            int returnCode = 0;
            returnCode = CheckBankAccountCanArchivedOrDeleted(bankAccountId, employeeId);
            if (returnCode != -1)
            {
                var expdata = new DBConnection(cAccounts.getConnectionString(this.AccountId));
                expdata.sqlexecute.Parameters.AddWithValue("@bankAccountId", bankAccountId);
                expdata.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
                CurrentUser currentUser = cMisc.GetCurrentUser();
                expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeId", currentUser.EmployeeID);
                if (currentUser.isDelegate == true)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateId", DBNull.Value);
                }
                expdata.ExecuteProc("ChangeBankAccountStatus");
                expdata.sqlexecute.Parameters.Clear();
            }
            return returnCode;
        }

        /// <summary>
        /// Check if BankAccount can be Archived or Deleted
        /// </summary>
        /// <param name="bankAccountId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private int CheckBankAccountCanArchivedOrDeleted(int bankAccountId, int employeeId)
        {
            int returnCode = 0;
            var currentUser = cMisc.GetCurrentUser();
            var accounts = new BankAccounts(currentUser.AccountID, employeeId);
            BankAccount account = accounts.GetBankAccountById(bankAccountId);
            if (accounts.GetActiveAccountByEmployeeId(employeeId).Count == 1 && currentUser.MustHaveBankAccount && !account.Archived)
            {
                returnCode = -1;
            }           
            return returnCode;
        }

        /// <summary>
        /// Proccesses a <see cref="IDataReader"/> to extract the bank account data
        /// </summary>
        /// <param name="reader">
        /// The <see cref="IDataReader"/>.
        /// </param>
        /// <returns>
        /// A list of <see cref="BankAccount"/>
        /// </returns>
        private static List<BankAccount> ProcessReader(IDataReader reader)
        {
            var bankAccounts = new List<BankAccount>();

            int bankAccountIdOrd = reader.GetOrdinal("BankAccountId");
            int accountNameOrd = reader.GetOrdinal("AccountName");
            int accountNumberOrd = reader.GetOrdinal("AccountNumber");
            int accountTypeOrd = reader.GetOrdinal("AccountType");
            int sortCodeOrd = reader.GetOrdinal("SortCode");
            int employeeIdOrd = reader.GetOrdinal("EmployeeId");
            int referenceOrd = reader.GetOrdinal("Reference");
            int currencyIdOrd = reader.GetOrdinal("CurrencyId");
            int countryIdOrd = reader.GetOrdinal("CountryId");
            int archivedOrd = reader.GetOrdinal("archived");
            int ibanOrd = reader.GetOrdinal("Iban");
            int swiftCodeOrd = reader.GetOrdinal("SwiftCode");

            while (reader.Read())
            {
                var bankAccountId = reader.GetInt32(bankAccountIdOrd);
                var accountName = (!reader.IsDBNull(accountNameOrd)) ? reader.GetString(accountNameOrd) : string.Empty;
                var accountNumber = (!reader.IsDBNull(accountNumberOrd)) ? reader.GetString(accountNumberOrd) : string.Empty;
                var accountType = reader.GetInt32(accountTypeOrd);
                var sortCode = (!reader.IsDBNull(sortCodeOrd)) ? reader.GetString(sortCodeOrd) : string.Empty;
                var reference = (!reader.IsDBNull(referenceOrd)) ? reader.GetString(referenceOrd) : string.Empty;
                var currencyId = reader.GetInt32(currencyIdOrd);
                var countryId = reader.GetInt32(countryIdOrd);
                var archived = reader.GetBoolean(archivedOrd);
                var iban = (!reader.IsDBNull(ibanOrd)) ? reader.GetString(ibanOrd) : string.Empty;
                var swiftCode = (!reader.IsDBNull(swiftCodeOrd)) ? reader.GetString(swiftCodeOrd) : string.Empty;
                var employeeId = reader.GetInt32(employeeIdOrd);

                var bankAccount = new BankAccount(
                    bankAccountId,
                    employeeId,
                    accountName,
                    accountNumber,
                    accountType,
                    sortCode,
                    reference,
                    currencyId,
                    countryId,
                    archived,
                    iban,
                    swiftCode);

                bankAccounts.Add(bankAccount);
            }

            reader.Close();

            return bankAccounts;
        }

        #endregion

    }
}