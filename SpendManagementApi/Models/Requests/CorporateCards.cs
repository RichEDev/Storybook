namespace SpendManagementApi.Models.Requests
{
    using Common;
    using Attributes.Validation;
    using SpendManagementApi.Common.Enums;
    using Utilities;

    /// <summary>
    /// Facilitates the finding of CorporateCards, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindCorporateCardsRequest : FindRequest
    {
        /// <summary>
        /// The Id of the Card Provider.
        /// </summary>
        public int? CardProviderId { get; set; }

        /// <summary>
        /// Whether the card is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Find by Employee Id.
        /// </summary>
        public int? EmployeeId { get; set; }
    }
}