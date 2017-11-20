namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// An interface representing the common properties that are present in ALL Field Settings.
    /// </summary>
    public interface IFieldSetting
    {
        /// <summary>
        /// What to display in the field's label
        /// </summary>
        string DisplayAs { get; set; }

        /// <summary>
        /// Whether to show field for cash items
        /// </summary>
        bool DisplayForCash { get; set; }

        /// <summary>
        /// Whether to show field for credit card items
        /// </summary>
        bool DisplayForCreditCard { get; set; }

        /// <summary>
        /// Whether to show field for purchase card items
        /// </summary>
        bool DisplayForPurchaseCard { get; set; }

        /// <summary>
        /// Whether to show field for individual expense items
        /// </summary>
        bool DisplayOnIndividualItem { get; set; }

        /// <summary>
        /// Field is mandatory for cash items
        /// </summary>
        bool MandatoryForCash { get; set; }

        /// <summary>
        /// Field is mandatory for credit card items
        /// </summary>
        bool MandatoryForCreditCard { get; set; }

        /// <summary>
        /// Field is mandatory for purchase card items
        /// </summary>
        bool MandatoryForPurchaseCard { get; set; }
    }
}
