namespace SpendManagementLibrary.Helpers
{
    using System;

    /// <summary>
    /// The ESRrounding down (truncating to integer).
    /// </summary>
    public class EsrRoundingDown : IEsrRounding
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
            return (int)Math.Abs(value);
        }
    }
}