namespace SpendManagementLibrary.MobileInformationMessage
{
    /// <summary>
    /// The mobile information message.
    /// </summary>
    public class MobileInformationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileInformationMessage"/> class.
        /// </summary>
        /// <param name="informationId">
        /// The information id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public MobileInformationMessage(int informationId, string title, string message)
        {
            this.InformationId = informationId;
            this.Title = title;
            this.Message = message;
        } 

        /// <summary>
        /// Gets or sets the information id.
        /// </summary>
        public int InformationId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
    }
}
