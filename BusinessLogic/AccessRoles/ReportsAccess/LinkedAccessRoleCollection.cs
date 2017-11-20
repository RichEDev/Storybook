namespace BusinessLogic.AccessRoles.ReportsAccess
{
    using System.Collections.Generic;

    /// <summary> 
    /// <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see> defines a backing collection of accessRoleIds
    /// </summary>
    public class LinkedAccessRoleCollection
    {
        /// <summary>
        /// Gets or sets the PrimaryAccessRoleID of the AccessRole
        /// </summary>
        public int PrimaryAccessRoleId { get; protected set; }

        private readonly IList<int> _backingCollection;

        /// <summary>
        /// Initialises an instance of <see cref="LinkedAccessRoleCollection">LinkedAccessRoleCollection</see>
        /// </summary>
        /// <param name="primaryAccessRoleId"></param>
        public LinkedAccessRoleCollection(int primaryAccessRoleId)
        {
            this.PrimaryAccessRoleId = primaryAccessRoleId;
            this._backingCollection = new List<int>();
        }

        /// <summary>
        /// Adds an access role Id to the backing collection
        /// </summary>
        /// <param name="accessRoleId"></param>
        public void Add(int accessRoleId)
        {
            this._backingCollection.Add(accessRoleId);
        }

        /// <summary>
        /// Returns whether an accessRoleId is in the backing collection
        /// </summary>
        /// <param name="accessRoleId">The access role Id to check for</param>
        /// <returns>The result of the lookup</returns>
        public bool Contains(int accessRoleId)
        {
            return this._backingCollection.Contains(accessRoleId);
        }

        /// <summary>
        /// Gets the count of the number of accessRoleIds in the backing collection
        /// </summary>
        public int Count => this._backingCollection.Count;
    }
}
