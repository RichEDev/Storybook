namespace Sso.Assert
{
    using System;

    using Sso.Assert.Authentication;

    /// <summary>
    ///     The authentication statement.
    /// </summary>
    internal class AuthenticationStatement
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationStatement"/> class.
        /// </summary>
        /// <param name="authenticationContext">
        /// The authentication context.
        /// </param>
        internal AuthenticationStatement(AuthenticationContext authenticationContext)
        {
            this.AuthenticationContext = authenticationContext;
        }
        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the authentication context.
        /// </summary>
        internal AuthenticationContext AuthenticationContext { get; set; }

        /// <summary>
        ///     Gets or sets the time at which the authentication took place. [Core,
        ///     2.7.2]
        ///     Returns:
        ///     A System.DateTime that represents the time of authentication in UTC.
        ///     Gets or sets the authentication instant.
        /// </summary>
        internal DateTime AuthenticationInstant { get; set; }

        /// <summary>
        ///     Gets or sets the index of a particular session between the principal identified
        ///     by the subject and the authenticating authority. [Core, 2.7.2]
        ///     Returns:
        ///     A string that contains the session index.
        ///     Gets or sets the session index.
        /// </summary>
        internal string SessionIndex { get; set; }

        /// <summary>
        ///     Gets or sets the time instant at which the session between the principal
        ///     identified by the subject and the SAML authority issuing this statement must
        ///     be considered ended.
        ///     Returns:
        ///     Returns a NULLABLE System.DateTime that represents the session expiration
        ///     time in UTC. A null value indicates that the attribute is not specified.///
        /// </summary>
        internal DateTime? SessionNotOnOrAfter { get; set; }

        /// <summary>
        ///     Gets or sets the DNS domain name and IP address for the system from which
        ///     the assertion subject was apparently authenticated.
        ///     Returns:
        ///     A System.IdentityModel.Tokens.SubjectLocality that specifies the DNS
        ///     domain name and IP address.
        ///     Gets or sets the subject locality.
        /// </summary>
        internal SubjectLocality SubjectLocality { get; set; }

        #endregion
    }
}