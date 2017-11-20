namespace SpendManagementLibrary.Addresses
{
    /// <summary>
    /// The distance type.
    /// </summary>
    public enum DistanceType
    {
        STRAIGHTLINE,
        DRIVETIME,

        /// <summary>
        /// Take the fastest route possible
        /// </summary>
        Fastest,

        /// <summary>
        /// Take the shortest route possible
        /// </summary>
        Shortest
    }
}