namespace BusinessLogic.CustomEntities.AccessRoles
{
    using BusinessLogic.AccessRoles;

    /// <summary>
    /// <see cref="CustomEntityFormAccess">CustomEntityFormAccess</see> defines the Access for a Custom Entity Form
    /// </summary>
    public class CustomEntityFormAccess : ElementAccessLevel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEntityFormAccess"/> class. 
        /// </summary>
        /// <param name="customEntityId">
        /// The customEntityId
        /// </param>
        /// <param name="customEntityFormId">
        /// The customEntityFormId
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
        public CustomEntityFormAccess(int customEntityId, int customEntityFormId, int elementId, bool view, bool add, bool edit, bool delete) : base(elementId, view, add, edit, delete)
        {
            this.CustomEntityId = customEntityId;
            this.CustomEntityFormId = customEntityFormId;
        }

        /// <summary>
        /// Gets or sets the customEntityId
        /// </summary>
        public int CustomEntityId { get; set; }

        /// <summary>
        /// Gets or sets the customEntityFormId
        /// </summary>
        public int CustomEntityFormId { get; set; }
    }
}
