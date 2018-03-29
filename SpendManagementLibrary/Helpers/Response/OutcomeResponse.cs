namespace SpendManagementLibrary.Helpers.Response
{
    /// <summary>
    /// The OutcomeResponse class.
    /// </summary>
    public class OutcomeResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the outcome was a success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the associated controls.
        /// </summary>
        public string[] Controls { get; set; }
    }   
}