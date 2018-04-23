using System;

namespace SpendManagementLibrary.Helpers.AuditLogger
{
    /// <summary>
    /// The audit logger class
    /// </summary>
    public class AuditLogger : IAuditLogger
    {
        /// <summary>
        /// Records a view action in the audit log.
        /// </summary>
        /// <param name="currentUser">The <see cref="ICurrentUserBase"/>.</param>
        /// <param name="element">The <see cref="SpendManagementElement"/>.</param>
        /// <param name="viewEvent">The details of the view event.</param>
        public void ViewRecordAuditLog(ICurrentUserBase currentUser, SpendManagementElement element, string viewEvent)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", currentUser.EmployeeID);

                if (currentUser.isDelegate)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                expdata.sqlexecute.Parameters.AddWithValue("@elementid", (int) element);
                expdata.sqlexecute.Parameters.AddWithValue("@recordTitle", viewEvent);
                expdata.sqlexecute.Parameters.AddWithValue("@subAccountId", currentUser.CurrentSubAccountId);
                expdata.ExecuteProc("AddViewEntryToAuditLog");
                expdata.sqlexecute.Parameters.Clear();
            }
        }
    }
}
