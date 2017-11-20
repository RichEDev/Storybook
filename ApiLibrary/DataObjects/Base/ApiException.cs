namespace ApiLibrary.DataObjects.Base
{
    /// <summary>
    /// The API exception.
    /// </summary>
    public class ApiException : System.Exception
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string ApiMessage { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public ApiActionResult Result { get; set; }
    }
}
