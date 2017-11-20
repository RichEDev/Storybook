namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Http.Description;

    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;

    /// <summary>
    /// Manages data access for custom entity forms
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    internal class CustomEntityFormRepository : BaseRepository<CustomEntityForm>, ISupportsActionContext
    {
        /// <summary>
        /// The data class for this repository
        /// </summary>
        private cCustomEntities _customEntities;

        /// <summary>
        /// Initialises a new instance of the <see cref="CustomEntityFormRepository"/> class with the passed user and action context.
        /// </summary>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="actionContext">
        /// The action context
        /// </param>
        public CustomEntityFormRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, form => form.Id, form => form.Id.ToString(CultureInfo.InvariantCulture))
        {
            this._customEntities = this.ActionContext.CustomEntities;
        }

        /// <summary>
        /// Gets a list of all custom entity views
        /// </summary>
        /// <returns>List of custom entity views</returns>
        public override IList<CustomEntityForm> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a custom entity view by its identifier
        /// </summary>
        /// <param name="id">The custom entity identifier</param>
        /// <returns>The custom entity view</returns>
        public override CustomEntityForm Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of custom entity views for a particular menu
        /// </summary>
        /// <param name="id">The identifier of the custom entity form</param>
        /// <param name="entityId">The identifier of the custom entity</param>
        /// <param name="user">The SpendManagementLibrary <see cref="ICurrentUser">user object</see></param>
        /// <returns>A list of custom entity views</returns>
        public CustomEntityForm Get(int id, int entityId, ICurrentUser user)
        {
            cCustomEntity entity = this._customEntities.getEntityById(entityId);
            cCustomEntityForm form = entity.getFormById(id);

            return new CustomEntityForm().From(form, entity, user);
        }
    }
}