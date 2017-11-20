namespace BusinessLogic.ProjectCodes
{
    using System;

    /// <summary>
    /// Defines a basic <see cref="ProjectCode"/> and its members.
    /// </summary>
    [Serializable]
    public class ProjectCode : IProjectCode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCode"/> class. 
        /// </summary>
        /// <param name="id">The ID of the <see cref="ProjectCode"/></param>
        /// <param name="name">The name of the <see cref="ProjectCode"/></param>
        /// <param name="description">The description of the <see cref="ProjectCode"/></param>
        /// <param name="archived">True if the <see cref="ProjectCode"/> is archived.</param>
        /// <param name="rechargeable">True if the <see cref="ProjectCode"/> is rechargable.</param>
        public ProjectCode(int id, string name, string description, bool archived, bool rechargeable)
        {
            this.Id = id;
            this.Archived = archived;
            this.Name = name;
            this.Description = description;
            this.Rechargeable = rechargeable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCode"/> class.
        /// </summary>
        /// <remarks>Used for creaing default instances.</remarks>
        public ProjectCode()
        {
            
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether archived.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the description for this <see cref="ProjectCode"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="ProjectCode"/> is rechargeable.
        /// </summary>
        public bool Rechargeable { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="ProjectCode"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get either the description or the name dependant on <paramref name="useDescription"/>.
        /// </summary>
        /// <param name="useDescription">A <see cref="bool"/> determining if the description or name should be used.</param>
        /// <returns>Either the description or name for this <see cref="ProjectCode"/></returns>
        public string ToString(bool useDescription)
        {
            return useDescription ? this.Description : this.Name;
        }
    }
}
