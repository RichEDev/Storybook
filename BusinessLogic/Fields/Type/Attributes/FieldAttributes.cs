namespace BusinessLogic.Fields.Type.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// A list of attributes for a specific <see cref="IField"/>.
    /// </summary>
    public class FieldAttributes
    {
        /// <summary>
        /// An internal list of <see cref="IFieldAttribute"/>
        /// </summary>
        private readonly List<IFieldAttribute> _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAttributes"/> class.
        /// </summary>
        public FieldAttributes()
        {
            this._attributes = new List<IFieldAttribute>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldAttributes"/> class.
        /// </summary>
        /// <param name="attributes">
        /// The attributes to seed the list with.
        /// </param>
        public FieldAttributes(List<IFieldAttribute> attributes)
        {
            this._attributes = attributes;
        }

        /// <summary>
        /// Add an entry to the list.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to add.
        /// </param>
        public void Add(IFieldAttribute attribute)
        {
            this._attributes.Add(attribute);
        }

        /// <summary>
        /// Get a specific entry from the list.
        /// </summary>
        /// <param name="fileType">
        /// The file type.
        /// </param>
        /// <returns>
        /// The <see cref="IFieldAttribute"/>.
        /// </returns>
        public IFieldAttribute Get(Type fileType)
        {
            return this._attributes.FirstOrDefault(fieldAttribute => fieldAttribute.GetType() == fileType);
        }

        /// <summary>
        /// Get the enumerator for ths list.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<IFieldAttribute>.Enumerator GetEnumerator()
        {
            return this._attributes.GetEnumerator();
        }

        /// <summary>
        /// Check if the list has an entry of the given type..
        /// </summary>
        /// <param name="fileType">
        /// The file type to check for.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> if the list contains that type.
        /// </returns>
        public bool Has(Type fileType)
        {
            return this._attributes.Any(fieldAttribute => fieldAttribute.GetType() == fileType);
        }
    }
}