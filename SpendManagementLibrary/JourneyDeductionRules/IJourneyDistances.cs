namespace SpendManagementLibrary.JourneyDeductionRules
{
    using SpendManagementLibrary.Addresses;

    /// <summary>
    /// Encapsulate the journey distances calculations which are static methods, into an instance with an interface
    /// </summary>
    public interface IJourneyDistances
    {
        /// <summary>
        /// Gets the distance between two addresses, using the overridden (custom) value if there is one, otherwise, looks it up.
        /// </summary>
        /// <param name="fromAddress">The start <see cref="Address"/></param>
        /// <param name="toAddress">The endd <see cref="Address"/></param>
        /// <returns>The distance between the two addresses</returns>
        decimal? GetRecommendedOrCustomDistance(Address fromAddress, Address toAddress);
    }
}