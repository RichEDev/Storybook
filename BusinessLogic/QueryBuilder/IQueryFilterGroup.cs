namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// Defines a Filter group used in <see cref="IQueryBuilder"/>
    /// </summary>
    public interface IQueryFilterGroup
    {
        /// <summary>
        /// Gets a <see cref="IQueryBuilder"/> for this <see cref="IQueryFilterGroup"/>
        /// </summary>
        QueryBuilder QueryFilters { get; }
    }

    /// <summary>
    /// Defines a Query Group with an AND join
    /// </summary>
    public interface IQueryFilterGrouppAnd : IQueryFilterGroup
    {
        
    }

    /// <summary>
    /// Defines a Query Group with an OR join
    /// </summary>
    public interface IQueryFilterGroupOr : IQueryFilterGroup
    {
        
    }

    /// <summary>
    /// Defines a Query Group with an ASWELLAS join
    /// </summary>
    public interface IQueryFilterGroupAsWellAs : IQueryFilterGroup
    {

    }
}
