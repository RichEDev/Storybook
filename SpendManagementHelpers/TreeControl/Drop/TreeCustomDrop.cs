namespace SpendManagementHelpers.TreeControl.Drop
{
    using System.Text;
    using System.Web.UI;

    using SpendManagementHelpers.Helpers;

    public class TreeCustomDrop : TreeDrop
    {
        public TreeCustomDrop()
            : base(HtmlTextWriterTag.Div)
        {
        }

        public override string GenerateJavaScript()
        {
            return new StringBuilder(@"
            $('#").Append(this.ClientID).Append(@"').jstree({ 
                core: { animation: 0 }, 
                themes: {
                    theme: 'default',
                    dots: false
                }, 
                ui: { select_limit: 1 }, 
                crrm: {
                    move: {
                        always_copy: 'multitree'
                    }
                }, 
                json_data: { data: {} }, 
                types: {
                    max_depth: -2,
                    max_children: -2,
                    types:
                    {
                        'default': {
                            valid_children: 'none',
                            start_drag: false,
                            select_node: false,
                            move_node: false,
                            delete_node: false,
                            hover_node: false
                        },
                        node: {
                            valid_children: 'none',
                            icon: { image: '").Append(this.StaticLibraryPath).Append(@"icons/16/Plain/table_column.png' },
                            start_drag: function(e) {
                                $(e).addClass('tree-drag-remove').data('outOfDrop', false);
                                $(document).bind('mouseup.tree', function () {
                                    $(e).removeClass('tree-drag-remove');
                                    var dropObj = $('#").Append(this.ClientID).Append(@"');
                                    var treeID = dropObj.attr('associatedcontrolid');
                                    if ($(e).data('outOfDrop') === true)
                                    {
                                        dropObj.jstree('delete_node', e);
                                        SEL.Trees.Node.Enable(treeID, e[0].id.replace('copy_', ''));
                                        SEL.Trees.Tree.DisplayEmptyMessage('").Append(this.ClientID).Append(@"');
                                    }
                                    $('#' + dropObj.attr('associatedcontrolid')).css('backgroundImage', 'none');
                                    $(document).unbind('mouseup.tree');
                                });
                            },
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
            
            var assocID = $('#").Append(this.ClientID).Append(@"').attr('associatedcontrolid');
            $('#' + assocID).mouseleave(function ()
            {
                $('.tree-drag-remove').data('outOfDrop', false);
                $('#' + assocID).css('backgroundImage', 'none');
            })
            .mouseenter(function ()
            {
                var dragObj = $('.tree-drag-remove');
                dragObj.data('outOfDrop', true);
                if(dragObj.length > 0)
                {
                    $('#' + assocID).css('backgroundImage', 'url(").Append(this.StaticLibraryPath).Append(@"/images/backgrounds/gradients/availableFieldsDropHover.png)');
                }
            });

            var fm = SEL.Trees.Custom;
            $('#").Append(this.ClientID).Append(@"').bind('move_node.jstree', function (e, d) {                                
                
                SEL.Trees.Filters.RefreshCustomSummaryLayout('").Append(this.ClientID).Append(@"');

                SEL.Trees.Tree.DisplayEmptyMessage('").Append(this.ClientID).Append(@"');
            });

            $('#").Append(this.ClientID).Append(@"').bind('create.jstree', function (e, d) {                   
                 if (d.rslt.cy)
                {
                    fm.Edit(d.rslt.oc,'").Append(ClientID).Append(@"');
                }
                SEL.Trees.Filters.RefreshCustomSummaryLayout('").Append(this.ClientID).Append(@"');
            });

            $('#").Append(this.ClientID).Append(@"').bind('loaded.jstree', function ()
            {
                $('#").Append(this.ClientID).Append(@"').jstree('data').data.core.li_height = 30;
            });									

            $('#' + assocID).bind('open_node.jstree', function (e, d)
               {
                   SEL.Trees.Node.Disable(assocID, d.args[0].attr('id'), 'refreshBranch');
               });

			$('[id$=_tcFilters_pnlFilter]').appendTo('#aspnetForm');
			$('[id$=_tcFilters_modFilter_backgroundElement]').appendTo('#aspnetForm');
			$('#tcPopupListItemContainer').appendTo('#aspnetForm');

			$('[id$=_tcFilters_pnlFilter] .inputs INPUT').unbind('keypress.checkForEnter');
			SEL.Common.BindEnterKeyForSelector('[id$=_tcFilters_pnlFilter] .inputs INPUT', SEL.Trees.Custom.Save);

            SEL.Trees.New('").Append(this.ClientID).Append(@"', '").Append(this.WebServicePath).Append(@"', '', '', '").Append(this.WebServiceSelectedNodesMethod).Append(@"');

            ").MinifyToString();
        }
    }
}
//if (d.rslt.cy)
//{
//    fm.Edit(d.rslt.oc,'").Append(this.ClientID).Append(@"');
//}
