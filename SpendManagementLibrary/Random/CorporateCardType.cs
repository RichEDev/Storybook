using System;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Defines the sub type of a Coroporate Card.
    /// </summary>
    [Serializable()]
    public enum CorporateCardType
    {
        /// <summary>
        /// The Credit Card Type.
        /// </summary>
        CreditCard = 1,

        /// <summary>
        /// The Purchase Card Type.
        /// </summary>
        PurchaseCard = 2
    }
}
