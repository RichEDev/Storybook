using System.Collections.Generic;
using System.Linq;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Models.Common;
using SpendManagementApi.Models.Types;
using SpendManagementApi.Utilities;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementApi.Repositories
{
    /// <summary>
    /// Repository for AccessRole.
    /// </summary>
    internal class AccessRoleRepository : BaseRepository<AccessRole>, ISupportsActionContext
    {
        private readonly cAccessRoles _data;
        private readonly cEmployees _employees;

        /// <summary>
        /// Creates a new Access Role repository.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public AccessRoleRepository(ICurrentUser user, IActionContext actionContext = null) : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.AccessRoles;
            _employees = ActionContext.Employees;
        }

        /// <summary>
        /// Creates a new Access Role repository.
        /// </summary>
        /// <param name="user">The current user.</param>
        public AccessRoleRepository(ICurrentUser user) : base(user, x => x.Id, x => x.Label)
        {
            _data = new cAccessRoles(User.AccountID, cAccounts.getConnectionString(User.AccountID));
            _employees = new cEmployees(User.AccountID);
        }

        /// <summary>
        /// Gets all the Access Roles for this account.
        /// </summary>
        /// <returns>An IList of AccessRole</returns>
        public override IList<AccessRole> GetAll()
        {
            return _data.GetAllAccessRoles().Select(ar => new AccessRole().From(ar.Value, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a particular Access Role, by it's supplied Id
        /// </summary>
        /// <param name="id">The Id of the AcccessRole</param>
        /// <returns>The AccessRole</returns>
        public override AccessRole Get(int id)
        {
            var item = _data.GetAccessRoleByID(id);
            return item == null ? null : new AccessRole().From(item, ActionContext);
        }

        /// <summary>
        /// Adds an AccessRole.
        /// </summary>
        /// <param name="role">The AccessRole to add.</param>
        /// <returns>The Id of the newly added AccessRole.</returns>
        public override AccessRole Add(AccessRole role)
        {
            role = base.Add(role);
            return AttemptSaveWithConversions(role);
        }

        /// <summary>
        /// Updates an AccessRole.
        /// </summary>
        /// <param name="role">The AccessRole to add.</param>
        /// <returns>The Id of the newly added AccessRole.</returns>
        public override AccessRole Update(AccessRole role)
        {
            role = base.Update(role);
            return AttemptSaveWithConversions(role);
        }
        
        /// <summary>
        /// Either creates or removes a link between an Employee and an AccessRole.
        /// </summary>
        /// <param name="id">The Id of the Employee.</param>
        /// <param name="aid">The Id of the AccessRole.</param>
        /// <param name="employeeAndRoleAreToBeLinked">Whether to link or unlink them.</param>
        /// <param name="subAccountId">The optional sub account id.</param>
        /// <returns>An AccessRoleResponse containing the modified AccessRole.</returns>
        public bool UpdateEmployeeAccessRole(int id, int aid, bool employeeAndRoleAreToBeLinked, int? subAccountId = 1)
        {
            var employee = _employees.GetEmployeeById(id);
            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var data = new cAccessRoles(User.AccountID, cAccounts.getConnectionString(User.AccountID));
            var accessRole = data.GetAccessRoleByID(aid);

            if (accessRole == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.ApiErrorWrongIdAccessRole);
            }

            if (employeeAndRoleAreToBeLinked)
            {
                employee.GetAccessRoles().Add(new List<int> { aid }, subAccountId, User);
            }
            else
            {
                employee.GetAccessRoles().Remove(aid, subAccountId, User);
            }

            var result = employee.Save(User);

            if (result < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            return true;
        }
        
        /// <summary>
        /// Deletes an AccessRole.
        /// </summary>
        /// <param name="id">The id of the AccessRole to delete.</param>
        /// <returns>The Id of the deleted record, or -1</returns>
        public override AccessRole Delete(int id)
        {
            var accessRole = base.Delete(id);

            if (_data.DeleteAccessRole(accessRole.Id, User.EmployeeID, User.isDelegate ? User.Delegate.EmployeeID : (int?)null))
            {
                return accessRole;
            }

            throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                ApiResources.ApiErrorDeleteUnsuccessfulMessage +
                ApiResources.ApiErrorDeleteUnsuccessfulMessageAccessRoleEmployees);
        }


        private AccessRole AttemptSaveWithConversions(AccessRole role)
        {
            // convert the spend management elements
            var elementDetails = role.ElementAccess.Select(x => x.To(ActionContext)).ToArray();
            
            // convert the greenlight elements
            var customDetails = new SortedList<int, cCustomEntityAccess>();

            if (role.CustomEntityAccess.Count > 0 && role.CustomEntityAccess[0].ElementAccess.Count > 0)
            {
                var customEntities = new cCustomEntities(this.User);
                var currentEntity = customEntities.getEntityByName(role.CustomEntityAccess[0].CustomEntityName);
                role.CustomEntityAccess[0].Id = role.CustomEntityAccess[0].ElementAccess[0].CustomEntityId = currentEntity.entityid;
                role.CustomEntityAccess[0].ElementAccess[0].Id = currentEntity.Views.Values.Where(views => views.entityid == currentEntity.entityid).Select(view => view.viewid).ToList()[0];
            }


            role.CustomEntityAccess.ForEach(x => customDetails.Add(x.Id, x.To(ActionContext)));

            // attempt to save the result
            var result = _data.SaveAccessRoleApi(User.EmployeeID, role.Id, role.Label, role.Description, (short)role.AccessLevel, elementDetails,
                        role.ExpenseClaimMaximumAmount, role.ExpenseClaimMinimumAmount, role.CanEditCostCode,
                        role.CanEditDepartment, role.CanEditProjectCode, role.MustHaveBankAccount, new object[] { role.AccessRoleLinks.ToArray() }, User.isDelegate ? User.Delegate.EmployeeID : (int?) null, customDetails, role.AllowWebsiteAccess, role.AllowMobileAccess, role.AllowApiAccess);


            // return a successful edit.
            if (result > 0)
            {
                return Get(result);
            }

            // there is some kind of error.
            switch ((ReturnValues)result)
            {
                case ReturnValues.AlreadyExists:
                {
                    throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
                }
                case ReturnValues.Error:
                {
                    throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorSaveUnsuccessfulMessage);
                }
                default:
                {
                    throw new ApiException(ApiResources.ApiErrorGeneralError, ApiResources.ApiErrorGeneralError);
                }
            }

        }

    }
}