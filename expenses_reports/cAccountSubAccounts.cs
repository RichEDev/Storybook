using SpendManagementLibrary;

namespace Expenses_Reports
{
    /// <summary>
    /// cAccountSubAccounts class
    /// </summary>
    public class cAccountSubAccounts : cAccountSubAccountsBase
    {
        /// <summary>
        /// Customer Account Id
        /// </summary>
        private readonly int _nAccountId;

        /// <summary>
        /// cAccountSubAccounts constructor
        /// </summary>
        /// <param name="accountId">Customer DB Account Id</param>
        public cAccountSubAccounts(int accountId) : base(accountId)
        {
            _nAccountId = accountId;

        }
       
    }
}
