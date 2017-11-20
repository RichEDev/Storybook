namespace SpendManagementLibrary.Interfaces
{
    /// <summary>
    /// The cGlobalCountries Interface
    /// </summary>
    public interface IGlobalCountries
    {
        /// <summary>
        /// Returns the global country Id for the specified ISO 3166-1 alpha-3 code.
        /// </summary>
        /// <param name="code">The alpha-3 code to search for</param>
        /// <returns>The global country Id for the matching GlobalCountry object</returns>
        int GetGlobalCountryIdByAlpha3Code(string code);

        /// <summary>
        /// Returns a global country by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        cGlobalCountry getGlobalCountryById(int id);
    }
}
