namespace BusinessLogic.AccessRoles.ApplicationAccess
{
    /// <summary>
    /// Defines a <see cref="ApplicationScopeCollection"/> must be implemented.
    /// </summary>
    public interface IApplicationAccess
    {
        /// <summary>
        /// Gets the application scopes.
        /// </summary>
        ApplicationScopeCollection ApplicationScopes { get; }
    }
}
