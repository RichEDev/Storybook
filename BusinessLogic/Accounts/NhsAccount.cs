namespace BusinessLogic.Accounts
{
    using System;

    using BusinessLogic.Databases;

    /// <summary>
    /// <see cref="NhsAccount"/> .
    /// </summary>
    [Serializable]
    public class NhsAccount : IAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NhsAccount"/> class.
        /// </summary>
        /// <param name="accountId">The unique identifier for this <see cref="IAccount"/>.</param>
        /// <param name="databaseCatalogue">An instance of <see cref="IDatabaseCatalogue"/> which stores all customer data regarding this <see cref="IAccount"/>.</param>
        /// <param name="archived">A bool indicating whether or not this <see cref="Account"/> is archived or not.</param>
        public NhsAccount(int accountId, IDatabaseCatalogue databaseCatalogue, bool archived)
        {
            this.DatabaseCatalogue = databaseCatalogue;
            this.Id = accountId;
            this.Archived = archived;
        }

        /// <summary>
        /// Gets an instance of <see cref="IDatabaseCatalogue"/> which stores all customer data regarding this <see cref="IAccount"/>.
        /// </summary>
        public IDatabaseCatalogue DatabaseCatalogue { get; }

        /// <summary>
        /// Gets or sets the <see cref="IAccount"/> unique identifier <c>Id</c>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not this <see cref="Account"/> is archived.
        /// </summary>
        public bool Archived { get; set; }

    }
}
