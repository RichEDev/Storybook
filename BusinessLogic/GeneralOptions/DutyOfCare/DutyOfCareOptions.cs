namespace BusinessLogic.GeneralOptions.DutyOfCare
{
    public class DutyOfCareOptions : IDutyOfCareOptions
    {
        /// <summary>
        /// Gets or sets the block driving licence
        /// </summary>
        public bool BlockDrivingLicence { get; set; }

        /// <summary>
        /// Gets or set the block tax expiry
        /// </summary>
        public bool BlockTaxExpiry { get; set; }

        /// <summary>
        /// Gets or sets the block MOT expiry
        /// </summary>
        public bool BlockMOTExpiry { get; set; }

        /// <summary>
        /// Gets or sets the block insurance expiry
        /// </summary>
        public bool BlockInsuranceExpiry { get; set; }

        /// <summary>
        /// Gets or sets the block breakdown cover expiry
        /// </summary>
        public bool BlockBreakdownCoverExpiry { get; set; }

        /// <summary>
        /// Gets or sets the enable driving licence review
        /// </summary>
        public bool EnableDrivingLicenceReview { get; set; }

        /// <summary>
        /// Gets or set the driving licence review reminder
        /// </summary>
        public bool DrivingLicenceReviewReminder { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review reminder days
        /// </summary>
        public byte DrivingLicenceReviewReminderDays { get; set; }

        /// <summary>
        /// Gets or sets the use date for duty of care checks
        /// </summary>
        public bool UseDateOfExpenseForDutyOfCareChecks { get; set; }

        /// <summary>
        /// Gets or sets the frequency of consent reminders lookup
        /// </summary>
        public string FrequencyOfConsentRemindersLookup { get; set; }

        /// <summary>
        /// Gets or set the enable automatic driving licence lookup
        /// </summary>
        public bool EnableAutomaticDrivingLicenceLookup { get; set; }

        /// <summary>
        /// Gets or sets the driving licence lookup frequency
        /// </summary>
        public string DrivingLicenceLookupFrequency { get; set; }

        /// <summary>
        /// Gets or sets the remind claimant on DOC documents expiry days
        /// </summary>
        public int RemindClaimantOnDOCDocumentExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the remind the approver on DOC document expiry days
        /// </summary>
        public int RemindApproverOnDOCDocumentExpiryDays { get; set; }

        /// <summary>
        /// Gets or sets the driving licence review frequency
        /// </summary>
        public int DrivingLicenceReviewFrequency { get; set; }

        /// <summary>
        /// Gets or sets the duty of care approver
        /// </summary>
        public string DutyOfCareApprover { get; set; }

        /// <summary>
        /// Gets or set the duty of care team as approver
        /// </summary>
        public string DutyOfCareTeamAsApprover { get; set; }
    }
}
