namespace SpendManagementHelpers.TreeControl.Drop
{
    using System.Text;
    using System.Web.UI;

    using SpendManagementHelpers.Helpers;

    /// <summary>
    /// Used for dropping field/column filters that have been selected on to
    ///  - only use if you need full control on rendering the filter drop area, otherwise TreeCombo
    /// </summary>
    public class TreeReportColumnDrop : TreeDrop
    {
        #region Constructors

        /// <summary>
        /// The default constructor that uses a div element to contain the tree
        /// </summary>
        public TreeReportColumnDrop() : base(HtmlTextWriterTag.Div) { }

        #endregion Constructors

        #region Methods

        public override string GenerateJavaScript()
        {
            return new StringBuilder(@"
                $('#").Append(this.ClientID).Append(@"').jstree({

                    core: { animation: 0 },

                    themes:
                    {
                        theme: 'default',
                        dots: false
                    },

                    ui: { select_limit: 1 },

                    crrm:
                    {
                        move: { always_copy: 'multitree' }
                    },

                    json_data: { data: {} },

                    types:
                    {
                        max_depth: -2,
                        max_children: -2,

                        types:
                        {
                            'default':
                            {
                                valid_children: 'none',
                                start_drag: false,
                                select_node: false,
                                move_node: false,
                                delete_node: false,
                                hover_node: false
                            },

                            node:
                            {
                                valid_children: 'none',
                                icon: { image: '").Append(this.StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' },
                                start_drag: true,
                                select_node: true,
                                move_node: true,
                                delete_node: true,
                                hover_node: false
                            },

                            start_drag: false,
                            select_node: false,
                            move_node: false,
                            delete_node: false,
                            hover_node: false
                        }
                    },

                    plugins: ['themes', 'json_data', 'ui', 'dnd', 'crrm', 'types']
                });
            ").MinifyToString();
        }

        #endregion Methods
    }
}