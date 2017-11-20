namespace SpendManagementApi.Models.Requests
{
    using SpendManagementApi.Models.Common;

    /// <summary>
    /// The allocate card to employee request.
    /// </summary>
    public class AllocateCardToEmployeeRequest : ApiRequest
    {
        /// <summary>
        /// Gets or sets the employee id.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the statement id.
        /// </summary>
        public int StatementId { get; set; }

        /// <summary>
        /// Gets or sets the card number.
        /// </summary>
        public string CardNumber { get; set; }
    }
}