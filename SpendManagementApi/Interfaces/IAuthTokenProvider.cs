namespace SpendManagementApi.Interfaces
{
    /// <summary>
    /// Defines the behaviour of an Auth Token Provider for the API.
    /// </summary>
    public interface IAuthTokenProvider
    {
        /// <summary>
        /// Generates a new Certificate, and returns its xml string representation.
        /// This contains both thw private and public keys.
        /// </summary>
        /// <returns>An Xml string.</returns>
        string Generate();

        /// <summary>
        /// Decrypts an AuthToken using the private key of a certificate.
        /// </summary>
        /// <param name="key">The private part of the certificate, in xml string format.</param>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="separator">the separator string to separate segments of the token.</param>
        /// <returns>The unencrypted string.</returns>
        string Decrypt(string key, string data, string separator);

        /// <summary>
        /// Encrypts some string data with the public key of a certificate.
        /// </summary>
        /// <param name="key">The public part of the certificate, in xml string format.</param>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        string Encrypt(string key, string data);
    }
}
