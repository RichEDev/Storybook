namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Globalization;

    using SpendManagementApi.Common.Enums;
    using Interfaces;
    using Common;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;

    using Spend_Management;
    using Spend_Management.shared.code.ApprovalMatrix;

    using SignoffType = SpendManagementApi.Common.Enums.SignoffType;
    using StageInclusionType = SpendManagementApi.Common.Enums.StageInclusionType;

    /// <summary>
    /// Defines a stage in a <see cref="SignOffGroup">SignOffGroup</see>.
    /// </summary>
    public class Stage : BaseExternalType, IRequiresValidation, IEquatable<Stage>
    {
        /// <summary>
        /// The unique Id of this stage.
        /// </summary>
        public int SignOffId { get; set; }

        /// <summary>
        /// The SignOffType of this stage.
        /// </summary>
        public SignoffType SignOffType { get; set; }

        /// <summary>
        /// The name or label of this stage.
        /// </summary>
        public byte SignOffStage { get; set; }


        /// <summary>
        /// The Id of this stage's related user.
        /// </summary>
        public int Relid { get; set; }

        /// <summary>
        /// The name of this stage's related user.
        /// </summary>
        public string RelSignOffPerson { get; internal set; }

        /// <summary>
        /// The InclusionType of this stage.
        /// </summary>
        public StageInclusionType StageInclusionType { get; set; }


        /// <summary>
        /// The amount of this stage.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The notify for this stage.
        /// </summary>
        public Notify Notify { get; set; }

        /// <summary>
        /// The action to be taken when the approver is on holiday.
        /// </summary>
        public HolidayProvision OnHolidayProvision { get; set; }


        /// <summary>
        /// The holiday delegate sign off type.
        /// </summary>
        public SignoffType HolidayType { get; set; }

        /// <summary>
        /// The specific id of holiday delegate type.
        /// </summary>
        public int HolidayId { get; set; }

        /// <summary>
        /// The holiday delegate name.
        /// </summary>
        public string HolidayDelegate { get; internal set; }

        /// <summary>
        /// The included item id of the inclusion type specified.
        /// </summary>
        public int IncludeId { get; set; }

        /// <summary>
        /// The included item description.
        /// </summary>
        public string IncludedItem
        {
            get
            {
                StageInclusionType includedType = this.StageInclusionType;
                if (this.AccountId.HasValue)
                {
                    switch (includedType)
                    {
                        case StageInclusionType.IncludesCostCode:
                            cCostcodes costcodes = new cCostcodes(this.AccountId.Value);
                            return costcodes.GetCostcodeById(this.IncludeId).Costcode;
                        case StageInclusionType.IncludesDepartment:
                            var departments = new cDepartments(this.AccountId.Value);
                            return departments.GetDepartmentById(this.IncludeId).Description;
                        case StageInclusionType.IncludesExpenseItem:
                            var clssubcats = new cSubcats(this.AccountId.Value);
                            return clssubcats.GetSubcatById(this.IncludeId).subcat;
                        default:
                            return string.Empty;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// The claimant mail of this stage.
        /// </summary>
        public bool ClaimantMail { get; set; }

        /// <summary>
        /// The single sign off of this stage.
        /// </summary>
        public bool SingleSignOff { get; set; }

        /// <summary>
        /// The send mail of this stage.
        /// </summary>
        public bool SendMail { get; set; }

        /// <summary>
        /// The display declaration of this stage.
        /// </summary>
        public bool DisplayDeclaration { get; set; }

        /// <summary>
        /// The employee id of this stage.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// The extra levels of this stage.
        /// </summary>
        public int ExtraLevels { get; set; }

        /// <summary>
        /// Whether to approve higher levels only for this stage.
        /// </summary>
        public bool ApproveHigherLevelsOnly { get; set; }

        /// <summary>
        /// Whether to approve via an NhsAssignmentSupervisor when the cost code owner is missing.
        /// </summary>
        internal bool NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner { get; set; }

        /// <summary>
        /// Flag this stage as to when payment should be allocated (cannot be set on the first stage)
        /// </summary>
        public bool AllocateForPayment { get; set; }

        /// <summary>
        /// Flag this stage as the auto-created post-validation stage
        /// </summary>
        public bool IsPostValidationCleanupStage { get; set; }

        /// <summary>
        /// Correction verification threshold percentage
        /// </summary>
        public int? ValidationCorrectionThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not an approver must specify a justification for this <see ref="Flag" />.
        /// </summary>
        public bool ApproverJustificationsRequired { get; set; }

        public void Validate(IActionContext actionContext)
        {
            if (!Enum.IsDefined(typeof(SignoffType), this.SignOffType))
            {
                throw new ApiException("Invalid Sign Off Type", "One of the stage sign off types is invalid. Sign off stages provided have not been saved");
            }

            this.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = this.SignOffType == SignoffType.AssignmentSupervisor;

            switch (SignOffType)
            {
                case SignoffType.LineManager:
                case SignoffType.ClaimantSelectsOwnChecker:
                case SignoffType.CostCodeOwner:
                    this.Relid = 0;
                    break;
                case SignoffType.AssignmentSupervisor:
                    this.Relid = 0;
                    break;
            }

            StageInclusionType include = StageInclusionType.None;
            bool inclusionTypeParsed = Enum.TryParse(IncludeId.ToString(CultureInfo.InvariantCulture), out include);
            if (!Enum.IsDefined(typeof(StageInclusionType),this.IncludeId))
            {
                throw new ApiException("Invalid Inclusion Type", "Valid IncludeId must be provided. Refer to StageInclusionType for valid values. Sign off stages provided have not been saved");
            }
            
            if (this.OnHolidayProvision == (HolidayProvision)3)
            {
                if (!Enum.IsDefined(typeof(SignoffType), this.HolidayType))
                {
                    throw new ApiException("Invalid Holiday Type", "Valid HolidayTypeId must be provided. Refer to SignOffTyoe for valid values. Sign off stages provided have not been saved");
                }
            }

            if (!Enum.IsDefined(typeof(HolidayProvision), (int)this.OnHolidayProvision))
            {
                throw new ApiException("Invalid Holiday Provision Type", "Valid OnHolidayId must be provided. Refer to HolidayProvision for valid values. Sign off stages provided have not been saved");
            }

            if (!Enum.IsDefined(typeof(Notify), (int)this.Notify))
            {
                throw new ApiException("Invalid Notify Id", "Valid NotifyId must be provided. Refer to Notify for valid values. Sign off stages provided have not been saved");
            }

            if (this.ValidationCorrectionThreshold < 0 || this.ValidationCorrectionThreshold > 100)
            {
                throw new ApiException("Invalid Validation Correction Threshold", "Valid Validation Correction Threshold must be provided. Refer to Validation Correction Threshold for valid values. Sign off stages provided have not been saved");
            }

        }

        public bool Equals(Stage other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Amount.Equals(other.Amount) && this.ApproveHigherLevelsOnly.Equals(other.ApproveHigherLevelsOnly)
                && this.ClaimantMail.Equals(other.ClaimantMail) && this.DisplayDeclaration.Equals(other.DisplayDeclaration)
                && this.ExtraLevels.Equals(other.ExtraLevels)
                && this.OnHolidayProvision.Equals(other.OnHolidayProvision) && this.HolidayId.Equals(other.HolidayId)
                && this.HolidayType.Equals(other.HolidayType) && this.IncludeId.Equals(other.IncludeId) 
                && this.StageInclusionType.Equals(other.StageInclusionType) 
                && this.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner.Equals(other.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner)
                && this.Notify.Equals(other.Notify) && this.OnHolidayProvision.Equals(other.OnHolidayProvision) 
                && this.Relid.Equals(other.Relid) && this.SendMail.Equals(other.SendMail)
                && this.SignOffType.Equals(other.SignOffType) && this.SingleSignOff.Equals(other.SingleSignOff)
                && this.AllocateForPayment == other.AllocateForPayment
                && this.IsPostValidationCleanupStage == other.IsPostValidationCleanupStage
                && this.ValidationCorrectionThreshold == other.ValidationCorrectionThreshold;
        }



        public override bool Equals(object obj)
        {
            return this.Equals(obj as Stage);
        }
    }

    internal static class StageConversion
    {
        internal static TResult Cast<TResult>(this cStage stage, int accountId, int employeeId, cEmployees employees) where TResult : Stage, new()
        {
            TResult result = new TResult
                       {
                           AccountId = accountId,
                           Amount = stage.amount,
                           ApproveHigherLevelsOnly = stage.FromMyLevel,
                           ClaimantMail = stage.claimantmail,
                           CreatedById = stage.createdby,
                           CreatedOn = stage.createdon,
                           DisplayDeclaration = stage.displaydeclaration,
                           EmployeeId = employeeId,
                           ExtraLevels = stage.ExtraApprovalLevels,
                           HolidayId = stage.holidayid,
                           HolidayDelegate = GetSignOffPerson(stage.holidayid, (int)stage.holidaytype, accountId),
                           HolidayType = (SpendManagementApi.Common.Enums.SignoffType)stage.holidaytype,
                           StageInclusionType = (StageInclusionType)stage.include,
                           ModifiedById = stage.modifiedby,
                           ModifiedOn = stage.modifiedon,
                           IncludeId = stage.includeid,
                           NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = stage.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner,
                           Notify = (Notify)stage.notify,
                           OnHolidayProvision = (HolidayProvision)stage.onholiday,
                           Relid = stage.relid,
                           RelSignOffPerson = GetSignOffPerson(stage.relid, (int)stage.signofftype, accountId),
                           SendMail = stage.sendmail,
                           SignOffId = stage.signoffid,
                           SignOffType = (SignoffType)stage.signofftype,
                           SingleSignOff = stage.singlesignoff,
                           SignOffStage = stage.stage,
                           AllocateForPayment = stage.AllocateForPayment,
                           ApproverJustificationsRequired = stage.ApproverJustificationsRequired,
                           IsPostValidationCleanupStage = stage.IsPostValidationCleanupStage,
                           ValidationCorrectionThreshold = stage.ValidationCorrectionThreshold
                       };

            return result;
        }

        private static string GetSignOffPerson(int relId, int signoffType, int accountId)
        {
            switch ((SignoffType)signoffType)
            {
                case SignoffType.BudgetHolder:
                    cBudgetholders budgetHolders = new cBudgetholders(accountId);
                    cBudgetHolder budgetHolder = budgetHolders.getBudgetHolderById(relId);
                    return (budgetHolder != null) ? budgetHolder.budgetholder : string.Empty;
                    
                case SignoffType.Employee:
                    cEmployees employees = new cEmployees(accountId);
                    Employee employee = employees.GetEmployeeById(relId);
                    return (employee != null) ? $"{employee.Surname}, {employee.Title} {employee.Forename}"
                               : string.Empty;
                    
                case SignoffType.Team:
                    cTeams teams = new cTeams(accountId);
                    cTeam team = teams.GetTeamById(relId);
                    return (team != null) ? team.teamname : String.Empty;

                case SignoffType.ApprovalMatrix:
                case SignoffType.DeterminedByClaimantFromApprovalMatrix:
                    ApprovalMatrices approvalMatrices = new ApprovalMatrices(accountId);
                    ApprovalMatrix approvalMatrix = approvalMatrices.GetById(relId);
                    return (approvalMatrix != null) ? approvalMatrix.Name : string.Empty;
                    
                default:
                    return string.Empty;
            }
        }

        internal static cStage Cast<TResult>(this Stage stage)
            where TResult : cStage
        {
            int relid = stage.Relid;
            bool fromMyLevel = false;
            
            switch (stage.SignOffType)
            {
                case SignoffType.LineManager:
                case SignoffType.ClaimantSelectsOwnChecker:
                case SignoffType.CostCodeOwner:
                    relid = 0;
                    break;
                case SignoffType.AssignmentSupervisor:
                    relid = 0;
                    break;
            }

            var result = new cStage(
                stage.SignOffId, 
                (SpendManagementLibrary.SignoffType)stage.SignOffType, 
                relid, 
                stage.ExtraLevels, 
                (SpendManagementLibrary.Enumerators.StageInclusionType)(stage.StageInclusionType),
                stage.Amount,
                (int)stage.Notify,
                stage.SignOffStage,
                (byte)stage.OnHolidayProvision,
                (SpendManagementLibrary.SignoffType)stage.HolidayType,
                stage.HolidayId,
                stage.IncludeId,
                stage.ClaimantMail,
                stage.SingleSignOff,
                stage.SendMail,
                stage.DisplayDeclaration,
                stage.CreatedOn,
                stage.CreatedById,
                (stage.ModifiedOn != null) ? stage.ModifiedOn.Value : stage.CreatedOn,
                (stage.ModifiedById != null) ? stage.ModifiedById.Value : stage.CreatedById,
                stage.ApproveHigherLevelsOnly,
                stage.ApproverJustificationsRequired,
                stage.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner,
                stage.AllocateForPayment,
                stage.IsPostValidationCleanupStage,
                stage.ValidationCorrectionThreshold);

            result.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner = (int)result.signofftype == (int)SignoffType.AssignmentSupervisor;

            return result;
        }
    }
}