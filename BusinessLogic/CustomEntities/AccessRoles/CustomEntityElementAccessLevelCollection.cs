namespace BusinessLogic.CustomEntities.AccessRoles
{
    using System.Collections.Generic;
    using BusinessLogic.AccessRoles;

    /// <summary>
    /// <see cref="CustomEntityElementAccessLevelCollection">CustomEntityElementAccessLevelCollection</see> defines the ElementAccessLevelsCollection for a Custom Entity
    /// </summary>
    public class CustomEntityElementAccessLevelCollection
    {
        /// <summary>
        /// Gets or sets the AccessRoleId
        /// </summary>
        public int AccessRoleId { get; private set; }

        private readonly IDictionary<int, ElementAccessLevel> _customEntityElementAccessLevelCollection;

        /// <summary>
        /// Initialises an instance of <see cref="CustomEntityElementAccessLevelCollection">CustomEntityElementAccessLevelCollection</see>
        /// </summary>
        /// <param name="accessRoleId">The accessRoleId</param>
        public CustomEntityElementAccessLevelCollection(int accessRoleId)
        {
            this.AccessRoleId = accessRoleId;
            this._customEntityElementAccessLevelCollection = new Dictionary<int, ElementAccessLevel>();
        }

        /// <summary>
        /// Gets the <see cref="CustomEntityElementAccessLevel">CustomEntityElementAccessLevel</see> for the customEntityId
        /// </summary>
        /// <param name="customEntityId">The customEntityId</param>
        /// <returns>A<see cref="CustomEntityElementAccessLevel">CustomEntityElementAccessLevel</see></returns>
        public CustomEntityElementAccessLevel this[int customEntityId] => (CustomEntityElementAccessLevel)this._customEntityElementAccessLevelCollection[customEntityId];

        /// <summary>
        /// Returns whether the supplied customerEntityId is in the class backing collection
        /// </summary>
        /// <param name="customEntityId">The customEntityId to check for</param>
        /// <returns>The outcome of the lookup</returns>
        public bool Contains(int customEntityId)
        {
            return this._customEntityElementAccessLevelCollection.ContainsKey(customEntityId);
        }

        /// <summary>
        /// Adds a customentityId and <see cref="ElementAccessLevel">ElementAccessLevel</see> to the classes backing collection
        /// </summary>
        /// <param name="customEntityId"></param>
        /// <param name="customEntityElementAccessLevel"></param>
        public void Add(int customEntityId, ElementAccessLevel customEntityElementAccessLevel)
        {
            this._customEntityElementAccessLevelCollection.Add(customEntityId, customEntityElementAccessLevel);
        }
    }
}
