namespace Spend_Management.shared.code
{
    using System.ComponentModel;

    /// <summary>
    /// The email templates related to claimant duty of care
    /// </summary>
    public enum DutyOfCareClaimantEmailTemplates
    {
        /// <summary>
        /// Notify claimant if the approver is unhappy with the vehicle insurance document
        /// </summary>
        [Description("9F2D2730-E054-446C-B395-CEED12D15577")]
        NotifyClaimantIfInsuranceDocumentFailsApproval = 1,

        /// <summary>
        /// Notify claimant if the approver is unhappy with the MOT document
        /// </summary>
        [Description("9677B1AB-B0CA-463F-9A1B-CCE72865A576")]
        NotifyClaimantIfMotDocumentFailsApproval = 2,

        /// <summary>
        /// Notify claimant if the approver is unhappy with the vehicle tax document
        /// </summary>
        [Description("35654985-270E-4F59-94DB-3F585FFFFE9E")]
        NotifyClaimantIfTaxDocumentFailsApproval = 3,

        /// <summary>
        /// Notify claimant if the approver is unhappy with the vehicle service document
        /// </summary>
        [Description("C55B5DD7-8A7A-4E50-94B7-E9CADD8F0B1B")]
        NotifyClaimantIfServiceDocumentFailsApproval = 4,

        /// <summary>
        /// Notify claimant if the approver is unhappy with the vehicle breakdown cover document
        /// </summary>
        [Description("8070FC86-7F31-4D99-84C3-CC1FCC0B8651")]
        NotifyClaimantIfBreakdownCoverFailsApproval = 5,
        
        /// <summary>
        /// Notify claimant if the approver is unhappy with the vehicle breakdown cover document
        /// </summary>
        [Description("2D929DDA-C076-4AAD-A1A7-5531A9CA57EF")]
        NotifyClaimantIfDocumentApproved = 6
    }


    /// <summary>
    /// The email templates related to approver duty of care
    /// </summary>
    public enum DutyOfCareApproverEmailTemplates
    {
        /// <summary>
        /// Send to an approver when claimant adds a new insurance document
        /// </summary>
        [Description("F301C44D-D013-4DCA-B9A2-FD5FACD200AB")]
        NotifyApproverOfNewInsuranceDocument = 1,

        /// <summary>
        /// Send to an approver when claimant adds a new tax document
        /// </summary>
        [Description("172F62C2-F405-4C9C-9B01-2BAE99905D6A")]
        NotifyApproverOfNewTaxDocument = 2,

        /// <summary>
        /// Send to an approver when claimant adds a new driving licence document
        /// </summary>
        [Description("3A52CBC9-FE4C-46D0-B5F5-1E6615933C10")]
        NotifyApproverOfNewDrivingLicence = 3,

        /// <summary>
        /// Send to an approver when claimant adds a new MOT document
        /// </summary>
        [Description("55FE9178-D440-4E23-A91F-AACC3BD106E5")]
        NotifyApproverOfNewMotDocument = 4,

        /// <summary>
        /// Send to an approver when claimant adds a new Service document
        /// </summary>
        [Description("090ADB58-44CD-4440-A853-4707F523160B")]
        NotifyApproverOfNewServiceDocument = 5,

        /// <summary>
        /// Send to an approver when claimant adds a new breakdown cover document
        /// </summary>
        [Description("273F83A4-8E6D-479C-8610-F299932993BD")]
        NotifyApproverOfNewBreakdownCoverDocument = 6,

        /// <summary>
        /// Send to an approver when claimant modifies the DOC document
        /// </summary>
        [Description("922924E5-D63D-4332-B8A7-74CE8564D154")]
        NotifyApproverIfClaimantUpdatesDocument = 7,

        /// <summary>
        /// Send to an approver when claimant modifies the DOC Driving Licence document
        /// </summary>
        [Description("F1C3673F-B1B8-4FC7-AF9C-AC235B35D1D9")]
        NotifyApproverIfClaimantUpdatesDrivingLicenceDocument = 8
    }

    /// <summary>
    /// The email templates related to claimant duty of care driving licence
    /// </summary>
    public enum DutyOfCareDrivingLicenceClaimantEmailTemplates
    {
        /// <summary>
        /// Notify claimant if the approver is unhappy with the driving licence document
        /// </summary>
        [Description("4B82EE25-BF61-4072-A7CC-98DAC4FDE392")]
        NotifyClaimantIfDrivingLicenceFailsApproval = 1,

        /// <summary>
        /// Notify claimant if the approver is unhappy with the driving licence document
        /// </summary>
        [Description("04302FFB-33B4-468E-98D3-DD9EF67E9C20")]
        NotifyClaimantIfDrivingLicenceApproved = 2
    }
}