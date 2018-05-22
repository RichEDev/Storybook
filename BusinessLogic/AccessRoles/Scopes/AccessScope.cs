namespace BusinessLogic.AccessRoles.Scopes
{
    using System;

    using BusinessLogic.Accounts.Elements;

    /// <summary>
    /// <see cref="AccessScope"/> defines what access should be 
    /// </summary>
    [Serializable]
    public class AccessScope : IAccessScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessScope"/> class. 
        /// </summary>
        /// <param name="moduleElement">The <see cref="ModuleElements"/> this scope relates to.</param>
        /// <param name="view">Whether this scope grants view permissions.</param>
        /// <param name="add">Whether this scope grants add permissions.</param>
        /// <param name="edit">Whether this scope grants edit permissions.</param>
        /// <param name="delete">Whether this scope grants delete permissions.</param>
        public AccessScope(ModuleElements moduleElement, bool view, bool add, bool edit, bool delete)
        {
            this.Element = moduleElement;
            this.Add = add;
            this.Edit = edit;
            this.Delete = delete;

            // If view is false but you can add, edit or delete view is automatically set to true
            this.View = view || (add || edit || delete);
        }

        /// <summary>
        /// Gets a value indicating which <see cref="ModuleElements"/> this <see cref="AccessScope"/> relates to.
        /// </summary>
        public ModuleElements Element { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this scope grants add permissions.
        /// </summary>
        public bool Add { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this scope grants delete permissions.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this scope grants edit permissions.
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this scope grants view permissions.
        /// </summary>
        public bool View { get; set; }
    }
}
