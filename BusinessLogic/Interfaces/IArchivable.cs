namespace BusinessLogic.Interfaces
{
    /// <summary>
    /// Defines that an object can be archived.
    /// </summary>
    public interface IArchivable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IArchivable"/> is archived.
        /// </summary>
        bool Archived { get; set; }
    }
}