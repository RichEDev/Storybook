namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using SpendManagementLibrary.Employees.DutyOfCare;
    using SpendManagementLibrary;
    using System.Diagnostics;
    using expenses.code;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    using Spend_Management.shared.code.DVLA;

    /// <summary>
    /// DutyOfCare class includes implementation needed for merging Greenlight code with Duty Of Care and also for sending email reminders.
    /// </summary>
    public class DutyOfCare : ChangeApproverPermissionBase
    {
        private const string ChangeViewPermissionStoredProcedure = "[ChangeDutyOfCareTeamViewAccessPermission]";
        private const string ChangeFormPermissionStoredProcedure = "[ChangeDutyOfCareTeamFormAccessPermission]";


        /// <summary>
        /// The create employee data table for email templates body.
        /// </summary>
        /// <param name="drivingLicenceData">
        /// The driving Licence Data.
        /// </param>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/> of employee details.
        /// </returns>
        private static DataTable CreateEmployeeDataTableForEmailTemplatesBody(DrivingLicenceReviewExpiry drivingLicenceData, int accountId)
        {
            var claimantDrivingLicenceReviewDetails = new DataTable();
            claimantDrivingLicenceReviewDetails.Clear();
            var subAccounts = new cAccountSubAccounts(accountId);
            var reqSubAccount = subAccounts.getFirstSubAccount();
            int frequencyCheckForReview = reqSubAccount.SubAccountProperties.DrivingLicenceReviewFrequency;
            claimantDrivingLicenceReviewDetails.Columns.Add("EmployeeId");
            claimantDrivingLicenceReviewDetails.Columns.Add("ReviewDate");
            claimantDrivingLicenceReviewDetails.Columns.Add("ExpiryDate");
            claimantDrivingLicenceReviewDetails.Columns.Add("FrequencyInMonths");
            claimantDrivingLicenceReviewDetails.Rows.Add(drivingLicenceData.EmployeeId, drivingLicenceData.ReviewDate.ToString("dd/MM/yyyy"), drivingLicenceData.ReviewDate.AddMonths(frequencyCheckForReview).ToString("dd/MM/yyyy"), DvlaConsentLookUp.BuildFrequencyMessage(frequencyCheckForReview.ToString()));
            return claimantDrivingLicenceReviewDetails;
        }


        #region Public Methods of Duty Of Care Email reminders
        /// <summary>
        /// Send email to Approver on claimants Duty Of Care documents expiry.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <returns>
        /// flag indicating success/failure of notification
        /// </returns>
        public bool NotifyApproverOnExpiryOfDOC(int accountId)
        {
            try
            {
                //get the list of claimants whose Duty Of Care documents have expired and need to notify Line manager
                DateTime filterDate = new DateTime();
                var claimantIds = DutyOfCareEmailReminder.GetClaimantIdsForExpiredDOCForLineManager(accountId);
                if (claimantIds == null || claimantIds.Count == 0)
                {
                    return false;
                }

                foreach (var claimantDetails in claimantIds)
                {
                    cAccountSubAccounts subAccounts = new cAccountSubAccounts(claimantDetails.AccountId);
                    cAccountSubAccount reqSubAccount = subAccounts.getFirstSubAccount();
                    if (reqSubAccount.SubAccountProperties.RemindApproverOnDOCDocumentExpiryDays != -1)
                    {
                        int reminderDays = reqSubAccount.SubAccountProperties.RemindApproverOnDOCDocumentExpiryDays;
                        filterDate = DateTime.Today.AddDays(reminderDays);
                    }
                    var notifications = new NotificationTemplates(claimantDetails.AccountId, claimantDetails.ClaimantId, string.Empty, 0, Modules.expenses);
                    int[] recipientsId = claimantDetails.Approver == "Line Manager" ? new int[] { Employee.Get(claimantDetails.ClaimantId, claimantDetails.AccountId).LineManager } : claimantDetails.TeamIds;
                    notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAsApproverWhenClaimantsDocIsAboutToExpire), claimantDetails.ClaimantId, recipientsId, filterfieldval: filterDate, filterFieldGuid: new Guid("9F6E275F-516E-4A71-A94C-0D3D26A77D38"));
                }
            }
            catch (Exception ex)
            {
                cEventlog.LogEntry(
                    ex.Message, 
                    true,
                    EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send email to claimants whose Duty Of Care documents have expired.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <returns>
        /// flag indicating success/failure of notification
        /// </returns>
        public bool NotifyClaimantOnExpiryOfDOC(int accountId)
        {
            try
            {
                //get the list of claimants whose Duty Of Care documents have expired
                DateTime filterDate = new DateTime();
                var claimantsEmailDetails = DutyOfCareEmailReminder.GetClaimantIdsForExpiredDutyOfCareDocuments(accountId);

                if (claimantsEmailDetails == null || claimantsEmailDetails.Count == 0)
                {
                    return false;
                }

                cAccountSubAccounts subAccounts = new cAccountSubAccounts(accountId);
                cAccountSubAccount reqSubAccount = subAccounts.getFirstSubAccount();
                if (reqSubAccount.SubAccountProperties.RemindApproverOnDOCDocumentExpiryDays != -1)
                {
                    int reminderDays = reqSubAccount.SubAccountProperties.RemindApproverOnDOCDocumentExpiryDays;
                    filterDate = DateTime.Today.AddDays(reminderDays);
                }

                var notifications = new NotificationTemplates(accountId, 0, string.Empty, 0, Modules.expenses);

                foreach (var claimant in claimantsEmailDetails)
                {
                    notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenADutyOfCareDocumentIsDueToExpire), claimant.ClaimantId, new[] { claimant.ClaimantId }, filterfieldval: filterDate, filterFieldGuid: new Guid("9F6E275F-516E-4A71-A94C-0D3D26A77D38"));
                }
            }

            catch (Exception ex)
            {
                cEventlog.LogEntry(
                    ex.Message, true,
                    EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// The notify claimant on expiry of driving licence.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool NotifyClaimantOnExpiryOfDrivingLicence(int accountId)
        {
            var claimantsEmailDetails = DutyOfCareEmailReminder.GetClaimantInformationForExpiredDrivingLicenceReviews(accountId);

            if (claimantsEmailDetails == null || claimantsEmailDetails.Count == 0)
            {
                cEventlog.LogEntry(
                    "Error while retrieving claimant details whose Driving licence document has expired",
                    true,
                    EventLogEntryType.Error);
                return false;
            }

            foreach (var claimant in claimantsEmailDetails)
            {
                var claimantDetails = CreateEmployeeDataTableForEmailTemplatesBody(claimant, accountId);
                var notifications = new NotificationTemplates(accountId, claimant.EmployeeId, string.Empty, 0, Modules.expenses);
                notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToClaimantWhenDrivingLicenceReviewIsDueToExpire), claimant.EmployeeId, new[] { claimant.EmployeeId }, EmployeeDetailsForBody: claimantDetails);
            }

            return true;
        }
        #endregion

        /// <summary>
        /// This will change the filters on a team view
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="dutyOfCareAproverOnForm"></param>
        public new void ChangeTeamViewFilters(int accountId, string dutyOfCareAproverOnForm)
        {
            this.Accountid = accountId;
            var newValue = this.CalculateDatabaseValue(dutyOfCareAproverOnForm);
            this.CallStoredProc(ChangeViewPermissionStoredProcedure, newValue);
        }

        /// <summary>
        /// This will change the filters on a team form
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="dutyOfCareAproverOnForm"></param>
        public new void ChangeTeamFormFilters(int accountId, string dutyOfCareAproverOnForm)
        {
            this.Accountid = accountId;
            var newValue = this.CalculateDatabaseValue(dutyOfCareAproverOnForm);
            this.CallStoredProc(ChangeFormPermissionStoredProcedure, newValue);
        }

        /// <summary>
        /// Gets the recipients for a duty of care email template
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="senderId">The Id of the sender</param>
        /// <returns>An array with the recipient Ids</returns>
        public int[] GetRecipientsForApproverEmails(int accountId, int senderId)
        {
            var accountSubAccounts = new cAccountSubAccounts(accountId);
            var accountProperties = accountSubAccounts.getFirstSubAccount().SubAccountProperties;

            var recipientIds = new List<int>();

            if (accountProperties.DutyOfCareApprover.ToLower() == "line manager")
            {
                recipientIds = GetApproverIdsBasedOnApproverType(
                    accountId,
                    "GetLineManagerOfAnEmployee",
                    "employeeId",
                    senderId,
                    "linemanager");
            }
            else
            {
                recipientIds = GetApproverIdsBasedOnApproverType(
                    accountId,
                    "GetTeamMemberIds",
                    "teamId",
                    int.Parse(accountProperties.DutyOfCareTeamAsApprover),
                    "employeeid");
            }

            // if there is no line manager set then send to main administrator
            if (recipientIds[0] == 0)
            {
                recipientIds[0] = accountProperties.MainAdministrator;
            }

            return recipientIds.ToArray();
        }

        private static List<int> GetApproverIdsBasedOnApproverType(int accountId, string storedProcedure, string parameterToPass, int value, string parameterToRetrieve, IDBConnection connection = null)
        {
            var recipientIds = new List<int>();

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue($"@{parameterToPass}", value);

                using (IDataReader reader = databaseConnection.GetReader(storedProcedure, CommandType.StoredProcedure))
                {
                    var approverOrdinal = reader.GetOrdinal(parameterToRetrieve);

                    while (reader.Read())
                    {
                        recipientIds.Add(reader.IsDBNull(approverOrdinal) ? 0 : reader.GetInt32(approverOrdinal));
                    }

                    reader.Close();
                }
            }

            return recipientIds;
        }

        /// <summary>
        /// This gets the Employee Id of the Vehicle attached to a Duty of care document.
        /// </summary>
        /// <param name="filterval">
        /// The filterval.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="hasVehicle">
        /// The account id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetOwnerIdForDutyOfCareDocument(int filterval, int accountId, bool hasVehicle, IDBConnection connection = null)
        {
            int ownerId = 0;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                databaseConnection.sqlexecute.Parameters.Clear();
                databaseConnection.sqlexecute.Parameters.AddWithValue($"@DoCEntityId", filterval);
                var storedProcedureName = hasVehicle ? "GetVehicleOwnerForDocumentID" : "GetEmployeeIDForDrivingLicence";
                using (IDataReader reader = databaseConnection.GetReader(storedProcedureName, CommandType.StoredProcedure))
                {
                    var ownerOrdinal = reader.GetOrdinal("employeeid");

                    while (reader.Read())
                    {
                        ownerId = reader.IsDBNull(ownerOrdinal) ? 0 : reader.GetInt32(ownerOrdinal);
                    }

                    reader.Close();
                }
            }

            return ownerId;
        }

        /// <summary>
        /// This will change the filters on a team view based on General option disable car outside start and end date
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="vehicleStatusFilter"></param>
        /// <param name="connection"></param>
        public void ChangeVehicleStatusFilter(int accountId,bool vehicleStatusFilter, IDBConnection connection = null)
        {           
            var newFilter = string.Empty;
            newFilter = vehicleStatusFilter == true ? "VehicleStartAndEndDate" : "VehicleStatus";
            using (var expdata = connection  ?? new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NewFilter", newFilter);
                expdata.ExecuteProc("ChangeDutyOfCareVehicleStatusFilter");
            }
        }
    }
}