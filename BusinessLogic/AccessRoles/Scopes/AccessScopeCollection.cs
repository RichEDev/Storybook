namespace BusinessLogic.AccessRoles.Scopes
{
    using System;
    using System.Linq;

    using BusinessLogic.Accounts.Elements;

    /// <summary>
    /// A collection of <see cref="IAccessScope"/>.
    /// </summary>
    [Serializable]
    public class AccessScopeCollection : ListWrapper<IAccessScope>
    {
        /// <summary>
        /// Returns the first instance of an <see cref="IAccessScope"/> associated with the specified <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The <see cref="IAccessScope"/> associated with the <paramref name="element"/>.</returns>
        public IAccessScope this[ModuleElements element]
        {
            get
            {
                return this.BackingCollection.FirstOrDefault(scope => scope.Element == element);
            }
        }

        /// <summary>
        /// Checks if an <see cref="IAccessScope"/> should grant access to add an object.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <returns>True if the <see cref="IAccessScope"/> grants access; otherwise false.</returns>
        public bool CanAdd(ModuleElements element)
        {
            return this.GrantAccess(element, ScopeType.Add);
        }

        /// <summary>
        /// Checks if an <see cref="IAccessScope"/> should grant access to delete an object.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <returns>True if the <see cref="IAccessScope"/> grants access; otherwise false.</returns>
        public bool CanDelete(ModuleElements element)
        {
            return this.GrantAccess(element, ScopeType.Delete);
        }

        /// <summary>
        /// Checks if an <see cref="IAccessScope"/> should grant access to edit an object.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <returns>True if the <see cref="IAccessScope"/> grants access; otherwise false.</returns>
        public bool CanEdit(ModuleElements element)
        {
            return this.GrantAccess(element, ScopeType.Edit);
        }

        /// <summary>
        /// Checks if an <see cref="IAccessScope"/> should grant access to view an object.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <returns>True if the <see cref="IAccessScope"/> grants access; otherwise false.</returns>
        public bool CanView(ModuleElements element)
        {
            return this.GrantAccess(element, ScopeType.View);
        }

        /// <summary>
        /// Checks if an <see cref="IAccessScope"/> should grant access to an object for the <see cref="ScopeType"/> action.
        /// </summary>
        /// <param name="element">The element to check for.</param>
        /// <param name="type">The <see cref="ScopeType"/> action to check if the element has permission to perform.</param>
        /// <returns>True if the <see cref="IAccessScope"/> grants access; otherwise false.</returns>
        public bool GrantAccess(ModuleElements element, ScopeType type)
        {
            IAccessScope accessScope = this[element];

            if (accessScope == null)
            {
                return false;
            }
            
            switch (type)
            {
                case ScopeType.Add:
                    return accessScope.Add;
                case ScopeType.Delete:
                    return accessScope.Delete;
                case ScopeType.Edit:
                    return accessScope.Edit;
                case ScopeType.View:
                    return accessScope.View;
                default:
                    return false;
            }
        }
    }
}
