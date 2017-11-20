namespace APICallsHelper
{
    /// <summary>
    /// Response object type of login api call
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        public string AuthToken { get; set; }
       
        /// <summary>
        /// Gets or sets the attempts remaining before the employee has their account locked.
        /// </summary>
        public int AttemptsRemaining { get; set; }

        /// <summary>
        /// Gets or sets the message returned in a HTTP response exception in the case of a failed login.
        /// </summary>
        public string Message { get; set; }

    }
}
