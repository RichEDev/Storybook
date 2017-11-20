namespace Spend_Management.shared.code.Validation.BankAccount
{
    using System;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Account;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// A class to handle the results of any Bank Account validations.
    /// </summary>
    public class BankAccountValidationResults
    {
        /// <summary>
        /// An instance of <see cref="ICurrentUser"/>
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// Creates a new instance of <see cref="BankAccountValidationResults"/>
        /// </summary>
        /// <param name="currentUser">An instance if <see cref="ICurrentUser"/></param>
        public BankAccountValidationResults(ICurrentUser currentUser)
        {
            this._currentUser = currentUser;
        }

        /// <summary>
        /// Get an instance of <see cref="IBankAccountValid"/> from the data store
        /// </summary>
        /// <param name="bankAccount">An instance of <see cref="BankAccount"/>that te validation is for</param>
        /// <returns>If available, an instance of <see cref="IBankAccountValid"/> or Null</returns>
        public IBankAccountValid Get(SpendManagementLibrary.Account.BankAccount bankAccount)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this._currentUser.Account.accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@bankAccountId", bankAccount.BankAccountId);
                connection.sqlexecute.Parameters.AddWithValue("@status", BankAccountConstants.NoService);
                using (var reader = connection.GetReader("SELECT TOP 1 [Valid], [StatusInformation] FROM [dbo].[bankAccountValidation] WHERE BankAccountId = @bankAccountId AND StatusInformation <> @status  ORDER BY ValidatedOn DESC"))
                {
                    while (reader.Read())
                    {
                        var valid = reader.GetBoolean(0);
                        var status = reader.GetString(1);
                        return new SpendManagementBankAccountValid(valid, status, string.Empty, string.Empty);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Save an instance of <see cref="IBankAccountValid"/> to the data store
        /// If the property NoService is set to True then no record will be written
        /// </summary>
        /// <param name="bankAccount">An instance of <see cref="BankAccount"/>used for Bank and employee ID's</param>
        /// <param name="bankAccountValid">The instance of <see cref="IBankAccountValid"/>to store</param>
        public void Save(SpendManagementLibrary.Account.BankAccount bankAccount, IBankAccountValid bankAccountValid)
        {
            if (bankAccountValid.NoService)
            {
                return;
            }

            using (var connection =
                new DatabaseConnection(cAccounts.getConnectionString(this._currentUser.Account.accountid)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@BankAccountId", bankAccount.BankAccountId);
                connection.sqlexecute.Parameters.AddWithValue("@Employeeid", bankAccount.EmployeeId);
                connection.sqlexecute.Parameters.AddWithValue("@ValidatedOn", DateTime.Now);
                connection.sqlexecute.Parameters.AddWithValue("@ValidatedBy", this._currentUser.EmployeeID);
                connection.sqlexecute.Parameters.AddWithValue("@Valid", bankAccountValid.IsCorrect);
                connection.sqlexecute.Parameters.AddWithValue("@StatusInformation", bankAccountValid.StatusInformation);
                connection.ExecuteSQL(
                    "INSERT INTO [dbo].[bankAccountValidation] ([BankAccountId],[EmployeeId],[ValidatedOn],[ValidatedBy],[Valid],[StatusInformation]) VALUES (@BankAccountId ,@EmployeeId ,@ValidatedOn ,@ValidatedBy ,@Valid,@StatusInformation)");
            }
        }
    }
}