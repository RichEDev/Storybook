namespace BusinessLogic.JoinVia
{
    using System;

    /// <summary>
    /// An implentation of <see cref="IJoinVia"/>.
    /// </summary>
    public class JoinVia : IJoinVia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinVia"/> class. 
        /// </summary>
        /// <param name="id">
        /// The identifier for <see cref="IJoinVia"/>
        /// </param>
        /// <param name="description">
        /// The description of the Join
        /// </param>
        /// <param name="alias">
        /// The "alias" to use when constructing <see cref="IQueryBuilder"/>
        /// </param>
        /// <param name="joinViaList">
        /// The <see cref="JoinViaParts"/> for this join
        /// </param>
        public JoinVia(int id, string description, Guid alias, JoinViaParts joinViaList)
        {
            this.Id = id;
            this.Description = description;
            this.Alias = alias;
            this.JoinViaList = joinViaList;
        }

        /// <summary>
        /// Gets the description of the Join
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the "alias" to use when constructing <see cref="IQueryBuilder"/>
        /// </summary>
        public Guid Alias { get; }

        /// <summary>
        /// Gets the <see cref="JoinViaParts"/> for this join
        /// </summary>
        public JoinViaParts JoinViaList { get; }

        /// <summary>
        /// Gets or sets the identifier for <see cref="IJoinVia"/>
        /// </summary>
        public int Id { get; set; }
    }
}
