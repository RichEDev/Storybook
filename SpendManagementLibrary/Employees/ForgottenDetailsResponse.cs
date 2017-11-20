namespace SpendManagementLibrary
{
    /// <summary>
    /// Potential outcomes from the RequestForgottenDetails method
    /// </summary>
    public enum ForgottenDetailsResponse
    {
        /// <summary>
        /// Sorry, the email address you have entered is not unique so your logon details cannot be sent there. Please call your administrator for assistance.
        /// </summary>
        EmailNotUnique = 1,

        /// <summary>
        /// Sorry, the email address you have entered does not exist.  Please call your administrator for assistance.
        /// </summary>
        EmailNotFound = 2,

        /// <summary>
        /// Your account is currently locked, please call your administrator to un-archive your account and reset your password.
        /// </summary>
        ArchivedEmployee = 3,

        /// <summary>
        /// Your account is currently waiting to be approved, please call your administrator to have your account activated.
        /// </summary>
        InactiveEmployee = 4,

        /// <summary>
        /// Thank you, you will shortly receive an email with instructions on how to reset your password.
        /// </summary>
        EmployeeDetailsSent = 5,

        /// <summary>
        /// Employee account has been locked by Admin or by too many login attempts.
        /// </summary>
        EmployeeLocked = 6
    }
}
