namespace SpendManagementLibrary
{
    /// <summary>
    /// The approval matrix level data class.
    /// </summary>
    public class ApprovalMatrixLevel
    {
        /// <summary>
        /// The approval matrix level id.
        /// </summary>
        private int approvalMatrixLevelId;

        /// <summary>
        /// The approval matrix id.
        /// </summary>
        private int approvalMatrixId;

        /// <summary>
        /// The threshold amount.
        /// </summary>
        private decimal approvalLimit;

        /// <summary>
        /// The approver employee id.
        /// </summary>
        private int? approverEmployeeId;

        /// <summary>
        /// The approver team id.
        /// </summary>
        private int? approverTeamId;

        /// <summary>
        /// The approver budget holder id.
        /// </summary>
        private int? approverBudgetHolderId;

        public const string SqlItems = "select approvalMatrixLevelId, approvalMatrixId, approvalLimit, approverEmployeeId, approverBudgetHolderId, approverTeamId from approvalMatrixLevels";

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrixLevel"/> class.
        /// </summary>
        /// <param name="approvalMatrixLevelId">
        /// The approval matrix level id.
        /// </param>
        /// <param name="approvalMatrixId">
        /// The approval matrix id.
        /// </param>
        /// <param name="approvalLimit">
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
        public ApprovalMatrixLevel(int approvalMatrixLevelId, int approvalMatrixId, decimal approvalLimit, int? approverEmployeeId, int? approverTeamId, int? approverBudgetHolderId)
        {
            this.approvalMatrixLevelId = approvalMatrixLevelId;
            this.approvalMatrixId = approvalMatrixId;
            this.approvalLimit = approvalLimit;
            this.approverEmployeeId = approverEmployeeId;
            this.approverTeamId = approverTeamId;
            this.approverBudgetHolderId = approverBudgetHolderId;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrixLevel"/> class.
        /// </summary>
        public ApprovalMatrixLevel()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrixLevel"/> class.
        /// </summary>
        /// <param name="approvalMatrixLevelId">
        /// The approval matrix level id.
        /// </param>
        /// <param name="approvalMatrixId">
        /// The approval matrix id.
        /// </param>
        /// <param name="approvalLimit">
        /// The threshold amount.
        /// </param>
        /// <param name="approverKey">
        /// The approver key.
        /// </param>
        public ApprovalMatrixLevel(int approvalMatrixLevelId, int approvalMatrixId, decimal approvalLimit, string approverKey)
        {
            this.approvalMatrixLevelId = approvalMatrixLevelId;
            this.approvalMatrixId = approvalMatrixId;
            this.approvalLimit = approvalLimit;
            this.SetIdFromApproverKey(approverKey);
        }

        /// <summary>
        /// Gets or sets the approval matrix level id.
        /// </summary>
        public int ApprovalMatrixLevelId
        {
            get
            {
                return this.approvalMatrixLevelId;
            }

            set
            {
                this.approvalMatrixLevelId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approval matrix id.
        /// </summary>
        public int ApprovalMatrixId
        {
            get
            {
                return this.approvalMatrixId;
            }

            set
            {
                this.approvalMatrixId = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold amount.
        /// </summary>
        public decimal ApprovalLimit
        {
            get
            {
                return this.approvalLimit;
            }

            set
            {
                this.approvalLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver employee id.
        /// </summary>
        public int? ApproverEmployeeId
        {
            get
            {
                return this.approverEmployeeId;
            }

            set
            {
                this.approverEmployeeId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver team id.
        /// </summary>
        public int? ApproverTeamId
        {
            get
            {
                return this.approverTeamId;
            }

            set
            {
                this.approverTeamId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver budget holder id.
        /// </summary>
        public int? ApproverBudgetHolderId
        {
            get
            {
                return this.approverBudgetHolderId;
            }

            set
            {
                this.approverBudgetHolderId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver key.
        /// </summary>
        public string ApproverKey
        {
            get
            {
                return this.GetApproverKeyFromIds();
            }

            set
            {
                this.SetIdFromApproverKey(value);
            }
        }

        public string ApproverFriendlyName { get; set; }

        /// <summary>
        /// The get approver key from ids.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetApproverKeyFromIds()
        {
            if (this.approverBudgetHolderId != null)
            {
                return string.Format("{0},{1}", (int)SpendManagementElement.BudgetHolders, this.approverBudgetHolderId);
            }

            if (this.approverEmployeeId != null)
            {
                return string.Format("{0},{1}", (int)SpendManagementElement.Employees, this.approverEmployeeId);
            }

            if (this.approverTeamId != null)
            {
                return string.Format("{0},{1}", (int)SpendManagementElement.Teams, this.approverTeamId);
            }

            return string.Empty;
        }

        /// <summary>
        /// The set id from approver key.
        /// </summary>
        /// <param name="approverKey">
        /// The approver key.
        /// </param>
        private void SetIdFromApproverKey(string approverKey)
        {
            var splitKey = approverKey.Split(',');
            if (splitKey.Length != 2)
            {
                return;
            }

            var itemType = (SpendManagementElement)byte.Parse(splitKey[0]);
            switch (itemType)
            {
                case SpendManagementElement.Teams:
                    this.approverTeamId = int.Parse(splitKey[1]);
                    break;
                case SpendManagementElement.BudgetHolders:
                    this.approverBudgetHolderId = int.Parse(splitKey[1]);
                    break;
                case SpendManagementElement.Employees:
                    this.approverEmployeeId = int.Parse(splitKey[1]);
                    break;
            }
        }
    }
}
