namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;

    using SpendManagementApi.Models.Types;

    using Types.Employees;

    /// <summary>
    /// A response containing a list of <see cref="CorporateCard">CorporateCard</see>s.
    /// </summary>
    public class GetCorporateCardsResponse : GetApiResponse<CorporateCard>
    {
        /// <summary>
        /// Creates a new GetCorporateCardsResponse.
        /// </summary>
        public GetCorporateCardsResponse()
        {
            List = new List<CorporateCard>();
        }
    }

    /// <summary>
    /// A response containing a particular <see cref="CorporateCard">CorporateCard</see>.
    /// </summary>
    public class CorporateCardResponse : ApiResponse<CorporateCard>
    {

    }

    /// <summary>
    /// A response containing a list of <see cref="CardStatement"/> class.
    /// </summary>
    public class CardStatementsResponse : GetApiResponse<CardStatement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardStatementsResponse"/> class.
        /// </summary>
        public CardStatementsResponse()
        {
            this.List = new List<CardStatement>();
        }
    }
}