using System.Collections.Generic;

namespace ApproverReminder
{
    /// <summary>
    /// The Response received from the "Get Accounts" Api end point
    /// </summary>
    public class GetAccountsResponse 
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GetAccountsResponse"/> class.
        /// </summary>
        public GetAccountsResponse()
        {
            this.AccountList = new List<cAccount>();
        }

        /// <summary>
        /// Gets or sets the list of accounts.
        /// </summary>
        public List<cAccount> AccountList { get; set; }
    }
}