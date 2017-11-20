namespace Utilities.Cryptography
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The crypto class to encrypt and decrypt strings.
    /// This is a copy of the standard method used within Expenses.
    /// </summary>
    [Serializable]
    public class ExpensesCryptography : ICryptography
    {
        /// <summary>
        /// Gets the Key.
        /// </summary>
        private static byte[] Key
        {
            get
            {
                var thekey = new byte[]
                                 {
                                     201, 34, 61, 177, 73, 61, 42, 198, 179, 115, 39, 113, 42, 80, 255, 104, 185, 137, 89,
                                     174, 45, 65, 172, 144, 206, 102, 201, 71, 178, 0, 11, 4
                                 };
                return thekey;
            }
        }

        /// <summary>
        /// Gets the Iv.
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                var theiv = new byte[] { 148, 93, 123, 24, 109, 9, 122, 147, 64, 112, 218, 217, 11, 116, 235, 55 };
                return theiv;
            }
        }

        /// <summary>
        /// The basic decrypt method.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Decrypt(string data)
        {
            string result;
            using (var decryptor = new RijndaelManaged())
            {
                using (var stream = new MemoryStream())
                {
                    byte[] inputByteArray = Convert.FromBase64String(data);
                    using (var cs = new CryptoStream(stream, decryptor.CreateDecryptor(Key, Iv), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        result = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The basic decrypt method.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecryptString(string data)
        {
            string result;
            using (var decryptor = new RijndaelManaged())
            {
                using (var stream = new MemoryStream())
                {
                    byte[] inputByteArray = Convert.FromBase64String(data);
                    using (var cs = new CryptoStream(stream, decryptor.CreateDecryptor(Key, Iv), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        result = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The basic encrypt method.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Encrypt(string data)
        {
            string result;
            using (var encryptor = new RijndaelManaged())
            {
                byte[] inputByteArray = Encoding.UTF8.GetBytes(data);

                using (var stream = new MemoryStream())
                {
                    using (var cs = new CryptoStream(stream, encryptor.CreateEncryptor(Key, Iv), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();

                        result = Convert.ToBase64String(stream.ToArray());
                    }
                }
            }

            return result;
        }
    }
}
