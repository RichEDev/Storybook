namespace SpendManagementApi.Models.Types
{
    using SpendManagementApi.Common.Enum;
    using Interfaces;

    /// <summary>
    /// A class for handling the result of a claims submission
    /// </summary>
    public class SubmitClaimResult : IApiFrontForDbObject<SpendManagementLibrary.Claims.SubmitClaimResult, SubmitClaimResult>
    {
        /// <summary>
        /// The reason for rejection
        /// </summary>
        public SubmitRejectionReason Reason { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FrequencyValue { get; set; }

        /// <summary>
        /// The frequency Value
        /// </summary>
        public byte FrequencyPeriod { get; set; }

        /// <summary>
        /// The flagged items
        /// </summary>
        public FlaggedItemsManager FlagResults { get; set; }

        /// <summary>
        /// The claim Id
        /// </summary>
        public int ClaimID { get; set; }

        /// <summary>
        /// Is the approver?
        /// </summary>
        public bool Approver { get; set; }

        /// <summary>
        /// The minimum amount
        /// </summary>
        public decimal? MinimumAmount { get; set; }

        /// <summary>
        /// The maximum account
        /// </summary>
        public decimal? MaximumAmount { get; set; }

        /// <summary>
        /// The new location URL
        /// </summary>
        public string NewLocationURL { get; set; }

        /// <summary>
        /// Is there a no default authoriser present?
        /// </summary>
        public bool NoDefaultAuthoriserPresent { get; set; }

        /// <summary>
        /// Converts a SpendManagement SubmitClaimResult  to a API SubmitClaimResult Type
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        public SubmitClaimResult From(SpendManagementLibrary.Claims.SubmitClaimResult dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }
            else
            {
                this.Reason = (SubmitRejectionReason)dbType.Reason;
                this.FrequencyPeriod = dbType.FrequencyPeriod;
                this.FrequencyValue = dbType.FrequencyValue;
                this.FlagResults = new FlaggedItemsManager().From(dbType.FlagResults, actionContext);
                this.ClaimID = dbType.ClaimID;
                this.Approver = dbType.Approver;
                this.MaximumAmount = dbType.MaximumAmount;
                this.MinimumAmount = dbType.MinimumAmount;
                this.NewLocationURL = dbType.NewLocationURL;
                this.NoDefaultAuthoriserPresent = dbType.NoDefaultAuthoriserPresent;
            }

            return this;
        }

        /// Converts API SubmitClaimResult Type to a SpendManagement SubmitClaimResult 
        public SpendManagementLibrary.Claims.SubmitClaimResult To(IActionContext actionContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
