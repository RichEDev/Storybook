namespace SpendManagementApi.Models.Common
{
    using SpendManagementApi.Common.Enums;

    /// <summary>
    /// ApiRequest is the root of the request heirarchy for the API.
    /// Note that for any request, the AuthToken and AccountId should be added to the header, not the body of the request.
    /// Extend this class for new requests.
    /// </summary>
    public class ApiRequest
    {
    }

    /// <summary>
    /// Encapsulates requests that possibly require an Id.
    /// </summary>
    public class IdRequest : ApiRequest
    {
        /// <summary>
        /// An Id parameter to identify a record in a request.
        /// </summary>
        public int? Id { get; set; }
    }

    public class FindRequest : ApiRequest
    {
        public SearchOperator SearchOperator { get; set; }
    }
}