namespace BusinessLogic.AccessRoles.ReportsAccess
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// <see cref="LinkedAccessRoleCollections">LinkedAccessRoleCollections</see> defines a backing collection of <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see>
    /// </summary>
    public class LinkedAccessRoleCollections
    {
        /// <summary>
        /// The _backing collection of Linked Access Roles collections.
        /// </summary>
        private readonly List<LinkedAccessRoleCollection> _backingCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedAccessRoleCollections"/> class. 
        /// </summary>
        public LinkedAccessRoleCollections()
        {
            this._backingCollection = new List<LinkedAccessRoleCollection>();
        }

        /// <summary>
        /// Gets the <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see> for the supllied primaryAccessRoleId
        /// </summary>
        /// <param name="primaryAccessRoleId">The primaryAccessRoleId</param>
        /// <returns>The <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see>LinkedAccessRoleCollection</returns>
        public LinkedAccessRoleCollection this[int primaryAccessRoleId]
        {
            get { return this._backingCollection.FirstOrDefault(x => x.PrimaryAccessRoleId == primaryAccessRoleId); }
        }

        /// <summary>
        /// Returns whether an primaryAccessRoleID is in the backing collection
        /// </summary>
        /// <param name="primaryAccessRoleId">The primary Access Role ID to check for</param>
        /// <returns>The result of the lookup</returns>
        public bool Contains(int primaryAccessRoleId)
        {
            return this._backingCollection.Any(x => x.PrimaryAccessRoleId == primaryAccessRoleId);
        }

        /// <summary>
        /// Adds a <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see> to the backing collection
        /// </summary>
        /// <param name="linkedAccessRoleCollection">
        /// The linked Access Role Collection to add.
        /// </param>
        public void Add(LinkedAccessRoleCollection linkedAccessRoleCollection)
        {
            this._backingCollection.Add(linkedAccessRoleCollection);
        }
    }
}
