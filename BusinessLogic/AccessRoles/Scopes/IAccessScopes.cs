namespace BusinessLogic.AccessRoles.Scopes
{
    /// <summary>
    /// Defines that the <see cref="AccessScopeCollection"/> Scopes property must be implemented.
    /// </summary>
    public interface IAccessScopes
    {
        /// <summary>
        /// Gets the <see cref="AccessScopeCollection"/> specifying what access this <see cref="AccessRole"/> grants.
        /// </summary>
        AccessScopeCollection Scopes { get; }
    }
}
