namespace BusinessLogic.UserDefinedFields
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    [Serializable]
    public class UserDefinedFieldValueCollection : IEnumerable<KeyValuePair<int, object>>
    {
        private readonly Dictionary<int, object> _userDefinedFieldValueCollection;

        /// <summary>
        /// Create a new instance of the <see cref="UserDefinedFieldValueCollection"/>
        /// </summary>
        /// <param name="seedList">The <see cref="SortedList{TKey,TValue}"/>to populate the collection.</param>
        public UserDefinedFieldValueCollection(IEnumerable<KeyValuePair<int, object>> seedList)
        {
            this._userDefinedFieldValueCollection = new Dictionary<int, object>();

            foreach (KeyValuePair<int, object> keyValuePair in seedList)
            {
                this._userDefinedFieldValueCollection.Add(keyValuePair.Key, keyValuePair.Value);    
            }
        }

        /// <summary>
        /// Create a new instance of the <see cref="UserDefinedFieldValueCollection"/>
        /// </summary>
        public UserDefinedFieldValueCollection()
        {
            this._userDefinedFieldValueCollection = new Dictionary<int, object>();
        }

        /// <summary>
        /// Get's the number of items in the current list.
        /// </summary>
        public int Count => this._userDefinedFieldValueCollection.Count;

        /// <summary>
        /// Gets a collection containing the keys in <see cref="UserDefinedFieldValueCollection"/>
        /// </summary>
        public Dictionary<int, object>.KeyCollection Keys => this._userDefinedFieldValueCollection.Keys;

        /// <summary>
        /// Gets a collection containing the values of <see cref="UserDefinedFieldValueCollection"/>
        /// </summary>
        public Dictionary<int, object>.ValueCollection Values => this._userDefinedFieldValueCollection.Values;

        /// <summary>
        /// Get the <see cref="object"/> stored via the <see cref="int"/> key.
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <returns>The <see cref="object"/> associated to the given key or null.</returns>
        public object this[int key] => this._userDefinedFieldValueCollection.ContainsKey(key) ? this._userDefinedFieldValueCollection[key] : null;

        /// <summary>
        /// Add a value to the <see cref="UserDefinedFieldValueCollection"/>
        /// </summary>
        /// <param name="item">The <see cref="KeyValuePair{TKey,TValue}"/>to add</param>
        public void Add(KeyValuePair<int, object> item)
        {
            this._userDefinedFieldValueCollection.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Add an objexct to the list.
        /// </summary>
        /// <param name="key">The Key value</param>
        /// <param name="value">The value</param>
        public void Add(int key, object value)
        {
            if (this._userDefinedFieldValueCollection.ContainsKey(key))
            {
                this._userDefinedFieldValueCollection[key] = value;
            }
            else
            {
                this._userDefinedFieldValueCollection.Add(key, value);
            }

        }

        /// <summary>
        /// Clear the current list.
        /// </summary>
        public void Clear()
        {
            this._userDefinedFieldValueCollection.Clear();
        }

        /// <summary>
        /// Get the Enumerator for the <see cref="UserDefinedFieldValueCollection"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<int, object>> GetEnumerator()
        {
            return this._userDefinedFieldValueCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the System.Collections.Generic.Dictionary{int, object}
        /// </summary>
        /// <returns>A System.Collections.Generic.Dictionary{int, object}.Enumerator structure for the System.Collections.Generic.Dictionary{int, object}.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._userDefinedFieldValueCollection.GetEnumerator();
        }

        /// <summary>
        /// Get the <see cref="Dictionary{TKey,TValue}"/> as a <see cref="SortedList{TKey,TValue}"/>
        /// </summary>
        /// <returns>A <see cref="SortedList{TKey,TValue}"/>of the values in the internal list.</returns>
        public SortedList<int, object> ToSortedList()
        {
            var result = new SortedList<int,object>();
            foreach (KeyValuePair<int, object> keyValuePair in this._userDefinedFieldValueCollection)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return result;
        }
    }
}

