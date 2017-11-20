namespace BusinessLogic.QueryBuilder
{
    using BusinessLogic.Fields.Type.Base;
    using BusinessLogic.JoinVia;

    /// <summary>
    /// Defines the implementation of a <see cref="IQueryField"/>.
    /// </summary>
    public interface IQueryField 
    {
        /// <summary>
        /// The <see cref="IField"/>used to query against.
        /// </summary>
        IField Field { get; set; }

        /// <summary>
        /// The alias used when generating queries.
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// The <see cref="JoinVia"/> used to obtain the data
        /// </summary>
        JoinVia JoinVia { get; set; }

        /// <summary>
        /// True if the query should use the List Item Text and not the Value (ID).
        /// </summary>
        bool UseListItemText { get; set; }
    }
}