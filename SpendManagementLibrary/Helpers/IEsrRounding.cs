namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// Defines the rounding operations allowed for Esr Fields..
    /// </summary>
    public interface IEsrRounding
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
        decimal Round(decimal value);
    }
}