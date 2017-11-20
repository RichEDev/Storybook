namespace SpendManagementApi.Models.Types
{
    using Interfaces;

    /// <summary>
    /// The fields settings for Country
    /// </summary>
    public class CountrySettings : IFieldSetting
    {
        /// <summary>
        /// What to display in the field label
        /// </summary>
        public string DisplayAs { get; set; }

        /// <summary>
        /// Whether to show Country for cash items
        /// </summary>
        public bool DisplayForCash { get; set; }

        /// <summary>
        /// Whether to show Country for credit card items
        /// </summary>
        public bool DisplayForCreditCard { get; set; }

        /// <summary>
        /// Whether to show Country for purchase card items
        /// </summary>
        public bool DisplayForPurchaseCard { get; set; }

        /// <summary>
        /// Whether to show Country for individual expense items
        /// </summary>
        public bool DisplayOnIndividualItem { get; set; }

        /// <summary>
        /// Whether Country is mandatory for cash items
        /// </summary>
        public bool MandatoryForCash { get; set; }

        /// <summary>
        /// Whether Country is mandatory for credit card items
        /// </summary>
        public bool MandatoryForCreditCard { get; set; }

        /// <summary>
        // /Whether Country is mandatory for purchase card items
        /// </summary>
        public bool MandatoryForPurchaseCard { get; set; }

        /// <summary>
        /// Whether the account permits employees to add a new Country from Add/Edit Expense
        /// </summary>
        public bool CanAddNewCountry { get; set; }
    }

}