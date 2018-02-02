using Common.Cryptography;

namespace SpendManagementLibrary.Employees
{
    using System;

    /// <summary>
    /// Password encryption methods used - logged against the user account
    /// </summary>
    [Serializable]
    public enum PasswordEncryptionMethod
    {
        /// <summary>
        /// Old nasty Framework hashing. Should not be used.
        /// </summary>
        FWBasic = 0,

        /// <summary>
        /// Old nasty Framework hashing. Should not be used.
        /// </summary>
        Hash = 1,

        /// <summary>
        /// SHA 256 hashing. Should not be used.
        /// </summary>
        ShaHash = 2,

        /// <summary>
        /// MD5 hashing. Should not be used.
        /// </summary>
        MD5 = 3,

        /// <summary>
        /// RijndaelManaged (cSecureData).
        /// </summary>
        RijndaelManaged = 4,

        /// <summary>
        /// Salted hash <see cref="Pbkdf2Encryptor"/>
        /// </summary>
        SaltedHash = 5,

        /// <summary>
        /// Plain text. Should not be used.
        /// </summary>
        NoCrypt = 99
    }
}
