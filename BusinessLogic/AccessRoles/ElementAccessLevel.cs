namespace BusinessLogic.AccessRoles
{
    /// <summary>
    /// <see cref="ElementAccessLevel"/> defines an element access level
    /// </summary>
    public class ElementAccessLevel : IAccessLevel<int>
    {
        /// <summary>
        /// The private indicator or view access..
        /// </summary>
        private bool _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAccessLevel"/> class. 
        /// </summary>
        /// <param name="elementId">
        /// The elementId
        /// </param>
        /// <param name="view">
        /// Whether the element has View permissions
        /// </param>
        /// <param name="add">
        /// Whether the element has Add permissions
        /// </param>
        /// <param name="edit">
        /// Whether the element has Edit permissions
        /// </param>
        /// <param name="delete">
        /// Whether the element has Delete permissions
        /// </param>
        public ElementAccessLevel(int elementId, bool view, bool add, bool edit, bool delete)
        {
            this.Id = elementId;
            this.Add = add;
            this.Edit = edit;
            this.Delete = delete;
            this._view = view;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the element has Add permissions
        /// </summary>
        public bool Add { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element has Delete permissions
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element has Edit permissions
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// Gets or sets the Id for the element
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element has View permissions
        /// </summary>
        public bool View
        {
            get
            {
                if (this.Add || this.Edit || this.Delete)
                {
                    return true;
                }

                return this._view;
            }

            set
            {
                this._view = value;
            }
        }
    }
}
