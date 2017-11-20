namespace SpendManagementApi.Models.Requests
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Defines a request that saves flag justifications provided by for the given flagged item id
    /// </summary>
    public class FlagsJustificationRequest : ApiRequest
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FlagsJustificationRequest"/> class.
        /// </summary>
        public FlagsJustificationRequest()
        {
            this.FlagsJustificationRequests = new List<FlagJustification>();
        }

        /// <summary>
        /// Gets or sets the justification requests.
        /// </summary>
        public List<FlagJustification> FlagsJustificationRequests { get; set; }
    }

    /// <summary>
    /// A class to hold the details of a flag justification
    /// </summary>
    public class FlagJustification
    {
        /// <summary>
        /// The Id of the claim
        /// </summary>
        public int ClaimId { get; set; }
        /// <summary>
        /// The Id of the expense item
        /// </summary>
        public int ExpenseId { get; set; }
        /// <summary>
        /// The Id of the flag
        /// </summary>
        public int FlaggedItemId { get; set; }
        /// <summary>
        /// The justification for the flag 
        /// </summary>
        public string Justification { get; set; }
    }
}