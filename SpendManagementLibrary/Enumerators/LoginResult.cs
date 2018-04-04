namespace SpendManagementLibrary.Enumerators
{
    /// <summary>
    /// The login result enumerator.
    /// </summary>
    public enum LoginResult
    {
        /// <summary>
        /// The success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The incorrect company name.
        /// </summary>
        IncorrectCompanyName = 1,

        /// <summary>
        /// The invalid username password combo.
        /// </summary>
        InvalidUsernamePasswordCombo = 2,

        /// <summary>
        /// The account archived.
        /// </summary>
        AccountArchived = 3,

        /// <summary>
        /// The employee archived.
        /// </summary>
        EmployeeArchived = 4,

        /// <summary>
        /// The change password.
        /// </summary>
        ChangePassword = 5,

        /// <summary>
        /// The enter odometer values.
        /// </summary>
        EnterOdometerValues = 6,

        /// <summary>
        /// The inactive account.
        /// </summary>
        InactiveAccount = 7,

        /// <summary>
        /// The logon attempts exceeded.
        /// </summary>
        LogonAttemptsExceeded = 8,

        /// <summary>
        /// The no sub account.
        /// </summary>
        NoSubAccount = 9,

        /// <summary>
        /// The invalid IP address.
        /// </summary>
        InvalidIPAddress = 10,

        /// <summary>
        /// Employee locked.
        /// </summary>
        EmployeeLocked = 11,

        /// <summary>
        /// The account does not have mobile device activacted
        /// </summary>
        MobileDevicesDeactivated = 12,

        /// <summary>
        /// Indicates that the employee does not sufficent access role permissions
        /// </summary>
        EmployeeHasInsufficientPermissions = 13,

        /// <summary>
        /// Indicates that the password reset key has failed validation
        /// </summary>
        PasswordResetKeyInvalid = 14,

        /// <summary>
        /// A valid current user has been found so the user is already logged in.
        /// </summary>
        AlreadyLoggedIn = 15
    }
}
