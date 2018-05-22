namespace BusinessLogic.AccessRoles
{
    using System;

    using BusinessLogic.AccessRoles.ApplicationAccess;
    using BusinessLogic.AccessRoles.Scopes;

    using ReportsAccess;

    /// <summary>
    /// <see cref="AccessRole">AccessRole</see> defines access rights to elements of the system.
    /// </summary>
    [Serializable]
    public class AccessRole : IAccessRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRole"/> class. 
        /// </summary>
        /// <param name="id">The unique id Of the <see cref="AccessRole"/></param>
        /// <param name="name">The name of the <see cref="AccessRole"/></param>
        /// <param name="description">The description of the AccessRole.</param>
        /// <param name="applicationScopes">The <see cref="ApplicationScopeCollection"/> for this <see cref="AccessRole"/>.</param>
        /// <param name="reportAccessLevel">The <see cref="IReportsAccess">IReportAccess</see>the <see cref="AccessRole"/> has.</param>
        /// <param name="accessScopeCollection">The <see cref="AccessScopeCollection" /> for this <see cref="AccessRole"/>.</param>
        public AccessRole(int id, string name, string description, ApplicationScopeCollection applicationScopes, IReportsAccess reportAccessLevel, AccessScopeCollection accessScopeCollection)
        {
            Guard.ThrowIfNull(applicationScopes, nameof(applicationScopes));
            Guard.ThrowIfNull(reportAccessLevel, nameof(reportAccessLevel));
            Guard.ThrowIfNull(accessScopeCollection, nameof(accessScopeCollection));

            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.ApplicationScopes = applicationScopes;
            this.ReportsAccess = reportAccessLevel;
            this.AccessScopes = accessScopeCollection;
        }

        /// <inheritdoc />
        public ApplicationScopeCollection ApplicationScopes { get; protected set; }
        
        /// <summary>
        /// Gets or sets the <see cref="AccessRole"/> description.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Gets or sets the unique identifier <see cref="AccessRole"/> ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AccessRole"/> Name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="IReportsAccess"/> for this <see cref="AccessRole"/>.
        /// </summary>
        public IReportsAccess ReportsAccess { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="AccessRole"/> <see cref="AccessScopeCollection" />.
        /// </summary>
        public AccessScopeCollection AccessScopes { get; protected set; }
    }
}
