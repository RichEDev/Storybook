namespace SpendManagementApi.Models.Requests
{
    using Common;

    /// <summary>
    /// Facilitates the finding of SignOffGroups, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindSignOffGroupsRequest : FindRequest
    {
        /// <summary>
        /// Find my GroupId.
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Find with matching group name.
        /// </summary>
        public string GroupName { get; set; }
    }
}