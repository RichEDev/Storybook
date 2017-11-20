namespace SpendManagementLibrary.Expedite
{
    using SpendManagementLibrary.Enumerators.Expedite;

    public class ValidationResult

    {
        /// <summary>
        /// Gets or sets the validation result status.
        /// </summary>
        public ExpenseValidationResultStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the validation type.
        /// </summary>
        public ExpediteValidationType Type { get; set; }

        /// <summary>
        /// Gets or sets the type icon url.
        /// </summary>
        public string TypeIconUrl { get; set; }

        /// <summary>
        /// Gets or sets the type icon tooltip.
        /// </summary>
        public string TypeIconTooltip { get; set; }

        /// <summary>
        /// Gets or sets the status icon url.
        /// </summary>
        public string StatusIconUrl { get; set; }

        /// <summary>
        /// Gets or sets the status icon tooltip.
        /// </summary>
        public string StatusIconTooltip { get; set; }

        /// <summary>
        /// Gets or sets the friendly validation message.
        /// </summary>
        public string FriendlyMessage { get; set; }

        /// <summary>
        /// Gets or sets the validation comments.
        /// </summary>
        public string Comments { get; set; }
    }
}
