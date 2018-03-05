namespace BusinessLogic.Receipts
{
    /// <summary>
    /// Defines the enum collection of <see cref="WalletReceiptStatus"/> for the different states of a <see cref="WalletReceipt"/>
    /// </summary>
    public enum WalletReceiptStatus
    {
        /// <summary>
        /// The receipt is not to be processed
        /// </summary>
        Skip = -1,

        /// <summary>
        /// The receipt needs to be processed
        /// </summary>
        New = 0,

        /// <summary>
        /// The processing of the receipt is in progress
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// The receipt has been processed
        /// </summary>
        Done = 2
    }
}
