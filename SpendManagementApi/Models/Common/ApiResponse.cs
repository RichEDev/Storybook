namespace SpendManagementApi.Models.Common
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Types;

    /// <summary>
    /// ApiResponse is the root of the response heirarchy for the API
    /// The properties here represent the result of the operation.
    /// Extend this class for new responses, where extra data is required.
    /// We might consider sending the standard API Response back where there is no additional data being returned eg for put and post operations
    /// </summary>
    public class ApiResponse : IApiResponse
    {
        /// <summary>
        /// The error will contain a status code and a list of errors
        /// </summary>
        public ApiResponseInformation ResponseInformation { get; set; }

    }

    public class ApiResponse<T> : ApiResponse, IResponse<T>
    {
        public T Item { get; set; }
    }

    public interface IApiResponse
    {
        ApiResponseInformation ResponseInformation { get; set; }
    }

    public class GetApiResponse<T> : ApiResponse, IGetResponse<T>
        where T:BaseExternalType
    {

        public List<T> List { get; set; }
    }

    public interface IGetResponse<T> : IApiResponse 
        where T : BaseExternalType
    {
        List<T> List { get; set; }
    }

    public interface IResponse<T> : IApiResponse
    {
        T Item { get; set; }
    }
    
}