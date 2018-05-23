namespace Spend_Management
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;
    using BusinessLogic.Modules;
    using BusinessLogic.Identity;

    using global::expenses;

    using Microsoft.SqlServer.Server;

	using shared.code;
	using shared.code.ApprovalMatrix;

    using Spend_Management.expenses.code;
    using Spend_Management.expenses.code.Claims;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Claims;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Enumerators;
    using SpendManagementLibrary.Enumerators.Expedite;
    using SpendManagementLibrary.Expedite;
    using SpendManagementLibrary.ExpenseItems;
    using SpendManagementLibrary.Flags;
    using SpendManagementLibrary.Helpers;
    using SpendManagementLibrary.Interfaces;
    using SpendManagementLibrary.Mileage;
	using SpendManagementLibrary.MobileDeviceNotifications;
	using SpendManagementLibrary.UserDefinedFields;

	using Cache = Utilities.DistributedCaching.Cache;
	using Convert = System.Convert;

	public class cClaims
	{
		internal Cache Cache = new Cache();

		internal cGroups groups;

		private readonly int _accountId;

		public const string CacheArea = "defaultclaim";

		const string ExpenseItemsTableId = "D70D9E5F-37E2-4025-9492-3BCF6AA746A8";

        /// <summary>
        /// The list of claims for approvers who are cost code owners, budget holders or belong to a team along with claimant id
        /// </summary>
        private List<ClaimsForApprover> claimsToInclude;

        private readonly Lazy<IDataFactory<IGeneralOptions, int>> _generalOptionsFactory = new Lazy<IDataFactory<IGeneralOptions, int>>(() => FunkyInjector.Container.GetInstance<IDataFactory<IGeneralOptions, int>>());

        public cClaims(int accountId)
        {
            _accountId = accountId;
            this.groups = new cGroups(accountId);
        }

		/// <summary>
		/// Parameterless constructor - DO NOT USE: only to be used by cGridNew InitialseRow event
		/// </summary>
		public cClaims()
		{

		}

		#region properties

		public int accountid
		{
			get
			{
				return this._accountId;
			}
		}

		#endregion

		public virtual cClaim getClaimById(int claimid)
		{
			return getClaimFromDB(claimid);
		}


		private cClaim getClaimFromDB(int claimid)
		{
			cClaim claim;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.AddWithValue("@claimId", claimid);
				claim = ExtractClaimFromSql("GetClaimByClaimId", connection);
				connection.ClearParameters();
			}

			return claim;
		}

		public int getCount()
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			const string sql = "select count(claimid) from claims_base";
			int count = expdata.getcount(sql);
			return count;
		}

		public int getDefaultClaim(ClaimStage stage, int employeeid)
		{
			int claimid = 0;

			if (stage == ClaimStage.Current)
			{
				if (this.Cache.Contains(this.accountid, CacheArea, employeeid.ToString()))
				{
					var cacheId = this.Cache.Get(this.accountid, CacheArea, employeeid.ToString());
					if (cacheId != null && int.TryParse(cacheId.ToString(), out claimid))
					{
						var claim = this.getClaimById(claimid);
						if (claim == null || claim.ClaimStage != stage)
						{
							claimid = 0;
							Cache.Delete(this.accountid, CacheArea, employeeid.ToString());
						}
					}
				}
			}

			if (claimid == 0)
			{
				var expdata = new DBConnection(cAccounts.getConnectionString(this.accountid));
				SqlDataReader reader;
				string strsql = string.Empty;

				switch (stage)
				{
					case ClaimStage.Current:
						strsql = "select top 1 claimid from claims_base where employeeid = @employeeid and submitted = 0 order by claimno";
						break;
					case ClaimStage.Submitted:
						strsql = "select top 1 claimid from claims_base where employeeid = @employeeid and submitted = 1 and paid = 0 order by claimno";
						break;
					case ClaimStage.Previous:
						strsql = "select top 1 claimid from claims_base where employeeid = @employeeid and submitted = 1 and paid = 1 order by claimno";
						break;
				}
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				using (reader = expdata.GetReader(strsql))
				{
					while (reader.Read())
					{
						if (reader.IsDBNull(0) == false)
						{
							claimid = reader.GetInt32(0);
						}
					}
					expdata.sqlexecute.Parameters.Clear();
					reader.Close();
				}

				if (claimid > 0 && stage == ClaimStage.Current)
				{
					this.Cache.Add(this.accountid, CacheArea, employeeid.ToString(), claimid);
				}
				else
				{
					return claimid;
				}
			}

			return claimid;
		}

		public string[] generateClaimHistoryGrid(int claimId, int employeeId)
		{
			cGridNew grid = new cGridNew(accountid, employeeId, "gridHistory", "select claimhistoryid, datestamp, employee, comment, stage, refnum from claimhistoryview");
			grid.KeyField = "claimhistoryid";
			grid.getColumnByName("claimhistoryid").hidden = true;
			grid.getColumnByName("datestamp").ForceSingleLine = true;
			grid.getColumnByName("employee").ForceSingleLine = true;
			grid.getColumnByName("refnum").ForceSingleLine = true;
			cFields clsfields = new cFields(accountid);
			grid.addFilter(clsfields.GetFieldByID(Guid.Parse("c6f212e6-148d-49c6-b33b-b310a47d58bc")), ConditionType.Equals, new object[] { claimId }, null, ConditionJoiner.None);
			return grid.generateGrid();
		}

		public int updateClaim(int claimId, string name, string description, SortedList<int, object> userDefined, int userId)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				DateTime modifiedon = DateTime.Now.ToUniversalTime();
				CurrentUser currentUser = cMisc.GetCurrentUser();

				//get old claim to compare for audit log
				cClaim oldClaim = getClaimById(claimId);

				//does the claim already exist
				connection.sqlexecute.Parameters.AddWithValue("@employeeId", oldClaim.employeeid);
				connection.sqlexecute.Parameters.AddWithValue("@name", name);
				connection.sqlexecute.Parameters.AddWithValue("@claimId", claimId);
				var count = connection.ExecuteScalar<int>("dbo.CheckIfClaimNameAlreadyExists", CommandType.StoredProcedure);
				connection.sqlexecute.Parameters.Clear();

				if (count > 0)
				{
					return -1;
				}

				connection.sqlexecute.Parameters.AddWithValue("@name", name);
				connection.sqlexecute.Parameters.AddWithValue("@description", description.Length > 2000 ? description.Substring(0, 1999) : description);
				connection.sqlexecute.Parameters.AddWithValue("@claimId", claimId);
				connection.sqlexecute.Parameters.AddWithValue("@modifiedOn", modifiedon);
				connection.sqlexecute.Parameters.AddWithValue("@userId", userId);
				connection.ExecuteProc("dbo.UpdateClaim");
				connection.sqlexecute.Parameters.Clear();
				var clstables = new cTables(accountid);
				var clsfields = new cFields(accountid);
				cTable tbl = clstables.GetTableByID(new Guid("0efa50b5-da7b-49c7-a9aa-1017d5f741d0"));
				var clsuserdefined = new cUserdefinedFields(accountid);

				clsuserdefined.SaveValues(clstables.GetTableByID(tbl.UserDefinedTableID), claimId, userDefined, clstables, clsfields, currentUser, elementId: (int)SpendManagementElement.Claims, record: name);

				#region Audit Log

				var clsaudit = new cAuditLog(accountid, userId);

				if (oldClaim.name != name)
				{
					clsaudit.editRecord(claimId, name, SpendManagementElement.Claims, new Guid("71474e06-ab80-482c-88e4-5941b71d06b1"), oldClaim.name, name);
				}

				if (oldClaim.description != description)
				{
					clsaudit.editRecord(claimId, name, SpendManagementElement.Claims, new Guid("eebc6d3f-a14c-4b33-af40-4b894526745c"), oldClaim.description, description);
				}

				#endregion

			}

			return 0;
		}

		/// <summary>
		/// Generates the html for a claims grid for a specified employee. The grid can be filtered by claim stage and also whether the grid is for the claim selector page.
		/// </summary>
		/// <param name="employeeId">The employee id.</param>
		/// <param name="claimStage">The stage of the claim (submitted/approved etc.)</param>
		/// <param name="fromClaimSelector">A boolean specifying whether this call has been made from the claim selector page. Defaults to false.</param>
		/// <param name="claimName">The name of the claim to search results for.</param>
		/// <param name="selectable">Whether the claim line is selectable using a radio button.</param>
		/// <returns>A cGrid new data object array.</returns>
		public string[] GetClaimSummaryGrid(int employeeId, ClaimStage claimStage, bool fromClaimSelector = false, string claimName = "", bool selectable = false)
		{
			var user = cMisc.GetCurrentUser();
			var sql = "select claimid, claimno, [name], description, numberofitems, total, currencyid, employeeid";

			if (claimStage == ClaimStage.Submitted)
			{
				sql += ",datesubmitted";
			}
			else if (claimStage == ClaimStage.Previous || claimStage == ClaimStage.Any)
			{
				sql += ",datesubmitted,datepaid";
			}

			sql += " from claims";

			var fields = new cFields(this.accountid);
			var grid = new cGridNew(this.accountid, employeeId, "gridClaims", sql);

			if (fromClaimSelector && claimName != string.Empty)
			{
				var claimNameField = fields.GetFieldByID(Guid.Parse("71474E06-AB80-482C-88E4-5941B71D06B1")); // claim name
				grid.addFilter(claimNameField, ConditionType.Like, new object[] { claimName }, null, ConditionJoiner.None);

				var filters = FieldFilterGenerator.GetHeirarchyFieldFilters(user);

				for (byte i = 0; i < 2; i++)
				{
					var filter = filters.ContainsKey(i) ? filters[i] : null;
					if (filter == null)
					{
						continue;
					}

					var fieldFilterValues = FieldFilters.GetFilterValuesFromFieldFilter(filter, user);
					grid.addFilter(filters[i].Field, fieldFilterValues.conditionType, fieldFilterValues.valueOne, null, ConditionJoiner.And);
				}
			}
			else
			{
				var employeeIdField = fields.GetFieldByID(Guid.Parse("2501BE3D-AA94-437D-98BB-A28788A35DC4")); // employeeID
				grid.addFilter(employeeIdField, ConditionType.Equals, new object[] { employeeId }, null, ConditionJoiner.None);
			}

			var accessRoleLevel = user.HighestAccessLevel;
			var claimsToInclude = new List<int>();

			if (accessRoleLevel == AccessRoleLevel.EmployeesResponsibleFor)
			{
				if (claimName == string.Empty)
				{
					claimsToInclude = IsInExtendedHierarchy(employeeId, user) ? GetClaimsToInclude(user, employeeId) : getClaimIds(employeeId, false);
				}
				else
				{
					//Viewer is searching by claim name, so get all possible claimantIds who have a match and
					//filter the claims to ensure the viewer has the viwer has permission to viwer them               
					var claimantIds = GetClaimantIdsMatchesForClaimName(claimName);

					foreach (var claimantId in claimantIds)
					{
						claimsToInclude.AddRange(IsInExtendedHierarchy(claimantId, user)
							? GetClaimsToInclude(user, claimantId)
							: getClaimIds(claimantId, false));
					}
				}

				const string claimIdFieldId = "E3AF2B67-A613-437E-AABF-6853C4553977";
				var claimIds = ClaimIds(claimsToInclude);

				//Filter grid by included ClaimIds
				grid.addFilter(fields.GetFieldByID(Guid.Parse(claimIdFieldId)), ConditionType.Equals, claimIds, null, ConditionJoiner.And);
			}

			var submittedField = fields.GetFieldByID(Guid.Parse("47DB6E7D-78AC-4322-8211-359DDCA0C1AB"));   // submitted
			var paidField = fields.GetFieldByID(Guid.Parse("382D575A-CE76-45AE-847A-7D374383E383"));   // paid
			var approvedField = fields.GetFieldByID(Guid.Parse("26ED183C-2233-40D5-BAAE-7C828EC045F3"));   // approved

			switch (claimStage)
			{
				case ClaimStage.Current:
					grid.addFilter(submittedField, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
					grid.EmptyText = "There are no current claims to display.";
					break;
				case ClaimStage.Submitted:
					grid.addFilter(submittedField, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);
					grid.addFilter(paidField, ConditionType.Equals, new object[] { 0 }, null, ConditionJoiner.And);
					grid.EmptyText = "There are no submitted claims to display.";
					break;
				case ClaimStage.Previous:
					grid.addFilter(submittedField, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);
					grid.addFilter(paidField, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);
					grid.addFilter(approvedField, ConditionType.Equals, new object[] { 1 }, null, ConditionJoiner.And);
					grid.EmptyText = "There are no previous claims to display.";
					break;
				case ClaimStage.Any:
					grid.EmptyText = "There are no claims to display.";
					break;
			}

			grid.KeyField = "claimid";
			grid.getColumnByName("claimid").hidden = true;
			grid.getColumnByName("employeeid").hidden = true;
			grid.getColumnByName("currencyid").hidden = true;

			if (selectable)
			{
				grid.EnableSelect = true;
				grid.GridSelectType = GridSelectType.RadioButton;
			}

			if (claimStage == ClaimStage.Current)
			{
				grid.enableupdating = true;
				grid.editlink = "../aeclaim.aspx?action=2&claimid={claimid}";
				grid.enabledeleting = true;
				grid.deletelink = "javascript:SEL.Claims.Summary.DeleteClaim({claimid});";
			}

			var gridInfo = new SerializableDictionary<string, object> { { "accountId", this.accountid }, { "fromClaimSelector", fromClaimSelector }, { "employeeId", employeeId }, { "userId", user.EmployeeID } };
			grid.InitialiseRowGridInfo = gridInfo;
			grid.InitialiseRow += this.claimGrid_InitialiseRow;
			grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cClaims";
			grid.ServiceClassMethodForInitialiseRowEvent = "claimGrid_InitialiseRow";
			return grid.generateGrid();
		}

		/// <summary>
		/// Turns a list of claimsIds into an object of claimIds
		/// If no claimIds to process then return an object containing 0 as we don't want 
		/// and claims returned in the claim summary grid
		/// </summary>
		/// <param name="claimIds"></param>
		/// <returns>An object oif claimIds</returns>
		private static object[] ClaimIds(IList<int> claimIds)
		{

			object[] objClaimIds;

			if (claimIds.Count > 0)
			{
				objClaimIds = new object[claimIds.Count];

				foreach (int claimId in claimIds)
				{
					objClaimIds[claimIds.IndexOf(claimId)] = claimId;
				}
			}
			else
			{
				objClaimIds = new object[] { 0 };
			}

			return objClaimIds;
		}

		/// <summary>
		/// Checks if the employee is in the extended hierarchy or not. 
		/// </summary>
		/// <param name="claimantId"></param>
		/// <param name="user"></param>
		/// <returns>If the claimant is in the extended hierarchy or not</returns>
		public bool IsInExtendedHierarchy(int claimantId, ICurrentUser user)
		{
			bool result = true;
			Guid lookup = new Guid("96f11c6d-7615-4abd-94ec-0e4d34e187a0");
			IEnumerable claimHierarchy = FieldFilters.GetClaimHeirachyList(lookup, user.EmployeeID, user);

			foreach (int employeeId in claimHierarchy.Cast<int>().Where(employeeId => employeeId == claimantId))
			{
				//User is in the original get hierarchy calculation, so therefore, was not retrieved using the extended hierarchy
				result = false;
			}

			return result;
		}

		/// <summary>
		/// Gives claim access status for logged in user.
		/// </summary>
		/// <param name="claim">
		/// Claim to validate.
		/// </param>
		/// <param name="currentUser">
		/// Current logged in user
		/// </param>
		/// <param name="checkUserIsClaimOwner">
		/// Check if claim owner is logged in user.
		/// </param>
		/// <returns>Returns status whether claim is accessible for current logged in user</returns>
		public ClaimToAccessStatus CheckClaimAndOwnership(cClaim claim, ICurrentUser currentUser, bool checkUserIsClaimOwner)
		{
			if (claim == null)
			{
				claim.ClaimAccessStatus = ClaimToAccessStatus.ClaimNotFound;
				return claim.ClaimAccessStatus;
			}

			if (checkUserIsClaimOwner && claim.employeeid == currentUser.EmployeeID || currentUser.Employee.AdminOverride)
			{
				claim.ClaimAccessStatus = ClaimToAccessStatus.Success;
				return claim.ClaimAccessStatus;
			}

			if (!cAccessRoles.CanCheckAndPay(currentUser.AccountID, currentUser.EmployeeID))
			{
				bool hasClaimViewerAccess =
					currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ClaimViewer, true);

				if (hasClaimViewerAccess && currentUser.HighestAccessLevel == AccessRoleLevel.AllData)
				{
					claim.ClaimAccessStatus = ClaimToAccessStatus.Success;
					return claim.ClaimAccessStatus;
				}

				claim.ClaimAccessStatus = ClaimToAccessStatus.InSufficientAccess;
			}
			else
			{
				claim.ClaimAccessStatus = ClaimToAccessStatus.Success;
				return claim.ClaimAccessStatus;
			}

			if (currentUser.EmployeeID == claim.checkerid) return claim.ClaimAccessStatus;
			var claimsToInclude = this.IsInExtendedHierarchy(claim.employeeid, currentUser)
											? this.GetClaimsToInclude(currentUser, claim.employeeid)
											: this.getClaimIds(claim.employeeid, false);

			if (claimsToInclude.Contains(claim.claimid)) return claim.ClaimAccessStatus;
			claim.ClaimAccessStatus = ClaimToAccessStatus.InSufficientAccess;
			return claim.ClaimAccessStatus;
		}

		/// <summary>
		/// Determines which claims the user will be able to see depeneding on if they are the cost code owner,
		/// a memeber of a team who is a cost code owner, or a budget holder
		/// </summary>
		/// <param name="user">An instance of <see cref="ICurrentuserBase"/>representing the current user</param>
		/// <param name="claimantId">The Id of the Claimant to get claims for.</param>
		/// <returns></returns>
		public List<int> GetClaimsToInclude(ICurrentUserBase user, int claimantId)
		{
			var claimIdsToInclude = new List<int>();

			if (this.claimsToInclude == null || !this.claimsToInclude.Any(x => x.ApproverId == user.EmployeeID))
			{
				this.claimsToInclude = this.GetClaimsForApproverWithCostCodeIncluded(user.AccountID, user.EmployeeID);
			}

			var sortedList = this.claimsToInclude.Where(x => x.ClaimantId == claimantId);
			foreach (var claim in sortedList)
			{
				claimIdsToInclude.Add(claim.ClaimId);
			}

			return claimIdsToInclude;
		}


		/// <summary>
		/// Gets claimantIds for a matching ClaimName 
		/// </summary>
		/// <param name="claimName"></param>
		/// <returns>A list of claimantIds</returns>
		private IEnumerable<int> GetClaimantIdsMatchesForClaimName(string claimName)
		{
			var claimantIds = new List<int>();
			var connection = new DBConnection(cAccounts.getConnectionString(this.accountid));
			connection.sqlexecute.Parameters.AddWithValue("@claimName", claimName);

			using (SqlDataReader reader = connection.GetStoredProcReader("GetClaimantIdsMatchesForClaimName"))
			{
				connection.sqlexecute.Parameters.Clear();
				while (reader.Read())
				{
					claimantIds.Add(reader.GetInt32(0));
				}
			}

			return claimantIds;
		}

		/// <summary>
		/// Checks if a cost code is used in a claim
		/// </summary>
		/// <param name="accountId"></param>
		/// <param name="claimId"></param>
		/// <param name="costCodeId"></param>
		/// <returns>If the cost code is used against any items in the claim</returns>
		public bool CheckCostCodeIncluded(int accountId, int claimId, int costCodeId)
		{
			bool hasItems;

			using (var data = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				data.sqlexecute.Parameters.AddWithValue("@costcodeId", costCodeId);
				data.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Bit);
				data.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
				data.ExecuteProc("ClaimHasCostcodeIncluded");
				hasItems = Convert.ToBoolean(data.sqlexecute.Parameters["@returnvalue"].Value);
				data.sqlexecute.Parameters.Clear();
			}

			return hasItems;

		}

		/// <summary>
		/// Gets the list of claim ids for an approver who is a cost code owner or a budget holder or belongs to a team
		/// </summary>
		/// <param name="accountId">The account id that the approver belongs to</param>
		/// <returns>The list of claim ids for the approver</returns>
		public List<ClaimsForApprover> GetClaimsForApproverWithCostCodeIncluded(int accountId, int approverId)
		{
			var claimsToInclude = new List<ClaimsForApprover>();

			var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid));

			connection.sqlexecute.Parameters.AddWithValue("@approverId", approverId);

			using (IDataReader reader = connection.GetReader("GetClaimsForApproverWithCostCodeIncluded", CommandType.StoredProcedure))
			{
				int claimId;
				int claimantId;
				while (reader.Read())
				{
					claimId = reader.GetInt32(0);
					claimantId = reader.GetInt32(1);

					claimsToInclude.Add(new ClaimsForApprover(approverId, claimId, claimantId));
				}
				connection.sqlexecute.Parameters.Clear();
				reader.Close();
			}

			return claimsToInclude;
		}

		/// <summary>
		/// The claim grid_ initialise row event. This happens on creating each row where added to the grids event list.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="gridInfo">
		/// The grid info. Dictionary of information that can be supplied to the event.
		/// </param>
		private void claimGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
		{
			var accountId = (int)gridInfo["accountId"];
			var fromClaimSelector = (bool)gridInfo["fromClaimSelector"];
			var employeeId = (int)gridInfo["employeeId"];
			var userId = (int)gridInfo["userId"];

			if (employeeId == 0)
			{
				employeeId = (int)row.getCellByID("employeeid").Value;
			}

			if (row.getCellByID("currencyid").Value != DBNull.Value)
			{
				var currencies = new cCurrencies(accountId, null);
				cCurrency currency = currencies.getCurrencyById((int)row.getCellByID("currencyid").Value);
				var globalCurrencies = new cGlobalCurrencies();
				cGlobalCurrency globalCurrency = globalCurrencies.getGlobalCurrencyById(currency.globalcurrencyid);
				row.getCellByID("total").Format.Symbol = globalCurrency.symbol;
			}

			string claimSelectorToUrlPart = fromClaimSelector ? string.Format("&employeeid={0}&claimSelector=true", employeeId) : string.Empty;
			string claimSelectorFuction = fromClaimSelector ? string.Format("OnClick='SEL.ClaimSelector.UpdateSelectorURL({0},{1}); return false;'", row.getCellByID("claimid").Value, employeeId) : string.Empty;
			row.getCellByID("name").Value = string.Format("<a href=\"/expenses/claimViewer.aspx?claimid={0}{2}\" {3}>{1}</a>", row.getCellByID("claimid").Value, row.getCellByID("name").Value, claimSelectorToUrlPart, claimSelectorFuction);
		}

        public void changeStatus(cClaim reqclaim, ClaimStatus status, int? userid)
        {
            using (DatabaseConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
            {
                DateTime modifiedon = DateTime.Now.ToUniversalTime();

                expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
                expdata.sqlexecute.Parameters.AddWithValue("@status", (byte)status);
             

                var sql = new StringBuilder();
                sql.Append("UPDATE claims_base SET status = @status");

                if (userid.HasValue)
                {
                    sql.Append(", ModifiedOn = @modifiedon");
                    sql.Append(", ModifiedBy = @userid");                 
                    expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
                    expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
                }

                sql.Append(" WHERE claimid = @claimid");

                expdata.ExecuteSQL(sql.ToString());
                expdata.sqlexecute.Parameters.Clear();
                reqclaim.changeStatus(status);
            }      
        }

		public string getPreviousClaimsDropDown(int employeeid)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();

			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			output.Append("<select onchange=\"previousclaims_onchange(" + employeeid + ");\" name=previousclaims id=previousclaims>");
			output.Append("<option>Please select a claim</option>");

			const string strsql = "select claimid, [name] from dbo.claims where employeeid = @employeeid and submitted = 1 and approved = 1 and paid = 1";
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			using (SqlDataReader reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();

				while (reader.Read())
				{
					output.Append("<option value=\"" + reader.GetInt32(0) + "\">" + reader.GetString(1) + "</option>");
				}

				reader.Close();
			}
			output.Append("</select>");
			return output.ToString();
		}

		public virtual cExpenseItem getExpenseItemById(int id)
		{
			SortedList<int, cExpenseItem> items = this.getExpenseItemsFromDB(expenseId: id);
			if (items.Count == 1)
			{
				return items.Values[0];
			}
			return null;
		}

		/// <summary>
		/// Retrieve one or more expense items from the database
		/// </summary>
		/// <param name="expenseIds">Get a collection of expense items</param>
		/// <returns></returns>
		public virtual SortedList<int, cExpenseItem> getExpenseItemsFromDB(List<int> expenseIds)
		{
			var items = new SortedList<int, cExpenseItem>();

			var account = new cAccounts().GetAccountByID(this.accountid);
			var userDefinedList = new SortedList<int, SortedList<int, object>>();

			foreach (int expenseId in expenseIds)
			{
				userDefinedList.Add(expenseId, this.GetUserDefinedFieldsForUdfCategory(expenseId, ExpenseItemsTableId));
			}

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				connection.AddWithValue("@expenseids", expenseIds);

				using (var reader = connection.GetReader("dbo.GetExpenseItemsFromIds", CommandType.StoredProcedure))
				{
					this.ExpenseItemsFromReader(reader, userDefinedList, account, items);
				}
			}

			return items;
		}

		/// <summary>
		/// Retrieve one or more expense items from the database
		/// </summary>
		/// <param name="claimId">Get all expense items for a specific claim ID</param>
		/// <param name="expenseId">Get a speciaif expense item</param>
		/// <returns></returns>
		public virtual SortedList<int, cExpenseItem> getExpenseItemsFromDB(int? claimId = null, int? expenseId = null)
		{
			var items = new SortedList<int, cExpenseItem>();

			var account = new cAccounts().GetAccountByID(this.accountid);

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				const string ExpenseItemsTableId = "D70D9E5F-37E2-4025-9492-3BCF6AA746A8";
				var userDefinedList = new SortedList<int, SortedList<int, object>>();

				if (expenseId != null)
				{
					userDefinedList.Add(expenseId.Value, this.GetUserDefinedFieldsForUdfCategory(expenseId.Value, ExpenseItemsTableId));
				}

				connection.sqlexecute.Parameters.Clear();
				if (claimId == null)
				{
					connection.sqlexecute.Parameters.AddWithValue("@claimid", DBNull.Value);
				}
				else
				{
					connection.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
				}

				if (expenseId == null)
				{
					connection.sqlexecute.Parameters.AddWithValue("@expenseid", DBNull.Value);
				}
				else
				{
					connection.sqlexecute.Parameters.AddWithValue("@expenseid", expenseId);
				}

				using (var reader = connection.GetReader("dbo.GetExpenseItems ", CommandType.StoredProcedure))
				{
					this.ExpenseItemsFromReader(reader, userDefinedList, account, items);
				}

			}

			return items;
		}

		private void ExpenseItemsFromReader(IDataReader reader, IDictionary<int, SortedList<int, object>> userDefinedList, cAccount account, SortedList<int, cExpenseItem> items)
		{
			var expenseIdOrd = reader.GetOrdinal("expenseid");
			var claimIdOrd = reader.GetOrdinal("claimid");
			var itemTypeOrd = reader.GetOrdinal("itemtype");
			var attendeesOrd = reader.GetOrdinal("attendees");
			var litresOrd = reader.GetOrdinal("blitres");
			var milesOrd = reader.GetOrdinal("bmiles");
			var companyIdOrd = reader.GetOrdinal("organisationIdentifier");
			var convertedTotalOrd = reader.GetOrdinal("convertedtotal");
			var countryIdOrd = reader.GetOrdinal("countryid");
			var currencyIdOrd = reader.GetOrdinal("currencyid");
			var dateOrd = reader.GetOrdinal("date");

			var exchangerateOrd = reader.GetOrdinal("exchangerate");
			var foreignvatOrd = reader.GetOrdinal("foreignvat");
			var homeOrd = reader.GetOrdinal("home");
			var netOrd = reader.GetOrdinal("net");
			var normalreceiptOrd = reader.GetOrdinal("normalreceipt");
			var othersOrd = reader.GetOrdinal("others");
			var plitresOrd = reader.GetOrdinal("plitres");
			var pmilesOrd = reader.GetOrdinal("pmiles");
			var reasonOrd = reader.GetOrdinal("reason");
			var reasonidOrd = reader.GetOrdinal("reasonid");
			var receiptOrd = reader.GetOrdinal("receipt");
			var refnumOrd = reader.GetOrdinal("refnum");
			var returnedOrd = reader.GetOrdinal("returned");
			var correctedOrd = reader.GetOrdinal("corrected");
			var noteOrd = reader.GetOrdinal("note");
			var disputeOrd = reader.GetOrdinal("dispute");
			var staffOrd = reader.GetOrdinal("staff");
			var subcatidOrd = reader.GetOrdinal("subcatid");
			var tempallowOrd = reader.GetOrdinal("tempallow");
			var tipOrd = reader.GetOrdinal("tip");
			var totalOrd = reader.GetOrdinal("total");
			var vatOrd = reader.GetOrdinal("vat");
			var nonightsOrd = reader.GetOrdinal("nonights");
			var noroomsOrd = reader.GetOrdinal("norooms");
			var allowancestartdateOrd = reader.GetOrdinal("allowancestartdate");
			var allowanceenddateOrd = reader.GetOrdinal("allowanceenddate");
			var caridOrd = reader.GetOrdinal("carid");
			var allowancedeductOrd = reader.GetOrdinal("allowancededuct");
			var allowanceidOrd = reader.GetOrdinal("allowanceid");
			var quantityOrd = reader.GetOrdinal("quantity");
			var directorsOrd = reader.GetOrdinal("directors");
			var amountpayableOrd = reader.GetOrdinal("amountpayable");
			var hotelidOrd = reader.GetOrdinal("hotelid");
			var vatnumberOrd = reader.GetOrdinal("vatnumber");
			var personalguestsOrd = reader.GetOrdinal("personalguests");
			var remoteworkersOrd = reader.GetOrdinal("remoteworkers");
			var accountcodeOrd = reader.GetOrdinal("accountcode");
			var basecurrencyOrd = reader.GetOrdinal("basecurrency");
			var globalbasecurrencyOrd = reader.GetOrdinal("globalbasecurrency");
			var globalexchangerateOrd = reader.GetOrdinal("globalexchangerate");
			var globaltotalOrd = reader.GetOrdinal("globaltotal");
			var floatidOrd = reader.GetOrdinal("floatid");
			var primaryitemOrd = reader.GetOrdinal("primaryitem");
			var receiptattachedOrd = reader.GetOrdinal("receiptattached");
			var createdonOrd = reader.GetOrdinal("createdon");
			var createdbyOrd = reader.GetOrdinal("createdby");
			var modifiedonOrd = reader.GetOrdinal("modifiedon");
			var modifiedbyOrd = reader.GetOrdinal("modifiedby");
			var mileageidOrd = reader.GetOrdinal("mileageid");
			var journeyunitOrd = reader.GetOrdinal("journey_unit");
			var transactionidOrd = reader.GetOrdinal("transactionid");
			var assignmentidOrd = reader.GetOrdinal("esrAssignID");
			var hometoofficeDeductionMethodOrd = reader.GetOrdinal("hometooffice_deduction_method");
			var itemCheckerIdOrd = reader.GetOrdinal("itemCheckerId");
			var itemCheckerTeamIdOrd = reader.GetOrdinal("itemCheckerTeamId");
			var validationProgressOrd = reader.GetOrdinal("ValidationProgress");
			var validationCountOrd = reader.GetOrdinal("ValidationCount");
			var editedOrd = reader.GetOrdinal("Edited");
			var paidOrd = reader.GetOrdinal("Paid");
			var originalExpenseIdOrd = reader.GetOrdinal("OriginalExpenseId");
			var bankAccountIdOrd = reader.GetOrdinal("BankAccountId");
			var workAddressIdOrd = reader.GetOrdinal("WorkAddressId");
			var isItemEscalatedOrd = reader.GetOrdinal("IsItemEscalated");

			var OperatorvalidationProgressOrd = reader.GetOrdinal("ExpediteValidationProgress");
			while (reader.Read())
			{
				var expenseid = reader.GetInt32(expenseIdOrd);
				var itemtype = (ItemType)reader.GetByte(itemTypeOrd);
				var attendees = reader.IsDBNull(attendeesOrd) ? string.Empty : reader.GetString(attendeesOrd);
				var currentClaimId = reader.GetInt32(claimIdOrd);
				var blitres = reader.IsDBNull(litresOrd) ? 0 : reader.GetInt32(litresOrd);
				var bmiles = reader.IsDBNull(milesOrd) ? 0 : reader.GetDecimal(milesOrd);

				var companyid = reader.IsDBNull(companyIdOrd) ? 0 : reader.GetInt32(companyIdOrd);
				var convertedtotal = reader.GetDecimal(convertedTotalOrd);
				var countryid = reader.IsDBNull(countryIdOrd) ? 0 : reader.GetInt32(countryIdOrd);

				var currencyid = reader.IsDBNull(currencyIdOrd) ? 0 : reader.GetInt32(currencyIdOrd);
				var date = reader.GetDateTime(dateOrd);

				var exchangerate = reader.GetDouble(exchangerateOrd);
				var foreignvat = reader.GetDecimal(foreignvatOrd);
				var home = reader.GetBoolean(homeOrd);
				var net = reader.GetDecimal(netOrd);
				var normalreceipt = reader.GetBoolean(normalreceiptOrd);

				var others = reader.IsDBNull(othersOrd) ? (byte)0 : reader.GetByte(othersOrd);
				var plitres = reader.IsDBNull(plitresOrd) ? 0 : reader.GetInt32(plitresOrd);
				var pmiles = reader.IsDBNull(pmilesOrd) ? 0 : reader.GetDecimal(pmilesOrd);
				var reason = reader.IsDBNull(reasonOrd) ? string.Empty : reader.GetString(reasonOrd);
				var reasonid = reader.IsDBNull(reasonidOrd) ? 0 : reader.GetInt32(reasonidOrd);
				var receipt = reader.GetBoolean(receiptOrd);
				var refnum = reader.GetString(refnumOrd);

				var returned = reader.GetBoolean(returnedOrd);
				var corrected = reader.IsDBNull(correctedOrd) == false && reader.GetBoolean(correctedOrd);
				var note = reader.IsDBNull(noteOrd) ? string.Empty : reader.GetString(noteOrd);
				var dispute = reader.IsDBNull(disputeOrd) ? string.Empty : reader.GetString(disputeOrd);
				var staff = reader.IsDBNull(staffOrd) ? (byte)0 : reader.GetByte(staffOrd);
				var subcatid = reader.GetInt32(subcatidOrd);
				var tempallow = reader.GetBoolean(tempallowOrd);
				var tip = reader.IsDBNull(tipOrd) ? 0 : reader.GetDecimal(tipOrd);

				var total = reader.GetDecimal(totalOrd);

				var vat = reader.GetDecimal(vatOrd);
				var nonights = reader.IsDBNull(nonightsOrd) ? (byte)0 : reader.GetByte(nonightsOrd);
				var norooms = reader.IsDBNull(noroomsOrd) ? (byte)0 : reader.GetByte(noroomsOrd);
				var allowancestartdate = reader.IsDBNull(allowancestartdateOrd)
											 ? DateTime.Parse("01/01/1900")
											 : reader.GetDateTime(allowancestartdateOrd);
				var allowanceenddate = reader.IsDBNull(allowanceenddateOrd)
										   ? DateTime.Parse("01/01/1900")
										   : reader.GetDateTime(allowanceenddateOrd);

				var carid = reader.IsDBNull(caridOrd) ? 0 : reader.GetInt32(caridOrd);
				var allowancededuct = reader.IsDBNull(allowancedeductOrd) ? 0 : reader.GetDecimal(allowancedeductOrd);
				var allowanceid = reader.IsDBNull(allowanceidOrd) ? 0 : reader.GetInt32(allowanceidOrd);

				var quantity = reader.IsDBNull(quantityOrd) ? 0 : reader.GetDouble(quantityOrd);
				var directors = reader.IsDBNull(directorsOrd) ? (byte)0 : reader.GetByte(directorsOrd);
				var amountpayable = reader.IsDBNull(amountpayableOrd) ? 0 : reader.GetDecimal(amountpayableOrd);
				var hotelid = reader.IsDBNull(hotelidOrd) ? 0 : reader.GetInt32(hotelidOrd);
				var vatnumber = reader.IsDBNull(vatnumberOrd) ? string.Empty : reader.GetString(vatnumberOrd);
				var personalguests = reader.IsDBNull(personalguestsOrd) ? (byte)0 : reader.GetByte(personalguestsOrd);
				var remoteworkers = reader.IsDBNull(remoteworkersOrd) ? (byte)0 : reader.GetByte(remoteworkersOrd);
				var accountcode = reader.IsDBNull(accountcodeOrd) ? string.Empty : reader.GetString(accountcodeOrd);
				var basecurrency = reader.IsDBNull(basecurrencyOrd) ? 0 : reader.GetInt32(basecurrencyOrd);
				var globalbasecurrency = reader.IsDBNull(globalbasecurrencyOrd) ? 0 : reader.GetInt32(globalbasecurrencyOrd);
				var globalexchangerate = reader.IsDBNull(globalexchangerateOrd) ? 0 : reader.GetDouble(globalexchangerateOrd);
				var globaltotal = reader.IsDBNull(globaltotalOrd) ? 0 : reader.GetDecimal(globaltotalOrd);
				var floatid = reader.IsDBNull(floatidOrd) ? 0 : reader.GetInt32(floatidOrd);

				var primaryitem = reader.GetBoolean(primaryitemOrd);
				var receiptattached = reader.GetBoolean(receiptattachedOrd);

				var createdon = reader.IsDBNull(createdonOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdonOrd);
				var createdby = reader.IsDBNull(createdbyOrd) ? 0 : reader.GetInt32(createdbyOrd);
				var modifiedon = reader.IsDBNull(modifiedonOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(modifiedonOrd);
				var modifiedby = reader.IsDBNull(modifiedbyOrd) ? 0 : reader.GetInt32(modifiedbyOrd);
				var mileageid = reader.IsDBNull(mileageidOrd) ? 0 : reader.GetInt32(mileageidOrd);
				var journeyunit = reader.IsDBNull(journeyunitOrd) ? 0 : (MileageUOM)reader.GetByte(journeyunitOrd);
				var transactionid = reader.IsDBNull(transactionidOrd) ? 0 : reader.GetInt32(transactionidOrd);
				var assignmentid = reader.IsDBNull(assignmentidOrd) ? 0 : reader.GetInt32(assignmentidOrd);

				SortedList<int, object> userDefinedFieldsForExpenseItem;
				userDefinedList.TryGetValue(expenseid, out userDefinedFieldsForExpenseItem);

				var hometoofficeDeductionMethod = reader.IsDBNull(hometoofficeDeductionMethodOrd)
													  ? HomeToLocationType.None
											  : (HomeToLocationType)reader.GetByte(hometoofficeDeductionMethodOrd);

				var itemCheckerId = reader.IsDBNull(itemCheckerIdOrd) ? (int?)null : reader.GetInt32(itemCheckerIdOrd);
				var itemCheckerTeamId = reader.IsDBNull(itemCheckerTeamIdOrd)
											 ? null
											 : (int?)reader.GetInt32(itemCheckerTeamIdOrd);

				List<ExpenseValidationResult> validationResults;

				// check if the account has validation enabled.
				int validationCount;
				ExpenseValidationProgress validationProgress;
				ExpediteOperatorValidationProgress operatorValidationProgress;
				if (account.ValidationServiceEnabled)
				{
					validationProgress = (ExpenseValidationProgress)reader.GetInt32(validationProgressOrd);
					operatorValidationProgress = (ExpediteOperatorValidationProgress)reader.GetInt32(OperatorvalidationProgressOrd);
					validationCount = reader.GetInt32(validationCountOrd);
				}
				else
				{
					validationProgress = ExpenseValidationProgress.ValidationServiceDisabled;
					validationCount = 0;
					validationResults = new List<ExpenseValidationResult>();
					operatorValidationProgress = 0;
				}
				var edited = !reader.IsDBNull(editedOrd) && reader.GetBoolean(editedOrd);
				var paid = !reader.IsDBNull(paidOrd) && reader.GetBoolean(paidOrd);
				var originalExpenseId = reader.IsDBNull(originalExpenseIdOrd) ? 0 : reader.GetInt32(originalExpenseIdOrd);
				var bankAccountId = reader.IsDBNull(bankAccountIdOrd) ? 0 : reader.GetInt32(bankAccountIdOrd);
				var workAddressId = reader.IsDBNull(workAddressIdOrd) ? 0 : reader.GetInt32(workAddressIdOrd);
				bool? isItemEscalated = null;
				if (reader.IsDBNull(isItemEscalatedOrd) == false)
				{
					isItemEscalated = reader.GetBoolean(isItemEscalatedOrd);
				}
				var newitem = new cExpenseItem(
					expenseid,
					itemtype,
					bmiles,
					pmiles,
					reason,
					receipt,
					net,
					vat,
					total,
					subcatid,
					date,
					staff,
					others,
					companyid,
					returned,
					home,
					refnum,
					currentClaimId,
					plitres,
					blitres,
					currencyid,
					attendees,
					tip,
					countryid,
					foreignvat,
					convertedtotal,
					exchangerate,
					tempallow,
					reasonid,
					normalreceipt,
					allowancestartdate,
					allowanceenddate,
					carid,
					allowancededuct,
					allowanceid,
					nonights,
					quantity,
					directors,
					amountpayable,
					hotelid,
					primaryitem,
					norooms,
					vatnumber,
					personalguests,
					remoteworkers,
					accountcode,
					basecurrency,
					globalbasecurrency,
					globalexchangerate,
					globaltotal,
					floatid,
					corrected,
					receiptattached,
					transactionid,
					createdon,
					createdby,
					modifiedon,
					modifiedby,
					mileageid,
					journeyunit,
					assignmentid,
					hometoofficeDeductionMethod,
					false,
					0,
					note,
					dispute,
					itemCheckerId,
					itemCheckerTeamId,
					validationProgress,
					validationCount,
					null,
					edited,
					paid,
					originalExpenseId,
					bankAccountId,
					workAddressId,
					isItemEscalated,
					operatorValidationProgress);

				newitem.userdefined = userDefinedFieldsForExpenseItem;

				if (!primaryitem)
				{
					// add to split items
					int parentid = this.getParentId(expenseid);
					cExpenseItem parentItem;
					items.TryGetValue(parentid, out parentItem);
					if (parentItem == null)
					{
						// must be split of a split
						foreach (cExpenseItem temp in items.Values)
						{
							foreach (cExpenseItem split in temp.splititems)
							{
								if (split.expenseid == parentid)
								{
									parentItem = split;
									break;
								}
							}

							if (parentItem != null)
							{
								break;
							}
						}
					}

					newitem.setPrimaryItem(parentItem);
					if (parentItem != null)
					{
						parentItem.splititems.Add(newitem);
					}
				}
				else
				{
					items.Add(expenseid, newitem);
				}
			}

			reader.Close();
		}

		/// <summary>
		/// Gets a list of cExpenses by checking if the primary expense ID has any split item expenses related to it
		/// </summary>
		/// <param name="accountId">
		/// The accountID
		/// </param>
		/// <param name="expenseIds">
		/// Optional param. A list of primary expense item ids being approved.
		/// </param>
		/// <param name="unapprovedExpenseId">
		/// Optional param. The expense id being unapproved.
		/// </param>
		/// <returns>
		/// Returns a list of cExpenses including split expenses related to the primary expense item.
		/// </returns>
		public List<cExpenseItem> GetSplitExpenseItems(int accountId, List<int> expenseIds = null, int unapprovedExpenseId = 0)
		{
			List<cExpenseItem> expenseItems = new List<cExpenseItem>();

			if (unapprovedExpenseId > 0)
			{
				expenseIds = new List<int>();
				expenseIds.Add(unapprovedExpenseId);
			}

			foreach (int primaryExpenseId in expenseIds)
			{
				cExpenseItem expItem = getExpenseItemById(primaryExpenseId);

				if (expItem.splititems.Count > 0)
				{
					foreach (cExpenseItem splitItem in expItem.splititems)
					{
						cExpenseItem tempItem = splitItem;
						expenseItems.Add(tempItem);
					}
				}
				expenseItems.Add(getExpenseItemById(primaryExpenseId));
			}
			return expenseItems;
		}

		public int getParentId(int expenseid)
		{
			SqlDataReader reader;
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int id = 0;
			const string strsql = "select primaryitem from savedexpenses_splititems where splititem = @splititem";
			expdata.sqlexecute.Parameters.AddWithValue("@splititem", expenseid);
			using (reader = expdata.GetReader(strsql))
			{
				while (reader.Read())
				{
					id = reader.GetInt32(0);
				}
				reader.Close();
			}
			expdata.sqlexecute.Parameters.Clear();
			return id;
		}

		/// <summary>
		/// Returns whether the claim includes items with the fueld card mileage calculation type
		/// </summary>
		/// <param name="claimID">
		/// The claim id.
		/// </param>
		/// <param name="connection">
		/// The connection.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool IncludesFuelCardMileage(int claimID, IDBConnection connection = null)
		{
			using (
				IDBConnection data = connection ?? new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				data.sqlexecute.Parameters.AddWithValue("@claimid", claimID);
				int count = data.ExecuteScalar<int>("DoesClaimIncludeFuelCardMileage", CommandType.StoredProcedure);
				if (count == 0)
				{
					return false;
				}
				else
				{
					return true;
				}
			}


		}
		/// <summary>
		/// Deletes an expense item.
		/// </summary>
		/// <param name="reqclaim">
		/// The claim the expense item is in.
		/// </param>
		/// <param name="reqitem">
		/// The expense item to be deleted.
		/// </param>
		/// <param name="approver">
		/// The approver.
		/// </param>
		/// <param name="fromReducedSplit">
		/// Is this expense item being deleted on the back of a split item being reduced.
		/// </param>
		/// <returns>
		/// The <see cref="byte"/>.
		/// </returns>
		public byte deleteExpense(cClaim reqclaim, cExpenseItem reqitem, bool approver, ICurrentUser user, bool fromReducedSplit = false)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cSubcats clssubcats = new cSubcats(accountid);

			int[] expenseids = new int[1];
			int employeeid = reqclaim.employeeid;

			FlagManagement flags = new FlagManagement(this.accountid);
			List<int> expenseflagsToReevaluate =
				flags.GetSavedExpensesRequiringRevalidationAfterDelete(reqitem.expenseid);

			List<cExpenseItem> items = new List<cExpenseItem> { reqitem };

			if (reqclaim.submitted == true && reqitem.returned == true) //add delete comment
			{
				var notifications = new NotificationTemplates(user);

				int[] recipients = new int[1];

				recipients[0] = reqclaim.splitApprovalStage && reqitem.itemCheckerId.HasValue ? reqitem.itemCheckerId.Value : reqclaim.checkerid;
				expenseids[0] = reqitem.expenseid;
				addDeleteComment(reqclaim, reqitem);
				updateReturned(reqclaim, reqitem, user);

				notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToNotifyAdministratorOfAnyReturnedExpensesBeingCorrected), reqclaim.employeeid, recipients, reqitem.expenseid);
			}

			if (approver) //do we need to send email to claimant?
			{
				int[] recipient = new int[1];

				expenseids[0] = reqitem.expenseid;
				recipient[0] = employeeid;

				var notifications = new NotificationTemplates(user);
				var checkerid = 0;

				if (reqclaim.splitApprovalStage && reqitem.itemCheckerId.HasValue)
				{
					checkerid = reqitem.itemCheckerId.Value;
				}
				else
				{
					checkerid = reqclaim.checkerid;
				}

				notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenAnItemHasBeenDeleted), checkerid, recipient, reqitem.expenseid);
			}

			if (reqitem.floatid != 0)
			{
				cFloats clsfloats = new cFloats(accountid);
				clsfloats.deleteAllocation(reqitem.expenseid, reqitem.floatid);
			}

			foreach (cExpenseItem splititem in reqitem.splititems)
			{
				deleteExpense(reqclaim, splititem, approver, user);
			}

			expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
			//strsql = "delete from cardmatches where expenseid = @expenseid;";


			string strsql;
			strsql = "delete from savedexpensesFlagAssociatedExpenses where associatedExpenseId = @expenseId " +
							"delete from [savedexpensesFlags] where duplicateExpenseID = @expenseid and flagtype = 1 " +
							"delete from savedexpenses_splititems where splititem = @expenseid " +
							"delete savedexpenses_journey_steps_passengers where expenseid = @expenseid " +
							"delete from ExpenseValidationResults where expenseid = @expenseid " +
							"delete from savedexpenses where expenseid = @expenseid;";

			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();

			SubcatBasic reqsubcat = clssubcats.GetSubcatBasic(reqitem.subcatid);
			cAuditLog clsaudit = new cAuditLog(accountid, reqclaim.employeeid);

			string auditValue = reqitem.expenseid + "_" + reqitem.date.ToString("dd/MM/yy") + "_" + reqsubcat.Subcat + "_&pound;" + reqitem.total.ToString("###,##,##0.00");
			clsaudit.deleteRecord(SpendManagementElement.Expenses, reqitem.expenseid, auditValue);

			//reevaluate flags
			if (expenseflagsToReevaluate.Count > 0)
			{
				flags.RevalidateAfterDelete(expenseflagsToReevaluate, employeeid, user);
			}
			// delete the claim if no expense items are left
			if (reqclaim.submitted)
			{
				int itemcount = reqclaim.NumberOfItems - 1;

				if (itemcount == 0 && !fromReducedSplit)
				{
					DeleteClaim(reqclaim);
					return 1;
				}
				else
				{
					if (user.Account.ValidationServiceEnabled)
					{
						var groups = new cGroups(this.accountid);
						cGroup group = this.GetGroupForClaim(groups, reqclaim);
						cStage stage = group.stages.Values[reqclaim.stage - 1];

						//if the deleted item was returned after validation failure                    
						if (stage.signofftype == SignoffType.SELValidation && reqitem.returned == true)
						{
							var expenseItems = this.getExpenseItemsFromDB(reqclaim.claimid).Values;

							//check if there are expense items Waiting For Claimant/Validation Completed and Failed 
							if (!expenseItems.Any(item => item.ValidationProgress == ExpenseValidationProgress.WaitingForClaimant || item.ValidationProgress == ExpenseValidationProgress.CompletedFailed || item.ValidationProgress == ExpenseValidationProgress.Required))
							{
								var submission = new ClaimSubmission(accountid, user.EmployeeID);
								submission.SendClaimToNextStage(reqclaim, false, user.EmployeeID, user.EmployeeID, user.EmployeeID, false, true);
							}
						}
					}
				}
			}
			return 0;
		}

		public void addDeleteComment(cClaim reqclaim, cExpenseItem item)
		{

			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			DateTime datestamp = DateTime.Now;

			const string comment = "This expense has been deleted from the claim";

			expdata.sqlexecute.Parameters.AddWithValue("@claimid", item.claimid);
			expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
			expdata.sqlexecute.Parameters.AddWithValue("@refnum", item.refnum);
			expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
			string strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum) " + "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum)";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void addComment(cClaim claim, int employeeid, string comment, string refnum)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			DateTime datestamp = DateTime.Now;

			expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
			expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
			expdata.sqlexecute.Parameters.AddWithValue("@refnum", refnum);
			expdata.sqlexecute.Parameters.AddWithValue("@stage", claim.stage);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			string strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " + "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum,@employeeid)";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void updateReturned(cClaim reqclaim, cExpenseItem reqitem, ICurrentUser userid)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			int count = 0;

			addReturnComment(reqclaim, reqitem);

			expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
			string strsql = "update returnedexpenses set corrected = 1 where expenseid = @expenseid";
			expdata.ExecuteSQL(strsql);
			reqitem.corrected = true;
			int[] recipients = new int[1];
			int[] expenseids = new int[1];
			expenseids[0] = reqitem.expenseid;
			List<cExpenseItem> items = new List<cExpenseItem>();
			items.Add(reqitem);

			strsql = "select count(*) from returnedexpenses where corrected = 0 and expenseid = @expenseid";
			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();

			if (count != 0) return;

			strsql = "SELECT COUNT(savedexpenses.expenseid) FROM savedexpenses INNER JOIN returnedexpenses ON (returnedexpenses.expenseid = savedexpenses.expenseid AND returnedexpenses.corrected = 0) and claimid = (SELECT claimid FROM savedexpenses WHERE expenseid = @expenseID)";
			expdata.sqlexecute.Parameters.AddWithValue("@expenseID", reqitem.expenseid);
			count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();

			if (count != 0) return;

			// if the claim is not submitted then we don't need to change the status of the claim
			if (reqclaim.status != ClaimStatus.None)
			{
				changeStatus(reqclaim, ClaimStatus.ItemCorrectedAwaitingApprover, userid.EmployeeID);
			}

			if (reqclaim.splitApprovalStage)
			{
				if (!reqitem.itemCheckerId.HasValue) return;

				recipients[0] = reqitem.itemCheckerId.Value;
			}
			else
			{
				recipients[0] = reqclaim.checkerid;
			}

			var notifications = new NotificationTemplates(userid);
			notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToNotifyAdministratorOfAnyReturnedExpensesBeingCorrected), reqclaim.employeeid, recipients, reqitem.expenseid);

			this.UpdateApproverLastRemindedDateWhenApproved(reqclaim.claimid, recipients[0]);
		}

		public void addReturnComment(cClaim reqclaim, cExpenseItem reqitem, string reason, int employeeid, int? delegateid)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			expdata.sqlexecute.Parameters.Clear();
			DateTime datestamp = DateTime.Now;

			string refnum = string.Empty;

			string comment = "Expense Returned: " + reason;

			expdata.sqlexecute.Parameters.AddWithValue("@reason", comment);
			if (reqitem == null)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@expenseid", DBNull.Value);
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@expenseid", reqitem.expenseid);
			}
			expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
			expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);


			if (reqitem != null)
			{
				refnum = reqitem.refnum;
			}

			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", delegateid.HasValue ? delegateid.Value : employeeid);

			string sql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " + "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@reason,@stage,'" + refnum + "',@employeeid)";

			expdata.ExecuteSQL(sql);
			expdata.sqlexecute.Parameters.Clear();
		}

		public void addReturnComment(cClaim reqClaim, cExpenseItem reqItem)
		{
			var appInfo = System.Web.HttpContext.Current.ApplicationInstance;

			CurrentUser user = cMisc.GetCurrentUser();
			int employeeId;
			const string comment = "This expense has been amended.";

			if (appInfo.Context.Session == null || appInfo.Session["myid"] == null)
			{
				employeeId = user.EmployeeID;
			}
			else
			{
				employeeId = (int)appInfo.Session["myid"];
			}

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.AddWithValue("@claimId", reqItem.claimid);
				connection.AddWithValue("@stage", reqClaim.stage);
				connection.AddWithValue("@comment", comment);
				connection.AddWithValue("@datestamp", DateTime.Now);
				connection.AddWithValue("@employeeId", employeeId);
				connection.AddWithValue("@refnum", reqItem.refnum);
				connection.AddWithValue("@createdOn", DateTime.Now.ToUniversalTime());
				connection.ExecuteProc("UpdateClaimHistory");
				connection.ClearParameters();
			}
		}


		/// <summary>
		/// Get a count of the number of claims for an employee at the specified stage.
		/// </summary>
		/// <param name="employeeid"></param>
		/// <param name="claimtype"></param>
		/// <returns></returns>
		public int getCount(int employeeid, ClaimStage claimtype)
		{
			int count;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				string sql = string.Empty;

				switch (claimtype)
				{
					case ClaimStage.Current:
						sql = "select count(claimid) from claims_base where employeeid = @employeeid and submitted = 0";
						break;
					case ClaimStage.Submitted:
						sql = "select count(claimid) from claims where employeeid = @employeeid and submitted = 1 and paid = 0";
						break;
					case ClaimStage.Previous:
						sql = "select count(claimid) from claims where employeeid = @employeeid and submitted = 1 and approved = 1 and paid = 1";
						break;
					case ClaimStage.Any:
						sql = "select count(claimid) from claims where employeeid = @employeeid";
						break;
				}

				connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				count = connection.ExecuteScalar<int>(sql);
				connection.sqlexecute.Parameters.Clear();
			}

			return count;
		}


		public List<ListItem> CreateDropDown(int employeeid)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			List<ListItem> items = new List<ListItem>();

			const string strsql = "select claimid, [name] from claims_base where employeeid = @employeeid and submitted = 0 order by claimno";
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

			using (SqlDataReader reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();

				while (reader.Read())
				{
					items.Add(new ListItem(reader.GetString(1), reader.GetInt32(0).ToString()));
				}
				reader.Close();
			}

			return items;
		}

		/// <summary>
		/// Sets an expense item to unapproved
		/// </summary>
		/// <param name="item">The expense item</param>
		/// <returns>The number of rows affected by the action</returns>
		public int UnapproveItem(cExpenseItem item)
		{
			int affectedRows;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
				const string strsql = "update savedexpenses set tempallow = 0 where expenseid = @expenseid";
				affectedRows = connection.ExecuteSQL(strsql);
				connection.sqlexecute.Parameters.Clear();
			}

			item.tempallow = false;
			return affectedRows;
		}

		/// <summary>
		/// Sets the expense items to Approved, providing there are no flagged items that require attention.
		/// </summary>
		/// <param name="accountId">The accountId</param>
		/// <param name="currentUserId">The current user Id</param>
		/// <param name="delegateId">The delegate Id</param>
		/// <param name="claimId">The claim Id</param>
		/// <param name="expenseItemIds">The expense item Ids</param>
		/// <returns>The <see cref="AllowExpenseItemsResult">AllowExpenseItemResult</see>/></returns>
		public AllowExpenseItemsResult AllowExpenseItems(int accountId, int currentUserId, int? delegateId, int claimId, List<int> expenseItemIds)
		{
			var expenseIds = new List<int>();
			List<cExpenseItem> expenseItems = this.GetSplitExpenseItems(accountId, expenseItemIds);
			decimal claimAmount = 0;

			foreach (cExpenseItem expenseItem in expenseItems)
			{
				expenseIds.Add(expenseItem.expenseid);
				claimAmount += expenseItem.total;
			}

			FlaggedItemsManager flagResults = new FlagManagement(accountId).CheckJustificationsHaveBeenProvidedByAuthoriser(claimId);
			cClaim claim = this.getClaimById(claimId);
			Employee authoriser = Employee.Get(currentUserId, accountId);
			Employee claimant = Employee.Get(claim.employeeid, accountId);
			AllowExpenseItemsResult result;

			string logInformation = "AllowExpenseItems" + Environment.NewLine;
			logInformation += "--------------------------" + Environment.NewLine;
			logInformation += string.Format("Account ID: {0}{1}", accountId, Environment.NewLine);
			logInformation += string.Format("Claim ID: {0}{1}", claimId, Environment.NewLine);
			logInformation += string.Format("Claimant: {0} (ID: {1}) {2}", claimant.FullNameUsername, claimant.EmployeeID, Environment.NewLine);
			logInformation += string.Format("Authoriser: {0} (ID: {1}) {2}", authoriser.FullNameUsername, authoriser.EmployeeID, Environment.NewLine);
			logInformation += string.Format("Expense item ID(s): {0}{1}", string.Join(",", expenseItemIds.ToArray()), Environment.NewLine);
			logInformation += string.Format("Expense item total: {0}{1}", claimAmount, Environment.NewLine);
			logInformation += "--------------------------" + Environment.NewLine;

			result = CheckIfApproverJustificationsAreRequired(accountId, flagResults, expenseIds, claim);

			if (result != null)
			{
				logInformation += "Approver justications != null. Returning back to calling method.";
				AuthoriserLevelLogEntry(logInformation, accountId);
				return result;
			}
			else
			{
				// Continue to a allow expense item(s).

				var groups = new cGroups(accountId);
				cGroup group = this.GetGroupForClaim(groups, claim);
				cStage stage = group.stages.Values[claim.stage - 1];
				SignoffType signofftype = stage.signofftype;
				decimal? authoriserLevelAmount = this.GetAuthoriserLevelAmountByEmployee(currentUserId, accountId);
				bool message = false;
				bool noDefaultAuthoriserPresent = false;
				int? commenter = delegateId == null || delegateId == 0 ? currentUserId : (int)delegateId;

				logInformation += string.Format("Signoff group name: {0}{1}", group.groupname, Environment.NewLine);
				logInformation += string.Format("Signoff type: {0}{1}", signofftype.ToString(), Environment.NewLine);
				logInformation += string.Format("Signoff stage number: {0}{1}", stage.stage, Environment.NewLine);
				logInformation += "--------------------------" + Environment.NewLine;

				if (authoriserLevelAmount.HasValue == false || authoriserLevelAmount == null || authoriserLevelAmount == -1)
				{
					// No authoriser level assigned or it is assigned Default Authoriser Level
					// Can approve any limit and forward to next stage if stage exists else allocate for payment if final stage
					if (authoriserLevelAmount == -1)
					{
						if (SignoffType.CostCodeOwner == signofftype || SignoffType.AssignmentSignOffOwner == signofftype)
						{
							this.UpdateCheckerClaimHistory(currentUserId, claim, commenter);
						}
						else
						{
							this.UpdateCheckerIntoDatabase(currentUserId, claim, commenter.Value);
						}
					}

					logInformation += "No authoriser level assigned or user is the default authoriser, no further escalation required, now marking items as approved.";

					if (IsSplitClaimWithMultipleCostCodeOwnersOrSupervisorAssignments(claim, signofftype))
					{
						this.UpdateSplitClaimHistory(currentUserId, claim, commenter);
					}
					this.approveItems(expenseIds);
				}
				else
				{
					logInformation += string.Format("Authoriser level amount: {0}{1}", authoriserLevelAmount.Value, Environment.NewLine);

					// It means current user has authoriser level assigned. Get the authoriser limit.
					// Get the total amount approved by this approver for this claimaint in this month
					var totalApprovedAmountForMonth = this.GetApprovedAmountByChecker(claim, currentUserId);
					if (totalApprovedAmountForMonth.HasValue == false)
					{
						totalApprovedAmountForMonth = 0;
					}
					logInformation += string.Format("Total approved for month: {0}{1}", totalApprovedAmountForMonth, Environment.NewLine);

					// Remaining amount that one can approve this month for that claimant
					var remainingBalance = authoriserLevelAmount.Value - totalApprovedAmountForMonth.Value;
					logInformation += string.Format("Remaining balance: {0}{1}", remainingBalance, Environment.NewLine);

					if (remainingBalance < claimAmount)
					{
						logInformation += string.Format("Remaining balance is less than claim amount ({0} vs {1}){2}", remainingBalance, claimAmount, Environment.NewLine);

						// Check if remaining balance is positive
						// That means one can approve and amount will be deducted from one's balance(no matter if it is negative value after deduction)
						if (remainingBalance > 0)
						{
							// Save the amount in DB and approve
							this.UpdateApproverApprovedBalance(claim, expenseIds, currentUserId);
						}

						// Current approver can approve but will be forwarded to Line Manager if exists, else to next stage
						// Get the line manager
						int? currentApproverLineManagerId = this.GetIdOfLineManagerOfCurrentApprover(currentUserId);

						logInformation += string.Format("Current approver (employee ID: {0}) can approve the claim but it will be forward to their line manager.{1}", currentUserId, Environment.NewLine);


						// It must be progressed to next stage if no line manager exists for current approver.
						// Else if it is in final stage then update the checker or send it default authoriser level.
						// Also when current approver is set to its own line manager. Vivek
						var defaultAuthoriserLevelEmployeeId = this.GetDefaultAuthoriserLevelEmployeeId();

						if (currentApproverLineManagerId.HasValue == false || currentApproverLineManagerId == null || claim.checkerid == currentApproverLineManagerId.Value)
						{
							logInformation += "No line manager present so attempting escalation to default authoriser" + Environment.NewLine;

							//  Remove next line once DAL implemented.
							if (defaultAuthoriserLevelEmployeeId.HasValue == false || defaultAuthoriserLevelEmployeeId.Value == 0)
							{
								logInformation += ".. but there is no default authoriser set, so we can't do anything" + Environment.NewLine;
								// Show an error message and ask to set Default Authoriser Level person first
								noDefaultAuthoriserPresent = true;
							}
							else
							{
								var defaultAuthoriser = Employee.Get(defaultAuthoriserLevelEmployeeId.Value, accountId);
								logInformation += string.Format("The default authoriser employee is {0} (ID: {1}) {2}", defaultAuthoriser.FullNameUsername, defaultAuthoriser.EmployeeID, Environment.NewLine);

								if (signofftype == SignoffType.CostCodeOwner || signofftype == SignoffType.AssignmentSignOffOwner)
								{
									this.UpdateCheckerClaimHistory(defaultAuthoriserLevelEmployeeId.Value, claim, commenter);
								}
								else
								{
									this.UpdateCheckerIntoDatabase(defaultAuthoriserLevelEmployeeId.Value, claim, commenter.Value);
								}

								logInformation += string.Format("Assigning items to {0} for authorisation{1}", defaultAuthoriser.FullNameUsername, Environment.NewLine);

								this.UpdateItemCheckerIdInCaseOfCostCode(expenseIds, defaultAuthoriserLevelEmployeeId.Value);
								this.SendClaimMail(stage, claim, claim.employeeid, defaultAuthoriserLevelEmployeeId.Value);
								message = true;
							}
						}
						else
						{
							var authorisersManager = Employee.Get(currentApproverLineManagerId.Value, accountId);

							var properties = new cAccountSubAccounts(accountId).getFirstSubAccount().SubAccountProperties;

							if (authorisersManager.EmployeeID == claim.employeeid && !properties.AllowEmployeeInOwnSignoffGroup)
							{
								currentApproverLineManagerId = authorisersManager.LineManager == 0
									? defaultAuthoriserLevelEmployeeId.Value
									: authorisersManager.LineManager;
								authorisersManager = Employee.Get(currentApproverLineManagerId.Value, accountId);
							}

							logInformation += string.Format("Current approver's line manager is {0} (ID: {1}), this claim is now being assigned to him/her.{2}", authorisersManager.FullNameUsername, authorisersManager.EmployeeID, Environment.NewLine);

							// Forward it to Line manager
							if (signofftype == SignoffType.CostCodeOwner || signofftype == SignoffType.AssignmentSignOffOwner)
							{
								this.UpdateCheckerClaimHistory(currentApproverLineManagerId.Value, claim, commenter);
							}
							else
							{
								this.UpdateCheckerIntoDatabase(currentApproverLineManagerId.Value, claim, commenter.Value);
							}

							this.UpdateItemCheckerIdInCaseOfCostCode(expenseIds, currentApproverLineManagerId.Value);
							this.SendClaimMail(stage, claim, claim.employeeid, currentApproverLineManagerId.Value);
							message = true;
						}
					}
					else
					{
						logInformation += string.Format("Expense item total is within remaining balance for {0}, marking items as approved and continuing{1}", authoriser.FullNameUsername, Environment.NewLine);
						this.UpdateApproverApprovedBalance(claim, expenseIds, currentUserId);
						if (IsSplitClaimWithMultipleCostCodeOwnersOrSupervisorAssignments(claim, signofftype))
						{
							this.UpdateSplitClaimHistory(currentUserId, claim, commenter);
						}
						this.approveItems(expenseIds);
					}
				}

				cClaim claimInstance = this.getClaimById(claimId);

				result = new AllowExpenseItemsResult(claimInstance.NumberOfUnapprovedItems, claimInstance.HasReturnedItems, null, message, noDefaultAuthoriserPresent);
			}

			AuthoriserLevelLogEntry(logInformation, accountId);

			return result;
		}

		/// <summary>
		/// Event log for Authoriser level approval
		/// </summary>
		/// <param name="logInformation">Information that need to be logged</param>       
		private static void AuthoriserLevelLogEntry(string logInformation, int accountId)
		{
			if (accountId == 538) // Ensures the event log isn't filled for accounts we don't care about. This logging is only to assistant investigation into issue 119843.
			{
				var logName = "Authoriser Level Log - Expenses";

				if (EventLog.Exists(logName) == false)
				{
					EventLog.CreateEventSource(logName, logName);
				}

				EventLog.WriteEntry(logName, logInformation, EventLogEntryType.Information);
			}

		}

		/// <summary>
		/// Checks a user's EmployeeId (or DelegateId) to see if they are the checker for the claim or one of its expense items.
		/// </summary>
		/// <param name="user">The user to check.</param>
		/// <param name="claim">The claim.</param>
		/// <param name="claimSelectorMode">Whether the the request is from the claim viewer.</param>
		/// <param name="expenseItems">If you have already fetched the expense items from the claim, pass them in here.</param>
		/// <returns>Whether the user is allowed access.</returns>
		public bool IsUserClaimsCurrentApprover(ICurrentUser user, cClaim claim, bool claimSelectorMode = false, List<cExpenseItem> expenseItems = null)
		{
			// obviously the user must be able to see their own claims
			if (user.EmployeeID == claim.employeeid)
			{
				return true;
			}

			// do checks for the claim selector
			if (!claimSelectorMode)
			{
				var currentStage = claim.stage == 0 ? null : this.GetSignoffStages(claim)[claim.stage - 1];

				// check that it's not split via expense items due to a team style stage
				var singleEmployeeStyle = false;
				if (currentStage != null)
				{
					switch (currentStage.signofftype)
					{
						case SignoffType.BudgetHolder:
						case SignoffType.Employee:
						case SignoffType.LineManager:
						case SignoffType.DeterminedByClaimantFromApprovalMatrix:
							singleEmployeeStyle = true;
							break;
					}
				}

				if (currentStage == null || (claim.checkerid != 0 && singleEmployeeStyle))
				{
					// check the checker
					if (claim.employeeid != user.EmployeeID && claim.checkerid != user.EmployeeID)
					{
						return false;
					}
				}
				else
				{
					switch (currentStage.signofftype)
					{
						case SignoffType.AssignmentSignOffOwner:
						case SignoffType.CostCodeOwner:
							// check the expenses in the claim to see if the current user is the checker for any of them.
							var items = expenseItems ?? this.getExpenseItemsFromDB(claim.claimid).Values;

							if (items.Any(item => item.itemCheckerId == user.EmployeeID))
							{
								break;
							}

							return false;
						case SignoffType.Team:
							var teams = new cTeams(claim.accountid, null);
							var claimTeam = teams.GetTeamById(currentStage.relid);
							if (claimTeam == null || !claimTeam.teammembers.Contains(user.EmployeeID))
							{
								return false;
							}

							break;
						default:
							return claim.checkerid == user.EmployeeID;
					}
				}
			}
			else
			{
				if (!user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ClaimViewer, true, false))
				{
					return false;
				}

				AccessRoleLevel accessLevel = user.HighestAccessLevel;
				switch (accessLevel)
				{
					case AccessRoleLevel.EmployeesResponsibleFor:
						var employee = Employee.Get(user.EmployeeID, user.AccountID);
						List<int> hierarchy = employee.Hierarchy();
						bool employeeInHierarchy = hierarchy.Find(emp => emp == user.EmployeeID) > 0;

						if (!employeeInHierarchy)
						{
							//User is not in extended hierarchy, so need to check if they have permission to view the claim based on
							//the extended claim hierarchy. This is to prevent a user simply changing the claimId in the claimviwer URL

							List<int> viewableClaims = this.GetClaimsToInclude(user, user.EmployeeID);
							bool permissionToViewClaim = viewableClaims.Find(i => i == claim.claimid) > 0;

							if (!permissionToViewClaim)
							{
								return false;
							}
						}

						break;
					case AccessRoleLevel.SelectedRoles:
						// get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
						var roles = new cAccessRoles(user.AccountID, cAccounts.getConnectionString(user.AccountID));
						var reportRoles = new List<int>();
						List<int> lstAccessRoles = user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

						foreach (
							int link in
								lstAccessRoles.Select(roles.GetAccessRoleByID)
									.SelectMany(
										accessRole =>
											accessRole.AccessRoleLinks.Where(link => !reportRoles.Contains(link))))
						{
							reportRoles.Add(link);
						}

						List<int> employeeHierarchy = Employee.GetEmployeesWithAccessRoles(user.AccountID, reportRoles);
						bool employeeInRolesHeirarchy = employeeHierarchy.Find(emp => emp == user.EmployeeID) != -1;

						if (!employeeInRolesHeirarchy)
						{
							return false;
						}

						break;
					case AccessRoleLevel.AllData: break;
					default: throw new ArgumentOutOfRangeException();
				}
			}

			return true;
		}

		/// <summary>
		/// Returns a value indicating whether the claim is split for approval from multiple cost code owners or supervisor assignments
		/// </summary>
		/// <param name="claim">The claim</param>
		/// <param name="signoffType">Current stage sign off type</param>
		/// <returns></returns>
		public bool IsSplitClaimWithMultipleCostCodeOwnersOrSupervisorAssignments(cClaim claim, SignoffType signoffType)
		{
			return claim.splitApprovalStage && this.GetApproverIds(claim, false).Length > 1
				   && (signoffType == SignoffType.CostCodeOwner || signoffType == SignoffType.AssignmentSignOffOwner);
		}

		/// <summary>
		/// Returns a value indicating whether receipts can be modified.
		/// Supplying no expense item means that we are doing the claim header.
		/// Supplying an expense item means the expense item is checked.
		/// Does not check if the user is the current approver. Use <see cref="IsUserClaimsCurrentApprover"/>.
		/// </summary>
		/// <param name="why">The out param to populate the reason why receipt can or cannot be deleted.</param>
		/// <param name="account">The current account.</param>
		/// <param name="signoffStages">The signoff stages for this employee.</param>
		/// <param name="user">The current user.</param>
		/// <param name="claim">The claim.</param>
		/// <param name="expenseItem">The expense item, if there is one.</param>
		/// <param name="allEnvelopesComplete">Whether all envelopes are complete. If there are no envelopes then pass true.</param>
		/// <param name="allExpensesHaveReceiptsAttached">Whether all the expenses for this claim have receipts attached.</param>
		/// <param name="fromClaimSelector">Whether this method call has arisen as a result of browsing someone else's claim from the claim selector.</param>
		/// <returns>Null if you can delete receipts, or an exception detailing why you can't.</returns>
		public bool CanUserDeleteReceiptsForCurrentClaim(out string why, cAccount account, List<SignoffType> signoffStages, ICurrentUserBase user, cClaim claim, cExpenseItem expenseItem, bool allEnvelopesComplete, bool allExpensesHaveReceiptsAttached, bool fromClaimSelector)
		{
			if (fromClaimSelector)
			{
				why = "You cannot modify receipts from the claim viewer.";
				return false;
			}

			// check user
			if (user == null)
			{
				why = "User doesn't exist.";
				return false;
			}

			// check we have a claim.
			if (claim == null)
			{
				why = "Claim doesn't exist.";
				return false;
			}

			// can always edit when the claim is being created.
			if (claim.ClaimStage == ClaimStage.Current)
			{
				why = "Drag me to move or copy me";
				return true;
			}

			// can always edit when the claim is being created.
			if (claim.ClaimStage == ClaimStage.Previous)
			{
				why = "You cannot modify receipts for completed claims.";
				return false;
			}

			// this leaves us managing the Submitted ClaimStage.
			// if the user owns the claim
			if (claim.employeeid == user.EmployeeID)
			{
				// check that the claim isn't in scan/attach and validate
				if (account.ReceiptServiceEnabled)
				{
					var isInScanAttach = HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELScanAttach, signoffStages, true);

					if (isInScanAttach && allEnvelopesComplete && !allExpensesHaveReceiptsAttached && signoffStages.Contains(SignoffType.SELValidation))
					{
						if (expenseItem == null)
						{
							why = "Should this receipt be attached to an expense below?";
							return true;
						}

						why = "All expenses need at least one receipt.";
						return true;
					}
				}

				//  check the claim state permits editing.
				if ((claim.submitted && !claim.HasReturnedItems) || claim.approved || claim.paid)
				{
					why = "You cannot modify receipts as the claim is being or has been processed.";
					return false;
				}

				// check the expense state permits editing.
				if (expenseItem != null)
				{
					if (claim.stage > 0 && (!expenseItem.returned || expenseItem.corrected))
					{
						why = "You cannot modify receipts on this item as it has not been returned, or has already been corrected.";
						return false;
					}
				}
			}

			why = "Drag me to move or copy me.";
			return true;
		}

		/// <summary>
		/// Deletes the supplied claim.
		/// </summary>
		/// <param name="claim">The claim to delete.</param>
		public void DeleteClaim(cClaim claim)
		{
			var account = new cAccounts().GetAccountByID(accountid);

			// first unlink any envelopes
			if (account.ReceiptServiceEnabled && !string.IsNullOrEmpty(claim.ReferenceNumber))
			{
				var envelopeData = new Envelopes();
				var envelopes = envelopeData.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);
				ICurrentUser user = cMisc.GetCurrentUser();

				foreach (var envelope in envelopes)
				{
					envelopeData.DetachFromClaim(envelope.EnvelopeId, claim, user);
				}
			}

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.AddWithValue("@claimId", claim.claimid);
				connection.ExecuteProc("DeleteClaim");
				connection.ClearParameters();
			}

			var audit = new cAuditLog(accountid, claim.employeeid);
			audit.deleteRecord(SpendManagementElement.Claims, claim.claimid, claim.name);

			var defaultClaim = this.getDefaultClaim(ClaimStage.Current, claim.employeeid);
			if (defaultClaim == claim.claimid)
			{
				Cache.Delete(this.accountid, CacheArea, defaultClaim.ToString());
			}
		}

		/// <summary>
		/// Adds an entry to the claim history.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="comment"></param>
		/// <param name="employeeid">The id of the employee.</param>
		/// <param name="refnum">The refnum of the expense item.</param>
		public virtual void UpdateClaimHistory(cClaim claim, string comment, int? employeeid = null, string refnum = null)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.AddWithValue("@claimId", claim.claimid);
				connection.AddWithValue("@stage", claim.stage);
				connection.AddWithValue("@comment", comment);
				connection.AddWithValue("@datestamp", DateTime.Now);
				connection.AddWithValue("@createdOn", DateTime.Now.ToUniversalTime());

				if (employeeid.HasValue)
				{
					connection.AddWithValue("@employeeId", employeeid.Value);
				}
				else
				{
					connection.AddWithValue("@employeeId", DBNull.Value);
				}

				if (refnum != null)
				{
					connection.AddWithValue("@refnum", refnum);
				}
				else
				{
					connection.AddWithValue("@refnum", DBNull.Value);
				}

				connection.ExecuteProc("UpdateClaimHistory");
				connection.ClearParameters();
			}
		}

		public void unallocateClaim(int claimid, int userid)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			cClaim reqclaim = getClaimById(claimid);

			if (!reqclaim.splitApprovalStage)
			{
				cGroups clsgroups = new cGroups(accountid);
				cGroup reqgroup = this.GetGroupForClaim(clsgroups, reqclaim);

				SortedList stages = clsgroups.sortStages(reqgroup);
				cStage reqstage = (cStage)stages.GetByIndex(reqclaim.stage - 1);
				int teamId = 0;

				switch (reqstage.signofftype)
				{
					case SignoffType.Team:
						teamId = reqstage.relid;
						break;
					case SignoffType.ApprovalMatrix:
						ApprovalMatrix matrix = new ApprovalMatrices(this.accountid).GetById(reqstage.relid);
						if (matrix != null)
						{
							IEnumerable<ApprovalMatrixLevel> levels = ApprovalMatrices.GetListOfLevelsForThisAmount(matrix, 0, reqclaim.Total).ToList();

							foreach (ApprovalMatrixLevel level in levels.Where(level => level.ApproverTeamId.HasValue))
							{
								teamId = level.ApproverTeamId.Value;
							}
						}
						break;
				}

				if (teamId == 0)
				{
					return;
				}

				const string strsql = "update claims_base set checkerid = null, teamid = @teamid, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
				expdata.sqlexecute.Parameters.AddWithValue("@teamid", teamId);
				expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
				expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", DateTime.UtcNow);
				expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();

				reqclaim.checkerid = 0;
				reqclaim.teamid = reqstage.relid;
			}
			else
			{
				expdata.sqlexecute.Parameters.AddWithValue("@claimId", claimid);
				expdata.sqlexecute.Parameters.AddWithValue("@checkerId", userid);
				expdata.ExecuteProc("UnallocateCostCodeOwnerTeamChecker");
			}
		}

		/// <summary>
		/// Get correct signoff group for a given claim 
		/// </summary>
		/// <param name="groups"></param>
		/// <param name="claim"></param>
		/// <returns></returns>
		public cGroup GetGroupForClaim(cGroups groups, cClaim claim)
		{
			cGroup group = null;
			var clsemployees = new cEmployees(accountid);
			var reqemp = clsemployees.GetEmployeeById(claim.employeeid);
			var subAccounts = new cAccountSubAccounts(accountid);
			cAccountProperties accountProperties = subAccounts.getFirstSubAccount().SubAccountProperties;


			// there is a method GetGroupForClaim in ClaimSubmission.cs so any changes here might need to be reflected there
			if (accountProperties.PartSubmit && accountProperties.OnlyCashCredit)
			{
				switch (claim.claimtype)
				{
					case ClaimType.Credit:
						group = groups.GetGroupById(reqemp.CreditCardSignOffGroup);
						break;
					case ClaimType.Purchase:
						group = groups.GetGroupById(reqemp.PurchaseCardSignOffGroup);
						break;
					default:
						group = groups.GetGroupById(reqemp.SignOffGroupID);
						break;
				}
			}
			else
			{
				group = groups.GetGroupById(reqemp.SignOffGroupID);
			}

			return group;
		}

		public string[] generateUnallocatedClaimsGrid(int employeeId, string surname, byte filter, int? delegateID)
		{
			cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
			IOwnership dcco = clsSubAccounts.GetDefaultCostCodeOwner(accountid, clsSubAccounts.getFirstSubAccount().SubAccountID);
			cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
			cFields fields = new cFields(accountid);
			cGridNew grid = new cGridNew(accountid, employeeId, "gridUnallocated", "select claimid, claimno, stage, empname, name, datesubmitted, status, numreceipts, numreceiptsattached, flaggeditems, total, cash, credit, purchase, basecurrency, currencySymbol from unallocatedClaims");
			grid.EmptyText = "There are currently no claims to display.";
			grid.KeyField = "claimid";
			grid.getColumnByName("claimid").hidden = true;
			grid.getColumnByName("currencySymbol").hidden = true;
			grid.getColumnByName("basecurrency").hidden = true;
			grid.EnableSelect = true;
			cFieldColumn statusColumn = (cFieldColumn)grid.getColumnByName("status");
			statusColumn.addValueListItem((byte)0, string.Empty);
			statusColumn.addValueListItem((byte)1, "Claim submitted, awaiting action");
			statusColumn.addValueListItem((byte)2, "Claim being processed");
			statusColumn.addValueListItem((byte)3, "Sent to next stage, awaiting action");
			statusColumn.addValueListItem((byte)4, "An item(s) has been returned to employee for amendment");
			statusColumn.addValueListItem((byte)5, "Returned item(s) has been corrected, awaiting action");
			statusColumn.addValueListItem((byte)6, "Claim has been approved, awaiting payment");
			statusColumn.addValueListItem((byte)7, "Claim Paid");
			statusColumn.addValueListItem((byte)8, "Awaiting allocation to approver");
			statusColumn.addValueListItem((byte)9, "Claim awaiting pre-approval");

			cTeams teams = new cTeams(accountid, reqProperties.SubAccountID);

			// check if default cost code owner is a team
			bool isDefaultOwner = IsDefaultCostCodeOwner(employeeId, dcco, teams);

			if (isDefaultOwner)
			{
				grid.WhereClause += "(";
			}

			grid.WhereClause += "(" +
				"(teamemployeeid = @employeeid and costcodeteamemployeeid is null and itemCheckerTeamEmployeeId is null)" +
				" or (teamemployeeid is null and costcodeteamemployeeid = @costcodeTeamEmployeeid and itemcheckerid is null)" +
				" or (teamemployeeid is null and itemCheckerTeamEmployeeId = @itemCheckerTeamEmployeeid and itemcheckerid is null)" +
				")";

			if (isDefaultOwner)
			{
				grid.WhereClause += " or (teamemployeeid is null and itemcheckerid is null and teamemployeeid is null and costcodeteamemployeeid is null and itemCheckerTeamEmployeeId is null))";
			}

			grid.addFilter(fields.GetFieldByID(Guid.Parse("db9fe713-2154-4d8a-9695-b82a7a722512")), "@employeeid", employeeId);
			grid.addFilter(fields.GetFieldByID(Guid.Parse("a39a833c-b655-4003-844f-aeb87ba03ada")), "@costcodeTeamEmployeeid", employeeId);
			grid.addFilter(fields.GetFieldByID(Guid.Parse("aec7533e-0df8-4f91-8cae-ad3ba1a59ece")), "@itemCheckerTeamEmployeeid", employeeId);

			if (surname != string.Empty)
			{
				grid.WhereClause += " and employees.surname like @surname";
				grid.addFilter(fields.GetFieldByID(Guid.Parse("9D70D151-5905-4A67-944F-1AD6D22CD931")), "@surname", surname + "%");

			}

			if (filter != 0)
			{
				grid.WhereClause += " and claimtype = @claimtype";
				grid.addFilter(fields.GetFieldByID(Guid.Parse("0f6a82cd-7c8b-4b14-90f1-96d01e7d3e49")), "@claimtype", filter);
			}

			if (!reqProperties.AllowTeamMemberToApproveOwnClaim)
			{

				grid.WhereClause += " and unallocatedClaims.employeeid != @employeeid";

				if (delegateID.HasValue)
				{
					grid.addFilter(fields.GetFieldByID(Guid.Parse("db9fe713-2154-4d8a-9695-b82a7a722512")), "@delegateID", delegateID.HasValue);
					grid.WhereClause += " and unallocatedClaims.employeeid != @delegateID";
				}
			}

			SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object> { };
			grid.InitialiseRowGridInfo = gridInfo;
			grid.InitialiseRow += this.unallocatedClaimsGrid_InitialiseRow;
			grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cClaims";
			grid.ServiceClassMethodForInitialiseRowEvent = "unallocatedClaimsGrid_InitialiseRow";

			return grid.generateGrid();
		}

		private void unallocatedClaimsGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
		{
			if (row.getCellByID("currencySymbol").Value != DBNull.Value)
			{
				string symbol = row.getCellByID("currencySymbol").Value.ToString();
				row.getCellByID("total").Format.Symbol = symbol;
				row.getCellByID("cash").Format.Symbol = symbol;
				row.getCellByID("credit").Format.Symbol = symbol;
				row.getCellByID("purchase").Format.Symbol = symbol;

			}

			object claimId = row.getCellByID("claimid").Value;
			cGridNew.CheckClaimRowForBankAccountRedaction(row, claimId);
		}

		public string[] generateClaimsToCheckGrid(int employeeId, string surname, byte filter, int? delegateID)
		{
			cAccountSubAccounts clsSubAccounts = new cAccountSubAccounts(accountid);
			cAccountProperties reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;
			var grid = new cGridNew(accountid, employeeId, "gridClaims", "select claimid, claimno, stage, empname, name, datesubmitted, status, numreceipts, numreceiptsattached, flaggeditems, total, cash, credit, purchase, paiditems, approved, stagecount, basecurrency, displayunallocate, displaysinglesignoff, splitApprovalStage, ClaimItemsCount, ItemsApproved, CheckerItemsUnapproved, CheckerItemsApproved, currencySymbol, SignoffGroup from checkandpay");
			var fields = new cFields(accountid);
			grid.addEventColumn("unapprove", "../shared/images/icons/16/plain/undo.png", "javascript:SEL.Claims.CheckAndPay.UnallocateClaim({claimid});", "Un-allocate Claim", "Un-allocate Claim");
			grid.addEventColumn("approveclaim", "Approve Claim", string.Empty, string.Empty);
			grid.addEventColumn("action", "&nbsp;", "&nbsp;", "&nbsp;");
			grid.KeyField = "claimid";
			grid.getColumnByName("claimid").hidden = true;
			grid.getColumnByName("approved").hidden = true;
			grid.getColumnByName("stagecount").hidden = true;
			grid.getColumnByName("displayunallocate").hidden = true;
			grid.getColumnByName("displaysinglesignoff").hidden = true;
			grid.getColumnByName("CheckerItemsUnapproved").hidden = true;
			grid.getColumnByName("CheckerItemsApproved").hidden = true;
			grid.getColumnByName("currencySymbol").hidden = true;
			grid.getColumnByName("basecurrency").hidden = true;
			grid.getColumnByName("SignoffGroup").hidden = true;

			grid.EmptyText = "There are currently no claims to display.";
			cFieldColumn statusColumn = (cFieldColumn)grid.getColumnByName("status");
			statusColumn.addValueListItem((byte)0, string.Empty);
			statusColumn.addValueListItem((byte)1, "Claim submitted, awaiting action");
			statusColumn.addValueListItem((byte)2, "Claim being processed");
			statusColumn.addValueListItem((byte)3, "Sent to next stage, awaiting action");
			statusColumn.addValueListItem((byte)4, "An item(s) has been returned to employee for amendment");
			statusColumn.addValueListItem((byte)5, "Returned item(s) has been corrected, awaiting action");
			statusColumn.addValueListItem((byte)6, "Claim has been approved, awaiting payment");
			statusColumn.addValueListItem((byte)7, "Claim Paid");
			statusColumn.addValueListItem((byte)8, "Awaiting allocation to approver");
			statusColumn.addValueListItem((byte)9, "Claim awaiting pre-approval");

			grid.WhereClause = "(checkerid = @checkerid or itemcheckerid = @itemcheckerid)";
			grid.addFilter(fields.GetFieldByID(Guid.Parse("8818dd4b-51ee-46df-8be9-11e49ca96ddc")), "@checkerid", employeeId);
			grid.addFilter(fields.GetFieldByID(Guid.Parse("92a18809-0edc-494a-ba90-889f6f24d46a")), "@itemcheckerid", employeeId);


			if (surname != string.Empty)
			{
				grid.WhereClause += " and employees.surname like @surname";
				grid.addFilter(fields.GetFieldByID(Guid.Parse("9D70D151-5905-4A67-944F-1AD6D22CD931")), "@surname", surname + "%");

			}
			if (filter != 0)
			{
				grid.WhereClause += " and claimtype = @claimtype";
				grid.addFilter(fields.GetFieldByID(Guid.Parse("fb4a9137-36d3-4546-8a8f-1ddd90c3d913")), "@claimtype", filter);
			}
			if (!reqProperties.AllowEmployeeInOwnSignoffGroup)
			{
				grid.WhereClause += " and checkandpay.employeeid != @employeeid";

				if (delegateID.HasValue)
				{
					grid.addFilter(fields.GetFieldByID(Guid.Parse("587F4837-4147-46D5-8BDB-0A62FB7B6631")), "@delegateID", delegateID.Value);
					grid.WhereClause += " and checkandpay.employeeid != @delegateID";
				}

				grid.addFilter(fields.GetFieldByID(Guid.Parse("587F4837-4147-46D5-8BDB-0A62FB7B6631")), "@employeeid", employeeId);
			}
			SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object> { { "accountId", accountid } };
			grid.InitialiseRowGridInfo = gridInfo;
			grid.InitialiseRow += this.checkAndPayClaimGrid_InitialiseRow;
			grid.ServiceClassForInitialiseRowEvent = "Spend_Management.cClaims";
			grid.ServiceClassMethodForInitialiseRowEvent = "checkAndPayClaimGrid_InitialiseRow";
			return grid.generateGrid();
		}

		private void checkAndPayClaimGrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
		{
			object claimId = row.getCellByID("claimid").Value;
			bool isOneClickSignoffStage = (bool)row.getCellByID("displaysinglesignoff").Value;
			bool isSplitApprovalStage = (bool)row.getCellByID("splitApprovalStage").Value;
			bool isApproved = (bool)row.getCellByID("approved").Value;
			int numberOfItems = (int)row.getCellByID("ClaimItemsCount").Value;
			int numberOfApprovedItems = (int)row.getCellByID("ItemsApproved").Value;
			int numberOfCheckerUnapprovedItems = (int)row.getCellByID("CheckerItemsUnapproved").Value;

			if (!(bool)row.getCellByID("displayunallocate").Value)
			{
				row.getCellByID("unapprove").Value = string.Empty;
			}

			CurrentUser user = cMisc.GetCurrentUser();
			int accountId = this.accountid > 0 ? this.accountid : (int)gridInfo.Values.FirstOrDefault();
			decimal? authoriserLevelAmount = this.GetAuthoriserLevelAmountByEmployee(user.EmployeeID, accountId);

			if (authoriserLevelAmount.HasValue)
			{
				if (ShowUnallocateIcon((int)claimId, accountId))
				{
					row.getCellByID("unapprove").Value = string.Empty;
				}
			}

			if (isApproved)
			{
				row.getCellByID("approveclaim").Value = "<a href=\"#\" onclick=\"SEL.Claims.CheckAndPay.PayClaim(" + claimId + ");\">Allocate For Payment</a>";
				row.getCellByID("action").Value = string.Empty;
			}
			else
			{
				if (isOneClickSignoffStage)
				{
					row.getCellByID("approveclaim").Value = "<a href=\"javascript:SEL.Claims.CheckAndPay.OneClickApproveClaim(" + claimId + ");\">Approve Claim</a>";
				}
				else if (isSplitApprovalStage && numberOfCheckerUnapprovedItems == 0 && numberOfApprovedItems != numberOfItems)
				{
					row.getCellByID("approveclaim").Value = "<i>Approval by others<br />outstanding</i>";
				}
				else
				{
					row.getCellByID("approveclaim").Value = string.Empty;
				}

				row.getCellByID("action").Value = string.Format("<a href=\"checkexpenselist.aspx?claimid={0}&stage={1}\">Check Expenses</a>", claimId, row.getCellByID("stage").Value);
			}

			row.getCellByID("stage").Value = row.getCellByID("stage").Value + " of " + row.getCellByID("stagecount").Value;

			if (row.getCellByID("currencySymbol").Value != DBNull.Value)
			{
				string symbol = row.getCellByID("currencySymbol").Value.ToString();
				row.getCellByID("total").Format.Symbol = symbol;
				row.getCellByID("cash").Format.Symbol = symbol;
				row.getCellByID("credit").Format.Symbol = symbol;
				row.getCellByID("purchase").Format.Symbol = symbol;
				row.getCellByID("paiditems").Format.Symbol = symbol;
			}

			cGridNew.CheckClaimRowForBankAccountRedaction(row, claimId);
		}

		/// <summary>
		/// Get a count of the claims to check for the current user.
		/// </summary>
		/// <param name="employeeid">
		/// The employeeid.
		/// </param>
		/// <param name="excludeTeamClaims">
		/// Whether to exclude team claims from the count.
		/// </param>
		/// <param name="delegateID">
		/// The delegate ID.
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		public int getClaimsToCheckCount(int employeeid, bool excludeTeamClaims, int? delegateID)
		{
			var expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			var subaccs = new cAccountSubAccounts(accountid);
			IOwnership dcco = subaccs.GetDefaultCostCodeOwner(accountid, subaccs.getFirstSubAccount().SubAccountID);

			bool isDefaultApprover = false;


			if (dcco != null && dcco.ItemPrimaryID() > 0)
			{
				switch (dcco.ElementType())
				{
					case SpendManagementElement.Employees:
						if (dcco.ItemPrimaryID() == employeeid)
						{
							isDefaultApprover = true;
						}
						break;
					case SpendManagementElement.BudgetHolders:
						cBudgetholders holders = new cBudgetholders(accountid);
						cBudgetHolder holder = holders.getBudgetHolderById(dcco.ItemPrimaryID());
						if (holder != null && holder.employeeid == employeeid)
						{
							isDefaultApprover = true;
						}
						break;
					case SpendManagementElement.Teams:
						cTeams teams = new cTeams(accountid);
						cTeam team = teams.GetTeamById(dcco.ItemPrimaryID());
						if (team != null && team.teammembers.Contains(employeeid))
						{
							isDefaultApprover = true;
						}
						break;
				}

			}

			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@isDefaultApprover", Convert.ToInt32(isDefaultApprover));
			expdata.sqlexecute.Parameters.AddWithValue("@excludeTeamClaims", Convert.ToInt32(excludeTeamClaims));
			expdata.sqlexecute.Parameters.AddWithValue("@delegateID", delegateID.HasValue ? delegateID.Value : 0);

			expdata.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
			expdata.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
			expdata.ExecuteProc("GetApproverClaimCount");
			int count = (int)expdata.sqlexecute.Parameters["@returnvalue"].Value;
			expdata.sqlexecute.Parameters.Clear();
			return count;
		}

		/// <summary>
		/// Allocates a claim to a team member
		/// </summary>
		/// <param name="claimid">The id of the claim</param>
		/// <param name="employeeid">The id of the employee to allocate the claim to</param>
		/// <returns>If the claim was successfully allocated or not</returns>
		public AllocateClaimResult AllocateClaim(int claimid, int employeeid)
		{
			cClaim reqclaim = getClaimById(claimid);
			var result = new AllocateClaimResult() { ClaimId = reqclaim.claimid, ClaimName = reqclaim.name };

			var count = 0;

			using (var databaseConnection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				databaseConnection.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
				databaseConnection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				count = databaseConnection.ExecuteScalar<int>("dbo.GetAllocatedClaimsCount", CommandType.StoredProcedure);

				databaseConnection.sqlexecute.Parameters.Clear();
			}

			if (count <= 0)
			{
				result.Success = false;
				return result;
			}

			var expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			if (!reqclaim.splitApprovalStage)
			{
				cEmployees clsemployees = new cEmployees(accountid);
				Employee reqemp = clsemployees.GetEmployeeById(employeeid);
				DateTime modifiedon = DateTime.Now.ToUniversalTime();

				expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
				expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
				expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
				expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
				string strsql =
					"update claims_base set checkerid = @employeeid, teamid = null, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();

				//claim history
				strsql =
					"insert into claimhistory (claimid, stage, comment, datestamp) values (@claimid,@stage,@comment,@datestamp)";
				expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
				expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
				expdata.sqlexecute.Parameters.AddWithValue("@comment",
					string.Format("The claim has been allocated and awaiting approval by {0} {1} {2}", reqemp.Title,
						reqemp.Forename, reqemp.Surname));
				expdata.sqlexecute.Parameters.AddWithValue("@datestamp", DateTime.Now);
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
				reqclaim.checkerid = employeeid;
			}
			else
			{
				cAccountSubAccounts subaccs = new cAccountSubAccounts(accountid);

				// costcode owner stage where ownership must be a team
				expdata.sqlexecute.Parameters.AddWithValue("@claimId", claimid);
				expdata.sqlexecute.Parameters.AddWithValue("@checkerId", employeeid);
				expdata.sqlexecute.Parameters.AddWithValue("@subAccountId",
					subaccs.getFirstSubAccount().SubAccountID);
				expdata.ExecuteProc("AllocateCostCodeTeamItemCheckers");
			}

			result.Success = true;
			return result;
		}

		public void calculateReimbursableFuelCardMileage(int employeeid, int claimId, int claimEmployeeId)
		{
			cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, employeeid);

			var cars = this.GetEmployeesActiveCarsWithAFuelCard(clsEmployeeCars);

			cSubcats clsSubcats = new cSubcats(accountid);
			cExpenseItems clsExpItems = new cExpenseItems(accountid);

			if (claimId > 0)
			{
				cSubcat reimburseSubcat = null;
				decimal reimbursePersonalTotal = 0;
				decimal businessMiles = 0;
				int currencyID = 0;
				int countryID = 0;
				int reasonID = 0;
				long esrAssignmentID = 0;
				int basecurrency = 0;
				int globalbasecurrency = 0;
				double globalExchangeRate = 0;
				decimal fuelcardTotal = 0;
				int? bankAccountId = null;

				List<cExpenseItem> lstNonItems = new List<cExpenseItem>();
				List<cExpenseItem> lstReimburseItems = new List<cExpenseItem>();

				cSubcat subcat = null;
				cClaims claims = new cClaims(accountid);
				SortedList<int, cExpenseItem> expenseItems = claims.getExpenseItemsFromDB(claimId);
				foreach (cExpenseItem item in expenseItems.Values)
				{
					subcat = clsSubcats.GetSubcatById(item.subcatid);
					item.journeysteps = clsExpItems.GetJourneySteps(item.expenseid);

					if (subcat.calculation == CalculationType.FuelCardMileage)
					{
						lstReimburseItems.Add(item);
						fuelcardTotal += item.total;

						if (item.bankAccountId.HasValue && !bankAccountId.HasValue)
						{
							bankAccountId = item.bankAccountId;
						}
					}

					if (subcat.calculation == CalculationType.PencePerMile && !subcat.reimbursable)
					{
						item.costcodebreakdown = clsExpItems.getCostCodeBreakdown(item.expenseid);
						lstNonItems.Add(item);

						businessMiles += item.miles;
					}
				}

				int odoOldReadingTotal = 0;
				int odoNewReadingTotal = 0;

				foreach (cCar car in cars)
				{
					cOdometerReading reading = car.getLastOdometerReading();

					if (reading == null) continue;

					cEventlog.LogEntry(string.Format("Claim {0} submitted using odometer reading ID {1} (old reading: {2}, new reading: {3}, employee: {4})", claimId, reading.odometerid, reading.oldreading, reading.newreading, employeeid));

					odoNewReadingTotal += reading.newreading;
					odoOldReadingTotal += reading.oldreading;
				}

				decimal odoDifference = odoNewReadingTotal - odoOldReadingTotal;
				decimal noOfPersonalMiles = odoDifference - businessMiles;

				decimal pencePerMile = clsSubcats.CalculateFuelCardReimbursablePencePerMiles(odoOldReadingTotal, odoNewReadingTotal, businessMiles, claimEmployeeId, fuelcardTotal);
				reimbursePersonalTotal = Math.Round((pencePerMile * noOfPersonalMiles) / -1, 2, MidpointRounding.AwayFromZero);

				var costCodeBreakdown = new List<cDepCostItem>();
				foreach (cExpenseItem reimburseItem in lstReimburseItems)
				{
					currencyID = reimburseItem.currencyid;
					countryID = reimburseItem.countryid;
					reasonID = reimburseItem.reasonid;
					esrAssignmentID = reimburseItem.ESRAssignmentId;
					basecurrency = reimburseItem.basecurrency;
					globalbasecurrency = reimburseItem.globalbasecurrency;
					globalExchangeRate = reimburseItem.globalexchangerate;
					costCodeBreakdown = clsExpItems.getCostCodeBreakdown(reimburseItem.expenseid);
					subcat = clsSubcats.GetSubcatById(reimburseItem.subcatid);
					if (subcat != null && subcat.reimbursableSubcatID.HasValue)
					{
						reimburseSubcat = clsSubcats.GetSubcatById(subcat.reimbursableSubcatID.Value);
						break;
					}
				}

				foreach (cExpenseItem itm in lstNonItems)
				{
					var clsitems = new cExpenseItems(accountid);
					if (itm.costcodebreakdown == null)
					{
						itm.costcodebreakdown = clsitems.getCostCodeBreakdown(itm.expenseid);
					}

					decimal reimburseBusinessTotal = Math.Round(pencePerMile * itm.miles, 2, MidpointRounding.AwayFromZero);
					itm.updateVAT(reimburseBusinessTotal, 0, reimburseBusinessTotal);
					itm.mileageid = 0;
					clsitems.updateItem(itm, employeeid, 0, false);
				}

				if (reimburseSubcat != null)
				{
					var newItem = new cExpenseItem(
						0,
						ItemType.Cash,
						0,
						0,
						string.Empty,
						false,
						reimbursePersonalTotal,
						0,
						reimbursePersonalTotal,
						reimburseSubcat.subcatid,
						DateTime.Now,
						0,
						0,
						0,
						false,
						false,
						string.Empty,
						claimId,
						0,
						0,
						currencyID,
						string.Empty,
						0,
						countryID,
						0,
						0,
						0,
						false,
						reasonID,
						false,
						new DateTime(1900, 01, 01),
						new DateTime(1900, 01, 01),
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						true,
						0,
						string.Empty,
						0,
						0,
						string.Empty,
						basecurrency,
						globalbasecurrency,
						globalExchangeRate,
						0,
						0,
						false,
						false,
						0,
						DateTime.UtcNow,
						employeeid,
						new DateTime(1900, 01, 01),
						0,
						0,
						MileageUOM.Mile,
						esrAssignmentID,
						reimburseSubcat.HomeToLocationType
						);
					newItem.bankAccountId = bankAccountId;
					newItem.costcodebreakdown = costCodeBreakdown;
					clsExpItems.addItem(newItem, new cEmployees(this.accountid).GetEmployeeById(employeeid));
					if (newItem.flags != null)
					{
						FlagManagement flagManagement = new FlagManagement(this.accountid);
						//flag any items
						if (newItem.splititems.Count > 0)
						{
							foreach (cExpenseItem splitItem in newItem.splititems)
							{
								if (splitItem.flags != null)
								{
									flagManagement.FlagItem(splitItem.expenseid, splitItem.flags);
								}
							}
						}

						if (newItem.flags != null)
						{
							flagManagement.FlagItem(newItem.expenseid, newItem.flags);
						}
					}
					////update the business mileage
				}
			}
		}

        public decimal calculatePencePerMile(int employeeid, cClaim claim)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cEmployees clsemployees = new cEmployees(accountid);
            Employee reqemp = clsemployees.GetEmployeeById(employeeid);
            cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, employeeid);
            cMisc clsmisc = new cMisc(accountid);

            var generalOptions = this._generalOptionsFactory.Value[reqemp.DefaultSubAccount];

            decimal fuelpurchased = 0;
            decimal itemtotal;
            decimal businessmiles = 0;
            decimal totalmiles = 0;
            decimal pencepermile;
            //calculate fuel purchased
            string strsql = "select sum(total) from savedexpenses inner join subcats on subcats.subcatid = savedexpenses.subcatid where claimid = @claimid and subcats.calculation = 5";
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            using (SqlDataReader reader = expdata.GetReader(strsql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        fuelpurchased = reader.GetDecimal(0);
                    }
                }
                reader.Close();
            }

			//calculate business miles
			strsql =
				"select sum(num_miles) from savedexpenses_journey_steps inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid inner join subcats on subcats.subcatid = savedexpenses.subcatid where claimid = @claimid and subcats.calculation = 6";
			using (SqlDataReader reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();
				while (reader.Read())
				{
					if (reader.IsDBNull(0) == false)
					{
						businessmiles = reader.GetDecimal(0);
					}
				}
				reader.Close();
			}
			if (fuelpurchased == 0 && businessmiles != 0) //need to enter fuel receitps
			{
				return -1;
			}

			var cars = clsEmployeeCars.GetActiveCars().Where(e => e.fuelcard == true).ToList();
			for (int i = 0; i < cars.Count; i++)
			{
				cOdometerReading reading = cars[i].getLastOdometerReading();
				totalmiles += (reading.newreading - reading.oldreading);
			}

			if (totalmiles == 0)
			{
				return 0;
			}
			//calculate pence per mile
			pencepermile = fuelpurchased / totalmiles;

			//update the relevant transactions and calculate the mileage

			var claims = new cClaims(accountid);
			SortedList<int, cExpenseItem> sortedExpenseItems = claims.getExpenseItemsFromDB(claim.claimid);
			var expenseItems = new cExpenseItems(accountid);
			var clssubcats = new cSubcats(accountid);

            foreach (cExpenseItem item in sortedExpenseItems.Values)
            {
                SubcatBasic subcat = clssubcats.GetSubcatBasic(item.subcatid);
                if (subcat.CalculationType != CalculationType.PencePerMileReceipt) continue;
                item.journeysteps = expenseItems.GetJourneySteps(item.expenseid);
                //update the total;
                itemtotal = Math.Round(pencepermile * item.miles, 2, MidpointRounding.AwayFromZero);
                item.updateVAT(itemtotal, 0, itemtotal);
                cExpenseItem vatitem = item;
                cVat clsvat = new cVat(accountid, ref vatitem, reqemp, clsmisc, null);
                clsvat.calculateVAT();
                strsql = "update savedexpenses set net = @net, vat = @vat, total = @total, amountpayable = @total, pencepermile = @pencepermile where expenseid = @expenseid";
                expdata.sqlexecute.Parameters.AddWithValue("@net", item.net);
                expdata.sqlexecute.Parameters.AddWithValue("@vat", item.vat);
                expdata.sqlexecute.Parameters.AddWithValue("@total", item.total);
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
                expdata.sqlexecute.Parameters.AddWithValue("@pencepermile", pencepermile);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();
            }

			return 0;
		}

		#region Data Sync Methods

		public sOnlineClaimInfo getModifiedClaims(DateTime date, int employeeid, bool prevItemsSynched)
		{
			sOnlineClaimInfo onlineClaimInfo = new sOnlineClaimInfo();
			Dictionary<int, cClaim> lst = new Dictionary<int, cClaim>();
			List<int> lstclaimids = getClaimIds(employeeid, prevItemsSynched);

			foreach (int id in lstclaimids)
			{
				cClaim claim = getClaimFromDB(id);

				if (claim.createdon > date || claim.modifiedon > date)
				{
					lst.Add(claim.claimid, claim);
				}
			}

			onlineClaimInfo.lstonlineclaims = lst;
			onlineClaimInfo.lstclaimids = lstclaimids;

			return onlineClaimInfo;
		}

		public List<int> getClaimIds(int employeeid, bool prevItemsSynched)
		{
			List<int> lstclaimids = new List<int>();
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			string strsql;

			if (prevItemsSynched)
			{
				strsql = "SELECT claimid FROM claims_base where employeeid = @employeeid AND paid = 0";
			}
			else
			{
				strsql = "SELECT claimid FROM claims_base where employeeid = @employeeid";
			}

			using (SqlDataReader reader = expdata.GetReader(strsql))
			{
				expdata.sqlexecute.Parameters.Clear();

				while (reader.Read())
				{
					int claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
					lstclaimids.Add(claimid);
				}
				reader.Close();
			}
			return lstclaimids;
		}



		#endregion

		/// <summary>
		/// Check if this claim can progress through the approval process
		/// </summary>
		/// <param name="claimID">The claimID to check for</param>
		/// <returns></returns>
		public int AllowClaimProgression(int claimID)
		{
			DBConnection db = new DBConnection(cAccounts.getConnectionString(this.accountid));
			db.sqlexecute.Parameters.AddWithValue("@claimID", claimID);
			db.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
			db.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;

			db.ExecuteProc("AllowClaimProgression");

			return (int)db.sqlexecute.Parameters["@returnValue"].Value;
		}

		#region claim submittal

		/// <summary>
		/// Flags the claim as paid in the DB. Sends out Check and Pay email.
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="employeeId"></param>
		/// <param name="delegateId"></param>
		public void payClaim(cClaim claim, int employeeId, int? delegateId)
		{
			using (var expData = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				expData.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
				expData.sqlexecute.Parameters.AddWithValue("@employeeID", employeeId);

				if (delegateId.HasValue)
				{
					expData.sqlexecute.Parameters.AddWithValue("@delegateID", delegateId.Value);
				}
				else
				{
					expData.sqlexecute.Parameters.AddWithValue("@delegateID", DBNull.Value);
				}

				expData.ExecuteProc("payClaim");
				expData.sqlexecute.Parameters.Clear();
			}

			claim.claimPaid();

			if (claim.AmountPayable != 0)
			{
				var notifications = new NotificationTemplates(this.ClaimUser(employeeId));

				try
				{
					var claims = new cClaims(claim.accountid);
					var expenseItemsIds = claims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();
					notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToNotifyAUserTheirExpensesHaveBeenPaid), employeeId, new int[] { claim.employeeid }, expenseItemsIds);
				}
				catch (Exception ex)
				{
					cEventlog.LogEntry("Failed to send Pay Claim email. " + ex.Message);
				}
			}

			UpdateClaimHistory(claim, "This claim has been approved and is due to be paid in the next payment run.", employeeId);
			claim = this.getClaimById(claim.claimid);
			if (claim.PayBeforeValidate)
			{
				var submission = new ClaimSubmission(accountid, employeeId);
				submission.SendClaimToNextStage(
					claim,
					false,
					employeeId,
					employeeId,
					delegateId,
					false,
					true);
			}
		}




		public byte CheckClaimTotal(cClaim claim, bool cash, bool credit, bool purchase, CurrentUser currentUser)
		{
			//System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

			decimal? minclaim = currentUser.ExpenseClaimMinimumValue;
			decimal? maxclaim = currentUser.ExpenseClaimMaximumValue;

			if (cash == false && (credit == true || purchase == true))
			{
				return 0;
			}

			if ((minclaim != null && minclaim != 0) || (maxclaim != null && maxclaim != 0))
			{
				decimal total = claim.Total;

				if (cash != credit)
				{
					total -= claim.CreditCardTotal;
				}
				if (cash != purchase)
				{
					total -= claim.PurchaseCardTotal;
				}

				if (minclaim != null)
				{
					if (total < minclaim)
					{
						return 1;
					}
				}
				if (maxclaim != null)
				{
					if (total > maxclaim)
					{
						return 2;
					}
				}
			}

			return 0;
		}

		public void moveItems(int oldclaimid, int newclaimid, bool cash, bool credit, bool purchase)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			string strsql = "update savedexpenses set claimid = @newclaimid where claimid = @oldclaimid and (";
			if (cash)
			{
				strsql += "itemtype = 1";
			}
			if (credit)
			{
				if (cash)
				{
					strsql += " or ";
				}
				strsql += "itemtype = 2";
			}
			if (purchase)
			{
				if (cash || credit)
				{
					strsql += " or ";
				}
				strsql += "itemtype = 3";
			}
			strsql += ")";

			expdata.sqlexecute.Parameters.AddWithValue("@newclaimid", newclaimid);
			expdata.sqlexecute.Parameters.AddWithValue("@oldclaimid", oldclaimid);
			expdata.sqlexecute.Parameters.AddWithValue("@credit", Convert.ToByte(credit));
			expdata.sqlexecute.Parameters.AddWithValue("@purchase", Convert.ToByte(purchase));
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}

		private bool checkFrequency(int employeeid)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

            var generalOptions = this._generalOptionsFactory.Value[cMisc.GetCurrentUser().CurrentSubAccountId].WithClaim();

            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            if (generalOptions.Claim.LimitFrequency == false)
            {
                return true;
            }

            byte frequencytype = generalOptions.Claim.FrequencyType;
            int frequencyvalue = generalOptions.Claim.FrequencyValue;

			DateTime startdate;

			if (frequencytype == 1) //monthly
			{
				startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
			}
			else //weekly
			{
				startdate = DateTime.Today;
				while (startdate.DayOfWeek != DayOfWeek.Monday)
				{
					startdate = startdate.AddDays(-1);
				}
			}

			//get count
			const string strsql = "select count(*) from claims_base where employeeid = @employeeid and datesubmitted >= @date";
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			expdata.sqlexecute.Parameters.AddWithValue("@date", startdate.Year + "/" + startdate.Month + "/" + startdate.Day);
			int count = expdata.getcount(strsql);
			expdata.sqlexecute.Parameters.Clear();

			return count < frequencyvalue;
		}

		/// <summary>
		/// Set claim to approved.
		/// </summary>
		/// <param name="claim">
		/// The claim.
		/// </param>
		/// <param name="employeeid">
		/// The employee ID.
		/// </param>
		/// <param name="delegateid">
		/// The delegate ID.
		/// </param>
		/// <param name="payBeforeValidate">
		/// True if pay before validate stage is triggering this update.
		/// </param>
		internal void ApproveClaim(cClaim claim, int employeeid, int? delegateid, bool payBeforeValidate)
		{
			int commenter;
			if (delegateid == null)
			{
				commenter = employeeid;
			}
			else
			{
				commenter = (int)delegateid;
			}

			var modifiedon = DateTime.Now.ToUniversalTime();

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				connection.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
				connection.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
				connection.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
				connection.sqlexecute.Parameters.AddWithValue("@payBeforeValidate", payBeforeValidate);
				string strsql = "update savedexpenses set tempallow = 1, returned = 0 where claimid = @claimid";
				connection.ExecuteSQL(strsql);
				if (payBeforeValidate)
				{
					strsql = "update claims_base set approved = 1, status = 3, ModifiedOn = @modifiedon, ModifiedBy = @userid, PayBeforeValidate = @payBeforeValidate where claimid = @claimid";
				}
				else
				{
					strsql = "update claims_base set approved = 1, status = 6, ModifiedOn = @modifiedon, ModifiedBy = @userid, PayBeforeValidate = @payBeforeValidate where claimid = @claimid";
				}

				connection.ExecuteSQL(strsql);
				connection.sqlexecute.Parameters.Clear();
			}

			if (!claim.PayBeforeValidate)
			{
				this.UpdateClaimHistory(claim, "The claim has been approved and is awaiting payment", commenter);
			}
		}

		/// <summary>
		/// Approve multiple expense items - DO NOT USE
		/// </summary>
		/// <param name="items">The expense items to approve</param>
		/// <param name="connection">A mockable database connection to override the default connection</param>
		[Obsolete("Please do not use this method directly, instead use cClaims.AllowExpenseItems")]
		public void approveItems(List<int> items, IDBConnection connection = null)
		{
			if (items.Count == 0)
			{
				return;
			}

			using (IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				expdata.AddWithValue("@ids", items);

				expdata.ExecuteProc("allowExpenseItems");
				expdata.sqlexecute.Parameters.Clear();
			}
		}
		/// <summary>
		/// send mail to line manager of employee
		/// </summary>
		/// <param name="reqStage">stage of claim</param>
		/// <param name="claim">claim item</param>
		/// <param name="senderId">employee id of claim</param>
		/// <param name="recieverId">employee id of reciever</param>
		public void SendClaimMail(cStage reqStage, cClaim claim, int senderId, int recieverId)
		{
			int[] nextcheckerid = new int[1];
			nextcheckerid[0] = recieverId;
			if (reqStage.sendmail)
			{
				var notifications = new NotificationTemplates(this.ClaimUser(senderId));
				var claims = new cClaims(claim.accountid);
				var expenseItemsIds = claims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();

				var templateId = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnAdministratorAfterASetOfExpensesHaveBeenSubmitted);

				var currentNotificationTemplate = notifications.Get(templateId);
				// Send Email
				notifications.SendMessage(templateId, (int)senderId, nextcheckerid, expenseItemsIds);
				// Send Push Messages
				new PushNotificationEngine(currentNotificationTemplate, nextcheckerid.ToList(), claim.claimid, this._accountId).SendPushMessagesAsync();
			}
		}

		/// <summary>
		/// Get authoriser level amount by employee
		/// </summary>
		/// <param name="employeeid"></param>
		/// <param name="accountId"></param>
		/// <returns></returns>

		public decimal? GetAuthoriserLevelAmountByEmployee(int employeeid, int accountId)
		{
			decimal? authoriserLevelAmount = null;
			using (IDBConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				expdata.sqlexecute.Parameters.Clear();
				expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeid);
				// Following stored procedure checks if any Authoriser Level assigned to current approver. Returns NULL for no AL, -1 for DAL, or actual amount if assigned.
				authoriserLevelAmount = expdata.ExecuteScalar<decimal?>("dbo.GetAuthoriserLevelAmountByEmployee", CommandType.StoredProcedure);
				expdata.sqlexecute.Parameters.Clear();
			}

			return authoriserLevelAmount;
		}


		/// <summary>
		/// Updates approvers' approved amount details for a particular employee(claimant) for a particular month
		/// </summary>
		/// <param name="claim">Current Claim</param>
		/// <param name="expenseItems">Expense item of current claim</param>
		/// <param name="checkerId">checkerId of claim</param>
		/// <returns>Id of the updated record</returns>
		public int UpdateApproverApprovedBalance(cClaim claim, List<int> expenseItems, int checkerId)
		{
			int identity = 0;


			List<SqlDataRecord> expenseItemIds = new List<SqlDataRecord>();
			SqlDataRecord row;
			SqlMetaData[] tvpexpenseItems = { new SqlMetaData("SavedExpencesId", System.Data.SqlDbType.Int) };
			//string expenseItemIds = string.Empty;
			foreach (var item in expenseItems)
			{
				row = new SqlDataRecord(tvpexpenseItems);
				row.SetInt32(0, (int)item);
				expenseItemIds.Add(row);
			}
			//expenseItemIds = expenseItemIds.Substring(0, expenseItemIds.Length - 1);

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				connection.sqlexecute.Parameters.AddWithValue("@ClaimId", claim.claimid);
				connection.sqlexecute.Parameters.AddWithValue("@ClaimantId", claim.employeeid);
				connection.sqlexecute.Parameters.AddWithValue("@CheckerId", checkerId);
				connection.sqlexecute.Parameters.AddWithValue("@CreatedOn", DateTime.UtcNow);
				connection.sqlexecute.Parameters.Add("@SavedExpenseIds", System.Data.SqlDbType.Structured);
				connection.sqlexecute.Parameters["@SavedExpenseIds"].Value = expenseItemIds;
				connection.ExecuteProc("dbo.SaveClaimApproverDetails");
				connection.sqlexecute.Parameters.Clear();
			}
			return identity;

		}


		/// <summary>
		/// Gets the id of line manager of current approver
		/// </summary>
		/// <param name="checkerId">Id of current approver for which line manager to be found</param>
		/// <param name="connection">Database connection</param>
		/// <returns></returns>
		public int? GetIdOfLineManagerOfCurrentApprover(int checkerId)
		{
			int? currentApproverLineManager = null;
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				connection.sqlexecute.Parameters.AddWithValue("@employeeId", checkerId);
				using (var reader = connection.GetReader("dbo.GetLineManagerOfAnEmployee", CommandType.StoredProcedure))
				{
					if (reader != null)
					{
						while (reader.Read())
						{
							if (!reader.IsDBNull(reader.GetOrdinal("linemanager")))
							{
								currentApproverLineManager = reader.GetInt32(reader.GetOrdinal("linemanager"));
							}

						}
					}
				}
				connection.sqlexecute.Parameters.Clear();
			}
			return currentApproverLineManager;
		}

		/// <summary>
		/// Reset expenses item for approval
		/// </summary>
		/// <param name="claim">Current claim</param>
		public void ResetApproval(cClaim claim)
		{
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				expdata.sqlexecute.Parameters.Clear();

				expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
				const string strsql = "update savedexpenses set tempallow = 0, itemCheckerId = null where claimid = @claimid";
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}
		}

		/// <summary>
		/// Reset expenses item for approval when expenses item is not apporoved.
		/// </summary>
		/// <param name="claim">Current claim</param>
		public void ResetItemCheckerId(cClaim claim)
		{
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				expdata.sqlexecute.Parameters.Clear();

				expdata.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
				const string strsql = "update savedexpenses set itemCheckerId = null where claimid = @claimid and tempallow = 0";
				expdata.ExecuteSQL(strsql);
				expdata.sqlexecute.Parameters.Clear();
			}
		}

		/// <summary>
		/// Get the total amount approved by an approver for a particular employee for that month.
		/// </summary>
		/// <param name="claim">Current submitted claim</param>
		/// <returns>Returns total amount approved by an approver for a particular employee for a particular month</returns>
		public decimal? GetApprovedAmountByChecker(cClaim claim, int checkerId)
		{
			decimal? claimAmount = 0;
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				var dateTimeNowInUTC = DateTime.UtcNow;
				var monthFirstDayDate = new DateTime(dateTimeNowInUTC.Year, dateTimeNowInUTC.Month, 1);
				var monthLastDayDate = monthFirstDayDate.AddMonths(1).AddSeconds(-1);
				expdata.sqlexecute.Parameters.Clear();
				expdata.sqlexecute.Parameters.Add("@ClaimantId", claim.employeeid);
				expdata.sqlexecute.Parameters.Add("@CheckerId", checkerId);
				expdata.sqlexecute.Parameters.Add("@StartDate", monthFirstDayDate);
				expdata.sqlexecute.Parameters.Add("@EndDate", monthLastDayDate);
				using (var reader = expdata.GetReader("dbo.GetApprovedAmountByChecker", CommandType.StoredProcedure))
				{
					if (reader != null)
					{
						while (reader.Read())
						{
							if (!reader.IsDBNull(reader.GetOrdinal("amount")))
							{
								claimAmount = reader.GetDecimal(reader.GetOrdinal("amount"));
							}
						}
					}
				}
				expdata.sqlexecute.Parameters.Clear();
			}
			return claimAmount;
		}

		/// <summary>
		/// Delate claim approver detail by SavedExpenseId
		/// </summary>
		/// <param name="savedExpenseId">SavedExpenseId of saveexpenses</param>
		/// <param name="checkerId">CheckerId of saveexpenses</param>
		public void DeleteClaimApproverDetailByExpensesId(int savedExpenseId, int? checkerId)
		{
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				expdata.sqlexecute.Parameters.Clear();
				expdata.sqlexecute.Parameters.Add("@ExpenseId", savedExpenseId);
				expdata.sqlexecute.Parameters.Add("@CheckerId", checkerId);
				expdata.ExecuteProc("dbo.DeleteClaimApproverDetails");
				expdata.sqlexecute.Parameters.Clear();
			}
		}

        /// <summary>
        /// Update itemCheckerId for CaseOfCostCode
        /// </summary>
        /// <param name="expenseItem">List of expenseItems</param>
        /// <param name="checkerId">CheckerId of saveexpenses</param>
        public void UpdateItemCheckerIdInCaseOfCostCode(List<int> expenseItems, int? checkerId)
        {
            List<SqlDataRecord> expenseItemIds = new List<SqlDataRecord>();
            SqlDataRecord row;
            SqlMetaData[] tvpexpenseItems = { new SqlMetaData("SavedExpencesId", System.Data.SqlDbType.Int) };
            //string expenseItemIds = string.Empty;
            foreach (var item in expenseItems)
            {
                row = new SqlDataRecord(tvpexpenseItems);
                row.SetInt32(0, (int)item);
                expenseItemIds.Add(row);
            }
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@SavedExpenseIds", System.Data.SqlDbType.Structured);
                expdata.sqlexecute.Parameters["@SavedExpenseIds"].Value = expenseItemIds;

                expdata.sqlexecute.Parameters.Add("@CheckerId", checkerId);
                expdata.ExecuteProc("dbo.UpdateItemCheckerIdSavedExpenses");
                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Delate claim approver detail ExpenceItems
        /// </summary>
        /// <param name="expenseItem">List of expenseItems</param>
        /// <param name="checkerId">CheckerId of saveexpenses</param>
         public void DeleteClaimApproverDetailExpenseItems(List<int> expenseItems, int? checkerId)
        {
            List<SqlDataRecord> expenseItemIds = new List<SqlDataRecord>();
            SqlDataRecord row;
            SqlMetaData[] tvpexpenseItems = { new SqlMetaData("SavedExpencesId", System.Data.SqlDbType.Int) };
            //string expenseItemIds = string.Empty;
            foreach (var item in expenseItems)
            {
                row = new SqlDataRecord(tvpexpenseItems);
                row.SetInt32(0, (int)item);
                expenseItemIds.Add(row);
            }
            //expenseIds = expenseIds.Substring(0, expenseIds.Length - 1);
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                expdata.sqlexecute.Parameters.Clear();
                expdata.sqlexecute.Parameters.Add("@SavedExpenseIds", System.Data.SqlDbType.Structured);
                expdata.sqlexecute.Parameters["@SavedExpenseIds"].Value = expenseItemIds;

                if (checkerId.HasValue)
                {
                    expdata.sqlexecute.Parameters.Add("@CheckerId", checkerId);
                }

                expdata.ExecuteProc("dbo.DeleteClaimApproverDetailExpenceItems");
                expdata.sqlexecute.Parameters.Clear();
            }
        }
 
 

		/// <summary>
		/// Get id of an employee who is assigned default authoriser level.
		/// </summary>
		/// <returns>Default authoriser id</returns>
		public int? GetDefaultAuthoriserLevelEmployeeId()
		{
			int? GetDefaultAuthoriserLevelId = null;
			using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				using (var reader = expdata.GetReader("dbo.GetDefaultAuthoriserLevelEmployeeId", CommandType.StoredProcedure))
				{
					if (reader != null)
					{
						while (reader.Read())
						{
							if (!reader.IsDBNull(reader.GetOrdinal("employeeid")))
							{
								GetDefaultAuthoriserLevelId = reader.GetInt32(reader.GetOrdinal("employeeid"));
							}

						}
					}
				}
			}
			return GetDefaultAuthoriserLevelId;
		}


		/// <summary>
		/// Update checker id into database for selected claim
		/// </summary>
		/// <param name="claim">selected claim</param>
		/// <param name="commenter">history commenter</param>
		/// <param name="checkerId">Employee Id of next approver</param>
		public void UpdateCheckerIntoDatabase(int? checkerId, cClaim claim, int commenter)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				connection.sqlexecute.Parameters.AddWithValue("@CheckerId", checkerId);
				connection.sqlexecute.Parameters.AddWithValue("@ClaimId", claim.claimid);
				connection.ExecuteProc("dbo.UpdateChecker");
				UpdateCheckerClaimHistory(checkerId, claim, commenter);
			}
		}

		/// <summary>
		/// Adds a record to the claim history to show who the new approver is
		/// </summary>
		/// <param name="claim">selected claim</param>
		/// <param name="commenter">history commenter</param>
		/// <param name="checkerId">Employee Id of next approver</param>
		public void UpdateCheckerClaimHistory(int? checkerId, cClaim claim, int? commenter)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				cEmployees employees = new cEmployees(this.accountid);
				Employee nextChecker = employees.GetEmployeeById((int)checkerId);
				string comment = string.Format("The claim has been sent to the next level and is awaiting approval by {0} {1} {2}", nextChecker.Title, nextChecker.Forename, nextChecker.Surname);
				connection.sqlexecute.Parameters.AddWithValue("@ClaimId", claim.claimid);
				connection.sqlexecute.Parameters.AddWithValue("@DateStamp", DateTime.Now);
				connection.sqlexecute.Parameters.AddWithValue("@Comment", comment);
				connection.sqlexecute.Parameters.AddWithValue("@Stage", claim.stage);
				connection.sqlexecute.Parameters.AddWithValue("@EmployeeId", commenter);
				connection.sqlexecute.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
				connection.ExecuteProc("dbo.SaveClaimHistory");
				connection.sqlexecute.Parameters.Clear();
			}
		}

		/// <summary>
		/// Adds a record to the claim history to indicate a part of the claim has been approved
		/// </summary>
		/// <param name="claim">selected claim</param>
		/// <param name="commenter">history commenter</param>
		/// <param name="checkerId">Employee Id of current approver</param>
		public void UpdateSplitClaimHistory(int? checkerId, cClaim claim, int? commenter)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.Clear();
				cEmployees employees = new cEmployees(this.accountid);
				Employee currentChecker = employees.GetEmployeeById((int)checkerId);
				string comment = string.Format("{0} {1} {2} has approved part(s) of the claim", currentChecker.Title, currentChecker.Forename, currentChecker.Surname);
				connection.sqlexecute.Parameters.AddWithValue("@ClaimId", claim.claimid);
				connection.sqlexecute.Parameters.AddWithValue("@DateStamp", DateTime.Now);
				connection.sqlexecute.Parameters.AddWithValue("@Comment", comment);
				connection.sqlexecute.Parameters.AddWithValue("@Stage", claim.stage);
				connection.sqlexecute.Parameters.AddWithValue("@EmployeeId", commenter);
				connection.sqlexecute.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
				connection.ExecuteProc("dbo.SaveClaimHistory");
				connection.sqlexecute.Parameters.Clear();
			}
		}

		/// <summary>
		/// Un Submit the current claim
		/// </summary>
		/// <param name="claim">The claim to un submit</param>
		/// <param name="approver">The current approver</param>
		/// <param name="employeeid">The current Employee ID</param>
		/// <param name="delegateid">The current delegate ID (if any).</param>
		/// <returns><see cref="ClaimUnsubmittableReason"/></returns>
		public ClaimUnsubmittableReason UnSubmitclaim(cClaim claim, bool approver, int employeeid, int? delegateid)
		{
			if (!claim.submitted)
			{
				return ClaimUnsubmittableReason.Unsubmitted;
			}

			var clsemployees = new cEmployees(this.accountid);
			Employee employee = clsemployees.GetEmployeeById(employeeid);
			SortedList<int, cExpenseItem> expenseItems = this.getExpenseItemsFromDB(claim.claimid);
			var itemsAllowed = expenseItems.Values.Any(expenseItem => expenseItem.tempallow);

			if (approver)
			{
				itemsAllowed = false;
			}

			var returnValue = this.CanClaimBeUnSubmitted(claim, expenseItems, itemsAllowed);
			if (returnValue != ClaimUnsubmittableReason.Unsubmitable)
			{
				return returnValue;
			}

			this.UpdateApproverLastRemindedDate(claim.claimid, 0);

            var user = cMisc.GetCurrentUser();

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
            {
                var clsEmployeeCars = new cEmployeeCars(this.accountid, claim.employeeid);

				int commenter;
				if (delegateid == null)
				{
					commenter = employeeid;
				}
				else
				{
					commenter = (int)delegateid;
				}
				var email = new NotificationTemplates(this.ClaimUser(claim.employeeid));
				var claims = new cClaims(claim.accountid);
				var expenseItemsIds = claims.getExpenseItemsFromDB(claim.claimid).Keys.ToList();
				if (approver == true)
				{
					this.UpdateClaimHistory(claim, "The claim has been unsubmitted by your approver.", commenter);
					// send email                   
					email.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnClaimantToNotifyThemIfTheirClaimHasBeenUnsubmittedByAnAdministrator), employee.EmployeeID, new int[] { claim.employeeid }, expenseItemsIds.Count == 0 ? null : expenseItemsIds);
				}
				else
				{
					this.UpdateClaimHistory(claim, "Claim unsubmitted by claimant.", commenter);
					int[] checkerIds = GetApproverIds(claim);
					List<int> approvers = checkerIds.ToList();

					//remove the current user who is approver from the approverList
					if (checkerIds.Count() > 0)
					{
						var users = cMisc.GetCurrentUser();
						foreach (int id in checkerIds)
						{
							if (id == users.EmployeeID)
							{
								approvers.Remove(id);
							}
						}
						checkerIds = approvers.ToArray();
						email.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToArroverWhenClaimIsUnsubmittedByClaimant), employee.EmployeeID, checkerIds, expenseItemsIds.Count == 0 ? null : expenseItemsIds);
					}
				}

				var claimSubmission = new ClaimSubmission(this.accountid, employeeid);
				claimSubmission.ResetApproval(claim);
				connection.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);

				DateTime modifiedon = DateTime.UtcNow;

				connection.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
				connection.sqlexecute.Parameters.AddWithValue("@userid", employeeid);
				var strsql =
					"update claims_base set checkerid = null, teamid = null, datesubmitted = null, submitted = 0, status=0, approved = 0, stage = 0, splitApprovalStage = 0, ModifiedOn = @modifiedon, ModifiedBy = @userid where claimid = @claimid";

				connection.ExecuteSQL(strsql);

                var generalOptions = this._generalOptionsFactory.Value[user.CurrentSubAccountId].WithMileage();

                if (generalOptions.Mileage.EnterOdometerOnSubmit && generalOptions.Mileage.RecordOdometer)
                {
                    Employee claimemp = clsemployees.GetEmployeeById(claim.employeeid);
                    this.deleteLastOdometerReading(claimemp);
                }
                else
                {
                    strsql =
                        "select count(*) from savedexpenses where claimid = @claimid and subcatid in (select subcatid from subcats where calculation = 6)";

					int businessmilescount = connection.ExecuteScalar<int>(strsql);
					if (businessmilescount != 0)
					{
						var cars = this.GetEmployeesActiveCarsWithAFuelCard(clsEmployeeCars);
						foreach (cCar car in cars)
						{
							cOdometerReading reading = car.getLastOdometerReading();
							clsemployees.deleteOdometerReading(employee.EmployeeID, car.carid, reading.odometerid);
						}

						// undo the mileage
						strsql =
							"update savedexpenses set net = 0, vat = 0, total = 0, amountpayable = 0 where claimid = @claimid and subcatid in (select subcatid from subcats where calculation = 6)";

						connection.ExecuteSQL(strsql);
					}
				}
			}

			claim.unSubmitClaim();

			var clsSubcats = new cSubcats(accountid);
			var lstDelItems = new List<cExpenseItem>();

            List<SignoffType> claimStages = this.GetSignoffStagesAsTypes(claim);
          

            foreach (cExpenseItem item in expenseItems.Values)
            {
                if (claimStages.Contains(SignoffType.SELValidation) && item.ValidationProgress == ExpenseValidationProgress.NotSelectedForValidation)
                {
                    var validationManager = new ExpenseValidationManager(this.accountid);
                    validationManager.UpdateProgressForExpenseItem(item.expenseid, item.ValidationProgress, ExpenseValidationProgress.Required);
                }

                // Delete any reimbursable fuel card items
                SubcatBasic subcat = clsSubcats.GetSubcatBasic(item.subcatid);

				if (subcat != null)
				{
					if (subcat.CalculationType == CalculationType.ItemReimburse)
					{
						lstDelItems.Add(item);
					}
				}
			}

			foreach (cExpenseItem delItem in lstDelItems)
			{
				this.deleteExpense(claim, delItem, false, user);
			}

			var log = new cAuditLog();
			log.editRecord(
				claim.claimid,
				claim.name,
				SpendManagementElement.Claims,
				new Guid("47DB6E7D-78AC-4322-8211-359DDCA0C1AB"),
				"Submitted",
				"Unsubmitted");

			return ClaimUnsubmittableReason.Unsubmitted;
		}

		/// <summary>
		/// Get set of Approver ids
		/// </summary>
		/// <param name="claim">The claim that you need the approver ids for</param>
		/// <param name="notifyUnsubmission">Check whether you need to filter out the checker ids with 'Notify claim unsubmission' disabled against the employee record</para>
		/// <returns>List of employeeids of approver</returns>
		private int[] GetApproverIds(cClaim claim, bool notifyUnsubmission = true)
		{
			var groups = new cGroups(claim.accountid);
			cGroup group = this.GetGroupForClaim(groups, claim);
			int claimStage = 0;

			if (claim.stage > 0)
			{
				claimStage = claim.stage - 1;
			}
			cStage stage = group.stages.Values[claimStage];
			int[] checkerIds;
			var teams = new cTeams(claim.accountid);
			cEmployees employees = new cEmployees(claim.accountid);
			Employee requiredEmployee = employees.GetEmployeeById(claim.employeeid);
			SignoffType signoffType = stage.signofftype;
			int relid = stage.relid;
			var user = cMisc.GetCurrentUser();
			var claimSubmission = new ClaimSubmission(user);
			if (signoffType == SignoffType.ApprovalMatrix)
			{
				relid = claimSubmission.SetRelIdforApprovalMatrixStage(claim, relid, stage, claim.Total, requiredEmployee, true, user.EmployeeID, ref signoffType, false);
			}
			switch (signoffType)
			{

				case SignoffType.Team:
					cTeam team = teams.GetTeamById(relid);
					List<int> teamMembers = teams.GetTeamMembers(stage.relid);
					checkerIds = teamMembers.Distinct().ToArray();
					if (notifyUnsubmission)
					{
						checkerIds = FilterCheckerIdsWithNotificationEnabled(claim, teamMembers).ToArray();
					}
					break;
				case SignoffType.CostCodeOwner:
					var subAccounts = new cAccountSubAccounts(claim.accountid);
					var subAccount = subAccounts.getFirstSubAccount();
					var ccoCheckers = claimSubmission.GetExpenseItemCheckerIds(claim.claimid, claim.employeeid, subAccount.SubAccountID);
					List<int> costCodeOwners = ccoCheckers.ToList();
					checkerIds = costCodeOwners.Distinct().ToArray();
					if (notifyUnsubmission)
					{
						checkerIds = FilterCheckerIdsWithNotificationEnabled(claim, costCodeOwners).ToArray();
					}
					break;
				case SignoffType.AssignmentSignOffOwner:
					claims claims = new claims();
					var expenseItems = this.getExpenseItemsFromDB(claim.claimid);
					var assignmentSignOffOwner = (from expenseItem in expenseItems.Values where expenseItem.itemCheckerId.HasValue select expenseItem.itemCheckerId.Value).ToList();
					var teamIds = (from expenseItem in expenseItems.Values where expenseItem.ItemCheckerTeamId != null select (int)expenseItem.ItemCheckerTeamId).ToArray();
					foreach (var teamId in teamIds)
					{
						cTeam requiredTeam = teams.GetTeamById(teamId);
						if (requiredTeam != null)
						{
							assignmentSignOffOwner.AddRange(requiredTeam.teammembers);
						}
					}
					checkerIds = assignmentSignOffOwner.Distinct().ToArray();
					if (notifyUnsubmission)
					{
						checkerIds = FilterCheckerIdsWithNotificationEnabled(claim, assignmentSignOffOwner).ToArray();
					}
					break;
				case SignoffType.LineManager:
					checkerIds = new int[1];
					Employee lineManager = employees.GetEmployeeById(requiredEmployee.LineManager);
					if (lineManager != null && lineManager.NotifyClaimUnsubmission && notifyUnsubmission)
					{
						checkerIds[0] = lineManager.EmployeeID;
					}
					break;

				case SignoffType.BudgetHolder:
					checkerIds = new int[1];
					var budgetHolders = new cBudgetholders(this.accountid);
					cBudgetHolder budgetHolder = budgetHolders.getBudgetHolderById(relid);
					if (budgetHolder != null)
					{
						requiredEmployee = employees.GetEmployeeById(budgetHolder.employeeid);
						if (requiredEmployee.NotifyClaimUnsubmission && notifyUnsubmission)
						{
							checkerIds[0] = requiredEmployee.EmployeeID;
						}
					}
					break;

				case SignoffType.Employee:
				case SignoffType.DeterminedByClaimantFromApprovalMatrix:
				case SignoffType.ClaimantSelectsOwnChecker:
					checkerIds = new int[1];
					Employee employee = employees.GetEmployeeById(relid);
					if (employee != null && employee.NotifyClaimUnsubmission && notifyUnsubmission)
					{
						checkerIds[0] = relid;
					}
					break;
				default:
					checkerIds = new int[1];
					checkerIds[0] = 0;
					break;
			}
			return checkerIds;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="claim"></param>
		/// <param name="approverIds"></param>
		/// <returns>List of approver ids with notification enabled</returns>
		private List<int> FilterCheckerIdsWithNotificationEnabled(cClaim claim, List<int> approverIds)
		{
			List<int> checkerIds = new List<int>();
			int noOfApprovers = approverIds.Count;
			checkerIds = approverIds;
			for (int i = 0; i < approverIds.Count; i++)
			{
				cEmployees employees = new cEmployees(claim.accountid);
				Employee approver = employees.GetEmployeeById(approverIds[i]);
				if (!approver.NotifyClaimUnsubmission)
				{
					checkerIds.RemoveAt(i);
				}
			}
			return checkerIds;
		}


		/// <summary>
		/// Show Unallocate Icon
		/// </summary>
		/// <param name="accountId">AccountId of user</param>
		/// <param name="claimId">Claim Id of Claim</param>
		public bool ShowUnallocateIcon(int claimId, int accountId)
		{
			bool isShowUnallocateIcon = false;
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				connection.sqlexecute.Parameters.Clear();
				connection.sqlexecute.Parameters.AddWithValue("@ClaimId", claimId);
				isShowUnallocateIcon = connection.ExecuteScalar<bool>("dbo.ShowUnallocateIcon", CommandType.StoredProcedure);
			}
			return isShowUnallocateIcon;
		}

		/// <summary>
		/// The can claim be un submitted.
		/// </summary>
		/// <param name="claim">
		/// The claim to check.
		/// </param>
		/// <param name="expenseItems">
		/// The expense items on the claim.
		/// </param>
		/// <param name="itemsAllowed">
		/// True if any of the expense items are set to "allowed".
		/// </param>
		/// <returns>
		/// The <see cref="ClaimUnsubmittableReason"/>.
		/// </returns>
		private ClaimUnsubmittableReason CanClaimBeUnSubmitted(cClaim claim, SortedList<int, cExpenseItem> expenseItems, bool itemsAllowed)
		{
			var account = new cAccounts().GetAccountByID(this.accountid);
			var claimStages = this.GetSignoffStagesAsTypes(claim);

			if (claim.paid)
			{
				return ClaimUnsubmittableReason.AlreadyPaid;
			}

			if (claim.status == ClaimStatus.None
				|| (claim.status == ClaimStatus.NextStageAwaitingAction && !itemsAllowed)
				|| claim.status == ClaimStatus.AwaitingAllocation)
			{

				// if the receipts service is enabled
				if (account.ReceiptServiceEnabled)
				{
					if (claim.PayBeforeValidate)
					{
						{
							return ClaimUnsubmittableReason.AlreadyPaid;
						}
					}

					// if the current stage is Scan, then check for envelopes
					if ( /*claimStages[claim.stage - 1] == SignoffType.SELScanAttach && */
						!string.IsNullOrWhiteSpace(claim.ReferenceNumber)) // uncomment the commented bit after FriendsLife migration
					{
						var envelopes = new Envelopes();
						var envelopesForThisClaim = envelopes.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);

						if (
							envelopesForThisClaim.Any(
								envelope =>
								envelope.Status >= EnvelopeStatus.ReceivedBySEL && envelope.DateReceived.HasValue))
						{
							{
								return ClaimUnsubmittableReason.StartedApprovalProcess;
							}
						}
					}
					else if (this.HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELScanAttach, claimStages))
					{
						{
							return ClaimUnsubmittableReason.ClaimHasBeenInvolvedInSelStage;
						}
					}
				}

				return ClaimUnsubmittableReason.Unsubmitable;
			}

			// if the validation service is enabled, check for any expenseitems that have a validation status of in progress or higher.
			if (account.ValidationServiceEnabled && claimStages.Contains(SignoffType.SELValidation))
			{
				// if current stage is validation check for any results.
				if (claimStages[claim.stage - 1] == SignoffType.SELValidation)
				{
					if (expenseItems.Values.Any(item => item.ValidationProgress >= ExpenseValidationProgress.InProgress))
					{
						{
							return ClaimUnsubmittableReason.ItemsApproved;
						}
					}
				}
				else if (this.HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELValidation, claimStages))
				{
					{
						return ClaimUnsubmittableReason.ClaimHasBeenInvolvedInSelStage;
					}
				}
			}

			return claim.PayBeforeValidate == true ? ClaimUnsubmittableReason.AlreadyPaid : ClaimUnsubmittableReason.Unsubmitable;
		}

		/// <summary>
		/// Assign the itemCheckerId for each expense item in the claim when stage is Cost Code Owner
		/// </summary>
		/// <param name="claim">Claim to update the item checkers for</param>
		/// <param name="itemCheckerType">Whether to update item checkers or assignment supervisor item checkers.</param>
		/// <param name="validatingOnly">Whether to not commit the changes to the DB, using it for validation purposes only.</param>
		public int UpdateItemCheckers(cClaim claim, ItemCheckerType itemCheckerType = ItemCheckerType.Normal, bool validatingOnly = false)
		{
			int returnValue;
			string sql;

			switch (itemCheckerType)
			{
				case ItemCheckerType.Normal:
					sql = "dbo.UpdateItemCheckers";
					break;
				case ItemCheckerType.AssignmentSupervisor:
					sql = "dbo.UpdateAssignmentSupervisorItemCheckers";
					break;
				default:
					throw new ArgumentOutOfRangeException("itemCheckerType");
			}

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
				connection.sqlexecute.Parameters.AddWithValue("@stage", claim.stage);
				connection.sqlexecute.Parameters.AddWithValue("@validatingOnly", validatingOnly);
				connection.sqlexecute.Parameters.Add("@retVal", SqlDbType.Int);
				connection.sqlexecute.Parameters["@retVal"].Direction = ParameterDirection.ReturnValue;

				connection.ExecuteProc(sql);

				returnValue = (int)connection.sqlexecute.Parameters["@retVal"].Value;
			}

			return returnValue;
		}

		public void deleteLastOdometerReading(Employee reqemp)
		{
			cEmployeeCars clsEmployeeCars = new cEmployeeCars(accountid, reqemp.EmployeeID);
			var cars = this.GetEmployeesActiveCarsWithAFuelCard(clsEmployeeCars);
			cEmployees clsemployees = new cEmployees(accountid);
			foreach (cCar car in cars)
			{
				cOdometerReading reading = car.getLastOdometerReading();
				clsemployees.deleteOdometerReading(reqemp.EmployeeID, car.carid, reading.odometerid);
			}
		}

        /// <summary>
        /// Return multiple expense items
        /// </summary>
        /// <param name="claim">The expense claim</param>
        /// <param name="items">The expense items to return</param>
        /// <param name="reason">The reason for returning the items</param>
        /// <param name="currentUserId">The employee returning the items</param>
        /// <param name="delegateId">The employee returning the items if doing it as a delegate</param>
        /// <param name="connection">A mockable connection to override the default database connection</param>
        /// <param name="isFromValidation">Whether this return is from the SEL Validation stage. Defaults to false.</param>
        /// <returns>Boolean false</returns>
        public virtual bool ReturnExpenses(cClaim claim, List<int> items, string reason, int? currentUserId, int? delegateId, IDBConnection connection = null, bool isFromValidation = false)
        {
            try
            {
                using (IDBConnection expdata = connection ?? new DatabaseConnection(cAccounts.getConnectionString(accountid)))
                {
                    expdata.AddWithValue("@claimId", claim.claimid);
                    expdata.AddWithValue("@ids", items);

                    if (delegateId.HasValue)
                    {
                        expdata.AddWithValue("@userid", delegateId);
                    }
                    else if (currentUserId.HasValue)
                    {
                        expdata.AddWithValue("@userid", currentUserId);
                    }
                   
                    expdata.AddWithValue("@modifiedon", DateTime.UtcNow);
                    expdata.AddWithValue("@reason", reason, 4000);

                    expdata.ExecuteProc("returnExpenses");
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
            catch (Exception)
            {
                return false;
            }

			changeStatus(claim, ClaimStatus.ItemReturnedAwaitingEmployee, currentUserId);

            var currentEmployeeId = currentUserId.HasValue ? currentUserId.Value : 0;

            var email = new NotificationTemplates(accountid);
            email.SendMessage((Guid)(isFromValidation ? SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantWhenAnExpenseFailsReceiptValidation) : SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAUserIfAnExpenseItemGetsReturned)), currentEmployeeId, new[] { claim.employeeid }, items);
            DeleteClaimApproverDetailExpenseItems(items, currentUserId);
           
            return true;
        }

		/// <summary>
		/// Disputes and expense item, updating the claim history, and emailing the user.
		/// </summary>
		/// <param name="claim">The claim object</param>
		/// <param name="item">The expense item object</param>
		/// <param name="dispute">The reason for the dispute</param>
		/// <param name="isFromMobile">Whether the request is from mobile</param>
		public void DisputeExpense(cClaim claim, cExpenseItem item, string dispute, bool isFromMobile = false)
		{
			addDisputeComment(claim, item, dispute, isFromMobile);

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				const string strsql = "update returnedexpenses set corrected = 1, dispute = @dispute where expenseid = @expenseid";
				connection.AddWithValue("@dispute", dispute);
				connection.AddWithValue("@expenseid", item.expenseid);
				connection.ExecuteSQL(strsql);
				connection.ClearParameters();
			}

			var user = cMisc.GetCurrentUser();
			item.corrected = true;

			this.changeStatus(claim, ClaimStatus.ItemCorrectedAwaitingApprover, claim.employeeid);
			claim.changeStatus(ClaimStatus.ItemCorrectedAwaitingApprover);

			// if the item has validation enabled, update the claim history to indicate invalidation of validation results.
			if (user.Account.ValidationServiceEnabled && (item.ValidationProgress >= ExpenseValidationProgress.WaitingForClaimant))
			{
				var validationManager = new ExpenseValidationManager(accountid);
				var subcat = new cSubcats(user.AccountID).GetSubcatById(item.subcatid);
				var claimStages = GetSignoffStagesAsTypes(claim);
				var isCurrentlyInValidation = HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELValidation, claimStages, true);
				var result = item.DetermineValidationProgress(user.Account, subcat.Validate, claimStages.Contains(SignoffType.SELValidation), validationManager);

				if (isCurrentlyInValidation)
				{
					item.corrected = true;

					// Attempt to reset the validation - the result will dictate whether this was allowed or not.
					result = item.ResetValidation(new ExpenseValidationManager(accountid), true);
				}
				else
				{
					// see if the item has previously passed validation
					if (HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELValidation, claimStages))
					{
						var oldResult = item.ValidationProgress;
						result = item.DetermineValidationProgressValidity(true);
						validationManager.UpdateProgressForExpenseItem(item.expenseid, oldResult, result);
					}
				}

				// update the history
				var notice = result == ExpenseValidationProgress.Required
					? "Expense will be marked for receipt re-validation."
					: "Previous validation results are no longer applicable.";
				UpdateClaimHistory(claim, notice, user.EmployeeID, item.refnum);
			}
			else
			{
				// let the approver know...
				var notifications = new NotificationTemplates(user);
				int[] approverIds = new int[] { claim.checkerid };
				if (claim.checkerid == 0)
				{
					approverIds = this.GetApproverIds(claim, false);
				}
				notifications.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToNotifyAdministratorOfAnyReturnedExpensesBeingCorrected), user.EmployeeID, approverIds, item.expenseid);
				this.UpdateApproverLastRemindedDateWhenApproved(claim.claimid, claim.checkerid);
			}
		}

		private void addDisputeComment(cClaim reqclaim, cExpenseItem item, string dispute, bool isFromMobile = false)
		{
			System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			CurrentUser user = cMisc.GetCurrentUser();

			int employeeid;

			DateTime datestamp = DateTime.Now;

			if (isFromMobile || appinfo.Session["myid"] == null)
			{
				employeeid = user.EmployeeID;
			}
			else
			{
				employeeid = (int)appinfo.Session["myid"];
			}

			string comment = "The returned expense has been disputed: " + dispute;

			expdata.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
			expdata.sqlexecute.Parameters.AddWithValue("@comment", comment);
			expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
			expdata.sqlexecute.Parameters.AddWithValue("@stage", reqclaim.stage);
			expdata.sqlexecute.Parameters.AddWithValue("@refnum", item.refnum);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			string strsql = "insert into claimhistory (claimid, datestamp, comment, stage, refnum, employeeid) " + "values (@claimid,'" + datestamp.Year + "/" + datestamp.Month + "/" + datestamp.Day + " " + datestamp.Hour + ":" + datestamp.Minute + ":" + datestamp.Second + "',@comment,@stage,@refnum,@employeeid)";
			expdata.ExecuteSQL(strsql);
			expdata.sqlexecute.Parameters.Clear();
		}


		/// <summary>
		/// Gets the signoff stages for a claim. The claim is the only required parameter. 
		/// Supply any other parameters that you can. They will be generated if not.
		/// </summary>
		/// <param name="claim">The claim - mandatory</param>
		/// <param name="group">The claim's <see cref="Employee"/>'s signoff group, if you have it.</param>
		/// <param name="employee">The claim's <see cref="Employee"/> if you have it.</param>
		/// <param name="employees">An instance of <see cref="cEmployees"/> if you have one.</param>
		/// <param name="groups">An instance of <see cref="cGroups"/> if you have one.</param>
		/// <returns>A list of <see cref="cStage"/>s</returns>
		/// <exception cref="ArgumentException">When the claim is null.</exception>
		public List<cStage> GetSignoffStages(cClaim claim, cGroup group = null, Employee employee = null, cEmployees employees = null, cGroups groups = null)
		{
			if (claim == null)
			{
				throw new ArgumentException(@"You must provide a claim", "claim");
			}

			if (group != null)
			{
				return group.stages.Values.ToList();
			}

			if (employee == null)
			{
				employees = employees ?? new cEmployees(claim.accountid);
				employee = employees.GetEmployeeById(claim.employeeid);
			}

			groups = groups ?? new cGroups(claim.accountid);
			group = groups.GetGroupById(employee.SignOffGroupID);

			return @group == null ? new List<cStage>() : @group.stages.Values.ToList();
		}

		/// <summary>
		/// Gets the types of each signoff stage for a claim. The claim is the only required parameter. 
		/// Supply any other parameters that you can. They will be generated if not.
		/// </summary>
		/// <param name="claim">The claim - mandatory</param>
		/// <param name="group">The claim's <see cref="Employee"/>'s signoff group, if you have it.</param>
		/// <param name="employee">The claim's <see cref="Employee"/> if you have it.</param>
		/// <param name="employees">An instance of <see cref="cEmployees"/> if you have one.</param>
		/// <param name="groups">An instance of <see cref="cGroups"/> if you have one.</param>
		/// <returns>A list of <see cref="SignoffType"/>s</returns>
		/// <exception cref="ArgumentException">When the claim is null.</exception>
		public List<SignoffType> GetSignoffStagesAsTypes(cClaim claim, cGroup group = null, Employee employee = null, cEmployees employees = null, cGroups groups = null)
		{
			return GetSignoffStages(claim, group, employee, employees, groups).Select(s => s.signofftype).ToList();
		}


		public bool userOnHoliday(SignoffType signofftype, int relid, int claimantId = 0)
		{
			switch (signofftype)
			{
				case SignoffType.ApprovalMatrix:
					return false;
				case SignoffType.CostCodeOwner:
					return false;
				case SignoffType.AssignmentSignOffOwner:
					return false;
			}

			int count;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountid)))
			{
				DateTime today = DateTime.Today;
				string strsql;

				if (signofftype == SignoffType.LineManager)
				{
					relid = claimantId;
				}

				connection.sqlexecute.Parameters.AddWithValue("@relid", relid);
				connection.sqlexecute.Parameters.AddWithValue("@today", today);

				switch (signofftype)
				{
					case SignoffType.BudgetHolder:
						strsql = "select count(holidayid) from holidays inner join budgetholders on budgetholders.employeeid = holidays.employeeid where holidays.employeeid = (select employeeid from budgetholders where budgetholders.budgetholderid = @relid) and (@today between startdate and enddate)";
						break;
					case SignoffType.ClaimantSelectsOwnChecker:
					case SignoffType.Employee:
						strsql = "select count(holidayid) as holidaycount from holidays where employeeid = @relid and (@today between startdate and enddate)";
						break;
					case SignoffType.Team:
						strsql = "IF EXISTS (select count(holidayid), count(*) from teamemps left join holidays on teamemps.employeeid = holidays.employeeid where teamemps.teamid = @relid and ((@today between holidays.startdate and holidays.enddate) OR holidays.startdate is null) having count(holidayid) = count(*)) BEGIN 	SELECT 1 AS holidaycount END ELSE BEGIN SELECT 0 AS holidaycount END";
						break;
					case SignoffType.LineManager:
						strsql = "select count(holidayid) as holidaycount from holidays where employeeid in (select linemanager from employees where employeeid = @relid) and (@today between startdate and enddate)";
						break;
					default:
						return false;

				}

				count = connection.ExecuteScalar<int>(strsql);

				connection.sqlexecute.Parameters.Clear();
			}

			return count != 0;
		}

		public int getNextCheckerId(cClaim claim, SignoffType signofftype, int relid)
		{
			int nextcheckerid = 0;
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			switch (signofftype)
			{
				case SignoffType.BudgetHolder:
					var budgetHolders = new cBudgetholders(this.accountid);
					var holder = budgetHolders.getBudgetHolderById(relid);
					nextcheckerid = holder.employeeid;
					break;
				case SignoffType.Employee:
					nextcheckerid = relid;
					break;
				case SignoffType.Team:
				case SignoffType.CostCodeOwner:
				case SignoffType.AssignmentSignOffOwner:
					nextcheckerid = 0;
					break;
				case SignoffType.LineManager:
					var clsemployees = new cEmployees(this.accountid);
					Employee reqemp = clsemployees.GetEmployeeById(claim.employeeid);
					nextcheckerid = reqemp.LineManager;
					break;
				case SignoffType.ApprovalMatrix:
					nextcheckerid = 0;
					break;
			}

			return nextcheckerid;
		}




		/// <summary>
		/// Get approver formatted into a literal, used on submit claim page.
		/// </summary>
		/// <param name="total">
		/// The claim total.
		/// </param>
		/// <param name="stage">
		/// The stage.
		/// </param>
		/// <param name="employeeId">
		/// The employee Id.
		/// </param>
		/// <param name="acccountId">
		/// The account Id.
		/// </param>
		/// <param name="subAccountId">
		/// The subaccount Id.</param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetApprover(decimal total, cStage stage, int employeeId, int acccountId, int subAccountId)
		{
			if (stage.signofftype == SignoffType.ClaimantSelectsOwnChecker || stage.signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix)
			{
				var clsemployees = new cEmployees(this.accountid);

				var subaccs = new cAccountSubAccounts(acccountId);
				var subacc = subaccs.getSubAccountById(subAccountId);
				var output = new StringBuilder();

				output.Append("<div class='sectiontitle'>Who would you like to approve your claim?</div>");
				output.Append("<div class=\"twocolumn\"><label>Approver</label><span class=\"inputs\">");
				ListItem[] emps;
				if (stage.signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix)
				{
					var approvalMatrices = new ApprovalMatrices(this.accountid);
					emps = approvalMatrices.GetEmployeesForApprover(total, stage);
				}
				else
				{
					emps = clsemployees.getEmployeesBySpendManagementElement(SpendManagementElement.CheckAndPay, this.accountid);
				}

				output.Append("<select name=\"approver\" id=\"approver\" onchange=\"SEL.Claims.ClaimViewer.ContinueAlthoughAuthoriserIsOnHoliday = false\">");
				output.Append("<option value=\"0\">[None]</option>");
				bool addingFavourites = emps.FirstOrDefault(x => x.Text == @"#ENDFAVOURITES#") != null;
				int originalApproverListStartIndex = 0;

				if (addingFavourites)
				{
					output.AppendFormat("<optgroup label=\"Recent Approvers\">");

					for (int i = 0; i < emps.Length; i++)
					{
						if (stage.signofftype == SignoffType.ClaimantSelectsOwnChecker && !subacc.SubAccountProperties.AllowEmployeeInOwnSignoffGroup && (employeeId.ToString() == emps[i].Value))
						{
							continue;
						}

						if (emps[i].Text == @"#ENDFAVOURITES#")
						{
							originalApproverListStartIndex = i + 1;
							break;
						}

						string selectedAttribute = emps[i].Selected ? " selected" : string.Empty;
						output.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", emps[i].Value, selectedAttribute, emps[i].Text);
					}

					output.AppendFormat("</optgroup>");
				}

				if (addingFavourites)
				{
					output.AppendFormat("<optgroup label=\"All Approvers\">");
				}

				for (int i = originalApproverListStartIndex; i < emps.Length; i++)
				{
					if (stage.signofftype == SignoffType.ClaimantSelectsOwnChecker && !subacc.SubAccountProperties.AllowEmployeeInOwnSignoffGroup && (employeeId.ToString() == emps[i].Value))
					{
						continue;
					}

					output.AppendFormat("<option value=\"{0}\">{1}</option>", emps[i].Value, emps[i].Text);
				}

				if (addingFavourites)
				{
					output.AppendFormat("</optgroup>");
				}
				output.Append("</select>");
				output.Append("</span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"><span id=\"compApprover\" style=\"color:red\">*</span></span><span class=\"inputs\"></span><span class=\"inputicon\"></span><span class=\"inputtooltipfield\"></span><span class=\"inputvalidatorfield\"></span></div>");


				return output.ToString();
			}

			return string.Empty;
		}


		#endregion

		public Dictionary<int, cClaimHistory> getModifiedClaimHistory(DateTime date, int employeeid)
		{
			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			Dictionary<int, cClaimHistory> lstclaimhistory = new Dictionary<int, cClaimHistory>();
			List<int> lstClaimids = new List<int>();
			SqlDataReader reader;
			int claimhistoryid, claimid, stage;

			string comment;
			DateTime datestamp;

			expdata.sqlexecute.Parameters.AddWithValue("@date", date);
			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			string strsql = "SELECT claimid from claimhistory WHERE employeeid = @employeeid AND createdon > @date;";
			using (reader = expdata.GetReader(strsql))
			{
				while (reader.Read())
				{
					claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
					if (!lstClaimids.Contains(claimid))
					{
						lstClaimids.Add(claimid);
					}
				}
				reader.Close();
			}

			foreach (int id in lstClaimids)
			{
				expdata.sqlexecute.Parameters.AddWithValue("@claimid", id);
				strsql = "SELECT claimhistoryid, claimid, datestamp, comment, stage FROM claimhistory WHERE claimid = @claimid;";
				using (reader = expdata.GetReader(strsql))
				{
					while (reader.Read())
					{
						claimhistoryid = reader.GetInt32(reader.GetOrdinal("claimhistoryid"));
						claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
						if (reader.IsDBNull(reader.GetOrdinal("datestamp")) == false)
						{
							datestamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
						}
						else
						{
							datestamp = new DateTime(1900, 01, 01);
						}
						comment = reader.GetString(reader.GetOrdinal("comment"));
						stage = reader.GetByte(reader.GetOrdinal("stage"));

						cClaimHistory claimhistory = new cClaimHistory(claimhistoryid, claimid, comment, employeeid, datestamp, stage);
						lstclaimhistory.Add(claimhistory.claimhistoryid, claimhistory);
					}
					expdata.sqlexecute.Parameters.Clear();
					reader.Close();
				}
			}
			return lstclaimhistory;
		}

		/// <summary>
		/// Get claim history
		/// </summary>
		/// <param name="claimId">Id of the claim</param>
		/// <returns></returns>
		public List<cClaimHistory> GetClaimHistory(int claimId)
		{
			DatabaseConnection expdata = new DatabaseConnection(cAccounts.getConnectionString(accountid));
			List<cClaimHistory> lstClaimHistory = new List<cClaimHistory>();
			int claimHistoryId, stage;
			string comment, refNum, enteredBy;
			DateTime dateStamp;

			var claim = getClaimById(claimId);
			var employeeId = claim.employeeid;

			expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimId);

			string sql = "select claimhistoryid, datestamp, employee, comment, stage, refnum from claimhistoryview WHERE claimid = @claimid";

			using (var reader = expdata.GetReader(sql))
			{
				while (reader.Read())
				{
					claimHistoryId = reader.GetInt32(reader.GetOrdinal("claimhistoryid"));
					if (reader.IsDBNull(reader.GetOrdinal("datestamp")) == false)
					{
						dateStamp = reader.GetDateTime(reader.GetOrdinal("datestamp"));
					}
					else
					{
						dateStamp = new DateTime(1900, 01, 01);
					}
					if (reader.IsDBNull(reader.GetOrdinal("refnum")) == false)
					{
						refNum = reader.GetString(reader.GetOrdinal("refnum"));
					}
					else
					{
						refNum = "";
					}
					if (reader.IsDBNull(reader.GetOrdinal("employee")) == false)
					{
						enteredBy = reader.GetString(reader.GetOrdinal("employee"));
					}
					else
					{
						enteredBy = "";
					}
					comment = reader.GetString(reader.GetOrdinal("comment"));
					stage = reader.GetByte(reader.GetOrdinal("stage"));

					cClaimHistory claimHistory = new cClaimHistory(claimHistoryId, claimId, comment, employeeId, dateStamp, stage, refNum, enteredBy);

					lstClaimHistory.Add(claimHistory);
				}
				expdata.sqlexecute.Parameters.Clear();
				reader.Close();
			}
			return lstClaimHistory.OrderByDescending(x => x.datestamp).ToList();
		}

		/// <summary>
		/// Get all claims in a particular stage for a given employee
		/// </summary>
		/// <param name="employeeid">Employee to retrieve claims for</param>
		/// <param name="claimtype">Type of claim to be retrieved (Current/Submitted/Previous)</param>
		/// <returns>List of cClaim records</returns>
		public int getSubmittedClaimsCount(int employeeid)
		{

			DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));

			expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);
			int count = expdata.getcount("select count(claimid) from claims where employeeid = @employeeid and submitted = 1 and paid = 0");
			expdata.sqlexecute.Parameters.Clear();
			return count;
		}

		/// <summary>
		/// Output a grid containing unallocated expense items uploaded from claimants mobile device
		/// </summary>
		/// <param name="reqClaim">Current claim class object</param>
		/// <returns>Array containing grid and js block</returns>
		public string[] generateMobileItemsGrid(cClaim reqClaim)
		{
			CurrentUser user = cMisc.GetCurrentUser();
			cGridNew clsgrid = new cGridNew(user.AccountID, user.EmployeeID, "gridMobileItems", "select mobileID, date, subcats.subcat, reasons.reason, otherdetails, total, currencyId from mobileExpenseItems");
			cFields clsfields = new cFields(user.AccountID);
			clsgrid.addFilter(clsfields.GetFieldByID(new Guid("38c4fce7-c89d-4a18-a7ef-7081e1a10aa3")), ConditionType.Equals, new object[] { reqClaim.employeeid }, null, ConditionJoiner.None);
			clsgrid.addEventColumn("viewreceipt", GlobalVariables.StaticContentLibrary + "/icons/16/plain/scroll.png", cMisc.Path + "/shared/getDocument.axd?mobileID={mobileID}", "View Receipt", "View Receipt");
			clsgrid.addEventColumn(
			  "delete",
			  cMisc.Path + "/shared/images/icons/16/plain/delete2.png",
			  "javascript:SEL.Claims.ClaimViewer.DeleteMobileExpenseItem({mobileID});",
			  "Delete",
			  "Delete mobile expense item");
			clsgrid.addEventColumn("addasnew", cMisc.Path + "/shared/images/icons/16/plain/add2.png", cMisc.Path + "/aeexpense.aspx?mobileID={mobileID}", "Add as new", string.Empty);
			clsgrid.KeyField = "mobileID";
			clsgrid.getColumnByName("mobileID").hidden = true;
			clsgrid.getColumnByName("currencyId").hidden = true;
			clsgrid.EmptyText = "You do not have any mobile items to reconcile.";
			clsgrid.CssClass = "datatbl";
			clsgrid.enablepaging = true;
			clsgrid.CurrencyColumnName = "currencyId";

			// Initialise row method implementation
			SerializableDictionary<string, object> gridInfo = new SerializableDictionary<string, object> { { "employeeid", user.EmployeeID }, { "accountid", user.AccountID }, { "gridid", clsgrid.GridID } };

			clsgrid.InitialiseRowGridInfo = gridInfo;
			clsgrid.InitialiseRow += new cGridNew.InitialiseRowEvent(clsgrid_InitialiseRow);
			clsgrid.ServiceClassForInitialiseRowEvent = "Spend_Management.cClaims";
			clsgrid.ServiceClassMethodForInitialiseRowEvent = "clsgrid_InitialiseRow";
			string[] grid = clsgrid.generateGrid();

			return grid;
		}

		/// <summary>
		/// Generates a <see cref="cGridNew"/> for outstanding mobile journeys.
		/// </summary>
		/// <param name="employeeID">
		/// The employee identifier.
		/// </param>
		/// <param name="accountID">
		/// The account identifier.
		/// </param>
		/// <returns>
		/// A fully populated <see cref="cGridNew"/>
		/// </returns>
		public string[] GenerateMobileJourneysGrid(int employeeID, int accountID, int claimID)
		{
			cGridNew clsgrid = new cGridNew(accountID, employeeID, "gridMobileJourneys", "select journeyID,journeyDate,journeyStartTime,journeyEndTime from mobileJourneys");

			cFields clsfields = new cFields(accountID);
			clsgrid.addFilter(clsfields.GetFieldByID(new Guid("15F60E56-455E-4926-A6A3-FE2F7E584302")), ConditionType.Equals, new object[] { employeeID }, null, ConditionJoiner.And);

			//Filter out active journeys, but include historic mobile journeys as active will be null       
			clsgrid.addFilter(clsfields.GetFieldByID(new Guid("087E10B3-6D76-438D-A8C2-FB486F9B0537")), ConditionType.Equals, new object[] { false }, new object[] { null }, ConditionJoiner.And);

			clsgrid.addEventColumn(
			  "delete",
			  cMisc.Path + "/shared/images/icons/16/plain/delete2.png",
			  string.Format("javascript:SEL.Claims.ClaimViewer.DeleteMobileJourney({{JourneyID}}, {0});", employeeID),
			  "Delete",
				"Delete mobile journey");

			string path = string.Format("{0}/aeexpense.aspx?claimid={1}&mobileJourneyID={{JourneyID}}", cMisc.Path, claimID);

			clsgrid.addEventColumn("addasnew", cMisc.Path + "/shared/images/icons/16/plain/add2.png", path, "Add as new", string.Empty);
			clsgrid.KeyField = "JourneyID";
			clsgrid.getColumnByName("JourneyID").hidden = true;
			clsgrid.EmptyText = "You do not have any mobile journeys to reconcile.";
			clsgrid.CssClass = "datatbl";
			clsgrid.enablepaging = true;

			var gridInfo = new SerializableDictionary<string, object> { { "employeeid", employeeID }, { "accountid", accountID }, { "gridid", clsgrid.GridID } };
			clsgrid.InitialiseRowGridInfo = gridInfo;
			string[] grid = clsgrid.generateGrid();

			return grid;
		}

		private void clsgrid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
		{
			int mobileItemID = (int)row.getCellByID("mobileID").Value;

			if (!cMobileDevices.MobileItemHasReceipt((int)gridInfo["accountid"], mobileItemID))
			{
				row.getCellByID("viewreceipt").Value = "&nbsp;";
			}
		}

		/// <summary>
		/// Calculates the entire Reference Number for a given claim ID.
		/// </summary>
		/// <param name="claimId">
		/// The claim ID to use.
		/// </param>
		/// <param name="accountId">
		/// The account ID to use.
		/// </param>
		/// <returns>
		/// The Reference number for the specified claim.
		/// </returns>
		private static string CalculateClaimReferenceNumber(int claimId, int accountId)
		{
			var alphaList = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "K", "L", "M", "N", "O", "P", "R", "S", "T", "V", "W", "X", "Y", "Z" };

			int baseNum = alphaList.Count;

			var paddedAccountId = accountId.ToString("000");

			int firstCompanyDigit = Convert.ToInt32(paddedAccountId.Substring(0, 1));

			int secondCompanyDigit = Convert.ToInt32(paddedAccountId.Substring(1, 1));

			int thirdCompanyDigit = Convert.ToInt32(paddedAccountId.Substring(2, 1));

			int firstCheckDigit = CalculateSequenceNumber(claimId, baseNum, 3);

			int secondCheckDigit = CalculateSequenceNumber(claimId, baseNum, 2);

			int thirdCheckDigit = CalculateSequenceNumber(claimId, baseNum, 1);

			int firstSequenceNumber = CalculateSequenceNumber(claimId, baseNum, 5);

			int secondSequenceNumber = CalculateSequenceNumber(claimId, baseNum, 4);

			int thirdSequenceNumber = baseNum - ((((3 * (thirdCheckDigit + firstCheckDigit + firstSequenceNumber + secondCompanyDigit)) + secondCheckDigit + secondSequenceNumber + thirdCompanyDigit + firstCompanyDigit) - 1) % baseNum) - 1;

			var refNumber = string.Format("{0}-{1}-{2}", paddedAccountId, alphaList[firstSequenceNumber] + alphaList[secondSequenceNumber] + alphaList[thirdSequenceNumber], alphaList[firstCheckDigit] + alphaList[secondCheckDigit] + alphaList[thirdCheckDigit]);

			return refNumber;
		}

		/// <summary>
		/// Calculates an individual digit within a Claim Reference Number, given a base number and a power number
		/// </summary>
		/// <param name="claimId">
		/// The claim ID the reference number is being calculated for.
		/// </param>
		/// <param name="baseNumber">
		/// The base number to work from. This is the total number of letters available in the reference number character-set.
		/// </param>
		/// <param name="powerNumber">
		/// The power number to use. This is calculated using the position of the sequence number within the reference number.
		/// </param>
		/// <returns>
		/// An individual digit within a Claim Reference Number. Note that this is NOT converted to a character.
		/// </returns>
		private static int CalculateSequenceNumber(int claimId, int baseNumber, int powerNumber)
		{
			return Convert.ToInt32(Math.Floor(claimId / Math.Pow(baseNumber, powerNumber - 1)) - (baseNumber * Math.Floor(claimId / Math.Pow(baseNumber, powerNumber))));
		}

		/// <summary>
		/// Sets the Reference Number for the specified claim ID.
		/// </summary>
		/// <param name="claimId">
		/// The claim ID to use.
		/// </param>
		/// <returns>
		/// The Reference Number which has been set for the specified claim ID.
		/// </returns>
		public string SetClaimReferenceNumber(int claimId)
		{
			var claim = this.getClaimById(claimId);

			if (claim == null)
			{
				return "-1";
			}

			if (string.IsNullOrEmpty(claim.ReferenceNumber) == false)
			{
				return claim.ReferenceNumber;
			}

			var referenceNumber = CalculateClaimReferenceNumber(claimId, this.accountid);

			if (string.IsNullOrEmpty(referenceNumber))
			{
				return "-1";
			}

			var dbconnection = new DBConnection(cAccounts.getConnectionString(this.accountid));

			const string SetRefNumberSql = "UPDATE claims_base SET ReferenceNumber = @referenceNumber WHERE claimid = @claimid;";

			dbconnection.sqlexecute.Parameters.AddWithValue("@claimid", claimId);
			dbconnection.sqlexecute.Parameters.AddWithValue("@referenceNumber", referenceNumber);

			dbconnection.ExecuteSQL(SetRefNumberSql);
			dbconnection.sqlexecute.Parameters.Clear();

			return referenceNumber;
		}

		/// <summary>
		/// Clears the Reference Number for the specified claim ID.
		/// </summary>
		/// <param name="claimId">
		/// The claim ID to use.
		/// </param>
		public void ClearClaimReferenceNumber(int claimId)
		{
			var claim = this.getClaimById(claimId);

			if (claim == null)
			{
				throw new Exception("Claim doesn't exist.");
			}

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.AddWithValue("@claimId", claimId);
				connection.ExecuteProc("ClearClaimReferenceNumber");
				connection.ClearParameters();
			}
		}

		/// <summary>
		/// Gets any claim envelope numbers for the specified claim ID from the database.
		/// </summary>
		/// <param name="claimId">
		/// The claim ID to use.
		/// </param>
		/// <returns>
		/// A list of ClaimEnvelopeInfo objects, containing the ClaimEnvelopeId and the EnvelopeNumber.
		/// </returns>
		public List<ClaimEnvelopeInfo> GetClaimEnvelopeNumbers(int claimId)
		{
			if (claimId < 1)
			{
				return new List<ClaimEnvelopeInfo>();
			}

			var data = new Envelopes();
			var claim = getClaimById(claimId);
			if (claim == null || string.IsNullOrWhiteSpace(claim.ReferenceNumber))
			{
				return new List<ClaimEnvelopeInfo>();
			}

			return data.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber)
					.Select(e => new ClaimEnvelopeInfo
					{
						ClaimEnvelopeId = e.EnvelopeId,
						EnvelopeNumber = e.EnvelopeNumber,
						ExcessCharge = e.OverpaymentCharge.HasValue ? string.Format(" - Incurred a charge of {0}", e.OverpaymentCharge.Value.ToString("C")) : string.Empty,
						ProcessedAfterMarkedLost = e.DeclaredLostInPost && e.Status > EnvelopeStatus.ConfirmedSent ? " - Has been received and processed after being declared 'Lost in Post'. " : string.Empty,
						PhysicalState = e.PhysicalState.Any() ? string.Format(" - Has defects: {0}. ", e.PhysicalState.Select(x => x.Details).Aggregate((a, b) => a + ", " + b)) : string.Empty
					})
					.ToList();
		}

		/// <summary>
		/// Gets the information for the small entry in the top of the claimviewer, that describes the envelope statuses.
		/// </summary>
		/// <param name="claimId">The id of the claim.</param>
		/// <returns>A ClaimEnvelopeSummary object.</returns>
		public ClaimEnvelopeSummary GetClaimEnvelopeSummary(int claimId)
		{
			string labelText = "Envelope Number:";

			string firstEnvelopenumber = string.Empty;

			string additionalEnvelopeText = string.Empty;

			var additionalEnvelopeList = new List<HtmlGenericControl>();

			var envelopeNumberList = this.GetClaimEnvelopeNumbers(claimId);

			var issueWithAnEnvelope = envelopeNumberList.Any(e => !string.IsNullOrWhiteSpace(e.PhysicalState) || !string.IsNullOrWhiteSpace(e.ExcessCharge) || !string.IsNullOrWhiteSpace(e.ProcessedAfterMarkedLost));

			if (envelopeNumberList.Count > 0)
			{
				var firstEnvelope = envelopeNumberList.First();
				var firstEnvelopHasDefectsOrCharge = !string.IsNullOrWhiteSpace(firstEnvelope.ExcessCharge) || !string.IsNullOrWhiteSpace(firstEnvelope.PhysicalState);
				firstEnvelopenumber = firstEnvelopHasDefectsOrCharge ? "<span class='important'>" + firstEnvelope.EnvelopeNumber + "</span>" : firstEnvelope.EnvelopeNumber;

				if (envelopeNumberList.Count > 1 || firstEnvelopHasDefectsOrCharge)
				{
					labelText = "Envelope Numbers:";

					additionalEnvelopeText = envelopeNumberList.Count == 1 ? "issues >" : string.Format("and {0} other{1}", envelopeNumberList.Count - 1, envelopeNumberList.Count > 2 ? "s" : string.Empty);
				}

				additionalEnvelopeList.AddRange(
					envelopeNumberList.Select(envelopeNumber => (!string.IsNullOrWhiteSpace(envelopeNumber.ExcessCharge) || !string.IsNullOrWhiteSpace(envelopeNumber.PhysicalState))
								? new HtmlGenericControl("span") { InnerHtml = "<span class='important'>" + envelopeNumber.EnvelopeNumber + envelopeNumber.ExcessCharge + envelopeNumber.ProcessedAfterMarkedLost + envelopeNumber.PhysicalState + "</span>" }
								: new HtmlGenericControl("span") { InnerText = envelopeNumber.EnvelopeNumber }));
			}

			return new ClaimEnvelopeSummary() { LabelText = labelText, FirstEnvelopeNumber = firstEnvelopenumber, AdditionalEnvelopesText = additionalEnvelopeText, AdditionalEnvelopeList = additionalEnvelopeList };
		}

		/// <summary>
		/// Validates the claim envelope numbers for the specified claim.
		/// </summary>
		/// <param name="envelopeNumbers">
		/// The list of Claim Envelope Numbers to validate
		/// </param>
		/// <param name="claimId">
		/// The claim ID to use.
		/// </param>
		/// <returns>
		/// ClaimEnvelopeAttachmentResults containing details of the validation.
		/// </returns>
		public ClaimEnvelopeAttachmentResults ValidateClaimEnvelopeNumbers(List<ClaimEnvelopeInfo> envelopeNumbers, int claimId)
		{
			var results = new ClaimEnvelopeAttachmentResults();

			if (claimId < 1)
			{
				results.AddStatus(ClaimEnvelopeAttachmentStatus.FailedNoClaimFound);
				return results;
			}

			if (envelopeNumbers.Count == 0)
			{
				results.AddStatus(ClaimEnvelopeAttachmentStatus.FailedNoEnvelopeNumbersSupplied);
				return results;
			}

			var data = new Envelopes();
			var claim = getClaimById(claimId);

			if (claim == null)
			{
				results.AddStatus(ClaimEnvelopeAttachmentStatus.FailedNoClaimFound);
				return results;
			}

			envelopeNumbers.ForEach(en =>
			{
				// attempt to get what should be the only envelope with this envelope number.
				var envelope = data.GetEnvelopesByEnvelopeNumber(en.EnvelopeNumber).LastOrDefault();

				// throw the error if not.
				if (envelope == null)
				{
					results.AddStatus(ClaimEnvelopeAttachmentStatus.FailedNoEnvelopeFound, en.EnvelopeNumber);
					return;
				}

				if (envelope.AccountId == null)
				{
					results.AddStatus(ClaimEnvelopeAttachmentStatus.FailedEnvelopeNotAssigned, en.EnvelopeNumber);
					return;
				}

				// ONLY VALIDATE THE ENVELOPE - pass true as last argument.
				try
				{
					data.AttachToClaim(envelope.EnvelopeId, claim, cMisc.GetCurrentUser(), true);
				}
				catch
				{
					// if we got this far then the only error that hasn't already been caught is that of envelope duplication.
					// for validating we will ignore this, as we are allowing duplicates in SetClaimEnvelopeNumbers() below.
				}
				finally
				{
					results.AddStatus(ClaimEnvelopeAttachmentStatus.Success, envelope.EnvelopeNumber);
				}
			});

			return results;
		}

		/// <summary>
		/// Sets the claim envelope numbers for the specified claim.
		/// </summary>
		/// <param name="envelopeNumbers">
		/// The list of Claim Envelope Numbers to save
		/// </param>
		/// <param name="claimId">
		/// The claim ID to use.
		/// </param>
		/// <returns>
		/// A bool indicating whether the save was successful
		/// </returns>
		public ClaimEnvelopeAttachmentResults SetClaimEnvelopeNumbers(List<ClaimEnvelopeInfo> envelopeNumbers, int claimId)
		{
			var data = new Envelopes();
			var results = new ClaimEnvelopeAttachmentResults { ClaimReferenceNumber = SetClaimReferenceNumber(claimId) };
			var claim = getClaimById(claimId);
			var user = cMisc.GetCurrentUser();

			// determine the old list of envelopes.
			var newIds = envelopeNumbers.Select(x => x.EnvelopeNumber);
			var envelopesToRemove = data.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber).ToList();
			envelopesToRemove = envelopesToRemove.Where(x => !newIds.Contains(x.EnvelopeNumber)).ToList();

			// wipe their claim reference.
			envelopesToRemove.ForEach(x => data.DetachFromClaim(x.EnvelopeId, claim, cMisc.GetCurrentUser()));

			// now link up the new envelope selection.
			envelopeNumbers.ForEach(en =>
			{
				// attempt to get what should be the only envelope with this envelope number.
				var envelope = data.GetEnvelopesByEnvelopeNumber(en.EnvelopeNumber).FirstOrDefault();

				if (envelope == null)
				{
					throw new InvalidDataException("Cannot generate a new envelope with arbitrary EnvelopeNumber.");
				}

				// if the envelope is on another claim, or assigned to another account, create a new one.
				if (envelope.AccountId != user.AccountID || (envelope.ClaimId.HasValue && envelope.ClaimId.Value != claim.claimid))
				{
					envelope = new Envelope
					{
						AccountId = claim.accountid,
						ClaimId = claim.claimid,
						ClaimReferenceNumber = claim.ReferenceNumber,
						EnvelopeId = en.ClaimEnvelopeId,
						EnvelopeNumber = en.EnvelopeNumber,
						Status = EnvelopeStatus.AttachedToClaim,
						DateAssignedToClaim = DateTime.UtcNow,
						Type = new EnvelopeType { EnvelopeTypeId = 1 },
						PhysicalState = new List<EnvelopePhysicalState>()
					};

					data.AddEnvelope(envelope, user);
				}
				else
				{
					// update the envelope as attached.
					data.AttachToClaim(envelope.EnvelopeId, claim, user);
				}

				results.AddStatus(ClaimEnvelopeAttachmentStatus.Success, envelope.EnvelopeNumber);
			});

			results.AddStatus(ClaimEnvelopeAttachmentStatus.Success);
			return results;
		}

		/// <summary>
		/// Unlinks an envelope from a claim.
		/// </summary>
		/// <param name="claimId">The Id of the claim to unlink the envelope from.</param>
		/// <param name="envelopeId">The Id of the envelope.</param>
		/// <param name="user">The user making the call.</param>
		/// <returns>The Claim Reference Number.</returns>
		public string UnlinkEnvelopeFromClaim(int claimId, int envelopeId, ICurrentUserBase user)
		{
			var claim = getClaimById(claimId);
			var data = new Envelopes();
			var envelope = data.GetEnvelopeById(envelopeId);

			if (envelope == null)
			{
				return "No envelope found.";
			}

			// just check that this envelope is on this claim before detatching.
			if (envelope.ClaimId.HasValue && envelope.ClaimId.Value == claim.claimid)
			{
				data.DetachFromClaim(envelope.EnvelopeId, claim, user);
			}

			var envelopesOnThisClaim = data.GetEnvelopesByClaimReferenceNumber(claim.ReferenceNumber);

			if (!envelopesOnThisClaim.Any())
			{
				this.ClearClaimReferenceNumber(claimId);
			}

			return claim.ReferenceNumber;
		}

		#region Mobile API methods

		/// <summary>
		/// Saves a list of envelope numbers against a claim
		/// </summary>
		/// <param name="envelopeNumbers">The envelope numbers to save</param>
		/// <param name="claimId">The ID of the claim to use</param>
		/// <param name="accountId">The current account ID</param>
		/// <returns>The reference number for the specified claim ID</returns>
		public ClaimEnvelopeAttachmentResults SaveClaimEnvelopeNumbers(List<ClaimEnvelopeInfo> envelopeNumbers, int claimId, int accountId)
		{
			ClaimEnvelopeAttachmentResults results = null;

			// need to validate all envelopes first.
			var claims = new cClaims(accountId);
			results = claims.ValidateClaimEnvelopeNumbers(envelopeNumbers, claimId);

			// actually do the operation.
			if (results.OverallResult)
			{
				claims = new cClaims(accountId);
				results = claims.SetClaimEnvelopeNumbers(envelopeNumbers, claimId);
			}

			return results;
		}

		/// <summary>
		/// Gets collection of claims awaiting approval for mobileAPI
		/// </summary>
		/// <param name="accountid">Customer Account Id</param>
		/// <param name="employeeid">Checker Employee Id to get claims for</param>
		/// <returns>Collection of claims awaiting approval by employee id provided</returns>
		public static List<SpendManagementLibrary.Mobile.Claim> GetClaimsAwaitingApproval(int accountid, int employeeid)
		{
			List<SpendManagementLibrary.Mobile.Claim> claims = new List<SpendManagementLibrary.Mobile.Claim>();
			cGroups clsgroups = new cGroups(accountid);
			cEmployees clsemployees = new cEmployees(accountid);

			DBConnection data = new DBConnection(cAccounts.getConnectionString(accountid));
			data.sqlexecute.Parameters.AddWithValue("@employeeid", employeeid);

			using (SqlDataReader reader = data.GetReader("SELECT [claimid],[claimno],[stage],[empname],[name],[datesubmitted],[status],[numreceipts],[flaggeditems],[total],[cash],[credit],[purchase],[paiditems],[checkerid], [approved],[stagecount],[employeeid],[claimtype],[basecurrency],[displayunallocate],[displaysinglesignoff] FROM [checkandpay] where (checkerid = @employeeid OR (itemcheckerid is not null and itemcheckerid = @employeeId))"))
			{
				while (reader.Read())
				{
					SpendManagementLibrary.Mobile.Claim claim = new SpendManagementLibrary.Mobile.Claim { ClaimID = reader.GetInt32(0), ClaimNumber = reader.GetInt32(1), Stage = reader.GetInt32(2), Status = (ClaimStatus)reader.GetByte(6), EmployeeName = reader.GetString(3), ClaimName = reader.GetString(4), Total = reader.GetDecimal(9), Approved = reader.GetBoolean(15), DisplayOneClickSignoff = reader.GetBoolean(21), ApprovedTotal = reader.IsDBNull(13) ? 0 : reader.GetDecimal(13), BaseCurrency = reader.IsDBNull(19) ? 0 : reader.GetInt32(19) };
					int claimEmployeeId = reader.GetInt32(reader.GetOrdinal("employeeid"));
					Employee claimemp = clsemployees.GetEmployeeById(claimEmployeeId);
					cGroup reqgroup = clsgroups.GetGroupById(claimemp.SignOffGroupID);
					cStage currentstage = reqgroup.stages.Values[claim.Stage - 1];
					claim.DisplayDeclaration = currentstage.displaydeclaration;
					claims.Add(claim);
				}
				reader.Close();
			}
			data.sqlexecute.Parameters.Clear();

			return claims;
		}


		/// <summary>
		/// Gets a collection of claims awaiting approval that are available to the checker, but unassigned.
		/// </summary>
		/// <param name="accountId">
		/// Customer Account Id
		/// </param>
		/// <param name="employeeId">
		/// Checker Employee Id to get claims for
		/// </param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="groups">
		/// An instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="employees">
		/// An instance of <see cref="cEmployees">cEmployees</see>
		/// </param>
		/// <param name="connection">
		/// An instance of <see cref="DatabaseConnection">DatabaseConnection</see>
		/// </param>
		/// <param name="subAccounts">
		/// An instance of <see cref="cAccountSubAccounts">cAccountSubAccounts</see>
		/// </param>
		/// <param name="teams">
		/// The teams.
		/// </param>
		/// <returns>
		/// Collection of claims awaiting approval by employee id provided
		/// </returns>
		public static List<ClaimBasic> GetClaimsListAwaitingApprovalUnassigned(int accountId, int employeeId, List<ClaimBasic> claims, cGroups groups, cEmployees employees, DatabaseConnection connection, cAccountSubAccounts subAccounts, cTeams teams)
		{
			string sql =
			  "SELECT claimid, claimno, stage, empname, name, datesubmitted, status, total, basecurrency, currencySymbol, employeeId FROM unallocatedClaims";
			sql +=
				" WHERE (teamemployeeid = @employeeid AND costcodeteamemployeeid IS NULL AND itemCheckerTeamEmployeeId IS NULL)";
			sql +=
				"OR (teamemployeeid IS NULL AND costcodeteamemployeeid = @employeeid AND itemcheckerid IS NULL) ";
			sql +=
				"OR (teamemployeeid IS NULL AND itemCheckerTeamEmployeeId = @employeeid AND itemcheckerid IS NULL)";

			IOwnership defaultCostCodeOwner = subAccounts.GetDefaultCostCodeOwner(accountId, subAccounts.getFirstSubAccount().SubAccountID);
			cAccountProperties properties = subAccounts.getFirstSubAccount().SubAccountProperties;

			bool isDefaultOwner = IsDefaultCostCodeOwner(employeeId, defaultCostCodeOwner, teams);

			if (isDefaultOwner)
			{
				sql += " OR (teamemployeeid IS NULL AND itemcheckerid IS NULL AND teamemployeeid IS NULL and costcodeteamemployeeid IS NULL AND itemCheckerTeamEmployeeId IS NULL)";
			}

			if (!properties.AllowTeamMemberToApproveOwnClaim)
			{
				sql += " AND unallocatedClaims.employeeid != @employeeid";
			}

			return GetUnassignedClaimBasicList(accountId, employeeId, sql, CommandType.Text, employees, groups, claims, connection, subAccounts);
		}

		/// <summary>
		/// Gets collection of claims awaiting approval for mobileAPI
		/// </summary>
		/// <param name="accountId">
		/// Customer Account Id
		/// </param>
		/// <param name="employeeId">
		/// Checker Employee Id to get claims for
		/// </param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="groups">
		/// A instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="employees">
		/// A instance of <see cref="cEmployees">CEmployees </see>
		/// </param>
		/// <param name="subAccounts">
		/// A instance of <see cref="cAccountSubAccounts">cAccountsSubAccounts</see>
		/// </param>
		/// <returns>
		/// Collection of claims awaiting approval by employee id provided
		/// </returns>
		public static List<ClaimBasic> GetClaimsListAwaitingApproval(int accountId, int employeeId, List<ClaimBasic> claims, cGroups groups, cEmployees employees, cAccountSubAccounts subAccounts)
		{
			return GetClaimBasicList(accountId, employeeId, "dbo.GetClaimsAwaitingApprovalByEmployee", claims, groups, employees, subAccounts);
		}

		/// <summary>
		/// Gets a collection of claims that are unsubmitted for the employee
		/// </summary>
		/// <param name="employeeId">Employee Id to get claims for</param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="groups">
		/// A instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="employees">
		/// A instance of <see cref="cEmployees">CEmployees </see>
		/// </param>
		/// <param name="subAccounts">
		/// A instance of <see cref="cAccountSubAccounts">cAccountsSubAccounts</see>
		/// </param>
		/// <returns>Collection of claims that are unsubmitted by employee id provided</returns>
		public List<ClaimBasic> GetUnsubmittedClaims(int employeeId, List<ClaimBasic> claims, cGroups groups, cEmployees employees, cAccountSubAccounts subAccounts)
		{
			return GetClaimBasicList(this._accountId, employeeId, "dbo.GetUnsubmittedClaimsByEmployee", claims, groups, employees, subAccounts);
		}

		/// <summary>
		/// Gets a collection of claims that are submitted for the employee
		/// </summary>
		/// <param name="employeeId">Employee Id to get claims for</param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="groups">
		/// A instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="employees">
		/// A instance of <see cref="cEmployees">CEmployees </see>
		/// </param>
		/// <param name="subAccounts">
		/// A instance of <see cref="cAccountSubAccounts">cAccountsSubAccounts</see>
		/// </param>
		/// <returns>Collection of claims that are submitted by employee id provided</returns>
		public List<ClaimBasic> GetSubmittedClaims(int employeeId, List<ClaimBasic> claims, cGroups groups, cEmployees employees, cAccountSubAccounts subAccounts)
		{
			return GetClaimBasicList(this._accountId, employeeId, "dbo.GetSubmittedClaimsByEmployee", claims, groups, employees, subAccounts);
		}

		/// <summary>
		/// Gets a collection of previous claims for the employee
		/// </summary>
		/// <param name="employeeId">Employee Id to get claims for</param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="groups">
		/// A instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="employees">
		/// A instance of <see cref="cEmployees">CEmployees </see>
		/// </param>
		/// <param name="subAccounts">
		/// A instance of <see cref="cAccountSubAccounts">cAccountsSubAccounts</see>
		/// </param>
		/// <returns>Collection of previous claims for the suppliede employeeId</returns>
		public List<ClaimBasic> GetPreviousClaims(int employeeId, List<ClaimBasic> claims, cGroups groups, cEmployees employees, cAccountSubAccounts subAccounts)
		{
			return GetClaimBasicList(this._accountId, employeeId, "dbo.GetPreviousClaimsByEmployee", claims, groups, employees, subAccounts);
		}

		/// <summary>
		/// Takes a stored procedure that returns a fixed set of data that can be populated into a <see cref="ClaimBasic">ClaimBasic</see> object
		/// </summary>
		/// <param name="accountId">The account id</param>
		/// <param name="employeeId">The employee id</param>
		/// <param name="storedProcedure">The name of the stored procedure to retrieve the ClaimBasic information (include dbo.)</param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="groups">
		/// A instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="employees">
		/// A instance of <see cref="cEmployees">CEmployees </see>
		/// </param>
		/// <param name="subAccounts">
		/// A instance of <see cref="cAccountsSubAccounts">cAccountsSubAccounts</see>
		/// </param>
		/// <returns></returns>
		private static List<ClaimBasic> GetClaimBasicList(int accountId, int employeeId, string storedProcedure, List<ClaimBasic> claims, cGroups groups, cEmployees employees, cAccountSubAccounts subAccounts)
		{

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);

				using (var reader = connection.GetReader(storedProcedure, CommandType.StoredProcedure))
				{
					claims = ProcessClaimBasicReader(accountId, reader, employees, groups, subAccounts);
				}

				connection.sqlexecute.Parameters.Clear();
			}

			return claims;
		}


		/// <summary>
		/// Takes a sql string that returns a fixed set of data that can be populated into a <see cref="ClaimBasic">ClaimBasic</see> object of unassigned claims that are awaiting approval and are available to the user
		/// </summary>
		/// <param name="accountId">
		/// The account id
		/// </param>
		/// <param name="employeeId">
		/// The employee id
		/// </param>
		/// <param name="sql">
		/// The name of the stored procedure to retrieve the ClaimBasic information (include dbo.)
		/// </param>
		/// <param name="commandType">
		/// The type of sql command to execute
		/// </param>
		/// <param name="employees">
		/// An instance of <see cref="cEmployees">Employees</see>
		/// </param>
		/// <param name="groups">
		/// An instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>
		/// <param name="connection">
		/// An instance of <see cref="DatabaseConnection">DatabaseConnection</see>
		/// </param>
		/// <param name="subAccounts">
		/// An instance of <see cref="cAccountSubAccounts">cAccountSubAccounts</see>
		/// </param>
		/// <returns>
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </returns>
		private static List<ClaimBasic> GetUnassignedClaimBasicList(int accountId, int employeeId, string sql, CommandType commandType, cEmployees employees, cGroups groups, List<ClaimBasic> claims, DatabaseConnection connection, cAccountSubAccounts subAccounts)
		{
			using (connection)
			{
				connection.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);

				using (var reader = connection.GetReader(sql, commandType))
				{
					claims = ProcessUnassignedClaimBasicReader(accountId, reader, employees, groups, claims, subAccounts);
				}

				connection.sqlexecute.Parameters.Clear();
			}

			return claims;
		}

		/// <summary>
		/// Gets the claimbasic 
		/// </summary>
		/// <param name="claimId">The claimId</param>
		/// <param name="accountId">The accountId</param>
		/// <param name="employeeId">The employeeId</param>
		/// <returns>returns a <see cref="ClaimBasic">ClaimBasic</see></returns>
		public ClaimBasic GetClaimBasicById(int claimId, int accountId, int employeeId)
		{
			var claims = new List<ClaimBasic>();
			var groups = new cGroups(accountId);
			var employees = new cEmployees(accountId);
			var subAccounts = new cAccountSubAccounts(accountId);
			ClaimBasic claim = null;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@claimId", claimId);

				using (var reader = connection.GetReader("GetClaimBasicById", CommandType.StoredProcedure))
				{
					claims = ProcessClaimBasicReader(accountId, reader, employees, groups, subAccounts);
				}

				connection.sqlexecute.Parameters.Clear();
			}

			return claims.FirstOrDefault();

		}

		/// <summary>
		/// The process unassigned claim basic reader.
		/// </summary>
		/// <param name="accountId">
		/// The account id.
		/// </param>
		/// <param name="reader">
		/// The reader.
		/// </param>
		/// <param name="employees">
		/// An instance of <see cref="cEmployees">Employees</see>
		/// </param>
		/// <param name="groups">
		/// An instance of <see cref="cGroups">cGroups</see>
		/// </param>
		/// <param name="claims">
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </param>     
		/// <param name="subAccounts">
		/// An instance of <see cref="cAccountSubAccounts">cAccountSubAccounts</see>
		/// </param>
		/// <returns>
		/// A list of <see cref="ClaimBasic">ClaimBasic</see>
		/// </returns>
		public static List<ClaimBasic> ProcessUnassignedClaimBasicReader(int accountId, IDataReader reader, cEmployees employees, cGroups groups, List<ClaimBasic> claims, cAccountSubAccounts subAccounts)
		{
			int claimantIdOrdinal = reader.GetOrdinal("employeeId");
			int employeeNameOrdinal = reader.GetOrdinal("empname");
			int claimdIdOrdinal = reader.GetOrdinal("claimId");
			int claimNumberOrdinal = reader.GetOrdinal("claimno");
			int statusOrdinal = reader.GetOrdinal("status");
			int stageOrdinal = reader.GetOrdinal("stage");
			int claimNameOrdinal = reader.GetOrdinal("name");
			int baseCurrencyIdOrdinal = reader.GetOrdinal("baseCurrency");
			int currencySymbolOrdinal = reader.GetOrdinal("currencySymbol");
			int amountPayableOrdinal = reader.GetOrdinal("total");

			while (reader.Read())
			{
				string employeeUnformattedName = reader.GetString(employeeNameOrdinal);
				var employeeName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(employeeUnformattedName);
				int claimdId = reader.GetInt32(claimdIdOrdinal);
				int claimNumber = reader.GetInt32(claimNumberOrdinal);
				var status = (ClaimStatus)reader.GetByte(statusOrdinal);
				var stage = reader.GetInt32(stageOrdinal);
				string claimName = reader.GetString(claimNameOrdinal);
				int baseCurrencyId = reader.IsDBNull(baseCurrencyIdOrdinal) ? 0 : reader.GetInt32(baseCurrencyIdOrdinal);
				string referenceNumber = null;
				string currencySymbol = reader.IsDBNull(currencySymbolOrdinal)
					? null
					: reader.GetString(currencySymbolOrdinal);

				int claimantId = reader.GetInt32(claimantIdOrdinal);

				Employee employee = employees.GetEmployeeById(claimantId);
				cGroup group = groups.GetGroupById(employee.SignOffGroupID);

				decimal amountPayable = reader.GetDecimal(amountPayableOrdinal);

				var claim = new ClaimBasic
				{
					Approved = false,
					BaseCurrency = baseCurrencyId,
					CheckerId = null,
					ClaimId = claimdId,
					ClaimName = claimName,
					ClaimNumber = claimNumber,
					EmployeeId = claimantId,
					EmployeeName = employeeName,
					Stage = stage,
					Status = status,
					ReferenceNumber = referenceNumber,
					CurrencySymbol = currencySymbol,
					Submitted = true,
					AmountPayable = amountPayable,
					DisplayOneClickSignoff = false,
					DateSubmitted = null,
					DatePaid = null
				};

				claim.DisplayDeclaration = DetermineIfDeclarationShouldBeShown(group, stage, subAccounts);

				claims.Add(claim);
			}

			reader.Close();

			return claims;
		}


		/// <summary>
		/// There's a chance that a previous claim, that belongs to a sign off group, that has been modified (i.e. had stages removed) since the claim was paid
		/// This check ensures that the is no out of range exception when getting the current stage.
		/// </summary>  
		/// <param name="group">
		/// An instance of <see cref="cGroup">cGroup</see>
		/// </param>
		/// <param name="stage">
		/// The stage Id.
		/// </param>
		/// <param name="subAccounts">
		/// An instance of <see cref="cAccountSubAccounts">cAccountSubAccounts</see>
		/// </param>
		/// <returns>
		/// Whether the declaration message shoould be shown<see cref="bool"/>.
		/// </returns>
		private static bool DetermineIfDeclarationShouldBeShown(cGroup group, int stage, cAccountSubAccounts subAccounts)
		{
			bool displayDeclaration = false;

			if (@group != null && stage > 0 && stage <= @group.stages.Count)
			{
				cStage currentstage = @group.stages.Values[stage - 1];
				displayDeclaration = currentstage.displaydeclaration;
			}
			else
			{
				displayDeclaration = subAccounts.getFirstSubAccount().SubAccountProperties.ClaimantDeclaration;
			}

			return displayDeclaration;
		}

		/// <summary>
		/// The process claim basic reader.
		/// </summary>
		/// <param name="accountId">
		/// The account id.
		/// </param>
		/// <param name="reader">
		/// The reader.
		/// </param>
		/// <param name="employees">
		/// The employees.
		/// </param>
		/// <param name="groups">
		/// The groups.
		/// </param>
		/// <param name="subAccounts">
		/// The sub accounts.
		/// </param>
		/// <returns>
		/// The <see cref="List"/>.
		/// </returns>
		private static List<ClaimBasic> ProcessClaimBasicReader(int accountId, IDataReader reader, cEmployees employees, cGroups groups, cAccountSubAccounts subAccounts)
		{
			var claims = new List<ClaimBasic>();

			int claimantIdOrdinal = reader.GetOrdinal("employeeId");
			int employeeNameOrdinal = reader.GetOrdinal("employeeName");
			int claimdIdOrdinal = reader.GetOrdinal("claimId");
			int claimNumberOrdinal = reader.GetOrdinal("claimNumber");
			int descriptionOrdinal = reader.GetOrdinal("description");
			int statusOrdinal = reader.GetOrdinal("status");
			int checkerIdOrdinal = reader.GetOrdinal("checkerId");
			int itemCheckerIdOrdinal = reader.GetOrdinal("itemCheckerId");
			int stageOrdinal = reader.GetOrdinal("stage");
			int claimNameOrdinal = reader.GetOrdinal("claimName");
			int baseCurrencyIdOrdinal = reader.GetOrdinal("baseCurrencyId");
			int referenceNumberOrdinal = reader.GetOrdinal("referenceNumber");
			int currencySymbolOrdinal = reader.GetOrdinal("currencySymbol");
			int submittedOrdinal = reader.GetOrdinal("submitted");
			int amountPayableOrdinal = reader.GetOrdinal("total");
			int displayOneClickSignOffOrdinal = reader.GetOrdinal("displaysinglesignoff");
			int numberOfItemsOrdinal = reader.GetOrdinal("numberOfItems");
			int dateSubmittedOrdinal = reader.GetOrdinal("dateSubmitted");
			int datePaidOrdinal = reader.GetOrdinal("datePaid");

			// canBeUnassigned is not applicable in some cases
			bool hasCanBeAssignedColumn = HasColumn(reader, "canBeUnassigned");
			int canBeUnassignedOrdinal = hasCanBeAssignedColumn ? reader.GetOrdinal("canBeUnassigned") : -1;

			while (reader.Read())
			{
				string employeeUnformattedName = reader.GetString(employeeNameOrdinal);
				var employeeName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(employeeUnformattedName);

				int claimdId = reader.GetInt32(claimdIdOrdinal);
				int claimNumber = reader.GetInt32(claimNumberOrdinal);
				string Description = reader.GetString(descriptionOrdinal);
				var status = (ClaimStatus)reader.GetByte(statusOrdinal);

				int? checkerId = null;
				if (!reader.IsDBNull(checkerIdOrdinal))
				{
					checkerId = reader.GetInt32(checkerIdOrdinal);
				}

				int? itemCheckerId = null;

				if (!reader.IsDBNull(itemCheckerIdOrdinal))
				{
					itemCheckerId = reader.GetInt32(itemCheckerIdOrdinal);
				}

				var stage = reader.GetInt32(stageOrdinal);
				string claimName = reader.GetString(claimNameOrdinal);
				int baseCurrencyId = reader.IsDBNull(baseCurrencyIdOrdinal) ? 0 : reader.GetInt32(baseCurrencyIdOrdinal);

				string referenceNumber = null;

				if (!reader.IsDBNull(referenceNumberOrdinal))
				{
					referenceNumber = reader.GetString(referenceNumberOrdinal);
				}

				string currencySymbol = reader.IsDBNull(currencySymbolOrdinal)
					? null
					: reader.GetString(currencySymbolOrdinal);

				bool submitted = reader.GetBoolean(submittedOrdinal);

				int claimantId = reader.GetInt32(claimantIdOrdinal);

				Employee employee = employees.GetEmployeeById(claimantId);
				cGroup group = groups.GetGroupById(employee.SignOffGroupID);

				decimal amountPayable = reader.GetDecimal(amountPayableOrdinal);

				bool displayOneClickSignOff = false;

				if (!reader.IsDBNull(displayOneClickSignOffOrdinal))
				{
					displayOneClickSignOff = reader.GetBoolean(displayOneClickSignOffOrdinal);
				}

				int numberOfItems = reader.GetInt32(numberOfItemsOrdinal);

				DateTime? dateSubmitted = null;

				if (!reader.IsDBNull(dateSubmittedOrdinal))
				{
					dateSubmitted = reader.GetDateTime(dateSubmittedOrdinal);
				}

				DateTime? datePaid = null;
				if (!reader.IsDBNull(datePaidOrdinal))
				{
					datePaid = reader.GetDateTime(datePaidOrdinal);
				}

				bool canBeUnassigned = false;

				if (canBeUnassignedOrdinal > -1 && !reader.IsDBNull(canBeUnassignedOrdinal))
				{
					canBeUnassigned = reader.GetBoolean(canBeUnassignedOrdinal);
				}

				var claim = new ClaimBasic
				{
					Approved = false,
					BaseCurrency = baseCurrencyId,
					CheckerId = checkerId,
					ItemCheckerId = itemCheckerId,
					ClaimId = claimdId,
					ClaimName = claimName,
					Description = Description,
					ClaimNumber = claimNumber,
					EmployeeId = claimantId,
					EmployeeName = employeeName,
					Stage = stage,
					Status = status,
					ReferenceNumber = referenceNumber,
					CurrencySymbol = currencySymbol,
					Submitted = submitted,
					AmountPayable = amountPayable,
					DisplayOneClickSignoff = displayOneClickSignOff,
					NumberOfItems = numberOfItems,
					DateSubmitted = dateSubmitted,
					DatePaid = datePaid,
					CanBeUnassigned = canBeUnassigned
				};

				claim.DisplayDeclaration = DetermineIfDeclarationShouldBeShown(group, stage, subAccounts);

				claims.Add(claim);
			}

			reader.Close();

			return claims;
		}

		#endregion


        public string[] GetPartSubmit(cClaim reqclaim, byte viewfilter)
        {
            var user = cMisc.GetCurrentUser();
            var misc = new cMisc(user.AccountID);

            var generalOptions = this._generalOptionsFactory.Value[user.CurrentSubAccountId].WithClaim();

            var javascript = new StringBuilder();
            javascript.Append("<script language=\"javascript\" type=\"text/javascript\">");
            string partsubmit = string.Empty;

            if (generalOptions.Claim.PartSubmit)
            {
                javascript.Append("var partsubmittal = true;");
                if (reqclaim.containsCashAndCredit())
                {
                    ////contains cash and credit so which do they want to submit
                    bool onlycashcredit = generalOptions.Claim.OnlyCashCredit;
                    partsubmit = "<div class=\"inputpanel\">";
                    partsubmit += "<div class=\"inputpaneltitle\">Which items would you like to submit?</div>";
                    partsubmit += "<table>";
                    partsubmit += "<tr><td class=\"labeltd\">Cash Items</td><td class=\"inputtd\">";
                    if (onlycashcredit)
                    {
                        partsubmit += "<input name=\"tosubmit\" id=\"tosubmitcash\" onclick=\"setHdnFieldVal(1)\" type=\"radio\" value=\"1\"";
                        if (viewfilter == 1)
                        {
                            partsubmit += " checked";
                        }
                        partsubmit += ">";

					}
					else
					{
						partsubmit += "<input name=\"tosubmitcash\" id=\"tosubmitcash\" type=\"checkbox\" value=\"1\"";
						if (viewfilter == 0 || viewfilter == 1)
						{
							partsubmit += " checked";
						}
						partsubmit += ">";
					}
					partsubmit += "</td></tr>";
					partsubmit += "<tr><td class=\"labeltd\">Credit Card Items</td><td class=\"inputtd\">";
					if (onlycashcredit)
					{
						partsubmit += "<input name=\"tosubmit\" id=\"tosubmitcredit\" onclick=\"setHdnFieldVal(2)\" type=\"radio\" value=\"2\"";
						if (viewfilter == 2)
						{
							partsubmit += " checked";
						}
						partsubmit += ">";

					}
					else
					{
						partsubmit += "<input name=\"tosubmitcredit\" id=\"tosubmitcredit\" type=\"checkbox\" value=\"2\"";
						if (viewfilter == 0 || viewfilter == 2)
						{
							partsubmit += " checked";
						}
						partsubmit += ">";
					}
					partsubmit += "</td></tr>";
					partsubmit += "<tr><td class=\"labeltd\">Purchase Card Items</td><td class=\"inputtd\">";
					if (onlycashcredit)
					{
						partsubmit += "<input name=\"tosubmit\" id=\"tosubmitpurchase\" onclick=\"setHdnFieldVal(3)\" type=\"radio\" value=\"3\"";
						if (viewfilter == 3)
						{
							partsubmit += " checked";
						}
						partsubmit += ">";

					}
					else
					{
						partsubmit += "<input name=\"tosubmitpurchase\" id=\"tosubmitpurchase\" type=\"checkbox\" value=\"3\"";
						if (viewfilter == 0 || viewfilter == 3)
						{
							partsubmit += " checked";
						}
						partsubmit += ">";
					}
					partsubmit += "</td></tr>";
					partsubmit += "</table>";
					partsubmit += "</div>";

					if (onlycashcredit)
					{
						javascript.Append("var cashcredit = true;");
					}
					else
					{
						javascript.Append("var cashcredit = false;");
					}
				}
				javascript.Append("var containscashcredit = " + reqclaim.containsCashAndCredit().ToString().ToLower() + ";");
			}
			else
			{
				javascript.Append("var partsubmittal = false;");
			}

			javascript.Append("</script>");
			return new string[] { partsubmit, javascript.ToString() };
		}

		public int insertDefaultClaim(int employeeid)
		{
			cEmployees clsemployees = new cEmployees(accountid);
			Employee reqemp = clsemployees.GetEmployeeById(employeeid);

			string name = reqemp.Forename[0] + reqemp.Surname + (reqemp.CurrentClaimNumber);
			string description = string.Empty;
			var claimSubmission = new ClaimSubmission(cMisc.GetCurrentUser());
			int claimid = claimSubmission.addClaim(employeeid, name, description, new SortedList<int, object>());

			return claimid;
		}

		/// <summary>
		/// Unsubmit a claim as an approver
		/// </summary>
		/// <param name="approverEmployeeId">The employee id of the approver unsubmitting the claim</param>
		/// <param name="delegateId">The delegateid from currentuser</param>
		/// <param name="claimId">The id of the claim being unsubmitted</param>
		/// <param name="reason">The reason given for unsubmission</param>
		/// <returns>A returncode indicating success (0) or failure (negative)</returns>
		public int UnsubmitClaimAsApprover(int approverEmployeeId, int? delegateId, int claimId, string reason, ICurrentUser user)
		{
			cClaim claim = this.getClaimById(claimId);
			var expdata = new DBConnection(cAccounts.getConnectionString(accountid));
			if (claim.PayBeforeValidate)
			{
				return -20;
			}
			// check here whether the claim has passed a SEL stage in the SEL stage
			if (HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELScanAttach) || HasClaimPreviouslyPassedSelStage(claim, SignoffType.SELValidation))
			{
				return -10;
			}

			expdata.sqlexecute.Parameters.AddWithValue("@claimId", claim.claimid);
			expdata.sqlexecute.Parameters.AddWithValue("@approverId", approverEmployeeId);
			expdata.sqlexecute.Parameters.AddWithValue("@historyEmployeeId", delegateId.HasValue ? delegateId.Value : approverEmployeeId);
			expdata.sqlexecute.Parameters.AddWithValue("@modifiedOn", DateTime.UtcNow);
			expdata.sqlexecute.Parameters.Add("@reason", SqlDbType.NVarChar, 4000);
			expdata.sqlexecute.Parameters["@reason"].Value = reason;
			expdata.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
			expdata.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;
			expdata.ExecuteProc("UnsubmitClaimAsApprover");

			int returnValue = (int)expdata.sqlexecute.Parameters["@returnValue"].Value;
			expdata.sqlexecute.Parameters.Clear();

			if (returnValue == 0)
			{
				claim.changeStatus(ClaimStatus.None);
				this.UnSubmitclaim(claim, true, approverEmployeeId, delegateId);

				new Cache().Delete(accountid, cFloats.CacheArea, "0");
			}

			// Once claim is rejected then approvers' balance for particular employee for particular month should be updated.
			// In order to achieve that we are saving amounts one has approved not remaining balance.
			// So deleting all the records belonging to that approver for that claim does the trick.
			expdata.sqlexecute.Parameters.AddWithValue("@ClaimId", claim.claimid);
			expdata.ExecuteProc("dbo.DeleteClaimApproverDetailsByClaimId");
			expdata.sqlexecute.Parameters.Clear();
			return returnValue;
		}

		/// <summary>
		/// Is the claim 'un-submittable'
		/// </summary>
		/// <param name="approverId">The current approver ID</param>
		/// <param name="claimId">The current claim ID</param>
		/// <returns>
		/// The reason the claim cannot be un-submitted.
		/// </returns>
		public ClaimUnsubmittableReason IsClaimUnsubmittable(int approverId, int claimId)
		{
			var returnValue = ClaimUnsubmittableReason.Unsubmitable;
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@claimId", claimId);
				connection.sqlexecute.Parameters.AddWithValue("@approverId", approverId);
				connection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
				connection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;
				connection.ExecuteProc("dbo.IsClaimUnsubmittable");
				returnValue = (ClaimUnsubmittableReason)connection.sqlexecute.Parameters["@returnValue"].Value;
				connection.sqlexecute.Parameters.Clear();
			}
			SortedList<int, cExpenseItem> expenseItems = this.getExpenseItemsFromDB(claimId);
			var claimIsProgress = expenseItems.Values.Any(expenseItem => expenseItem.IsItemEscalated == true);
			if (claimIsProgress)
			{
				returnValue = ClaimUnsubmittableReason.EscalatedProcess;
			}
			if (returnValue == ClaimUnsubmittableReason.Unsubmitable)
			{
				var itemsAllowed = expenseItems.Values.Any(expenseItem => expenseItem.tempallow);
				return this.CanClaimBeUnSubmitted(
					this.getClaimById(claimId),
					expenseItems,
					itemsAllowed);
			}

			return returnValue;
		}


		/// <summary>
		/// Gets the claim id for a claim with the supplied claim name and claim number.
		/// </summary>
		/// <param name="claimName">The claim name - name field.</param>
		/// <param name="claimNumber">The claim number - claimno field.</param>
		/// <returns>The claim id or 0 is unfound.</returns>
		public int GetClaimIdFromNameAndNumber(string claimName, int claimNumber)
		{
			int id;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@claimName", claimName);
				connection.sqlexecute.Parameters.AddWithValue("@claimNumber", claimNumber);
				connection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int);
				connection.sqlexecute.Parameters["@returnValue"].Direction = ParameterDirection.ReturnValue;
				connection.ExecuteProc("GetClaimIdFromNameAndNumber");

				id = (int)connection.sqlexecute.Parameters["@returnValue"].Value;
				connection.sqlexecute.Parameters.Clear();
			}

			return id;
		}

		/// <summary>
		/// Gets a claim by it's reference number (CRN).
		/// </summary>
		/// <param name="referenceNumber">The CRN or ClaimReferenceNumber.</param>
		/// <returns>The claim id or 0 is unfound.</returns>
		public cClaim GetClaimByReferenceNumber(string referenceNumber)
		{
			cClaim claim;

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.AddWithValue("@referenceNumber", referenceNumber);
				claim = ExtractClaimFromSql("GetClaimByReferenceNumber", connection);
				connection.ClearParameters();
			}

			return claim;
		}

		/// <summary>
		/// Adds to the history of selected approvers for the claimant.
		/// </summary>
		/// <param name="user">
		/// The current system user.
		/// </param>
		/// <param name="approverId">
		/// The approver id.
		/// </param>
		public void AddClaimantSelectedApprover(ICurrentUser user, int approverId)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@employeeId", user.EmployeeID);
				connection.sqlexecute.Parameters.AddWithValue("@approverId", approverId);
				connection.ExecuteProc("AddClaimantApproverSelection");
				connection.sqlexecute.Parameters.Clear();
			}
		}

		/// <summary>
		/// Determines whether a claim has passed a SEL stage. The last 2 arguments passed modify the default behaviour.
		/// If left as default, then any stage from the previouse stage backwards will be checked, and if that stage matches the one supplied, true will be returned.
		/// Passing true for <see cref="onlyCheckCurrentStage"/> forces the current stage only to be checked for a match against <see cref="whichStage"/>. No previous stages are checked.
		/// Passing true for <see cref="includeCurrentStage"/> will include the current stage from the list to check. This would mean that if the claim's current stage matches whichStage, true would be returned.
		/// </summary>
		/// <param name="claim">The claim</param>
		/// <param name="whichStage">The SEL stage to look for</param>
		/// <param name="allClaimStageTypes">A list of stage types for this claim. Will be created if not supplied.</param>
		/// <param name="onlyCheckCurrentStage">Pass true to ignore previous stages and only check the claim's current stage.</param>
		/// <param name="includeCurrentStage">Whether to include the current stage in the list of stages to check. Only used if <see cref="onlyCheckCurrentStage"/> is false.</param>
		/// <returns>A Bool indicating the above.</returns>
		public bool HasClaimPreviouslyPassedSelStage(cClaim claim, SignoffType whichStage, List<SignoffType> allClaimStageTypes = null, bool onlyCheckCurrentStage = false, bool includeCurrentStage = false)
		{
			if (whichStage != SignoffType.SELScanAttach && whichStage != SignoffType.SELValidation)
			{
				throw new InvalidOperationException("Please supply a SEL stage.");
			}

			if (allClaimStageTypes == null)
			{
				allClaimStageTypes = GetSignoffStagesAsTypes(claim);
			}

			if (claim.stage == 0)
			{
				return false;
			}

			// if override in arguments, check current stage only
			if (onlyCheckCurrentStage)
			{
				return allClaimStageTypes[claim.stage - 1] == whichStage;
			}

			// check previous stages
			var currentStage = claim.stage - 2;

			// if the current stage sohuld be included.
			if (includeCurrentStage)
			{
				currentStage++;
			}

			while (currentStage >= 0)
			{
				if (allClaimStageTypes[currentStage] == whichStage)
				{
					return true;
				}

				currentStage--;
			}

			return false;
		}

		/// <summary>
		/// Runs a piece of SQL against an IDBConnection, converting the result into a cClaim.
		/// </summary>
		/// <param name="sql">The SQL to run.</param>
		/// <param name="connection">The connection to run the SQL on.</param>
		/// <returns>A cClaim.</returns>
		private cClaim ExtractClaimFromSql(string sql, IDBConnection connection)
		{
			cClaim claim = null;

			using (var reader = connection.GetReader(sql, CommandType.StoredProcedure))
			{
				int claimidOrd = reader.GetOrdinal("claimid");
				int claimnoOrd = reader.GetOrdinal("claimno");
				int employeeidOrd = reader.GetOrdinal("employeeid");
				int approvedOrd = reader.GetOrdinal("approved");
				int paidOrd = reader.GetOrdinal("paid");
				int datesubmittedOrd = reader.GetOrdinal("datesubmitted");
				int datepaidOrd = reader.GetOrdinal("datepaid");
				int descriptionOrd = reader.GetOrdinal("description");
				int statusOrd = reader.GetOrdinal("status");
				int teamidOrd = reader.GetOrdinal("teamid");
				int checkeridOrd = reader.GetOrdinal("checkerid");
				int stageOrd = reader.GetOrdinal("stage");
				int submittedOrd = reader.GetOrdinal("submitted");
				int splitapprovalstageOrd = reader.GetOrdinal("splitApprovalStage");
				int nameOrd = reader.GetOrdinal("name");
				int currencyidOrd = reader.GetOrdinal("currencyid");
				int createdOnOrd = reader.GetOrdinal("CreatedOn");
				int createdByOrd = reader.GetOrdinal("CreatedBy");
				int modifiedOnOrd = reader.GetOrdinal("ModifiedOn");
				int modifiedByOrd = reader.GetOrdinal("ModifiedBy");
				int referenceNumberOrd = reader.GetOrdinal("ReferenceNumber");
				int hasHistoryOrd = reader.GetOrdinal("hasHistory");
				int currentApproverOrd = reader.GetOrdinal("currentApprover");
				int totalStageCountOrd = reader.GetOrdinal("totalStageCount");
				int hasReturnedItemsOrd = reader.GetOrdinal("hasReturnedItems");
				int hasCashItemsOrd = reader.GetOrdinal("hasCashItems");
				int hasCreditCardItemsOrd = reader.GetOrdinal("hasCreditCardItems");
				int hasPurchaseCardItemsOrd = reader.GetOrdinal("hasPurchaseCardItems");
				int hasFlaggedItemsOrd = reader.GetOrdinal("hasFlaggedItems");
				int numberOfItemsOrd = reader.GetOrdinal("numberOfItems");
				int startDateOrd = reader.GetOrdinal("startDate");
				int endDateOrd = reader.GetOrdinal("endDate");
				int totalOrd = reader.GetOrdinal("total");
				int amountPayableOrd = reader.GetOrdinal("amountPayable");
				int numberOfReceiptsOrd = reader.GetOrdinal("numberOfReceipts");
				int numberOfUnapprovedItemsOrd = reader.GetOrdinal("numberOfUnapprovedItems");
				int creditCardTotalOrd = reader.GetOrdinal("creditCardTotal");
				int purchaseCardTotalOrd = reader.GetOrdinal("purchaseCardTotal");
				int payBeforeValidateOrd = reader.GetOrdinal("PayBeforeValidate");

				if (reader.Read())
				{
					int claimid = reader.GetInt32(claimidOrd);
					int claimno = reader.GetInt32(claimnoOrd);
					int empid = reader.GetInt32(employeeidOrd);
					bool approved = reader.GetBoolean(approvedOrd);
					bool paid = reader.GetBoolean(paidOrd);
					DateTime datesubmitted = reader.IsDBNull(datesubmittedOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(datesubmittedOrd);
					DateTime datepaid = reader.IsDBNull(datepaidOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(datepaidOrd);
					string description = reader.IsDBNull(descriptionOrd) ? string.Empty : reader.GetString(descriptionOrd);
					var status = (ClaimStatus)reader.GetByte(statusOrd);
					int teamid = reader.IsDBNull(teamidOrd) ? 0 : reader.GetInt32(teamidOrd);
					int checkerid = reader.IsDBNull(checkeridOrd) ? 0 : reader.GetInt32(checkeridOrd);
					int stage = reader.GetInt32(stageOrd);
					bool submitted = reader.GetBoolean(submittedOrd);
					string name = reader.GetString(nameOrd);
					bool splitapprovalstage = reader.GetBoolean(splitapprovalstageOrd);
					int currencyid = reader.IsDBNull(currencyidOrd) ? 0 : reader.GetInt32(currencyidOrd);
					DateTime createdon = reader.IsDBNull(createdOnOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(createdOnOrd);
					int createdby = reader.IsDBNull(createdByOrd) ? 0 : reader.GetInt32(createdByOrd);
					DateTime modifiedon = reader.IsDBNull(modifiedOnOrd) ? new DateTime(1900, 01, 01) : reader.GetDateTime(modifiedOnOrd);
					int modifiedby = reader.IsDBNull(modifiedByOrd) ? 0 : reader.GetInt32(modifiedByOrd);
					bool hasHistory = reader.GetBoolean(hasHistoryOrd);
					string currentApprover = reader.IsDBNull(currentApproverOrd) ? string.Empty : reader.GetString(currentApproverOrd);
					int totalStageCount = reader.GetInt32(totalStageCountOrd);
					var referenceNumber = reader.IsDBNull(referenceNumberOrd) ? null : reader.GetString(referenceNumberOrd);
					bool hasReturnedItems = reader.GetBoolean(hasReturnedItemsOrd);
					bool hasCashItems = reader.GetBoolean(hasCashItemsOrd);
					bool hasCreditCardItems = reader.GetBoolean(hasCreditCardItemsOrd);
					bool hasPurchaseCardItems = reader.GetBoolean(hasPurchaseCardItemsOrd);
					bool hasflaggeditems = reader.GetBoolean(hasFlaggedItemsOrd);
					int numberofitems = reader.GetInt32(numberOfItemsOrd);
					DateTime? startDate = reader.IsDBNull(startDateOrd) ? (DateTime?)null : reader.GetDateTime(startDateOrd);
					DateTime? endDate = reader.IsDBNull(endDateOrd) ? (DateTime?)null : reader.GetDateTime(endDateOrd);
					decimal total = reader.IsDBNull(totalOrd) ? 0 : reader.GetDecimal(totalOrd);
					decimal amountpayable = reader.IsDBNull(amountPayableOrd) ? 0 : reader.GetDecimal(amountPayableOrd);
					int numberofreceipts = reader.GetInt32(numberOfReceiptsOrd);
					int numberofunapproveditems = reader.GetInt32(numberOfUnapprovedItemsOrd);
					decimal creditcardtotal = reader.IsDBNull(creditCardTotalOrd) ? 0 : reader.GetDecimal(creditCardTotalOrd);
					decimal purchasecardtotal = reader.IsDBNull(purchaseCardTotalOrd) ? 0 : reader.GetDecimal(purchaseCardTotalOrd);
					var payBeforeValidate = !reader.IsDBNull(payBeforeValidateOrd) && reader.GetBoolean(payBeforeValidateOrd);
					claim = new cClaim(
						this.accountid,
						claimid,
						claimno,
						empid,
						name,
						description,
						stage,
						approved,
						paid,
						datesubmitted,
						datepaid,
						status,
						teamid,
						checkerid,
						submitted,
						splitapprovalstage,
						createdon,
						createdby,
						modifiedon,
						modifiedby,
						currencyid,
						referenceNumber,
						hasHistory,
						currentApprover,
						totalStageCount,
						hasReturnedItems,
						hasCashItems,
						hasCreditCardItems,
						hasPurchaseCardItems,
						hasflaggeditems,
						numberofitems,
						startDate,
						endDate,
						total,
						amountpayable,
						numberofreceipts,
						numberofunapproveditems,
						creditcardtotal,
						purchasecardtotal,
						payBeforeValidate);
				}
				reader.Close();
			}

			return claim;
		}

		public void ResetClaimToValidationStage(cClaim reqclaim, List<SignoffType> claimStages)
		{
			var stage = reqclaim.stage;
			for (int i = 0; i < claimStages.Count; i++)
			{
				if (claimStages[i] == SignoffType.SELValidation)
				{
					stage = i;
					break;
				}
			}

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				var sql = "UPDATE claims_base SET stage = @stage WHERE claimid = @claimid";
				connection.sqlexecute.Parameters.AddWithValue("@stage", stage);
				connection.sqlexecute.Parameters.AddWithValue("@claimid", reqclaim.claimid);
				connection.ExecuteSQL(sql);
			}
		}


		private ICurrentUser ClaimUser(int employeeId)
		{
			return cMisc.GetCurrentUser(string.Format("{0},{1}", this._accountId, employeeId));
		}

		/// <summary>
		/// Get claim submission details.
		/// </summary>
		/// <param name="user">
		/// Current user.
		/// </param>
		/// <param name="claimId">
		/// Claim id to get the submission details for.
		/// </param>
		/// <returns>
		/// The <see cref="ClaimSubmissionApi"/> ClaimSubmission, containing claim submission details.
		/// </returns>
		public ClaimSubmissionApi GetClaimSubmissionDetailsForApi(ICurrentUser user, int claimId)
		{
			var currentUserSubAccounts = new cAccountSubAccounts(user.AccountID);
			var subAccountProperties = currentUserSubAccounts.getFirstSubAccount().SubAccountProperties;
			var requestedClaim = this.getClaimById(claimId);
			if (requestedClaim == null)
			{
				return null;
			}
			// Claim and description
			var claimSubmissionDetails = new ClaimSubmissionApi { ClaimName = requestedClaim.name };
			string description;
			if (requestedClaim.description == string.Empty)
			{
				var startdate = string.Empty;
				var enddate = string.Empty;
				if (requestedClaim.StartDate != null)
				{
					startdate = requestedClaim.StartDate.Value.ToShortDateString();
				}

				if (requestedClaim.EndDate != null)
				{
					enddate = requestedClaim.EndDate.Value.ToShortDateString();
				}

				description = string.Format("Expense Claim {0}: {1} - {2}", requestedClaim.claimno, startdate, enddate);
			}
			else
			{
				description = requestedClaim.description;
			}
			claimSubmissionDetails.ClaimDescription = description;

			// Credit card
			claimSubmissionDetails.PartSubmittal = subAccountProperties.PartSubmit && requestedClaim.containsCashAndCredit();
			if (claimSubmissionDetails.PartSubmittal)
			{
				claimSubmissionDetails.PartSubmittalFieldType = subAccountProperties.OnlyCashCredit
																	? PartSubmittalFieldType.Radio
																	: PartSubmittalFieldType.Checkbox;
				claimSubmissionDetails.PartSubmittalItems.AddRange(new List<string>() { "Cash Items", "Credit Card Items", "Purchase Card Items" });
			}

			// Declaration
			if (subAccountProperties.ClaimantDeclaration)
			{
				claimSubmissionDetails.Declaration = subAccountProperties.DeclarationMsg.Replace("\r\n", string.Empty);
			}

			// FlagMessage
			if (subAccountProperties.FlagMessage != string.Empty && requestedClaim.HasFlaggedItems)
			{
				claimSubmissionDetails.FlagMessage = subAccountProperties.FlagMessage.Replace("\r\n", string.Empty);
			}
			else
			{
				claimSubmissionDetails.FlagMessage = string.Empty;
			}

			// Odometer Readings
			var employees = new cEmployees(user.AccountID);
			if (subAccountProperties.RecordOdometer && subAccountProperties.EnterOdometerOnSubmit)
			{
				var employeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);

				var cars = employeeCars.GetCarArray(true);
				claimSubmissionDetails.OdometerRequired = cars.GetLength(0) != 0;
				if (claimSubmissionDetails.OdometerRequired)
				{
					var startDate = DateTime.Today;
					var earliestDate = DateTime.Today;
					claimSubmissionDetails.ClaimIncludesFuelCardMileage = this.IncludesFuelCardMileage(claimId);
					var odometerReadings = new List<OdometerReading>();
					foreach (var car in cars)
					{
						var odometerReading = new OdometerReading();
						odometerReading.CarID = car.carid;

						var reading = car.getLastOdometerReading();
						int oldReading;
						if (reading == null)
						{
							oldReading = 0;
							startDate = DateTime.Now;
						}
						else
						{
							oldReading = reading.newreading;
							startDate = reading.datestamp;
						}
						odometerReading.LastReadingDate = startDate.ToShortDateString();
						odometerReading.LastReading = oldReading;
						odometerReading.CarMakeModel = car.make + " " + car.model;
						odometerReading.CarRegistration = car.registration.ToUpper();
						odometerReading.VehicleType = (CarTypes.VehicleType)car.VehicleTypeID;
						odometerReadings.Add(odometerReading);

						if (startDate < earliestDate)
						{
							earliestDate = startDate;
						}
					}
					claimSubmissionDetails.OdometerReadings = odometerReadings;
					claimSubmissionDetails.ShowBusinessMileage = employees.getBusinessMiles(
						requestedClaim.employeeid,
						earliestDate,
						DateTime.Today) == 0;
				}
			}

			// Expedite info for any relating claim reference and envelope numbers            
			if (requestedClaim.ReferenceNumber != null)
			{
				claimSubmissionDetails.ClaimReferenceNumber = requestedClaim.ReferenceNumber;
				claimSubmissionDetails.ClaimEnvelopeInfo = this.GetClaimEnvelopeNumbers(claimId);
			}

			// Approvers list
			var requestedEmployee = employees.GetEmployeeById(user.EmployeeID);
			var groups = new cGroups(user.AccountID);
			var requestedGroup = groups.GetGroupById(requestedEmployee.SignOffGroupID);
			var listOfStages = groups.sortStages(requestedGroup);
			var stage = (cStage)listOfStages.GetByIndex(0);
			if (stage.signofftype != SignoffType.ClaimantSelectsOwnChecker
				&& stage.signofftype != SignoffType.DeterminedByClaimantFromApprovalMatrix)
			{
				return claimSubmissionDetails;
			}
			ListItem[] employeeList;
			if (stage.signofftype == SignoffType.DeterminedByClaimantFromApprovalMatrix)
			{
				var approvalMatrices = new ApprovalMatrices(this.accountid);
				employeeList = approvalMatrices.GetEmployeesForApprover(requestedClaim.Total, stage);
			}
			else
			{
				employeeList = employees.getEmployeesBySpendManagementElement(SpendManagementElement.CheckAndPay, this.accountid);
			}
			var addingFavourites = employeeList.FirstOrDefault(x => x.Text == @"#ENDFAVOURITES#") != null;
			var originalApproverListStartIndex = 0;
			var recentApprover = new SortedList<string, string>();
			var allApprover = new SortedList<string, string>();

			for (var i = 0; i < employeeList.Length; i++)
			{
				if (stage.signofftype == SignoffType.ClaimantSelectsOwnChecker && !subAccountProperties.AllowEmployeeInOwnSignoffGroup && (requestedEmployee.EmployeeID.ToString() == employeeList[i].Value))
				{
					continue;
				}

				if (addingFavourites)
				{
					if (employeeList[i].Text == @"#ENDFAVOURITES#")
					{
						break;
					}

					originalApproverListStartIndex = i + 1;
					recentApprover.Add(employeeList[i].Value, employeeList[i].Text);
				}
			}

			for (var i = originalApproverListStartIndex; i < employeeList.Length; i++)
			{
				if (stage.signofftype == SignoffType.ClaimantSelectsOwnChecker && !subAccountProperties.AllowEmployeeInOwnSignoffGroup && (requestedEmployee.EmployeeID.ToString() == employeeList[i].Value))
				{
					continue;
				}
				allApprover.Add(employeeList[i].Value, employeeList[i].Text);
			}
			claimSubmissionDetails.Approvers = new ClaimSubmissionApprover() { RecentApprovers = recentApprover, AllApprovers = allApprover };

			foreach (var employee in employeeList)
			{
				if (employee.Selected)
				{
					claimSubmissionDetails.Approvers.LastApproverId = Convert.ToInt32(employee.Value);
					claimSubmissionDetails.Approvers.LastApprover = employee.Text;
				}
			}

			return claimSubmissionDetails;
		}

        
        /// <summary>
        /// Determine if claim can be submitted or not.
        /// </summary>
        /// <param name="accountid">
        /// Current accountid.
        /// </param>
        /// <param name="claimid">
        /// Claimid to be submitted.
        /// </param>
        /// <param name="employeeid">
        /// Current employeeid.
        /// </param>
        /// <param name="viewfilter">
        /// viewfilter.
        /// </param>
        /// <returns>
        /// <see cref="SubmitClaimResult">SubmitClaimResult</see>.
        /// </returns>
        public SubmitClaimResult DetermineIfClaimCanBeSubmitted(
           int accountid,
           int claimid,
           int employeeid,
           byte viewfilter)
        {
            var currentUser = cMisc.GetCurrentUser();
            var claim = this.getClaimById(claimid);
            var claimSubmission = new ClaimSubmission(currentUser);
            return claimSubmission.DetermineIfClaimCanBeSubmitted(claim, viewfilter);
        }

		/// <summary>
		/// Determine if a claim can be unsubmitted or not.
		/// </summary>
		/// <param name="claimId">The ID of the claim to be unsubmitted.</param>
		/// <returns><see cref="ClaimUnsubmittableReason"/></returns>
		public ClaimUnsubmittableReason DetermineIfClaimCanBeUnsubmitted(int claimId)
		{
			var claim = this.getClaimById(claimId);

			SortedList<int, cExpenseItem> expenseItems = this.getExpenseItemsFromDB(claimId);

			bool itemsAllowed = expenseItems.Values.Any(expenseItem => expenseItem.tempallow);

			var resposne = CanClaimBeUnSubmitted(claim, expenseItems, itemsAllowed);

			return resposne;
		}

		/// <summary>
		/// Save claimant justification.
		/// </summary>
		/// <param name="flaggedItemId">The Id of the flag</param>
		/// <param name="justification">The justification for the flag</param>
		public void SaveClaimantJustification(int flaggedItemId, string justification)
		{
			var user = cMisc.GetCurrentUser();
			var flagManagement = new FlagManagement(user.AccountID);

			flagManagement.SaveClaimantJustification(flaggedItemId, justification, user);
		}

		/// <summary>
		/// Save approver justification.
		/// </summary>
		/// <param name="flaggedItemId">The Id of the flag</param>
		/// <param name="justification">The justification for the flag</param>
		public void SaveApproverJustification(int flaggedItemId, string justification)
		{
			var user = cMisc.GetCurrentUser();
			var flagManagement = new FlagManagement(user.AccountID);

			flagManagement.SaveAuthoriserJustification(flaggedItemId, justification, user.EmployeeID, user);
		}

		/// <summary>
		/// Get the basic definition details of a claim... name, description, user defined fields.
		/// </summary>
		/// <param name="claimId">The id of the claim.</param>
		/// <returns>A <see cref="ClaimDefinition">ClaimDefinition</see></returns>
		public ClaimDefinition GetClaimDefinition(int claimId)
		{
			var claimDefinition = new ClaimDefinition { ClaimId = claimId };

			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.sqlexecute.Parameters.AddWithValue("@claimId", claimId);

				using (var reader = connection.GetReader("dbo.GetClaimDefinition", CommandType.StoredProcedure))
				{
					int nameOrdinal = reader.GetOrdinal("name");
					int descriptionOrdinal = reader.GetOrdinal("description");

					while (reader.Read())
					{
						claimDefinition.Name = reader.GetString(nameOrdinal);
						claimDefinition.Description = reader.GetString(descriptionOrdinal);
					}
				}
			}

			claimDefinition.UserDefinedFields = GetClaimDefinitionUDFs();

			return claimDefinition;
		}

		/// <summary>
		/// Gets the UDF details for associated with Claims
		/// </summary>
		/// <returns>A list of <see cref="UserDefinedFieldValue">UserDefinedFieldValue</see></returns>
		public List<UserDefinedFieldValue> GetClaimDefinitionUDFs()
		{
			SortedList<int, cUserDefinedField> userdefinedList = this.GetUserDefinedFieldsForClaims();
			var userDefinedFieldValues = new List<UserDefinedFieldValue>();

			foreach (KeyValuePair<int, cUserDefinedField> keyValuePair in userdefinedList)
			{
				userDefinedFieldValues.Add(new UserDefinedFieldValue(keyValuePair.Key, keyValuePair.Value));
			}

			return userDefinedFieldValues;
		}

		/// <summary>
		/// Notifies Approvers Who Have Pending Claims
		/// </summary>
		/// <returns>Returns if the email was sent or not</returns>
		public bool NotifyClaimApproverOfPendingClaims(int accountId = 0)
		{
			try
			{
				// get the list of approvers who have pending claims to approve
				var approverEmailDetails = accountId == 0 ? ClaimEmailReminder.GetApproverIdsWhoHavePendingClaims() : ClaimEmailReminder.GetApproverIdsWhoHavePendingClaims(accountId);

				if (approverEmailDetails == null || approverEmailDetails.Count == 0)
				{
					return false;
				}

				var approverAccountId = 0;
				var msgFrom = string.Empty;
				NotificationTemplates emails = null;
				approverEmailDetails = approverEmailDetails.OrderBy(o => o.AccountId).ToList();
				foreach (var approver in approverEmailDetails)
				{
				    HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(approver.AccountId, approver.EmployeeId));

                    if (approverAccountId == 0 || approverAccountId != approver.AccountId)
					{
						emails = new NotificationTemplates(approver.AccountId, approver.EmployeeId, string.Empty, 0, Modules.Expenses);
						var clsSubAccounts = new cAccountSubAccounts(approver.AccountId);
						var reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();
						msgFrom = reqProperties.SourceAddress == 1 ? reqProperties.EmailAdministrator : "admin@sel-expenses.com";
						approverAccountId = approver.AccountId;
					}

					if (emails != null)
					{
						emails.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAnApproverWhoHasPendingClaims), 0, new[] { approver.EmployeeId }, defaultSender: msgFrom);
					}
				}
			}
			catch (Exception ex)
			{
				var message =
					$"cClaims : NotifyClaimApproverOfPendingClaims : {Environment.NewLine} {ex.Message}{Environment.NewLine}{ex.StackTrace}";
				cEventlog.LogEntry(message, true, EventLogEntryType.Error);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Notifies claimants who have unsubmitted Claims
		/// </summary>
		/// <param name="id">If used, the Account Id to Get Claimants for</param>
		/// <returns>True if emails were sent.</returns>
		public bool NotifyClaimantsOfCurrentClaims(int id = 0)
		{
			try
			{
				// get the list of claimants who have unsubmitted claims
				var claimantEmailDetails = id == 0 ? ClaimEmailReminder.GetClaimantIdsWhoHaveCurrentClaims() : ClaimEmailReminder.GetClaimantIdsWhoHaveCurrentClaims(id);

				if (claimantEmailDetails == null || claimantEmailDetails.Count == 0)
				{
					return false;
				}

				var claimantAccountId = 0;
				var msgFrom = string.Empty;
				NotificationTemplates notifications = null;
				claimantEmailDetails = claimantEmailDetails.OrderBy(o => o.AccountId).ToList();
				foreach (var claimant in claimantEmailDetails)
				{
				    HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(claimant.AccountId, claimant.EmployeeId));

                    if (claimantAccountId == 0 || claimantAccountId != claimant.AccountId)
					{
						notifications = new NotificationTemplates(claimant.AccountId, claimant.EmployeeId, string.Empty, 0, Modules.Expenses);
						var clsSubAccounts = new cAccountSubAccounts(claimant.AccountId);
						var reqProperties = clsSubAccounts.getFirstSubAccount().SubAccountProperties.Clone();
						msgFrom = reqProperties.SourceAddress == 1 ? reqProperties.EmailAdministrator : "admin@sel-expenses.com";
						claimantAccountId = claimant.AccountId;
					}

					notifications?.SendMessage(SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToAClaimantToRemindThemOfUnsubmittedClaims), 0, new[] { claimant.EmployeeId }, defaultSender: msgFrom);
				}
			}
			catch (Exception ex)
			{
				var message =
					$"cClaims : NotifyClaimantsOfCurrentClaims : {Environment.NewLine} {ex.Message}{Environment.NewLine}{ex.StackTrace}";
				cEventlog.LogEntry(message, true, EventLogEntryType.Error);

				return false;
			}

			return true;
		}
		private SortedList<int, cUserDefinedField> GetUserDefinedFieldsForClaims()
		{
			var userDefinedFields = new cUserdefinedFields(this.accountid);
			var tables = new cTables(this.accountid);
			const string ClaimTableId = "f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9";
			cTable table = tables.GetTableByID(new Guid(ClaimTableId));

			SortedList<int, cUserDefinedField> claimDefinitionUDF = new SortedList<int, cUserDefinedField>();

			foreach (var field in userDefinedFields.UserdefinedFields.Values)
			{

				if (field.table.TableID == table.TableID)
				{
					claimDefinitionUDF.Add(field.order, field);
				}
			}

			return claimDefinitionUDF;
		}

		/// <summary>
		/// Gets the user defined field records for a udf category.
		/// </summary>
		/// <param name="itemId">The id of the type of udf record.</param>
		/// <param name="categoryTableGuid">The guid for0 the table of interest for the userdefined fields</param>
		/// <returns>The user defined field values for the udf category</returns>
		private SortedList<int, object> GetUserDefinedFieldsForUdfCategory(int itemId, string categoryTableGuid)
		{
			var userDefinedFields = new cUserdefinedFields(this.accountid);
			var tables = new cTables(this.accountid);
			cTable claimTable = tables.GetTableByID(new Guid(categoryTableGuid));
			cTable udfTable = tables.GetTableByID(claimTable.UserDefinedTableID);

			return userDefinedFields.GetRecord(udfTable, itemId, tables, new cFields(this.accountid));
		}

		/// <summary>
		/// Determines the Claim name for a new claim
		/// </summary>
		/// <param name="user">The current user</param>
		/// <returns>The claim name</returns>
		public string GenerateNewClaimName(ICurrentUser user)
		{
			return user.Employee.Forename[0] + user.Employee.Surname + (user.Employee.CurrentClaimNumber);
		}

		/// <summary>
		/// updates the approverLastRemindedDate to current date for all approvers of the claim who have only one pending claim.
		/// </summary>
		/// <param name="claimId">Id of the claim</param>
		/// <param name="submitted">whether the claim is going to be submitted or not, 1 - submitted, 0 - unsubmitted</param>
		public void UpdateApproverLastRemindedDate(int claimId, int submitted = 1)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.AddWithValue("@claimId", claimId);
				connection.AddWithValue("@submitted", submitted);
				connection.AddReturn("@result");
				using (connection.GetReader("UpdateApproverLastRemindedDate", CommandType.StoredProcedure))
				{
					var result = connection.sqlexecute.Parameters["@result"].Value;
					if (Convert.ToInt32(result) < 0)
					{
						throw new Exception("Failed updating approver last reminded date");
					}
				}
				connection.ClearParameters();
			}
		}

		/// <summary>
		/// Checks if the approved claim is of the same day as the last reminded date. if yes it updates the last reminded date to the oldest claim he received if the oldest claim submitted to approver date is greater than reminded date
		/// </summary>
		/// <param name="claimId">Id of the claim being approved</param>
		/// <param name="approverId">approver checking the claim</param>
		/// <param name="submitted">whether the claim is going to be submitted or not, 1 - submitted, 0 - unsubmitted</param>
		public void UpdateApproverLastRemindedDateWhenApproved(int claimId, int approverId, int submitted = 1)
		{
			using (var connection = new DatabaseConnection(cAccounts.getConnectionString(this.accountid)))
			{
				connection.AddWithValue("@claimId", claimId);
				connection.AddWithValue("@approverId", approverId);
				connection.AddWithValue("@submitted", submitted);
				connection.AddReturn("@result");
				using (connection.GetReader("CheckAndUpdateApproverRemindedDate", CommandType.StoredProcedure))
				{
					var result = connection.sqlexecute.Parameters["@result"].Value;
					if (Convert.ToInt32(result) < 0)
					{
						throw new Exception("Failed updating approver last reminded date");
					}
				}
				connection.ClearParameters();
			}
		}

		/// <summary>
		/// The check if approver justifications are required for the expense items being actioned
		/// </summary>
		/// <param name="accountId">
		/// The account id.
		/// </param>
		/// <param name="flagResults">
		/// The flag results.
		/// </param>
		/// <param name="expenseIds">
		/// The expense ids being actioned.
		/// </param>
		/// <param name="claim">
		/// The claim.
		/// </param>
		/// <returns>
		/// The <see cref="AllowExpenseItemsResult">AllowExpenseItemsResult</see>.
		/// </returns>
		private static AllowExpenseItemsResult CheckIfApproverJustificationsAreRequired(int accountId, FlaggedItemsManager flagResults, List<int> expenseIds, cClaim claim)
		{
			if (flagResults.Count == 0)
			{
				return null;
			}

			AllowExpenseItemsResult result;
			var flagSummaries = new List<ExpenseItemFlagSummary>();

			foreach (ExpenseItemFlagSummary flagSummary in flagResults.List)
			{
				if (expenseIds.Contains(flagSummary.ExpenseID))
				{
					//Add only flag details for the expense items in the request.
					flagSummaries.Add(flagSummary);
				}
			}

			if (flagSummaries.Count > 0)
			{
				//justifications are required for flagged item(s).
				//clear and rebuild the expense collection with the relevant expense flag details.
				flagResults.ClearExpenseCollection();

				foreach (var flag in flagSummaries)
				{
					flagResults.Add(flag);
				}

				var employees = new cEmployees(accountId);
				Employee employee = employees.GetEmployeeById(claim.employeeid);
				flagResults.Claimant = false;
				flagResults.Stage = claim.stage;
				flagResults.Authorising = true;
				flagResults.SubmittingClaim = false;
				flagResults.OnlyDisplayBlockedItems = false;
				flagResults.AllowingOrApproving = true;
				flagResults.ClaimantName = employee.FullName;

				result = new AllowExpenseItemsResult(null, false, flagResults, false, false);
			}
			else
			{
				result = null;
			}

			return result;
		}

		/// <summary>
		/// Determines if the user belongs to a team that is the cost code owner.
		/// </summary>
		/// <param name="employeeId">
		/// The employee id.
		/// </param>
		/// <param name="ownership">
		/// The ownership.
		/// </param>
		/// <param name="teams">
		/// An instance of <see cref="cTeams">cTeams</see>
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private static bool IsDefaultCostCodeOwner(int employeeId, IOwnership ownership, cTeams teams)
		{
			bool isDefaultOwner = false;

			if (ownership != null && ownership.ElementType() == SpendManagementElement.Teams)
			{
				cTeam defaultCostCodeTeam = teams.GetTeamById(ownership.ItemPrimaryID());

				if (defaultCostCodeTeam != null && defaultCostCodeTeam.teammembers.Contains(employeeId))
				{
					isDefaultOwner = true;
				}
			}

			return isDefaultOwner;
		}

		/// <summary>
		/// Determines whether a column is present in a data reader
		/// </summary>
		/// <param name="dataReader">
		/// The data reader.
		/// </param>
		/// <param name="columnName">
		/// The column name.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/> of the outcome.
		/// </returns>
		private static bool HasColumn(IDataRecord dataReader, string columnName)
		{
			for (int i = 0; i < dataReader.FieldCount; i++)
			{
				if (dataReader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}


        /// <summary>
        /// Submit the claim. Checks that the user can submit the claim before updating.
        /// </summary>
        /// <param name="claimId">
        /// Claim id to update.
        /// </param>
        /// <param name="claimName">
        /// Claim name to update. 
        /// </param>
        /// <param name="description">
        /// Claim description to update.
        /// </param>
        /// <param name="cash">
        /// True if a cach claim
        /// </param>
        /// <param name="credit">
        /// True if credit claim.
        /// </param>
        /// <param name="purchase">
        /// True if purchase claim.
        /// </param>
        /// <param name="approver">
        /// Claim approver id to update.
        /// </param>
        /// <param name="odometerReadings">
        /// Odometer readings.
        /// </param>
        /// <param name="businessMileage">
        /// Business mileage.
        /// </param>
        /// <param name="ignoreApproverOnHoliday">
        /// Ignore approver on holiday if true.
        /// </param>
        /// <param name="viewfilter">
        /// Viewfilter.
        /// </param>
        /// <param name="continueAlthoughAuthoriserIsOnHoliday">
        /// Continue although authoriser is on holiday if true.
        /// </param>
        /// <returns>
        /// <see cref="SubmitClaimResult">SubmitClaimResult</see>.
        /// </returns>
        public SubmitClaimResult SubmitClaim(
           int claimId,
           string claimName,
           string description,
           bool cash,
           bool credit,
           bool purchase,
           int? approver,
           List<List<object>> odometerReadings,
           bool businessMileage,
           bool ignoreApproverOnHoliday,
           byte viewfilter,
           bool continueAlthoughAuthoriserIsOnHoliday = false)
        {
            var user = cMisc.GetCurrentUser();
            var result = new SubmitClaimResult();
            var employeeCards = new cEmployeeCorporateCards(user.AccountID);
            var cards = employeeCards.GetEmployeeCorporateCards(user.EmployeeID);
            if (this.updateClaim(claimId, claimName, description, null, user.EmployeeID) == -1)
            {
                result.Reason = SubmitRejectionReason.ClaimNameAlreadyExists;
                return result;
            }
            var reqclaim = this.getClaimById(claimId);
            if (reqclaim.submitted)
            {
                result.Reason = SubmitRejectionReason.Success;
                return result;
            }
            if (cards != null && cards.Count > 0)
            {
                var statements = new cCardStatements(user.AccountID);
                var lstStatements = statements.createStatementDropDown(user.EmployeeID);
                foreach (var itm in lstStatements)
                {
                    var statementId = 0;
                    if (!int.TryParse(itm.Value, out statementId))
                    {
                        continue;
                    }
                    var statement = statements.getStatementById(statementId);
                    if (statement == null)
                    {
                        continue;
                    }
                    if (statement.Corporatecard.blockcash)
                    {
                        if (statements.getUnallocatedItemCount(user.EmployeeID, statementId) > 0)
                        {
                            result.Reason = SubmitRejectionReason.CreditCardHasUreconciledItems;
                            return result;
                        }
                    }
                    
                }
            }

            var clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            var properties = clsSubAccounts.getFirstSubAccount().SubAccountProperties;

            if (properties.BlockUnmachedExpenseItemsBeingSubmitted)
            {
                
                var lstItems = this.getExpenseItemsFromDB(claimId);
                var unallocatedItems =
                    lstItems.Values.Any(
                        item => item.itemtype == ItemType.CreditCard && item.transactionid == 0);

                if (unallocatedItems)
                {
                    result.Reason = SubmitRejectionReason.EmployeeHasUnmatchedCardItems;
                    return result;
                }
                
            }
            if (approver.HasValue)
            {
                this.AddClaimantSelectedApprover(user, (int)approver);
            }
            else
            {
                approver = 0;
            }
            var claimSubmission = new ClaimSubmission(user);
            result = claimSubmission.SubmitClaim(
                reqclaim,
                cash,
                credit,
                purchase,
                approver.Value,
                user.EmployeeID,
                null,
                viewfilter, false,
                continueAlthoughAuthoriserIsOnHoliday);
            if (result.Reason != SubmitRejectionReason.Success
                && result.Reason != SubmitRejectionReason.ClaimSentToNextStage)
            {
                return result;
            }
            // save the odometer readings
            if (odometerReadings == null)
            {
                return result;
            }
            var employeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);
            var employees = new cEmployees(user.AccountID);
            foreach (var reading in odometerReadings)
            {
                var carid = Convert.ToInt32(reading[0]);
                var car = employeeCars.GetCarByID(carid);
                var lastReading = car.getLastOdometerReading();
                var oldReading = lastReading == null ? 0 : lastReading.newreading;
                employees.saveOdometerReading(
                    0,
                    user.EmployeeID,
                    carid,
                    null,
                    oldReading,
                    Convert.ToInt32(reading[2]),
                    Convert.ToByte(businessMileage));
            }
            
            if (properties.EnterOdometerOnSubmit)
            {
                this.calculatePencePerMile(reqclaim.employeeid, reqclaim);
            }
            this.calculateReimbursableFuelCardMileage(user.EmployeeID, reqclaim.claimid, reqclaim.employeeid);
            return result;
        }

        /// <summary>
        /// Audits the view of a claim
        /// </summary>
        /// <param name="element">The <see cref="SpendManagementElement"/></param>
        /// <param name="claimName">The claim name to audit</param>
        /// <param name="claimOwnerId">The Id of the claim owner</param>
        /// <param name="user">The <see cref="ICurrentUser"/></param>
        public void AuditViewClaim(SpendManagementElement element, string claimName, int claimOwnerId, ICurrentUser user)
        {
            if (user.EmployeeID != claimOwnerId || (user.isDelegate && user.Delegate.EmployeeID != claimOwnerId))
            {
                var auditLog = new cAuditLog();
                var employees = new cEmployees(user.AccountID);
                auditLog.ViewRecord(element, $"{claimName} ({employees.GetEmployeeById(claimOwnerId).Username})", user);
            }
        }

        /// <summary>
        /// Audits the view of a claim
        /// </summary>
        /// <param name="element">The <see cref="SpendManagementElement"/></param>
        /// <param name="claimName">The claim name to audit</param>
        /// <param name="claimOwnerId">The Id of the claim owner</param>
        /// <param name="accountId">The account Id of the claim to audit</param>
        public void AuditViewClaimForSystemUser(SpendManagementElement element, string claimName, int claimOwnerId, int accountId)
        {
            var auditLog = new cAuditLog(accountId, 0);
            var employees = new cEmployees(accountId);
            auditLog.ViewRecord(element, $"{claimName} ({employees.GetEmployeeById(claimOwnerId).Username})", null);
        }

        /// <summary>
        /// Gets the claim owner employee id
        /// </summary>
        /// <param name="claimId">The id of the claim so we can identify the owner</param>
        /// <returns>The claim owner employee id</returns>
        public int GetClaimOwnerByClaimId(int claimId)
        {
            var claim = this.getClaimById(claimId);
            return claim.employeeid;
        }

        /// <summary>
        /// Gets all the employees cars with a fuel cards
        /// </summary>
        /// <param name="employeeCars"><see cref="cEmployeeCars"/></param>
        /// <returns>A list of <see cref="cCar"/></returns>
        private List<cCar> GetEmployeesActiveCarsWithAFuelCard(cEmployeeCars employeeCars)
        {
            return employeeCars.GetActiveCars().Where(e => e.fuelcard == true).ToList();
        }
    }
}