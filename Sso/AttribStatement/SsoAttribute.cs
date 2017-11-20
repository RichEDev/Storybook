namespace Sso.AttribStatement
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The SSO attribute.
    /// </summary>
    public class SsoAttribute
    {
        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SsoAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public SsoAttribute(string name, string value)
        {
            this.Values = new Collection<string> { value };
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the attribute value XSI type.
        /// </summary>
        public string AttributeValueXsiType { get; set; }

        /// <summary>
        ///     Gets or sets a human-readable name for the attribute.
        ///     Returns:
        ///     A string that contains the friendly name for the attribute.
        ///     Gets or sets the friendly name.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the name format.
        /// </summary>
        public Uri NameFormat { get; set; }

        /// <summary>
        ///     Gets or sets the original issuer.
        /// </summary>
        public string OriginalIssuer { get; set; }

        /// <summary>
        ///     Gets the values.
        /// </summary>
        public Collection<string> Values { get; private set; }

        #endregion
    }
}