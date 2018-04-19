namespace BusinessLogic.GeneralOptions.Validate
{
    /// <summary>
    /// Defines a <see cref="ValidateOptions"/> and it's members
    /// </summary>
    public class ValidateOptions : IValidateOptions
    {
        /// <summary>
        /// Gets or set the notify when envelope not received days
        /// </summary>
        public int NotifyWhenEnvelopeNotReceivedDays { get; set; }
    }
}
