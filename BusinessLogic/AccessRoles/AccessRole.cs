namespace BusinessLogic.AccessRoles
{
    using CustomEntities.AccessRoles;

    using ReportsAccess;

    /// <summary>
    /// <see cref="AccessRole">AccessRole</see> defines access rights to elements of the system.
    /// </summary>
    public class AccessRole : IAccessRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessRole"/> class. 
        /// </summary>
        /// <param name="id">
        /// The unique id Of the AccessRole
        /// </param>
        /// <param name="name">
        /// The name of the AccessRole
        /// </param>
        /// <param name="description">
        /// The description of the AccessRole
        /// </param>
        /// <param name="allowApiAccess">
        /// Whether the AccessRole has access to the API
        /// </param>
        /// <param name="allowMobileAccess">
        /// Whether the AccessRole has access to the Mobile App
        /// </param>
        /// <param name="allowWebsiteAccess">
        /// Whether the AccessRole has access to the Web APP
        /// </param>
        /// <param name="accessLevel">
        /// The <see cref="IReportsAccess">IReportAccess</see>the AccessRole has
        /// </param>
        /// <param name="elementAccess">
        /// The <see cref="ElementAccessCollection">ElementAccessCollection the AccessRole</see>
        /// </param>
        public AccessRole(int id, string name, string description, bool allowApiAccess, bool allowMobileAccess, bool allowWebsiteAccess, IReportsAccess accessLevel, ElementAccessCollection elementAccess)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.AllowApiAccess = allowApiAccess;
            this.AllowMobileAccess = allowMobileAccess;
            this.AllowWebsiteAccess = allowWebsiteAccess;
            this.AccessLevel = accessLevel;
            this.ElementAccess = elementAccess;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AccessRole has access to the API.
        /// </summary>
        public bool AllowApiAccess { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the AccessRole has access to the Mobile App.
        /// </summary>
        public bool AllowMobileAccess { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="AccessRole"/> has access to the Web App.
        /// </summary>
        public bool AllowWebsiteAccess { get; protected set; }

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
        /// Gets or sets the <see cref="AccessRole"/> description.
        /// </summary>
        public IReportsAccess AccessLevel { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="AccessRole"/> <see cref="ElementAccessCollection">ElementAccessCollection</see>ElementAccessCollection.
        /// </summary>
        public ElementAccessCollection ElementAccess { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="AccessRole"/> <see cref="CustomEntityElementAccessLevelCollection" />.
        /// </summary>
        public CustomEntityElementAccessLevelCollection CustomEntityAccess { get; protected set; }
    }
}
