namespace Spend_Management.shared.code.Validation.BankAccount
{
    using System;

    /// <summary>
    /// An exception raised when validating bank accounts.
    /// </summary>
    public class BankAccountValidationException:Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="BankAccountValidationException"/>
        /// </summary>
        /// <param name="message">The message to display when the exception is raised.</param>
        public BankAccountValidationException(string message) : base(message)
        {
        }
    }
}