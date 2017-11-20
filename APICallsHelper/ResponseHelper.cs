namespace APICallsHelper
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Class to help with the operations on response which we get after making an API call.
    /// </summary>
    public class ResponseHelper
    {
        /// <summary>
        /// Get the response of API in the known object form. 
        /// </summary>
        /// <typeparam name="T"><see cref="T"/> The expected type of the API response. </typeparam>
        /// <param name="response">instance fo type <see cref="HttpWebResponse"/>, which is the response from API call. </param>
        /// <param name="streamReader">object of type <see cref="StreamReader"/> which has an initialised base stream.</param>
        /// <returns>Object of Generic type.</returns>
        public T GetResponseObject<T>(HttpWebResponse response, StreamReader streamReader)
        {
            var content = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
