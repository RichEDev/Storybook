namespace Common.Cryptography
{
    using System;

    /// <summary>
    /// An Exception caused by invalid hash string.
    /// </summary>
    public class HashException : Exception
    {
        /// <summary>
        /// Create a new instance of <see cref="HashException"/>
        /// </summary>
        /// <param name="message"></param>
        public HashException(string message) : base(message)
        {
        }
    }
}