namespace BusinessLogic.AccessRoles.ApplicationAccess
{
    using System;

    /// <summary>
    /// Indicates an access to the api should be granted.
    /// </summary>
    [Serializable]
    public class ApiAccess : IApplicationAccessScope
    {
    }
}
