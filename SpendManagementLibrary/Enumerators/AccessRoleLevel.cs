/// <summary>
/// The access level determines what data can be viewed
/// </summary>
public enum AccessRoleLevel
{
    /// <summary>
    /// All employees whome this employee currently signs off
    /// </summary>
    EmployeesResponsibleFor=1,
    /// <summary>
    /// The selected access roles
    /// </summary>
    SelectedRoles=2,
    /// <summary>
    /// All data in the database
    /// </summary>
    AllData=3
}