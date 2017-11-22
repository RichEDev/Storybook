namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// A class to convert kilometers to miles.
    /// </summary>
    public static class ConvertKilometersToMiles
    {
        /// <summary>
        /// Converts kilometers to miles.
        /// </summary>
        /// <param name="kilometers">
        /// The kilometers to convert.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> converted miles.
        /// </returns>
        public static decimal PerformConversion(decimal kilometers)
        {
            return (kilometers == 0m) ? 0m : kilometers / 1.609344m;
        }
    }
}
