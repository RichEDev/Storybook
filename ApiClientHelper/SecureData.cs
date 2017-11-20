namespace ApiClientHelper
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Used to encrypt data
    /// </summary>
    public class SecureData
    {
        /// <summary>
        /// Gets the secret key to be used for the symmetric algorithm
        /// </summary>
        private static byte[] Key
        {
            get
            {
                var key = new byte[32];
                key[0] = 201;
                key[1] = 34;
                key[2] = 61;
                key[3] = 177;
                key[4] = 73;
                key[5] = 61;
                key[6] = 42;
                key[7] = 198;
                key[8] = 179;
                key[9] = 115;
                key[10] = 39;
                key[11] = 113;
                key[12] = 42;
                key[13] = 80;
                key[14] = 255;
                key[15] = 104;
                key[16] = 185;
                key[17] = 137;
                key[18] = 89;
                key[19] = 174;
                key[20] = 45;
                key[21] = 65;
                key[22] = 172;
                key[23] = 144;
                key[24] = 206;
                key[25] = 102;
                key[26] = 201;
                key[27] = 71;
                key[28] = 178;
                key[29] = 0;
                key[30] = 11;
                key[31] = 4;

                return key;
            }
        }

        /// <summary>
        /// Gets the initialization vector to be used for the symmetric algorithm
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                var iv = new byte[16];
                iv[0] = 148;
                iv[1] = 93;
                iv[2] = 123;
                iv[3] = 24;
                iv[4] = 109;
                iv[5] = 9;
                iv[6] = 122;
                iv[7] = 147;
                iv[8] = 64;
                iv[9] = 112;
                iv[10] = 218;
                iv[11] = 217;
                iv[12] = 11;
                iv[13] = 116;
                iv[14] = 235;
                iv[15] = 55;
                return iv;
            }
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="data">The value to encrypt</param>
        /// <returns>The encrypted value</returns>
        public static string Encrypt(string data)
        {
            string encrypted;

            var encryptor = new RijndaelManaged();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(data);

            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, encryptor.CreateEncryptor(Key, Iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();

                encrypted = Convert.ToBase64String(stream.ToArray());
            }

            return encrypted;
        }

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="data"> The value to decrypt </param>
        /// <returns> The decrypted value</returns>
        public static string Decrypt(string data)
        {
            string decrypted;

            var decryptor = new RijndaelManaged();
            byte[] inputByteArray = Convert.FromBase64String(data);

            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, decryptor.CreateDecryptor(Key, Iv), CryptoStreamMode.Write))
            {
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();

                decrypted = Encoding.UTF8.GetString(stream.ToArray());
            }

            return decrypted;
        }
    }
}