namespace Sso
{
    using SpendManagementLibrary;

    /// <summary>
    ///     The SAML response.
    /// </summary>
    public class SamlResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SamlResponse" /> class.
        /// </summary>
        internal SamlResponse()
        {
            this.AttributeStatement = new AttributeStatement();
        }

        /// <summary>
        ///     Gets or sets the statement.
        /// </summary>
        public AttributeStatement AttributeStatement { get; set; }

        /// <summary>
        ///     Gets or sets the destination.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The Account ID of this response
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The single sign-on configuration
        /// </summary>
        public SingleSignOn SsoConfig { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether valid.
        /// </summary>
        public bool Valid { get; set; }
    }
}