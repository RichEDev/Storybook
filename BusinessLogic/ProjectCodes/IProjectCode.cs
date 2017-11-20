namespace BusinessLogic.ProjectCodes
{
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Interface defining common field of a project code.
    /// </summary>
    public interface IProjectCode : IIdentifier<int>, IArchivable
    {
        /// <summary>
        /// Gets or sets the name for this <see cref="IProjectCode"/>.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for this <see cref="IProjectCode"/>.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="IProjectCode"/> is rechargeable.
        /// </summary>
        bool Rechargeable { get; }

        /// <summary>
        /// Get the value to display in the list.
        /// </summary>
        /// <param name="useDescription">If true, use the "Description" field of the object, else use the "Reference"</param>
        /// <returns>The text description or reference of the object.</returns>
        string ToString(bool useDescription);
    }
}