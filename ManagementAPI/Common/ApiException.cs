namespace ManagementAPI.Common
{
    using System.Web;

    /// <summary>
    /// ApiException is different to ApiError, in that the ApiError indicates issues within the API and not http.
    /// Api Exception is a way of adding more detail and convenience to HttpException.
    /// </summary>
    public class ApiException : HttpException
    {
        /// <summary>
        /// Gets the code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public ApiException(string code, string message)
            : base(message)
        {
            this.Code = code;
        }

    }
}