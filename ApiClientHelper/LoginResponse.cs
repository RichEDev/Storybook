namespace ApiClientHelper
{
    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// The logon response.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginResponse"/> class.
        /// </summary>
        public LoginResponse()
        {
            this.LoginResult = LoginResult.InactiveAccount;
            this.Message = null;
            this.LoginResponseCode = null;
            this.AuthToken = string.Empty;
        }

        /// <summary>
        /// Gets or sets the authentication token.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Sets the login response.
        /// </summary>
        public int? LoginResponseCode
        {
            set
            {
                if (value != null)
                {
                    this.LoginResult = (LoginResult)value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the attempts remaining before the employee has their account locked.
        /// </summary>
        public int AttemptsRemaining { get; set; }

        /// <summary>
        /// Gets or sets the message returned in a HTTP response exception in the case of a failed login.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the login result.
        /// </summary>
        public LoginResult LoginResult { get; set; }
    }
}