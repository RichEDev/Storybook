namespace SpendManagementLibrary.Helpers
{
    using System;

    /// <summary>
    /// The ESR rounding force up o next integer.
    /// </summary>
    public class EsrRoundingForceUp : IEsrRounding
    {
        /// <summary>
        /// Round the given value based on the type of ESR Rounding.
        /// </summary>
        /// <param name="value">
        /// The value to round.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// The rounded value.
        /// </returns>
        public decimal Round(decimal value)
        {
            var result = (int)Math.Abs(value);
            if (value > result)
            {
                result++;
            }

            return result;
        }
    }
}