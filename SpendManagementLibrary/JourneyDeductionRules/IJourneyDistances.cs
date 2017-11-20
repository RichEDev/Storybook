namespace SpendManagementLibrary.JourneyDeductionRules
{
    /// <summary>
    /// Encapsulate the journey distances calculations which are static methods, into an instance with an interface
    /// </summary>
    public interface IJourneyDistances
    {
        /// <summary>
        /// Gets the distance between two addresses, using the overridden (custom) value if there is one, otherwise, looks it up.
        /// </summary>
        /// <param name="fromAddressId">The start address</param>
        /// <param name="toAddressId">The endd address</param>
        /// <returns>The distance between the two addresses</returns>
        decimal? GetRecommendedOrCustomDistance(int fromAddressId, int toAddressId);
    }
}