namespace SpendManagementLibrary.Helpers
{
    using System;

    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Esxtension methods for the decimal class.
    /// </summary>
    public static class DecimalExtension
    {
        /// <summary>
        /// The ESR rounding extention for decimal objects.
        /// </summary>
        /// <param name="value">
        /// The value to round.
        /// </param>
        /// <param name="esrRoundingType">
        /// The ESR rounding type.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// The rounded value based on the ESR rounding type
        /// </returns>
        public static decimal EsrRounding(this decimal value, EsrRoundingType esrRoundingType)
        {
            var num = value;
            if (num > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value to large");
            }

            var roundingType = EsrRoundingFactory.New(esrRoundingType);
            return roundingType.Round(value);
        }
    }
}
