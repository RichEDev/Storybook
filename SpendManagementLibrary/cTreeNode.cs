using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cTreeNode
    {
        private Guid gAssociatedID;
        private string sGroupname;
        private Guid? gParentID;
        private NodeType eNodeType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AssociatedID"></param>
        /// <param name="GroupName"></param>
        /// <param name="ParentID"></param>
        /// <param name="TreeNodeType"></param>
        public cTreeNode(Guid AssociatedID, string GroupName, NodeType TreeNodeType)
        {
            gAssociatedID = AssociatedID;
            sGroupname = GroupName;
            eNodeType = TreeNodeType;
        }

        /// <summary>
        /// The 
        /// </summary>
        public Guid AssociatedID
        {
            get { return gAssociatedID; }
        }

        public string GroupName
        {
            get { return sGroupname; }
        }

        public NodeType TreeNodeType
        {
            get { return eNodeType; }
        }
    }

    /// <summary>
    /// The type of node used whether its a folder or not 
    /// </summary>
    public enum NodeType
    {
        None = 0,
        Node,
        Parent
    }   

}
