using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using SpendManagementLibrary;
using System.Collections.Generic;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcAccessRoles
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcAccessRoles : System.Web.Services.WebService
    {
        /// <summary>
        /// Deletes an access role from the database
        /// </summary>
        /// <param name="accessRoleID"></param>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public object[] DeleteAccessRole(int accessRoleID)
        {
            bool success = false;
            List<object> data = new List<object>();
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                int? delID = null;
                if (currentUser.isDelegate == true)
                {
                    delID = currentUser.Delegate.EmployeeID;
                }

                if (currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.AccessRoles, true) == true)
                {
                    cAccessRoles clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));
                    success = clsAccessRoles.DeleteAccessRole(accessRoleID, currentUser.EmployeeID, delID);
                }

                data.Add(success);
                data.Add(accessRoleID);
                if (success)
                {
                    data.Add("Access Role has been deleted");
                }
                else
                {
                    data.Add("Access Role could not be deleted as it is assigned to one or more employees");
                }
            }
            else
            {
                data.Add(false);
                data.Add(accessRoleID);
                data.Add("User is not authenticated");
            }
            return data.ToArray();
        }

        /// <summary>
        /// Saves elements access into the database, returns a string with either an int or a string message, use NaN() to check what it is in JavaScript
        /// </summary>
        /// <param name="elementDetails">object[elementID(int)][bool] Element 0: view, Element 1: add, Element 2: edit, Element 3: delete</param>
        /// <param name="accessRoleID">The AccessRoleID the elementDetails are for</param>
        /// <param name="accessRoleName">The name of the access role</param>
        /// <param name="canAdjustCostCodes">If employees can adjust cost codes</param>
        /// <param name="canAdjustDepartment">If employees can adjust departments</param>
        /// <param name="canAdjustProjectCodes">If employees can adjust project codes</param>
        /// <param name="description">Description of the access role</param>
        /// <param name="customEntityDetails"></param>
        /// <param name="maximumClaimAmount">The maximum a claim can be when submitted, null if no maximum</param>
        /// <param name="minimumClaimAmount">The minimum a claim can be when submitted, null if no minimum</param>
        /// <param name="roleAccessLevel">The role access level (AccessRoleLevel enum's Int16 value)</param>
        /// <param name="mustHaveBankAccount">Does the Employee require a Bank Account to claim an Expense Item?</param>
        /// <param name="lstReportableAccessRoles">Lists all of the acess roles this access role can report on</param>
        /// <param name="allowWebsiteAccess">allow Website access for an access role</param>
        /// <param name="allowMobileAccess">allow Mobile access for an access role</param>
        /// <param name="allowApiAccess">allow API access for an access role</param>
        /// <param name="selectedFields">selected reportable fields for an access role</param>
        /// <param name="removedFields">Removed reportable fields for an access role</param>
        /// <param name="exclusionType">The exclusion type for an access role</param>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string SaveAccessRoleElementAccess(int accessRoleID, string accessRoleName, string description, Int16 roleAccessLevel, object[][] elementDetails, object[][][][] customEntityDetails, decimal? maximumClaimAmount, decimal? minimumClaimAmount, bool canAdjustCostCodes, bool canAdjustDepartment, bool canAdjustProjectCodes, bool mustHaveBankAccount, object[] lstReportableAccessRoles, bool allowWebsiteAccess, bool allowMobileAccess, bool allowApiAccess, object[] selectedFields, object[] removedFields, int exclusionType = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!allowApiAccess && !allowMobileAccess && !allowWebsiteAccess)
                {
                    allowWebsiteAccess = true;
                }

                AccessRoleType requiredPermission = AccessRoleType.Edit;
                if (accessRoleID == 0)
                {
                    requiredPermission = AccessRoleType.Add;
                }

                CurrentUser currentUser = cMisc.GetCurrentUser();
                if (currentUser.CheckAccessRole(requiredPermission, SpendManagementElement.AccessRoles, true) == true)
                {
                    cAccessRoles clsAccessRoles = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));

                    #region reformat the access element array
                    object[,] splitElementDetails = null;

                    if (elementDetails != null)
                    {
                        splitElementDetails = new object[elementDetails.Length, 4];
                        for (int i = 0; i < elementDetails.Length; i++)
                        {
                            if (elementDetails[i] != null)
                            {
                                if (elementDetails[i][0] != null)
                                {
                                    splitElementDetails[i, 0] = Convert.ToBoolean(elementDetails[i][0]);
                                }
                                else
                                {
                                    splitElementDetails[i, 0] = false;
                                }

                                if (elementDetails[i].Length >= 2 && elementDetails[i][1] != null)
                                {
                                    splitElementDetails[i, 1] = Convert.ToBoolean(elementDetails[i][1]);
                                }
                                else
                                {
                                    splitElementDetails[i, 1] = false;
                                }

                                if (elementDetails[i].Length >= 3 && elementDetails[i][2] != null)
                                {
                                    splitElementDetails[i, 2] = Convert.ToBoolean(elementDetails[i][2]);
                                }
                                else
                                {
                                    splitElementDetails[i, 2] = false;
                                }

                                if (elementDetails[i].Length >= 4 && elementDetails[i][3] != null)
                                {
                                    splitElementDetails[i, 3] = Convert.ToBoolean(elementDetails[i][3]);
                                }
                                else
                                {
                                    splitElementDetails[i, 3] = false;
                                }
                            }
                        }
                    }
                    #endregion reformat the access element array

                    int? delID = null;
                    if (currentUser.isDelegate == true)
                    {
                        delID = currentUser.Delegate.EmployeeID;
                    }

                    int result = clsAccessRoles.SaveAccessRole(currentUser.EmployeeID, accessRoleID, accessRoleName, description, roleAccessLevel, splitElementDetails, maximumClaimAmount, minimumClaimAmount, canAdjustCostCodes, canAdjustDepartment, canAdjustProjectCodes, mustHaveBankAccount, lstReportableAccessRoles, delID, customEntityDetails, allowWebsiteAccess, allowMobileAccess, allowApiAccess, selectedFields, removedFields, exclusionType);

                    return result.ToString();
                }
                else
                {
                    return "Insufficient AccessRoles to perform this action.";
                }
            }
            else
            {
                return "You are currently not logged in.";
            }
        }

        /// <summary>
        /// The populate reporting fields.
        /// </summary>
        /// <param name="tableId">
        /// The table id selected from the dropdown.
        /// </param>
        /// <param name="accessRoleId">
        /// The access role id of the current access role being edited.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/> array object to load the grid
        /// </returns>
        [WebMethod(EnableSession = true)]
        public string[] PopulateReportingFields(string tableId, int accessRoleId)
        {
            var currentUser = cMisc.GetCurrentUser();
            var currentAccessRole = new cAccessRoles(currentUser.AccountID, cAccounts.getConnectionString(currentUser.AccountID));
            return currentAccessRole.GetReportingFieldsByTableId(tableId, accessRoleId, currentUser);
        }
    }
}
