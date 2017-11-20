using System;

namespace BusinessLogic
{
    /// <summary>
    /// Defines standard guard methods such as <c>ThrowIfNull</c>, used when validating arguments and parameters passed to object members
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Check if <paramref name="argumentValue"/> is <see langword="null"/>, if so a <see cref="ArgumentNullException"/> is thrown specifying <paramref name="argumentName"/>
        /// </summary>
        /// <param name="argumentValue">The object to test the <see langword="null"/> condition on.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ThrowIfNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Check if <paramref name="argumentValue"/> is <c>string.IsNullOrWhiteSpace(argumentValue)</c>, if so a <see cref="ArgumentNullException"/> is thrown specifying <paramref name="argumentName"/>
        /// </summary>
        /// <param name="argumentValue">The object to test the <c>string.IsNullOrWhiteSpace(argumentValue)</c> condition on.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ThrowIfNullOrWhiteSpace(string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentNullException(argumentName);
            }
        }
    }
}
