namespace BusinessLogic.JoinVia
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that manages a list of <see cref="IJoinViaPart"/>
    /// </summary>
    public class JoinViaParts
    {
        /// <summary>
        /// The _list of parts.
        /// </summary>
        private readonly List<IJoinViaPart> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinViaParts"/> class. 
        /// </summary>
        /// <param name="parts">
        /// The parts or steps for the join
        /// </param>
        public JoinViaParts(List<IJoinViaPart> parts)
        {
            this._list = parts;
        }

        /// <summary>
        /// Get the Enumerator for the collection.
        /// </summary>
        /// <returns>The Enumerator of the parts.</returns>
        public List<IJoinViaPart>.Enumerator GetEnumerator()
        {
            return this._list.GetEnumerator();
        }
    }
}