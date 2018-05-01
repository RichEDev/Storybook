namespace Spend_Management
{
    #region Using Directives
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Helpers.EnumDescription;
    using SpendManagementLibrary.Interfaces;
    #endregion

    /// <summary>
    ///     Summary description for costcodes.
    /// </summary>
    public class cCostcodes
    {
        #region Fields

        /// <summary>
        /// The expdata.
        /// </summary>
        /// <summary>
        /// The n accountid.
        /// </summary>
        private readonly int _accountId;

        /// <summary>
        /// The n current sub account id.
        /// </summary>
        private readonly int? _currentSubAccountId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initialises a new instance of the <see cref="cCostcodes" /> class.
        /// </summary>
        /// <param name="accountId">
        /// The accountid.
        /// </param>
        public cCostcodes(int accountId)
        {
            this._accountId = accountId;
            var subAccounts = new cAccountSubAccounts(this._accountId);
            this._currentSubAccountId = subAccounts.getFirstSubAccount().SubAccountID;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the accountid.
        /// </summary>
        public int AccountId
        {
            get { return this._accountId; }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Get coscodes
        /// </summary>
        /// <param name="includeArchived">Whether to include archived costcodes</param>
        /// <param name="id">If a single costcode is required then pass the id</param>
        /// <param name="connection">An instance of <see cref="IDBConnection"/></param>
        /// <returns>A list of <see cref="cCostCode"/></returns>
        public List<cCostCode> Get(bool includeArchived, int id = 0, IDBConnection connection = null)
        {
            var costcodeList = new List<cCostCode>();

            using (var databaseConnection =
                connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                var userdefinedFields = new cUserdefinedFields(this.AccountId);
                var tables = new cTables(this.AccountId);
                var fields = new cFields(this.AccountId);
                cTable costcodeTable = tables.GetTableByID(new Guid("02009E21-AA1D-4E0D-908A-4E9D73DDFBDF"));
                cTable userdefinedFieldsTable = tables.GetTableByID(costcodeTable.UserDefinedTableID);
                SortedList<int, SortedList<int, object>> userdefinedFieldsList = userdefinedFields.GetAllRecords(
                    userdefinedFieldsTable,
                    tables,
                    fields);

                var sql = 
                    new StringBuilder("SELECT ModifiedBy, ModifiedOn, CreatedBy, CreatedOn, archived, description, costcode, costcodeid, ownerEmployeeId, ownerTeamId, ownerBudgetHolderId FROM dbo.[costcodes]");

                if (id > 0 || !includeArchived)
                {
                    sql.Append(" WHERE");
                }


                if (id > 0)
                {
                    sql.Append(" costcodeid = @CostCodeId");

                    databaseConnection.AddWithValue("@CostCodeId", id);
                }

                if (id > 0 && !includeArchived)
                {
                    sql.Append(" AND");
                }

                if (!includeArchived)
                {
                    sql.Append(" archived = 0");
                }

                sql.Append(" ORDER BY costcode");

                using (IDataReader reader = databaseConnection.GetReader(sql.ToString()))
                {
                    int ccIdOrdinal = reader.GetOrdinal("costcodeid");
                    int ccOrdinal = reader.GetOrdinal("costcode");
                    int descriptionOrdinal = reader.GetOrdinal("description");
                    int archivedOrdinal = reader.GetOrdinal("archived");
                    int ownerEmployeeIdOrdinal = reader.GetOrdinal("ownerEmployeeId");
                    int ownerTeamIdOrdinal = reader.GetOrdinal("ownerTeamId");
                    int ownerBudgetHolderIdOrdinal = reader.GetOrdinal("ownerBudgetHolderId");
                    int createdOnOrdinal = reader.GetOrdinal("createdon");
                    int createdByOrdinal = reader.GetOrdinal("createdby");
                    int modifiedOnOrdinal = reader.GetOrdinal("modifiedon");
                    int modifiedByOrdinal = reader.GetOrdinal("modifiedby");

                    while (reader.Read())
                    {
                        int costcodeId = reader.GetInt32(ccIdOrdinal);
                        string costcode = reader.GetString(ccOrdinal);
                        string description = !reader.IsDBNull(descriptionOrdinal)
                            ? reader.GetString(descriptionOrdinal)
                            : string.Empty;
                        bool archived = reader.GetBoolean(archivedOrdinal);

                        int? ownerEmployeeId;
                        if (!reader.IsDBNull(ownerEmployeeIdOrdinal))
                        {
                            ownerEmployeeId = reader.GetInt32(ownerEmployeeIdOrdinal);
                        }
                        else
                        {
                            ownerEmployeeId = null;
                        }

                        int? ownerTeamId;
                        if (!reader.IsDBNull(ownerTeamIdOrdinal))
                        {
                            ownerTeamId = reader.GetInt32(ownerTeamIdOrdinal);
                        }
                        else
                        {
                            ownerTeamId = null;
                        }

                        int? ownerBudgetHolderId;
                        if (!reader.IsDBNull(ownerBudgetHolderIdOrdinal))
                        {
                            ownerBudgetHolderId = reader.GetInt32(ownerBudgetHolderIdOrdinal);
                        }
                        else
                        {
                            ownerBudgetHolderId = null;
                        }

                        DateTime createdOn = !reader.IsDBNull(createdOnOrdinal)
                            ? reader.GetDateTime(createdOnOrdinal)
                            : new DateTime(1900, 01, 01);

                        int createdBy = !reader.IsDBNull(createdByOrdinal) ? reader.GetInt32(createdByOrdinal) : 0;

                        DateTime? modifiedOn;
                        if (!reader.IsDBNull(modifiedOnOrdinal))
                        {
                            modifiedOn = reader.GetDateTime(modifiedOnOrdinal);
                        }
                        else
                        {
                            modifiedOn = null;
                        }

                        int? modifiedBy;
                        if (!reader.IsDBNull(modifiedByOrdinal))
                        {
                            modifiedBy = reader.GetInt32(modifiedByOrdinal);
                        }
                        else
                        {
                            modifiedBy = null;
                        }

                        SortedList<int, object> userdefined;
                        userdefinedFieldsList.TryGetValue(costcodeId, out userdefined);

                        if (userdefined == null)
                        {
                            userdefined = new SortedList<int, object>();
                        }

                        var curcostcode = new cCostCode(
                            costcodeId,
                            costcode,
                            description,
                            archived,
                            createdOn,
                            createdBy,
                            modifiedOn,
                            modifiedBy,
                            userdefined,
                            ownerEmployeeId,
                            ownerTeamId,
                            ownerBudgetHolderId);

                        costcodeList.Add(curcostcode);
                    }

                    reader.Close();
                }
            }

            return costcodeList;
        }

        /// <summary>
        ///     Creates a list of the web control ListItem containing cost code information.
        /// </summary>
        /// <param name="useDescription">Wether to use description or name of cost code</param>
        /// <param name="includeNoneOption">Whether the [None] option should be created also</param>
        /// <returns></returns>
        public List<ListItem> CreateDropDown(bool useDescription, bool includeNoneOption = false,
            IDBConnection connection = null)
        {
            var items = new List<ListItem>();

            using (var databaseConnection =
                connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                StringBuilder sql = new StringBuilder("SELECT costcodeid, ");

                sql.Append(useDescription ? "description" : "costcode");

                sql.Append(" FROM costcodes WHERE archived = 0 ORDER BY ");

                sql.Append(useDescription ? "description" : "costcode");

                using (IDataReader reader = databaseConnection.GetReader(sql.ToString()))
                {
                    while (reader.Read())
                    {
                        items.Add(new ListItem(reader.GetString(1), reader.GetInt32(0).ToString()));
                    }
                }

                if (includeNoneOption)
                {
                    items.Insert(0, new ListItem("[None]", "0"));
                }

                return items;
            }
        }

        /// <summary>
        /// Gets a count of the number of CostCodes
        /// </summary>
        /// <returns>The number of CostCodes</returns>
        public int GetCount(IDBConnection connection = null)
        {
            int count = 0;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                var sql = "SELECT COUNT(costcodeid) FROM costcodes";

                count = databaseConnection.ExecuteScalar<int>(sql);
            }

            return count;
        }

        /// <summary>
        ///     Gets a list of active cost code data.
        /// </summary>
        /// <returns>A list of active <see cref="cCostCode">cCostCode</see>.</returns>
        public List<cCostCode> GetAllActiveCostCodes()
        {
            return this.Get(false);
        }

        /// <summary>
        /// The get cost code by id.
        /// </summary>
        /// <param name="costcodeid">
        /// The costcodeid.
        /// </param>
        /// <returns>
        /// The <see cref="cCostCode" />.
        /// </returns>
        public virtual cCostCode GetCostcodeById(int costcodeid)
        {
            return costcodeid == 0 ? null : this.Get(true, costcodeid)[0];
        }

        /// <summary>
        ///     The change status.
        /// </summary>
        /// <param name="costCodeId">
        /// The Id of the cost code to change the status of.
        /// </param>
        /// <param name="archive">Whether to set the cost code archive status to true or false. </param>
        /// <param name="connection">An instance of <see cref="IDBConnection"/>.</param>
        /// <returns>
        /// A <see cref="ChangeCostCodeArchiveStatus"/> with the result of the action.
        /// </returns>
        public ChangeCostCodeArchiveStatus ChangeStatus(int costCodeId, bool archive, IDBConnection connection = null)
        {
            int returnCode;

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();

                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID",
                        currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@costcodeid", costCodeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
                databaseConnection.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("changeCostcodeStatus");

                returnCode = (int)databaseConnection.sqlexecute.Parameters["@returncode"].Value;
            }

            return (ChangeCostCodeArchiveStatus)returnCode;

        }
    
        /// <summary>
        ///     The delete cost code.
        /// </summary>
        /// <param name="costcodeid">
        ///     The costcodeid.
        /// </param>
        /// <param name="employeeid">
        ///     The employeeid.
        /// </param>
        /// <param name="connection">Optional connection object</param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int DeleteCostCode(int costcodeid, int employeeid, IDBConnection connection = null)
        {
            int returnValue;

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();

                databaseConnection.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeid);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);

                if (currentUser.isDelegate)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID",
                        currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("deleteCostcode");
                returnValue = (int) databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
            }

            return returnValue;
        }

        /// <summary>
        ///     The get cost code by desc.
        /// </summary>
        /// <param name="description">
        ///     The desc.
        /// </param>
        /// <returns>
        ///     The <see cref="cCostCode" />.
        /// </returns>
        public int GetCostcodeIdByDescription(string description, IDBConnection connection = null)
        {
            int returnVal = 0;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                string sql = "SELECT costcodeid FROM costcodes WHERE description = @Description";

                databaseConnection.AddWithValue("@Description", description, 4000);

                returnVal = databaseConnection.ExecuteScalar<int>(sql);
            }

            return returnVal;
        }

        /// <summary>
        ///     The get cost code by string.
        /// </summary>
        /// <param name="costcode">
        ///     The costcode.
        /// </param>
        /// <returns>
        ///     The <see cref="cCostCode" />.
        /// </returns>
        public int GetCostcodeIdByName(string costcode, IDBConnection connection = null)
        {
            int returnVal = 0;

            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
            {
                string sql = "SELECT costcodeid FROM costcodes WHERE costcode = @CostCode";

                databaseConnection.AddWithValue("@CostCode", costcode, 50);

                returnVal = databaseConnection.ExecuteScalar<int>(sql);
            }

            return returnVal;
        }

        /// <summary>
        ///     The get grid.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public readonly string GridSql = "SELECT costcodeid, costcode, description, archived FROM costcodes";

        /// <summary>
        ///     The save costcode.
        /// </summary>
        /// <param name="costcode">
        ///     The costcode.
        /// </param>
        /// <param name="connection">OPtional database connection</param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int SaveCostcode(cCostCode costcode, IDBConnection connection = null)
        {
            int returnId;

            using (
                var databaseConnection = connection
                                         ?? new DatabaseConnection(cAccounts.getConnectionString(this._accountId)))
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();

                databaseConnection.sqlexecute.Parameters.AddWithValue("@costcodeid", costcode.CostcodeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@costcode", costcode.Costcode);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@description", costcode.Description);
                if (costcode.ModifiedBy == null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", costcode.CreatedBy);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@date", costcode.CreatedOn);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@userid", costcode.ModifiedBy);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@date", costcode.ModifiedOn);
                }

                if (costcode.OwnerEmployeeId.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerEmployeeId",
                        costcode.OwnerEmployeeId.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerEmployeeId", DBNull.Value);
                }

                if (costcode.OwnerTeamId.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerTeamId", costcode.OwnerTeamId.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerTeamId", DBNull.Value);
                }

                if (costcode.OwnerBudgetHolderId.HasValue)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerBudgetHolderId",
                        costcode.OwnerBudgetHolderId.Value);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerBudgetHolderId", DBNull.Value);
                }

                if (currentUser != null)
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", currentUser.EmployeeID);
                    if (currentUser.isDelegate)
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID",
                            currentUser.Delegate.EmployeeID);
                    }
                    else
                    {
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                    }
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeID", 0);
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.Add("@id", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@id"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("saveCostcode");
                returnId = (int) databaseConnection.sqlexecute.Parameters["@id"].Value;
                databaseConnection.sqlexecute.Parameters.Clear();

                if (returnId < 0)
                {
                    return returnId;
                }

                var tables = new cTables(this.AccountId);
                var fields = new cFields(this.AccountId);
                cTable costcodeTable = tables.GetTableByID(new Guid("02009e21-aa1d-4e0d-908a-4e9d73ddfbdf"));
                var clsuserdefined = new cUserdefinedFields(this.AccountId);

                clsuserdefined.SaveValues(
                    tables.GetTableByID(costcodeTable.UserDefinedTableID),
                    returnId,
                    costcode.UserdefinedFields,
                    tables,
                    fields,
                    currentUser,
                    elementId: (int) SpendManagementElement.CostCodes,
                    record: costcode.Costcode);
            }

            return returnId;
        }

        #endregion
    }
}
