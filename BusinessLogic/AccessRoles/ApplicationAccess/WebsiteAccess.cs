namespace BusinessLogic.AccessRoles.ApplicationAccess
{
    using System;

    /// <summary>
    /// Indicates an access to the website should be granted.
    /// </summary>
    [Serializable]
    public class WebsiteAccess : IApplicationAccessScope
    {
    }
}
