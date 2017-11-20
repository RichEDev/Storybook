namespace SpendManagementApi.Common.Mobile
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    /// <summary>
    /// Extension methods to make handling <see cref="HttpRequestMessageExtensions"/> easier.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Returns an individual HTTP Header value
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage">HttpRequestMessage</see></param>
        /// <param name="key">The key you're looking for</param>
        /// <returns>The value of the header or null if empty.</returns>
        public static string GetHeader(HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys;
            return !request.Headers.TryGetValues(key, out keys) ? null : keys.First();
        }
    }
}