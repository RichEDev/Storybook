namespace Sso.Assert.SubjectConf
{
    using System;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Tokens;

    using Sso.Shared;

    /// <summary>
    ///     The subject confirmation data.
    /// </summary>
    public class SubjectConfirmationData
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubjectConfirmationData" /> class.
        /// </summary>
        public SubjectConfirmationData()
        {
            this.KeyIdentifiers = new Collection<SecurityKeyIdentifier>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Gets or sets the in response to.
        /// </summary>
        public Id InResponseTo { get; set; }

        /// <summary>
        ///     Gets the key identifiers.
        /// </summary>
        public Collection<SecurityKeyIdentifier> KeyIdentifiers { get; private set; }

        /// <summary>
        ///     Gets or sets the not before.
        /// </summary>
        public DateTime? NotBefore { get; set; }

        /// <summary>
        ///     Gets or sets the not on or after.
        /// </summary>
        public DateTime? NotOnOrAfter { get; set; }

        /// <summary>
        ///     Gets or sets the recipient.
        /// </summary>
        public Uri Recipient { get; set; }

        #endregion
    }
}