using System.Net;
using System.Web.Http;

namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Common;
    using Models.Types;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Responses;

    using Utilities;
    using Spend_Management;

    /// <summary>
    /// DepartmentRepository manages data access for Departments.
    /// </summary>
    internal class DepartmentRepository : ArchivingBaseRepository<Department>, ISupportsActionContext
    {
        private cDepartments _data;

        /// <summary>
        /// Creates a new DepartmentRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public DepartmentRepository(ICurrentUser user)
            : base(user, x => x.Id, x => x.Label)
        {
            _data = new cDepartments(User.AccountID);
        }

        /// <summary>
        /// Creates a new DepartmentsRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public DepartmentRepository(ICurrentUser user, IActionContext actionContext) 
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
            _data = ActionContext.Departments;
        }

        /// <summary>
        /// Gets all the Departments within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<Department> GetAll()
        {
            return _data.InitialiseData().Select(b => new Department().From(b.Value, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single Department by it's id.
        /// </summary>
        /// <param name="id">The id of the Department to get.</param>
        /// <returns>The Department.</returns>
        public override Department Get(int id)
        {
            _data = new cDepartments(User.AccountID);
            var item = _data.GetDepartmentById(id);
            if (item == null)
            {
                return null;
            }
            return new Department().From(item, ActionContext);
        }

        /// <summary>
        /// Gets all the active departments within the system.
        /// </summary>
        /// <returns>A list of <see cref="DepartmentBasic">CostCode</see></returns>
        public List<DepartmentBasic> GetAllActive()
        {
            return this._data.GetAllActiveDepartments().Select(b => new DepartmentBasic().From(b, this.ActionContext)).ToList();
        }

        /// <summary>
        /// Adds a Department.
        /// </summary>
        /// <param name="item">The Department to add.</param>
        /// <returns></returns>
        public override Department Add(Department item)
        {
            item = base.Add(item);
            return SaveDepartment(item);
        }

        /// <summary>
        /// Updates a Department.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Department.</returns>
        public override Department Update(Department item)
        {
            item = base.Update(item);
            return SaveDepartment(item);
        }

        /// <summary>
        /// Deletes a Department, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Department to delete.</param>
        /// <returns>The deleted Department.</returns>
        public override Department Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteDepartment(id, User.EmployeeID);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }

        /// <summary>
        /// Archives / unarchives the item with the specified id.
        /// </summary>
        /// <param name="id">The Id of the item to Archive/Unarchive.</param>
        /// <param name="archive">Whether to archive or unarchive.</param>
        /// <returns>The edited Department</returns>
        public override Department Archive(int id, bool archive)
        {
            var item = base.Archive(id, archive);
            _data.ChangeStatus(id, archive);
            return item;
        }


        private Department SaveDepartment(Department item)
        {
            item.UserDefined = UdfValidator.Validate(item.UserDefined, this.ActionContext, "userdefinedDepartments");

            var id = _data.SaveDepartment(item.To(ActionContext));

            // throw here for when the description is in use.
            if (id == -2)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorInvalidDescriptionAlreadyExists);
            }

            // no change on the label
            if (id == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorInvalidNameAlreadyExists);
            }

            if (id == 0)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return Get(id);
        }
    }
}
