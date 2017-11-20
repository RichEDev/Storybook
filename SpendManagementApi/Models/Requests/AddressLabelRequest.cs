namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The address label request.
    /// </summary>
    public class AddressLabelRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label id.
        /// </summary>
        public int LabelId { get; set; }
    }
}