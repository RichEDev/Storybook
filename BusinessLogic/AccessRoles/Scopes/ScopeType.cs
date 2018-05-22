namespace BusinessLogic.AccessRoles.Scopes
{
    /// <summary>
    /// Defines values for the action of a <see cref="IAccessScope"/>.
    /// </summary>
    public enum ScopeType
    {
        /// <summary>
        /// The ability to add a specified object.
        /// </summary>
        Add,

        /// <summary>
        /// The ability to edit a specified object.
        /// </summary>
        Edit,

        /// <summary>
        /// The ability to view a specified object.
        /// </summary>
        View,

        /// <summary>
        /// The ability to delete a specified object.
        /// </summary>
        Delete
    }
}
