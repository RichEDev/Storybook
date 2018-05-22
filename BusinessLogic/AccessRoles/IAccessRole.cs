namespace BusinessLogic.AccessRoles
{
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.Scopes;

    using Interfaces;

    using ReportsAccess;

    /// <summary>
    /// Interface defining common fields of an AccessRole.
    /// </summary>
    public interface IAccessRole : IIdentifier<int>
    {
        /// <summary>
        /// Gets the <see cref="IReportsAccess" /> for this <see cref="IAccessRole"/>.
        /// </summary>
        IReportsAccess ReportsAccess { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationScopeCollection"/> specifying what applications this <see cref="IAccessRole"/> grants access to.
        /// </summary>
        ApplicationScopeCollection ApplicationScopes { get; }
        
        /// <summary>
        /// Gets the Description for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the <see cref="AccessScopeCollection" /> for the <see cref="IAccessRole">AccessRole</see>
        /// </summary>
        AccessScopeCollection AccessScopes { get; }

        /// <summary>
        /// Gets the Name for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        string Name { get; }
    }
}