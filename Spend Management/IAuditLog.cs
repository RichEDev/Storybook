using System;

namespace Spend_Management
{
    public interface IAuditLog
    {
        void editRecord(int id, string item, SpendManagementElement element, Guid fieldID, string oldvalue, string newvalue);
        
        /// <summary>
        /// Add a view record entry to the audit log
        /// </summary>
        /// <param name="element">The category for the audit log.</param>
        /// <param name="value">The record being viewed.</param>
        /// <param name="currentUser">The current user.</param>
        void ViewRecord(SpendManagementElement element, string value, ICurrentUser currentUser);
    }
}
