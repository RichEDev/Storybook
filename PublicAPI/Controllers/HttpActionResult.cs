namespace PublicAPI.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Asynchronously creates an System.Net.Http.HttpResponseMessage with a specified <see cref="HttpStatusCode"/> and a string message.
    /// </summary>
    public class HttpActionResult : IHttpActionResult
    {
        // TODO - work in progress
        private readonly string _message;
        private readonly HttpStatusCode _statusCode;

        public HttpActionResult(HttpStatusCode statusCode, string message)
        {
            this._statusCode = statusCode;
            this._message = message;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(this._statusCode)
            {
                Content = new StringContent(this._message)
            };

            return Task.FromResult(response);
        }
    }
}