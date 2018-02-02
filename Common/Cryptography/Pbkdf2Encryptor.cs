namespace Common.Cryptography
{
        using System;
        using System.Security.Cryptography;

        /// <summary>
        /// Salted password hashing with PBKDF2-SHA1.
        /// Compatibility: .NET 3.0 and later.
        /// </summary>
        public class Pbkdf2Encryptor : IEncryptor
        {
            // The following constants may be changed without breaking existing hashes.
            public const int SaltByteSize = 24;
            public const int HashByteSize = 24;
            public const int Pbkdf2Iterations = 1000;

            public const int IterationIndex = 0;
            public const int SaltIndex = 1;
            public const int Pbkdf2Index = 2;

            /// <summary>
            /// Encrypt <paramref name="input"/>
            /// </summary>
            /// <param name="input">The data to encrypt.</param>
            /// <returns><paramref name="input"/> as an encrypted string.</returns>
            public string Encrypt(string input)
            {
                // Generate a random salt
                RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
                byte[] salt = new byte[SaltByteSize];
                csprng.GetBytes(salt);

                return this.Encrypt(input, salt);
            }

            /// <summary>
            /// Encrypt <paramref name="data"/>
            /// </summary>
            /// <param name="data">The data to encrypt.</param>
            /// <param name="salt">The salt to use with this encryption.</param>
            /// <returns><paramref name="data"/> as an encrypted string </returns>
            public string Encrypt(string data, byte[] salt)
            {
                // Hash the password and encode the parameters
                byte[] hash = this.Pbkdf2(data, salt, Pbkdf2Iterations, HashByteSize);
                return Pbkdf2Iterations + ":" +
                       Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(hash);
            }

            /// <summary>
            /// Verify when <paramref name="input"/> is encrypted it matches an already encrypted string
            /// </summary>
            /// <param name="input">plain text string to encrypt and compare against <paramref name="correctHash"/>.</param>
            /// <param name="correctHash">encrypted string to compare again <paramref name="input"/>.</param>
            /// <returns><c>true</c> if the two match, <c>false</c> if they do not.</returns>
            public bool Verify(string input, string correctHash)
            {
                // Extract the parameters from the hash
                char[] delimiter = { ':' };
                string[] split = correctHash.Split(delimiter);
                int iterations = 0;
                if (int.TryParse(split[IterationIndex], out iterations))
                {
                    byte[] salt = Convert.FromBase64String(split[SaltIndex]);
                    byte[] hash = Convert.FromBase64String(split[Pbkdf2Index]);

                    return this.Verify(input, salt, hash, iterations);
                }

                throw new HashException("Hash is incorrect format");
            }

            /// <summary>
            /// Verify when <paramref name="input"/> is encrypted it matches an already encrypted string
            /// </summary>
            /// <param name="input">Plain text string to encrypt and compare against <paramref name="hashedData"/>.</param>
            /// <param name="salt">The salt to use when encrypting <paramref name="input"/>.</param>
            /// <param name="hashedData">Encrypted string to compare again <paramref name="input"/>.</param>#
            /// <param name="iterations">The iterations used when encrypting</param>
            /// <returns><c>true</c> if the two match, <c>false</c> if they do not.</returns>
            public bool Verify(string input, byte[] salt, byte[] hashedData, int iterations)
            {
                byte[] testHash = this.Pbkdf2(input, salt, iterations, hashedData.Length);
                return SlowEquals(hashedData, testHash);    
            }

            /// <summary>
            /// Compares two byte arrays in length-constant time. This comparison
            /// method is used so that password hashes cannot be extracted from
            /// on-line systems using a timing attack and then attacked off-line.
            /// </summary>
            /// <param name="a">The first byte array.</param>
            /// <param name="b">The second byte array.</param>
            /// <returns>True if both byte arrays are equal. False otherwise.</returns>
            private static bool SlowEquals(byte[] a, byte[] b)
            {
                uint diff = (uint)a.Length ^ (uint)b.Length;
                for (int i = 0; i < a.Length && i < b.Length; i++)
                    diff |= (uint)(a[i] ^ b[i]);
                return diff == 0;
            }

            /// <summary>
            /// Computes the PBKDF2-SHA1 hash of a password.
            /// </summary>
            /// <param name="password">The password to hash.</param>
            /// <param name="salt">The salt.</param>
            /// <param name="iterations">The PBKDF2 iteration count.</param>
            /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
            /// <returns>A hash of the password.</returns>
            private byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
            {
                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
                pbkdf2.IterationCount = iterations;
                return pbkdf2.GetBytes(outputBytes);
            }
        }
}
