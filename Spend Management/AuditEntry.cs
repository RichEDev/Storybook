namespace Spend_Management
{
    using System;

    /// <summary>
    /// Individual audit log entry class
    /// </summary>
    public class AuditEntry
    {
        private int _auditLogId;
        private string _companyId;
        private string _username;
        private DateTime _dateStamp;
        private char _action;
        private string _category;
        private string _oldValue;
        private string _newValue;

        /// <summary>
        /// Audit log entry constructor
        /// </summary>
        /// <param name="auditLogId">The ID of the Audit Log item</param>
        /// <param name="companyId">the company ID</param>
        /// <param name="userName">The Current Users Name</param>
        /// <param name="dateStamp">The <see cref="DateTime"/>Stamp</param>
        /// <param name="action">The log action</param>
        /// <param name="category">The Category of the audit log item</param>
        /// <param name="oldValue">The old value (if any)</param>
        /// <param name="newValue">The new value (if any)</param>
        public AuditEntry(int auditLogId, string companyId, string userName, DateTime dateStamp, char action, string category, string oldValue, string newValue)
        {
            this._auditLogId = auditLogId;
            this._companyId = companyId;
            this._username = userName;
            this._dateStamp = dateStamp;
            this._action = action;
            this._category = category;
            this._oldValue = oldValue;
            this._newValue = newValue;
        }
    }
}