namespace ApiClientHelper.Responses
{
    /// <summary>
    /// The currency response.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Gets or sets the unique Id of the currency.
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the currency name
        /// </summary>
        public string AlphaCode { get; set; }
        
    }
}
