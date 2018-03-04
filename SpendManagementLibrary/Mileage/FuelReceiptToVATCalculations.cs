namespace SpendManagementLibrary.Mileage
{
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary;
    using System;

    public class FuelReceiptToVATCalculations
    {
        #region Constructor
        public FuelReceiptToVATCalculations()
        {

        }
        #endregion

       
        /// <summary>
        /// Method to call the ProcessFuelReceiptToMileageAllocations procedure
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public int ProcessFuelReceiptToMileageAllocations(int accountId)
        {
            int isUpdated = 0;
            try
            {
                var accountConnectionString = cAccounts.getConnectionString(accountId);
                using (var connection = new DatabaseConnection(accountConnectionString))
                {
                    connection.sqlexecute.CommandTimeout = 3600;
                    connection.ExecuteProc("ProcessFuelReceiptToMileageAllocations");
                    isUpdated = 1;
                    connection.ClearParameters();
                }
            }
            catch
            {
                isUpdated = 0;
            }
            return isUpdated;
        }

        /// <summary>
        /// Add the audit log for Vat calculation 
        /// </summary>
        /// <param name="accountId">Account Id for which the audit log is made</param>
        /// <param name="employeeId">Employee Id of the current api user</param>
        /// <param name="element">Element for which the audit is logged</param>
        /// <param name="newvalue">Description of the log</param>
        /// <param name="currentSubAccountId">Current SubAccountId</param>
        public void AddAuditLog(int accountId,int employeeId,SpendManagementElement element, string newvalue, int currentSubAccountId,string accountConnectionString)
        {               
                using (var expdata = new DatabaseConnection(accountConnectionString))
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int)element);
                    expdata.sqlexecute.Parameters.AddWithValue("@entityid", accountId);
                    expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", newvalue);
                    expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", currentSubAccountId);
                    expdata.ExecuteProc("addInsertEntryToAuditLog");                 
                    expdata.sqlexecute.Parameters.Clear();
                }                      
        }
    }
}
