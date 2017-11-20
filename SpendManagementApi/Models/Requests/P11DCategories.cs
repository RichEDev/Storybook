namespace SpendManagementApi.Models.Requests
{
    using Common;

    /// <summary>
    /// Facilitates the finding of P11DCategories, by providing a few optional search / filter parameters.
    /// </summary>
    public class FindP11DCategoryRequest : FindRequest
    {
        /// <summary>
        /// The name or label for this P11d Category
        /// </summary>
        public string Label { get; set; }
    }
}