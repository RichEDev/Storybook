using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spend_Management.shared.code.EasyTree
{
    public class ReportNode : EasyTreeNode
    {
        /// <summary>
        /// The id of the field in the node
        /// </summary>
        public Guid fieldid { get; set; }

        /// <summary>
        /// The type of the current field.
        /// </summary>
        public string fieldType { get; set; }

        /// <summary>
        /// The ID of the join via for this field. 
        /// </summary>
        public int joinviaid { get; set; }
    }
}