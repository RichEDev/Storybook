namespace BusinessLogic.GeneralOptions.Delegates
{
    /// <summary>
    /// Defines a <see cref="IDelegateOptions"/> and it's members
    /// </summary>
    public class DelegateOptions : IDelegateOptions
    {
        /// <summary>
        /// Gets or sets delegate setup.
        /// </summary>
        public bool DelSetup { get; set; }

        /// <summary>
        /// Gets or sets delegate employee admin.
        /// </summary>
        public bool DelEmployeeAdmin { get; set; }

        /// <summary>
        /// Gets or sets delegate employee accounts.
        /// </summary>
        public bool DelEmployeeAccounts { get; set; }

        /// <summary>
        /// Gets or sets delegate reports.
        /// </summary>
        public bool DelReports { get; set; }

        /// <summary>
        /// Gets or sets delegate reports claimants.
        /// </summary>
        public bool DelReportsClaimants { get; set; }

        /// <summary>
        /// Gets or sets delegate check and pay.
        /// </summary>
        public bool DelCheckAndPay { get; set; }

        /// <summary>
        /// Gets or sets delegate QE design.
        /// </summary>
        public bool DelQEDesign { get; set; }

        /// <summary>
        /// Gets or sets delegate corporate cards.
        /// </summary>
        public bool DelCorporateCards { get; set; }

        /// <summary>
        /// Gets or sets delegate approvals.
        /// </summary>
        public bool DelApprovals { get; set; }

        /// <summary>
        /// Gets or sets delegate exports.
        /// </summary>
        public bool DelExports { get; set; }

        /// <summary>
        /// Gets or sets delegate audit log.
        /// </summary>
        public bool DelAuditLog { get; set; }

        /// <summary>
        /// Gets or sets delegate submit claim.
        /// </summary>
        public bool DelSubmitClaim { get; set; }

        /// <summary>
        /// Gets or sets delegate options for delegate access role.
        /// </summary>
        public bool EnableDelegateOptionsForDelegateAccessRole { get; set; }
    }
}
