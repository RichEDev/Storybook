namespace SpendManagementApi.Models.Common
{
    /// <summary>
    /// Represents the result of an operation in the API.
    /// </summary>
    public enum ApiStatusCode
    {   
        Failure = 0,
        Success = 1,
        PartialSuccessWithWarnings = 2,


        InternalError = 1000,
        GenericError = 1001,
        ValidationError = 1002,

    }
}