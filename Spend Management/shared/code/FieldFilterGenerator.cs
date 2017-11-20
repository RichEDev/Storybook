using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spend_Management.shared.code
{
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators;

    public class FieldFilterGenerator
    {
        /// <summary>
        /// Gets a collection of field filters ready for claim hierarchy.
        /// </summary>
        /// <param name="user">
        /// The current user.
        /// </param>
        /// <returns>
        /// Collection of FieldFilter for claim hierarchy.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// ArgumentOutOfRangeException for access role level.
        /// </exception>
        public static SortedList<int, FieldFilter> GetHeirarchyFieldFilters(ICurrentUser user)
        {
            AccessRoleLevel accessRoleLevel = user.HighestAccessLevel;
            var employeeFieldGuid = new Guid("EDA990E3-6B7E-4C26-8D38-AD1D77FB2FBF");
            cField employeeField = new cFields(user.AccountID).GetFieldByID(employeeFieldGuid);
            var usernameField = new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795");
            const string LINE_MANAGER_MATCH_FIELD = "96F11C6D-7615-4ABD-94EC-0E4D34E187A0";

            var filters = new SortedList<int, FieldFilter>
                              {
                                  {
                                      0,
                                      new FieldFilter(
                                      new cFields(user.AccountID).GetFieldByID(
                                          usernameField),
                                      ConditionType.NotLike,
                                      "admin%",
                                      null,
                                      0,
                                      null)
                                  }
                              };

            // only add filters if the user is filtered on access role visibility or employee responsibility
            switch (accessRoleLevel)
            {
                case AccessRoleLevel.EmployeesResponsibleFor:
                    filters.Add(
                        1,
                        new FieldFilter(
                            employeeField, ConditionType.AtMyClaimsHierarchy, LINE_MANAGER_MATCH_FIELD, null, 1, null));
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

                    string accessRoles = string.Join(",", reportRoles);
                    filters.Add(
                        1, new FieldFilter(employeeField, ConditionType.WithAccessRoles, accessRoles, null, 1, null));
                    break;
                case AccessRoleLevel.AllData:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return filters;
        }

        /// <summary>
        /// Gets a collection of field filters for claims hierarchy.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// A dictionary of field filters for javascript use
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// ArgumentOutOfRangeException for HierarchyType.
        /// </exception>
        public static Dictionary<string, JSFieldFilter> GetHierarchyJsFieldFilters(ICurrentUser user)
        {
            AccessRoleLevel accessRoleLevel = user.HighestAccessLevel;
            var employeeFieldGuid = new Guid("EDA990E3-6B7E-4C26-8D38-AD1D77FB2FBF");
            var usernameField = new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795");
            const string LINE_MANAGER_MATCH_FIELD = "96F11C6D-7615-4ABD-94EC-0E4D34E187A0";
            var javascriptFilters = new Dictionary<string, JSFieldFilter>
                                        {
                                            {
                                                "0",
                                                new JSFieldFilter
                                                    {
                                                        ConditionType =
                                                            ConditionType
                                                            .NotLike,
                                                        FieldID =
                                                            usernameField,
                                                        ValueOne =
                                                            "admin%"
                                                    }
                                            }
                                        };

            // only add filters if the user is filtered on access role visibility or employee responsibility
            switch (accessRoleLevel)
            {
                case AccessRoleLevel.EmployeesResponsibleFor:
                    javascriptFilters.Add(
                        "1",
                        new JSFieldFilter
                        {
                            ConditionType = ConditionType.AtMyClaimsHierarchy,
                            FieldID = employeeFieldGuid,
                            ValueOne = LINE_MANAGER_MATCH_FIELD
                        });
                    break;
                case AccessRoleLevel.SelectedRoles:
                    // get the roles that can be reported on. If > 1 role with SelectedRoles, then need to merge
                    var roles = new cAccessRoles(user.AccountID, cAccounts.getConnectionString(user.AccountID));
                    var reportRoles = new List<int>();
                    List<int> lstAccessRoles = user.Employee.GetAccessRoles().GetBy(user.CurrentSubAccountId);

                    foreach (int link in lstAccessRoles.Select(roles.GetAccessRoleByID).SelectMany(accessRole => accessRole.AccessRoleLinks.Where(link => !reportRoles.Contains(link))))
                    {
                        reportRoles.Add(link);
                    }

                    string accessRoles = string.Join(",", reportRoles);
                    var fieldFilter = new JSFieldFilter
                                                    {
                                                        FieldID = employeeFieldGuid,
                                                        ConditionType = ConditionType.WithAccessRoles,
                                                        Order = 1,
                                                        ValueOne = accessRoles
                                                    };
                    javascriptFilters.Add("1", fieldFilter);
                    break;
                case AccessRoleLevel.AllData:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return javascriptFilters;
        }
    }
}