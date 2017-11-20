namespace BusinessLogic.QueryBuilder.QueryFields
{
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.JoinVia;

    /// <summary>
    /// An instance of <see cref="IQueryField"/>.
    /// </summary>
    public class Queryfield : IQueryField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queryfield"/> class. 
        /// </summary>
        /// <param name="field">
        /// The <see cref="IField"/>used to query against.
        /// </param>
        /// <param name="alias">
        /// The alias used when generating queries.
        /// </param>
        /// <param name="joinVia">
        /// The <see cref="IQueryField.JoinVia"/> used to obtain the data
        /// </param>
        /// <param name="useListItemText">
        /// True if the query should use the List Item Text and not the Value (ID).
        /// </param>
        public Queryfield(IField field, string alias, JoinVia joinVia, bool useListItemText)
        {
            this.Field = field;
            this.Alias = alias;
            this.JoinVia = joinVia;
            this.UseListItemText = useListItemText;
        }

        /// <summary>
        /// Gets or sets the <see cref="IField"/>used to query against.
        /// </summary>
        public IField Field { get; set; }

        /// <summary>
        /// Gets or sets the alias used when generating queries.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IQueryField.JoinVia"/> used to obtain the data
        /// </summary>
        public JoinVia JoinVia { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the query should use the List Item Text and not the Value (ID).
        /// </summary>
        public bool UseListItemText { get; set; }
    }
}
