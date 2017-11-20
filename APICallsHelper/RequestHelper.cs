namespace APICallsHelper
{
    using System.Configuration;
    using System.Net;

    /// <summary>
    /// Class to help make requests to API.
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// Method to create an object of type <see cref="HttpWebRequest"/> to make calls to the specific API, whose url is passed in the parameter.
        /// </summary>
        /// <param name="action">The URL to be called (without the domain).</param>
        /// <returns>object of type <see cref="HttpWebRequest"/> which has implementation to get the response from the API </returns>
        public HttpWebRequest GetHttpWebRequest(string action)
        {
            // configure URI
            var useHttps = ConfigurationManager.AppSettings["https"] == "1";
            var domain = ConfigurationManager.AppSettings["domain"];
            var url = (useHttps ? "https://" : "http://")+ domain + "/";
            return (HttpWebRequest)WebRequest.Create(url + action);
        }
    }
}
