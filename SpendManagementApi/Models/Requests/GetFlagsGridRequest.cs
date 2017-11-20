namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Handles the request for the expenses flag gird
    /// </summary>
    public class GetFlagsGridRequest
    {
        /// <summary>
        /// The claimId
        /// </summary>
        [Required]
        public int claimId;  
        
        /// <summary>
        /// The list of expense ids
        /// </summary>
        [Required]
        public List<int> expenseIds;

        /// <summary>
        /// The source of the request i.e. claimviewer
        /// </summary>
        [Required]
        public string pageSource;     
    }
}