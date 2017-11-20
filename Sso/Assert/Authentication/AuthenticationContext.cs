namespace Sso.Assert.Authentication
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The authentication context.
    /// </summary>
    internal class AuthenticationContext
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthenticationContext" /> class.
        /// </summary>
        internal AuthenticationContext()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        ///     Gets a collection of zero or more unique identifiers (URIs) of authentication
        ///     authorities that were involved in the authentication of the principal (not
        ///     including the assertion issuer, who is presumed to have been involved without
        ///     being explicitly included in the collection). [Core, 2.7.2.2]
        ///     Returns:
        ///     Returns a collection of type System.Uri that identifies the authenticating
        ///     authorities that were involved in the authentication of the principal.
        /// </summary>
        internal Collection<Uri> AuthenticatingAuthorities { get; private set; }

        /// <summary>
        ///     Gets or sets the URI reference that identifies an authentication context
        ///     class that describes the authentication context declaration that follows.
        ///     Returns:
        ///     A System.Uri that identifies the context class.
        ///     Exceptions:
        ///     System.ArgumentException:
        ///     An attempt to set a value that is not null and is not an absolute URI occurs.
        /// </summary>
        internal Uri ClassReference { get; set; }

        /// <summary>
        ///     Gets or sets a URI reference that identifies an authentication context declaration.
        ///     Returns:
        ///     A System.Uri that identifies an authentication context declaration.
        ///     Exceptions:
        ///     System.ArgumentException:
        ///     An attempt to set a value that is not null and is not an absolute URI occurs.
        /// </summary>
        internal Uri DeclarationReference { get; set; }

        #endregion
    }
}