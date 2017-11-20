using SpendManagementLibrary.Employees;

namespace Spend_Management.shared.code.ApprovalMatrix
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.Caching;
    using System.Web.UI.WebControls;

    using Microsoft.SqlServer.Server;

    using SpendManagementLibrary.Helpers;

    using Spend_Management.shared.code.Interfaces;
    using SpendManagementLibrary;

    using SortDirection = SpendManagementLibrary.SortDirection;

    /// <summary>
    /// The approval matrices class for caching Approval Matrices.
    /// </summary>
    public class ApprovalMatrices : CodeClasses<ApprovalMatrix>
    {
        #region Private Fields

        /// <summary>
        /// Tracking variable for caching lock
        /// </summary>
        private static object CacheLock = new object();

        /// <summary>
        /// The approval matrices.
        /// </summary>
        private SortedList<int, ApprovalMatrix> approvalMatrices;

        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="ApprovalMatrices"/> class.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="databaseConnection">
        /// The database Connection to use for this session.
        /// </param>
        public ApprovalMatrices(int accountId, DBConnection databaseConnection = null)
            : base(accountId, databaseConnection)
        {
            this.InitialiseData();
        }

        #region Properties

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        private string CacheKey
        {
            get
            {
                return string.Format("approvalMatrix_{0}", this.AccountId);
            }
        }

        #endregion

        #region Overridden Methods

        /// <summary>
        /// The cache list.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>SortedList</cref>
        ///     </see>
        ///     .
        /// </returns>
        public override SortedList<int, ApprovalMatrix> CacheList()
        {
            SortedList<int, List<ApprovalMatrixLevel>> levelList = this.GetLevels();
            this.DbConnection = new DBConnection(cAccounts.getConnectionString(this.AccountId)) { sqlexecute = { CommandText = string.Format(ApprovalMatrix.SqlCache, this.AccountId) } };
            var list = new SortedList<int, ApprovalMatrix>();
            if (GlobalVariables.GetAppSettingAsBoolean("EnableBrokers"))
            {
                var dep = new SqlCacheDependency(this.DbConnection.sqlexecute);
                this.Cache.Insert(this.CacheKey, list, dep, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes((int)Caching.CacheTimeSpans.Short), CacheItemPriority.Default, null);
            }

            using (var reader = this.DbConnection.GetReader(ApprovalMatrix.SqlItems))
            {
                var matrixIdOrd = reader.GetOrdinal("approvalMatrixId");
                var nameOrd = reader.GetOrdinal("name");
                var descriptionOrd = reader.GetOrdinal("description");
                var defaultApproverEmployeeIdOrd = reader.GetOrdinal("defaultApproverEmployeeId");
                var defaultApproverBudgetHolderIdOrd = reader.GetOrdinal("defaultApproverBudgetHolderId");
                var defaultApproverTeamIdOrd = reader.GetOrdinal("defaultApproverTeamId");

                while (reader.Read())
                {
                    var matrixId = reader.GetInt32(matrixIdOrd);
                    var name = reader.GetString(nameOrd);
                    var description = reader.GetString(descriptionOrd);
                    int? defaultApproverEmployeeId = reader.IsDBNull(defaultApproverEmployeeIdOrd) ? (int?)null : reader.GetInt32(defaultApproverEmployeeIdOrd);
                    int? defaultApproverBudgetHolderId = reader.IsDBNull(defaultApproverBudgetHolderIdOrd) ? (int?)null : reader.GetInt32(defaultApproverBudgetHolderIdOrd);
                    int? defaultApproverTeamId = reader.IsDBNull(defaultApproverTeamIdOrd) ? (int?)null : reader.GetInt32(defaultApproverTeamIdOrd);

                    list.Add(matrixId, new ApprovalMatrix(matrixId, name, description, defaultApproverBudgetHolderId, defaultApproverEmployeeId, defaultApproverTeamId, this.FindLevels(matrixId, levelList)));
                }

                reader.Close();
            }

            return list;
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Save(ApprovalMatrix entity)
        {
            this.DbConnection = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            CurrentUser currentUser = cMisc.GetCurrentUser();

            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", entity.ApprovalMatrixId);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@name", entity.Name);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@description", entity.Description);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@defaultApproverBudgetHolderId", DBNull.Value);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@defaultApproverEmployeeId", DBNull.Value);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@defaultApproverTeamId", DBNull.Value);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            if (entity.DefaultApproverBudgetHolderId != null)
            {
                this.DbConnection.sqlexecute.Parameters["@defaultApproverBudgetHolderId"].Value = entity.DefaultApproverBudgetHolderId;
            }

            if (entity.DefaultApproverEmployeeId != null)
            {
                this.DbConnection.sqlexecute.Parameters["@defaultApproverEmployeeId"].Value = entity.DefaultApproverEmployeeId;
            }

            if (entity.DefaultApproverTeamId != null)
            {
                this.DbConnection.sqlexecute.Parameters["@defaultApproverTeamId"].Value = entity.DefaultApproverTeamId;
            }

            if (currentUser != null)
            {
                this.DbConnection.sqlexecute.Parameters["@employeeID"].Value = currentUser.EmployeeID;
                if (currentUser.isDelegate)
                {
                    this.DbConnection.sqlexecute.Parameters["@delegateID"].Value = currentUser.Delegate.EmployeeID;
                }
            }

            this.DbConnection.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            this.DbConnection.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            this.DbConnection.ExecuteProc("saveApprovalMatrix");

            var id = (int)DbConnection.sqlexecute.Parameters["@id"].Value;
            this.DbConnection.sqlexecute.Parameters.Clear();

            //if (id > 0 && entity.ApprovalMatrixLevels != null)
            //{
            //    foreach (ApprovalMatrixLevel level in entity.ApprovalMatrixLevels)
            //    {
            //        this.SaveLevel(level);
            //    }
            //}

            return id;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ApprovalMatrix"/>.
        /// </returns>
        public override ApprovalMatrix GetById(int id)
        {
            if (this.approvalMatrices != null)
            {
                ApprovalMatrix result;
                this.approvalMatrices.TryGetValue(id, out result);
                return result;
            }

            return null;
        }

        /// <summary>
        /// The get by string.
        /// </summary>
        /// <param name="searchString">
        /// The search string.
        /// </param>
        /// <returns>
        /// The <see cref="ApprovalMatrix"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override ApprovalMatrix GetByString(string searchString)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Count()
        {
            return this.approvalMatrices.Count;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Delete(int id)
        {
            this.DbConnection = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            CurrentUser currentUser = cMisc.GetCurrentUser();
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", id);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@auditEmployeeId", 0);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@auditDelegateId", DBNull.Value);

            if (currentUser != null)
            {
                this.DbConnection.sqlexecute.Parameters["@auditEmployeeId"].Value = currentUser.EmployeeID;
                if (currentUser.isDelegate)
                {
                    this.DbConnection.sqlexecute.Parameters["@auditDelegateId"].Value = currentUser.Delegate.EmployeeID;
                }
            }

            DbConnection.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            DbConnection.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            DbConnection.ExecuteProc("deleteApprovalMatrix");

            var returnid = (int)DbConnection.sqlexecute.Parameters["@id"].Value;
            DbConnection.sqlexecute.Parameters.Clear();

            return returnid;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Delete(ApprovalMatrix entity)
        {
            return this.Delete(entity.ApprovalMatrixId);
        }

        /// <summary>
        /// The initialise data.
        /// </summary>
        public override void InitialiseData()
        {
            this.approvalMatrices = this.Cache[this.CacheKey] as SortedList<int, ApprovalMatrix>;
            if (this.approvalMatrices == null)
            {
                lock (CacheLock)
                {
                    this.approvalMatrices = this.Cache[this.CacheKey] as SortedList<int, ApprovalMatrix> ?? this.CacheList();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Save a matrix level.
        /// </summary>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveLevel(ApprovalMatrixLevel level)
        {
            this.DbConnection = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            CurrentUser currentUser = cMisc.GetCurrentUser();

            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", level.ApprovalMatrixId);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalmatrixlevelid", level.ApprovalMatrixLevelId);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalLimit", level.ApprovalLimit);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approverBudgetHolderId", DBNull.Value);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approverEmployeeId", DBNull.Value);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approverTeamId", DBNull.Value);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);

            if (level.ApproverBudgetHolderId != null)
            {
                this.DbConnection.sqlexecute.Parameters["@approverBudgetHolderId"].Value = level.ApproverBudgetHolderId;
            }

            if (level.ApproverEmployeeId != null)
            {
                this.DbConnection.sqlexecute.Parameters["@approverEmployeeId"].Value = level.ApproverEmployeeId;
            }

            if (level.ApproverTeamId != null)
            {
                this.DbConnection.sqlexecute.Parameters["@approverTeamId"].Value = level.ApproverTeamId;
            }

            if (currentUser != null)
            {
                this.DbConnection.sqlexecute.Parameters["@employeeID"].Value = currentUser.EmployeeID;
                if (currentUser.isDelegate)
                {
                    this.DbConnection.sqlexecute.Parameters["@delegateID"].Value = currentUser.Delegate.EmployeeID;
                }
            }

            this.DbConnection.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            this.DbConnection.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            this.DbConnection.ExecuteProc("saveApprovalMatrixLevel");

            var id = (int)DbConnection.sqlexecute.Parameters["@id"].Value;
            this.DbConnection.sqlexecute.Parameters.Clear();

            return id;
        }

        /// <summary>
        /// The get matrix grid.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string[] GetMatrixGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser == null)
            {
                return new string[0];
            }

            var clsgrid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridMatrices", "select approvalMatrixid, name, Description from approvalMatrices");

            clsgrid.getColumnByName("approvalMatrixid").hidden = true;
            clsgrid.editlink = "aeapprovalMatrix.aspx?matrixid={approvalMatrixId}";
            clsgrid.deletelink = "javascript:SEL.ApprovalMatrices.Matrix.Delete({approvalMatrixId});";
            clsgrid.KeyField = "approvalMatrixId";
            clsgrid.CssClass = "datatbl";
            clsgrid.EmptyText = "There are no Approval Matrices to display.";
            clsgrid.enabledeleting = currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ApprovalMatrix, true);
            clsgrid.enableupdating = currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true);

            if (currentUser.Employee.GetNewGridSortOrders().GetBy("gridMatrices") == null)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName("matrixName");
                clsgrid.SortDirection = SortDirection.Ascending;
            }

            return clsgrid.generateGrid();
        }

        /// <summary>
        /// Get the level grid.
        /// </summary>
        /// <param name="entityid">
        /// The entity id.
        /// </param>
        /// <param name="addingNew"></param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public string[] GetLevelGrid(int entityid, bool addingNew = false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ApprovalMatrix, true))
            {
                return new string[0];
            }

            var clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridMatrixLevels", string.Format("select approvalMatrixId, approvalMatrixLevelId, approvalLimit, approverType, approver from approvalMatrixLevelsCombinedApprover WHERE approvalMatrixId = {0}", entityid));

            clsgrid.getColumnByName("approvalMatrixLevelId").hidden = true;
            clsgrid.getColumnByName("approvalMatrixId").hidden = true;
            clsgrid.KeyField = "approvalMatrixLevelId";
            clsgrid.CssClass = "datatbl";
            clsgrid.EmptyText = "There are no levels to display.";
            clsgrid.enableupdating = addingNew || user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true);
            clsgrid.enabledeleting = addingNew || user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ApprovalMatrix, true);
            clsgrid.editlink = clsgrid.enableupdating ? "javascript:SEL.ApprovalMatrices.Level.Edit({approvalMatrixLevelId});" : string.Empty;
            clsgrid.deletelink = clsgrid.enabledeleting ? "javascript:SEL.ApprovalMatrices.Level.Delete({approvalMatrixLevelId});" : string.Empty;
            clsgrid.addFilter(((cFieldColumn)clsgrid.getColumnByName("approvalMatrixId")).field, ConditionType.Equals, new object[] { entityid }, null, ConditionJoiner.And);

            if (user.Employee.GetNewGridSortOrders().GetBy("gridMatrixLevels") == null)
            {
                clsgrid.SortedColumn = clsgrid.getColumnByName("approvalLimit");
                clsgrid.SortDirection = SortDirection.Ascending;
            }

            return clsgrid.generateGrid();
        }

        /// <summary>
        /// Create a list of 'list items' for a drop down list
        /// </summary>
        /// <returns>List of ApprovalMatrix ListItems</returns>
        public List<ListItem> CreateDropDown(int selectedApprovalMatrixId = -1, bool includeNone = true)
        {
            List<ListItem> matrices = new List<ListItem>();

            if (approvalMatrices.Count > 0)
            {
                foreach (ApprovalMatrix approvalMatrix in this.approvalMatrices.Values)
                {
                    ListItem matrixListItem = new ListItem(approvalMatrix.Name, approvalMatrix.ApprovalMatrixId.ToString(CultureInfo.InvariantCulture));

                    if (selectedApprovalMatrixId > -1 && selectedApprovalMatrixId == approvalMatrix.ApprovalMatrixId)
                    {
                        matrixListItem.Selected = true;
                    }

                    matrices.Add(matrixListItem);
                }
            }

            matrices.Sort((li1, li2) => li1.Text.CompareTo(li2.Text));

            if (includeNone)
            {
                matrices.Insert(0, new ListItem("[None]", "0"));
            }

            return matrices;
        }

        /// <summary>
        /// Get friendly name of approver.
        /// Name + [Type]
        /// </summary>
        /// <param name="approverKey">
        /// The approver key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFriendlyNameOfApprover(string approverKey)
        {
            var approvers = approverKey.Split(',');
            string friendlyApproverName = string.Empty;
            if (approvers.Count() == 2)
            {
                var approvertype = (SpendManagementElement)byte.Parse(approvers[0]);
                var approverId = int.Parse(approvers[1]);
                if (approverId > 0)
                {
                    switch (approvertype)
                    {
                        case SpendManagementElement.BudgetHolders:
                            var budgetHolders = new cBudgetholders(this.AccountId);
                            cBudgetHolder ownerBudgetHolder = budgetHolders.getBudgetHolderById(approverId);
                            if (ownerBudgetHolder == null)
                            {
                                return string.Empty;
                            }

                            friendlyApproverName = ownerBudgetHolder.budgetholder + " (Budget Holder)";
                            break;
                        case SpendManagementElement.Employees:
                            var employees = new cEmployees(this.AccountId);
                            Employee ownerEmployee = employees.GetEmployeeById(approverId);
                            if (ownerEmployee == null)
                            {
                                return string.Empty;
                            }

                            friendlyApproverName = ownerEmployee.FullName + " (Employee)";
                            break;
                        case SpendManagementElement.Teams:
                            var user = cMisc.GetCurrentUser();
                            var teams = new cTeams(this.AccountId, user.CurrentSubAccountId);
                            cTeam ownerTeam = teams.GetTeamById(approverId);
                            if (ownerTeam == null)
                            {
                                return string.Empty;
                            }

                            friendlyApproverName = ownerTeam.teamname + " (Team)";
                            break;
                    }
                }
            }

            return friendlyApproverName;
        }

        /// <summary>
        /// The delete level.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <param name="matrixLevelId">
        /// The matrix level id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteLevel(int matrixId, int matrixLevelId)
        {
            this.DbConnection = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            CurrentUser currentUser = cMisc.GetCurrentUser();

            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalmatrixid", matrixId);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@approvalmatrixlevelid", matrixLevelId);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@auditEmployeeId", 0);
            this.DbConnection.sqlexecute.Parameters.AddWithValue("@auditDelegateId", DBNull.Value);

            if (currentUser != null)
            {
                this.DbConnection.sqlexecute.Parameters["@auditEmployeeId"].Value = currentUser.EmployeeID;
                if (currentUser.isDelegate)
                {
                    this.DbConnection.sqlexecute.Parameters["@auditDelegateId"].Value = currentUser.Delegate.EmployeeID;
                }
            }

            this.DbConnection.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
            this.DbConnection.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
            this.DbConnection.ExecuteProc("deleteApprovalMatrixLevel");

            var returnid = (int)DbConnection.sqlexecute.Parameters["@id"].Value;
            this.DbConnection.sqlexecute.Parameters.Clear();

            return returnid;
        }

        /// <summary>
        /// Get employees for claimant chooses by Matrix approver.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="stage">
        /// The stage.
        /// </param>
        /// <returns>
        /// The <see cref="ListItem[]"/>.
        /// </returns>
        /// <remarks>
        ///     First this should check for the ApprovalMatrixLevel for the appropriate amount, then include any levels above that it needs to (treating the default approver as the top level)
        ///     Then it should remove any occurrance of the claimant from the list.
        ///     If this leaves a blank list then it should check for the next level above those already included.
        ///     Then it should remove any occurrance of the claimant from the list.
        ///     If this leaves a blank list then it should use the default approver.
        ///     Then it should remove any occurrance of the claimant from the lists.
        ///     The remaining list should then be returned regardless of content.
        /// </remarks>
        public ListItem[] GetEmployeesForApprover(decimal amount, cStage stage)
        {
            var result = new List<ListItem>();
            var matrix = this.GetById(stage.relid);
            var user = cMisc.GetCurrentUser();
            if (stage.FromMyLevel)
            {
                var newAmount = GetClaimantsLevel(
                    user.EmployeeID, matrix, stage, new cBudgetholders(user.AccountID), new cTeams(user.AccountID, user.CurrentSubAccountId));
                if (newAmount > -1 && newAmount > amount)
                {
                    amount = newAmount;
                }
            }

            IEnumerable<ApprovalMatrixLevel> stageLevels = GetListOfLevelsForThisAmount(matrix, stage.ExtraApprovalLevels, amount);
            cBudgetholders bugetHolders = new cBudgetholders(AccountId);

            foreach (ApprovalMatrixLevel matrixLevel in stageLevels)
            {
                result = this.AddEmployeeListItemsFromLevel(user, matrixLevel, result,bugetHolders, user.EmployeeID);
            }

            // add the next level if the list is empty
            if (result.Count == 0)
            {
                decimal highestStageLevel = stageLevels.Max(level => level.ApprovalLimit);
                ApprovalMatrixLevel nextHighestLevel = matrix.ApprovalMatrixLevels.Where(level => level.ApprovalLimit > highestStageLevel).OrderBy(level => level.ApprovalLimit).FirstOrDefault();
                if (nextHighestLevel != null)
                {
                    this.AddEmployeeListItemsFromLevel(user, nextHighestLevel, result, bugetHolders, user.EmployeeID);
                }
            }

            // add the default approver if the list is empty
            if (result.Count == 0)
            {
                result = this.AddEmployeeListItemsFromLevel(user, new ApprovalMatrixLevel(0, matrix.ApprovalMatrixId, decimal.MaxValue, matrix.DefaultApproverEmployeeId, matrix.DefaultApproverTeamId, matrix.DefaultApproverBudgetHolderId), result, bugetHolders, user.EmployeeID);
            }

            List<ListItem> rememberedClaimantSelectedApprovers = this.GetLastXClaimantSelectedApprovers(user, result);

            result = result.OrderBy(x => x.Text).ToList();

            if (rememberedClaimantSelectedApprovers.Count > 0)
            {
                // we have a list of remembered approvers to insert at the start of the list.
                result.InsertRange(0, rememberedClaimantSelectedApprovers);
            }

            return result.ToArray();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get list of tiers for this amount.
        /// </summary>
        /// <param name="matrix">
        /// The matrix.
        /// </param>
        /// <param name="numberOfHigherTiers">
        /// The no of higher tiers.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public static IEnumerable<ApprovalMatrixLevel> GetListOfLevelsForThisAmount(ApprovalMatrix matrix, int numberOfHigherTiers, decimal amount)
        {
            var result = new List<ApprovalMatrixLevel>();
            var sortedMatrix = new SortedList<decimal, ApprovalMatrixLevel>();
            var defaultApprover = new ApprovalMatrixLevel(0, matrix.ApprovalMatrixId, decimal.MaxValue, matrix.DefaultApproverEmployeeId, matrix.DefaultApproverTeamId, matrix.DefaultApproverBudgetHolderId);

            foreach (ApprovalMatrixLevel level in matrix.ApprovalMatrixLevels)
            {
                if (!sortedMatrix.ContainsKey(level.ApprovalLimit))
                {
                    sortedMatrix.Add(level.ApprovalLimit, level);
                }
            }

            if (!sortedMatrix.ContainsKey(defaultApprover.ApprovalLimit))
            {
                sortedMatrix.Add(defaultApprover.ApprovalLimit, defaultApprover);
            }

            var keyList = new List<decimal>();
            foreach (decimal item in sortedMatrix.Keys)
            {
                if (item < amount)
                {
                    continue;
                }

                keyList.Add(item);

                if (keyList.Count > numberOfHigherTiers)
                {
                    break;
                }
            }

            foreach (decimal key in keyList)
            {
                result.Add(sortedMatrix[key]);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="approvalMatrixLevel"></param>
        /// <param name="employeeListItems"></param>
        /// <param name="doNotAddEmployeeId"></param>
        /// <param name="budgetHolders"></param>
        /// <returns></returns>
        private List<ListItem> AddEmployeeListItemsFromLevel(ICurrentUser currentUser, ApprovalMatrixLevel approvalMatrixLevel, List<ListItem> employeeListItems, cBudgetholders budgetHolders, int doNotAddEmployeeId = -1)
        {
            if (approvalMatrixLevel == null || (!approvalMatrixLevel.ApproverBudgetHolderId.HasValue && !approvalMatrixLevel.ApproverEmployeeId.HasValue && !approvalMatrixLevel.ApproverTeamId.HasValue))
            {
                return employeeListItems;
            }

            var accessRoles = new cAccessRoles(AccountId, cAccounts.getConnectionString(AccountId));
            cEmployees employees = new cEmployees(AccountId);
            List<int> employeeIds = new List<int>();

            if (approvalMatrixLevel.ApproverBudgetHolderId.HasValue)
            {

                if (budgetHolders == null)
                {
                    budgetHolders = new cBudgetholders(AccountId);
                }

                cBudgetHolder budgetHolder = budgetHolders.getBudgetHolderById(approvalMatrixLevel.ApproverBudgetHolderId.Value);
                if (budgetHolder != null && budgetHolder.employeeid != doNotAddEmployeeId)
                {
                    employeeIds.Add(budgetHolder.employeeid);
                }
            }

            if (approvalMatrixLevel.ApproverEmployeeId.HasValue && approvalMatrixLevel.ApproverEmployeeId.Value != doNotAddEmployeeId)
            {
                employeeIds.Add(approvalMatrixLevel.ApproverEmployeeId.Value);
            }

            if (approvalMatrixLevel.ApproverTeamId.HasValue)
            {
                cTeams teams = new cTeams(AccountId);
                cTeam team = teams.GetTeamById(approvalMatrixLevel.ApproverTeamId.Value);

                if (team != null)
                {
                    if (team.teamLeaderId.HasValue && team.teamLeaderId.Value != doNotAddEmployeeId)
                    {
                        employeeIds.Add(team.teamLeaderId.Value);
                    }

                    if (team.teammembers.Count > 0)
                    {
                        List<int> tempIds = new List<int>(team.teammembers);
                        if (doNotAddEmployeeId != -1)
                        {
                            tempIds.RemoveAll(x => x == doNotAddEmployeeId);
                        }
                        employeeIds.AddRange(tempIds);
                    }
                }
            }

            foreach (int employeeId in employeeIds)
            {
                if (employeeListItems.All(item => item.Value != employeeId.ToString(CultureInfo.InvariantCulture)))
                {
                    var employee = this.GetCheckAndPayEmployee(employees, employeeId, currentUser, accessRoles);
                    if (employee != null)
                    {
                        employeeListItems.Add(new ListItem(employee.FullName, employee.EmployeeID.ToString(CultureInfo.InvariantCulture)));
                    }
                }
            }

            return employeeListItems;
        }

        /// <summary>
        /// Gets the last X claimant selections for his/her approver during ther sumbit process. This function will also validate that the approvers are not archived and they are valid 
        /// approvers in this instance by checking against a list of useable approvers passed in as a variable.
        /// The returned list is sorted alphabetically by firstname, surname and the last selected approver is set to "selected" if valid.
        /// </summary>
        /// <param name="user">
        /// The current system user.
        /// </param>
        /// <param name="validApprovers">
        /// The valid approvers for the claim submital.
        /// </param>
        /// <returns>
        /// A list of ListItem to be used in a dropdown list.
        /// </returns>
        private List<ListItem> GetLastXClaimantSelectedApprovers(ICurrentUserBase user, List<ListItem> validApprovers)
        {
            var selections = new List<ListItem>();
            var subaccs = new cAccountSubAccounts(user.AccountID);
            var subacc = subaccs.getSubAccountById(user.CurrentSubAccountId);
            int numberOfApprovers =
                subacc.SubAccountProperties.NumberOfApproversToRememberForClaimantInApprovalMatrixClaim;

            List<int> validApproverRecords = (from x in validApprovers select Convert.ToInt32(x.Value)).ToList();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                connection.AddWithValue("@employeeId", user.EmployeeID);
                connection.AddWithValue("@numberOfApprovers", numberOfApprovers);
                connection.AddWithValue("@approverList", validApproverRecords);

                using (var reader = connection.GetReader("GetLastXClaimantApprovers", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        int lastSelectedApproverIdOrdinal = reader.GetOrdinal("LastSelectedApproverId");
                        int lastSelectedApproverId = reader.GetInt32(lastSelectedApproverIdOrdinal);

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                int approverOrdinal = reader.GetOrdinal("Approver");
                                int approverIdOrdinal = reader.GetOrdinal("ApproverId");
                                int approverId = reader.GetInt32(approverIdOrdinal);

                                if (
                                    validApprovers.FirstOrDefault(
                                        x => x.Value == approverId.ToString(CultureInfo.InvariantCulture)) == null)
                                {
                                    continue;
                                }

                                // this is a valid approver to add
                                var item = new ListItem(
                                    reader.GetString(approverOrdinal), approverId.ToString(CultureInfo.InvariantCulture));

                                if (approverId == lastSelectedApproverId)
                                {
                                    // the last claimant selected approver is valid.
                                    item.Selected = true;
                                }

                                selections.Add(item);
                            }
                        }
                    }

                    reader.Close();
                }

                connection.sqlexecute.Parameters.Clear();
            }

            if (selections.Count > 0)
            {
                // add a seperator with no value for the list.
                selections.Add(new ListItem("#ENDFAVOURITES#", "-99"));
            }

            return selections;
        }

        /// <summary>
        /// The get check and pay employee.
        /// </summary>
        /// <param name="employees">
        /// The employees.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="accessRoles">
        /// The access roles.
        /// </param>
        /// <returns>
        /// The <see cref="cEmployee"/>.
        /// </returns>
        private Employee GetCheckAndPayEmployee(cEmployees employees, int employeeId, ICurrentUser user, cAccessRoles accessRoles)
        {

            Employee employee = employees.GetEmployeeById(employeeId);
            List<int> employeeAccessRoles = employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

            foreach (int i in employeeAccessRoles)
            {
                var role = accessRoles.GetAccessRoleByID(i);
                if (role.ElementAccess.ContainsKey(SpendManagementElement.CheckAndPay))
                {
                    return employee;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the levels associated to this matrix.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private SortedList<int, List<ApprovalMatrixLevel>> GetLevels()
        {
            this.DbConnection = new DBConnection(cAccounts.getConnectionString(this.AccountId));
            var result = new SortedList<int, List<ApprovalMatrixLevel>>();
            List<ApprovalMatrixLevel> levels;

            using (var reader = this.DbConnection.GetReader(ApprovalMatrixLevel.SqlItems))
            {
                var approvalMatrixLevelIdOrd = reader.GetOrdinal("approvalMatrixLevelId");
                var approvalMatrixIdOrd = reader.GetOrdinal("approvalMatrixId");
                var approvalLimitOrd = reader.GetOrdinal("approvalLimit");
                var approverEmployeeIdOrd = reader.GetOrdinal("approverEmployeeId");
                var approverBudgetHolderIdOrd = reader.GetOrdinal("approverBudgetHolderId");
                var approverTeamIdOrd = reader.GetOrdinal("approverTeamId");

                while (reader.Read())
                {
                    var approvalMatrixLevelId = reader.GetInt32(approvalMatrixLevelIdOrd);
                    var approvalMatrixId = reader.GetInt32(approvalMatrixIdOrd);
                    result.TryGetValue(approvalMatrixId, out levels);
                    if (levels == null)
                    {
                        levels = new List<ApprovalMatrixLevel>();
                        result.Add(approvalMatrixId, levels);
                    }

                    var approvalLimit = reader.GetDecimal(approvalLimitOrd);
                    int? approverEmployeeId = null;
                    int? approverBudgetHolderId = null;
                    int? approverTeamId = null;
                    if (!reader.IsDBNull(approverEmployeeIdOrd))
                    {
                        approverEmployeeId = reader.GetInt32(approverEmployeeIdOrd);
                    }

                    if (!reader.IsDBNull(approverBudgetHolderIdOrd))
                    {
                        approverBudgetHolderId = reader.GetInt32(approverBudgetHolderIdOrd);
                    }

                    if (!reader.IsDBNull(approverTeamIdOrd))
                    {
                        approverTeamId = reader.GetInt32(approverTeamIdOrd);
                    }

                    levels.Add(new ApprovalMatrixLevel(approvalMatrixLevelId, approvalMatrixId, approvalLimit, approverEmployeeId, approverTeamId, approverBudgetHolderId));
                }

                reader.Close();
            }

            return result;
        }

        /// <summary>
        /// The find levels.
        /// </summary>
        /// <param name="matrixId">
        /// The matrix id.
        /// </param>
        /// <param name="levelList">
        /// The level list.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        private List<ApprovalMatrixLevel> FindLevels(int matrixId, SortedList<int, List<ApprovalMatrixLevel>> levelList)
        {
            List<ApprovalMatrixLevel> levels;
            if (!levelList.TryGetValue(matrixId, out levels))
            {
                levels = new List<ApprovalMatrixLevel>();
            }
            return levels;
        }

        #endregion

        public static decimal GetClaimantsLevel(int employeeId, ApprovalMatrix matrix, cStage stage, cBudgetholders budgetholders, cTeams teams)
        {
            foreach (ApprovalMatrixLevel matrixLevel in matrix.ApprovalMatrixLevels)
            {
                if (matrixLevel.ApproverEmployeeId == employeeId)
                {
                    return matrixLevel.ApprovalLimit + 1;
                }

                if (matrixLevel.ApproverBudgetHolderId != null)
                {
                    var budget = budgetholders.getBudgetHolderById((int)matrixLevel.ApproverBudgetHolderId);
                    if (budget != null && budget.employeeid == employeeId)
                    {
                        return matrixLevel.ApprovalLimit + 1;
                    }
                }

                if (matrixLevel.ApproverTeamId != null)
                {
                    var team = teams.GetTeamById((int)matrixLevel.ApproverTeamId);
                    if (team != null && team.teammembers.Contains(employeeId))
                    {
                        return matrixLevel.ApprovalLimit + 1;
                    }
                }
            }

            return -1;
        }
    }
}
