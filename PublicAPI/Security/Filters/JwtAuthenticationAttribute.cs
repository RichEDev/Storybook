namespace PublicAPI.Security.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Filters;

    /// <summary>
    /// Authentication filter for verifying a JWT has been set and is valid.
    /// </summary>
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        /// <inheritdoc />
        public bool AllowMultiple => false;

        /// <summary>
        /// Gets or sets the Realm the Jwt token is issued in.
        /// </summary>
        public string Realm { get; set; }

        /// <inheritdoc />
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
            {
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await this.AuthenticateJwtToken(token);

            if (principal == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);
            }
            else
            {
                context.Principal = principal;
            }
        }

        /// <summary>
        /// Authenticates a JWT token.
        /// </summary>
        /// <param name="token">The JWT token to authenticate.</param>
        /// <returns>An IPrincipal using the JWT details.</returns>
        public Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            string username;
            string combinedAccessRoleId;
            string subAccountId;

            if (ValidateToken(token, out username, out combinedAccessRoleId, out subAccountId))
            {
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, combinedAccessRoleId),
                    new Claim("SubAccountId", subAccountId)
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        /// <inheritdoc />
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            this.Challenge(context);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Validates the supplied token and returns the username from within the JWT
        /// </summary>
        /// <param name="token">The token to verify.</param>
        /// <param name="username">out parameter to set as the username from within the JWT.</param>
        /// <param name="combinedAccessRoleId">out parameter to set as the combinedAccessRoleId from within the JWT.</param>
        /// <param name="subAccountId">out parameter to set as the subAccountId from within the JWT.</param>
        /// <returns>True if the token is valid; otherwise, false.</returns>
        private static bool ValidateToken(string token, out string username, out string combinedAccessRoleId, out string subAccountId)
        {
            username = null;
            combinedAccessRoleId = null;
            subAccountId = null;

            var simplePrinciple = JwtManager.GetPrincipal(token);

            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return false;
            }

            if (!identity.IsAuthenticated)
            {
                return false;
            }

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var roleClaim = identity.FindFirst(ClaimTypes.Role);
            combinedAccessRoleId = roleClaim?.Value;

            var subAccountIdClaim = identity.FindFirst("SubAccountId");
            subAccountId = subAccountIdClaim?.Value;

            return true;
        }

        /// <summary>
        /// Challenges the current request to ensure the bearer header is present.
        /// </summary>
        /// <param name="context">The context for the current request.</param>
        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(this.Realm))
            {
                parameter = "realm=\"" + this.Realm + "\"";
            }
            
            context.ChallengeWith("Bearer", parameter);
        }
    }
}