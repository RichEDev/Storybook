namespace SpendManagementLibrary.Helpers
{
    using System;

    /// <summary>
    /// The esr rounding up to next integer.
    /// </summary>
    public class EsrRoundingUp : IEsrRounding
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
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}