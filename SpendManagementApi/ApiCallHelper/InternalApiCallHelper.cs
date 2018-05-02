namespace SpendManagementApi.ApiCallHelper
{
    using System.Configuration;

    using RestSharp;

    /// <summary>
    /// Defines a <see cref="InternalApiCallHelper"/> and all it's members
    /// </summary>
    public static class InternalApiCallHelper
    {
        /// <summary>
        /// A base request for calling an api
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="request">The request to be made</param>
        /// <returns>The response from the APi</returns>
        public static IRestResponse<T> BaseRequest<T>(RestRequest request)
            where T : new()
        {
            var client = new RestClient(BaseAddress);
            IRestResponse<T> response;

            response = client.Execute<T>(request);

            return response;
        }

        /// <summary>
        /// The base address of the request
        /// </summary>
        private static string BaseAddress => ConfigurationManager.AppSettings["InternalApiUrl"];
    }
}