namespace BusinessLogic.DataConnections
{
    using System.Collections;
    using System.Collections.Generic;

    using BusinessLogic.Identity;

    /// <summary>
    /// A class to assist with common Data Parameter tasks
    /// </summary>
    public abstract class DataParameters<T> : IEnumerable<T> where T : class
    {
        protected readonly List<T> ParametersCollection;

        /// <summary>
        /// The return value for a lookup
        /// </summary>
        public object ReturnValue { get; set; }

        /// <summary>
        /// Initalises an instance of <see cref="DataParameters{T}">DataParameters</see>
        /// </summary>
        protected DataParameters()
        {
            this.ParametersCollection = new List<T>();
            this.ReturnValue = null;
        }

        /// <summary>
        /// Clears the backing collection of all records
        /// </summary>
        public void Clear()
        {
            this.ParametersCollection.Clear();
        }

        /// <summary>
        /// Adds a key and value to the backing collection
        /// </summary>
        /// <param name="value">The value</param>
        public abstract void Add(T value);

        /// <summary>
        /// Adds a collection of <typeparam name="T"></typeparam> to the collection.
        /// </summary>
        /// <param name="values"></param>
        public abstract void Add(IEnumerable<T> values);

        /// <summary>
        /// Gets the specficied <typeparam name="T"></typeparam> from the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract T this[string key] { get; }

        /// <summary>
        /// Adds the standard parameters as <typeparamref name="T"/> for auditing purposes.
        /// @CUdelegateID and @CUemployeeID
        /// </summary>
        /// <param name="currentUser">An instance of <see cref="UserIdentity"/> performing the action to be audited.</param>
        public abstract void AddAuditing(UserIdentity currentUser);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="DataParameters{T}"/>.
        /// </summary>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.ParametersCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="DataParameters{T}"/>.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return this.ParametersCollection.GetEnumerator();
        }
    }
}
