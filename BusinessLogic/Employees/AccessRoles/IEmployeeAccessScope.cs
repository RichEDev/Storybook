namespace BusinessLogic.Employees.AccessRoles
{
    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.ReportsAccess;
    using BusinessLogic.AccessRoles.Scopes;
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Defines the members for an <see cref="IEmployeeAccessScope"/> to expose the highest permissions 
    /// granted to a user from all their assigned <see cref="IAssignedAccessRole"/> objects.
    /// </summary>
    public interface IEmployeeAccessScope : IIdentifier<string>, IApplicationAccess, IAccessScopes, IReportAccessScopes
    {
    }
}
