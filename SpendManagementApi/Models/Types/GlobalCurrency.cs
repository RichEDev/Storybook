namespace SpendManagementApi.Models.Types
{
    using SpendManagementLibrary;

    /// <summary>
    /// A Global Currency is an immutable curency that exists on Earth. 
    /// Different Accounts pull in which currency they wish to use, by way of a <see cref="Currency">Currency</see>.
    /// </summary>
    public class GlobalCurrency : BaseExternalType
    {
        /// <summary>
        /// The unique Id of this global currency.
        /// </summary>
        public int GlobalCurrencyId { get; set; }

        /// <summary>
        /// The label for this global currency.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The Alpha code, as per ISO 3166-1 alpha 3.
        /// </summary>
        public string AlphaCode { get; set; }

        /// <summary>
        /// The 3 Letter currency code, as per ISO 3166-1.
        /// </summary>
        public string NumericCode { get; set; }

        /// <summary>
        /// The Symbol for this global currency.
        /// </summary>
        public string Symbol { get; set; }
    }

    internal static class GlobalCurrencyExtension
    {
        internal static TResult Cast<TResult>(this cGlobalCurrency globalCurrency) where TResult : GlobalCurrency, new()
        {
            if (globalCurrency != null)
                return new TResult
                {
                    AlphaCode = globalCurrency.alphacode,
                    CreatedOn = globalCurrency.createdon,
                    GlobalCurrencyId = globalCurrency.globalcurrencyid,
                    Label = globalCurrency.label,
                    ModifiedOn = globalCurrency.modifiedon,
                    NumericCode = globalCurrency.numericcode,
                    Symbol = globalCurrency.symbol
                };
            return null;
        }
    }
}