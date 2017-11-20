namespace SpendManagementApi.Models.Responses.InformationMessage
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types.InformationMessage;

    /// <summary>
    /// The information messages response.
    /// </summary>
    public class MobileInformationMessagesResponse : GetApiResponse<MobileInformationMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MobileInformationMessagesResponse"/> class.
        /// </summary>
        public MobileInformationMessagesResponse()
        {
            this.List = new List<MobileInformationMessage>();
        }
    }
}