namespace Common.Cryptography
{
    /// <summary>
    /// Create an instance of <see cref="IEncryptor"/> via a specified <see cref="IEncryptorFactory"/>.
    /// </summary>
    public class HashEncryptorFactory : IEncryptorFactory
    {
        /// <summary>
        /// Create an instance of a <see cref="IEncryptor"/>
        /// </summary>
        /// <returns>An instance of <see cref="IEncryptor"/></returns>
        public IEncryptor Create()
        {
            return new Pbkdf2Encryptor();
        }
    }
}
