namespace BusinessLogic.GeneralOptions.Admin
{
    /// <summary>
    /// Defines a <see cref="IAdminOptions"/> and it's members
    /// </summary>
    public interface IAdminOptions
    {
        /// <summary>
        /// Gets or sets the main administrator employee Id.
        /// </summary>
        int MainAdministrator { get; set; }
    }
}
