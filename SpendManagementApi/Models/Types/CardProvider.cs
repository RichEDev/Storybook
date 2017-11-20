namespace SpendManagementApi.Models.Types
{
    using SpendManagementLibrary;

    /// <summary>
    /// Represents a company that provides Credit / Debit cards, such as American Express or Barclaycard.
    /// </summary>
    public class CardProvider : BaseExternalType
    {
        /// <summary>
        /// The card provider Id.
        /// </summary>
        public int CardProviderId { get; set; }

        /// <summary>
        /// The name of the card provider.
        /// </summary>
        public string CardProviderName { get; set; }

        /// <summary>
        /// The type of the card.
        /// </summary>
        public CorporateCardType CardType { get; set; }

    }

    internal static class CardProviderConversion
    {
        internal static TResult Cast<TResult>(this cCardProvider cardProvider) where TResult : CardProvider, new()
        {
            if (cardProvider == null) return null;
            return new TResult
                       {
                           CreatedById = cardProvider.createdby,
                           CreatedOn = cardProvider.createdon,
                           ModifiedById = cardProvider.modifiedby,
                           ModifiedOn = cardProvider.modifiedon,
                           CardProviderName = cardProvider.cardprovider,
                           CardProviderId = cardProvider.cardproviderid,
                           CardType = cardProvider.cardtype
                       };
        }
    }
}