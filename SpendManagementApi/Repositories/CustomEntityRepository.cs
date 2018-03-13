namespace SpendManagementApi.Repositories
{
    using System;
    using System.Globalization;
    using System.Linq;
    using SpendManagementLibrary;

    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Interfaces;
    using System.Web.Http.Description;
    using Spend_Management;
    using System.Collections.Generic;

    [ApiExplorerSettings(IgnoreApi = true)]
    internal class CustomEntityRepository : BaseRepository<CustomEntity>, ISupportsActionContext
    {
        /// <summary>
        /// The data class for this repository
        /// </summary>
        private cCustomEntities _customEntities;

        /// <summary>
        /// Initialises a new instance of the <see cref="CustomEntityRecordRepository"/> class with the passed user and action context.
        /// </summary>
        /// <param name="user">
        /// The current user
        /// </param>
        /// <param name="actionContext">
        /// The action context
        /// </param>
        public CustomEntityRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, record => record.Id, record => record.Id.ToString(CultureInfo.InvariantCulture))
        {
            this._customEntities = this.ActionContext.CustomEntities;
        }

        /// <summary>
        /// Get the definition of a Custom Entity
        /// </summary>
        /// <param name="id">The <see cref="Guid"/>ID of the system Custom Entity to get</param>
        /// <returns>An instance of <see cref="CustomEntity"/></returns>
        public CustomEntity Get(Guid id)
        {
            var entity = this.ActionContext.CustomEntities.CustomEntities.Values.FirstOrDefault(e => e.SystemCustomEntityId.HasValue && e.SystemCustomEntityId.Value == id);
            if (entity == null)
            {
                return null;
            }

            var result = GetCustomEntity(entity);

            return result;
        }

        /// <summary>
        /// Get all of <see cref="CustomEntity"/>
        /// </summary>
        /// <returns></returns>
        public override IList<CustomEntity> GetAll()
        {
            return this.ActionContext.CustomEntities.CustomEntities.Values.Select(customEntity => GetCustomEntity(customEntity)).ToList();
        }

        /// <summary>
        /// Get a Custom Entity with id
        /// </summary>
        /// <param name="id">The ID of the entity to get</param>
        /// <returns>An instance of <see cref="CustomEntity"/></returns>
        public override CustomEntity Get(int id)
        {
            var entity = this.ActionContext.CustomEntities.getEntityById(id);
            if (entity == null)
            {
                return null;
            }

            var result = GetCustomEntity(entity);

            return result;
        }

        private static CustomEntity GetCustomEntity(cCustomEntity entity)
        {
            var result = new CustomEntity
            {
                Id = entity.entityid,
                Fields = new List<CustomEntityFormField>()
            };

            foreach (KeyValuePair<int, cAttribute> attribute in entity.attributes)
            {
                var field = new CustomEntityFormField
                {
                    Attribute = new CustomEntityAttribute().From(attribute.Value),
                    LabelText = attribute.Value.displayname
                };
                result.Fields.Add(field);
            }

            return result;
        }
    }
}