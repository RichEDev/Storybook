namespace SpendManagementHelpers.TreeControl
{
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    /// <summary>
    /// jsTree is a jQuery plugin (we are using v1.0 as of 11/2011)
    /// - this class is used to package return data to the tree in a format that is useable with the json_data plugin (once serialised using JavascriptSerialize or via a web method return)
    /// </summary>
    public class JavascriptTreeData
    {
        /// <summary>
        /// Required for serialisation
        /// </summary>
        public JavascriptTreeData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodes">List of Nodes to add to the array</param>
        public JavascriptTreeData(List<JavascriptTreeNode> nodes)
        {
            this.data = nodes ?? new List<JavascriptTreeNode>();
        }

        /// <summary>
        /// This array is the element that is usually required for the return of json_data
        /// </summary>
        public List<JavascriptTreeNode> data;

        /// <summary>
        /// Serialize the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var jvs = new JavaScriptSerializer();
            return jvs.Serialize(this);
        }

        /// <summary>
        /// The class for a node on the tree
        /// </summary>
        public class JavascriptTreeNode
        {
            /// <summary>
            /// Required for serialisation
            /// </summary>
            public JavascriptTreeNode()
            {
            }

            /// <summary>
            /// The title
            /// </summary>
            public string data = "";

            /// <summary>
            /// A set of element attributes: 
            /// - id (a series of Guids from the tree "path" with _ seperator) 
            /// - rel (tree node type) 
            /// - fieldid (cField guid) 
            /// - joinviaid (JoinVia Guid)
            /// </summary>
            public LiAttributes attr = new LiAttributes();

            /// <summary>
            /// Open for a field, closed for a foreignKey or folder
            /// </summary>
            public string state = "";

            /// <summary>
            /// A metadata blob
            /// </summary>
            public Dictionary<string, object> metadata = null;

            /// <summary>
            /// A set of element attributes that get added to the &lt;li>
            /// </summary>
            public class LiAttributes
            {
                /// <summary>
                /// Required for serialisation
                /// </summary>
                public LiAttributes()
                {
                }

                /// <summary>
                /// a series of Guids from the tree "path" with _ seperator
                /// </summary>
                public string id = "";

                /// <summary>
                /// a series of Guids from the tree "path" with _ seperator
                /// </summary>
                public string internalId = "";

                /// <summary>
                /// a series of names from the tree "path"
                /// </summary>
                public string crumbs = "";

                /// <summary>
                /// tree node type
                /// </summary>
                public string rel = "";

                /// <summary>
                /// cField guid
                /// </summary>
                public string fieldid = "";

                /// <summary>
                /// JoinVia Guid
                /// </summary>
                public int joinviaid = 0;

                /// <summary>
                /// The type of the current field.
                /// </summary>
                public string fieldtype = "";

                /// <summary>
                /// The existing column id .
                /// </summary>
                public string columnid = string.Empty;

                /// <summary>
                /// The field comment which explains it's purpose.
                /// </summary>
                public string comment = string.Empty;
            }
        }
    }
}