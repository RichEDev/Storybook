namespace SpendManagementApi.Models.Types.InformationMessage
{
    using SpendManagementApi.Interfaces;

    /// <summary>
    /// The mobile information message class.
    /// </summary>
    public class MobileInformationMessage :BaseExternalType, IBaseClassToAPIType<SpendManagementLibrary.MobileInformationMessage.MobileInformationMessage, MobileInformationMessage>
    {
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

        /// <summary>
        /// Converts a spend management library type to a API type
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="actionContext">
        /// The action context.
        /// </param>
        /// <returns>
        /// The <see cref="MobileInformationMessage"/>.
        /// </returns>
        public MobileInformationMessage ToApiType(SpendManagementLibrary.MobileInformationMessage.MobileInformationMessage dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.InformationId = dbType.InformationId;
            this.Title = dbType.Title;
            this.Message = dbType.Message;

            return this;
        }
    }
}