namespace SpendManagementLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Web.UI.WebControls;
    using Helpers;

    using SpendManagementLibrary.Enumerators;

    [Serializable]
    public enum SignoffType
    {
        /// <summary>
        /// No SignoffType
        /// </summary>
        [Display(Name = "None")]
        None = 0,

        /// <summary>
        /// Budget Holder
        /// </summary>
        [Display(Name = "Budget Holder")]
        BudgetHolder = 1,

        /// <summary>
        /// Employee
        /// </summary>
        [Display(Name = "Employee")]
        Employee = 2,

        /// <summary>
        /// Team
        /// </summary>
        [Display(Name = "Team")]
        Team = 3,

        /// <summary>
        /// Line Manager
        /// </summary>
        [Display(Name = "Line Manager")]
        LineManager = 4,

        /// <summary>
        /// Determined by Claimant
        /// </summary>
        [Display(Name = "Determined by Claimant")]
        ClaimantSelectsOwnChecker = 5,

        /// <summary>
        /// Approval Matrix
        /// </summary>
        [Display(Name = "Approval Matrix")]
        ApprovalMatrix = 6,

        /// <summary>
        /// Determined by Claimant from Approval Matrix
        /// </summary>
        [Display(Name = "Determined by Claimant from Approval Matrix")]
        DeterminedByClaimantFromApprovalMatrix = 7,

        /// <summary>
        /// Cost Code Owner
        /// </summary>
        [Display(Name = "Cost Code Owner")]
        CostCodeOwner = 8,

        /// <summary>
        /// Assignment Supervisor
        /// </summary>
        [Display(Name = "Assignment Supervisor")]
        AssignmentSignOffOwner = 9,

        /// <summary>
        /// SEL Scan & Attach
        /// </summary>
        [Display(Name = "Scan & Attach")]
        SELScanAttach = 100,

        /// <summary>
        /// SEL Validation
        /// </summary>
        [Display(Name="Validation")]
        SELValidation = 101

    }

    [Serializable]
    public class cStage
    {
        /// <summary>
        /// The default validation correction threshold for pay before validate.
        /// </summary>
        public const int DefaultValidationCorrectionThreshold = 10;

        public cStage(int signoffId, SignoffType signoffType, int relId, int extraApprovalLevels, StageInclusionType include, decimal amount, int notify, byte stage, byte onHoliday, SignoffType holidayType, int holidayId, int includeId, bool claimantMail, bool singleSignOff, bool sendMail, bool displayDeclaration, DateTime createdOn, int createdBy, DateTime modifiedOn, int modifiedBy, bool fromMyLevelOnly, bool approverJustificationsRequired, bool nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner, bool allocateForPayment, bool isPostValidationCleanupStage, int? validationCorrectionThreshold)
        {
            this.signoffid = signoffId;
            this.signofftype = signoffType;
            this.relid = relId;
            this.ExtraApprovalLevels = extraApprovalLevels;
            this.include = (allocateForPayment ? StageInclusionType.Always : include);
            this.amount = amount;
            this.notify = (allocateForPayment ? 2 : notify);
            this.stage = stage;
            this.onholiday = onHoliday;
            this.holidaytype = holidayType;
            this.holidayid = holidayId;
            this.includeid = includeId;
            this.claimantmail = claimantMail;
            this.singlesignoff = (!allocateForPayment && singleSignOff);
            this.sendmail = sendMail;
            this.displaydeclaration = displayDeclaration;
            this.createdon = createdOn;
            this.createdby = createdBy;
            this.modifiedon = modifiedOn;
            this.modifiedby = modifiedBy;
            this.FromMyLevel = fromMyLevelOnly;
            this.ApproverJustificationsRequired = approverJustificationsRequired;
            this.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = nhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner;
            this.AllocateForPayment = allocateForPayment;
            this.IsPostValidationCleanupStage = isPostValidationCleanupStage;
            this.ValidationCorrectionThreshold = validationCorrectionThreshold;
        }

        public int signoffid { get; private set; }

        public SignoffType signofftype { get; private set; }

        public int relid { get; private set; }

        public int ExtraApprovalLevels { get; private set; }

        public StageInclusionType include { get; private set; }

        public decimal amount { get; private set; }

        public int notify { get; private set; }

        public byte stage { get; private set; }

        public byte onholiday { get; private set; }

        public SignoffType holidaytype { get; private set; }

        public int holidayid { get; private set; }

        public int includeid { get; private set; }

        public bool claimantmail { get; private set; }

        public bool singlesignoff { get; private set; }

        public bool sendmail { get; private set; }

        public bool displaydeclaration { get; private set; }

        public DateTime createdon { get; private set; }

        public int createdby { get; private set; }

        public DateTime modifiedon { get; private set; }

        public int modifiedby { get; private set; }

        public bool FromMyLevel { get; set; }

        public bool ApproverJustificationsRequired { get; set; }

        public bool NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner { get; set; }

        /// <summary>
        /// In a pay-before-validate scenario, determines whether an approver can allocate an expense for payment at this stage.
        /// If the expense gets allocated, then future changes to it must be reflected in financial exports, validation and other areas.
        /// This property, when true, basically enables an 'Allocate for Payment' link in the menu on the 'CheckExpenseList' page.
        /// Only one stage in a group may have this property set to true; validation is done on the group save. This stage must appear
        /// at some point before SELValidation in the group, which in turn must be the penultimate stage, before a <see cref="IsPostValidationCleanupStage"/>. 
        /// </summary>
        public bool AllocateForPayment { get; set; }

        /// <summary>
        /// In a pay-before-validate scenario, a stage that requires an approver to check the claim's expenses must be the last stage,
        /// with the SELValidation stage being the penultimate. This is enforced on the saving of the group. 
        /// This property indicates to the system that approved or revalidated expenses will be sent for correction and financial export,
        /// or be sent for a manual check by the approver defined for this stage.
        /// </summary>
        public bool IsPostValidationCleanupStage { get; set; }

        /// <summary>
        /// In a pay-before-validate scenario, items that have either failed validation twice, or failed once and then succeeded have
        /// an extra check made before they are sent for payment. This happens if this stage has <see cref="IsPostValidationCleanupStage"/> 
        /// set to true. It will be the last stage if so. If the value of the expense is outside this threshold (which is a percentage), 
        /// then the item will be sent to this stage's approver to make the decision.
        /// </summary>
        public int? ValidationCorrectionThreshold { get; set; }

        public cGroup GetGroup(SortedList list)
        {
            return this.GetGroup(list.Values.OfType<cGroup>());
        }

        public cGroup GetGroup(IEnumerable<cGroup> groups)
        {
            return groups.FirstOrDefault(g => g.stages.ContainsKey(this.signoffid));
        }

    }

    /// <summary>
    /// Extension methods for the SignoffType enum.
    /// </summary>
    public static class SignoffTypeExtensions
    {
        /// <summary>
        /// Gets the value of the [Display(Name="")] tag, 
        /// given a SignoffType enum member.
        /// </summary>
        /// <param name="signoffType">The value of the enum.</param>
        /// <returns>A string display name.</returns>
        public static string GetDisplayValue(this SignoffType signoffType)
        {
            return EnumHelpers<SignoffType>.GetDisplayValue(signoffType);
        }

        /// <summary>
        /// Generates a <see cref="ListItem"/>, based on the [Display(Name="")] tag 
        /// and the value of a SignoffType enum member.
        /// </summary>
        /// <param name="signoffType">The value of the enum.</param>
        /// <returns>A <see cref="ListItem"/>.</returns>
        public static ListItem ToListItem(this SignoffType signoffType)
        {
            var display = GetDisplayValue(signoffType);
            if (!string.IsNullOrWhiteSpace(display))
            {
                return new ListItem(display, ((int)signoffType).ToString(CultureInfo.InvariantCulture));
            }
            throw new MethodAccessException(@"Ensure the enum you are passing in is decorated with a [Display(Name="")] attribute with a meaningful value.");
        }
    }

}
