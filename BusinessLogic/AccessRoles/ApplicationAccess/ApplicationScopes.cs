namespace BusinessLogic.AccessRoles.ApplicationAccess
{
    using System;

    /// <summary>
    /// A collection of <see cref="IApplicationAccessScope"/> objects.
    /// </summary>
    [Serializable]
    public class ApplicationScopeCollection : ListWrapper<IApplicationAccessScope>
    {
    }
}
