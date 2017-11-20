namespace BusinessLogic.AccessRoles
{
    using System.Collections.Generic;
    using Accounts.Elements;

    /// <summary>
    /// <see cref="ElementAccessCollection"/> defines a backing collection of <see cref="ModuleElements" /> with their respective <see cref="IAccessLevel{T}" />
    /// </summary>
    public class ElementAccessCollection
    {
        /// <summary>
        /// The backing collection for this <see cref="ElementAccessCollection" />.
        /// </summary>
        private readonly IDictionary<ModuleElements, IAccessLevel<int>> _elementAccessCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementAccessCollection"/> class. 
        /// </summary>
        /// <param name="accessRoleId">
        /// The Id of the <see cref="IAccessRole"/>
        /// </param>
        public ElementAccessCollection(int accessRoleId)
        {
            this._elementAccessCollection = new Dictionary<ModuleElements, IAccessLevel<int>>();
            this.AccessRoleId = accessRoleId;
        }

        /// <summary>
        /// Gets the Id of an <see cref="IAccessRole"/>
        /// </summary>
        public int AccessRoleId { get; }

        /// <summary>
        /// Returns whether a <see cref="ModuleElements" /> is in the <see cref="ElementAccessCollection"/>
        /// </summary>
        /// <param name="element">The <see cref="ModuleElements" />to check for</param>
        /// <returns>Determines whether or not the <see cref="ElementAccessCollection"/> contains this element.</returns>
        public bool Contains(ModuleElements element)
        {
            return this._elementAccessCollection.ContainsKey(element);
        }

        /// <summary>
        /// Adds a <see cref="ElementAccessLevel" /> of the <see cref="moduleElement" /> to the <see cref="ElementAccessCollection"/>.
        /// </summary>
        /// <param name="moduleElement">The <see cref="moduleElement" /></param>
        /// <param name="elementAccessLevel">The <see cref="ElementAccessLevel" /> of the <see cref="moduleElement" /></param>
        public void Add(ModuleElements moduleElement, ElementAccessLevel elementAccessLevel)
        {
            this._elementAccessCollection.Add(moduleElement, elementAccessLevel);
        }
    }
}
