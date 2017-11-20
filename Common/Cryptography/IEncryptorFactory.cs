namespace Common.Cryptography
{
    /// <summary>
    /// Create an instance of <see cref="IEncryptor"/> via a specified <see cref="IEncryptorFactory"/>.
    /// </summary>
    public interface IEncryptorFactory
    {
        /// <summary>
        /// Create an instance of a <see cref="IEncryptor"/>
        /// </summary>
        /// <returns>An instance of <see cref="IEncryptor"/></returns>
        IEncryptor Create();
    }
}
