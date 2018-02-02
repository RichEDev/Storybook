namespace Common.Cryptography
{
    /// <summary>
    /// Encryptor interface.
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypt <paramref name="input"/>
        /// </summary>
        /// <param name="input">The data to encrypt.</param>
        /// <returns><paramref name="input"/> as an encrypted string.</returns>
        string Encrypt(string input);

        /// <summary>
        /// Encrypt <paramref name="data"/>
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="salt">The salt to use with this encryption.</param>
        /// <returns><paramref name="data"/> as an encrypted string </returns>
        string Encrypt(string data, byte[] salt);

        /// <summary>
        /// Verify when <paramref name="input"/> is encrypted it matches an already encrypted string
        /// </summary>
        /// <param name="input">plain text string to encrypt and compare against <paramref name="hashedData"/>.</param>
        /// <param name="hashedData">encrypted string to compare again <paramref name="input"/>.</param>
        /// <returns><c>true</c> if the two match, <c>false</c> if they do not.</returns>
        bool Verify(string input, string hashedData);

        /// <summary>
        /// Verify when <paramref name="input"/> is encrypted it matches an already encrypted string
        /// </summary>
        /// <param name="input">Plain text string to encrypt and compare against <paramref name="hashedData"/>.</param>
        /// <param name="salt">The salt to use when encrypting <paramref name="input"/>.</param>
        /// <param name="hashedData">Encrypted string to compare again <paramref name="input"/>.</param>
        /// <param name="iterations">The iterations used when encrypting</param>
        /// <returns><c>true</c> if the two match, <c>false</c> if they do not.</returns>
        bool Verify(string input, byte[] salt, byte[] hashedData, int iterations);
    }
}
