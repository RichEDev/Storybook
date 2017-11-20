namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Models.Common;

    public class DvlaResponse : ApiResponse
    {
        /// <summary>
        /// flag telling whether sending email was successful or not
        /// </summary>
        public bool isSendingSuccessful { get; set; }
        /// <summary>
        /// Response message to record success/failure of consent revoke
        /// </summary>
        public string ResponseMessage { get; set; }
    }
}