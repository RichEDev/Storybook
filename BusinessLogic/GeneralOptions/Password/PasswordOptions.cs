namespace BusinessLogic.GeneralOptions.Password
{
    /// <summary>
    /// Defines a <see cref="PasswordOptions"/> and it's members
    /// </summary>
    public class PasswordOptions : IPasswordOptions
    {
        /// <summary>
        /// Gets or sets the password max retries
        /// </summary>
        public byte PwdMaxRetries { get; set; }

        /// <summary>
        /// Gets or sets the password expiry days
        /// </summary>
        public int PwdExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PasswordLength"/>
        /// </summary>
        public PasswordLength PwdConstraint { get; set; }

        /// <summary>
        /// Gets or sets the password length 1
        /// </summary>
        public int PwdLength1 { get; set; }

        /// <summary>
        /// Gets or sets the password length 2
        /// </summary>
        public int PwdLength2 { get; set; }

        /// <summary>
        /// Gets or sets the password must contain upper case
        /// </summary>
        public bool PwdMustContainUpperCase { get; set; }

        /// <summary>
        /// Gets or sets the password must contain numbers
        /// </summary>
        public bool PwdMustContainNumbers { get; set; }

        /// <summary>
        /// Gets or sets the password must contain symbol
        /// </summary>
        public bool PwdMustContainSymbol { get; set; }

        /// <summary>
        /// Gets or sets the password history number
        /// </summary>
        public int PwdHistoryNum { get; set; }

        /// <summary>
        /// Gets or sets the password expires
        /// </summary>
        public bool PwdExpires { get; set; }
    }
}
