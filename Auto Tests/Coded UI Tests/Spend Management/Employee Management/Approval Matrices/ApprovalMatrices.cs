namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Approval_Matrices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Budget_Holders;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;
    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams;

    using Auto_Tests.Tools;

    /// <summary>
    /// The Approval Matrices
    /// </summary>
    class ApprovalMatrices
    {

        /// <summary>
        /// The approval matrix id.
        /// </summary>
        internal int ApprovalMatrixId { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// The description.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// The approval matrix levels.
        /// </summary>
        internal List<ApprovalMatrixLevel> ApprovalMatrixLevels { get; set; }

        /// <summary>
        /// The default approver employee id
        /// </summary>
        internal int? DefaultApproverEmployeeId { get; set; }

        /// <summary>
        /// The default employee
        /// </summary>
        internal Employees DefaultEmployee { get; set; }

        /// <summary>
        /// The Default holder
        /// </summary>
        internal BudgetHolders DefaultHolder { get; set; }

        /// <summary>
        /// The default Team
        /// </summary>
        internal Teams DefaultTeam { get; set; }

        /// <summary>
        /// The default approver team id
        /// </summary>
        internal int? DefaultApproverTeamId { get; set; }

        /// <summary>
        /// The default approver budget holder id
        /// </summary>
        internal int? DefaultApproverBudgetHolderId { get; set; }

        /// <summary>
        /// the matrices grid values
        /// </summary>
        internal List<string> MatricesGridValues
        {
            get
            {
                return new List<string>
                {
                    this.Name,
                    this.Description,
                };
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrix"/> class.
        /// </summary>
        /// <param name="approvalMatrixId">
        /// The approval matrix id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="defaultapproverteamid"></param>
        /// <param name="matrixLevels">
        /// The Levels associated to this matrix
        /// </param>
        /// <param name="defaultapproverbudgetholderid"></param>
        /// <param name="defaultapproveremployeeid"></param>
        public ApprovalMatrices(int approvalMatrixId, string name, string description, int? defaultapproverbudgetholderid, int? defaultapproveremployeeid, int? defaultapproverteamid, List<ApprovalMatrixLevel> matrixLevels)
        {
            ApprovalMatrixId = approvalMatrixId;
            Name = name;
            Description = description;
            DefaultApproverBudgetHolderId = defaultapproverbudgetholderid;
            DefaultApproverEmployeeId = defaultapproveremployeeid;
            DefaultApproverTeamId = defaultapproverteamid;
            ApprovalMatrixLevels = matrixLevels;
        }

        public ApprovalMatrices()
        {
            // TODO: Complete member initialization
        }
    }

    /// <summary>
    /// The Approval Matrix Level
    /// </summary>
    class ApprovalMatrixLevel
    {
        /// <summary>
        /// The approval matrix level id.
        /// </summary>
        internal int ApprovalMatrixLevelId { get; set; }

        /// <summary>
        /// The approval matrix id.
        /// </summary>
        internal int ApprovalMatrixId { get; set; }

        /// <summary>
        /// The threshold amount.
        /// </summary>
        internal decimal ThresholdAmount { get; set; }

        /// <summary>
        /// The approver employee id.
        /// </summary>
        internal int? ApproverEmployeeId { get; set; }

        /// <summary>
        /// The approver team id.
        /// </summary>
        internal int? ApproverTeamId { get; set; }

        /// <summary>
        /// The approver budget holder id.
        /// </summary>
        internal int? ApproverBudgetHolderId;

        /// <summary>
        /// The level employee
        /// </summary>
        internal Employees LevelEmployee { get; set; }

        /// <summary>
        /// The level holder
        /// </summary>
        internal BudgetHolders LevelHolder { get; set; }

        /// <summary>
        /// The level Team
        /// </summary>
        internal Teams LevelTeam { get; set; }

        /// <summary>
        /// threshold ammount as displayed on grid
        /// </summary>
        internal string GridThreshHoldAmount
        { 
            get
            { 
                return string.Concat( decimal.Round(ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString());
            }
        }

        /// <summary>
        /// matrices grid values
        /// </summary>
        internal List<string> MatrixLevelGridValues
        {
            get
            {
                if (LevelEmployee != null)
                {
                    return new List<string> { decimal.Round(this.ThresholdAmount,2,MidpointRounding.AwayFromZero).ToString(), "Employee", this.LevelEmployee.EmployeeFullName };
                }
                else if(LevelTeam !=null)
                {
                    return new List<string> { decimal.Round(this.ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString(), "Team", this.LevelTeam.TeamName };
                }
                else
                {
                    return new List<string> { decimal.Round(this.ThresholdAmount, 2, MidpointRounding.AwayFromZero).ToString(), "Budget Holder", this.LevelHolder.BudgetHolder };
                }
            }
        }

        public const string SqlItems = "select approvalMatrixLevelId, approvalMatrixId, thresholdAmount, approverEmployeeId, approverBudgetHolderId, approverTeamId from approvalMatrixLevels";

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrixLevel"/> class.
        /// </summary>
        /// <param name="approvalMatrixLevelId">
        /// The approval matrix level id.
        /// </param>
        /// <param name="approvalMatrixId">
        /// The approval matrix id.
        /// </param>
        /// <param name="thresholdAmount">
        /// The threshold amount.
        /// </param>
        /// <param name="approverEmployeeId">
        /// The approver employee id.
        /// </param>
        /// <param name="approverTeamId">
        /// The approver team id.
        /// </param>
        /// <param name="approverBudgetHolderId">
        /// The approver budget holder id.
        /// </param>
        public ApprovalMatrixLevel(int approvalMatrixLevelId, int approvalMatrixId, decimal thresholdAmount, int? approverEmployeeId, int? approverTeamId, int? approverBudgetHolderId)
        {
            ApprovalMatrixLevelId = approvalMatrixLevelId;
            ApprovalMatrixId = approvalMatrixId;
            ThresholdAmount = thresholdAmount;
            ApproverEmployeeId = approverEmployeeId;
            ApproverTeamId = approverTeamId;
            ApproverBudgetHolderId = approverBudgetHolderId;
        }

        public ApprovalMatrixLevel()
        {
            // TODO: Complete member initialization
        }

        public ApproverType GetApproverType(object approver)
        {
            if (approver.GetType() == typeof(Employees))
            {
                return ApproverType.Employee;
            }
            else if (approver.GetType() == typeof(BudgetHolders))
            {
                return ApproverType.BudgetHolder;
            }
            else
            {
                return ApproverType.Team;
            }
        }
    }

    enum ApproverType
    {
        [Description("Employee")]
        Employee = 0,
        [Description("BudgetHolder")]
        BudgetHolder,
        [Description("Team")]
        Team
    }
}
