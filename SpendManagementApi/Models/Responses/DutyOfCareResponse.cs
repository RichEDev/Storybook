using SpendManagementApi.Models.Common;


namespace SpendManagementApi.Models.Responses
{
    /// <summary>
    /// Duty Of Care Email Response
    /// </summary>
    public class DutyOfCareResponse : ApiResponse
    {
            /// <summary>
            /// flag telling whether sending email was successful or not
            /// </summary>
            public bool isSendingSuccessful { get; set; }
            /// <summary>
            /// error message to be set if any exceptions arise
            /// </summary>
            public string errorMessage { get; set; }
        
    }
}
