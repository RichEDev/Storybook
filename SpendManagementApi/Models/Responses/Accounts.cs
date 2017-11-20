namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;
    using SpendManagementLibrary;

    using Common;

    /// <summary>
    /// Represents a Response containing a Sub Account and it's description.
    /// </summary>
    public class GetSubAccountNameResponse : ApiResponse
    {
        /// <summary>
        /// The Id of the Sub Account.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The description of the Sub Account.
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Represents a response containing a list of <see cref="cAccount"/> accounts.
    /// </summary>
    public class GetAccountsResponse : ApiResponse
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GetAccountsResponse"/> class.
        /// </summary>
        public GetAccountsResponse()
        {
            this.List = new List<cAccount>();
        }

        /// <summary>
        /// Gets or sets the list of accounts.
        /// </summary>
        public List<cAccount> List { get; set; }
    }
}