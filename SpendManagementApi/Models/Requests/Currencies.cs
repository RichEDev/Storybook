namespace SpendManagementApi.Models.Requests
{
    using Common;
    using Types;

    /// <summary>
    /// Facilitates the finding of Currencies, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindCurrencyRequest : FindRequest
    {
        /// <summary>
        /// The Currency Id.
        /// </summary>
        public int? CurrencyId { get; set; }

        /// <summary>
        /// The Global Currency Id.
        /// </summary>
        public int? GlobalCurrencyId { get; set; }

        /// <summary>
        /// Whether they are archived or not.
        /// </summary>
        public bool? Archived { get; set; }

        /// <summary>
        /// With this Label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// With this symbol.
        /// </summary>
        public string Symbol { get; set; }
    }
}