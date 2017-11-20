namespace Sso.Assert.Restriction
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The audience restriction.
    /// </summary>
    public class AudienceRestriction
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AudienceRestriction" /> class.
        /// </summary>
        public AudienceRestriction()
        {
            this.Audiences = new Collection<Uri>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a collection of URIs that specifies the audiences to which the assertion
        ///     is addressed. The condition is valid if the relying party is a member of
        ///     any of the specified audiences.
        ///     Returns:
        ///     A collection of type System.Uri that specifies the audiences.
        /// </summary>
        public Collection<Uri> Audiences { get; private set; }

        #endregion
    }
}