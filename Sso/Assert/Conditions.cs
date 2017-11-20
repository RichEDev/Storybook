namespace Sso.Assert
{
    using System;
    using System.Collections.ObjectModel;

    using Sso.Assert.Restriction;

    /// <summary>
    ///     The conditions.
    /// </summary>
    internal class Conditions
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Conditions" /> class.
        /// </summary>
        public Conditions()
        {
            this.AudienceRestrictions = new Collection<AudienceRestriction>();
            this.ProxyRestriction = new ProxyRestriction();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the audience restrictions.
        ///     A collection of type System.IdentityModel.Tokens.AudienceRestriction
        ///     that specifies the audience for the assertion. If the collection is empty
        ///     no restrictions on the audience apply.
        /// </summary>
        public Collection<AudienceRestriction> AudienceRestrictions { get; private set; }

        /// <summary>
        ///     Gets or sets the earliest time instant at which the assertion is valid. [Core,
        ///     Returns:
        ///     A NULLABLE System.DateTime that contains the time instant in UTC. A null
        ///     value indicates that the attribute is not present.
        ///     Exceptions:
        ///     System.ArgumentException:
        ///     The System.IdentityModel.Tokens.Conditions.NotOnOrAfter property is
        ///     not null and an attempt to set a value that occurs on or after the time instant
        ///     specified by the System.IdentityModel.Tokens.Conditions.NotOnOrAfter
        ///     property occurs.
        /// </summary>
        public DateTime? NotBefore { get; set; }

        // Gets or sets the time instant at which the assertion has expired. 
        // Returns:
        // A NULLABLE System.DateTime that contains the time instant in UTC. A null
        // value indicates that the attribute is not present.
        // Exceptions:
        // System.ArgumentException:
        // The System.IdentityModel.Tokens.Conditions.NotBefore property is not
        // null and an attempt to set a value that occurs before the time instant specified
        // by the System.IdentityModel.Tokens.Conditions.NotBefore property occurs.
        /// <summary>
        /// </summary>
        public DateTime? NotOnOrAfter { get; set; }

        // Summary:
        // Gets a value that specifies whether the assertion should be used immediately
        // and must not be retained for future use. [Core, 2.5.1]
        // Returns:
        // true if the assertion should be used immediately; otherwise, false.
        /// <summary>
        /// Gets or sets a value indicating whether one time use.
        /// </summary>
        public bool OneTimeUse { get; set; }

        // Summary:
        // Gets or sets the limitations that the asserting party imposes on relying
        // parties that wish to subsequently act as asserting parties themselves and
        // issue assertions of their own on the basis of the information contained in
        // the original assertion. [Core, 2.5.1]
        // Returns:
        // A System.IdentityModel.Tokens.ProxyRestriction that contains the restrictions
        // placed on subsequent asserting parties.

        /// <summary>
        /// Gets or sets the proxy restriction.
        /// </summary>
        public ProxyRestriction ProxyRestriction { get; set; }

        #endregion
    }
}