
namespace SpendManagementLibrary.Claims
{
    using SpendManagementLibrary.Flags;

    public class SubmitClaimResult
    {
        public SubmitRejectionReason Reason { get; set; }

        public int FrequencyValue { get; set; }

        public byte FrequencyPeriod { get; set; }

        public FlaggedItemsManager FlagResults { get; set; }

        public int ClaimID { get; set; }

        public bool Approver { get; set; }

        public decimal? MinimumAmount { get; set; }

        public decimal? MaximumAmount { get; set; }

        public string NewLocationURL { get; set; }

        public bool NoDefaultAuthoriserPresent { get; set; }

        /// <summary>
        /// Gets or sets the message for the Submit Result.
        /// </summary>
        public string Message { get; set; }
    }
}