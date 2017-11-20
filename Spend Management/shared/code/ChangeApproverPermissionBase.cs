namespace Spend_Management.shared.code
{
    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    
    /// <summary>
    /// This base class can be used to change the approver values for Greenlights
    /// </summary>
    public class ChangeApproverPermissionBase
    {
        internal int Accountid;

        internal string CalculateDatabaseValue(string dutyOfCareAproverOnForm)
        {
            var newValue = string.Empty;

            if (dutyOfCareAproverOnForm.ToLower() == "line manager")
            {
                newValue = "@MY_HIERARCHY";
            }

            return newValue;
        }

        internal void CallStoredProc(string storedProcedure, string paramValue, IDBConnection connection = null)
        { 
            using (var expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.Accountid)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NewValue", paramValue);
                expdata.ExecuteProc(storedProcedure);
            }
        }

        /// <summary>
        /// This will change the filters on a team view
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="dutyOfCareAproverOnForm"></param>
        public virtual void ChangeTeamViewFilters(int accountId, string dutyOfCareAproverOnForm) { }

        /// <summary>
        /// This will change the filters on a team view
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="dutyOfCareAproverOnForm"></param>
        public virtual void ChangeTeamFormFilters(int accountId, string dutyOfCareAproverOnForm) { }
    }
}