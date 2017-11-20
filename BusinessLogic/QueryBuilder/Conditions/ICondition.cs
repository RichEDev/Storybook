using BusinessLogic.Tables.Type;

namespace BusinessLogic.QueryBuilder.Conditions
{
    /// <summary>
    /// A defininition of a <see cref="ICondition"/> used in <seealso cref="IQueryBuilder"/>
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Return the name of the <see cref="IQueryField"/> within the <see cref="ICondition"/> 
        /// </summary>
        /// <returns>The field name.</returns>
        string GetFieldName();

        /// <summary>
        /// Return the name of the <see cref="ITable"/> within the <see cref="ICondition"/> 
        /// </summary>
        /// <returns>The table name.</returns>
        string GetTableName();

        /// <summary>
        /// Return the where field from the <see cref="ICondition"/>
        /// </summary>
        /// <returns>The where field.</returns>
        string WhereField();

        /// <summary>
        /// Return the operator of the <see cref="IQueryField"/> within the <see cref="ICondition"/> 
        /// </summary>
        /// <returns>The field name.</returns>
        string Operator();

        /// <summary>
        /// Clone the <see cref="ICondition"/> 
        /// </summary>
        /// <returns>A new object with the same properties as the parent.</returns>
        ICondition Clone();

        /// <summary>
        /// Get or set the <see cref="IQueryFilter"/> from the <see cref="ICondition"/>
        /// </summary>
        IQueryFilter QueryFilter { get; set; }

    }
}
