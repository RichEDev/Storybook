namespace BusinessLogic.P11DCategories
{
    using System;

    /// <summary>
    /// Defines a basic <see cref="P11DCategory"/> and its members.
    /// </summary>
    [Serializable]
    public class P11DCategory : IP11DCategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="P11DCategory"/> class.
        /// </summary>
        /// <param name="id">The id of the <see cref="P11DCategory"/></param>
        /// <param name="name">The name of the <see cref="P11DCategory"/></param>
        public P11DCategory(int id, string name)
        {
            Guard.ThrowIfNullOrWhiteSpace(name, nameof(name));

            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the id for this <see cref="P11DCategory"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this <see cref="P11DCategory"/>.
        /// </summary>
        public string Name { get; set; }
    }
}