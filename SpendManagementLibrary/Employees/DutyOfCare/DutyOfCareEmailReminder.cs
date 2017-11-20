using SpendManagementLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Employees.DutyOfCare
{
     /// <summary>
     /// Gets the email details of claimants whose Duty of Care document is due to expire and need to notify Line manager.
     /// </summary>
    public class DutyOfCareEmailReminder
    {
        /// <summary>
        /// The account connection string.
        /// </summary>
        static string _accountConnectionString;

        /// <summary>
        /// Procedure to get the Email details of Claimants whose Duty Of Care Documents are due to expire and notify Line manager.
        /// </summary>
        private const string StoredProcGetClaimantIdsWithExpiredDutyOfCareForLineManager = "getClaimantIdsWithExpiredDutyOfCareToScheduleEmailToTheApprover";
        
        /// <summary>
        /// Procedure to get the Email details of Claimants whose Duty Of Care Documents are due to expire.
        /// </summary>
        private const string StoredProcGetClaimantIdsWithExpiredDutyOfCare = "GetClaimantIdsWithExpiredDutyOfCare";

        /// <summary>
        /// Procedure to get the Email details of Claimants whose Driving Licence review is due to expire.
        /// </summary>
        private const string StoredProcGetExpiredDrivingLicenceReview = "GetExpiredDrivingLicenceReview";

        /// <summary>
        /// Gets the list of claimants whose Duty of Care document is due to expire and need to notify Line manager.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <returns>
        /// list of Claimant details
        /// </returns>
        public static List<ClaimantDutyOfCareDetails> GetClaimantIdsForExpiredDOCForLineManager(int accountid)
        {
            var claimantDetails = new List<ClaimantDutyOfCareDetails>();
                    _accountConnectionString = cAccounts.getConnectionString(accountid);
                    using (var connection = new DatabaseConnection(_accountConnectionString))
                    {
                        var ClaimantInfo = connection.GetProcDataSet(StoredProcGetClaimantIdsWithExpiredDutyOfCareForLineManager);
                        if (ClaimantInfo != null && ClaimantInfo.Tables.Count > 0 && ClaimantInfo.Tables[0].Rows.Count > 0)
                        {
                            string approver; 
                            int [] teamId; 
                            if (ClaimantInfo.Tables.Count > 1 && ClaimantInfo.Tables[1].Rows.Count > 0)
                            {
                                teamId = new int[ClaimantInfo.Tables[1].Rows.Count];
                                int teamCount = 0;
                                approver = "Team";
                                foreach(DataRow teamMembers in ClaimantInfo.Tables[1].Rows)
                                {
                                    teamId[teamCount++] = Convert.ToInt32(teamMembers["teamMembers"]);
                                }
                            }
                            else
                            {
                                approver = "Line Manager";
                                teamId = new int[] { };
                            }

                            foreach (DataRow claimantId in ClaimantInfo.Tables[0].Rows)
                            {
                                claimantDetails.Add(ClaimantDutyOfCareDetails.InitialiseClaimantIds(Convert.ToInt32(claimantId["EmployeeId"]), accountid, approver, teamId));
                            }
                            ClaimantInfo.Clear();
                        }
                    }
                
                return claimantDetails;
        }

        /// <summary>
        /// Gets the list of claimants whose Duty of Care document is due to expire.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <returns>
        /// list of Claimant details
        /// </returns>
        public static List<ClaimantDutyOfCareDetails> GetClaimantIdsForExpiredDutyOfCareDocuments(int accountid)
        {
                var claimantDetails = new List<ClaimantDutyOfCareDetails>();
                 _accountConnectionString = cAccounts.getConnectionString(accountid);
                    using (var connection = new DatabaseConnection(_accountConnectionString))
                    {
                        using (IDataReader ClaimantInfo = connection.GetReader(StoredProcGetClaimantIdsWithExpiredDutyOfCare))
                        {                          
                            while (ClaimantInfo.Read())
                            {
                                if (ClaimantInfo.GetInt32(0) > 0)

                                claimantDetails.Add(ClaimantDutyOfCareDetails.InitialiseClaimantIds(ClaimantInfo.GetInt32(0), accountid));

                            }
                        }
                    }

                return claimantDetails;
        }

        /// <summary>
        /// The get claimant information for expired driving licence reviews.
        /// </summary>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <returns>
        /// The List of expiry details of driving licence.
        /// </returns>
        public static List<DrivingLicenceReviewExpiry> GetClaimantInformationForExpiredDrivingLicenceReviews(int accountid)
        {
            var claimantDetails = new List<DrivingLicenceReviewExpiry>();
            _accountConnectionString = cAccounts.getConnectionString(accountid);

            using (var connection = new DatabaseConnection(_accountConnectionString))
            {
                using (IDataReader ClaimantInfo = connection.GetReader(StoredProcGetExpiredDrivingLicenceReview))
                {
                    while (ClaimantInfo.Read())
                    {
                        if (ClaimantInfo.GetInt32(0) > 0)
                        {
                            claimantDetails.Add(
                                new DrivingLicenceReviewExpiry()
                                    {
                                        EmployeeId = ClaimantInfo.GetInt32(ClaimantInfo.GetOrdinal("EmployeeId")),
                                        ReviewDate = ClaimantInfo.GetDateTime(ClaimantInfo.GetOrdinal("ReviewDate"))
                                    });
                        }
                    }
                }
            }

            return claimantDetails;
        }
    }

}
