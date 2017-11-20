using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using Microsoft.SqlServer.Server;
using SpendManagementLibrary.Helpers;
using Spend_Management;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.AccessRoles;

    /// <summary>
    /// Base class for access roles, this class must be inherited and caching implemented for the specific enviroment used
    /// </summary>
    public abstract class cAccessRolesBase
    {
        /// <summary>
        /// AccountID which this class is currently looking at
        /// </summary>
        protected int nAccountID;
        /// <summary>
        /// The connection string for the customer database
        /// </summary>
        protected string sConnectionString;
        /// <summary>
        /// Currently cached access roles
        /// </summary>
        protected Dictionary<int, cAccessRole> CachedList;

        private ICurrentUserBase user;

        #region Properties

        /// <summary>
        /// Gets the accountID
        /// </summary>
        public int AccountID
        {
            get { return nAccountID; }
            set { nAccountID = value; }
        }

        /// <summary>
        /// Gets or sets the accounts connection string
        /// </summary>
        public string ConnectionString
        {
            get { return sConnectionString; }
            set { sConnectionString = value; }
        }

        /// <summary>
        /// Access to the cached list of all roles for this account
        /// </summary>
        public Dictionary<int, cAccessRole> AccessRoles
        {
            get { return CachedList; }
        }

        #endregion

        /// <summary>
        /// Constructor for cAccessRoleBase
        /// </summary>
        /// <param name="accountID">AccountID</param>
        /// <param name="connectionString">The accounts connection string</param>
        public cAccessRolesBase(int accountID, string connectionString, ICurrentUserBase userBase)
        {
            nAccountID = accountID;
            sConnectionString = connectionString;
            user = userBase;
        }

        /// <summary>
        /// Used to create/save a cAccessRole - uses transactions
        /// </summary>
        /// <param name="employeeId">
        /// The current employee Id.
        /// </param>
        /// <param name="accessRoleId">
        /// The current access Role Id.
        /// </param>
        /// <param name="accessRoleName">
        /// The current access Role Name.
        /// </param>
        /// <param name="description">
        /// The description of an accessrole.
        /// </param>
        /// <param name="roleAccessLevel">
        /// The role Access Level of an accessrole.
        /// </param>
        /// <param name="elementDetails">
        /// The element Details of an accessrole.
        /// </param>
        /// <param name="maximumClaimAmount">
        /// The maximum Claim Amount assigned for an accessrole.
        /// </param>
        /// <param name="minimumClaimAmount">
        /// The minimum Claim Amount assigned for an accessrole.
        /// </param>
        /// <param name="canAdjustCostCodes">
        /// The can Adjust Cost Codes settings for an accessrole.
        /// </param>
        /// <param name="canAdjustDepartment">
        /// The can Adjust Department settings for an accessrole.
        /// </param>
        /// <param name="canAdjustProjectCodes">
        /// The can Adjust Project Codes settings for an accessrole.
        /// </param>
        /// <param name="mustHaveAccessRole">
        /// The must Have Access Role settings for an accessrole.
        /// </param>
        /// <param name="lstReportableAccessRoles">
        /// The lst Reportable Access Roles elements.
        /// </param>
        /// <param name="delegateId">
        /// The delegate Id for the current session.
        /// </param>
        /// <param name="lstCustomEntityAccess">
        /// The Custom Entity list for the access role.
        /// </param>
        /// <param name="allowWebsiteAccess">
        /// The allow Website Access settings for the access role.
        /// </param>
        /// <param name="allowMobileAccess">
        /// The allow Mobile Access settings for the access role.
        /// </param>
        /// <param name="allowApiAccess">
        /// The allow Api Access settings for the access role.
        /// </param>
        /// <param name="selectedReportableFields">
        /// The selected Reportable Fields list for the current accessrole.
        /// </param>
        /// <param name="removedReportableFields">
        /// The removed Reportable Fields list for the current accessrole.
        /// </param>
        /// <param name="exclusionType">
        /// The exclusion Type, by default its going to be 1 for all new accessroles.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> result from the DB operation.
        /// </returns>
        public int SaveAccessRoleBase(int employeeId, int accessRoleId, string accessRoleName, string description, Int16 roleAccessLevel, object[,] elementDetails, decimal? maximumClaimAmount, decimal? minimumClaimAmount, bool canAdjustCostCodes, bool canAdjustDepartment, bool canAdjustProjectCodes, bool mustHaveAccessRole, object[] lstReportableAccessRoles, int? delegateId, object[][][][] lstCustomEntityAccess, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess, object[] selectedReportableFields, object[] removedReportableFields, int exclusionType = 1)
        {
            int? accessRoleIDNullable;

            if(accessRoleId == 0) {
                accessRoleIDNullable = null;
            } else {
                accessRoleIDNullable = accessRoleId;
            }

            if (AccessRoleNameAlreadyExists(accessRoleName.Trim(), accessRoleIDNullable) == false)
            {
                using (var expdata = new DatabaseConnection(sConnectionString))
                {
                    try
                    {
                        using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0)))
                        {
                            accessRoleId = SaveAccessRoleId(employeeId, accessRoleId, accessRoleName, description, roleAccessLevel, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, mustHaveAccessRole, delegateId, expdata,  allowWebsiteAccess,  allowMobileAccess,  allowApiAccess, exclusionType);
                            var lstMatchData = new List<SqlDataRecord>();
                            if (elementDetails != null)
                            {
                                lstMatchData.AddRange(AddAccessRoleElements(elementDetails));
                            }

                            if (lstCustomEntityAccess != null)
                            {
                                lstMatchData.AddRange(ConvertJaggedArrayCustomEntitiesToSql(lstCustomEntityAccess));
                            }

                            UpdateAccessRoleElements(accessRoleId, expdata, lstMatchData);

                            if (removedReportableFields.Length > 0 || selectedReportableFields.Length > 0)
                            {
                                this.UpdateReportableFieldsForAccessRole(
                                    accessRoleId,
                                    expdata,
                                    selectedReportableFields.ToList(),
                                    removedReportableFields.ToList());
                            }

                            if (lstReportableAccessRoles != null)
                            {
                                SaveAccessRolesLink(accessRoleId, roleAccessLevel, lstReportableAccessRoles, expdata);
                            }

                            expdata.sqlexecute.Parameters.Clear();

                            transaction.Complete();

                            if (accessRoleId == -1)
                            {
                                return (int)ReturnValues.AlreadyExists;
                            }
                            else
                            {
                                return accessRoleId;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("StackTrace: " + ex.StackTrace + " / Message: " + ex.Message);
                        return (int)ReturnValues.Error;
                    }
                }
            }
            else
            {
                return (int)ReturnValues.AlreadyExists;
            }
        }


        /// <summary>
        /// Used to create/save a cAccessRole - uses transactions - note that this version takes a sorted list of custom entities
        /// </summary>
        public int SaveAccessRoleApiBase(int employeeId, int accessRoleId, string accessRoleName, string description, Int16 roleAccessLevel, IList<cElementAccess> elementDetails, decimal? maximumClaimAmount, decimal? minimumClaimAmount, bool canAdjustCostCodes, bool canAdjustDepartment, bool canAdjustProjectCodes, bool mustHaveBankAccount, object[] lstReportableAccessRoles, int? delegateId, SortedList<int, cCustomEntityAccess> lstCustomEntityAccess, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess)
        {
            int? accessRoleIdNullable;

            if (accessRoleId == 0)
            {
                accessRoleIdNullable = null;
            }
            else
            {
                accessRoleIdNullable = accessRoleId;
            }

            if (AccessRoleNameAlreadyExists(accessRoleName.Trim(), accessRoleIdNullable) == false)
            {
                using (var expdata = new DatabaseConnection(sConnectionString))
                {
                    try
                    {
                        using (var transaction = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0)))
                        {
                            accessRoleId = SaveAccessRoleId(employeeId, accessRoleId, accessRoleName, description, roleAccessLevel, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, mustHaveBankAccount, delegateId, expdata, allowWebsiteAccess, allowMobileAccess, allowApiAccess);
                            var lstMatchData = new List<SqlDataRecord>();
                            if (elementDetails != null)
                            {
                                lstMatchData.AddRange(AddAccessRoleElements(elementDetails));
                            }

                            if (lstCustomEntityAccess != null)
                            {
                                lstMatchData.AddRange(ConvertCustomEntityAccessRolesToSql(lstCustomEntityAccess));
                            }

                            UpdateAccessRoleElements(accessRoleId, expdata, lstMatchData);


                            if (lstReportableAccessRoles != null)
                            {
                                SaveAccessRolesLink(accessRoleId, roleAccessLevel, lstReportableAccessRoles, expdata);
                            }

                            expdata.sqlexecute.Parameters.Clear();

                            transaction.Complete();

                            if (accessRoleId == -1)
                            {
                                return (int)ReturnValues.AlreadyExists;
                            }
                            return accessRoleId;
                        }
                    }
                    catch (Exception ex)
                    {
                        cEventlog.LogEntry("StackTrace: " + ex.StackTrace + " / Message: " + ex.Message);
                        return (int)ReturnValues.Error;
                    }
                }
            }
            return (int)ReturnValues.AlreadyExists;
        }

        /// <summary>
        /// execute the 'SaveAccessRoleElementDetails' stored procedure.
        /// </summary>
        /// <param name="accessRoleID"></param>
        /// <param name="expdata"></param>
        /// <param name="accessRoleElement"></param>
        private void UpdateAccessRoleElements(int accessRoleID, DatabaseConnection expdata, List<SqlDataRecord> accessRoleElement)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUEmployeeID", user.EmployeeID);
            if (user.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUDelegateID", user.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUDelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@AccessRoles", SqlDbType.Structured);
            expdata.sqlexecute.Parameters["@AccessRoles"].Direction = ParameterDirection.Input;
            expdata.sqlexecute.Parameters["@AccessRoles"].Value = accessRoleElement.Count == 0 ? null : accessRoleElement;

            expdata.ExecuteProc("SaveAccessRoleElementDetails");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The update reportable fields for access role.
        /// </summary>
        /// <param name="accessRoleId">
        /// The access role id.
        /// </param>
        /// <param name="expdata">
        /// The expdata.
        /// </param>
        /// <param name="selectedFields">
        /// The selected fields.
        /// </param>
        /// <param name="removedFields">
        /// The removed fields.
        /// </param>
        private void UpdateReportableFieldsForAccessRole(int accessRoleId, DatabaseConnection expdata, List<object> selectedFields, List<object> removedFields)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleId);
            expdata.sqlexecute.Parameters.AddWithValue("@CUEmployeeID", user.EmployeeID);
            if (this.user.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUDelegateID", user.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUDelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@excludedField", SqlDbType.Structured);
            expdata.sqlexecute.Parameters["@excludedField"].Direction = ParameterDirection.Input;
            expdata.sqlexecute.Parameters["@excludedField"].Value = removedFields.Count == 0 ? null : ListFieldsToDataTable(removedFields);
            expdata.sqlexecute.Parameters.Add("@includedFields", SqlDbType.Structured);
            expdata.sqlexecute.Parameters["@includedFields"].Direction = ParameterDirection.Input;
            expdata.sqlexecute.Parameters["@includedFields"].Value = selectedFields.Count == 0 ? null : ListFieldsToDataTable(selectedFields);
            expdata.ExecuteProc("SaveReportableFieldsForAccessRole");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The list fields to data table.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        private DataTable ListFieldsToDataTable(List<object> list)
        {
            var table = new DataTable();
            table.Columns.Add();
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }

        /// <summary>
        /// Execute the SaveAccessRolesLink stored procedure
        /// </summary>
        /// <param name="accessRoleId"></param>
        /// <param name="roleAccessLevel"></param>
        /// <param name="lstReportableAccessRoles"></param>
        /// <param name="expdata"></param>
        private void SaveAccessRolesLink(int accessRoleId, short roleAccessLevel, object[] lstReportableAccessRoles, DatabaseConnection expdata)
        {
            var lstMatchData = new List<SqlDataRecord>();
            SqlMetaData[] accessRolesLink = this.AccessRoleElements();
            if ((AccessRoleLevel) roleAccessLevel == AccessRoleLevel.SelectedRoles && lstReportableAccessRoles.Length > 0)
            {
                for (int i = 0; i < lstReportableAccessRoles.Length; i++)
                {
                    var reportableAccessRoleId = Convert.ToInt32(lstReportableAccessRoles[i]);
                    var row = new SqlDataRecord(accessRolesLink);
                    row.SetInt32(0, reportableAccessRoleId);
                    row.SetInt32(1, 0);
                    row.SetInt32(2, 0);
                    row.SetBoolean(3, false);
                    row.SetBoolean(4, false);
                    row.SetBoolean(5, false);
                    row.SetBoolean(6, false);
                    lstMatchData.Add(row);
                }

;
            }

            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleId);
            expdata.sqlexecute.Parameters.AddWithValue("@CUEmployeeID", user.EmployeeID);
            if (user.isDelegate)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUDelegateID", user.Delegate.EmployeeID);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUDelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.Add("@AccessRoles", SqlDbType.Structured);
            expdata.sqlexecute.Parameters["@AccessRoles"].Direction = ParameterDirection.Input;
            expdata.sqlexecute.Parameters["@AccessRoles"].Value = lstMatchData.Count == 0 ? null : lstMatchData;

            expdata.ExecuteProc("SaveAccessRolesLink");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// Create a list of sql data records for the given array.
        /// </summary>
        /// <param name="customEntityAccess">The list of custom entities updated in the form.</param>
        /// <returns>List of Sql Data Records in the Access Role Elements format.</returns>
        private IEnumerable<SqlDataRecord> ConvertJaggedArrayCustomEntitiesToSql(object[][][][] customEntityAccess)
                        {
            var lstMatchData = new List<SqlDataRecord>();

            var accessRoleElements = AccessRoleElements();
            object[][][][] jsArr = customEntityAccess;
            for (int i = jsArr.GetLowerBound(0); i <= jsArr.GetUpperBound(0); i++)
                            {
                if (jsArr[i] != null)
                                {
                    var entityID = i;
                    for (int j = jsArr[i].GetLowerBound(0); j <= jsArr[i].GetUpperBound(0); j++)
                    {
                        if (jsArr[i][j] != null)
                        {
                            var entityType = (CustomEntityElementType) j;
                            for (int k = jsArr[i][j].GetLowerBound(0); k <= jsArr[i][j].GetUpperBound(0); k++)
                            {
                                if (jsArr[i][j][k] != null)
                                {
                                    var nView = ((Convert.ToBoolean(jsArr[i][j][k][0])));
                                    var nAdd = ((Convert.ToBoolean(jsArr[i][j][k][1])));
                                    var nEdit = ((Convert.ToBoolean(jsArr[i][j][k][2])));
                                    var nDelete = ((Convert.ToBoolean(jsArr[i][j][k][3])));
                                    var elementId = k;
                                    var row = new SqlDataRecord(accessRoleElements);
                                    row.SetInt32(0, entityID);
                                    row.SetInt32(1, (int) entityType);
                                    row.SetInt32(2, elementId);
                                    row.SetSqlBoolean(3, nView);
                                    row.SetSqlBoolean(4, nAdd);
                                    row.SetSqlBoolean(5, nEdit);
                                    row.SetSqlBoolean(6, nDelete);
                                    lstMatchData.Add(row);
                                }
                            }
                        }
                    }
                }
            }

            return lstMatchData;
        }



        /// <summary>
        /// Create a list of sql data records for the given sorted list.
        /// </summary>
        /// <param name="customEntityAccess">The list of custom entities.</param>
        /// <returns>List of Sql Data Records in the Access Role Elements format.</returns>
        private IEnumerable<SqlDataRecord> ConvertCustomEntityAccessRolesToSql(SortedList<int, cCustomEntityAccess> customEntityAccess)
        {
            var lstMatchData = new List<SqlDataRecord>();
            var accessRoleElements = AccessRoleElements();
            
            foreach (var indexedItem in customEntityAccess.Values)
            {
                SqlDataRecord row;
                foreach (var form in indexedItem.FormAccess)
                {
                    row = new SqlDataRecord(accessRoleElements);
                    row.SetInt32(0, form.Value.CustomEntityID);
                    row.SetInt32(1, (int)CustomEntityElementType.Form);
                    row.SetInt32(2, form.Value.CustomEntityFormID);
                    row.SetSqlBoolean(3, form.Value.CanView);
                    row.SetSqlBoolean(4, form.Value.CanAdd);
                    row.SetSqlBoolean(5, form.Value.CanEdit);
                    row.SetSqlBoolean(6, form.Value.CanDelete);
                    lstMatchData.Add(row);
                }

                foreach (var view in indexedItem.ViewAccess)
                {
                    row = new SqlDataRecord(accessRoleElements);
                    row.SetInt32(0, view.Value.CustomEntityID);
                    row.SetInt32(1, (int)CustomEntityElementType.View);
                    row.SetInt32(2, view.Value.CustomEntityViewID);
                    row.SetSqlBoolean(3, view.Value.CanView);
                    row.SetSqlBoolean(4, view.Value.CanAdd);
                    row.SetSqlBoolean(5, view.Value.CanEdit);
                    row.SetSqlBoolean(6, view.Value.CanDelete);
                    lstMatchData.Add(row);
                }
            }

            return lstMatchData;
                            }






        /// <summary>
        /// Add the Access role elements into the Access Role Elements sql table.
        /// </summary>
        /// <param name="elementDetails"></param>
        /// <returns>List of Sql Data Records in the Access Role Elements format.</returns>
        private IEnumerable<SqlDataRecord> AddAccessRoleElements(object[,] elementDetails)
        {
            var lstMatchData = new List<SqlDataRecord>();

            var accessRoleElements = AccessRoleElements();

            for (var i = 0; i < elementDetails.GetLongLength(0); i++)
            {
                if (elementDetails[i, 0] != null)
                {
                    // If add/edit/delete are true ensure view is set to true
                    if ((bool) elementDetails[i, 1] || (bool) elementDetails[i, 2] ||
                        (bool) elementDetails[i, 3])
                    {
                        elementDetails[i, 0] = true;
                        }

                    // If view is true add to the sql, (if add/edit/delete are true view will be true automatically)
                    if ((bool) elementDetails[i, 0])
                    {
                        var nView = (Convert.ToBoolean(elementDetails[i, 0]));
                        var nAdd = (Convert.ToBoolean(elementDetails[i, 1]));
                        var nEdit = (Convert.ToBoolean(elementDetails[i, 2]));
                        var nDelete = (Convert.ToBoolean(elementDetails[i, 3]));

                        var row = new SqlDataRecord(accessRoleElements);
                        row.SetInt32(0, i);
                        row.SetInt32(1, 0);
                        row.SetInt32(2, 0);
                        row.SetSqlBoolean(3, nView);
                        row.SetSqlBoolean(4, nAdd);
                        row.SetSqlBoolean(5, nEdit);
                        row.SetSqlBoolean(6, nDelete);
                        lstMatchData.Add(row);
                    }
                }
            }

            return lstMatchData;
                }


        /// <summary>
        /// Add the Access role elements into the Access Role Elements sql table.
        /// </summary>
        /// <param name="elementDetails">An IEnumerable of cAccessElement.</param>
        /// <returns>List of Sql Data Records in the Access Role Elements format.</returns>
        private IEnumerable<SqlDataRecord> AddAccessRoleElements(IEnumerable<cElementAccess> elementDetails)
        {
            var lstMatchData = new List<SqlDataRecord>();
            var accessRoleElements = AccessRoleElements();

            foreach (var detail in elementDetails)
            {
                if (!detail.CanView) continue;
                var row = new SqlDataRecord(accessRoleElements);
                row.SetInt32(0, detail.ElementID);
                row.SetInt32(1, 0);
                row.SetInt32(2, 0);
                row.SetSqlBoolean(3, detail.CanView);
                row.SetSqlBoolean(4, detail.CanAdd);
                row.SetSqlBoolean(5, detail.CanEdit);
                row.SetSqlBoolean(6, detail.CanDelete);
                lstMatchData.Add(row);
            }


            return lstMatchData;
                }




        /// <summary>
        ///  Generate a sql AccessRoleElement table param 
        /// </summary>
        /// <returns>Sql AccessRole Element table.</returns>
        internal SqlMetaData[] AccessRoleElements()
        {
            SqlMetaData[] accessRoleElements =
            {
                new SqlMetaData("elementID", SqlDbType.Int),
                new SqlMetaData("elementType", SqlDbType.Int),
                new SqlMetaData("entityElementID", SqlDbType.Int),
                new SqlMetaData("viewAccess", SqlDbType.Bit), new SqlMetaData("insertAccess", SqlDbType.Bit),
                new SqlMetaData("updateAccess", SqlDbType.Bit), new SqlMetaData("deleteAccess", SqlDbType.Bit)
            };
            return accessRoleElements;
            }

        private static int SaveAccessRoleId(int employeeID, int accessRoleID, string accessRoleName, string description,
            short roleAccessLevel, decimal? maximumClaimAmount, decimal? minimumClaimAmount, bool canAdjustCostCodes,
            bool canAdjustDepartment, bool canAdjustProjectCodes, bool mustHaveBankAccount, int? delegateID, DatabaseConnection expdata, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess, int exclusionType = 1)
        {
            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleID);
            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleName", accessRoleName.Trim());
            expdata.sqlexecute.Parameters.AddWithValue("@description", description);
            expdata.sqlexecute.Parameters.AddWithValue("@roleAccessLevel", roleAccessLevel);
            expdata.sqlexecute.Parameters.AddWithValue("@exclusionType", exclusionType);

            if (maximumClaimAmount != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseClaimMaximumAmount", maximumClaimAmount);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseClaimMaximumAmount", DBNull.Value);
            }

            if (minimumClaimAmount != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseClaimMinimumAmount", minimumClaimAmount);
        }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@expenseClaimMinimumAmount", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@employeesCanAmendDesignatedCostCode", canAdjustCostCodes);
            expdata.sqlexecute.Parameters.AddWithValue("@employeesCanAmendDesignatedDepartment", canAdjustDepartment);
            expdata.sqlexecute.Parameters.AddWithValue("@employeesCanAmendDesignatedProjectCode", canAdjustProjectCodes);
            expdata.sqlexecute.Parameters.AddWithValue("@employeesMustHaveBankAccount", mustHaveBankAccount);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            if (delegateID.HasValue)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateID.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@allowWebsiteAccess", allowWebsiteAccess);
            expdata.sqlexecute.Parameters.AddWithValue("@allowMobileAccess", allowMobileAccess);
            expdata.sqlexecute.Parameters.AddWithValue("@allowApiAccess", allowApiAccess);
            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;

            expdata.ExecuteProc("saveAccessRole");
            accessRoleID = (int) expdata.sqlexecute.Parameters["@identity"].Value;

            expdata.sqlexecute.Parameters.Clear();

            
            return accessRoleID;
        }

        /// <summary>
        /// Returns the required cAccessRole or null if not found
        /// </summary>
        /// <param name="accessRoleID">AccessRoleID you want</param>
        /// <returns></returns>
        public cAccessRole GetAccessRoleByID(int accessRoleID)
        {
            cAccessRole tmpAccessRole;
            CachedList.TryGetValue(accessRoleID, out tmpAccessRole);

            return tmpAccessRole;
        }

        /// <summary>
        /// Returns the required cAccessRole or null if not found
        /// </summary>
        /// <param name="name">Access Role name</param>
        /// <returns>cAccessRole object</returns>
        public cAccessRole GetAccessRoleByName(string name)
        {
            cAccessRole tmpAccessRole = (from x in CachedList.Values
                                             where x.RoleName == name
                                             select x).FirstOrDefault();

            return tmpAccessRole;
        }

        /// <summary>
        /// Returns a SortedList with all roles and element access, used to cache the list in cAccessRoles
        /// </summary>
        /// <returns></returns>
        protected Dictionary<int, cAccessRole> GetListToCache()
        {
            var lstAccessRoles = new Dictionary<int, cAccessRole>();
            var lstRoleElementDetails = this.GetElementDetails();
            var lstRoleCustomEntityDetails = this.GetCustomEntityDetails();
            var lstAccessRoleLink = this.GetAccessRoleLinks();
            var accesssRoleFields = new AccessRoleFields(sConnectionString);
            var lstReportableFields = accesssRoleFields.GetReportableFieldsForAccessRoles();

            const string strSql = "SELECT roleID, roleName, description, createdOn, createdBy, modifiedOn, modifiedBy, roleAccessLevel, expenseClaimMaximumAmount, expenseClaimMinimumAmount, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount, allowWebsiteAccess, allowMobileAccess, allowApiAccess, exclusionType FROM dbo.accessRoles";

            using (var expdata = new DatabaseConnection(sConnectionString) {sqlexecute = {CommandText = strSql}})
            {
                using (var reader = expdata.GetReader(strSql))
                {
                    int roleID;
                    string roleName, roleDescription;
                    int createdBy;
                    DateTime createdOn;
                    int? modifiedBy;
                    DateTime? modifiedOn;
                    AccessRoleLevel accessLevel;
                    decimal? expenseClaimMaximumAmount;
                    decimal? expenseClaimMinimumAmount;
                    bool employeesCanAmendDesignatedCostCode;
                    bool employeesCanAmendDesignatedDepartment;
                    bool employeesCanAmendDesignatedProjectCode;
                    bool employeesMustHaveBankAccount;
                    var exclusionType = 1;

                while (reader.Read())
                    {
                        roleID = reader.GetInt32(reader.GetOrdinal("roleID"));
                        roleName = reader.GetString(reader.GetOrdinal("roleName"));
                        roleDescription = reader.IsDBNull(reader.GetOrdinal("description")) == false ? reader.GetString(reader.GetOrdinal("description")) : "";

                        createdBy = reader.GetInt32(reader.GetOrdinal("createdBy"));
                        if (!reader.IsDBNull(reader.GetOrdinal("exclusionType")))
                        {
                            exclusionType = reader.GetInt32(reader.GetOrdinal("exclusionType"));
                        }

                        createdOn = reader.GetDateTime(reader.GetOrdinal("createdOn"));

                        if (reader.IsDBNull(reader.GetOrdinal("modifiedBy")) == true)
                        {
                            modifiedBy = null;
                        }
                        else
                        {
                            modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedBy"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("modifiedOn")) == true)
                        {
                            modifiedOn = null;
                        }
                        else
                        {
                            modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedOn"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("expenseClaimMaximumAmount")) == true)
                        {
                            expenseClaimMaximumAmount = null;
                        }
                        else
                        {
                            expenseClaimMaximumAmount = reader.GetDecimal(reader.GetOrdinal("expenseClaimMaximumAmount"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("expenseClaimMinimumAmount")) == true)
                        {
                            expenseClaimMinimumAmount = null;
                        }
                        else
                        {
                            expenseClaimMinimumAmount = reader.GetDecimal(reader.GetOrdinal("expenseClaimMinimumAmount"));
                        }

                        employeesCanAmendDesignatedCostCode = reader.GetBoolean(reader.GetOrdinal("employeesCanAmendDesignatedCostCode"));
                        employeesCanAmendDesignatedDepartment = reader.GetBoolean(reader.GetOrdinal("employeesCanAmendDesignatedDepartment"));
                        employeesCanAmendDesignatedProjectCode = reader.GetBoolean(reader.GetOrdinal("employeesCanAmendDesignatedProjectCode"));
                        employeesMustHaveBankAccount = reader.GetBoolean(reader.GetOrdinal("employeesMustHaveBankAccount"));

                        var allowWebsiteAccess = reader.GetBoolean(reader.GetOrdinal("allowWebsiteAccess"));
                        var allowMobileAccess = reader.GetBoolean(reader.GetOrdinal("allowMobileAccess"));
                        var allowApiAccess = reader.GetBoolean(reader.GetOrdinal("allowApiAccess"));

                        accessLevel = (AccessRoleLevel)reader.GetInt16(reader.GetOrdinal("roleAccessLevel"));

                        if (lstAccessRoleLink.ContainsKey(roleID) == false)
                        {
                            lstAccessRoleLink.Add(roleID, new List<int>());
                        }

                        if (lstRoleCustomEntityDetails.ContainsKey(roleID) == false)
                        {
                            lstRoleCustomEntityDetails.Add(roleID, new Dictionary<int, cCustomEntityAccess>());
                        }

                        if (lstReportableFields.ContainsKey(roleID) == false)
                        {
                            lstReportableFields.Add(roleID, new List<Guid>());
                        }

                        cAccessRole tmpAccessRole;
                        if (lstRoleElementDetails.ContainsKey(roleID))
                        {
                            // check existing role and merge with new one
                            tmpAccessRole = new cAccessRole(roleID, roleName, roleDescription, lstRoleElementDetails[roleID], accessLevel, createdBy, createdOn, modifiedBy, modifiedOn, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount, expenseClaimMinimumAmount, expenseClaimMaximumAmount, lstAccessRoleLink[roleID], lstRoleCustomEntityDetails[roleID], allowWebsiteAccess, allowMobileAccess, allowApiAccess, exclusionType, lstReportableFields[roleID]);
                        }
                        else
                        {
                            tmpAccessRole = new cAccessRole(roleID, roleName, roleDescription, new Dictionary<SpendManagementElement, cElementAccess>(), accessLevel, createdBy, createdOn, modifiedBy, modifiedOn, employeesCanAmendDesignatedCostCode, employeesCanAmendDesignatedDepartment, employeesCanAmendDesignatedProjectCode, employeesMustHaveBankAccount, expenseClaimMinimumAmount, expenseClaimMaximumAmount, lstAccessRoleLink[roleID], lstRoleCustomEntityDetails[roleID], allowWebsiteAccess, allowMobileAccess, allowApiAccess, exclusionType, lstReportableFields[roleID]);
                        }

                        lstAccessRoles.Add(roleID, tmpAccessRole);
                    }

                    reader.Close();
                }
                expdata.sqlexecute.Parameters.Clear();
            }


            return lstAccessRoles;
        }

        /// <summary>
        /// Returns a list of AccessRole links, i.e. AccessRoleID 1 may be able to report on AccessRole 2, 4 and 6
        /// </summary>
        /// <returns></returns>
        private SortedList<int, List<int>> GetAccessRoleLinks()
        {
            var lstAccessRoleLinks = new SortedList<int, List<int>>();
            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                const string strSql = "SELECT primaryAccessRoleID, secondaryAccessRoleID FROM dbo.accessRolesLink";

                using (var reader = expdata.GetReader(strSql))
                {
                int primaryAccessRoleID;
                int secondaryAccessRoleID;

                while (reader.Read())
                    {
                    primaryAccessRoleID = reader.GetInt32(reader.GetOrdinal("primaryAccessRoleID"));
                    secondaryAccessRoleID = reader.GetInt32(reader.GetOrdinal("secondaryAccessRoleID"));

                    if (lstAccessRoleLinks.ContainsKey(primaryAccessRoleID) == false)
                    {
                        lstAccessRoleLinks.Add(primaryAccessRoleID, new List<int>());
                    }

                    lstAccessRoleLinks[primaryAccessRoleID].Add(secondaryAccessRoleID);

                }

                reader.Close();
            }

                expdata.sqlexecute.Parameters.Clear();
            }

            return lstAccessRoleLinks;
        }
        
        /// <summary>
        /// Returns a list containing all element access for all roles
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Dictionary<SpendManagementElement, cElementAccess>> GetElementDetails()
        {
            Dictionary<int, Dictionary<SpendManagementElement, cElementAccess>> lstRoleElements = new Dictionary<int, Dictionary<SpendManagementElement, cElementAccess>>();

            const string strSQL = "SELECT roleID, elementID, updateAccess, insertAccess, deleteAccess, viewAccess FROM dbo.accessRoleElementDetails";

            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                using (var reader = expdata.GetReader(strSQL))
                {
                int roleID, elementID;
                SpendManagementElement element;
                bool updateAccess, insertAccess, deleteAccess, viewAccess;
                cElementAccess elementAccess;

                while (reader.Read())
                    {
                        roleID = reader.GetInt32(reader.GetOrdinal("roleID"));
                        elementID = reader.GetInt32(reader.GetOrdinal("elementID"));
                        element = (SpendManagementElement)elementID;
                        updateAccess = reader.GetBoolean(reader.GetOrdinal("updateAccess"));
                        insertAccess = reader.GetBoolean(reader.GetOrdinal("insertAccess"));
                        deleteAccess = reader.GetBoolean(reader.GetOrdinal("deleteAccess"));
                        viewAccess = reader.GetBoolean(reader.GetOrdinal("viewAccess"));

                        elementAccess = new cElementAccess(elementID, viewAccess, insertAccess, updateAccess, deleteAccess);

                        if (lstRoleElements.ContainsKey(roleID) == false)
                            {
                            lstRoleElements.Add(roleID, new Dictionary<SpendManagementElement, cElementAccess>());
                            }

                        if (lstRoleElements[roleID].ContainsKey(element) == false)
                            {
                            lstRoleElements[roleID].Add(element, elementAccess);
                            }
                        else
                        {
                            if (lstRoleElements[roleID][element].CanAdd == false && insertAccess == true)
                            {
                                lstRoleElements[roleID][element].CanAdd = true;
                            }

                            if (lstRoleElements[roleID][element].CanDelete == false && insertAccess == true)
                            {
                                lstRoleElements[roleID][element].CanDelete = true;
                            }

                            if (lstRoleElements[roleID][element].CanEdit == false && insertAccess == true)
                            {
                                lstRoleElements[roleID][element].CanEdit = true;
                        }

                        if (lstRoleElements[roleID][element].CanView == false && viewAccess == true)
                        {
                            lstRoleElements[roleID][element].CanView = true;
                        }
                    }
                }

                    reader.Close();
                }
            }

            return lstRoleElements;
        }


        /// <summary>
        /// Returns a list containing all element access for all roles
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Dictionary<int, cCustomEntityAccess>> GetCustomEntityDetails()
        {
            Dictionary<int, Dictionary<int, cCustomEntityAccess>> lstRoleCustomEntities = new Dictionary<int, Dictionary<int, cCustomEntityAccess>>();

            string strCustomEntitySQL = "SELECT accessRoleCustomEntityDetailsID, roleID, customEntityID, viewAccess, insertAccess, updateAccess, deleteAccess FROM dbo.accessRoleCustomEntityDetails";
            string strViewsSQL = "SELECT accessRoleCustomEntityViewDetailsID, roleID, customEntityID, customEntityViewID, viewAccess, insertAccess, updateAccess, deleteAccess FROM dbo.accessRoleCustomEntityViewDetails";
            string strFormsSQL = "SELECT accessRoleCustomEntityFormDetailsID, roleID, customEntityID, customEntityFormID, viewAccess, insertAccess, updateAccess, deleteAccess FROM dbo.accessRoleCustomEntityFormDetails";

            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                using (var reader = expdata.GetReader(strCustomEntitySQL))
                {
                int ID, roleID, customEntityID;
                bool updateAccess, insertAccess, deleteAccess, viewAccess;
                cCustomEntityAccess customEntityDetails;

                while (reader.Read())
                    {
                    ID = reader.GetInt32(reader.GetOrdinal("accessRoleCustomEntityDetailsID"));
                    roleID = reader.GetInt32(reader.GetOrdinal("roleID"));
                    customEntityID = reader.GetInt32(reader.GetOrdinal("customEntityID"));
                    viewAccess = reader.GetBoolean(reader.GetOrdinal("viewAccess"));
                    insertAccess = reader.GetBoolean(reader.GetOrdinal("insertAccess"));
                    updateAccess = reader.GetBoolean(reader.GetOrdinal("updateAccess"));
                    deleteAccess = reader.GetBoolean(reader.GetOrdinal("deleteAccess"));

                    customEntityDetails = new cCustomEntityAccess(customEntityID, viewAccess, insertAccess, updateAccess, deleteAccess, new SortedList<int,cCustomEntityViewAccess>(), new SortedList<int,cCustomEntityFormAccess>());

                    if (lstRoleCustomEntities.ContainsKey(roleID) == false)
                    {
                        lstRoleCustomEntities.Add(roleID, new Dictionary<int,cCustomEntityAccess>());
                    }

                    if (lstRoleCustomEntities[roleID].ContainsKey(customEntityID) == false)
                    {
                        lstRoleCustomEntities[roleID].Add(customEntityID, customEntityDetails);
                    }
                }

                reader.Close();
            }

                using (var reader = expdata.GetReader(strViewsSQL))
                {
                int ID, roleID, customEntityID, viewID;
                bool updateAccess, insertAccess, deleteAccess, viewAccess;
                cCustomEntityViewAccess customEntityViewDetails;

                while (reader.Read())
                    {
                    ID = reader.GetInt32(reader.GetOrdinal("accessRoleCustomEntityViewDetailsID"));
                    roleID = reader.GetInt32(reader.GetOrdinal("roleID"));
                    customEntityID = reader.GetInt32(reader.GetOrdinal("customEntityID"));
                    viewID = reader.GetInt32(reader.GetOrdinal("customEntityViewID"));
                    viewAccess = reader.GetBoolean(reader.GetOrdinal("viewAccess"));
                    insertAccess = reader.GetBoolean(reader.GetOrdinal("insertAccess"));
                    updateAccess = reader.GetBoolean(reader.GetOrdinal("updateAccess"));
                    deleteAccess = reader.GetBoolean(reader.GetOrdinal("deleteAccess"));

                    customEntityViewDetails = new cCustomEntityViewAccess(customEntityID, viewID, viewAccess, insertAccess, updateAccess, deleteAccess);

                    #region stop roles falling over if the custom entity has no role definitions but it's views do
                    if (lstRoleCustomEntities.ContainsKey(roleID) == false)
                        {
                        lstRoleCustomEntities.Add(roleID, new Dictionary<int, cCustomEntityAccess>());
                        }
                    if (lstRoleCustomEntities[roleID].ContainsKey(customEntityID) == false)
                    {
                        lstRoleCustomEntities[roleID].Add(customEntityID, new cCustomEntityAccess(customEntityID, false, false, false, false, new SortedList<int, cCustomEntityViewAccess>(), new SortedList<int, cCustomEntityFormAccess>()));
                    }
                    #endregion stop roles falling over if the custom entity has no role definitions but it's views do

                    if (lstRoleCustomEntities[roleID] != null && lstRoleCustomEntities[roleID].ContainsKey(customEntityID) == true)
                    {
                        if (lstRoleCustomEntities[roleID][customEntityID].ViewAccess.ContainsKey(viewID) == false)
                        {
                            lstRoleCustomEntities[roleID][customEntityID].ViewAccess.Add(viewID, customEntityViewDetails);
                        }
                    }
                }

                reader.Close();
            }

                using (var reader = expdata.GetReader(strFormsSQL))
                {
                int ID, roleID, customEntityID, formID;
                bool updateAccess, insertAccess, deleteAccess, viewAccess;
                cCustomEntityFormAccess customEntityFormDetails;

                while (reader.Read())
                {
                    ID = reader.GetInt32(reader.GetOrdinal("accessRoleCustomEntityFormDetailsID"));
                    roleID = reader.GetInt32(reader.GetOrdinal("roleID"));
                    customEntityID = reader.GetInt32(reader.GetOrdinal("customEntityID"));
                    formID = reader.GetInt32(reader.GetOrdinal("customEntityFormID"));
                    viewAccess = reader.GetBoolean(reader.GetOrdinal("viewAccess"));
                    insertAccess = reader.GetBoolean(reader.GetOrdinal("insertAccess"));
                    updateAccess = reader.GetBoolean(reader.GetOrdinal("updateAccess"));
                    deleteAccess = reader.GetBoolean(reader.GetOrdinal("deleteAccess"));
                    
                    customEntityFormDetails = new cCustomEntityFormAccess(customEntityID, formID, viewAccess, insertAccess, updateAccess, deleteAccess);

                    #region stop roles falling over if the custom entity has no role definitions but it's forms do
                    if (lstRoleCustomEntities.ContainsKey(roleID) == false)
                        {
                        lstRoleCustomEntities.Add(roleID, new Dictionary<int, cCustomEntityAccess>());
                        }
                    if (lstRoleCustomEntities[roleID].ContainsKey(customEntityID) == false)
                    {
                        lstRoleCustomEntities[roleID].Add(customEntityID, new cCustomEntityAccess(customEntityID,false,false,false,false,new SortedList<int,cCustomEntityViewAccess>(), new SortedList<int,cCustomEntityFormAccess>()));
                    }
                    #endregion stop roles falling over if the custom entity has no role definitions but it's forms do

                    if (lstRoleCustomEntities.ContainsKey(roleID) == true && lstRoleCustomEntities[roleID].ContainsKey(customEntityID) == true)
                    {
                        if (lstRoleCustomEntities[roleID][customEntityID].FormAccess.ContainsKey(formID) == false)
                        {
                            lstRoleCustomEntities[roleID][customEntityID].FormAccess.Add(formID, customEntityFormDetails);
                        }
                    }
                }

                    reader.Close();
                }
            }

            return lstRoleCustomEntities;
        }

        /// <summary>
        /// Returns a list of cAccessRoles sorted by the cAccessRole name
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, cAccessRole> SortAccessRoles()
        {
            return (from x in CachedList.Values
                    orderby x.RoleName
                    select x).ToDictionary(a => a.RoleName);
        }

        /// <summary>
        /// Saves an employees access role
        /// </summary>
        /// <param name="lstAccessRoleIDs"></param>
        /// <param name="employeeID"></param>
        public void SaveEmployeeAccessRoles(List<int> lstAccessRoleIDs, int employeeID)
        {
            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                StringBuilder sbSQL = new StringBuilder();

            expdata.sqlexecute.Parameters.AddWithValue("@employeeID", employeeID);
            sbSQL.Append("DELETE FROM employeeAccessRoles WHERE employeeID=@employeeID");
            expdata.ExecuteSQL(sbSQL.ToString());

            sbSQL = new StringBuilder();

            foreach (int accessRoleID in lstAccessRoleIDs)
            {
                sbSQL.Append("INSERT INTO employeeAccessRoles (employeeID, accessRoleID) VALUES (@employeeID, " + accessRoleID + ");\n");
            }

            expdata.ExecuteSQL(sbSQL.ToString());

                expdata.sqlexecute.Parameters.Clear();
            }
        }

        /// <summary>
        /// Deletes an AccessRole from the database, checks to see if the AccessRole is in use prior to deleting
        /// </summary>
        /// <param name="accessRoleID">The AccessRoleID you wish to delete</param>
        /// <param name="employeeID"></param>
        /// <param name="delegateID"></param>
        /// <returns></returns>
        public bool DeleteAccessRoleBase(int accessRoleID, int employeeID, int? delegateID)
        {
            bool executed;
            using (var expdata = new DatabaseConnection(sConnectionString))
            {
                string strSQL = "SELECT COUNT(*) FROM dbo.employeeAccessRoles WHERE accessRoleID=@accessRoleID";
                expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleID);
                executed = false;

                int count = expdata.ExecuteScalar<int>(strSQL);
                if (count == 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
                    if (delegateID.HasValue == true)
            {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", delegateID.Value);
                }
                else
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
                }
                expdata.ExecuteProc("deleteAccessRole");
                executed = true;
                }

                expdata.sqlexecute.Parameters.Clear();
            }

            return executed;
        }

        /// <summary>
        /// Returns if the specified AccessRole Name is already in use
        /// </summary>
        /// <param name="accessRoleName">The name of the AccessRole</param>
        /// <param name="accessRoleID">If editing an AccessRole specify the AccessRoleID, if a new AccessRole specify null</param>
        /// <returns></returns>
        public bool AccessRoleNameAlreadyExists(string accessRoleName, int? accessRoleID)
        {
            return (from x in CachedList.Values
                    where (!accessRoleID.HasValue && x.RoleName == accessRoleName) || (accessRoleID.HasValue && x.RoleName == accessRoleName && x.RoleID != accessRoleID.Value)
                    select x).ToList().Count > 0;
        }

        /// <summary>
        /// Returns a List of ListItems containins all access roles sorted by AccessRole Name
        /// </summary>
        /// <param name="accessRoleID">AccessRole to mark as selected if applicable</param>
        /// <param name="includeNoneOption">Choose if you need a [None] option added to the list</param>
        /// <returns></returns>
        public List<ListItem> CreateDropDown(int accessRoleID, bool includeNoneOption)
        {
            var lstSortedRoles = SortAccessRoles();
            var lstListItems = new List<ListItem>();

            if (includeNoneOption == true)
            {
                lstListItems.Add(new System.Web.UI.WebControls.ListItem("[None]", "0"));
            }
            
            foreach(cAccessRole accessRole in lstSortedRoles.Values) 
            {
                lstListItems.Add(new System.Web.UI.WebControls.ListItem(accessRole.RoleName, accessRole.RoleID.ToString()));

                if (accessRoleID == accessRole.RoleID)
                {
                    lstListItems[lstListItems.Count - 1].Selected = true;
                }
            }

            return lstListItems;
        }

        public List<ListItem> CreateDropDown()
        {
            var lstListItems = new List<ListItem>();
            lstListItems.AddRange((from x in CachedList.Values
                                   orderby x.RoleName
                                   select new ListItem(x.RoleName, x.RoleID.ToString())).ToList());
            return lstListItems;
        }
        /// <summary>
        /// Returns an ArrayList of all AccessRoles that can view CheckAndPay
        /// </summary>
        /// <returns></returns>
        public ArrayList filterCheckandPayRoles()
        {
            //Dictionary<string, cAccessRole> lstSortedAccessRoles = this.SortAccessRoles();
            var roles = new ArrayList();
            roles.AddRange((from x in CachedList.Values
                            orderby x.RoleName
                            where x.ElementAccess.ContainsKey(SpendManagementElement.CheckAndPay) && x.ElementAccess[SpendManagementElement.CheckAndPay].CanView
                            select x).ToList());
            return roles;
        }

        /// <summary>
        /// Used with expensesOffline
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public Dictionary<int, cAccessRole> GetModifiedRoles(DateTime date)
        {
            var lst = new Dictionary<int, cAccessRole>((from x in CachedList.Values
                                                                                     where x.CreatedOn > date || x.ModifiedOn > date
                                                                                     select x).ToDictionary(a => a.RoleID));
            return lst;
        }

        /// <summary>
        /// Used with expensesOffline
        /// </summary>
        /// <returns></returns>
        public List<int> GetRoleIds()
        {
            var ids = (from x in CachedList.Values
                             select x.RoleID).ToList();
            return ids;
        }
    }

    /// <summary>
    /// Used with expensesOffline
    /// </summary>
	public struct sRoleAccess
	{
		public int roleid;
		public int accessid;
	}


}
