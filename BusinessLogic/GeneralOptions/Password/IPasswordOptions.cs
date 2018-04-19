namespace BusinessLogic.GeneralOptions.Password
{
    /// <summary>
    /// Defines a <see cref="IPasswordOptions"/> and it's members
    /// </summary>
    public interface IPasswordOptions
    {
        /// <summary>
        /// Gets or sets the password max retries
        /// </summary>
        byte PwdMaxRetries { get; set; }

        /// <summary>
        /// Gets or sets the password expiry days
        /// </summary>
        int PwdExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PasswordLength"/>
        /// </summary>
        PasswordLength PwdConstraint { get; set; }

        /// <summary>
        /// Gets or sets the password length 1
        /// </summary>
        int PwdLength1 { get; set; }

        /// <summary>
        /// Gets or sets the password length 2
        /// </summary>
        int PwdLength2 { get; set; }

        /// <summary>
        /// Gets or sets the password must contain upper case
        /// </summary>
        bool PwdMustContainUpperCase { get; set; }

        /// <summary>
        /// Gets or sets the password must contain numbers
        /// </summary>
        bool PwdMustContainNumbers { get; set; }

        /// <summary>
        /// Gets or sets the password must contain symbol
        /// </summary>
        bool PwdMustContainSymbol { get; set; }

        /// <summary>
        /// Gets or sets the password history number
        /// </summary>
        int PwdHistoryNum { get; set; }

        /// <summary>
        /// Gets or sets the password expires
        /// </summary>
        bool PwdExpires { get; set; }
    }
}
