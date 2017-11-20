namespace SpendManagementLibrary.Claims
{
    using System.Collections.Generic;
    using System.Data;

    using Helpers;

    /// <summary>
    /// The claim email reminder.
    /// </summary>
    public class ClaimEmailReminder
    {
        /// <summary>
        /// Gets the Approver Ids who have pending claims
        /// </summary>
        /// <returns>list of Claimant details</returns>
        public static List<ClaimEmailDetails> GetApproverIdsWhoHavePendingClaims()
        {
            var expenseCustomers = new cAccounts().GetAllAccounts();
            if (expenseCustomers != null && expenseCustomers.Count != 0)
            {
                List<ClaimEmailDetails> claimantDetails = new List<ClaimEmailDetails>();

                foreach (cAccount claimApprover in expenseCustomers)
                {
                    claimantDetails.AddRange(GetApproverIdsWhoHavePendingClaims(claimApprover.accountid));
                }

                return claimantDetails;
            }

            return null;
        }

        /// <summary>
        /// Gets the Approver Ids who have pending claims
        /// </summary>
        /// <param name="accountId">The account ID to scan for Pending Claims</param>
        /// <returns><see cref="List{T}"/> of <see cref="ClaimEmailDetails"/></returns>
        public static List<ClaimEmailDetails> GetApproverIdsWhoHavePendingClaims(int accountId)
        {
            List<ClaimEmailDetails> claimantDetails = new List<ClaimEmailDetails>();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                using (IDataReader claimantInfo = connection.GetReader("GetApproversReminderList"))
                {
                    while (claimantInfo.Read())
                    {
                        if (claimantInfo.GetInt32(0) > 0)
                        {
                            claimantDetails.Add(
                                InitialiseEmployeeIds(
                                    claimantInfo.GetInt32(0),
                                    accountId));
                        }
                    }
                }
            }

            return claimantDetails;
        }

        /// <summary>
        /// Gets the claimants Ids who have current claims
        /// </summary>
        /// <returns>list of Claimant details</returns>
        public static List<ClaimEmailDetails> GetClaimantIdsWhoHaveCurrentClaims()
        {
            var expenseCustomers = new cAccounts().GetAllAccounts();
            if (expenseCustomers == null || expenseCustomers.Count == 0) return null;
            var claimantDetails = new List<ClaimEmailDetails>();

            foreach (var account in expenseCustomers)
            {
                claimantDetails.AddRange(GetClaimantIdsWhoHaveCurrentClaims(account.accountid));
            }

            return claimantDetails;
        }

        /// <summary>
        /// Gets the claimants Ids who have current claims
        /// </summary>
        /// <param name="accountId">The account to get claimants for</param>
        /// <returns>list of Claimant details</returns>
        public static List<ClaimEmailDetails> GetClaimantIdsWhoHaveCurrentClaims(int accountId)
        {
            List<ClaimEmailDetails> claimantDetails = new List<ClaimEmailDetails>();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                using (var claimantInfo = connection.GetReader("GetClaimantReminderList"))
                {
                    while (claimantInfo.Read())
                    {
                        if (claimantInfo.GetInt32(0) > 0)
                        {
                            claimantDetails.Add(
                                InitialiseEmployeeIds(
                                    claimantInfo.GetInt32(0),
                                    accountId));
                        }
                    }
                }
            }

            return claimantDetails;
        }

        /// <summary>
        /// The initialise approver ids.
        /// </summary>
        /// <param name="approverId">
        /// The approver id.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="ClaimEmailDetails"/>.
        /// </returns>
        public static ClaimEmailDetails InitialiseEmployeeIds(int employeeId, int accountId)
        {
            var approverDetails = new ClaimEmailDetails { EmployeeId = employeeId, AccountId = accountId };
            return approverDetails;
        }
    }
}
