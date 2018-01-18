namespace BusinessLogic.P11DCategories
{
    using BusinessLogic.Interfaces;

    /// <summary>
    /// Interface defining common field of a P11D category.
    /// </summary>
    public interface IP11DCategory : IIdentifier<int>
    {
        /// <summary>
        /// Gets or sets the name for this <see cref="IP11DCategory"/>.
        /// </summary>
        string Name { get; set; }
    }
}