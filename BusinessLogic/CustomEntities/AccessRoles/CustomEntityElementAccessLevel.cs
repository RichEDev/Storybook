namespace BusinessLogic.CustomEntities.AccessRoles
{
    using BusinessLogic.AccessRoles;

    /// <summary>
    /// <see cref="CustomEntityElementAccessLevel">CustomEntityElementAccessLevel</see> defines the Access Levels for a Custom Entity Element
    /// </summary>
    public class CustomEntityElementAccessLevel : ElementAccessLevel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEntityElementAccessLevel"/> class. 
        /// </summary>
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
        public CustomEntityElementAccessLevel(int elementId, bool view, bool add, bool edit, bool delete) : base(elementId, view, add, edit, delete)
        {
            this.ViewAccess = new CustomEntityViewAccessCollection();
            this.FormAccess = new CustomEntityFormAccessCollection();
        }

        /// <summary>
        /// Gets a list of view access details
        /// </summary>
        public CustomEntityViewAccessCollection ViewAccess { get; }

        /// <summary>
        /// Gets a list of form access details
        /// </summary>
        public CustomEntityFormAccessCollection FormAccess { get; }
    }
}
