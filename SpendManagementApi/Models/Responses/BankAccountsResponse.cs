namespace SpendManagementApi.Models.Responses
{
    using System.Collections.Generic;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Types;

    /// <summary>
    /// The bank accounts response.
    /// </summary>
    public class BankAccountsResponse : GetApiResponse<BankAccount>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BankAccountsResponse"/> class.
        /// </summary>
        public BankAccountsResponse()
        {
            this.List = new List<BankAccount>();
        }
    }
}
