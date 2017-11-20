namespace SpendManagementApi.Models.Types
{
    using System;

    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Interfaces;

    using SpendManagementLibrary;

    /// <summary>
    /// Entity represents Card Statements.
    /// </summary>
    public class CardStatement : BaseExternalType
    {
        /// <summary>
        /// Gets or sets the Statement Id for the Card Statement
        /// </summary>
        public int StatementId { get; set; }

        /// <summary>
        /// Gets or sets the Statement Name for the Card Statement
        /// </summary>
        public string StatementName { get; set; }

        /// <summary>
        /// Gets or sets the Statement Type for the Card Statement
        /// </summary>
        public ExpenseItemType StatementType { get; set; }
    }

    internal static class CardStatementConversion
    {
        /// <summary>
        /// Cast from Source to Target Type
        /// </summary>
        /// <typeparam name="TResult">Target Type</typeparam>
        /// <param name="cardStatement">Source Type</param>
        /// <returns> Instance of Target Type</returns>
        internal static TResult Cast<TResult>(this SpendManagementLibrary.CardStatement cardStatement)
            where TResult : CardStatement, new()
        {
            return cardStatement == null ? null : new TResult
            {
                StatementId = cardStatement.StatementId,
                StatementName = cardStatement.StatementName,
                StatementType = cardStatement.CreditCard ? ExpenseItemType.CreditCard : ExpenseItemType.PurchaseCard
            };
        }
    }
}