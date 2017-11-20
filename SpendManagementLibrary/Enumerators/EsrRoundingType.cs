namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// The ESR rounding type fpr integer fields.
    /// </summary>
    public enum EsrRoundingType
    {
        /// <summary>
        /// Round down (truncate).
        /// </summary>
        Down = 0,

        /// <summary>
        /// Round up using maths rounding.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Force rounding up (any decimal rounds up).
        /// </summary>
        ForceUp = 2
    }
}