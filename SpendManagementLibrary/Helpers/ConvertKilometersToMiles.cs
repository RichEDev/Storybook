namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// The convert kilometers to miles.
    /// </summary>
    public static class ConvertKilometersToMiles
    {
        /// <summary>
        /// The perform of kilometers to miles
        /// </summary>
        /// <param name="kilometers">
        /// The kilometer to convert.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> converted mileage.
        /// </returns>
        public static decimal PerformConversion(decimal kilometers)
        {
            return (kilometers == 0m) ? 0m : kilometers / 1.609344m;
        }
    }
}
