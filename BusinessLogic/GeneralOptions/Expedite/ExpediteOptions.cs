namespace BusinessLogic.GeneralOptions.Expedite
{
    /// <summary>
    /// Defines a <see cref="ExpediteOptions"/> and it's members
    /// </summary>
    public class ExpediteOptions : IExpediteOptions
    {
        /// <summary>
        /// Gets or sets the allow view fund details
        /// </summary>
        public bool AllowViewFundDetails { get; set; }

        /// <summary>
        /// Gets or sets the allow receipt total to pass validation
        /// </summary>
        public bool AllowReceiptTotalToPassValidation { get; set; }
    }
}
