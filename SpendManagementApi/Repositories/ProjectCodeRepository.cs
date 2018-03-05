using BusinessLogic.DataConnections;
using BusinessLogic.ProjectCodes;

namespace SpendManagementApi.Repositories
{
    using Models.Common;
    using Models.Types;
    using Spend_Management;
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementApi.Models.Responses;

    using Utilities;

    /// <summary>
    /// ProjectCodeRepository manages data access for ProjectCodes.
    /// </summary>
    internal class ProjectCodeRepository : ArchivingBaseRepository<ProjectCode>
    {
        private readonly IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> _projectCodes;

        /// <summary>
        /// Creates a new ProjectCodeRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public ProjectCodeRepository(ICurrentUser user)
            : base(user, x => x.Id, x => x.Label)
        {

            this._projectCodes = WebApiApplication.container.GetInstance<IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int>>();
        }

        /// <summary>
        /// Gets all the ProjectCodes within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<ProjectCode> GetAll()
        {
            return this._projectCodes.Get().Select(a => new ProjectCode().From(a, this.ActionContext)).ToList();
        }

        /// <summary>
        /// Gets all the active Project Codes within the system.
        /// </summary>
        /// <returns>A list of <see cref="ProjectCodeBasic">Project Code</see></returns>
        public List<ProjectCodeBasic> GetAllActive()
        {
            return this._projectCodes.Get().Where(a => a.Archived == false).Select(a => new ProjectCodeBasic().From(a)).ToList();
        }

        /// <summary>
        /// Gets a single ProjectCode by it's id.
        /// </summary>
        /// <param name="id">The id of the ProjectCode to get.</param>
        /// <returns>The ProjectCode.</returns>
        public override ProjectCode Get(int id)
        {
            IProjectCodeWithUserDefinedFields projectCode = this._projectCodes[id];
            
            if (projectCode == null)
            {
                return null;
            }

            return new ProjectCode().From(projectCode, this.ActionContext);
        }

        /// <summary>
        /// Adds a ProjectCode.
        /// </summary>
        /// <param name="item">The ProjectCode to add.</param>
        /// <returns></returns>
        public override ProjectCode Add(ProjectCode item)
        {
            item = base.Add(item);
            return this.SaveProjectCode(item);
        }

        /// <summary>
        /// Updates a ProjectCode.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated ProjectCode.</returns>
        public override ProjectCode Update(ProjectCode item)
        {
            item = base.Update(item);
            return this.SaveProjectCode(item);
        }



        /// <summary>
        /// Deletes a ProjectCode, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the ProjectCode to delete.</param>
        /// <returns>The deleted ProjectCode.</returns>
        public override ProjectCode Delete(int id)
        {
            var item = base.Delete(id);

            var result = this._projectCodes.Delete(id);

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
        /// <param name="id"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        public override ProjectCode Archive(int id, bool archive)
        {
            var item = base.Archive(id, archive);

            IProjectCodeWithUserDefinedFields projectCode = this._projectCodes[id];
            if (projectCode.Archived != archive)
            {
                this._projectCodes.Archive(id);
            }

            return item;
        }

        private ProjectCode SaveProjectCode(ProjectCode item)
        {
            item.UserDefined = UdfValidator.Validate(item.UserDefined, this.ActionContext, "userdefinedProjectcodes");

            var result = this._projectCodes.Save(item.To(this.ActionContext));

            if (result.Id > 1)
            {
                item.Id = result.Id;
            }

            // throw here for when the description is in use.
            if (result.Id == -2)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorInvalidDescriptionAlreadyExists);
            }

            // no change on the label
            if (result.Id == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorInvalidNameAlreadyExists);
            }

            if (result.Id == 0)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return this.Get(item.Id);
        }

    }
}
