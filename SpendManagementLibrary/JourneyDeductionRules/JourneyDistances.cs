using SpendManagementLibrary.Addresses;

namespace SpendManagementLibrary.JourneyDeductionRules
{
    /// <summary>
    /// Encapsulate the journey distances calculations which are static methods, into an instance with an interface
    /// </summary>
    public class JourneyDistances : IJourneyDistances
    {
        private int mileageCalculationType;
        private bool useMapPoint;
        private ICurrentUserBase user;

        /// <summary>
        /// Encapsulate the journey distances calculations which are static methods, into an instance with an interface
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="useMapPoint">True if the current account uses Map Point</param>
        /// <param name="mileageCalculationType">The current account's mileage calculaiton type.  Shortest or quickest</param>
        public JourneyDistances(ICurrentUserBase user, bool useMapPoint, int mileageCalculationType)
        {
            this.user = user;
            this.useMapPoint = useMapPoint;
            this.mileageCalculationType = mileageCalculationType;
        }


        /// <summary>
        /// Gets the distance between two addresses, using the overridden (custom) value if there is one, otherwise, looks it up.
        /// </summary>
        /// <param name="fromAddressId">The start address</param>
        /// <param name="toAddressId">The endd address</param>
        /// <returns>The distance between the two addresses</returns>
        public decimal? GetRecommendedOrCustomDistance(int fromAddressId, int toAddressId)
        {
            return AddressDistance.GetRecommendedOrCustomDistance(fromAddressId, toAddressId, this.useMapPoint, this.mileageCalculationType, this.user);
        }
    }
}
