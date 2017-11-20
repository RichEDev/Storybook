namespace SpendManagementApi.Models.Responses
{
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary.Claims;

    /// <summary>
    /// The result of assigning a claim to an approver for approval 
    /// </summary>
    public class AssignClaim : BaseExternalType, IBaseClassToAPIType<AllocateClaimResult, AssignClaim>
    {
        /// <summary>
        /// Gets or sets the Id of the claim
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the name of the claim
        /// </summary>
        public string ClaimName { get; set; }

        /// <summary>
        /// Gets or sets the result of allocating a claim to a team member for approval
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public AssignClaim ToApiType(AllocateClaimResult dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            this.ClaimId = dbType.ClaimId;
            this.ClaimName = dbType.ClaimName;
            this.Success = dbType.Success;

            return this;
        }
    }

}
