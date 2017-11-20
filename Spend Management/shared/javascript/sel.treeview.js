(function ()
{
    var scriptName = "TreeView";
    function execute()
    {
        SEL.registerNamespace("SEL.TreeView");
        SEL.TreeView =
        {
            _this: this,

            /// <summary>
            /// Enumerator Example
            /// </summary>   
            NodeType:
            {
                None: { toInt: (function () { return 0; }), toString: (function () { return "Please select a node type"; }) },
                Node: { toInt: (function () { return 1; }), toString: (function () { return "Standard Node"; }) },
                Parent: { toInt: (function () { return 2; }), toString: (function () { return "Parent Node"; }) },
                Container: { toInt: (function () { return 3; }), toString: (function () { return "Container Node"; }) }
            },
            /// <summary>
            /// Convert to Enumerator
            /// </summary>
            convertToNodeType: function (val)
            {
                switch (val)
                {
                    case 0:
                    case "0":
                    case "None":
                    case "Please select a node type":
                        return this.NodeType.None;
                    case 1:
                    case "1":
                    case "Node":
                    case "Standard Node":
                        return this.NodeType.Node;
                    case 2:
                    case "2":
                    case "Parent":
                    case "Parent Node":
                        return this.NodeType.Parent;
                    case 3:
                    case "3":
                    case "Container":
                    case "Container Node":
                        return this.NodeType.Container;
                    default:
                        return this.NodeType.None;
                }
            },
            /// <summary>
            /// TreeView Object
            /// </summary>
            Tree: function (sName)
            {
                this.id = sName.replace(/[\s\W]+/gi, '');
                this._this = this;
                this.name = sName;
                this.container = this.getContainer();
                /// TreeView Nodes Collection
                this.nodes = [];
            },
            /// <summary>
            /// TreeView Node Object
            /// </summary>
            Node: function (sName, eNodeType, bExpanded)
            {
                this.id = sName.replace(/[\s\W]+/gi, '');
                this.name = sName;
                this.nodeType = SEL.TreeView.NodeType.None;
                this.expanded = false;

                if (eNodeType !== null) { this.nodeType = eNodeType; }
                if (bExpanded !== null) { this.expanded = bExpanded; }
            },
            /// <summary>
            /// Called when a treeview is fetched by web service
            /// </summary>
            onComplete: function ()
            {
                showMasterPopup("Treeview Complete");
                return;
            },
            /// <summary>
            /// If an error occurs
            /// </summary>                
            onError: function ()
            {
                showMasterPopup("Error in treeview");
                return;
            }
        };

        /// <summary>
        /// Prototype Extensions for TreeView
        /// </summary>
        SEL.TreeView.Tree.prototype.buildTree = function ()
        {
            var i;

            if (this.nodes !== undefined && this.nodes !== null)
            {
                for (i = 0; i < this.nodes.length; i++)
                {
                    this.nodes[i].buildNode();
                }
            }
        };
        SEL.TreeView.Tree.prototype.buildNode = function (oNode, oContextDOMNode)
        {
            var i;
            if (oNode === null) { oNode = this.nodes; } // if no context, we're starting at the top
            if (oContextDOMNode === null) { oContextDOMNode = this.container; } // if no context, we're starting at the top

            oContextDOMNode.addChild(oNode.nodes[i].buildNode());

            if (oNode.nodes !== undefined && oNode.nodes !== null && (oNode.nodes[i].nodeType === SEL.TreeView.NodeType.Container || oContext.nodes[i].nodeType === SEL.TreeView.NodeType.Parent))
            {
                for (i = 0; i < oNode.nodes.length; i++)
                {
                    this.showTree(oContext.nodes[i]);
                }
            }
        };
        SEL.TreeView.Tree.prototype.getContainer = function ()
        {
            return document.getElementById(this.id);
        };
        SEL.TreeView.Tree.prototype.addNode = function (oNode)
        {
            this.nodes.push(oNode);
        };
        SEL.TreeView.Tree.prototype.removeNode = function (nodeID)
        {
            var i;
            for (i = 0; i < this.nodes.length; i++)
            {
                if (this.nodes[i] !== null && this.nodes[i].nodeID === nodeID)
                {
                    this.nodes[i] = null;
                    break;
                }
            }
        };
    }

    if (window.Sys && Sys.loader)
    {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}
)();

 
(function ()
{
    var scriptName = "Trees";
    function execute()
    {
        SEL.registerNamespace("SEL.Trees");
        SEL.Trees =
        {
            _this: this,
            /// <summary>
            /// Enumerator Example
            /// </summary>   
            NodeType:
            {
                None: { toInt: (function () { return 0; }), toString: (function () { return "Please select a node type"; }) },
                Node: { toInt: (function () { return 1; }), toString: (function () { return "Standard Node"; }) },
                Parent: { toInt: (function () { return 2; }), toString: (function () { return "Parent Node"; }) },
                Container: { toInt: (function () { return 3; }), toString: (function () { return "Container Node"; }) }
            },
            /// <summary>
            /// Convert to Enumerator
            /// </summary>
            convertToNodeType: function (val)
            {
                switch (val)
                {
                    case 0:
                    case "0":
                    case "None":
                    case "Please select a node type":
                        return _this.NodeType.None;
                    case 1:
                    case "1":
                    case "Node":
                    case "Standard Node":
                        return _this.NodeType.Node;
                    case 2:
                    case "2":
                    case "Parent":
                    case "Parent Node":
                        return _this.NodeType.Parent;
                    case 3:
                    case "3":
                    case "Container":
                    case "Container Node":
                        return _this.NodeType.Container;
                    default:
                        return _this.NodeType.None;
                }
            },

            /// <summary>
            /// Tree collection
            /// </summary>
            Trees: [],

            /// <summary>
            /// Tree Object
            /// </summary>
            Tree: function (name)
            {
                var self = this;
                var container = null;

                this.ID = name.replace(/[\s\W]+/gi, '');
                this.Name = name;
            },

            /// <summary>
            /// TreeView Object
            /// </summary>
            NewTree: function (sName, oParent)
            {
                var oTree = new SEL.Trees.Tree(sName);

                if(_this.getContainer(oTree, oParent))
                {
                    _this.Trees.push(oTree);
                }
            },

            /// <summary>
            /// Called when a treeview is fetched by web service
            /// </summary>
            onComplete: function ()
            {
                showMasterPopup("Tree operation Complete");
                return;
            },
            /// <summary>
            /// If an error occurs
            /// </summary>                
            onError: function ()
            {
                showMasterPopup("Error in tree");
                return;
            }
        };

        /// <summary>
        /// Prototype Extensions for Trees
        /// </summary>
        SEL.Trees.Tree.prototype.getContainer = function (oParent)
        {
            
        };
    }

    if (window.Sys && Sys.loader)
    {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}
)();