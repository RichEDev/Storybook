namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Http.Description;

    using Interfaces;
    using Models.Types;
    using Spend_Management;

    /// <summary>
    /// Manages data access for custom entity attributes
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    internal class CustomEntityAttributeRepository : BaseRepository<CustomEntityAttribute>, ISupportsActionContext
    {
        /// <summary>
        /// The data class for this repository
        /// </summary>
        private cCustomEntities _customEntities;

        /// <summary>
        /// Initialises a new instance of the <see cref="CustomEntityAttributeRepository"/> class with the passed user and action context.
        /// </summary>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="actionContext">
        /// The action context
        /// </param>
        public CustomEntityAttributeRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, record => record.Id, record => record.Id.ToString(CultureInfo.InvariantCulture))
        {
            this._customEntities = this.ActionContext.CustomEntities;
        }

        /// <summary>
        /// Gets a list of all custom entity attributes
        /// </summary>
        /// <returns>List of custom entity attributes</returns>
        public override IList<CustomEntityAttribute> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of Custom Entity Attributes for a view.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <param name="viewId">
        /// The view Id.
        /// </param>
        /// <param name="forMobile">
        /// The for mobile.
        /// </param>
        /// <returns>
        /// The <see cref="List"/> of <see cref="CustomEntityAttribute"/>.
        /// </returns>
        public List<CustomEntityAttribute> GetAll(int entityId, int viewId, bool forMobile = false)
        {
            var entity = this._customEntities.getEntityById(entityId);
            var view = entity.getViewById(viewId);

            var attributes = new List<CustomEntityAttribute>();

            foreach (var field in view.fields)
            {
                var attribute = entity.GetAttributeByFieldId(field.Value.Field.FieldID);
                if (attribute == null)
                {
                    continue;
                }

                if (!forMobile || attribute.DisplayInMobile)
                {
                    attributes.Add(new CustomEntityAttribute().From(attribute));
                }
            }

            return attributes;
        }

        /// <summary>
        /// Gets a custom entity attribute by its identifier
        /// </summary>
        /// <param name="id">The custom entity identifier</param>
        /// <returns>The custom entity attribute</returns>
        public override CustomEntityAttribute Get(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}