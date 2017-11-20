using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sso.Assert
{
    /// <summary>
    /// The encrypting credentials.
    /// </summary>
    internal class EncryptingCredentials
    {
        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        internal string Algorithm { get; set; }

        /// <summary>
        /// Gets or sets the security key.
        /// </summary>
        internal System.IdentityModel.Tokens.SecurityKey SecurityKey { get; set; }

        /// <summary>
        /// Gets or sets the security key identifier.
        /// </summary>
        internal System.IdentityModel.Tokens.SecurityKeyIdentifier SecurityKeyIdentifier { get;  set; }
    }
}
