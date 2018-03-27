namespace SpendManagementLibrary.Helpers.AuditLogger
{
    /// <summary>
    /// The IAuditLogger Interface
    /// </summary>
    public interface IAuditLogger
    {
        /// <summary>
        /// Records a view action in the audit log.
        /// </summary>
        /// <param name="currentUser">The <see cref="ICurrentUserBase"/>.</param>
        /// <param name="element">The <see cref="SpendManagementElement"/>.</param>
        /// <param name="viewEvent">The details of the view event.</param>
        void ViewRecordAuditLog(ICurrentUserBase currentUser, SpendManagementElement element, string viewEvent);
    }
}