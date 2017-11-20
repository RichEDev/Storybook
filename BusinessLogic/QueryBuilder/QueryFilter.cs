namespace BusinessLogic.QueryBuilder
{
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.JoinVia;

    /// <summary>
    /// An implementation of <see cref="IQueryFilter"/> that includes an <see cref="IQueryField"/> and values to check against.
    /// </summary>
    public class QueryFilter : IQueryFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFilter"/> class. 
        /// </summary>
        /// <param name="field">
        /// The <see cref="IField"/> to query against
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
        /// <param name="joiner">
        /// The <see cref="IJoiner"/> that defines the query element as AND or OR.
        /// </param>
        /// <param name="value1">
        /// The value to query against.
        /// </param>
        /// <param name="value2">
        /// The second value (used for between).
        /// </param>
        /// <param name="index">
        /// The index of the <see cref="IQueryFilter"/>
        /// </param>
        public QueryFilter(IField field, string alias, JoinVia joinVia, bool useListItemText, IJoiner joiner, object[] value1, object[] value2, int index)
        {
            this.Field = field;
            this.Alias = alias;
            this.JoinVia = joinVia;
            this.UseListItemText = useListItemText;
            this.Joiner = joiner;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Index = index;
        }

        public QueryFilter(IQueryField queryField, IJoiner joiner, object[] value1, object[] value2)
        {
            this.Field = queryField.Field;
            this.Alias = queryField.Alias;
            this.JoinVia = queryField.JoinVia;
            this.UseListItemText = queryField.UseListItemText;
            this.Joiner = joiner;
            this.Value1 = value1;
            this.Value2 = value2;
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

        /// <summary>
        /// Gets or sets the <see cref="IJoiner"/> that defines the query element as AND or OR.
        /// </summary>
        public IJoiner Joiner { get; set; }

        /// <summary>
        /// Gets or sets the value to query against.
        /// </summary>
        public object[] Value1 { get; set; }

        /// <summary>
        /// Gets or sets the second value (used for between).
        /// </summary>
        public object[] Value2 { get; set; }

        /// <summary>
        /// Gets or sets the index of the <see cref="IQueryFilter"/>
        /// </summary>
        public int Index { get; set; }
    }
}