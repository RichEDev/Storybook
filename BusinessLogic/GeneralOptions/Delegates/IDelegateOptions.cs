namespace BusinessLogic.GeneralOptions.Delegates
{
    /// <summary>
    /// Defines a <see cref="IDelegateOptions"/> and it's members
    /// </summary>
    public interface IDelegateOptions
    {
        /// <summary>
        /// Gets or sets delegate setup.
        /// </summary>
        bool DelSetup { get; set; }

        /// <summary>
        /// Gets or sets delegate employee admin.
        /// </summary>
        bool DelEmployeeAdmin { get; set; }

        /// <summary>
        /// Gets or sets delegate employee accounts.
        /// </summary>
        bool DelEmployeeAccounts { get; set; }

        /// <summary>
        /// Gets or sets delegate reports.
        /// </summary>
        bool DelReports { get; set; }

        /// <summary>
        /// Gets or sets delegate reports claimants.
        /// </summary>
        bool DelReportsClaimants { get; set; }

        /// <summary>
        /// Gets or sets delegate check and pay.
        /// </summary>
        bool DelCheckAndPay { get; set; }

        /// <summary>
        /// Gets or sets delegate QE design.
        /// </summary>
        bool DelQEDesign { get; set; }

        /// <summary>
        /// Gets or sets delegate corporate cards.
        /// </summary>
        bool DelCorporateCards { get; set; }

        /// <summary>
        /// Gets or sets delegate approvals.
        /// </summary>
        bool DelApprovals { get; set; }

        /// <summary>
        /// Gets or sets delegate exports.
        /// </summary>
        bool DelExports { get; set; }

        /// <summary>
        /// Gets or sets delegate audit log.
        /// </summary>
        bool DelAuditLog { get; set; }

        /// <summary>
        /// Gets or sets delegate submit claim.
        /// </summary>
        bool DelSubmitClaim { get; set; }

        /// <summary>
        /// Gets or sets delegate options for delegate access role.
        /// </summary>
        bool EnableDelegateOptionsForDelegateAccessRole { get; set; }
    }
}
