namespace Spend_Management.shared.code.Validation.BankAccount
{
    using SpendManagementLibrary.Account;

    /// <summary>
    /// A description of a validator service.
    /// </summary>
    public interface IBankAccountValidator
    {
        /// <summary>
        /// validate a specific bank account with a validation service
        /// </summary>
        /// <param name="bankAccount">An instance of <see cref="Spend_Management.shared.code.Validation.BankAccount"/>to validate</param>
        /// <returns>An instance of <see cref="IBankAccountValid"/></returns>
        IBankAccountValid Validate(SpendManagementLibrary.Account.BankAccount bankAccount);
    }
}
