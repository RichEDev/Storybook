namespace SpendManagementLibrary.DVLA
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// Gets the email details of claimants whose consent is due to expire.
    /// </summary>
    public class DvlaConsentReminders
    {
        /// <summary>
        /// Procedure to get the Email details of Claimants whose Duty Of Care Documents are due to expire and notify Line manager.
        /// </summary>
        private const string StoredProcGetClaimantIdsWithExpiredDutyOfCareForLineManager = "GetUsersWithExpiringConsent";

        /// <summary>
        /// The account connection string.
        /// </summary>
        private static string accountConnectionString;

        /// <summary>
        /// The account id of the current account.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <returns>
        /// The List of <see cref="DvlaConsentReminderEmaildetail"/> Dvla consent reminder email details.
        /// </returns>
        public static List<DvlaConsentReminderEmaildetail> GetClaimantDetailsForConsentReminders(int accountid)
        {
            var claimantDetails = new List<DvlaConsentReminderEmaildetail>();
            accountConnectionString = cAccounts.getConnectionString(accountid);
                    using (var connection = new DatabaseConnection(accountConnectionString))
                    {
                        var claimantInfo = connection.GetProcDataSet(StoredProcGetClaimantIdsWithExpiredDutyOfCareForLineManager);
                        if (claimantInfo != null && claimantInfo.Tables.Count > 0 && claimantInfo.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow claimantId in claimantInfo.Tables[0].Rows)
                            {
                                claimantDetails.Add(DvlaConsentReminderEmaildetail.InitialiseClaimantIds(Convert.ToInt32(claimantId["employeeid"]), claimantId["emailId"].ToString(), claimantId["SecurityCode"].ToString()));
                            }

                            claimantInfo.Clear();
                        }
                    }
              

            return claimantDetails;
        }
    }
}
