namespace Common.Cryptography
{
    /// <summary>
    /// Create an instance of <see cref="IEncryptor"/> via a specified <see cref="IEncryptorFactory"/>.
    /// </summary>
    public static class EncryptorFactory
    {
        /// <summary>
        /// Private variable for storing the current <see cref="IEncryptorFactory"/> .
        /// </summary>
        private static IEncryptorFactory _factory;

        /// <summary>
        /// Sets the current <see cref="IEncryptorFactory"/>.
        /// </summary>
        /// <param name="factory">The factory to use.</param>
        public static void SetCurrent(IEncryptorFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Creates an instance of <see cref="IEncryptor"/> based on the <see cref="IEncryptorFactory"/> set in <c>SetCurrent</c>.
        /// </summary>
        /// <returns>An instance of <see cref="IEncryptor"/> based on the <see cref="IEncryptorFactory"/> set in <c>SetCurrent</c>.</returns>
        public static IEncryptor CreateEncryptor()
        {
            return _factory?.Create();
        }
    }
}
