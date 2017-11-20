using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Utilities
{
    /// <summary>
    /// Simple wrapper for generation, encryption and decryption of AuthToken. This uses RSA Crypto.
    /// </summary>
    public class ApiAuthTokenProvider : IAuthTokenProvider
    {
        private static readonly UnicodeEncoding Encoder = new UnicodeEncoding();

        /// <summary>
        /// Generates a new Certificate, and returns its xml string representation.
        /// This contains both thw private and public keys.
        /// </summary>
        /// <returns>An Xml string.</returns>
        public string Generate()
        {
            return new RSACryptoServiceProvider().ToXmlString(true);
        }

        /// <summary>
        /// Decrypts an AuthToken using the private key of a certificate.
        /// </summary>
        /// <param name="key">The private part of the certificate, in xml string format.</param>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="separator">the separator string to separate segments of the token.</param>
        /// <returns>The unencrypted string.</returns>
        public string Decrypt(string key, string data, string separator)
        {
            // strip away the account id and api details id portion of the AuthToken
            data = data.Substring(data.LastIndexOf(separator, StringComparison.Ordinal) + 1);

            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(Convert.ToChar(","));
            var dataByte = new byte[dataArray.Length];
            for (var i = 0; i < dataArray.Length; i++) dataByte[i] = Convert.ToByte(dataArray[i]);
            rsa.FromXmlString(key);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return Encoder.GetString(decryptedByte);
        }

        /// <summary>
        /// Encrypts some string data with the public key of a certificate.
        /// </summary>
        /// <param name="key">The public part of the certificate, in xml string format.</param>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        public string Encrypt(string key, string data)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);
            var dataToEncrypt = Encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);
                if (item < length) sb.Append(",");
            }

            return sb.ToString();
        }
    }
}