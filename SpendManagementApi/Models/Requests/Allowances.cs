namespace SpendManagementApi.Models.Requests
{
    using Common;

    /// <summary>
    /// Facilitates the finding of Allowances, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindAllowancesRequest : FindRequest
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