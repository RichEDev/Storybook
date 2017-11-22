namespace SpendManagementLibrary.Helpers
{
    public static class ConvertMilesToKilometers
    {
        /// <summary>
        /// The perform of kilometers to miles
        /// </summary>
        /// <param name="miles">
        /// The miles to convert.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/> converted kilometers.
        /// </returns>
        public static decimal PerformConversion(decimal miles)
        {
            return (miles == 0m) ? 0m : miles * 1.609344m;
        }
    }
}
