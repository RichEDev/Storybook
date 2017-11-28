using System;

namespace SpendManagementLibrary.Account
{
    using System.Text;

    /// <summary>
    /// A bank account user in the system
    /// </summary>
    [Serializable()]
    public class BankAccount
    {
        /// <summary>
        /// Empty constructor for Ajax - not to be used
        /// </summary>
        public BankAccount()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="BankAccount"/> class. 
        /// Constructor for Bank Accounts
        /// </summary>
        /// <param name="bankAccountId">The bank account ID (Primary Key)</param>
        /// <param name="employeeId">The employeeID of the employee who owns this bank account</param>
        /// <param name="accountName">The name given to this account by the user</param>
        /// <param name="accountNumber">The unique number given to this account by the user</param>
        /// <param name="accountType">The account type</param>
        /// <param name="sortCode">The sort code</param>
        /// <param name="reference">Reference</param>
        /// <param name="currencyId">The currency associated to this account by the user</param>
        /// <param name="countryId">The country associated to this account by the user</param>
        /// <param name="archived">Archived status of the bankaccount</param>
        /// <param name="ibanCode">The country associated to this account by the user</param>
        /// <param name="swiftCode">Archived status of the bankaccount</param>
        public BankAccount(int bankAccountId, int employeeId, string accountName, string accountNumber, int accountType, string sortCode, string reference, int currencyId, int countryId, bool archived, string ibanCode = null, string swiftCode = null)
        {
            this.BankAccountId = bankAccountId;
            this.EmployeeId = employeeId;
            this.AccountName = accountName;
            this.AccountNumber = accountNumber;
            this.AccountType = accountType;
            this.SortCode = sortCode;
            this.Reference = reference;
            this.CurrencyId = currencyId;
            this.CountryId = countryId;
            this.Archived = archived;
            this.IbanCode = ibanCode;
            this.SwiftCode = swiftCode;
        }

        #region Properties - All properties are get and set for the ajax interface

        /// <summary>
        /// The bank account ID (Primary Key)
        /// </summary>
        public int BankAccountId { get; set; }
        /// <summary>
        /// The employeeID of the employee who owns this bank account
        /// </summary>
        public int EmployeeId { get; set; }
        /// <summary>
        /// The name given to this account by the user
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// The unique number given to this account by the user
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Account type
        /// </summary>
        public int AccountType { get; set; }
        /// <summary>
        /// Sort code
        /// </summary>
        public string SortCode { get; set; }
        /// <summary>
        /// Reference value
        /// </summary>
        public string Reference { get; set; }
        /// <summary>
        /// Currency id
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Country id
        /// </summary>
        public int CountryId { get; set; }
        
        /// <summary>
        /// archived
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Iban code of a bank account
        /// </summary>
        public string IbanCode { get; set; }

        /// <summary>
        /// Swift code of a bank account
        /// </summary>
        public string SwiftCode { get; set; }

        #endregion

        public static string GetRedactedValues(string value, int length)
        {
            int valueLength = value.Length;
            var redactedValue = new StringBuilder();

            if (string.IsNullOrEmpty(value) == false)
            {
                if (valueLength >= length)
                {
                    string lastCharacter = value.Substring(valueLength - length, length);

                    for (int i = 0; i < valueLength - length; i++)
                    {
                        redactedValue.Append("*");
                    }
                    redactedValue.Append(lastCharacter);
                }
                else
                {
                    redactedValue.Append(value);
                }
            }

            return redactedValue.ToString();
        }
    }
}
