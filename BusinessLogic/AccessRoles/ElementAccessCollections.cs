namespace BusinessLogic.AccessRoles
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// <see cref="ElementAccessCollections">ElementAccessCollections</see> defines a backing collection of a list of <see cref="ElementAccessCollection">ElementAccessCollection</see>
    /// </summary>
    public class ElementAccessCollections
    {
        /// <summary>
        /// The _backing collection of element access collections.
        /// </summary>
        private readonly IList<ElementAccessCollection> _backingCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAccessCollections"/> class. 
        /// </summary>
        public ElementAccessCollections()
        {
            this._backingCollection = new List<ElementAccessCollection>();
        }

        /// <summary>
        /// Gets a <see cref="ElementAccessCollection">ElementAccessCollection</see> by its accessRoleId
        /// </summary>
        /// <param name="accessRoleId">The accessRoleId</param>
        /// <returns>The <see cref="ElementAccessCollection">ElementAccessCollection</see></returns>
        public ElementAccessCollection this[int accessRoleId]
        {
            get
            {
                return this._backingCollection.FirstOrDefault(collection => collection.AccessRoleId == accessRoleId);
            }
        }

        /// <summary>
        /// Returns whether the accessRoleId is in the backing collection
        /// </summary>
        /// <param name="accessRoleId">The accessRoleId</param>
        /// <returns>The result of the lookup</returns>
        public bool Contains(int accessRoleId)
        {
            return this._backingCollection.Any(collection => collection.AccessRoleId == accessRoleId);
        }

        /// <summary>
        /// Adds a <see cref="ElementAccessCollection">ElementAccessCollection</see> to the backing collection
        /// </summary>
        /// <param name="elementAccessCollection">The <see cref="ElementAccessCollection">ElementAccessCollection</see></param>
        public void Add(ElementAccessCollection elementAccessCollection)
        {
            this._backingCollection.Add(elementAccessCollection);
        }
    }
}
