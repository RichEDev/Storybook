namespace BusinessLogic.GeneralOptions.DutyOfCare
{
    /// <summary>
    /// Defines a <see cref="IDutyOfCareOptions"/> and it's members
    /// </summary>
    public interface IDutyOfCareOptions
    {
        /// <summary>
        /// Gets or sets the block driving licence
        /// </summary>
        bool BlockDrivingLicence { get; set; }

        /// <summary>
        /// Gets or set the block tax expiry
        /// </summary>
        bool BlockTaxExpiry { get; set; }

        /// <summary>
        /// Gets or sets the block MOT expiry
        /// </summary>
        bool BlockMOTExpiry { get; set; }

        /// <summary>
        /// Gets or sets the block insurance expiry
        /// </summary>
        bool BlockInsuranceExpiry { get; set; }

        /// <summary>
        /// Gets or sets the block breakdown cover expiry
        /// </summary>
        bool BlockBreakdownCoverExpiry { get; set; }

        /// <summary>
        /// Gets or sets the enable driving licence review
        /// </summary>
        bool EnableDrivingLicenceReview { get; set; }
        
        /// <summary>
        /// Gets or set the driving licence review reminder
        /// </summary>
        bool DrivingLicenceReviewReminder { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review reminder days
        /// </summary>
        byte DrivingLicenceReviewReminderDays { get; set; }

        /// <summary>
        /// Gets or sets the use date for duty of care checks
        /// </summary>
        bool UseDateOfExpenseForDutyOfCareChecks { get; set; }

        /// <summary>
        /// Gets or sets the frequency of consent reminders lookup
        /// </summary>
        string FrequencyOfConsentRemindersLookup { get; set; }

        /// <summary>
        /// Gets or set the enable automatic driving licence lookup
        /// </summary>
        bool EnableAutomaticDrivingLicenceLookup { get; set; }

        /// <summary>
        /// Gets or sets the driving licence lookup frequency
        /// </summary>
        string DrivingLicenceLookupFrequency { get; set; }

        /// <summary>
        /// Gets or sets the remind claimant on DOC documents expiry days
        /// </summary>
        int RemindClaimantOnDOCDocumentExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the remind the approver on DOC document expiry days
        /// </summary>
        int RemindApproverOnDOCDocumentExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review frequency
        /// </summary>
        int DrivingLicenceReviewFrequency { get; set; }
        
        /// <summary>
        /// Gets or sets the duty of care approver
        /// </summary>
        string DutyOfCareApprover { get; set; }

        /// <summary>
        /// Gets or set the duty of care team as approver
        /// </summary>
        string DutyOfCareTeamAsApprover { get; set; }
    }
}
