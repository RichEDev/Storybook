namespace ApiCrud.DataAccess
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Data access filter class, allows addition of filters based on columns.
    /// </summary>
    public class DataAccessFilter : ICollection<DataAccessFilter.Filter>
    {
        /// <summary>
        /// The inner array.
        /// </summary>
        private ArrayList innerArray;

        /// <summary>
        /// Initialises a new instance of the <see cref="DataAccessFilter"/> class.
        /// </summary>
        public DataAccessFilter()
        {
            this.innerArray = new ArrayList();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.innerArray.Count;
            }

            private set
            {
            }
        }

        /// <summary>
        /// Gets a value indicating whether is read only.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// The filter object.  Fieldname must be "like" FilterValue for filter to be true.
        /// </summary>
        public class Filter
        {
            /// <summary>
            /// Gets or sets the field name.
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            /// Gets or sets the filter value.
            /// </summary>
            public object FilterValue { get; set; }
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public IEnumerator<Filter> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// The add a Filter item to the list.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(Filter item)
        {
            this.innerArray.Add(item);
        }

        /// <summary>
        /// The clear list.
        /// </summary>
        public void Clear()
        {
            this.innerArray.Clear();
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Contains(Filter item)
        {
            return this.innerArray.Cast<Filter>().Any(obj => obj.FieldName == item.FieldName);
        }

        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// The array index.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public void CopyTo(Filter[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Remove(Filter item)
        {
            bool result = false;

            for (int i = 0; i < this.innerArray.Count; i++)
            {
                var obj = (Filter)this.innerArray[i];

                if (obj.FieldName == item.FieldName)
                {
                    this.innerArray.RemoveAt(i);
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}