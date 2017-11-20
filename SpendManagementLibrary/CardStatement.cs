using System;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Entity represents Card Statement
    /// </summary>
    [Serializable]
    public class CardStatement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardStatement"/> class.
        /// </summary>
        /// <param name="statementId">Sets the statement id for card statement</param>
        /// <param name="statementName">Sets the statement name for card statement</param>
        /// <param name="creditCard">Is statement is type of credit card</param>
        /// <param name="purchaseCard">Is statement is type of purchase card</param>
        public CardStatement(int statementId, string statementName, bool creditCard, bool purchaseCard)
        {
            this.StatementId = statementId;
            this.StatementName = statementName;
            this.CreditCard = creditCard;
            this.PurchaseCard = purchaseCard;
        }

        /// <summary>
        /// Gets or sets Statement ID
        /// </summary>
        public int StatementId { get; set; }

        /// <summary>
        /// Gets or sets Statement Name
        /// </summary>
        public string StatementName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that Statement is type Of credit Card
        /// </summary>
        public bool CreditCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that Statement is type Of Purchase Card
        /// </summary>
        public bool PurchaseCard { get; set; }
    }
}