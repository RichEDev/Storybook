namespace BusinessLogic.AccessRoles.Scopes
{
    using BusinessLogic.Accounts.Elements;

    /// <summary>
    /// Defines the required field for access scopes.
    /// </summary>
    public interface IAccessScope
    {
        /// <summary>
        /// Gets or sets a value indicating whether add access should be granted.
        /// </summary>
        bool Add { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether delete access should be granted.
        /// </summary>
        bool Delete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether edit access should be granted.
        /// </summary>
        bool Edit { get; set; }

        /// <summary>
        /// Gets a value indicating which <see cref="ModuleElements"/> this <see cref="AccessScope"/> relates to.
        /// </summary>
        ModuleElements Element { get; }

        /// <summary>
        /// Gets or sets a value indicating whether view access should be granted.
        /// </summary>
        bool View { get; set; }
    }
}