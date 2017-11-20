namespace SpendManagementApi.Models.Responses
{
    using Common;
    using System.Collections.Generic;

    /// <summary>
    /// List of accounts with particular general option is enavbled.
    /// </summary>
    public class GeneralOptionAccountsResponse : ApiResponse
    {
        /// <summary>
        /// Account list with particular general option is enavbled.
        /// </summary>
        public List<GeneralOptionEnabledAccount> AccountList = new List<GeneralOptionEnabledAccount>();
    }
}