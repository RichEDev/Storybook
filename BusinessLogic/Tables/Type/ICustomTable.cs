namespace BusinessLogic.Tables.Type
{
    /// <summary>
    /// Allows implementation of a <see cref="ICustomTable"/>
    /// </summary>
    public interface ICustomTable
    {
        /// <summary>
        /// Gets or sets the <see cref="ICustomTable"/> Account ID.
        /// </summary>
        int AccountId { get; set; }
    }
}