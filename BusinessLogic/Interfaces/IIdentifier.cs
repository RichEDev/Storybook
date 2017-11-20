namespace BusinessLogic.Interfaces
{
    /// <summary>
    /// Defines that the object has an ID field.
    /// </summary>
    /// <typeparam name="T">The type of the identifier</typeparam>
    public interface IIdentifier<T>
    {
        /// <summary>
        /// Gets or sets the identifier for <see cref="T"/>
        /// </summary>
        T Id { get; set; }
    }
}