namespace BusinessLogic.QueryBuilder.Conditions
{
    /// <summary>
    /// The condition repository abstract class.
    /// </summary>
    public abstract class ConditionRepository
    {
        /// <summary>
        /// Create a new condition.
        /// </summary>
        /// <param name="field">
        /// The Query Filter field to base the condition on.
        /// </param>
        /// <typeparam name="T">
        /// The type of condition <see cref="ICondition"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="ICondition"/> based on the <see cref="IQueryField"/> and the given type.
        /// </returns>
        public abstract ICondition New<T>(IQueryFilter field) where T : ICondition;
    }
}
