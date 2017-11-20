using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spend_Management.shared.code.EasyTree
{
    public class CustomMenuNode : EasyTreeNode
    {
        /// <summary>
        /// Icon for the node, If null then default icon is selected in the plugin
        /// </summary>
        public string IconName { get; set; }

        /// <summary>
        /// Description of the node, Used for quick fetch, on edit call 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ParentId of the node 
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// If the menu is system menu 
        /// </summary>
        public bool IsSystemMenu { get; set; }

        /// <summary>
        /// Order of the menu 
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Reference parent id
        /// </summary>
        public int ReferenceDynamicParentId { get; set; }

    }
}