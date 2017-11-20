using System.Collections.Generic;

namespace BusinessLogic.Fields.Type
{
    /// <summary>
    ///  Emumerable class for storing and accessing <see cref="Dictionary{TKey,TValue}"/> which are the list item values for a speciaic field.
    /// </summary>
    public class ListItemValues 
    {
        /// <summary>
        /// The internal _list of item values.
        /// </summary>
        private readonly Dictionary<int, object> _listItemValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListItemValues"/> class.
        /// </summary>
        /// <param name="listItemValues">
        /// The list item values.
        /// </param>
        public ListItemValues(Dictionary<int, object> listItemValues)
        {
            this._listItemValues = listItemValues;
        }

        /// <summary>
        /// Gets the number of items in the list of values.
        /// </summary>
        public int Count => this._listItemValues.Count;

        /// <summary>
        /// Get the enumerator for the <see cref="Dictionary{TKey,TValue}"/> of values.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{T}"/> of the <see cref="Dictionary{TKey,TValue}"/></returns>
        public Dictionary<int, object>.Enumerator GetEnumerator()
        {
            return this._listItemValues.GetEnumerator();
        }
    }
}
