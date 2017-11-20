namespace SpendManagementLibrary
{
    using System.Collections.Generic;

    /// <summary>
    /// The approval matrix data class.
    /// </summary>
    public class ApprovalMatrix
    {
        /// <summary>
        /// The SQL cache script.
        /// </summary>
        public const string SqlCache = "SELECT CacheExpiry from dbo.[approvalMatrices] WHERE {0} = {0};";

        public const string SqlItems = "select approvalMatrixId, name, description, defaultApproverBudgetHolderId, defaultApproverEmployeeId, defaultApproverTeamId FROM approvalMatrices;";

        /// <summary>
        /// The approval matrix id.
        /// </summary>
        private int approvalMatrixId;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The approval matrix levels.
        /// </summary>
        private List<ApprovalMatrixLevel> approvalMatrixLevels;

        private int? defaultApproverEmployeeId;

        private int? defaultApproverTeamId;

        private int? defaultApproverBudgetHolderId;

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
        public ApprovalMatrix(int approvalMatrixId, string name, string description, int? defaultapproverbudgetholderid, int? defaultapproveremployeeid, int? defaultapproverteamid, List<ApprovalMatrixLevel> matrixLevels)
        {
            this.approvalMatrixId = approvalMatrixId;
            this.name = name;
            this.description = description;
            this.defaultApproverBudgetHolderId = defaultapproverbudgetholderid;
            this.defaultApproverEmployeeId = defaultapproveremployeeid;
            this.defaultApproverTeamId = defaultapproverteamid;
            this.ApprovalMatrixLevels = matrixLevels;
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
        /// <param name="defaultApproverKey"></param>
        /// <param name="matrixLevels">
        /// The Levels associated to this matrix
        /// </param>
        public ApprovalMatrix(int approvalMatrixId, string name, string description, string defaultApproverKey, List<ApprovalMatrixLevel> matrixLevels)
        {
            this.approvalMatrixId = approvalMatrixId;
            this.name = name;
            this.description = description;
            this.SetIdFromDefaultApproverKey(defaultApproverKey);
            this.ApprovalMatrixLevels = matrixLevels;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrix"/> class.
        /// </summary>
        public ApprovalMatrix()
        {
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
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// Gets or sets the approval matrix levels.
        /// </summary>
        public List<ApprovalMatrixLevel> ApprovalMatrixLevels
        {
            get
            {
                return this.approvalMatrixLevels;
            }

            set
            {
                this.approvalMatrixLevels = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver employee id.
        /// </summary>
        public int? DefaultApproverEmployeeId
        {
            get
            {
                return this.defaultApproverEmployeeId;
            }

            set
            {
                this.defaultApproverEmployeeId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver team id.
        /// </summary>
        public int? DefaultApproverTeamId
        {
            get
            {
                return this.defaultApproverTeamId;
            }

            set
            {
                this.defaultApproverTeamId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver budget holder id.
        /// </summary>
        public int? DefaultApproverBudgetHolderId
        {
            get
            {
                return this.defaultApproverBudgetHolderId;
            }

            set
            {
                this.defaultApproverBudgetHolderId = value;
            }
        }

        /// <summary>
        /// Gets or sets the approver key.
        /// </summary>
        public string DefaultApproverKey
        {
            get
            {
                return this.GetDefaultApproverKeyFromIds();
            }

            set
            {
                this.SetIdFromDefaultApproverKey(value);
            }
        }

        public string DefaultApproverFriendlyName
        {
            get;
            set;
        }

        /// <summary>
        /// The get approver key from ids.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDefaultApproverKeyFromIds()
        {
            if (this.defaultApproverBudgetHolderId != null)
            {
                return string.Format("{0},{1}", (int)SpendManagementElement.BudgetHolders, this.defaultApproverBudgetHolderId);
            }

            if (this.defaultApproverEmployeeId != null)
            {
                return string.Format("{0},{1}", (int)SpendManagementElement.Employees, this.defaultApproverEmployeeId);
            }

            if (this.defaultApproverTeamId != null)
            {
                return string.Format("{0},{1}", (int)SpendManagementElement.Teams, this.defaultApproverTeamId);
            }

            return string.Empty;
        }

        /// <summary>
        /// The set id from approver key.
        /// </summary>
        /// <param name="defaultApproverKey">
        /// The approver key.
        /// </param>
        private void SetIdFromDefaultApproverKey(string defaultApproverKey)
        {
            var splitKey = defaultApproverKey.Split(',');
            if (splitKey.Length != 2)
            {
                return;
            }

            var itemType = (SpendManagementElement)byte.Parse(splitKey[0]);
            switch (itemType)
            {
                case SpendManagementElement.Teams:
                    this.defaultApproverTeamId = int.Parse(splitKey[1]);
                    break;
                case SpendManagementElement.BudgetHolders:
                    this.defaultApproverBudgetHolderId = int.Parse(splitKey[1]);
                    break;
                case SpendManagementElement.Employees:
                    this.defaultApproverEmployeeId = int.Parse(splitKey[1]);
                    break;
            }
        }
    }
}
