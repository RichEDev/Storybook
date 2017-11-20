namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// A class to hold the count of claims at the various stages
    /// </summary>
    public class ClaimCountForAllStages
    {
        /// <summary>
        /// Holds the count of claims at the current stage
        /// </summary>
        public int CurrentClaimsCount { get; set;}

        /// <summary>
        /// Holds the count of claims at the submitted stage
        /// </summary>
        public int SubmittedClaimsCount { get; set; }

        /// <summary>
        /// Holds the count of claims at the previous stage
        /// </summary>
        public int PreviousClaimsCount { get; set; }
    }
}