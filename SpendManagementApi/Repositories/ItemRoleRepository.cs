namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;
    using Utilities;
    using System;

    using SpendManagementLibrary.Employees;

    internal class ItemRoleRepository : BaseRepository<Models.Types.ItemRole>, ISupportsActionContext
    {
        private ItemRoles _itemRoles;
        private readonly cEmployees _employees;

        public ItemRoleRepository(ICurrentUser user, IActionContext actionContext = null) : base(user, actionContext, role => role.ItemRoleId, role => role.RoleName)
        {
            _itemRoles = ActionContext.ItemRoles;
            _employees = ActionContext.Employees;
        }

        /// <summary>
        /// Gets all item roles with associated sub categories
        /// </summary>
        /// <returns></returns>
        public override IList<Models.Types.ItemRole> GetAll()
        {
            List<SpendManagementLibrary.ItemRole> itemRoles = _itemRoles.GetSortedList().Values.ToList();

            return itemRoles.Select(c=>c.Cast<Models.Types.ItemRole>()).ToList();
        }

        /// <summary>
        /// Get item role with subcategories with specified id
        /// </summary>
        /// <param name="itemRoleId"></param>
        /// <returns></returns>
        public override Models.Types.ItemRole Get(int itemRoleId)
        {
            SpendManagementLibrary.ItemRole itemRole = _itemRoles.GetItemRoleById(itemRoleId);
            return itemRole.Cast<Models.Types.ItemRole>();
        }

        /// <summary>
        /// Adds item role with associated subcategories
        /// </summary>
        /// <param name="itemRole">Item role with subcategories</param>
        /// <returns></returns>
        public override Models.Types.ItemRole Add(Models.Types.ItemRole itemRole)
        {
            base.Add(itemRole);

            SpendManagementLibrary.ItemRole cItemRole = itemRole.Cast<SpendManagementLibrary.ItemRole>(ActionContext);

            int itemRoleId = _itemRoles.SaveRole(cItemRole, User); 
                

            if (itemRoleId > 0)
            {
                itemRole.ItemRoleId = itemRoleId;

                if (!UpdateItemRoles(itemRole.SubCatItemRoles, itemRoleId))
                {
                    throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ItemRole_ErrorUpdatingItemRolesForSubcats);
                }
        
                itemRole = Get(itemRoleId);
            }

            return itemRole;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="itemRoleId">Id of role to delete</param>
        /// <returns></returns>
        public override Models.Types.ItemRole Delete(int itemRoleId)
        {
            Models.Types.ItemRole itemRole = Get(itemRoleId);
            if (itemRole == null)
            {
                throw new ApiException(ApiResources.ItemRoles_InvalidItemRoleId, ApiResources.ItemRoles_InvalidItemRoleIdMessage);
            }
            _itemRoles.DeleteRole(itemRoleId);
            return Get(itemRoleId);
        }

        /// <summary>
        /// Updates item role name, description and associated sub categories
        /// The subcategories provided will overwrite the existing subcategories
        /// </summary>
        /// <param name="itemRole">Item Role to update</param>
        /// <returns></returns>
        public override Models.Types.ItemRole Update(Models.Types.ItemRole itemRole)
        {
            base.Update(itemRole);

            var convertedItemRole = itemRole.Cast<SpendManagementLibrary.ItemRole>(ActionContext);

            int itemRoleId = _itemRoles.SaveRole(convertedItemRole, User);
            foreach (RoleSubcat roleSubcat in convertedItemRole.Items.Values)
            {
                _itemRoles.SaveRoleSubcat(roleSubcat);
            }
            
            if (itemRoleId <= 0)
            {
                return null;
            }

            if (itemRoleId > 0)
            {
                itemRole.ItemRoleId = itemRoleId;

                if (!UpdateItemRoles(itemRole.SubCatItemRoles, itemRoleId))
                {
                    throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ItemRole_ErrorUpdatingItemRolesForSubcats);
                }

                itemRole = Get(itemRoleId);
            }
            return itemRole;
        }

        /// <summary>
        /// Either creates or removes a link between an Employee and an ItemRole.
        /// </summary>
        /// <param name="id">
        /// The Id of the Employee.
        /// </param>
        /// <param name="iid">
        /// The Id of the AccessRole.
        /// </param>
        /// <param name="employeeAndRoleAreToBeLinked">
        /// Whether to link or unlink them.
        /// </param>
        /// <param name="subAccountId">
        /// The optional sub account id.
        /// </param>
        /// <param name="startDate">
        /// The start Date of item role.
        /// </param>
        /// <param name="endDate">
        /// The end Date of the item role.
        /// </param>
        /// <returns>
        /// A modified ItemRole.
        /// </returns>
        public bool UpdateEmployeeItemRole(int id, int iid, bool employeeAndRoleAreToBeLinked, int? subAccountId = 1, DateTime? startDate = null, DateTime? endDate = null)
        {
            var employee = this._employees.GetEmployeeById(id);
            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId, ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var data = new ItemRoles(User.AccountID);
            var itemRole = data.GetItemRoleById(iid);

            if (itemRole == null)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.ItemRoles_InvalidItemRoleIdMessage);
            }

            if (employeeAndRoleAreToBeLinked)
            {
                employee.GetItemRoles().Add(new List<EmployeeItemRole> { new EmployeeItemRole(iid, startDate, endDate) }, this.User);
            }
            else
            {
                employee.GetItemRoles().Remove(iid, this.User);
            }

            var result = employee.Save(this.User);

            if (result < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            return true;
        }

        /// <summary>
        /// Associates Sub Categories with and Item role
        /// </summary>
        /// <param name="itemRoles">The list of <see cref="SubCatItemRole">SubCatItemRoles</see></param>
        /// <param name="itemRoleId">The itemRoleId of the item role to associate with</param>
        /// <returns>The outcome of the action</returns>
        public bool UpdateItemRoles(List<SubCatItemRole> itemRoles, int itemRoleId)
        {
            List<SubCatItemRole> addedUpdatedSubcats =
                itemRoles.Where(role => !role.ForDelete).ToList();
            List<SubCatItemRole> deleted =
                itemRoles.Where(role => role.ItemRoleSubCatId > 0 && role.ForDelete).ToList();

            try
            {
                deleted.ForEach(role => ActionContext.ItemRoles.DeleteRoleSubcat(role.SubCatId, itemRoleId));

                addedUpdatedSubcats.ForEach(role =>
                    {
                        role.ItemRoleId = itemRoleId;
                        ActionContext.ItemRoles.SaveRoleSubcat(role.Cast(ActionContext));
                    });

                //Reinitialising to refresh cache
                _itemRoles = new ItemRoles(User.AccountID);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}