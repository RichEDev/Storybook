namespace SpendManagementLibrary.Helpers
{
    using System;

    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Creatges an instance of <see cref="IEsrRounding"/> based on the gieven enum value.
    /// </summary>
    public static class EsrRoundingFactory
    {
        /// <summary>
        /// Returns a New <see cref="IEsrRounding"/> based on the given type.
        /// </summary>
        /// <param name="esrRoundingType">
        /// The ESR rounding type.
        /// </param>
        /// <returns>
        /// An instance of  <see cref="IEsrRounding"/> based on the given Enum.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the enum is out of range.
        /// </exception>
        public static IEsrRounding New(EsrRoundingType esrRoundingType)
        {
            switch (esrRoundingType)
            {
                case EsrRoundingType.Down:
                    return new EsrRoundingDown();
                case EsrRoundingType.Up:
                    return new EsrRoundingUp();
                case EsrRoundingType.ForceUp:
                    return new EsrRoundingForceUp();
                default:
                    throw new ArgumentOutOfRangeException(nameof(esrRoundingType), esrRoundingType, null);
            }
        }
    }
}