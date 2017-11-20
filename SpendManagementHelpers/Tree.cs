namespace SpendManagementHelpers
{
    using System;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using SpendManagementHelpers.Helpers;
    using SpendManagementHelpers.TreeControl;

    /// <summary>
    /// A control for creating a jsTree within an aspx page (www.jstree.com)
    ///  - only use if you need full control on rendering the tree, otherwise TreeCombo
    /// </summary>
    public class Tree : WebControl
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public Tree() : base(HtmlTextWriterTag.Div) { }

        #endregion Constructors



        #region Fields

        bool _outputResourceBlock = true;
        bool _showMenu = false;

        string _staticLibraryPath = "/static/";  // poor on the decoupling aspect
        string _themesPath = "../themes/";
        string _webServicePath = "~/PathTo/svcServiceName.asmx";
        string _getInitialTreeNodesWebServiceMethodName = "GetInitialTreeNodes";
        string _getBranchNodesWebServiceMethodName = "GetBranchNodes";

        #endregion Fields



        #region Properties

        /// <summary>
        /// Set to false if you need to prevent the javascript for the jsTree being created directly after the control html
        /// </summary>
        public bool OutputResources
        {
            get { return this._outputResourceBlock; }
            set { this._outputResourceBlock = value; }
        }

        /// <summary>
        /// Show the clickable menu buttons above the tree 
        /// - useful if drag and drop is not available on the client
        /// </summary>
        public bool ShowButtonMenu
        {
            get { return this._showMenu; }
            set { this._showMenu = value; }
        }

        /// <summary>
        /// The path to the static library root, trailing slash needed
        /// </summary>
        public string StaticLibraryPath
        {
            get { return this._staticLibraryPath; }
            set { this._staticLibraryPath = value; }
        }


        /// <summary>
        /// The path to the jsTree themes directory, trailing slash needed
        /// </summary>
        public string ThemesPath
        {
            get { return this._themesPath; }
            set { this._themesPath = value; }
        }

        /// <summary>
        /// The path to the web service that will furnish this tree's ajax calls
        /// </summary>
        public string WebServicePath
        {
            get { return this._webServicePath; }
            set { this._webServicePath = this.ResolveUrl(value); }
        }
        /// <summary>
        /// The name of the method that will return the initial collection of jsTreeData
        /// </summary>
        public string WebServiceInitialTreeNodesMethod
        {
            get { return this._getInitialTreeNodesWebServiceMethodName; }
            set { this._getInitialTreeNodesWebServiceMethodName = value; }
        }
        /// <summary>
        /// The name of the method that will return the branch nodes when one is "opened"
        /// </summary>
        public string WebServiceBranchNodesMethod
        {
            get { return this._getBranchNodesWebServiceMethodName; }
            set { this._getBranchNodesWebServiceMethodName = value; }
        }

        #endregion Properties



        #region Enums

        #endregion Enums



        #region Method Overrides

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager parentSM = ScriptManager.GetCurrent(this.Page);
            SharedTreeMethods.TryAddWebResources(ref parentSM);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            if (this._outputResourceBlock)
            {
                writer.Write(new StringBuilder("<script type=\"\">").AppendLine(@"//<![CDATA[").AppendLine(@"$(document).ready(function() { ").AppendLine(this.GenerateJavaScript()).AppendLine(@" });").AppendLine(@"//]]>").Append(@"</script>"));
                writer.Write(SharedTreeMethods.GenerateCSS());
            }
        }

        #endregion Method Overrides



        #region Methods

        public string GenerateJavaScript()
        {
            return new StringBuilder(@" 
                $('#").Append(this.ClientID).Append(@"').jstree({
                    core: { animation: 0 },
                    themes: { theme: 'default' },
                    ui: { select_limit: 1 },
                    crrm: {},
                    dnd: {},
                    json_data: {
                        data: { data: 'loading...' },
                        ajax: {
                            url: '").Append(this._webServicePath).Append("/").Append(this._getBranchNodesWebServiceMethodName).Append(@"',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            async: true,
                            data: function (n) {
                                var nodeID = n.attr ? n.attr('id') : '';
                                var crumbs = n.attr ? n.attr('crumbs') : '';
                                var fieldID = n.attr ? n.attr('fieldid') : '00000000-0000-0000-0000-000000000000';
                                var metadata = n.metadata ? n.metadata : '';
                                return ").Append("\"{ fieldID: '\" + fieldID + \"', crumbs: '\" + (typeof crumbs === 'undefined' ? '' : crumbs) + \"', nodeID: '\" + nodeID + \"', metadata: '\" + metadata + \"' }\";").Append(@"
                            },
                            success: function (r) {
                                return r.d.data;
                            }
                        }
                    },
                    types: {
                        max_depth: -2,
                        max_children: -2,
                        types: {
                            'default': {
                                valid_children: 'none',
                                start_drag: false,
                                select_node: false,
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            group: {
                                valid_children: ['group', 'nodelink', 'node'],
                                icon: { image: '").Append(this.StaticLibraryPath).Append(@"icons/16/Plain/folder_blue.png' },
                                start_drag: false,
                                select_node: function (e) {
                                    this.toggle_node(e);
                                    return false;
                                },
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            nodelink: {
                                valid_children: ['group', 'nodelink', 'node'],
                                icon: { image: '").Append(this.StaticLibraryPath).Append(@"icons/16/Plain/table_view.png' },
                                start_drag: false,
                                select_node: function (e) {
                                    this.toggle_node(e);
                                    return false;
                                },
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            node: {
                                valid_children: 'none',
                                icon: { image: '").Append(this.StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' },
                                start_drag: function(e)
                                {
                                    $(e).addClass('tree-drag-add').data('outOfTree', false);
                                    $(document).bind('mouseup.tree', function ()
                                    {
                                        $(e).removeClass('tree-drag-add');
                                        var treeObj = $('#").Append(this.ClientID).Append(@"'),
                                            treeObjAssoc = $('#' + treeObj.attr('associatedcontrolid'));
                                        /*if ($(e).data('outOfTree') === true)
                                        {
                                            treeObj.jstree('delete_node', e);
                                            SEL.Trees.Tree.DisplayEmptyMessage('").Append(this.ClientID).Append(@"');
                                        }*/
                                        treeObjAssoc.find('.tree-drag-add').removeClass('tree-drag-add');
                                        treeObjAssoc.css('backgroundImage', 'none').find('li').css({ opacity: 1 });
                                        $(document).unbind('mouseup.tree');
                                    });
                                },
                                select_node: true,
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },
                            start_drag: false,
                            select_node: false,
                            move_node: false,
                            delete_node: false,
                            hover_node: false
                        }
                    },
                    plugins: ['themes', 'json_data', 'ui', 'crrm', 'dnd', 'types']
                });

                $('#' + $('#").Append(this.ClientID).Append(@"').attr('associatedcontrolid')).mouseleave(function ()
                {
                    $('.tree-drag-add').data('outOfTree', false);
                    $('#' + $('#").Append(this.ClientID).Append(@"').attr('associatedcontrolid'))
                        .css('backgroundImage', 'none')
                        .find('li').css({ opacity: 1 });
                })
                .mouseenter(function ()
                {
                    var dragObj = $('.tree-drag-add');
                    dragObj.data('outOfTree', true);
                    if(dragObj.length > 0)
                    {
                        $('#' + $('#").Append(this.ClientID).Append(@"').attr('associatedcontrolid'))
                            .css('backgroundImage', 'url(").Append(this.StaticLibraryPath).Append(@"/images/backgrounds/gradients/availableFieldsDropHover.png)')
                            .find('li').css({ opacity: 0.8 });
                    }
                });

                SEL.Trees.New('").Append(this.ClientID).Append(@"', '").Append(this.WebServicePath).Append(@"', '").Append(this.WebServiceInitialTreeNodesMethod).Append(@"', '").Append(this.WebServiceBranchNodesMethod).Append(@"', '');
            ").MinifyToString();
        }

        #endregion Methods
    }
}