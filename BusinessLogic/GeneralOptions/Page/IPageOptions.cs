namespace BusinessLogic.GeneralOptions.Page
{
    /// <summary>
    /// Defines a <see cref="IPageOptions"/> and it's members
    /// </summary>
    public interface IPageOptions
    {
        /// <summary>
        /// Gets or sets the default page size
        /// </summary>
        int DefaultPageSize { get; set; }
    }
}
