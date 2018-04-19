namespace BusinessLogic.GeneralOptions.Expedite
{
    /// <summary>
    /// Defines a <see cref="IExpediteOptions"/> and it's members
    /// </summary>
    public interface IExpediteOptions
    {
        /// <summary>
        /// Gets or sets the allow view fund details
        /// </summary>
        bool AllowViewFundDetails { get; set; }

        /// <summary>
        /// Gets or sets the allow receipt total to pass validation
        /// </summary>
        bool AllowReceiptTotalToPassValidation { get; set; }
    }
}
