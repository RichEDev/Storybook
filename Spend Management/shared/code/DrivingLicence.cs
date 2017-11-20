namespace Spend_Management.shared.code
{
    public class DrivingLicence : ChangeApproverPermissionBase
    {
        private const string ChangeViewPermissionStoredProcedure = "[ChangeDrivingLicenceTeamViewAccessPermission]";
        private const string ChangeFormPermissionStoredProcedure = "[ChangeDrivingLicenceTeamFormAccessPermission]";

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