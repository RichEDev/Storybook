namespace BusinessLogic.Elements
{
    using BusinessLogic.AccessRoles;
    using BusinessLogic.Interfaces;

    /// <summary>
    /// An interface defining the common fields for Elements
    /// </summary>
    public interface IElement : IIdentifier<int>
    {
        /// <summary>
        /// Gets the Id of the category that the <see cref="IElement"/> belongs to
        /// </summary>
        int CategoryId { get; }

        /// <summary>
        /// Gets the name of the <see cref="IElement"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the decription of the <see cref="IElement"/>
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a value indicating whether the associated <see cref="IAccessRole"/> can Edit the <see cref="IElement"/>
        /// </summary>
        bool AccessRolesCanEdit { get; }

        /// <summary>
        /// Gets a value indicating whether the associated <see cref="IAccessRole"/> can Add to the <see cref="IElement"/>
        /// </summary>
        bool AccessRolesCanAdd { get; }

        /// <summary>
        /// Gets a value indicating whether the associated <see cref="IAccessRole"/> can Delete the <see cref="IElement"/>
        /// </summary>
        bool AccessRolesCanDelete { get; }

        /// <summary>
        /// Gets the friendly name of the <see cref="IElement"/>
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets a value indicating whether <see cref="IAccessRole"/> are applicable to the <see cref="IElement"/>
        /// </summary>
        bool AccessRolesApplicable { get; }
    }
}