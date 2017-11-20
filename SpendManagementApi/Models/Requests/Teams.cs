namespace SpendManagementApi.Models.Requests
{
    using Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines a request that describes the linking of a team to team members.
    /// </summary>
    public class TeamLinkToMembersRequest : ApiRequest
    {
        /// <summary>
        /// A list of employee Ids.
        /// </summary>
        [Required]
        public List<int> EmployeeIds { get; set; }
    }

    /// <summary>
    /// Facilitates the finding of Teams, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindTeamsRequest : FindRequest
    {
        /// <summary>
        /// Search by label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Search by description.
        /// </summary>
        public string Description { get; set; }
    }
}
