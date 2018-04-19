namespace BusinessLogic.GeneralOptions.Validate
{
    /// <summary>
    /// Defines a <see cref="IValidateOptions"/> and it's members
    /// </summary>
    public interface IValidateOptions
    {
        /// <summary>
        /// Gets or set the notify when envelope not received days
        /// </summary>
        int NotifyWhenEnvelopeNotReceivedDays { get; set; }
    }
}
