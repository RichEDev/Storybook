namespace BusinessLogic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Wrapper for <see cref="IEnumerable{T}"/>. Prevents the exposure of unnecessary members found on <see cref="IEnumerable{T}"/> implementations.
    /// </summary>
    /// <typeparam name="T">The type of elements in this <see cref="ListWrapper{T}"/>.</typeparam>
    [Serializable]
    public abstract class ListWrapper<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListWrapper{T}"/> class.
        /// </summary>
        protected ListWrapper()
        {
            this.BackingCollection = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListWrapper{T}"/> class that
        /// contains elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        protected ListWrapper(IEnumerable<T> collection)
        {
            this.BackingCollection = new List<T>(collection);
        }

        /// <summary>
        /// Gets the number of elements contained in this <see cref="ListWrapper{T}"/>.
        /// </summary>
        public int Count => this.BackingCollection.Count;

        /// <summary>
        /// Gets the backing collection which stores the actual values of this <see cref="ListWrapper{T}"/> in a <see cref="List{T}"/>.
        /// </summary>
        protected List<T> BackingCollection { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The element at <paramref name="index"/>.</returns>
        public T this[int index] => this.BackingCollection[index];

        /// <summary>
        /// Adds a <typeparamref name="T"/> to the end of the <see cref="ListWrapper{T}"/>
        /// </summary>
        /// <param name="item">The object to be added to the end of the System.Collections.Generic.List`1. The value can be null for reference types.</param>
        /// <exception cref="ArgumentException">An item with the same key has already been added.</exception>
        public void Add(T item)
        {
            this.BackingCollection.Add(item);
        }

        /// <summary>
        /// Determines whether an element is in this collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return this.BackingCollection.Contains(item);
        }

        /// <summary>
        /// Determines whether an element of type <typeparamref name="T"/> is in this collection.
        /// </summary>
        /// <param name="type">The type to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool Contains(Type type)
        {
            return this.BackingCollection.Any(item => item.GetType() == type);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ListWrapper{T}"/>
        /// </summary>
        /// <returns>A System.Collections.Generic.List`1.Enumerator for the System.Collections.Generic.List`1.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.BackingCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ListWrapper{T}"/>
        /// </summary>
        /// <returns>A System.Collections.Generic.List`1.Enumerator for the System.Collections.Generic.List`1.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
