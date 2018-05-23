namespace BusinessLogic.Interfaces
{
    /// <summary>
    /// Defines that an object can have a description.
    /// </summary>
    public interface IDescription
    {
        /// <summary>
        /// Gets or sets a value indicating the description of this <see cref="IDescription"/>.
        /// </summary>
        string Description { get; set; }
    }
}
