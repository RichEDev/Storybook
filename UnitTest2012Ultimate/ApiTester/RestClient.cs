using System;
using System.Text;

public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace APITester
{
    using System.IO;
    using System.Net;

    static class RestClient
    {
        /// <summary>
        /// Makes the API request
        /// </summary>
        /// <param name="endpoint">The endpoint anmes</param>
        /// <param name="method">The HTTPVerb</param>
        /// <param name="postData">The data being sent to the </param>
        /// <param name="authToken">The auth token</param>
        /// <param name="parameters">any params added to the API Call</param>
        /// <returns>The deserialized json response</returns>
        public static JsonDeserializer MakeRequest(string endpoint = "", HttpVerb method = HttpVerb.GET, string postData = "", string authToken = "",string parameters = "" )
        {
            var responseValue = string.Empty;
            try
            {
                const string ContentType = "application/json; charset=utf-8";
                var request = (HttpWebRequest)WebRequest.Create(endpoint + parameters);

                request.Method = method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;
                if (!string.IsNullOrEmpty(authToken))
                {
                    request.Headers.Add("AuthToken", authToken);
                }

                if (!string.IsNullOrEmpty(postData) && (method == HttpVerb.PUT || method == HttpVerb.POST))
                {
                    request.ServicePoint.ReceiveBufferSize = 99999999;
                    request.ServicePoint.MaxIdleTime = 3000;
                    var encoding = new UTF8Encoding();
                    request.Accept = "*/*";
                    var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(postData);
                    request.ContentLength = bytes.Length;

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

             

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                responseValue = "{" + string.Format("endpoint: '{0}', method: '{1}', error: '{2}'", endpoint, method, ex.Message) + "}";
            }

     //       var dict = JsonConvert.DeserializeObject<Dictionary<string,List<AccessRoleResponse>>>(responseValue);

            return new JsonDeserializer((responseValue));
            }
        }

    }
