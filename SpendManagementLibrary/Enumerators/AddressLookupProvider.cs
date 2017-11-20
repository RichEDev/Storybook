namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// Denotes which address lookup database is linked to the account's Postcode Anywhere license key
    /// </summary>
    public enum AddressLookupProvider : int
    {
        /// <summary>
        /// The teleatlas database.
        /// </summary>
        Teleatlas = 0,

        /// <summary>
        /// The PAF database.
        /// </summary>
        Paf = 1,

        /// <summary>
        /// The AddressBase database.
        /// </summary>
        AddressBase = 2
    }
}
