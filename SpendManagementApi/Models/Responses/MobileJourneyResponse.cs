namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using Common;
    using SpendManagementLibrary.Mobile;

    /// <summary>
    /// A class to handle the MobileJourneyResponse response
    /// </summary>
    public class MobileJourneyResponse : ApiResponse
    {
        /// <summary>
        /// Creates a new MobileJourneyResponse.
        /// </summary>
        public MobileJourneyResponse()
        {
           this.List = new List<MobileJourney>();
        }

        /// <summary>
        /// Gets or sets the list of <see cref="MobileJourney">MobileJourney</see>.
        /// </summary>
        public List<MobileJourney> List { get; set; }
    }
}