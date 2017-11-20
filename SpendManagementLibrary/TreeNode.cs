using System.Collections.Generic;
using System.Linq;

namespace SpendManagementLibrary
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// A simple class to hold hierarchical information. 
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    public class TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        /// <summary>
        /// Initialiser with generic type
        /// </summary>
        /// <param name="value"></param>
        public TreeNode(T value)
        {
            _value = value;
        }

        /// <summary>
        /// return the indexed value
        /// </summary>
        /// <param name="i">the index</param>
        /// <returns></returns>
        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        /// <summary>
        /// Returns the parent element in the structure
        /// </summary>
        public TreeNode<T> Parent { get; private set; }

        /// <summary>
        /// Gets the value of the node
        /// </summary>
        public T Value { get { return _value; } }

        /// <summary>
        /// Gets a read only collection of the children nodes.
        /// </summary>
        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        /// <summary>
        /// Adds a child node under the current node
        /// </summary>
        /// <param name="value">The generic type node value</param>
        /// <returns>Returns the newly created node</returns>
        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            _children.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a range of nodes to the current node.
        /// </summary>
        /// <param name="values">Ann array of the generic type</param>
        /// <returns>Retuens the newly created range of nodes</returns>
        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        /// <summary>
        /// Removes the specified node from the current node set
        /// </summary>
        /// <param name="node">The node to remove</param>
        /// <returns>Returns the success of the remove operation</returns>
        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        /// <summary>
        /// Flattens the entire hierarchy
        /// </summary>
        /// <returns>An enumeratable collection of the generic node type</returns>
        public IEnumerable<T> Flatten()
        {
            return new[] { Value }.Union(_children.SelectMany(x => x.Flatten()));
        }
    }
}
