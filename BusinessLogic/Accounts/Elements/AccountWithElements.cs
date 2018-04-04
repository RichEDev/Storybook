namespace BusinessLogic.Accounts.Elements
{
    using System;
    using System.Collections.Generic;

    using BusinessLogic.Databases;

    /// <summary>
    /// Defines a <see cref="AccountWithElements"/> and it's members
    /// </summary>
    [Serializable]
    public class AccountWithElements : IAccountWithElement
    {
        /// <summary>
        /// Create a new instance of the <see cref="AccountWithElements"/>
        /// </summary>
        /// <param name="accountId">Id of the base <see cref="Account"/></param>
        /// <param name="databaseCatalogue"><see cref="IDatabaseCatalogue"/> of the base <see cref="Account"/></param>
        /// <param name="archived">Archived state of the base <see cref="Account"/></param>
        /// <param name="elements">A <see cref="IList{T}"/> of <see cref="IElement"/> for the <see cref="AccountWithElements"/></param>
        public AccountWithElements(int accountId, IDatabaseCatalogue databaseCatalogue, bool archived, IList<IElement> elements)
        {
            this.LicencedElements = elements;
            this.Id = accountId;
            this.DatabaseCatalogue = databaseCatalogue;
            this.Archived = archived;
        }

        /// <summary>
        /// Gets or sets the Id for this <see cref="AccountWithElements"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the archived state for this <see cref="AccountWithElements"/>.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets the <see cref="IDatabaseCatalogue"/> for this <see cref="AccountWithElements"/>.
        /// </summary>
        public IDatabaseCatalogue DatabaseCatalogue { get; }

        /// <summary>
        /// Gets the <see cref="IList{IElement}"/> for this <see cref="AccountWithElements"/>.
        /// </summary>
        public IList<IElement> LicencedElements { get; }
    }
}
