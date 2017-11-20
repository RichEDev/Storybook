namespace Spend_Management
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;

    #endregion

    /// <summary>
    ///     Summary description for costcodes.
	/// </summary>
	public class cCostcodes
	{
        #region Fields
        
        //private readonly IDBConnection expdata;
        /// <summary>
        ///     The expdata.
        /// </summary>
        /// <summary>
        ///     The n accountid.
        /// </summary>
        private readonly int _accountId;

        /// <summary>
        ///     The n current sub account id.
        /// </summary>
        private readonly int? _currentSubAccountId;

        /// <summary>
        ///     The list.
        /// </summary>
        private SortedList<int, cCostCode> _costcodeList;
        
        /// <summary>
        ///     Sql string.
        /// </summary>
        private string _sql;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initialises a new instance of the <see cref="cCostcodes" /> class.
        /// </summary>
        /// <param name="accountid">
        ///     The accountid.
        /// </param>
        /// <param name="dbconnection">
        ///     The dbconnection.
        /// </param>
        public cCostcodes(int accountid, IDBConnection dbconnection = null)
		{
            this._accountId = accountid;
            var subAccounts = new cAccountSubAccounts(this._accountId, dbconnection);
            this._currentSubAccountId = subAccounts.getFirstSubAccount().SubAccountID;
            this.InitialiseData();
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="cCostcodes" /> class.
        /// </summary>
        /// <param name="accountId">
        ///     The accountid.
        /// </param>
        public cCostcodes(int accountId)
        {
            this._accountId = accountId;
            var subAccounts = new cAccountSubAccounts(this._accountId);
            this._currentSubAccountId = subAccounts.getFirstSubAccount().SubAccountID;
            this.InitialiseData();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the accountid.
        /// </summary>
        public int AccountId
        {
            get
            {
                return this._accountId;
        }
        }

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this._costcodeList.Count;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The cache list.
        /// </summary>
        /// <returns>
        ///     The <see cref="SortedList" />.
        /// </returns>
        public SortedList<int, cCostCode> CacheList(IDBConnection connection = null)
		{
            var costcodeList = new SortedList<int, cCostCode>();
            using (var databaseConnection = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.AccountId)))
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

                this._sql =
                "SELECT ModifiedBy, ModifiedOn, CreatedBy, CreatedOn, archived, description, costcode, costcodeid, ownerEmployeeId, ownerTeamId, ownerBudgetHolderId from dbo.[costcodes] order by costcode";
            string cacheSql =
                string.Format(
                    "SELECT ModifiedBy, ModifiedOn, CreatedBy, CreatedOn FROM dbo.[costcodes] WHERE {0} = {0}", 
                        this.AccountId);
                databaseConnection.sqlexecute.CommandText = cacheSql;

                using (IDataReader reader = databaseConnection.GetReader(this._sql))
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
                        string description = !reader.IsDBNull(descriptionOrdinal) ? reader.GetString(descriptionOrdinal) : string.Empty;
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

                        DateTime createdOn = !reader.IsDBNull(createdOnOrdinal) ? reader.GetDateTime(createdOnOrdinal) : new DateTime(1900, 01, 01);
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
                    this.SetOwnerDescription(ref curcostcode);
                        costcodeList.Add(costcodeId, curcostcode);
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
        public List<ListItem> CreateDropDown(bool useDescription, bool includeNoneOption = false)
        {
            var items = new List<ListItem>();
            SortedList<string, cCostCode> sorted = useDescription ? this.SortListByDescription() : this.SortList();

            foreach (cCostCode costCode in sorted.Values.Where(code => !code.Archived))
            {
                string description = useDescription ? costCode.Description : costCode.Costcode;
                items.Add(new ListItem(description, costCode.CostcodeId.ToString(CultureInfo.InvariantCulture)));
            }

            if (includeNoneOption)
            {
                items.Insert(0, new ListItem("[None]", "0"));
            }

            return items;
        }

        /// <summary>
        ///     The create string drop down.
        /// </summary>
        /// <param name="costcodeId">
        ///     The costcodeid.
        /// </param>
        /// <param name="readOnly">
        ///     The read only.
        /// </param>
        /// <param name="blank">
        ///     The blank.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string CreateStringDropDown(int costcodeId, bool readOnly, bool blank)
        {
            var output = new StringBuilder();
            var properties = new cMisc(this.AccountId);
            var clsproperties = properties.GetGlobalProperties(this.AccountId);
            bool useDescription = clsproperties.usecostcodedesc;
            output.Append("<select name=costcode");

            if (readOnly)
			{
                output.Append(" disabled");
            }
				
            output.Append(">");

            if (blank)
				{
                output.Append("<option value=\"0\"");

                if (costcodeId == 0)
				{
                    output.Append(" selected");
				}
            
                output.Append("></option>");
		}
		
            SortedList<string, cCostCode> sortedCostcodeList = useDescription ? this.SortListByDescription() : this.SortList();

            foreach (cCostCode costcode in sortedCostcodeList.Values.Where(reqcode => !reqcode.Archived))
        {
                output.Append("<option value=\"" + costcode.CostcodeId + "\"");

                if (costcodeId == costcode.CostcodeId)
            {
                        output.Append(" selected");
            }

                    output.Append(">");
                output.Append(useDescription ? costcode.Description : costcode.Costcode);
                    output.Append("</option>");
                }

            output.Append("</select>");
            return output.ToString();
        }

        /// <summary>
        ///  Gets a list of active cost code data.
        /// </summary>
        /// <returns>A list of active <see cref="cCostCode">cCostCode</see>.</returns>
        public List<cCostCode> GetAllActiveCostCodes()
        {
            return this._costcodeList.Values.Where(costcode => !costcode.Archived).OrderBy(code => code.Costcode).ToList();
        }

        /// <summary>
        ///     The get cost code by id.
        /// </summary>
        /// <param name="costcodeid">
        ///     The costcodeid.
        /// </param>
        /// <returns>
        ///     The <see cref="cCostCode" />.
        /// </returns>
        public virtual cCostCode GetCostcodeById(int costcodeid)
            {
            cCostCode costcode;
            this._costcodeList.TryGetValue(costcodeid, out costcode);
            return costcode;
            }

        /// <summary>
        ///     The initialise data.
        /// </summary>
        public void InitialiseData()
            {
            this._costcodeList = this.CacheList();
            }

        /// <summary>
        ///     The change status.
        /// </summary>
        /// <param name="costcodeId">
        ///     The costcodeid.
        /// </param>
        /// <param name="archive">
        ///     The archive.
        /// </param>
        /// <param name="connection">Optional connection object</param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int ChangeStatus(int costcodeId, bool archive, IDBConnection connection = null)
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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
                }
                else
                {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.AddWithValue("@costcodeid", costcodeId);
                databaseConnection.sqlexecute.Parameters.AddWithValue("@archive", Convert.ToByte(archive));
                databaseConnection.sqlexecute.Parameters.Add("@returncode", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returncode"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("changeCostcodeStatus");
                returnCode = (int)databaseConnection.sqlexecute.Parameters["@returncode"].Value;
            }

            return returnCode == 1 ? -1 : 0;

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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
            }
            else
            {
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
                }

                databaseConnection.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
                databaseConnection.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
                databaseConnection.ExecuteProc("deleteCostcode");
                returnValue = (int)databaseConnection.sqlexecute.Parameters["@returnvalue"].Value;
            }


            if (returnValue == 0)
            {
                this._costcodeList.Remove(costcodeid);
            }

            return returnValue;
		}

        /// <summary>
        ///     The get column list.
        /// </summary>
        /// <returns>
        ///     The <see cref="cColumnList" />.
        /// </returns>
        public cColumnList GetColumnList()
		{
            var properties = new cMisc(this.AccountId);
            var clsproperties = properties.GetGlobalProperties(this.AccountId);
            bool useDescription = clsproperties.usedepartmentdesc;
            var columnList = new cColumnList();

            columnList.addItem(0, string.Empty);

            foreach (cCostCode costcode in this._costcodeList.Values)
                {
                columnList.addItem(costcode.CostcodeId, useDescription ? costcode.Description : costcode.Costcode);
            }

            return columnList;
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
        public cCostCode GetCostcodeByDescription(string description)
                {
            return this._costcodeList.Values.FirstOrDefault(costcode => costcode.Description == description);
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
        public cCostCode GetCostcodeByString(string costcode)
                {
            return this._costcodeList.Values.FirstOrDefault(cc => cc.Costcode == costcode);
        }

        /// <summary>
        ///     The get costcode ids.
        /// </summary>
        /// <returns>
        ///     The <see cref="ArrayList" />.
        /// </returns>
        public ArrayList GetCostcodeIds()
        {
            var ids = new ArrayList();
            foreach (cCostCode val in this._costcodeList.Values)
		{
                ids.Add(val.CostcodeId);
            }
			
            return ids;
        }
			
        /// <summary>
        ///     The get grid.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public readonly string GridSql = "select costcodeid, costcode, description, archived from costcodes";

        /// <summary>
        ///     The get modified categories.
        /// </summary>
        /// <param name="date">
        ///     The date.
        /// </param>
        /// <returns>
        ///     The <see cref="ArrayList" />.
        /// </returns>
        public ArrayList GetModifiedCategories(DateTime date)
			{
            var arrayList = new ArrayList();

            foreach (cCostCode costcode in this._costcodeList.Values.Where(cc => cc.CreatedOn > date || cc.ModifiedOn > date))
				{
                arrayList.Add(costcode);
				}

            return arrayList;
			}

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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerEmployeeId", costcode.OwnerEmployeeId.Value);
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
                    databaseConnection.sqlexecute.Parameters.AddWithValue("@ownerBudgetHolderId", costcode.OwnerBudgetHolderId.Value);
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
                        databaseConnection.sqlexecute.Parameters.AddWithValue("@delegateID", currentUser.Delegate.EmployeeID);
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
                returnId = (int)databaseConnection.sqlexecute.Parameters["@id"].Value;
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
                elementId: (int)SpendManagementElement.CostCodes,
                record: costcode.Costcode);
            
                if (this._costcodeList.ContainsKey(returnId))
            {
                    this._costcodeList[returnId] = new cCostCode(
                        returnId,
                        costcode.Costcode,
                        costcode.Description,
                        costcode.Archived,
                        costcode.CreatedOn,
                        costcode.CreatedBy,
                        costcode.ModifiedOn,
                        costcode.ModifiedBy,
                        costcode.UserdefinedFields,
                        costcode.OwnerEmployeeId,
                        costcode.OwnerTeamId,
                        costcode.OwnerBudgetHolderId);
            }
            else
                {
                    this._costcodeList.Add(
                        returnId,
                    new cCostCode(
                            returnId,
                            costcode.Costcode,
                            costcode.Description,
                            costcode.Archived,
                            costcode.CreatedOn,
                            costcode.CreatedBy,
                            costcode.ModifiedOn,
                            costcode.ModifiedBy,
                            costcode.UserdefinedFields,
                            costcode.OwnerEmployeeId,
                            costcode.OwnerTeamId,
                            costcode.OwnerBudgetHolderId));
                }
                }

            return returnId;
            }

        #endregion

        #region Methods

        /// <summary>
        ///     The set owner description.
        /// </summary>
        /// <param name="code">
        ///     The code.
        /// </param>
        private void SetOwnerDescription(ref cCostCode code)
        {
            IOwnership owner = Ownership.Parse(this._accountId, this._currentSubAccountId, code.CombinedOwnerKey);
            string ownerDescription = string.Empty;

            if (owner != null)
        {
                ownerDescription = owner.ItemDefinition();
            }

            code.OwnerDescription = ownerDescription;
        }

        /// <summary>
        ///     The sort list.
        /// </summary>
        /// <returns>
        ///     The <see cref="SortedList" />.
        /// </returns>
        private SortedList<string, cCostCode> SortList()
        {
            var sorted = new SortedList<string, cCostCode>();
            foreach (cCostCode costcode in this._costcodeList.Values.Where(costcode => !sorted.ContainsKey(costcode.Costcode)))
                {
                sorted.Add(costcode.Costcode, costcode);
        }

            return sorted;
        }
        
        /// <summary>
        ///     The sort list by description.
        /// </summary>
        /// <returns>
        ///     The <see cref="SortedList" />.
        /// </returns>
        private SortedList<string, cCostCode> SortListByDescription()
        {
            var sorted = new SortedList<string, cCostCode>();

            foreach (cCostCode costcode in this._costcodeList.Values.Where(costcode => !sorted.ContainsKey(costcode.Description)))
            {
                sorted.Add(costcode.Description, costcode);
	}

            return sorted;
        }

        #endregion
    }
}
