namespace BusinessLogic.Accounts
{
    using BusinessLogic.Databases;
    using BusinessLogic.Interfaces;

    /// <summary>
    /// <see cref="Account"/> defines an entry in registered users and its members.
    /// </summary>
    public interface IAccount : IIdentifier<int>, IArchivable
    {
        /// <summary>
        /// Gets an instance of <see cref="IDatabaseCatalogue"/> which stores all customer data regarding this <see cref="IAccount"/>.
        /// </summary>
        IDatabaseCatalogue DatabaseCatalogue { get; }
    }
}
