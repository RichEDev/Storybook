namespace Spend_Management.shared.code.EasyTree
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Internally EasyTree uses a structure of 'nodes' that represent tree. 
    /// Most of the time it is safe to ignore the internal structure but if you want/need to interact with EasyTree or initialise 
    /// EasyTree with a JSON obejct you will need to understand what a node is. 
    /// </summary>
    public abstract class EasyTreeNode
    {
        protected EasyTreeNode()
        {
            this.isExpanded = false;
        }

        /// <summary>
        /// Children is an array of nodes which represent the node's sub nodes
        /// </summary>
        public List<EasyTreeNode> children { get; set; }

        /// <summary>
        /// Whether or not a particualr node is active. Only one node should be active and it will be highlighted in blue
        /// </summary>
        public bool isActive { get; set; }


        /// <summary>
        /// Whether or not a node is a folder. By default folders have a different icon and they are the only type of node that can be dropped to
        /// </summary>
        public bool isFolder { get; set; }

        /// <summary>
        /// If a node is lazy it will fire the 'openLazyNode' event when opened and then call the url defined in lazyUrl and populate its children with the response
        /// </summary>
        public bool isLazy { get; set; }

        /// <summary>
        /// All nodes need an id, if one isn't passed in an id will be created automatically
        /// </summary>
        public string internalId { get; set; }

        /// <summary>
        /// Any classes passed in here will span the underlying li element when the tree is rendered
        /// </summary>
        public string liClass { get; set; }


        /// <summary>
        /// The text that is displayed when the node is rendered
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Bread crumbs for the current field
        /// </summary>
        public string crumbs { get; set; }

        /// <summary>
        /// Whether or not a node is open
        /// </summary>
        public bool isExpanded { get; set; }


    }
}
