namespace BusinessLogic.AccessRoles
{
    using ReportsAccess;
    using CustomEntities.AccessRoles;
    using Interfaces;

    /// <summary>
    /// Interface defining common fields of an AccessRole.
    /// </summary>
    public interface IAccessRole : IIdentifier<int>
    {
        /// <summary>
        /// Gets the <see cref="AccessLevel">AccessLevel</see> for the <see cref="ReportsAccess">AccessRole</see>
        /// </summary>
        IReportsAccess AccessLevel { get; }

        /// <summary>
        /// Gets the AllowApiAccess for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        bool AllowApiAccess { get; }

        /// <summary>
        /// Gets the AllowMobileAccess for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        bool AllowMobileAccess { get; }

        /// <summary>
        /// Gets the AllowWebsiteAccess for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        bool AllowWebsiteAccess { get; }

        /// <summary>
        /// Gets the <see cref="CustomEntityElementAccessLevelCollection">CustomEntityElementAccessLevelCollection</see> for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        CustomEntityElementAccessLevelCollection CustomEntityAccess { get; }

        /// <summary>
        /// Gets the Description for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the <see cref="ElementAccessCollection">ElementAccessCollection</see> for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        ElementAccessCollection ElementAccess { get; }

        /// <summary>
        /// Gets the Name for the <see cref="AccessRole">AccessRole</see>
        /// </summary>
        string Name { get; }
    }
}