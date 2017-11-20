using System.Collections.Generic;

namespace PublicAPI.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Manages the creation of JSON Web Tokens.
    /// </summary>
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        /// <summary>
        /// Creates a JWT based on the <paramref name="tokenRequest"/> and <paramref name="expireMinutes"/>.
        /// </summary>
        /// <param name="tokenRequest"></param>
        /// <param name="expireMinutes">The number of minutes this JWT is valid for (sets the expired date appropriately).</param>
        /// <returns>A generated JWT.</returns>
        public static string GenerateToken(TokenRequest tokenRequest, int expireMinutes = 20)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;

            string name = $"{tokenRequest.AccountId},{tokenRequest.EmployeeId}";

            if (tokenRequest.DelegateId.HasValue)
            {
                name = name + $",{tokenRequest.DelegateId.Value}";
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                
            };
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        /// <summary>
        /// Gets a <see cref="ClaimsPrincipal"/> from a JWT.
        /// </summary>
        /// <param name="token">The JWT to use.</param>
        /// <returns>An instance of <see cref="ClaimsPrincipal"/> created from the content of the JWT.</returns>
        public static ClaimsPrincipal GetPrincipal(string token)
        {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

            try
            {
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                return null;
            }

        }
    }
}