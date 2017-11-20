namespace BusinessLogic.CustomEntities.AccessRoles
{
    using BusinessLogic.AccessRoles;

    /// <summary>
    /// <see cref="CustomEntityViewAccess">CustomEntityViewAccess</see> defines the Access for a Custom Entity View
    /// </summary>
    public class CustomEntityViewAccess : ElementAccessLevel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEntityViewAccess"/> class. 
        /// </summary>
        /// <param name="customEntityId">
        /// The customEntityId
        /// </param>
        /// <param name="customEntityViewId">
        /// The custom Entity View ID.
        /// </param>
        /// <param name="elementId">
        /// The elementId
        /// </param>
        /// <param name="view">
        /// Whether the element has View permissions
        /// </param>
        /// <param name="add">
        /// whether the element has Add permissions
        /// </param>
        /// <param name="edit">
        /// whether the element has Edit permissions
        /// </param>
        /// <param name="delete">
        /// whether the element has Delete permissions
        /// </param>
        public CustomEntityViewAccess(int customEntityId, int customEntityViewId, int elementId, bool view, bool add, bool edit, bool delete) : base(elementId, view, add, edit, delete)
        {
            this.CustomEntityId = customEntityId;
            this.CustomEntityViewId = customEntityViewId;
        }
    
        /// <summary>
        /// Gets or sets the customEntityId
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Gets or sets the customEntityViewId
        /// </summary>
        public int CustomEntityViewId { get; set; }
    }
}
