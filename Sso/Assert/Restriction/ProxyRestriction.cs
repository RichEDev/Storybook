namespace Sso.Assert.Restriction
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The proxy restriction.
    /// </summary>
    public class ProxyRestriction
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyRestriction" /> class.
        /// </summary>
        public ProxyRestriction()
        {
            this.Audiences = new Collection<Uri>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the audiences.
        ///     Gets or sets the set of audiences to whom the asserting party permits new assertions to be issued on the basis of this assertion.
        ///     Return Values:
        ///     A collection of type System.Uri that contains the addresses of the entities about which new assertions can be issued
        /// </summary>
        public Collection<Uri> Audiences { get; private set; }

        /// <summary>
        ///     Gets or sets the count.
        ///     Gets or sets the maximum number of indirections that the asserting party permits to exist between this assertion and an assertion which has ultimately been issued on the basis of it.
        ///     Return Values:
        ///     A NULLABLE integer. null indicates that the attribute is not set and no limitation is set on the number of indirections.
        /// </summary>
        public int? Count { get; set; }

        #endregion
    }
}