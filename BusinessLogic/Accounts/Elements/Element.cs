namespace BusinessLogic.Accounts.Elements
{
    /// <summary>
    /// <see cref="Element">Element</see> defines 
    /// </summary>
    public class Element : IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class. 
        /// </summary>
        /// <param name="id">
        /// The Id of the Element
        /// </param>
        /// <param name="categoryId">
        /// The Id of the category that the Element belongs to
        /// </param>
        /// <param name="name">
        /// The name of the Element
        /// </param>
        /// <param name="description">
        /// The decription of the Element
        /// </param>
        /// <param name="accessRolesCanEdit">
        /// True if Can edit
        /// </param>
        /// <param name="accessRolesCanAdd">
        /// True if Can add
        /// </param>
        /// <param name="accessRolesCanDelete">
        /// True if Can delete
        /// </param>
        /// <param name="friendlyName">
        /// The friendly name of the Element
        /// </param>
        /// <param name="accessRolesApplicable">
        /// The access roles used for this element.
        /// </param>
        public Element(int id, int categoryId, string name, string description, bool accessRolesCanEdit, bool accessRolesCanAdd, bool accessRolesCanDelete, string friendlyName, bool accessRolesApplicable)
        {
            this.Id = id;
            this.CategoryId = categoryId;
            this.Name = name;
            this.Description = description;
            this.AccessRolesCanEdit = accessRolesCanEdit;
            this.AccessRolesCanAdd = accessRolesCanAdd;
            this.AccessRolesCanDelete = accessRolesCanDelete;
            this.FriendlyName = friendlyName;
            this.AccessRolesApplicable = accessRolesApplicable;
        }

        /// <summary>
        /// Gets or sets the Id of the Element
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the Id of the category that the Element belongs to
        /// </summary>
        public int CategoryId { get; }

        /// <summary>
        /// Gets  the name of the Element
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the Element
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether the can associated Access Roles can Edit the Element
        /// </summary>
        public bool AccessRolesCanEdit { get; }

        /// <summary>
        /// Gets a value indicating whether the can associated Access Roles can Add to the Element
        /// </summary>
        public bool AccessRolesCanAdd { get; }

        /// <summary>
        /// Gets a value indicating whether the can associated Access Roles can Delete the Element
        /// </summary>
        public bool AccessRolesCanDelete { get; }

        /// <summary>
        /// Gets the friendly name of the Element
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Gets a value indicating whether Access Roles are applicable to the Element
        /// </summary>
        public bool AccessRolesApplicable { get; }
    }
}
