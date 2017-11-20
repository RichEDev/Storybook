namespace SpendManagementApi.Models.Types
{
    using Interfaces;

    /// <summary>
    /// The fields settings for OtherDetails
    /// </summary>
    public class OtherDetailsSettings : IFieldSetting
    {
        /// <summary>
        /// What to display in the field label
        /// </summary>
        public string DisplayAs { get; set; }

        /// <summary>
        /// Whether to show OtherDetails for cash items
        /// </summary>
        public bool DisplayForCash { get; set; }

        /// <summary>
        /// Whether to show OtherDetails for credit card items
        /// </summary>
        public bool DisplayForCreditCard { get; set; }

        /// <summary>
        /// Whether to show OtherDetails for purchase card items
        /// </summary>
        public bool DisplayForPurchaseCard { get; set; }

        /// <summary>
        /// Whether to show OtherDetails for individual expense items
        /// </summary>
        public bool DisplayOnIndividualItem { get; set; }

        /// <summary>
        /// Whether OtherDetails is mandatory for cash items
        /// </summary>
        public bool MandatoryForCash { get; set; }

        /// <summary>
        /// Whether OtherDetails is mandatory for credit card items
        /// </summary>
        public bool MandatoryForCreditCard { get; set; }

        /// <summary>
        // /Whether OtherDetails is mandatory for purchase card items
        /// </summary>
        public bool MandatoryForPurchaseCard { get; set; }

        /// <summary>
        /// Whether the account permits employees to add a new OtherDetails from Add/Edit Expense
        /// </summary>
        public bool CanAddNewOtherDetails { get; set; }
    }

}