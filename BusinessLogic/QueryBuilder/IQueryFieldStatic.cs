namespace BusinessLogic.QueryBuilder
{
    /// <summary>
    /// A <see cref="IQueryField"/> that is a static value.
    /// </summary>
    public interface IQueryFieldStatic : IQueryField
    {
        string Value { get; set; }
    }
}