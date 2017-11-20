namespace Spend_Management.shared.code
{
    /// <summary>
    /// DutyOfCare class includes implementation needed for merging Greenlight code with Duty Of Care and also for sending email reminders.
    /// </summary>
    public class DrivingLicenceReview : ChangeApproverPermissionBase
    {
        private const string ChangeViewPermissionStoredProcedure = "[ChangeDrivingLicenceReviewTeamViewAccessPermission]";
        private const string ChangeFormPermissionStoredProcedure = "[ChangeDrivingLicenceReviewTeamFormAccessPermission]";

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
    }
}