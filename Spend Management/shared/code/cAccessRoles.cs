namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Helpers;
    using Spend_Management.shared.code.AccessRoles;

    /// <summary>
    /// AccessRoles class, inherits from cAccessRoleBase
    /// </summary>
    public class cAccessRoles : cAccessRolesBase
    {

        string CacheKey = "accessRoles";
        /// <summary>
        /// Constructor for cAccessRoles
        /// </summary>
        /// <param name="accountID">AccountID</param>
        /// <param name="connectionString">The accounts connectionString</param>
        /// <param name="ignoreCache">Are we coming from the scheduler</param>
        public cAccessRoles(int accountID, string connectionString, bool ignoreCache = false)
            : base(accountID, connectionString, cMisc.GetCurrentUser())
        {
            InitialiseData();
        }

        #region Caching Methods
        /// <summary>
        /// Check if the current accounts AccessRoles are currently in cache.
        /// </summary>
        private void InitialiseData()
        {
            
                var arCaching = new Utilities.DistributedCaching.Cache();
            
            CachedList = (Dictionary<int, cAccessRole>)arCaching.Get(AccountID, this.CacheKey, string.Empty);
            

            if (CachedList == null)
            {
                CachedList = CacheList();
            }

        }

        /// <summary>
        /// Gets all access roles, from the cache.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, cAccessRole> GetAllAccessRoles()
        {
            return CachedList;
        }


        /// <summary>
        /// Saves an access Role to the database
        /// </summary>
        /// <param name="employeeId">
        /// </param>
        /// <param name="accessRoleId">
        /// </param>
        /// <param name="accessRoleName">
        /// </param>
        /// <param name="description">
        /// </param>
        /// <param name="roleAccessLevel">
        /// </param>
        /// <param name="elementDetails">
        /// </param>
        /// <param name="maximumClaimAmount">
        /// </param>
        /// <param name="minimumClaimAmount">
        /// </param>
        /// <param name="canAdjustCostCodes">
        /// </param>
        /// <param name="canAdjustDepartment">
        /// </param>
        /// <param name="canAdjustProjectCodes">
        /// </param>
        /// <param name="mustHaveBankAccount">
        /// </param>
        /// <param name="lstReportableAccessRoles">
        /// </param>
        /// <param name="delegateId">
        /// </param>
        /// <param name="lstCustomEntityAccess">
        /// </param>
        /// <param name="allowWebsiteAccess">
        /// </param>
        /// <param name="allowMobileAccess">
        /// </param>
        /// <param name="allowApiAccess">
        /// </param>
        /// <param name="selectedReportableFields">
        /// The selected Reportable Fields.
        /// </param>
        /// <param name="removedReportableFields">
        /// The removed Reportable Fields.
        /// </param>
        /// <param name="exclusionType">
        /// The exclusion Type of a report.
        /// </param>
        /// <returns>
        /// </returns>
        public int SaveAccessRole(int employeeId, int accessRoleId, string accessRoleName, string description, Int16 roleAccessLevel, object[,] elementDetails, decimal? maximumClaimAmount, decimal? minimumClaimAmount, bool canAdjustCostCodes, bool canAdjustDepartment, bool canAdjustProjectCodes, bool mustHaveBankAccount, object[] lstReportableAccessRoles, int? delegateId, object[][][][] lstCustomEntityAccess, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess, object[] selectedReportableFields = null, object[] removedReportableFields = null, int exclusionType = 0)
        {
            int response = base.SaveAccessRoleBase(employeeId, accessRoleId, accessRoleName, description, roleAccessLevel, elementDetails, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, mustHaveBankAccount, lstReportableAccessRoles, delegateId, lstCustomEntityAccess,  allowWebsiteAccess,  allowMobileAccess,  allowApiAccess, selectedReportableFields, removedReportableFields, exclusionType);

            if (response > 0)
            {
                ClearCache();
            }

            return response;
        }

        /// <summary>
        /// The get reporting fields by table id for the current accessrole.
        /// </summary>
        /// <param name="tableId">
        /// The table id of the selected dropdown from the access role page.
        /// </param>
        /// <param name="accessRoleId">
        /// The access role id of the current access role.
        /// </param>
        /// <param name="currentUser">
        /// The current user object.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/> array object to build the Grid
        /// </returns>
        public string[] GetReportingFieldsByTableId(string tableId, int accessRoleId, CurrentUser currentUser)
        {
            var clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));
            var fields = new cFields(currentUser.AccountID);
            var accessRole = clsAccessRoles.GetAccessRoleByID(accessRoleId);
            var selectedFieldsToReport = accessRole == null ? new List<Guid>() : accessRole.reportableFieldsEnabled;
            var accessRoleFields = new AccessRoleReportableField(currentUser);
            var gridReportableFields = new cGridNew(currentUser, fields.ListFieldsToDataSet(accessRoleFields.GetReportableFieldsForAccessRole(new Guid(tableId)), selectedFieldsToReport), "ReportableFields");
            gridReportableFields.KeyField = "fieldId";
            gridReportableFields.getColumnByName("fieldId").hidden = true;
            gridReportableFields.getColumnByName("exclusiontype").hidden = true;
            gridReportableFields.enableupdating = false;
            gridReportableFields.enabledeleting = false;
            gridReportableFields.enablearchiving = false;
            gridReportableFields.enablepaging = false;
            gridReportableFields.EnableSelect = true;
            foreach (var i in selectedFieldsToReport)
            {
                gridReportableFields.SelectedItems.Add(i);
            }

            gridReportableFields.EmptyText = "There are currently no fields to display.";
            var gridInfo = new SerializableDictionary<string, object>();
            gridReportableFields.InitialiseRowGridInfo = gridInfo;
            return gridReportableFields.generateGrid();
        }

        /// <summary>
        /// Saves an AccessRole to the database (the Api way not the in-page JS way)
        /// Overload to support cCustomEntityAccess rather than a jagged array.
        /// </summary>
        /// <param name="employeeId">The Id of the employee who's access this affects.</param>
        /// <param name="accessRoleId">The Id of the access role.</param>
        /// <param name="accessRoleName">The name of the access role.</param>
        /// <param name="description">The access role description.</param>
        /// <param name="roleAccessLevel">The access role level.</param>
        /// <param name="elementDetails">Which spendmanagement details can be accessed in which ways.</param>
        /// <param name="maximumClaimAmount">The maximum claim amount for this role.</param>
        /// <param name="minimumClaimAmount">The minimum claim amount for this role.</param>
        /// <param name="canAdjustCostCodes">Whether users in this role can adjust cost codes.</param>
        /// <param name="canAdjustDepartment">Whether users in this role can adjust departments.</param>
        /// <param name="canAdjustProjectCodes">Whether users in this role can adjust project codes.</param>
        /// <param name="mustHaveBankAccount">Does the Employee require a Bank Account to claim and Expense</param>
        /// <param name="lstReportableAccessRoles">Which roles are reportable.</param>
        /// <param name="delegateId">A delegate Id, if there is one.</param>
        /// <param name="customEntityAccess">A list of GreenLight rights.</param>
        /// <param name="allowWebsiteAccess"></param>
        /// <param name="allowMobileAccess"></param>
        /// <param name="allowApiAccess"></param>
        /// <returns>An int dictating the status of the operation. A <see cref="ReturnValues">ReturnValues</see> value.</returns>
        public int SaveAccessRoleApi(int employeeId, int accessRoleId, string accessRoleName, string description, short roleAccessLevel, IList<cElementAccess> elementDetails, decimal? maximumClaimAmount, decimal? minimumClaimAmount, bool canAdjustCostCodes, bool canAdjustDepartment, bool canAdjustProjectCodes, bool mustHaveBankAccount, object[] lstReportableAccessRoles, int? delegateId, SortedList<int, cCustomEntityAccess> customEntityAccess, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess)
        {
            var response = SaveAccessRoleApiBase(employeeId, accessRoleId, accessRoleName, description, roleAccessLevel, elementDetails, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, mustHaveBankAccount, lstReportableAccessRoles, delegateId, customEntityAccess, allowWebsiteAccess, allowMobileAccess, allowApiAccess);

            if (response > 0)
            {
                ClearCache();
            }

            return response;
        }

        /// <summary>
        /// Deletes an access role from the database
        /// </summary>
        /// <param name="accessRoleID"></param>
        /// <param name="employeeID"></param>
        /// <param name="delegateID"></param>
        /// <returns></returns>
        public bool DeleteAccessRole(int accessRoleID, int employeeID, int? delegateID)
        {
            this.ClearCacheForAffectedEmployees(accessRoleID);

            bool response = base.DeleteAccessRoleBase(accessRoleID, employeeID, delegateID);

            if (response)
            {
                ClearCache();
            }

            return response;
        }

        /// <summary>
        /// Clears the cache for all EmployeeAccessRoles linked to a specified access role
        /// </summary>
        /// <param name="accessRoleId">The access role</param>
        private void ClearCacheForAffectedEmployees(int accessRoleId)
        {
            var cache = new Utilities.DistributedCaching.Cache();
            var expdata = new DBConnection(cAccounts.getConnectionString(this.AccountID));

            const string SQL = "SELECT employeeid FROM EmployeeAccessRoles WHERE accessRoleID = @accessRoleID;";
            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleId);

            using (SqlDataReader reader = expdata.GetReader(SQL))
            {
                while (reader.Read())
                {
                    cache.Delete(this.AccountID, EmployeeAccessRoles.CacheArea, reader.GetInt32(0).ToString(CultureInfo.InvariantCulture));
                }

                reader.Close();
            }
        }

        private void ClearCache()
        {
            var arCaching = new Utilities.DistributedCaching.Cache();
            arCaching.Delete(this.AccountID, this.CacheKey, string.Empty);
            CachedList = null;
            InitialiseData();
        }

        /// <summary>
        /// Places a list of all AccessRoles belonging to the current account into cache, the list is passed from cAccessRoleBase
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, cAccessRole> CacheList()
        {
            Dictionary<int, cAccessRole> lstAccessRoles = GetListToCache();

            var arCaching = new Utilities.DistributedCaching.Cache();

            arCaching.Add(this.AccountID, this.CacheKey, string.Empty, lstAccessRoles);


            return lstAccessRoles;
        }

        #endregion

        #region MobileAPI specific method
        /// <summary>
        /// Establishes whether the user has a check and pay access role
        /// </summary>
        /// <param name="accountId">Account ID</param>
        /// <param name="employeeId">Employee ID to check</param>
        /// <returns>TRUE if permitted</returns>
        public static bool CanCheckAndPay(int accountId, int employeeId)
        {
            int result;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@employeeID", employeeId);
                connection.sqlexecute.Parameters.Add("@retVal", SqlDbType.Int);
                connection.sqlexecute.Parameters["@retVal"].Direction = ParameterDirection.ReturnValue;
                connection.ExecuteProc("employeeHasCheckAndPayRole");
                result = (int)connection.sqlexecute.Parameters["@retVal"].Value;
            }

            return result == 1;
        }
        #endregion
    }
}