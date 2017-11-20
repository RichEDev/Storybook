namespace BusinessLogic.AccessRoles
{
    /// <summary>
    /// An Interface defining common fields of an AccessLevel.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the ID field.
    /// </typeparam>
    public interface IAccessLevel<out T>
    {
        /// <summary>
        /// Gets the AccessLevelId
        /// </summary>
        T Id { get; }

        /// <summary>
        /// Gets a value indicating whether the AccssLevel can View an element
        /// </summary>
        bool View { get; }

        /// <summary>
        /// Gets a value indicating whether the AccssLevel can Add an element
        /// </summary>
        bool Add { get; }

        /// <summary>
        /// Gets a value indicating whether the AccssLevel can Edit an element
        /// </summary>
        bool Edit { get; }

        /// <summary>
        /// Gets a value indicating whether the AccssLevel can Delete an element
        /// </summary>
        bool Delete { get; }
    }
}