

namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Models.Types;
    using Spend_Management;
    using Spend_Management.shared.code.GreenLight;
    using SpendManagementApi.Interfaces;
    using SpendManagementLibrary.Enumerators;
    /// <summary>
    /// Manages data access for Custom Entity Views
    /// </summary>
    internal class CustomEntityViewRepository : ArchivingBaseRepository<CustomEntityView>, ISupportsActionContext
    {
        /// <summary>
        /// The data class for this repository
        /// </summary>
        private cCustomEntities _customEntities;

        /// <summary>
        /// Creates a new CustomEntityViewRepository with the passed user and action context
        /// </summary>
        /// <param name="user">The current user</param>
        /// <param name="actionContext">The action context</param>
        public CustomEntityViewRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, entity => entity.Id, entity => entity.Name)
        {
            this._customEntities = this.ActionContext.CustomEntities;
        }

        /// <summary>
        /// Gets a list of all custom entity views
        /// </summary>
        /// <returns>List of custom entity views</returns>
        public override IList<CustomEntityView> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a custom entity view by its identifier
        /// </summary>
        /// <param name="id">The custom entity identifier</param>
        /// <returns>The custom entity view</returns>
        public override CustomEntityView Get(int id)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of custom entity views for a particular menu
        /// </summary>
        /// <param name="id">The identifier of the menu</param>
        /// <param name="forMobile">Optionally only return views that contain at least one field (attribute) that can be displayed on a mobile device.</param>
        /// <returns>A list of custom entity views</returns>
        public List<CustomEntityView> GetByMenuId(int id, bool forMobile = false)
        {
            var allViews = this._customEntities.getViewsByMenuId(id, forMobile);
            var disabledViews = new DisabledModuleMenuViews(this.User.AccountID, (int)this.User.CurrentActiveModule);

            return (from view in allViews
                where !disabledViews.IsViewDisabled(id, view.viewid)
                where this.User.CheckAccessRole(AccessRoleType.View, CustomEntityElementType.View, view.entityid, view.viewid, false, AccessRequestType.Mobile )
                let entity = this._customEntities.getEntityById(view.entityid)
                select new CustomEntityView
                {
                    Id = view.viewid,
                    EntityId = view.entityid,
                    Name = view.viewname,
                    EntityPluralName = entity.pluralname,
                    Description = view.MenuDescription,
                    IconUri = view.MenuIcon,
                    DefaultEditForm = new CustomEntityForm().From(view.DefaultEditForm, entity, this.User)
                }).ToList();
        }
    }
}