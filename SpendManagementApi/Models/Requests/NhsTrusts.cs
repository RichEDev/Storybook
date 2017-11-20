namespace SpendManagementApi.Models.Requests
{ 
    using Common;

    /// <summary>
    /// Facilitates the finding of Departments, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindNhsTrustRequest : FindRequest
    {
        /// <summary>
        /// Search by VPD.
        /// </summary>
        public string TrustVpd { get; set; }

        /// <summary>
        /// Search by label.
        /// </summary>
        public string Label { get; set; }
    }
}