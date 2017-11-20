namespace SpendManagementApi.Models.Types.Expedite
{
    /// <summary>
    /// Defines  Expedite Operator Validation Progress
    /// </summary>
    public enum ExpediteOperatorValidationProgress
    {
        /// <summary>
        /// Available for any Expedite Operator  
        /// </summary>
        Available = 0,

        /// <summary>
        /// Under validation by an Expedite Operator  
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Validation Completed by Expedite Operator  
        /// </summary>
        Completed = 2,
    }
}
