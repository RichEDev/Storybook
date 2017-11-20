/* <summary>Reports Methods</summary> */
(function (SEL) {
    var scriptName = "reports";

    function execute() {
        SEL.registerNamespace("SEL.Reports");
        SEL.Reports = {
            IDs:
            {
                General:
                {
                    ReportName: null,
                    Description: null,
                    Category: null,
                    ReportOn: null,
                    Claimant: null,
                    ReportID: null,
                    LimitReport: null,
                    LimitReportValidator: null,
                    CurrentField: null,
                    CurrentFieldType: null,
                    EasyTree: null,
                    EasyTreeExpanded: [],
                    PopulatingTree: null,
                    ScrollOffset: null,
                    NewCriteria: null,
                    CursorPosition: null,
                    CriteriaDetail: null,
                    ItemDetail: null,
                    TooltipTimer: null,
                    CurrentTooltipFieldId: null,
                    CalcTimeout: null,
                    PreventBuild: null,
                    CurrencySymbol: null,
                    CalculatedSaveButton: null
                },
                ColumnSelector:
                {
                    Tab: null,
                    TreeContainer: null,
                    Tree: null,
                    Drop: null,
                    CurrentId: null
                },
                CriteriaSelector:
                {
                    Tab: null,
                    TreeContainer: null,
                    Tree: null,
                    Drop: null,
                    Node:null
                },
                Charts:
                {
                    General:
                    {
                        imgAreaChart: null,
                        imgBarChart: null,
                        imgColumnChart: null,
                        imgDonutChart: null,
                        imgDotChart: null,
                        imgLineChart: null,
                        imgPieChart: null,
                        txtChartTitle: null,
                        chkShowLegend: null,
                        ddlXAxis: null,
                        ddlYAxis: null,
                        ddlGroupBy: null,
                        chkCumulative: null,
                        ddlChartTitleFont: null,
                        txtChartTitleColour: null,
                        ddlTextFont: null,
                        txtTextFontColour: null,
                        txtTextBackgroundColour: null,
                        chkShowValues: null,
                        chkShowPercent: null,
                        ddlChartSize: null,
                        ddlYAxisRange: null,
                        chkShowLabels: null,
                        ddlLegendPosition: null,
                        ddlCombineOthers: null,
                        ddlShowChart: null,
                        FilterInput: null
                    },
                    Columns:
                    {
                        Modal: null
                    },
                    Source: null

                },
                Timeout: null,
                CriteriaId: 0,
                OddRow: false,
                Loading: false,
                Function: null
            },
            DomIDs:
            {
                Filters: {
                    FilterModalObj: null,
                    FilterModal: {
                        Heading: null,
                        FilterDropDown: null,
                        Criteria1: null,
                        Criteria2: null,
                        Criteria1DropDown: null,
                        CriteriaListDropDown: null,
                        FilterRequiredValidator: null,
                        RequiredValidator1: null,
                        RequiredValidator2: null,
                        DataTypeValidator1: null,
                        DataTypeValidator2: null,
                        CompareValidator: null,
                        TimeValidator1: null,
                        TimeValidator2: null,
                        DateAndTimeCompareValidator: null,
                        TimeRangeCompareValidator: null,
                        CriteriaRow1: null,
                        CriteriaRow2: null,
                        CriteriaListRow: null,
                        Criteria1ImageCalc: null,
                        Criteria2ImageCalc: null,
                        Criteria1Time: null,
                        Criteria2Time: null,
                        List: null,
                        Criteria1Spacer: null,
                        Criteria2Spacer: null,
                        Criteria1Label: null,
                        Criteria2Label: null,
                        Criteria1ValidatorSpan: null,
                        Criteria2ValidatorSpan: null,
                        IntegerValidator1: null,
                        IntegerValidator2: null,
                        Runtime: null,
                        RuntimeRow: null
                    }
                }
            },
            TableID: null,
            ReportID: null,
            RequestNumber: null,
            ColumnsLoaded: false,
            CriteriaLoaded: false,
            EnterAtRunTime: "<EnterAtRunTime>",
            DisableColumns : false,
            SetupEnterKeyBindings: function() {
                // Base Save
                SEL.Common.BindEnterKeyForSelector('.primaryPage', SEL.Reports.Misc.SaveReport);
                // report column Save
                $('#chkStaticRuntime').unbind().change(function ()
                {
                    if ($(this).prop('checked')) {
                        $('#divFormula').hide();
                        $("[id$='btnClearCalc']").parent().hide()
                    } else {
                        $('#divFormula').show();
                        $("[id$='btnClearCalc']").parent().show()
                    }
                });
            },
            ColumnIndex: null,
            SetupMouseAndResizeEvents: function () {
                $(window).resize(function () {
                    var grid = $("#preview").data("ejGrid");
                    if (grid) {
                        grid.windowonresize();
                    }
                   });
               
                var mouseX;
                var mouseY;
                $(document).unbind('mousemove').on("mousemove", function (event) {
                    mouseX = event.pageX;
                    mouseY = event.pageY;
                   
                    $('#_st_clone_').css({ 'top': mouseY + 30, 'left': mouseX + 20 }).fadeIn('slow');

                    // Add styles and set column index when dragging and dropping an element from the tree to the grid
                    if ($("#_st_clone_").is(":visible")) {
                        if (!$(event.target).hasClass("e-gridheader")) {
                            $(".column-drop-border").removeClass("column-drop-border");
                            $(".column-drop-border-right").removeClass("column-drop-border-right");
                        }
                        if ($(event.target).hasClass("e-headercell") ||
                            $(event.target).hasClass("e-rowcell") ||
                            $(event.target).hasClass("e-headercelldiv")) {
                            if ($(event.target).parents("#preview").length === 1) {
                                $("#easytree-reject").hide();
                                $("#preview .e-rowcell").css("cursor", "pointer");
                                SEL.Reports.ColumnIndex = $(event.target).hasClass("e-headercelldiv") ? $(event.target).parent().index() + 1 : $(event.target).index() + 1;
                                // if the table has groups
                                if ($("#preview").find("div.e-headercelldiv.e-emptyCell").length > 0) {
                                    var numberOfGroups = $(".e-groupdroparea")[0].childElementCount;
                                    // if there is only one group
                                    if (numberOfGroups === 1) {
                                        // if the item from the tree is dropped on a row
                                        if ($(event.target).hasClass("e-rowcell")) {
                                            if (event.clientX - $(event.target).offset().left < ($(event.target).width()) / 2) {
                                                $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 1) + ")").addClass("column-drop-border");
                                                SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 1;
                                            } else {
                                                if ($("#preview td:nth-child(" + (SEL.Reports.ColumnIndex + 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 2) + ")").length > 0) {
                                                    $("#preview td:nth-child(" + (SEL.Reports.ColumnIndex + 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 2) + ")").addClass("column-drop-border");
                                                } else {
                                                    $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 1) + ")").addClass("column-drop-border-right");
                                                }
                                                
                                            }
                                        } else //the item from tree is dropped on headercell
                                        {
                                            if (event.clientX - $(event.target).offset().left < ($(event.target).width()) / 2) {
                                                $("#preview td:nth-child(" +(SEL.Reports.ColumnIndex - 1) +"),th:nth-child(" + SEL.Reports.ColumnIndex +")").addClass("column-drop-border");
                                                SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 2;
                                            } else {
                                                if ($("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 1) + ")").length >0) {
                                                    $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 1) + ")").addClass("column-drop-border");
                                                } else {
                                                    $("#preview td:nth-child(" + (SEL.Reports.ColumnIndex - 1) + "),th:nth-child(" + SEL.Reports.ColumnIndex + ")").addClass("column-drop-border-right");
                                                }
                                                SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 1;
                                            }
                                        }

                                    } else // more than 1 group
                                    {
                                        SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - (numberOfGroups - 1);
                                        // if the item from the tree is dropped on a row
                                        if ($(event.target).hasClass("e-rowcell")) {
                                            SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex + (numberOfGroups - 1);
                                            if (event.clientX - $(event.target).offset().left < ($(event.target).width()) / 2) {
                                                $("#preview td:nth-child(" + SEL.Reports.ColumnIndex +"),th:nth-child(" +(SEL.Reports.ColumnIndex + numberOfGroups) + ")").addClass("column-drop-border");
                                                SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 1;
                                            } else {
                                                if ($("#preview td:nth-child(" + (SEL.Reports.ColumnIndex + 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + numberOfGroups + 1) + ")").addClass("column-drop-border").length >0) {
                                                    $("#preview td:nth-child(" + (SEL.Reports.ColumnIndex + 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + numberOfGroups + 1) + ")").addClass("column-drop-border");
                                                } else {
                                                    $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + numberOfGroups) + ")").addClass("column-drop-border-right");
                                                }
                                                
                                            }

                                        } else // the item from tree is dropped on headercell
                                        {
                                            if (event.clientX - $(event.target).offset().left < ($(event.target).width()) / 2) {
                                                $("#preview td:nth-child(" + (SEL.Reports.ColumnIndex - 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + (numberOfGroups - 1)) + ")").addClass("column-drop-border");
                                                SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 2;
                                            } else {
                                                if ($("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + numberOfGroups) + ")").length > 0) {
                                                    $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + (SEL.Reports.ColumnIndex + numberOfGroups) + ")").addClass("column-drop-border");
                                                } else { $("#preview td:nth-child(" + (SEL.Reports.ColumnIndex - 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + (numberOfGroups - 1)) + ")").addClass("column-drop-border-right"); }
                                                
                                                SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 1;
                                            }

                                        }
                                    }
                                } else {
                                    if (event.clientX - $(event.target).offset().left < ($(event.target).width()) / 2) {
                                        $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + SEL.Reports.ColumnIndex + ")").addClass("column-drop-border");
                                        SEL.Reports.ColumnIndex = SEL.Reports.ColumnIndex - 1;
                                    } else {
                                        if ($("#preview td:nth-child(" + (SEL.Reports.ColumnIndex + 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 1) + ")").length > 1){
                                            $("#preview td:nth-child(" + (SEL.Reports.ColumnIndex + 1) + "),th:nth-child(" + (SEL.Reports.ColumnIndex + 1) + ")").addClass("column-drop-border");
                                        }
                                        else { $("#preview td:nth-child(" + SEL.Reports.ColumnIndex + "),th:nth-child(" + SEL.Reports.ColumnIndex + ")").addClass("column-drop-border-right"); }
                                    }

                                }
                            }
                        } else {
                            $("#easytree-reject").show();
                        }
                    } else {
                        $("#preview .e-rowcell").css("cursor", "default");
                    }

                    // Auto scroll when dropping an item from tree to the grid or while reordering -  _st_clone_ is the tree element, e-dragclone is the reorder element
                    if ($("#_st_clone_").is(":visible") || $(".e-dragclone").is(":visible")) {
                        if ($(event.target).parents("#preview").length === 1) {
                            var grid = $("#preview").data("ejGrid");
                            if (grid) {
                                var mouseXpos = event.clientX - $('#preview').position().left;
                                var divWidth = $('#preview').width();
                                var sTop;
                                if (grid._scrollObject._scrollXdata!= undefined && mouseXpos < 150) {
                                    sTop = grid._scrollObject._scrollXdata.sTop === undefined
                                                                           ? 0
                                                                           : grid._scrollObject._scrollXdata.sTop - 50;
                                    grid.getScrollObject().scrollX(sTop);
                                    if (grid._scrollObject._scrollXdata.sTop < 150) {
                                        grid.getScrollObject().scrollX(0);
                                    }
                                };

                                if (grid._scrollObject._scrollXdata != undefined && mouseXpos > divWidth - 25) {
                                    sTop = grid._scrollObject._scrollXdata.sTop === undefined
                                        ? 50
                                        : grid._scrollObject._scrollXdata.sTop + 50;
                                    grid.getScrollObject().scrollX(sTop);
                                };
                            }
                        }
                    }

                    if ($(".e-dragclone").is(":visible")) {
                        $("#removeColumnIcon").css({ left: mouseX + 20, top: mouseY + 10 });
                        $(".e-groupdroparea").css("cursor", "pointer");
                        $("body, #preview tbody").css("cursor", "default");
                        if ($(event.target).parents("#menuOwner").length !== 1 && $(event.target).parents("#preview .e-gridheader").length !== 1 && !$(event.target).hasClass("e-groupdroparea") && $(event.target).parents(".e-groupdroparea").length !== 1) {
                            $("#easytree-reject").show();
                        } else {
                            $("#easytree-reject").hide();
                        }
                        if ($(event.target).parents("#menuOwner").length === 1) {
                            $("#menuOwner").css("cursor", "pointer");
                            $("#removeColumnIcon").show();
                        } else {
                            $("#menuOwner").css("cursor", "default");
                            $("#removeColumnIcon").hide();
                        }
                    } else {
                        $(".e-groupdroparea").css("cursor", "default");
                    }
                });
            },
            SetupAvailableFields: function () {
                if ($.filterModal.Filters.Ids.CurrentId === null) {
                    $.filterModal.Filters.Ids.CurrentId = 0;
                }

                $('.field, fieldGroup').addClass('ui-menu-item');
                $("a:contains('Columns')").click(function () {
                    setTimeout(function () {
                        $(".e-gridcontent").css("width", $("#preview>.e-gridheader").width() + 'px');
                    }, 2000);
                });
                $('.field').mouseover(function () {
                        if (SEL.Reports.IDs.General.TooltipTimer !== null) {
                            clearTimeout(SEL.Reports.IDs.General.TooltipTimer);
                            SEL.Reports.IDs.General.TooltipTimer = null;
                        }
                        var id = $(this).attr('id');
                        var node = SEL.Reports.IDs.General.EasyTree.getNode(id);
                    
                        if (node.children === null) {
                            SEL.Reports.IDs.General.TooltipTimer = setTimeout(function () {
                                var treeItem = SEL.Reports.IDs.General.EasyTree.getNode(id);
                                SEL.Reports.IDs.General.CurrentTooltipFieldId = treeItem.id;
                                if (!$(".e-dragclone").is(":visible")) {
                                    SEL.Reports.Columns.GetFieldComment(treeItem.fieldid);
                                }
                            }, 1000);
                        }
                });
                var timeoutId = null;

                $('#txtsearch').unbind('propertychange input').on('propertychange input', function () {
                    
                    var searchText = $('#txtsearch').val().trim();
                    if (searchText.length > 2) {
                        clearTimeout(timeoutId);
                        timeoutId = setTimeout(function () { SEL.Reports.Columns.FilterColumns(); }, 600);
                    } else {
                        if (searchText.length === 0) {
                            $('.hiddenTreeItem').removeClass('hiddenTreeItem');
                        }
                    }

                });
                

                $("[id$='cmbreporton']").unbind().change(function () {
                    $('#txtsearch').val('');
                    $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis + ', #' + SEL.Reports.IDs.Charts.General.ddlYAxis + ', #' + SEL.Reports.IDs.Charts.General.ddlGroupBy).empty().append(new Option('[None]', '-1'));
                    $('#chartPreview').hide();
                    SEL.Reports.Columns.Refresh();
                });
               
                $('#imgCalc').unbind('click').click(function () {

                    SEL.Reports.Calculated.AddEditCalculatedColumn();
                });
                $('#imgStatic').unbind('click').click(function () {
                    SEL.Reports.Calculated.AddEditCalculatedColumn(null, true);
                });
                $("[id$='pnlFilter']").removeClass('modalpanel formpanel formpanelsmall').css('display', '').css('position', '').css('z-index', '');
                $('#imgFilterModalHelp').remove();
                $("[id$='divFilterModalHeading']").remove();

                $("[id$='pnlFilter']>.formbuttons").remove();
                $('[id$="modFilter_backgroundElement"]').remove();
                $('#divFilter').dialog({
                    open: function ()
                    {
                        var text = SEL.Reports.IDs.CriteriaSelector.Node.text;
                        $('.ui-dialog-title').text('Filter Detail: ' + text);
                        //replace button classes
                        var modalInner = $('#divFilter');
                        var modal = modalInner.parent();
                        $('#btnSaveFilter').focus();
                        var zIndex = SEL.Common.GetHighestZIndex();
                        if (zIndex > 0) {
                            $('.ui-widget-overlay:last').css('zIndex', zIndex + 1);
                            modal.css('zIndex', zIndex + 2);
                        }
                        $('.ui-dialog-titlebar-close').remove();
                    },
                    beforeClose: function (event) {
                        if (event.key === "Esc") {
                            if (SEL.Reports.IDs.General.NewCriteria) {
                                $('[criteriaid="' + $.filterModal.Filters.Ids.EditControl + '"]').parent().remove();
                            }

                            $.filterModal.Filters.FilterModal.ToggleListSelectorVisibility(false);
                            $.filterModal.Filters.Ids.EditControl = null;
                        }

                        var result = SEL.Reports.Criteria.StoreCriteria();
                        return result;
                    },
                    close: function () {
                        SEL.Reports.IDs.General.NewCriteria = false;
                    },
                    autoOpen: false,
                    resizable: false,
                    modal:true,
                    width: '623px'
                });
                if ($('#btnSaveFilter').length === 0) {
                    $('#divFilter').append('<div class="formpanel formbuttons calculatedButtons" style="position:relative;left:-19px;height:30px;z-index:0;float:left;"><span class="buttonContainer"><input type="button" id="btnSaveFilter" value="save" onclick="$(\'#divFilter\').dialog(\'close\');return false;" class="buttonInner" style="padding-bottom:0;padding-top:0;"></span><span class="buttonContainer"><input type="button" value="cancel" onclick="SEL.Reports.Criteria.Cancel();return false; " id="btnCancelFilter" class="buttonInner" style="padding-bottom:0;padding-top:0;"></span></div>');
                }
                
                $('#lblSearch').css('width', 'auto').css('padding-top', '7px;');

                $('#menuOwner').unbind('mouseout').mouseout(function () {
                    if (SEL.Reports.IDs.General.TooltipTimer !== null) {
                        clearTimeout(SEL.Reports.IDs.General.TooltipTimer);
                        SEL.Reports.IDs.General.TooltipTimer == null;
                    }
                });

                $('#dialog-confirm-column-delete').dialog({
                    autoOpen: false,
                    dialogClass: 'no-close',
                    draggable: true,
                    resizable: false,
                    height: 165,
                    width: 320,
                    modal: true
                });

                $('#cancelColumnDelete').click(function () {
                    $('#dialog-confirm-column-delete').dialog('close');
                    SEL.Reports.Columns.TargetColumnIndex = null;
                });

                $('#confirmColumnDelete').click(function () {
                    SEL.Reports.Preview.DeleteColumn(SEL.Reports.Columns.TargetColumnIndex);
                    $('#dialog-confirm-column-delete').dialog('close');

                });
            },
            GroupReportSource: function () {
                var groups = {};
                var selector = "#" + SEL.Reports.IDs.General.ReportOn;
                var value = $(selector).val();

                $(selector + " option[data-category]").each(function () {
                    groups[$.trim($(this).attr("data-category"))] = true;
                });
                $.each(groups, function (c) {
                    $(selector + " option[data-category='" + c + "']").wrapAll('<optgroup label="' + c + '">');
                });

                $(selector).val(value);
            },
            Report: {
                Save: function (source) {
                    SEL.Reports.IDs.Source = source;
                    var params = new SEL.Reports.Misc.WebserviceParameters.SaveReport();
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'SaveReport', params, SEL.Reports.Report.SaveComplete, SEL.Reports.Misc.ErrorHandler);
                },
                SaveComplete: function (data) {
                    if (data.d === '00000000-0000-0000-0000-000000000000') {
                        SEL.MasterPopup.ShowMasterPopup("The report name you have entered already exists.", "Message from " + moduleNameHTML);
                        return false;
                    }

                    if (SEL.Reports.RequestNumber > 0 && SEL.Reports.IDs.Source !== 'Chart') {
                        window.opener.document.location = window.opener.document.location + "&requestnum=" + SEL.Reports.RequestNumber;
                        window.close();
                        return false;
                    }

                    if (SEL.Reports.IDs.Source === 'main') {
                        document.location = "rptlist.aspx";
                    } else {
                        SEL.Reports.ReportID = data.d;
                        changePage(SEL.Reports.IDs.Source);
                    }
                },
                Cancel: function () {
                    document.location = "rptlist.aspx";
                },
                LimitReportChanged: function (checked) {
                    if (checked) {
                        $('.limitReport').css('display', '');
                    } else {
                        $('.limitReport').css('display', 'none');
                        $('#' + SEL.Reports.IDs.General.LimitReport).val('');
                    }
                }
            },
            Columns:
            {
                Add: function(treeNode) {
                    SEL.Reports.Columns.SelectedTreeNodes.push($.extend(true, {}, treeNode));
                },
                AddOrUpdate: function (column) {
                    var treeNode = column.attr;
                    var indexOfExistingColumn = -1;

                    $.each(SEL.Reports.Columns.SelectedTreeNodes, function (index, item) {
                        if (item.id === treeNode.id) {
                            indexOfExistingColumn = index;
                            return;
                        }
                    });

                    if (indexOfExistingColumn > -1) {
                        SEL.Reports.Columns.SelectedTreeNodes.splice(indexOfExistingColumn, 1, $.extend(true, {}, treeNode));
                    } else {
                        SEL.Reports.Columns.SelectedTreeNodes.push($.extend(true, {}, treeNode));
                    }
                },
                Remove: function (treeNode) {
                    var index = $.inArray(treeNode, SEL.Reports.Columns.SelectedTreeNodes);
                    if (index > -1) {
                        SEL.Reports.Columns.SelectedTreeNodes.splice(index, 1);
                    }
                },
                SelectedTreeNodes: [],
                Refresh: function () {
                    $("#treeviewLoading").show();
                    SEL.Reports.Columns.SelectedTreeNodes = [];
                    $('#Criteria .selectedCriteria').each(function () {
                        var criteria = $(this).children().first().attr('criteriaid');
                        $.data(document, criteria, null);
                    });
                    $('#Criteria .selectedCriteria').remove();
                    $('#Dropper .accordionheader').remove();
                    SEL.Reports.IDs.Loading = true;
                    $('#availablefieldstitle').text(SEL.Reports.Misc.Ellipsis($('#' + SEL.Reports.IDs.General.ReportOn + '  option:selected').text(), 25));
                    var params = new SEL.Reports.Misc.WebserviceParameters.GetInitialNodes();
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetEasyTreeNodes', params, SEL.Reports.Columns.RefreshComplete, function () {
                         $("#treeviewLoading").hide();
                         SEL.Reports.Misc.ErrorHandler();
                    });
                },
                TargetColumnIndex: null,
                RefreshComplete: function (data) {
                   $("#treeviewLoading").hide();
                   SEL.Reports.Preview.Refresh();
                    SEL.Reports.IDs.OddRow = true;                    
                    if (data!== undefined && data !== null) {
                        if (SEL.Reports.IDs.General.EasyTree === null) {
                            // dragging in some browsers (basically IE8/9) causes half the text on the page to become highlighted, prevent this
                            $("#pgPreview").disableSelection();
 
                            SEL.Reports.IDs.General.EasyTree = $('#menuOwner').easytree({
                                data: data.d,
                                enableDnd: true,
                                toggled: function (event, nodes, source) {
                                    //double-click of the tree item
                                    if (!SEL.Reports.DisableColumns && source.children === null && !source.isLazy) {
                                        SEL.Reports.Columns.Add(source);
                                        SEL.Reports.Columns.SelectedTreeNodes = SEL.Reports.Columns.MakeUniqueNodeNames(SEL.Reports.Columns.SelectedTreeNodes);
                                        SEL.Reports.Preview.Refresh();
                                    } 
                                },
                                canDrop: function (event, nodes, isSourceNode, source, isTargetNode, target) {
                                    var node = isSourceNode ? source : SEL.Reports.IDs.General.EasyTree.getNode($(source).attr("id"));
                                    return (node.children === null && !node.isLazy);
                                },
                                dropped: function (event, nodes, isSourceNode, source, isTargetNode, target)
                                {
                                    var node = isSourceNode ? source : SEL.Reports.IDs.General.EasyTree.getNode($(source).attr("id"));
                                    if (target.id === "Dropper") {
                                        SEL.Reports.Criteria.CriteriaCount++;
                                        var fieldId = node.fieldid;
                                        SEL.Reports.Criteria.AddEditCriteria(SEL.Reports.Criteria.CriteriaCount, fieldId, node);
                                    } else {
                                        SEL.Reports.Columns.TargetColumnIndex = SEL.Reports.ColumnIndex;
                                        SEL.Reports.Columns.Add(node);
                                        SEL.Reports.Columns.SelectedTreeNodes = SEL.Reports.Columns.MakeUniqueNodeNames(SEL.Reports.Columns.SelectedTreeNodes);
                                        SEL.Reports.Preview.Refresh();
                                        SEL.Reports.IDs.General.PreventBuild = true;
                                    }
                                },
                                openLazyNode: function (event, nodes, node, hasChildren) {
                                    node.lazyUrl = "/shared/webServices/svcReports.asmx/GetBranchEasyTreeNodes";
                                    node.lazyUrlJson = JSON.stringify(new SEL.Reports.Misc.WebserviceParameters.GetBranchNodes(node.internalId, node.fieldid, node.crumbs));
                                    
                                    if (!hasChildren && node.id !== SEL.Reports.IDs.ColumnSelector.CurrentId) {
                                        SEL.Reports.IDs.General.ScrollOffset = $('.ui-easytree').scrollTop();
                                        $('#spacer').css('cursor', 'wait');
                                        $('.ui-easytree').hide(1000);
                                        $('#' + node.id).removeClass('isLazy');
                                        return true;
                                    }

                                    return false;
                                },
                                disableIcons: true,
                                opening: function () {
                                    SEL.Reports.IDs.General.ScrollOffset = $('.ui-easytree').scrollTop();
                                },
                                building: function (nodes) {
                                    if (SEL.Reports.IDs.General.PreventBuild) {
                                        SEL.Reports.IDs.General.PreventBuild = false;
                                        return SEL.Reports.IDs.General.PreventBuild;
                                    }
                                    
                                    return true;
                                },
                                built: function (nodes) {
                                    if (SEL.Reports.IDs.General.ScrollOffset !== null) {
                                        $('#spacer').css('cursor', 'pointer');
                                        $('.ui-easytree').scrollTop(SEL.Reports.IDs.General.ScrollOffset);
                                        SEL.Reports.IDs.General.ScrollOffset = null;
                                        $('.ui-easytree').show();
                                        SEL.Reports.SetupAvailableFields();

                                        var searchText = $('#txtsearch').val().trim();
                                        if (searchText.length > 2) {
                                             SEL.Reports.Columns.FilterColumns();;
                                        }
                                    }

                                    //prevent parent nodes from being dragged
                                    $('.easytree-node:not(.field.f1)').removeClass('easytree-draggable');
                                }
                            });
                            $('.easytree-container').css('overflow-x', 'hidden');
                        } else {
                            SEL.Reports.IDs.General.EasyTree.rebuildTree(data.d);
                        }
                        SEL.Reports.SetupAvailableFields();
                    }
                    
                    if (SEL.Reports.ReportID !== null && SEL.Reports.ReportID !== '00000000-0000-0000-0000-000000000000') {
                        var existingValue = $('#' + SEL.Reports.IDs.General.ItemDetail).val();
                        if (existingValue > '')
                        {
                            SEL.Reports.Columns.SelectedNodesRefreshComplete(JSON.parse(existingValue));
                        }
                        existingValue = $('#' + SEL.Reports.IDs.General.CriteriaDetail).val();
                        if (existingValue > '')
                        {
                            SEL.Reports.Criteria.CriteriaNodesRefreshComplete(JSON.parse(existingValue));
                        }
                        SEL.Reports.CriteriaLoaded = true;
                    }
                    else
                    {
                        SEL.Reports.Criteria.Clear();
                        // default criteria when table is expense items
                        var tableId = $('#' + SEL.Reports.IDs.General.ReportOn + ' option:selected').val();

                        if (tableId.toLowerCase() === 'd70d9e5f-37e2-4025-9492-3bcf6aa746a8') {
                            SEL.Reports.Criteria.CriteriaNodesRefreshComplete(JSON.parse('{"data":[{"data":"Claim Submitted","attr":{"id":"copy_k34012174-7ce8-4f67-8b91-6c44ac1a4845_n47db6e7d-78ac-4322-8211-359ddca0c1ab","internalId":"copy_k34012174-7ce8-4f67-8b91-6c44ac1a4845_n47db6e7d-78ac-4322-8211-359ddca0c1ab","crumbs":"ClaimID","rel":"node","fieldid":"47db6e7d-78ac-4322-8211-359ddca0c1ab","joinviaid":0,"fieldtype":"X","columnid":"","comment":"Has the claim been submitted?"},"state":"","metadata":{"conditionType":1,"criterionOne":"1","criterionTwo":"","conditionTypeText":"Equals","fieldType":"X","isListItem":false,"firstListItemText":"","runtime":false,"conditionJoiner":1,"group":0}},{"data":"Claim Paid","attr":{"id":"copy_k34012174-7ce8-4f67-8b91-6c44ac1a4845_n382d575a-ce76-45ae-847a-7d374383e383","internalId":"copy_k34012174-7ce8-4f67-8b91-6c44ac1a4845_n382d575a-ce76-45ae-847a-7d374383e383","crumbs":"ClaimID","rel":"node","fieldid":"382d575a-ce76-45ae-847a-7d374383e383","joinviaid":0,"fieldtype":"X","columnid":"","comment":"Has the claim been paid?"},"state":"","metadata":{"conditionType":1,"criterionOne":"1","criterionTwo":"","conditionTypeText":"Equals","fieldType":"X","isListItem":false,"firstListItemText":"","runtime":false,"conditionJoiner":1,"group":0}}]}'));
                        }
                        else
                        {
                            SEL.Reports.IDs.Loading = true;
                            return;
                        }

                        SEL.Reports.IDs.Loading = false;
                    }
                },
                MakeUniqueNodeNames: function (treeNodes) {
                    if (treeNodes.length === 1 || treeNodes.length === 0) {
                        return treeNodes;
                    }

                    var columnTexts = [];
                    
                    for (var k = 0; k < treeNodes.length - 1; k++) {
                        columnTexts.push(treeNodes[k].text);
                    }

                    if (columnTexts.contains(treeNodes[treeNodes.length - 1].text) && treeNodes[treeNodes.length - 1].fieldtype !== "W" && treeNodes[treeNodes.length - 1].fieldtype !== "Z") {
                        for (var i = 1; i <= columnTexts.length; i++) {
                            if (!columnTexts.contains(treeNodes[treeNodes.length - 1].text + "(" + i + ")")) {
                                treeNodes[treeNodes.length - 1].text = treeNodes[treeNodes.length - 1].text + "(" + i + ")";
                                break;
                            }
                        }
                    }
                    return treeNodes;

                },
                SelectedNodesRefreshComplete: function (data) {
                    SEL.Reports.IDs.OddRow = true;
                    if (data !== null && data !== undefined) {
                        var treeData = data;
                        for (var i = 0; i < treeData.length; i++) {
                            var treeItem = treeData[i];
                            var newId = treeItem.attr.id;
                            newId = newId.replace('copy_', '');
                            treeItem.attr.id = newId;
                            treeItem.attr.internalId = newId;
                            treeItem.attr.text = treeItem.data;
                            if (treeItem.attr.fieldtype === "W" || treeItem.attr.fieldtype === "Z") {
                                $.data(document, treeItem.attr.id, treeItem);
                            }
                            treeItem.attr.avg = treeItem.metadata.Average;
                            treeItem.attr.count = treeItem.metadata.Count;
                            treeItem.attr.hide = treeItem.metadata.Hidden;
                            treeItem.attr.max = treeItem.metadata.Max;
                            treeItem.attr.min = treeItem.metadata.Min;
                            treeItem.attr.sum = treeItem.metadata.Sum;
                            treeItem.attr.groupby = treeItem.metadata.GroupBy;
                            treeItem.attr.sortorder = treeItem.metadata.SortOrder;


                            SEL.Reports.Columns.Add(treeItem.attr);
                        }
                    }
                },
                BranchRefreshComplete: function (data) {
                    SEL.Reports.IDs.OddRow = true;
                    if (data !== null) {
                        var parent = SEL.Reports.IDs.General.EasyTree.getNode(SEL.Reports.IDs.ColumnSelector.CurrentId);
                        parent.children = data.d;
                        parent.isLazy = false;
                        if (parent.liClass !== null) {
                            parent.liClass = parent.liClass.replace('isLazy', '');
                        }

                        SEL.Reports.IDs.General.ScrollOffset = $('.ui-easytree').scrollTop();
                        SEL.Reports.IDs.General.EasyTree.rebuildTree();

                        SEL.Reports.SetupAvailableFields();
                    }
                },
                FormatEasyTreeNode: function (treeItem) {
                    var formattedItem = $(SEL.Reports.FormatListItem(treeItem, name)), sourceNode = {}, usedItem = '';
                    sourceNode.text = treeItem.data;
                    sourceNode.id = treeItem.attr.id;
                    if ($('#Dropper #' + sourceNode.id).length > 0) {
                        usedItem = ' state-disabled';
                        if ($("#Tabs").tabs("option", "active") === 0) {
                            usedItem = usedItem + ' iconDisabled';
                        }
                    }
                    sourceNode.isFolder = false;
                    sourceNode.liClass = formattedItem.attr('class') + usedItem;
                    if (treeItem.attr.rel !== "node") {
                        sourceNode.liClass = sourceNode.liClass + ' isLazy';
                        sourceNode.isLazy = true;
                    }

                    return sourceNode;
                },
                FilterColumns: function () {
                    $('#menuOwner').addClass('searching');
                    // Call timeout so that the UI is updated with the opacity.
                    setTimeout(function() {
                        SEL.Reports.Columns.ExpandNodesForFilter();
                        SEL.Reports.SetupAvailableFields();
                        $('#menuOwner').removeClass('searching');
                    }, 0);
                },
                ExpandNodesForFilter: function () {
                    var searchTerm = $('#txtsearch').val().toLowerCase(), searchTarget;
                    $('.hiddenTreeItem').removeClass('hiddenTreeItem');
                    if (searchTerm === '') {
                        return;
                    }

                    var easyTree = SEL.Reports.IDs.General.EasyTree;
                    var nodes = easyTree.getAllNodes();
                    var i = 0;
                    for (i = 0; i < nodes.length; i++) {
                        nodes[i].liClass = nodes[i].liClass.replace('treeSearch', '');
                        searchTarget = nodes[i].text ? nodes[i].text : "";

                        if (searchTarget.toLowerCase().indexOf(searchTerm) > -1) {
                            nodes[i].liClass = nodes[i].liClass + ' treeSearch';
                            nodes[i].isExpanded = true;
                        }

                        // if has children, check if they have the text, open the node
                        if (nodes[i].children && nodes[i].children.length > 0) {
                            nodes[i].liClass = nodes[i].liClass.replace('treeSearch', '');
                            nodes[i].isExpanded = SEL.Reports.Columns.SearchNodes(searchTerm, nodes[i].children);
                            if (nodes[i].isExpanded) {
                                nodes[i].liClass = nodes[i].liClass + ' treeSearch';
                            }
                        }
                    }
                    easyTree.rebuildTree();
                    $('#menuOwner li').each(function () {
                        if ($(this).find('.treeSearch').length === 0) {
                            $(this).addClass('hiddenTreeItem');
                        }
                    });

                },
                SearchNodes: function(searchTerm, nodes) {
                    var i = 0;
                    var expand = false;
                    var searchTarget;
                    for (i = 0; i < nodes.length; i++) {
                        nodes[i].liClass = nodes[i].liClass.replace('treeSearch', '');
                        searchTarget = nodes[i].text ? nodes[i].text : "";
                        if (searchTarget.toLowerCase().indexOf(searchTerm) > -1) {
                            nodes[i].liClass = nodes[i].liClass + ' treeSearch';
                            expand = true;
                        }

                        if (nodes[i].children && nodes[i].children.length > 0) {
                            nodes[i].isExpanded = SEL.Reports.Columns.SearchNodes(searchTerm, nodes[i].children);
                            if (nodes[i].isExpanded) {
                                nodes[i].liClass = nodes[i].liClass + ' treeSearch';
                                expand = true;
                            }
                        }
                    }

                    return expand;
                },
                CompareArray: function (arr1, arr2) {
                    if (arr1.length !== arr2.length) return false;
                    for (var i = 0, len = arr1.length; i < len; i++) {
                        if (arr1[i] !== arr2[i]) {
                            return false;
                        }
                    }
                    return true;

                },
                GetFieldComment: function (fieldId) {
                    var GetFieldCommentComplete = function (data) {
                        var isOpen = $('#divCalculation').parent().hasClass('ui-dialog');
                        if (data.d !== undefined && data.d !== null && data.d > '' && !isOpen) {
                            SEL.Tooltip.Show('<div>' + data.d + '</div>', 'sm', $('#' + SEL.Reports.IDs.General.CurrentTooltipFieldId)[0]);
                            $('.tooltipcontainer').css('left', '380px');
                            var top = $('.tooltipcontainer').position().top - 15;
                            $('.tooltipcontainer').css('top', top + 'px');
                        }
                    };
                    var params = { fieldGuid: fieldId };
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetFieldComment', params, GetFieldCommentComplete, SEL.Reports.Misc.ErrorHandler);
                }
            },
            Criteria:
            {
                    CriteriaNodesRefreshComplete: function (data) {
                    SEL.Reports.IDs.OddRow = true;
                    SEL.Reports.IDs.CriteriaId = 1;
                    if (data !== null && data !== undefined && data.data !== undefined) {
                        var treeData = data.data;
                        for (var i = 0; i < treeData.length; i++) {
                            var treeItem = treeData[i];
                            treeItem["text"] = treeItem.data;
                            SEL.Reports.IDs.CriteriaSelector.Node = treeItem;
                            $.filterModal.Filters.Ids.FilterFieldID = treeItem.attr.fieldid;
                                    treeItem.attr.id = treeItem.attr.id.replace('copy_', '');
                                    treeItem.attr.internalId = treeItem.attr.id;
                                    SEL.Reports.Criteria.InsertCriteria(treeItem);
                                    SEL.Reports.IDs.CriteriaId++;
                                    SEL.Reports.Criteria.GroupCount = treeItem.metadata.group + 1 > SEL.Reports.Criteria.GroupCount ? treeItem.metadata.group + 1 : SEL.Reports.Criteria.GroupCount;
                            }
                        SEL.Reports.Criteria.CriteriaCount = treeData.length;
                        SEL.Reports.Criteria.InsertGroup(SEL.Reports.Criteria.GroupCount);
                    }
                    SEL.Reports.IDs.Loading = false;
                    if (SEL.Reports.Columns.SelectedTreeNodes.length) {
                        SEL.Reports.Preview.Refresh();
                    }
                },
                    Clear: function () {
                        var critiera = SEL.Reports.Criteria.GetSelectedCriteria();
                        for (var i = 0; i < critiera.length; i++) {
                            var item = critiera[i];
                            SEL.Reports.Criteria.Remove(item.domId);
                        }
                    },
                    SetupSortable: function () {
                    $('#criteriaList').sortable({
                        cancel: '.groupCriteria,.e-headercell',
                        items: 'tr:not(.e-headercell,.groupCriteria,.e-gridheader)',
                        start: function () {
                            setTimeout(function () {
                                for (var i = 1; i <= 8; i++) {
                                    var width = $("#criteriaListHeader td:nth-child(" + i + ")").width() - 5;
                                    $(".ui-sortable-helper td:nth-child(" + i + ")").css("width", width + "px");
                                }}, 50);
                        },
                        stop: function ()
                        {
                            SEL.Reports.Criteria.ReorderFilterIds();
                            SEL.Reports.Criteria.ShowGroupIcon();
                            SEL.Reports.Preview.Refresh();
                        }
                    });
                },
                    ReorderFilterIds: function () {
                        var criteriadivs = $('.filterInfo');
                        criteriadivs.removeClass('e-row').removeClass('e-alt_row').find('.andor').show();
                        criteriadivs.first().find('.andor').hide();
                        criteriadivs.filter(':even').addClass("e-row");
                        criteriadivs.filter(':odd').addClass("e-alt_row");

                        if ($('.filterInfo').length === 0)
                        {
                            $('.filterHeader').show();
                            $('#criteriaList').hide();
                        } else {
                            $('.filterHeader').hide();
                            $('#criteriaList').show();
                        }
                    },
                    AddGroup: function (groupId) {

                        var enabledCheckboxes = $("#criteriaList").find("input[type='checkbox']:checked"),
                            enabledCheckboxesHaveGroup = enabledCheckboxes.parent().next().hasClass("hasGroupCriteria"),
                            checkboxesWithoutGroups = $("#criteriaList").find("input:checkbox:not(:checked)");
                            
                        if (!enabledCheckboxesHaveGroup) {
                            $(enabledCheckboxes).each(function ()
                            {
                                if (!$(this).parent().hasClass("hasGroupCriteria")) {
                                    $(this).parent().addClass("hasGroupCriteria").attr('group', groupId).addClass('groupCriteria');
                                    $(this).parent().parent().attr('group', groupId).addClass('groupCriteria');
                                }
                            });


                            $(".groupIcon").attr("src", "/shared/images/icons/group_disabled.png").removeAttr("onClick").removeClass("selectGroup");
                            
                            SEL.Reports.Criteria.GroupCount++;
                            SEL.Reports.Criteria.InsertGroup(SEL.Reports.Criteria.GroupCount);
                            SEL.Reports.Preview.Refresh();
                        }
                        else {
                            SEL.MasterPopup.ShowMasterPopup("A group already exists for selected filters.");
                    }
                },
                    Ungroup: function (groupId) {
                        var group = $("[group = " + groupId + "]");

                        $(group).find("input[type='checkbox']").removeAttr('disabled');
                        $(group).find('.hasGroupCriteria').next().removeClass('cholder').removeClass('group-first').removeClass('group-last').find("img").remove();
                        $("[group = " + groupId + "]").removeClass('hasGroupCriteria').removeClass('groupCriteria').attr('group', '');
                        SEL.Reports.Preview.Refresh();
                },
                    InsertCriteria: function (treeItem) {
                        //show elements
                        $('.filterHeader').hide();
                        $('#criteriaList').show();
                    // add the data
                    $.data(document, "filter" + SEL.Reports.IDs.CriteriaId.toString(), treeItem.metadata);
                    // insert into div
                    SEL.Reports.IDs.CriteriaSelector.Node = treeItem.attr;
                    SEL.Reports.IDs.CriteriaSelector.Node["text"] = treeItem["data"];
                    // insert criteria and groups
                    $('#criteriaList').append(SEL.Reports.Criteria.DisplaySummaryFilterInformation("filter" + SEL.Reports.IDs.CriteriaId.toString()));                 
                   
                    $.filterModal.Filters.FilterModal.EditControl = null;
                    $.filterModal.Filters.Ids.EditControl = null;

                    },
                    InsertGroup: function(groupCount){
                        for (var i = 1; i < groupCount; i++) {
                            var group = $("td[group = " + i + "]");
                            if ($(group).first().next().find('img').length === 0) {
                                $(group).first().next().addClass('group-first').append("<img src='/shared/images/icons/group.png' style='cursor:pointer;margin-left:7px;' title='Ungroup filters' onclick='SEL.Reports.Criteria.Ungroup(" + i + ");'/>");
                                $(group).last().next().addClass('group-last');
                            }
                            $(group).next().addClass('cholder');
                        }

                        $('.hasGroupCriteria').find("input[type='checkbox']").attr('disabled', 'true').prop('checked', false);
                        $('.hasGroupCriteria').parent().addClass('groupCriteria');

                    },
                    ChangeCondition: function (criteriaId) {
                        var andOrButton = $('#filter'+criteriaId).find('.andor');
                        if (andOrButton.attr('value') === "1") {
                            andOrButton.attr('value', '2');
                            andOrButton.html("Or");
                        }
                        else {
                            andOrButton.attr('value', '1');
                            andOrButton.html("And");
                        }
                        SEL.Reports.Preview.Refresh();
                    },
                    GetSelectedCriteria: function () {
                    var result = [];
                        $('.filterInfo').not('.filterHeader').each(function () {
                            var group = $(this).find('.hasGroupCriteria'),
                                groupnum = $(this).attr('group');
                            if (group.length > 1) {
                                var conditionType = 249;
                                var conditionJoiner = 1;
                                if ($(this).find('button').attr('value') === "2") {
                                conditionType = 250;
                                conditionJoiner = 2;
                            }
                               
                                var metaData = { conditionType: conditionType, criterionOne: '', criterionTwo: '', conditionTypeText: '', fieldType: '', isListItem: '', firstListItemText: '', conditionJoiner: conditionJoiner};
                                metaData.group = groupnum;
                                var attributes = { id: groupnum, fieldid: '', columnid: groupnum, crumbs: '' };
                                var javascriptTreeNode = { data: groupnum, attr: attributes, metadata: metaData };
                            result.push(javascriptTreeNode);
                        }

                    });
                        $('.filterInfo').not('.filterHeader').each(function ()
                        {
                        var menuItem = $(this);
                            var group = menuItem.attr('group');
                        var metaData = $.data(document, $(menuItem).attr('id'));
                            var criterionOne = metaData.criterionOne === null ? '' : metaData.criterionOne;
                            var criterionTwo = metaData.criterionOne === null ? '' : metaData.criterionTwo;
                        metaData.group = group;
                        var groupNumber = $(menuItem).attr('group') === "" ? 0 : parseInt($(menuItem).attr('group'));
                        var selectedCriteria = { FieldId: $(menuItem).attr('fieldid'), Id: $(menuItem).attr('nodeId'), Condition: metaData.conditionType, Joiner: $(menuItem).find('button').attr('value'), Value1: criterionOne.split("#"), Value2: criterionTwo.split("#"), Order: 0, RunTime: metaData.runtime, JoinViaId: $(menuItem).attr('joinviaid'), Crumbs: $(menuItem).attr('crumbs'), Group: groupNumber, domId: $(menuItem).attr('id') };
                        result.push(selectedCriteria);
                    });

                    return result;
                },
                    StoreCriteria: function () {
                    if ($.filterModal.Filters.Ids.EditControl === null) {
                        return true;
                    }
                    var result = $.filterModal.Filters.FilterModal.Save('vgFilter');

                    if (result) {
                        $('.filterHeader').hide();
                        $('#criteriaList').show();
                        
                        if ($('#filter' + $.filterModal.Filters.Ids.CurrentId).length == 0) {
                            $('#criteriaList').append(SEL.Reports.Criteria.DisplaySummaryFilterInformation('filter' + $.filterModal.Filters.Ids.CurrentId));
                        }
                        else {
                           $(SEL.Reports.Criteria.DisplaySummaryFilterInformation('filter' + $.filterModal.Filters.Ids.CurrentId)).insertAfter($('#filter' + $.filterModal.Filters.Ids.CurrentId));
                           $('#filter' + $.filterModal.Filters.Ids.CurrentId).remove();
                           SEL.Reports.Criteria.InsertGroup(SEL.Reports.Criteria.GroupCount);
                        }

                        $.filterModal.Filters.FilterModal.EditControl = null;
                        $.filterModal.Filters.Ids.EditControl = null;
                        SEL.Reports.Preview.Refresh();
                        SEL.Reports.Criteria.ReorderFilterIds();
                    }

                    return result;
                },
                    GenerateCriteriaInformationForListItem: function (nodeObj) {
                    var firstListItemText = nodeObj.firstListItemText,
                    criteriaText = nodeObj.criterionOne === null ? '' : nodeObj.criterionOne,
                    listItems = criteriaText.split(',');

                    firstListItemText = SEL.Reports.Misc.Ellipsis(firstListItemText);


                    if (listItems.length === 1) {
                        criteriaText = firstListItemText;
                    }
                    else {
                        criteriaText = firstListItemText;
                        criteriaText += ' and ';
                        criteriaText += (listItems.length - 1) + ' other';

                        if (listItems.length > 2) { criteriaText += 's'; }
                    }

                    return criteriaText;
                    },
                    DisplaySummaryFilterInformation: function (criteriaId) {                        
                        // Get information from metadata
                        var nodeObj = $.data(document, criteriaId), criteria1 = nodeObj.criterionOne === null ? '' : nodeObj.criterionOne,
                            criteria2 = nodeObj.criterionTwo === null ? '' : nodeObj.criterionTwo,
                            conditionType = typeof nodeObj.conditionType === typeof undefined ? '' : nodeObj.conditionType,
                            conditionTypeText = typeof nodeObj.conditionTypeText === typeof undefined ? ' ' : nodeObj.conditionTypeText,
                            isListItem = nodeObj.isListItem,
                            filterType = nodeObj.fieldType === null ? '' : nodeObj.fieldType,
                            filterCellInfo,
                            criteriaCellInfo,
                            fullCriteriaValue = '',
                            group = nodeObj.group === (null || undefined) ? 0 : nodeObj.group,
                            enterAtRuntime = nodeObj.runtime;
                        
                        var itemCountEven = $('.filterInfo').not('.filterHeader').length % 2 === 0;
                        var rowClass = itemCountEven ?'e-row' : 'e-alt_row';
                        var onClick = ' SEL.Reports.Criteria.AddEditCriteria(' + criteriaId.replace('filter', '') + ',"' + $.filterModal.Filters.Ids.FilterFieldID + '",' + JSON.stringify(SEL.Reports.IDs.CriteriaSelector.Node).replace("'", "&##39;") + ');';
                        var deleteOnClick = 'SEL.Reports.Criteria.Remove("' + criteriaId + '")';
                        var groupSelectOnChange = 'SEL.Reports.Criteria.ShowGroupIcon();';
                        var andOrOnClick = 'SEL.Reports.Criteria.ChangeCondition(' + criteriaId.replace('filter', '') + ');return false;';
                        var editIcon = "<td class='e-rowcell' style='cursor:pointer;'><img id='" + criteriaId + "edit' src='/shared/images/icons/edit.png' border='0' title='Edit Filter' onclick='" + onClick + "'/>&nbsp; </td>";
                        var deleteIcon = "<td class='e-rowcell' style='cursor:pointer;'><img id='deleteicon'" + criteriaId + "   class=' ' src='/shared/images/icons/delete2.png' border='0' title='Remove Filter' onclick='" + deleteOnClick + "' />&nbsp; </td>";
                        var groupIcon = "<td></td>", checkboxDisable = "";
                        var andOrText = "And";
                        var andOrValue = "1";
                        if (nodeObj.conditionJoiner === 2) {
                            andOrText = "Or";
                            andOrValue = "2";
                        }
                        if ($("#" + criteriaId).length != 0) {
                            group = $("#" + criteriaId).attr('group');
                            andOrValue = $("#" + criteriaId).find('button').attr('value');
                            andOrText = andOrValue == 1 ? "And" : "Or";
                        };

                        var groupClass = group === 0 || group === "0" ? "" : "hasGroupCriteria";
                        var groupSelect = "<td class='e-rowcell "+ groupClass +"' group='" + group + "' colspan='1'><input id='" + criteriaId + "groupSelect' type='checkbox' title='Select to group criteria' " + checkboxDisable + "onchange='" + groupSelectOnChange + "' />&nbsp </td>";
                        var andOr = "<td class='e-rowcell' style=''><button class='andor' ";
                        if (criteriaId === 'filter1') andOr = andOr + "style='display:none' ";
                        andOr = andOr + "onclick='" + andOrOnClick + "' value=" + andOrValue + " >" + andOrText + "</button></td>";

                        // Set the condition text to display
                        filterCellInfo = "<tr class='filterInfo e-grid " + rowClass + "' id='" + criteriaId + "' nodeid='" + SEL.Reports.IDs.CriteriaSelector.Node.internalId + "' fieldid='" + $.filterModal.Filters.Ids.FilterFieldID + "' fieldtype='" + SEL.Reports.IDs.CriteriaSelector.Node.fieldType + "' joinviaid='" + SEL.Reports.IDs.CriteriaSelector.Node.joinviaid + "' crumbs='' group = '" + group + "'>" + editIcon + deleteIcon + groupSelect + groupIcon + "&nbsp;" + andOr + '<td class="e-rowcell">' + SEL.Reports.IDs.CriteriaSelector.Node.text + '</td><td class="e-rowcell">' + conditionTypeText + '</td>';
                    
                        if (enterAtRuntime) {
                            criteria1 = "I'll decide when I run the report";
                            criteriaCellInfo = '';
                        } else
                        {

                        // If adding a new filter or the metadata is not set correctly, set criteria1 accordingly
                        if (criteria1 === undefined || criteria1 === '' || criteria1 === ' ') {
                            criteria1 = '';
                        }

                        // If the filter is a Yes/No, set criteria1 accordingly
                        if (filterType === 'X') {
                            criteria1 = criteria1 === '1' ? 'Yes' : 'No';
                        }

                        var reBracketedCriterion = criteria1.replace(/&lt;/g, '<').replace(/&gt;/g, '>');

                        // If the filter is a List, set the list information
                        if (isListItem) {
                            if (criteria1 !== "@ME" && criteria1 !== "@MY_HIERARCHY") {
                                if (conditionType.toString() !== '10' && conditionType.toString() !== '9') {
                                    criteria1 = SEL.Reports.Criteria.GenerateCriteriaInformationForListItem(nodeObj);
                                }
                            }
                        }
                        else if (reBracketedCriterion.length > 25) {
                            fullCriteriaValue = 'title="' + criteria1 + '"';

                            criteria1 = reBracketedCriterion;
                            criteria1 = criteria1.substring(0, 25) + "...";
                            criteria1 = criteria1.replace(/</g, '&lt;').replace(/>/g, '&gt;');
                        }

                        // Set the criteria text to display
                        criteriaCellInfo = fullCriteriaValue;
                    }
                    // If the filter has a condition of "Between", display both values
                    if (conditionType.toString() === '8') {
                        var andString = ' and ';

                        if ($('#' + $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime).length !== 0) {
                            enterAtRuntime = $('#' + $.filterModal.options.domainRoot.DomIDs.Filters.FilterModal.Runtime).prop('checked');
                        }
                        if (enterAtRuntime || criteria2 === '') {
                            andString = '';
                        }

                        criteriaCellInfo = criteriaCellInfo + criteria1 + andString + criteria2;
                    }
                    else {
                        criteriaCellInfo += criteria1;
                    }


                    return filterCellInfo + "<td class='e-rowcell'>" + criteriaCellInfo + "</td></tr>";                   
                },
                AddEditCriteria: function (criteriaCount, fieldId, node) {
                    SEL.Reports.IDs.CriteriaSelector.Node = node;
                    $.filterModal.Filters.Ids.CurrentId = criteriaCount;
                    $.filterModal.Filters.FilterModal.ClearForm(false);
                    $.filterModal.Filters.Ids.EditControl = '#divFilter';
                    $.filterModal.Filters.FilterModal.EditControl = '#divFilter';

                    $.filterModal.Filters.FilterModal.EditTorchMode(fieldId);
                    $.filterModal.Filters.FilterModal.ChangeFilterCriteria(true, false);
                    $.filterModal.Filters.FilterModal.ExtractMetaDataToControls();
                    $('#divFilter').dialog('open');
                },
                CriteriaCount: 0,
                GroupCount: 1,
                Remove: function(criteriaId) {
                    $.data(document, criteriaId, null);
                    var groupId = $('#' + criteriaId).attr('group');
                    $('#' + criteriaId).remove();
                    var group = $("[group = " +groupId + "]").find('.hasGroupCriteria');
                    if (group.length == 1)
                    {
                        SEL.Reports.Criteria.Ungroup(groupId);
                    }
                    else {
                        SEL.Reports.Preview.Refresh();
                    };

                    SEL.Reports.Criteria.ShowGroupIcon();
                    SEL.Reports.Criteria.ReorderFilterIds();
                    },
                    Collapse: function(){
                    $('#criteriaContainer').click(function ()
                    {
                        $('#Dropper').slideToggle('fast');
                            if ($(this).hasClass("closeArrow")) {
                                $(this).removeClass("closeArrow").addClass("openArrow");
                            } else {
                                $(this).removeClass("openArrow").addClass("closeArrow");
                            }
                    });
                    },
                    ShowGroupIcon: function () {
                        var str = "";
                        if ($("#criteriaList").find('input[type="checkbox"]:checked').length > 1){
                            $("#criteriaList").find('input[type="checkbox"]').each(function () {
                                str += this.checked ? "1" : "0";
                                var firstIndex = str.indexOf("1");
                                var lastIndex = str.lastIndexOf("1");
                                var subStr = str.substring(firstIndex, lastIndex);
                                if (subStr.indexOf("0") == -1) {
                                    $(".groupIcon").attr({ src: "/shared/images/icons/group.png", onClick: "SEL.Reports.Criteria.AddGroup(" + SEL.Reports.Criteria.GroupCount + ");" }).addClass("selectGroup");
                                }
                                else {
                                    $(".groupIcon").attr("src", "/shared/images/icons/group_disabled.png").removeAttr("onClick").removeClass("selectGroup");
                                }
                            });
                }
                        else {
                            $(".groupIcon").attr("src", "/shared/images/icons/group_disabled.png").removeAttr("onClick").removeClass("selectGroup");
                        }             
                    },
                Cancel:function() {
                    if (SEL.Reports.IDs.General.NewCriteria !== true) {
                        $.filterModal.Filters.FilterModal.ExtractMetaDataToControls();
                    } else {
                        $('[criteriaid="' + $.filterModal.Filters.Ids.EditControl + '"]').parent().remove();
                    }
                    $('.filter-search').val(' Search..').hide();
                    $.filterModal.Filters.FilterModal.Hide();
                    $.filterModal.Filters.Ids.EditControl = null;
                    $.filterModal.Filters.FilterModal.ToggleListSelectorVisibility(false);
                    $('#divFilter').dialog('close');
                }
            },
            Chart:
            {
                SelectedType: function (data, type) {
                    $(".SelectedType").each(function () { $(this).attr("class", "Type"); });
                    data.className = "SelectedType";
                    $('.cumulative').css('display', 'none');
                    $('.percent').css('display', 'none');
                    $('.combine').css('display', 'none');
                    $('.groupby').css('display', '');
                    $('.groupbylabel').text('Group by');
                    $('.xaxislabel').text('X-Axis');
                    $('.yaxislabel').text('Y-Axis');
                    switch (type) {
                        case "area":
                            $('#' + SEL.Reports.IDs.Charts.General.chkShowPercent).prop('checked', false);
                            break;
                        case "bar":
                            $('#' + SEL.Reports.IDs.Charts.General.chkShowPercent).prop('checked', false);
                            break;
                        case "column":
                            $('#' + SEL.Reports.IDs.Charts.General.chkShowPercent).prop('checked', false);
                            break;
                        case "donut":
                            $('.percent').css('display', '');
                            $('.groupby').css('display', 'none');
                            $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy + ' option:selected').text('[None]');
                            $('.xaxislabel').text('Values');
                            $('.yaxislabel').text('Wedges');
                            $('.combine').css('display', '');
                            break;
                        case "dot":
                            $('#' + SEL.Reports.IDs.Charts.General.chkShowPercent).prop('checked', false);
                            $('.groupby').css('display', 'none');
                            $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy + ' option:selected').text('[None]');
                            break;
                        case "line":
                            $('#' + SEL.Reports.IDs.Charts.General.chkShowPercent).prop('checked', false);
                            break;
                        case "pie":
                            $('.percent').css('display', '');
                            $('.groupby').css('display', 'none');
                            $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy + ' option:selected').text('[None]');
                            $('.xaxislabel').text('Values');
                            $('.yaxislabel').text('Wedges');
                            $('.combine').css('display', '');
                            break;
                        case "funnel":
                            $('.groupby').css('display', 'none');
                            $('.percent').css('display', '');
                            $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy + ' option:selected').text('[None]');
                            break;
                        default:
                    }

                    $('.Type, .SelectedType').css('cursor', 'pointer');
                    SEL.Reports.Preview.Refresh(false);
                },
                ShowLegendChanged: function (checked) {
                    if (checked) {
                        $('.legendPosition').css('display', '');
                    } else {
                        $('.legendPosition').css('display', 'none');
                    }
                },
                PopulateField: function (index, node) {
                    var nodeText,
                        nodeId;
                    nodeText = node.attributename;
                    nodeId = index;

                    var addItemToList = function (nodeT, nodeI, object) {
                        if ($("#" + object + " option[value='" + nodeI + "']").length === 0) {
                            var o = new Option(nodeT, nodeI);
                            $("#" + object).append(o);
                            /// jquerify the DOM object 'o' so we can use the html method
                            $(o).html(nodeT);
                            $("#" + object).append(o);
                        }
                    };

                    var groupBy = node.groupby;
                    var count = node.count;
                    var sum = node.sum;
                    var average = node.average;
                    var max = node.max;
                    var min = node.min;
                    if (node.fieldType !== "W" && node.fieldType !== "Z") {
                        addItemToList(nodeText, nodeId, SEL.Reports.IDs.Charts.General.ddlXAxis);
                        if (count || max || min || sum || average) {
                            addItemToList(nodeText, nodeId, SEL.Reports.IDs.Charts.General.ddlYAxis);
                        }

                        if (groupBy) {
                            addItemToList(nodeText, nodeId, SEL.Reports.IDs.Charts.General.ddlGroupBy);
                        }
                    }
                },
                PopulateFields: function () {
                    var oldXAxis = $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis + ' option:selected').text();
                    var oldYAxis = $('#' + SEL.Reports.IDs.Charts.General.ddlYAxis + ' option:selected').text();
                    var oldGroupBy = $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy + ' option:selected').text();
                    var columns = SEL.Reports.GetSelectedColumns();
                    if (columns.length === 0 && oldXAxis !== '[None]') {
                        return;
                    }

                    $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis + ', #' + SEL.Reports.IDs.Charts.General.ddlYAxis + ', #' + SEL.Reports.IDs.Charts.General.ddlGroupBy).empty().append(new Option('[None]', '-1'));
                    
                    for (var i = 0; i < columns.length; i++) {
                        SEL.Reports.Chart.PopulateField(i, columns[i]);
                    }

                    $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis + ' option').each(function () {
                        if ($(this).text() === oldXAxis) {
                            $(this).attr('selected', 'selected');
                        }
                    });
                    $('#' + SEL.Reports.IDs.Charts.General.ddlYAxis + ' option').each(function () {
                        if ($(this).text() === oldYAxis) {
                            $(this).attr('selected', 'selected');
                        }
                    });

                    $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy + ' option').each(function () {
                        if ($(this).text() === oldGroupBy) {
                            $(this).attr('selected', 'selected');
                        }
                    });
                },

            },

            Misc:
            {
                ErrorHandler: function (data) {
                    SEL.Reports.IDs.Loading = false;
                    SEL.Common.WebService.ErrorHandler(data);
                },
                SaveReport: function (data) {
                    if (validateform('vgMain') === false) {
                        return;
                    }
                    // find all criteria elements if any have no $.data entry then validate
                    var unValidatedFilters = false;
                    $('#Criteria>.ui-menu-item').each(function () {
                        if ($.data(document, $(this).attr('criteriaid')) === null || $.data(document, $(this).attr('criteriaid')) === undefined) {
                            unValidatedFilters = true;
                            return;
                        }
                    });

                    if (unValidatedFilters) {
                        if (validateform('vgFilter') === false) {
                            return;
                        }
                    }

                    if ($('#' + SEL.Reports.IDs.General.LimitReport).css('display') !== 'none') {
                        if (validateform('vgLimit') === false) {
                            return;
                        }
                    }
                    SEL.Reports.Report.Save(data);
                },
                WebserviceParameters: {
                    GetInitialNodes: function () {
                        SEL.Reports.TableID = $('#' + SEL.Reports.IDs.General.ReportOn + ' option:selected').val();
                        this.baseTableString = SEL.Reports.TableID;
                    },
                    GetSelectedNodes: function () {
                        this.requestNumber = SEL.Reports.RequestNumber;
                        this.reportGuidString = SEL.Reports.ReportID;
                    },
                    GetBranchNodes: function (id, fieldId, crumbs) {
                        this.fieldID = fieldId;
                        this.crumbs = crumbs;
                        this.nodeID = id;
                    },
                    GetSelectedFilterData: function () {
                        this.reportGuidString = SEL.Reports.ReportID;
                        this.requestNumber = SEL.Reports.RequestNumber;
                    },
                    SaveReport: function () {
                        this.reportid = SEL.Reports.ReportID;
                        this.requestNumber = SEL.Reports.RequestNumber;
                        this.reportName = $('#' + SEL.Reports.IDs.General.ReportName).val();
                        this.reportDescription = $('#' + SEL.Reports.IDs.General.Description).val();
                        this.category = $('#' + SEL.Reports.IDs.General.Category + ' option:selected').val();
                        this.reportOn = $('#' + SEL.Reports.IDs.General.ReportOn + ' option:selected').val();
                        this.claimant = $('#' + SEL.Reports.IDs.General.Claimant).prop("checked");

                        this.fields = SEL.Reports.GetSelectedColumns();
                        this.reportCriteria = SEL.Reports.Criteria.GetSelectedCriteria();
                        this.criteriaLoaded = SEL.Reports.CriteriaLoaded;
                        this.columnsLoaded = SEL.Reports.ColumnsLoaded;
                        this.showChart = $('#' + SEL.Reports.IDs.Charts.General.ddlShowChart).val();
                        if (this.showChart === undefined || this.showChart === null) {
                            this.showChart = 0;
                        }
                        this.limitReport = $('#' + SEL.Reports.IDs.General.LimitReport).val();
                        if (this.limitReport === "") {
                            this.limitReport = "0";
                        }
                        this.reportChart = new SEL.Reports.Misc.WebserviceParameters.ReportChart();
                    },
                    PreviewReport :function(){
                        this.reportName = $('#' + SEL.Reports.IDs.General.ReportName).val();
                        this.reportOn = $('#' + SEL.Reports.IDs.General.ReportOn + ' option:selected').val();
                        this.claimant = $('#' + SEL.Reports.IDs.General.Claimant).prop("checked");
                        this.reportFields = SEL.Reports.GetSelectedColumns();
                        this.reportCriteria = SEL.Reports.Criteria.GetSelectedCriteria();
                        this.reportChart = new SEL.Reports.Misc.WebserviceParameters.ReportChart();
                        this.reportChart.Size = 8;
                    },
                    ReportChart: function () {
                        this.reportid = SEL.Reports.ReportID;
                        this.displayType = $(".SelectedType").attr('ChartType');
                        if (this.displayType === undefined) {
                            this.displayType = '1';
                        }

                        this.ChartTitle = $('#' + SEL.Reports.IDs.Charts.General.txtChartTitle).val();
                        this.ShowLegend = $('#' + SEL.Reports.IDs.Charts.General.chkShowLegend).is(':checked');
                        this.XAxis = $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis).val();
                        this.YAxis = $('#' + SEL.Reports.IDs.Charts.General.ddlYAxis).val();
                        this.GroupBy = $('#' + SEL.Reports.IDs.Charts.General.ddlGroupBy).val();
                        this.Cumulative = false;
                        this.ChartTitleFont = $('#' + SEL.Reports.IDs.Charts.General.ddlChartTitleFont).val();
                        this.ChartTitleColour = $('#' + SEL.Reports.IDs.Charts.General.txtChartTitleColour).val();
                        this.TextFont = $('#' + SEL.Reports.IDs.Charts.General.ddlTextFont).val();
                        this.TextFontColour = $('#' + SEL.Reports.IDs.Charts.General.txtTextFontColour).val();
                        this.TextBackgroundColour = $('#' + SEL.Reports.IDs.Charts.General.txtTextBackgroundColour).val();
                        this.ShowValues = $('#' + SEL.Reports.IDs.Charts.General.chkShowValues).is(':checked');
                        this.ShowPercent = $('#' + SEL.Reports.IDs.Charts.General.chkShowPercent).is(':checked');
                        this.Size = $('#' + SEL.Reports.IDs.Charts.General.ddlChartSize).val();
                        this.ShowLabels = $('#' + SEL.Reports.IDs.Charts.General.chkShowLabels).is(':checked');
                        this.LegendPosition = $('#' + SEL.Reports.IDs.Charts.General.ddlLegendPosition).val();
                        this.CombineOthersPercentage = $('#' + SEL.Reports.IDs.Charts.General.ddlCombineOthers).val();
                        this.EnableHover = false;

                    },
                    ValidateCalculation: function (calculation) {
                        this.calculation = calculation.replace(/\xa0/g, ' ');
                    },
                    FormatCalculation: function () {
                        this.id = SEL.Reports.IDs.General.CurrentField;
                        this.type = SEL.Reports.IDs.General.CurrentFieldType;
                        this.columnName = $('.columnName').val();
                        if ($('#chkStaticRuntime').prop('checked') === true) {
                            this.formattedCalculation = SEL.Reports.EnterAtRunTime;
                        }else {
                            this.formattedCalculation = $('#txtFormula').text();
                        }
                    },
                    GetReportProgress: function () {
                        this.requestnum = SEL.Reports.IDs.RequestNumber;
                    }
                },
                Ellipsis: function (str, len)
                {
                    if (len === undefined || len === null) {
                        len = 32;
                    }
                    if (str.length > len) {
                        return str.substring(0, len - 4) + '...';
                    }

                    return str;
                }
            },
            FormatListItem: function (treeItem) {
                var fieldType = treeItem.attr.fieldtype, title;
                if (fieldType.substring(0, 1) === "F") {
                    fieldType = fieldType.substring(1);
                }
                if (treeItem.attr.comment === treeItem.data) {
                    treeItem.attr.comment = '';
                }
                $.data(document, treeItem.attr.id, treeItem);
                SEL.Reports.IDs.OddRow = !SEL.Reports.IDs.OddRow;
                var label = treeItem.data;
                if (treeItem.attr.crumbs > '') {
                    title = treeItem.attr.crumbs + ":" + label;
                } else {
                    title = label;
                }

                label = SEL.Reports.Misc.Ellipsis(label);

                if (treeItem.attr.rel === "node") {
                    return "<li class='field f1' id='" + treeItem.attr.id + "' tabindex='-1' role='menuitem' fieldid='" + treeItem.attr.fieldid + "' fieldtype='" + fieldType + "' joinviaid='" + treeItem.attr.joinviaid + "' crumbs='" + treeItem.attr.crumbs + "' columnid='" + treeItem.attr.columnid + "' criteriaid='CiD' title='" + title + "' >" + label + "</li>";
                } else {
                    var level = treeItem.attr.id.split("_").length - 1;
                    var expandable = 'expandable';
                    if (level >= 4) {
                        expandable = '';
                    }
                    return "<li class='field " + expandable + " isLazy' id='" + treeItem.attr.id + "' tabindex='-1' role='menuitem' fieldid='" + treeItem.attr.fieldid + "' fieldtype='" + fieldType + "' joinviaid='" + treeItem.attr.joinviaid + "' crumbs='" + treeItem.attr.crumbs + "' columnid='" + treeItem.attr.columnid + "' criteriaid='CiD' title='" + title + "' >" + label + "</li>";
                }
            },
            FormatSelectedColumn: function (fieldType, sort, hide, count, average, sum, min, max, groupby, rowClass) {
                var html = '';
                html = html + "<div class='" + rowClass + "'><img id='deleteicon0' class='icon deleteicon ' src='/static/icons/24/plain/delete.png' border='0' title='Remove column'></div>";
                switch (sort) {
                    case 0:
                    case 'None':
                    case '[None]':
                        html = html + "<div class='" + rowClass + "'><img id='sorticon0' class='icon sorticon NoSort' src='/static/icons/24/plain/sort_up_down_question.png' border='0' title='Sort'></div>";
                        break;
                    case 1:
                    case 'Ascending':
                        html = html + "<div class='" + rowClass + "'><img id='sorticon0' class='icon sorticon SortUp' src='/static/icons/24/plain/sort_ascending.png' border='0' title='Sort'></div>";
                        break;
                    case 2:
                    case 'Descending':
                        html = html + "<div class='" + rowClass + "'><img id='sorticon0' class='icon sorticon SortDown' src='/static/icons/24/plain/sort_descending.png' border='0' title='Sort'></div>";
                        break;
                    default:
                }

                html = html + "<div class='" + rowClass + "'><img id='hideicon0' class='icon ";
                if (hide === false) {
                    html = html + "iconDisabled";
                }

                html = html + "' src='/static/icons/24/plain/spy.png' border='0' title='Hide'></div>";
                html = html + "<div class='" + rowClass + "'><img id='counticon0' class='icon ";
                if (count === false) {
                    html = html + "iconDisabled";
                }

                html = html + "' src='/static/icons/24/plain/add.png' border='0' title='Count'></div>";
                if (fieldType === "N" || fieldType === "C" || fieldType === "M") {
                    html = html + "<div class='" + rowClass + "'><img id='averageicon0' class='icon ";
                    if (average === false) {
                        html = html + "iconDisabled";
                    }

                    html = html + "' src='/static/icons/24/plain/moon_half.png' border='0' title='Average'></div>";
                    html = html + "<div style='padding-left:26px;'>";
                    html = html + "<div class='" + rowClass + "'><img id='sumicon0' class='icon ";
                    if (sum === false) {
                        html = html + "iconDisabled";
                    }

                    html = html + "' src='/static/icons/24/plain/text_sum.png' border='0'  title='Sum'></div>";
                } else {
                    html = html + "<div style='padding-left:26px;'>";
                }


                html = html + "<div class='" + rowClass + "'><img id='maxicon0' class='icon ";
                if (max === false) {
                    html = html + "iconDisabled";
                }
                html = html + "' src='/static/icons/24/plain/arrow_up_red.png' border='0' title='Max'></div>";
                html = html + "<div class='" + rowClass + "'><img id='minicon0' class='icon ";
                if (min === false) {
                    html = html + "iconDisabled";
                }
                html = html + "' src='/static/icons/24/plain/arrow2_down_blue.png' border='0' title='Min'></div>";
                html = html + "<div class='" + rowClass + "'><img id='groupicon0' class='icon ";
                if (groupby === false) {
                    html = html + "iconDisabled";
                }
                html = html + "' src='/static/icons/24/plain/branch_add.png' border='0' title='Group By'></div>";
                html = html + '</div>';
                return html;
            },
            GetSelectedColumns: function () {
                var result = [];
                $.each(SEL.Reports.Columns.SelectedTreeNodes, function (index, node) {
                    result.push(SEL.Reports.GetColumnByTreeNode(node));
                });

                return result;
            },
            GetColumnByTreeNode: function (node) {
                if (node.max === undefined) {
                    node.max = false;
                }

                if (node.min === undefined) {
                    node.min = false;
                }

                if (node.count === undefined) {
                    node.count = false;
                }

                if (node.avg === undefined) {
                    node.avg = false;
                }

                if (node.sum === undefined) {
                    node.sum = false;
                }

                if (node.hide === undefined) {
                    node.hide = false;
                }

                if (node.groupby === undefined) {
                    node.groupby = false;
                }

                if (node.sortorder === '[None]' || node.sortorder === 'None') {
                    node.sortorder = 0;
                }
                else if(node.sortorder ==='[Ascending]' || node.sortorder === 'Ascending') {
                    node.sortorder = 1;
                 }
                 else if (node.sortorder === '[Descending]' || node.sortorder === 'Descending') {
                    node.sortorder = 2;
                 }  
                var column = {
                    id: node.internalId,
                    fieldid: node.fieldid,
                    crumbs: node.crumbs,
                    fieldType: node.fieldType,
                    joinviaid: node.joinviaid,
                    columnid: node.columnid,
                    attributename: node.text,
                    hide: node.hide,
                    average: node.avg,
                    sum: node.sum,
                    count: node.count,
                    min: node.min,
                    max: node.max,
                    sort: node.sortorder,
                    groupby: node.groupby
                };

                if (node.fieldtype === "W" || node.fieldtype === "Z") {
                    var treeItem = $.data(document, node.id);
                    column.literalName = treeItem.data;
                    column.literalValue = treeItem.metadata.formattedCalculation;
                }

                return column;
            },
            GetSelectedColumn: function (header, body) {
                this.id = header.attr('nodeid');
                this.fieldid = header.attr('fieldid');
                this.crumbs = header.attr('crumbs');
                this.joinviaid = header.attr('joinviaid') ? parseInt(header.attr('joinviaid')) : 0;
                this.columnid = header.attr('columnid');
                this.attributename = header.text();
                this.fieldType = header.attr('fieldType');
                var that = this;
                if (this.fieldType === "W" || this.fieldType === "Z") {
                    var data = $.data(document, this.id);
                    if (data !== undefined && data !== null) {
                        this.literalValue = data.metadata.formattedCalculation;
                        this.literalName = this.crumbs;
                    }
                }

                body.find('img').each(function () {
                    switch ($(this).attr('id')) {
                        case "sorticon0":
                            if ($(this).hasClass('NoSort')) {
                                that.sort = 0;
                            }
                            if ($(this).hasClass('SortUp')) {
                                that.sort = 1;
                            }

                            if ($(this).hasClass('SortDown')) {
                                that.sort = 2;
                            }
                            break;
                        case "hideicon0":
                            that.hide = !$(this).hasClass('iconDisabled');
                            break;
                        case "averageicon0":
                            that.average = !$(this).hasClass('iconDisabled');
                            break;
                        case "sumicon0":
                            that.sum = !$(this).hasClass('iconDisabled');
                            break;
                        case "counticon0":
                            that.count = !$(this).hasClass('iconDisabled');
                            break;
                        case "maxicon0":
                            that.max = !$(this).hasClass('iconDisabled');
                            break;
                        case "minicon0":
                            that.min = !$(this).hasClass('iconDisabled');
                            break;
                        case "groupicon0":
                            that.groupby = !$(this).hasClass('iconDisabled');
                            break;
                        default:
                    }
                });
            },
            InsertSelectedField: function (selector, treeItem) {
                // format "normal" item 
                var normalItem = SEL.Reports.FormatListItem(treeItem, '');

                // disable "normal" item
                $('#' + treeItem.attr.id).addClass('state-disabled iconDisabled');
                // add to #Dropper
                SEL.Reports.AddSelectedItemToList('#Dropper', $(normalItem).removeClass('field f1').addClass('ui-menu-item'), 0, treeItem);
            },
            AddSelectedItemToList: function (selector, that, active, treeItem, fromUserClick) {
                var fieldType = that.attr('fieldtype'), id;
                if (active === 0 && (fieldType === undefined || treeItem === undefined)) {
                    treeItem = $.data(document, that.attr('id'));
                    if (treeItem === undefined || treeItem === null) {
                        var newNode = SEL.Reports.IDs.General.EasyTree.getNode($(that).attr('id'));
                        var metaData = {SortOrder: 0, Hidden: false, Count: false, Average: false, Sum: false, Max: false, Min: false, GroupBy: false};
                        metaData.group = 0;
                        var attributes = { id: newNode.id, fieldid: newNode.fieldid, columnid: '', crumbs: newNode.crumbs, fieldtype: newNode.fieldType, joinviaid: newNode.joinviaid };
                        var javascriptTreeNode = { data: newNode.text, attr: attributes, metadata: metaData };
                        treeItem = javascriptTreeNode;
                        $.data(document, that.attr('id'), treeItem);
                    }
                    that = $(SEL.Reports.FormatListItem(treeItem));
                    fieldType = treeItem.attr.fieldtype;
                }

                var html = that[0].outerHTML;
                html = html.replace('<li', '<div');
                html = html.replace('<LI', '<div');
                html = html.replace('</li', '</div');
                html = html.replace('</LI', '</div');
                var newClass = 'selectedField';
                html = html.replace('\r\n', '');
                var element = $(html);
                html = $(element).html();

                if (active === 0) {
                    var node = SEL.Reports.IDs.General.EasyTree.getNode(treeItem.attr.id);
                    if (node !== null && node !== undefined) {
                        node.liClass = node.liClass + ' state-disabled iconDisabled';
                    }
                    // disable "normal" item
                    $('#' + treeItem.attr.id).addClass('state-disabled iconDisabled');
                    html = "<div class='" + newClass + "'>";
                    html += $(element).removeClass('ui-menu-item').addClass('selectedFieldName')[0].outerHTML;
                    selector = "#Dropper";
                    if (fieldType === "W" || fieldType === "Z") {
                        html = html + SEL.Reports.Calculated.FormatSelectedCalculation(treeItem, 'iconContainer');
                    } else {
                        if (treeItem !== undefined && treeItem !== null) {
                            html = html + SEL.Reports.FormatSelectedColumn(fieldType, treeItem.metadata.SortOrder, treeItem.metadata.Hidden, treeItem.metadata.Count, treeItem.metadata.Average, treeItem.metadata.Sum, treeItem.metadata.Min, treeItem.metadata.Max, treeItem.metadata.GroupBy, 'iconContainer') + "</div>";
                        } else {
                            html = html + SEL.Reports.FormatSelectedColumn(fieldType, 0, false, false, false, false, false, false, false, 'iconContainer') + "</div>";
                        }
                    }

                    html = html.replace('\r\n', '');
                    html = html.replace(/"/g, "'");
                    $(selector).append($(html));
                    $(selector + ' .f1').removeClass('f1 ui-state-focus field');
                } else {
                    $(element).attr('criteriaid', SEL.Reports.IDs.CriteriaId);
                    id = SEL.Reports.IDs.CriteriaId;
                    SEL.Reports.IDs.CriteriaId++;

                    if (fromUserClick) {
                        element[0].innerHTML = "<span class='criteriaName'>" + element[0].innerHTML + "</span> - <span class='filterInfo'></span>";
                    }

                    var joinType = 'And';
                    var metadata = $.data(document, "filter" + id.toString());
                    if (metadata.conditionJoiner === 2) {
                        joinType = 'Or';
                    }

                    var newDiv = $("<div class='selectedCriteria criteriaName'>").append($(element)).append("<div><img id='" + id + "delete' class='icon' src='/static/icons/24/plain/delete.png' border='0' title='Remove Filter' style='padding-top:5px;'>" + "<img id='" + id + "edit' class='icon' src='/static/icons/24/plain/table_row_edit.png' border='0' title='Edit Filter' style='padding-top:5px;'/><img id='" + id + "joiner' class='icon joinerIcon' src='/static/icons/24/plain/logic_" + joinType + ".png' border='0' title='" + joinType + "' style='padding-top:5px;'/></div>");
                    $(selector).append(newDiv);
                }

                $(selector).find(".ui-menu-item").removeClass('ui-state-focus field f1 ui-menu-item').unbind('click');
                if (active === 0) {
                    that.addClass('state-disabled iconDisabled');

                    $(selector + ">div").addClass('accordionheader');
                    if (fromUserClick) {
                        SEL.Reports.Chart.PopulateFields();
                    }
                } else {
                    $('#' + id + "delete").click(function () {
                        var parent = $(this).parent().parent();
                        $('#divFilter').append($("[id$='pnlFilter']"));
                        $('#divFilter').hide();
                        parent.remove();
                    });

                    if (fromUserClick) {
                        SEL.Reports.IDs.General.NewCriteria = true;
                        $('#' + id + "edit").trigger("click");
                    }
                }
                $('.iconContainer>.icon').unbind('click').click(function () {
                    if ($(this).hasClass('iconDisabled')) {
                        $(this).removeClass('iconDisabled');
                    } else {
                        $(this).addClass('iconDisabled');
                    }
                    SEL.Reports.Chart.PopulateFields();
                });
                $('.sorticon').unbind('click').click(function () {
                    if ($(this).hasClass('NoSort')) {
                        $(this).removeClass('NoSort').addClass('SortUp');
                        $(this).attr('src', '/static/icons/24/plain/sort_ascending.png');
                    } else {
                        if ($(this).hasClass('SortUp')) {
                            $(this).removeClass('SortUp').addClass('SortDown');
                            $(this).attr('src', '/static/icons/24/plain/sort_descending.png');
                        } else {
                            if ($(this).hasClass('SortDown')) {
                                $(this).removeClass('SortDown').addClass('NoSort');
                                $(this).attr('src', '/static/icons/24/plain/sort_up_down_question.png');
                            }
                        }
                    }
                    SEL.Reports.Chart.PopulateFields();
                });

                $('.joinerIcon').unbind().click(function () {
                    var currentTitle = $(this).attr('title');
                    var id = $(this).attr('id').replace('joiner', '');
                    var data = $.data(document, id);
                    if (currentTitle === "Or") {
                        $(this).attr('src', '/static/icons/24/plain/logic_and.png').attr('title', 'And');
                        data.conditionJoiner = 1;
                    } else {
                        $(this).attr('src', '/static/icons/24/plain/logic_or.png').attr('title', 'Or');
                        data.conditionJoiner = 2;
                    }
                    $.data(document, id, data);
                });

                $('.deleteicon').unbind('click').click(function () {
                    var deleteid = $(this).parent().parent().children().first().attr('id');
                    $(this).parent().parent().remove();
                    $('#' + deleteid).removeClass('state-disabled iconDisabled');
                    var width = 151 * $('#Dropper>.accordionheader:not("[id$=\'pnlFilter\']")').length;
                    if (width != 0) {
                        $('#Dropper').css('width', width);
                    }
                    else {
                        $('#Dropper').css('width', '98.6%');
                    }
                });
                $('.editicon').unbind('click').click(function () {
                    var editId = $(this).parent().parent().children().first().attr('id'), isStatic = false;
                    if (editId.substring(0, 1) === "Z") {
                        isStatic = true;
                    }
                    SEL.Reports.Calculated.AddEditCalculatedColumn(editId, isStatic);
                });
                $('.selectedField').mouseover(function () {
                    $(this).find('.iconContainer').show();
                }).mouseout(function () {
                    $(this).find('.iconContainer').each(function () {
                        if ($(this).find('.iconDisabled').length > 0) {
                            $(this).hide();
                        }
                    });
                });

                $(".selectedField").trigger("mouseout");

                var width = 151 * $('#Dropper>.accordionheader:not("[id$=\'pnlFilter\']")').length;
                if (width != 0) {
                    $('#Dropper').css('width', width);
                }
                else {
                    $('#Dropper').css('width', '98.6%');
                }
            },
            isScrolledIntoView: function (elem) {
                var $elem = $(elem);
                var $window = $(window);

                var docViewTop = $window.scrollTop();
                var docViewBottom = docViewTop + $window.height();

                var elemTop = $elem.offset().top;
                var elemBottom = elemTop + $elem.height();

                return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
            },
            Calculated:
            {
                AddEditCalculatedColumn: function (calculatedColumnId, isStatic)
                {
                    if ($('#divScheduleCriteriaComment').css('display') !== 'none') {
                        return;
                    }

                    SEL.Reports.IDs.General.CursorPosition = 0;
                    $('#divCalculation [id*="reqColumnName"]').css('visibility', 'hidden');
                    if (isStatic) {
                        $('#menuHolder').hide();
                        $('#divMenuHolder').hide();
                        $('#divFormula').css('float', '');
                        $('#divFormulaButtons').hide();
                        $('#divFormulaHelp').hide();
                        $('#staticRuntime').show();
                    } else {
                        $('#menuHolder').show();
                        $('#divMenuHolder').show();
                        $('#divFormula').css('float', 'right');
                        $('#divFormulaButtons').show();
                        $('#divFormulaHelp').show();
                        $('#staticRuntime').hide();
                    }

                    var title, type, height, width;
                    $('#formulaOk').hide();
                    $('#formulaError').hide();
                    $('#' + SEL.Reports.IDs.General.CalculatedSaveButton).removeAttr('disabled');
                    $('#' + SEL.Reports.IDs.General.CalculatedSaveButton).parent().css('opacity', '');
                    if (isStatic === true) {
                        type = "Static";
                        height = 400;
                        width = 340;
                        $('#txtFormula').css('width', '310px');
                        $('#divFormula').css('width', '315px');
                        SEL.Reports.IDs.General.CurrentFieldType = "Z";
                    } else {
                        type = "Calculated";
                        height = 400;
                        width = 800;

                        $('#divFormula').css('width', '500px');
                        $('#txtFormula').css('width', '495px');
                        SEL.Reports.IDs.General.CurrentFieldType = "W";
                    }

                    if (calculatedColumnId === null || calculatedColumnId === undefined) {

                        title = "New " + type + " Column";
                        $('.columnName').removeAttr("disabled");
                        $('.columnName').val('');
                        $('#txtFormula').text('');
                        $('#txtFormula').html('');
                        $('#divFormula').show();
                        $("[id$='btnClearCalc']").parent().show()
                        $('#chkStaticRuntime').prop("checked", false);
                        SEL.Reports.IDs.General.CurrentField = null;
                    } else {
                        title = "Edit " + type + " Column";
                        var treeData = $.data(document, calculatedColumnId);
                        $('.columnName').val(treeData.attr.crumbs);
                        if (treeData.metadata.formattedCalculation === SEL.Reports.EnterAtRunTime) {
                            $('#chkStaticRuntime').prop("checked", true);
                            $('#txtFormula').html("");
                            $('#divFormula').hide();
                            $("[id$='btnClearCalc']").parent().hide()
                        } else {
                            $('#chkStaticRuntime').prop("checked", false);
                            $('#divFormula').show();
                            $("[id$='btnClearCalc']").parent().show()
                            $('#txtFormula').html(treeData.metadata.formattedCalculation);
                        }

                        SEL.Reports.IDs.General.CurrentField = calculatedColumnId;

                    }
                    var dialog = $('#divCalculation').dialog({
                        resizable: false,
                        modal: true,
                        width: width,
                        title: title,
                        open: function () {
                            $('#txtFormula').on('paste', function () {
                                SEL.Reports.Calculated.StartTimeout();
                            });
                            if (!isStatic) {
                                $('menuHolder').hide();
                                SEL.Reports.Calculated.Functions.Get();
                                $('[contentEditable]').unbind()
                                    .keyup(function () {
                                        SEL.Reports.Calculated.StartTimeout();

                                    }).click(function () {
                                        SEL.Reports.Calculated.StartTimeout();
                                    }).on('paste', function () {
                                        SEL.Reports.Calculated.StartTimeout();
                                    }).blur(function () { SEL.Reports.Calculated.StartTimeout(); });;

                            } else {
                                $('[contentEditable]').unbind();
                            }
                            $('.ui-dialog-titlebar-close').remove();
                        },

                        close: function () {
                            if(SEL.Reports.IDs.General.CalcTimeout !== null) {
                                clearTimeout(SEL.Reports.IDs.General.CalcTimeout);
                            }
                            SEL.Tooltip.hide();
                            $('#divCalculation').dialog('destroy');
                        }


                    });
                    if (!isStatic) {
                        if ($(dialog).parent().find('#imgViewCalcHelp').length === 0) {
                            $(dialog).parent().find('.calculatedButtons').append("<img id='imgViewCalcHelp' src='/static/icons/24/new-icons/information.png' alt='Show Calculated Column Information' style='border-width: 0px; display: inline-block; position: relative; left: 629px; top: 3px;'>");
                            $('#imgViewCalcHelp').mouseover(function () {
                                SEL.Tooltip.Show('140D15EB-35D5-4626-B5F4-B22FB4A9DF1A', 'sm', $(this)[0], 5);
                            });
                        }

                        SEL.Reports.Calculated.StartTimeout();
                    } else {
                        $(dialog).parent().find('#imgViewCalcHelp').remove();
                    }
                },
                Functions:
                {
                    Get: function () {
                        var columns = SEL.Reports.GetSelectedColumns();
                        $('#menuHolder ui-easytree').remove();
                        $('#menuHolder').html('<ul id="functionMenu" style="display: none;"><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Available Items<ul id="availableItemsMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Math<ul id="MathMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Date & Time<ul id="DateAndTimeMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Logical<ul id="LogicalMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Statistical<ul id="StatisticalMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Text & Data<ul id="TextAndDataMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Financial<ul id="FinancialMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Information<ul id="InformationMenu"></ul></li><li class="ui-menu-item"><img src="/static/icons/16/plain/table_view.png">Lookup & reference<ul id="LookupAndReferenceMenu"></ul></li></ul>');

                        for (var j = 0; j < columns.length; j++) {
                            var item = columns[j];
                            var fieldType = item.id.substring(0, 1);
                            if (fieldType !== "W") {
                                $('#availableItemsMenu').append('<li class="ui-menu-item availableColumn" fieldtype="' + item.fieldType + '"><img src="/static/icons/16/plain/table_column.png">' + item.attributename + '</li>');
                            }
                        }
                        if ($('#DateAndTimeMenu').children().length === 0) {
                            $.ajax({
                                url: appPath + '/shared/webServices/svcReports.asmx/GetAvailableFunctions',
                                type: 'POST',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: SEL.Reports.Calculated.Functions.GetComplete,
                                error: SEL.Reports.Misc.ErrorHandler
                            });
                        } else {
                            SEL.Reports.Calculated.CreateMenuEvents();
                        }
                    },
                    GetComplete: function (data) {
                        if (data.d !== null) {
                            var functions = data.d;
                            for (var i = 0; i < functions.length; i++) {
                                var func = functions[i];
                                var parent = func.Parent;
                                $('#' + parent + "Menu").append('<li class="ui-menu-item"><img src="/static/icons/16/plain/calculator.png">' + func.FunctionName + '</li>');
                                $.data(document, func.FunctionName, func);
                            }

                        }

                        $('#menuHolder').easytree({ ordering: 'ordered', disableIcons: true });
                        $('menuHolder').show();

                        SEL.Reports.Calculated.CreateMenuEvents();

                        $('#functionMenu .ui-menu-item').css('width', '');

                    },
                    Validate: function (calculation) {
                        var params = "calculation: '" + calculation + "'";
                        SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'ValidateCalcuation', params, SEL.Reports.Calculated.Functions.ValidateComplete, SEL.Reports.Misc.ErrorHandler);
                    },
                    ValidateComplete: function (data) {
                        alert(data.d);
                    },
                    AppendValue: function (text) {
                        $('#txtFormula').append(text);
                        SEL.Reports.Calculated.StartTimeout();
                    },
                    AppendOperator: function (opObject) {
                        var operator = opObject.textContent;
                        SEL.Reports.Calculated.Functions.AppendValue(operator);
                    },
                    ClearText: function () {
                        $('#txtFormula').val('');
                        $('#txtFormula').html('');
                    }
                },
                Save: function () {
                    var params = new SEL.Reports.Misc.WebserviceParameters.FormatCalculation();
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'FormatCalculatedField', params, SEL.Reports.Calculated.FormatComplete, SEL.Reports.Misc.ErrorHandler);
                },
                Cancel: function () { $('#divCalculation').dialog('close'); },
                Validate: function (calculation) {
                    var params = new SEL.Reports.Misc.WebserviceParameters.ValidateCalculation(calculation);
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'ValidateCalcuation', params, SEL.Reports.Calculated.ValidateComplete, SEL.Reports.Misc.ErrorHandler);
                },
                ValidateComplete: function (data) {
                    if (data.d === "") {
                        $('#formulaOk').show();
                        $('#formulaError').hide();
                        $('#' + SEL.Reports.IDs.General.CalculatedSaveButton).removeAttr('disabled');
                        $('#' + SEL.Reports.IDs.General.CalculatedSaveButton).parent().css('opacity', '');
                    } else {
                        $('#formulaOk').hide();
                        $('#formulaError').show();
                        $('#' + SEL.Reports.IDs.General.CalculatedSaveButton).prop('disabled', 'disabled');
                        $('#' + SEL.Reports.IDs.General.CalculatedSaveButton).parent().css('opacity', '.3');
                    }
                },
                Changed: function () {
                    SEL.Reports.Calculated.StartTimeout();
                },
                FormatComplete: function (data) {

                    data.d.attr.text = data.d.attr.crumbs;
                    if (SEL.Reports.IDs.General.CurrentField != null) {
                        data.d.attr.columnid = SEL.Reports.IDs.General.CurrentField.substring(1);
                    }
                    $.data(document, data.d.attr.id, data.d);

                    SEL.Reports.Columns.AddOrUpdate(data.d);
                    SEL.Reports.Columns.SelectedTreeNodes = SEL.Reports.Columns.MakeUniqueNodeNames(SEL.Reports.Columns.SelectedTreeNodes);
                    SEL.Reports.Preview.Refresh();

                    $('#divCalculation').dialog('close');
                },
                CreateMenuEvents: function () {
                    var menuTimer, menuItem;
                    $('#menuHolder .ui-menu-item').unbind().mouseover(function () {
                        $(this).addClass("ui-state-focus");
                        var showTooltip = function (that) {
                            var funcName = $(that).text();
                            var func = $.data(document, funcName);
                            var isOpen = $('#divCalculation').parent().hasClass('ui-dialog');
                            if (func !== undefined && func !== null && func.Description !== undefined && isOpen) {
                                var popup = '<div><div><h3><span >' + funcName + '</span></h3></div><div >&nbsp;</div></div><div><div><span><span>' + func.Description + '</span></span></div><div >' + func.Remarks + '</div></div><div><div><h4><span>' + func.Syntax + '</span></h4></div><div>&nbsp;</div></div><div><div><span><span>' + func.Example + '</span></span></div><div>&nbsp;</div></div>';
                                SEL.Tooltip.Show(popup, 'sm', $(that)[0]);
                                var menuPositionLeft = $(that).offset().left;
                                $('.tooltipcontainer').css('left', menuPositionLeft + 200 + 'px');
                                var top = $('.tooltipcontainer').position().top - 15;
                                $('.tooltipcontainer').css('top', top + 'px');
                            }
                        };

                        if (menuTimer !== undefined && menuTimer !== null) {
                            clearTimeout(menuTimer);
                        }

                        menuItem = $(this);

                        menuTimer = setTimeout(function () { showTooltip(menuItem); }, 1000);

                    }).mouseleave(function () {
                        $(this).removeClass("ui-state-focus");
                    }).click(function () {
                        var funcName = $(this).text();
                        if ($(this).hasClass("availableColumn")) {
                            SEL.Reports.Calculated.Functions.AppendValue("<font style='color: rgb(0,0,100); background-color: white;'>[" + $(this).text() + "]</font>");
                        } else {
                            var func = $.data(document, funcName);
                            if (func !== undefined && func !== null) {
                                var text = func.Syntax;
                                SEL.Reports.Calculated.Functions.AppendValue(text);
                            }
                        }

                    });

                    $('#menuHolder').mouseout(function () {
                        if (menuTimer !== undefined && menuTimer !== null) {
                            clearTimeout(menuTimer);
                        }
                    });

                    $('#menuHolder .easytree-container').css('overflow-x', 'hidden');
                },
                FormatSelectedCalculation: function (treeItem, rowClass) {
                    var html = '';
                    html = html + "<div class='" + rowClass + "'><img id='deleteicon0' class='icon deleteicon ' src='/static/icons/24/plain/delete.png' border='0' title='Remove column'></div>";
                    html = html + "<div class='" + rowClass + "'><img id='" + treeItem.attr.id + "edit' class='editicon' src='/static/icons/24/plain/table_row_edit.png' border='0' title='Edit column' style='padding-top:5px;'></div>";

                    var fieldType = treeItem.attr.fieldtype;
                    if (fieldType === "Z") {
                        html = html + "<div class='" + rowClass + "'><img id='counticon0' class='icon ";
                        if (treeItem.metadata.Count === false) {
                            html = html + "iconDisabled";
                        }

                        html = html + "' src='/static/icons/24/plain/element_run.png' border='0' title='I'll decide when I run the report'></div>";
                    }

                    html = html + '</div>';
                    return html;
                },
                StartTimeout: function () {
                    if (SEL.Reports.IDs.General.CalcTimeout !== null) {
                        clearTimeout(SEL.Reports.IDs.General.CalcTimeout);
                    }

                    SEL.Reports.IDs.General.CalcTimeout = setTimeout(function () { SEL.Reports.Calculated.Validate($('#txtFormula').text()); }, 500);
                }
            },
            Preview: {
                Menu: {},
                SetupMenu: function () {

                    var headerIndex = SEL.Reports.Preview.Menu.data("column-header");
                    if ($("#preview").find("div.e-headercelldiv.e-emptyCell").length > 0 && $(".e-groupdroparea")[0].childElementCount >= 2) {
                        headerIndex = headerIndex - ($(".e-groupdroparea")[0].childElementCount - 1);
                    }
                    var treeNode = SEL.Reports.Columns.SelectedTreeNodes[headerIndex];

                    $("#previewColumnHeaderMenu li a").show();
                    $("#previewColumnHeaderMenu .selectable:not('.ui-icon-delete')").removeClass('selectedMenuItem');

                    var fieldType = treeNode.fieldType || treeNode.fieldtype;
                    switch (fieldType) {
                        case "S":
                        case "FS":
                        case "FD":
                        case "D":
                        case "DT":
                        case "LT":
                        case "CO":
                            $('#previewColumnHeaderMenu .preview-menu-item-avg a').hide();
                            $('#previewColumnHeaderMenu .preview-menu-item-sum a').hide();
                            break;
                        case undefined:
                        case "X":
                        case "FX":
                        case "Y":
                            $('#previewColumnHeaderMenu .preview-menu-item-avg a').hide();
                            $('#previewColumnHeaderMenu .preview-menu-item-sum a').hide();
                            $('#previewColumnHeaderMenu .preview-menu-item-max a').hide();
                            $('#previewColumnHeaderMenu .preview-menu-item-min a').hide();
                            break;
                        case "W":
                        case "Z":
                        case "AT":
                        case "O":
                        case "FU":
                        case "U":
                            $("#previewColumnHeaderMenu :not('.preview-menu-item-hide, .preview-menu-item-delete') a").hide();
                            break;

                        default:
                    }

                    // Depending on the type of item and current settings reset iconDisabled and / or visibility.
                    $('#previewColumnHeaderMenu .preview-menu-item-hide .selectable').toggleClass('selectedMenuItem', treeNode.hide);
                    $('#previewColumnHeaderMenu .preview-menu-item-max .selectable').toggleClass('selectedMenuItem', treeNode.max);
                    $('#previewColumnHeaderMenu .preview-menu-item-min .selectable').toggleClass('selectedMenuItem', treeNode.min);
                    $('#previewColumnHeaderMenu .preview-menu-item-count .selectable').toggleClass('selectedMenuItem', treeNode.count);
                    $('#previewColumnHeaderMenu .preview-menu-item-avg .selectable').toggleClass('selectedMenuItem', treeNode.avg);
                    $('#previewColumnHeaderMenu .preview-menu-item-sum .selectable').toggleClass('selectedMenuItem', treeNode.sum);

                },
                Grid: undefined,
                GridLoadingMask: undefined,
                CurrencySymbol: "£",
                DeleteColumn: function (headerIndex) {
                    var treeNode = SEL.Reports.Columns.SelectedTreeNodes[headerIndex];
                    if (treeNode.groupby === true) {
                        SEL.Reports.Preview.UngroupColumn(headerIndex);
                    }
                    SEL.Reports.Columns.Remove(treeNode);
                    SEL.Reports.Preview.RemoveColumn(headerIndex);
                    SEL.Reports.Columns.TargetColumnIndex = null;

                },
                Initialise: function () {

                    if (SEL.Reports.Preview.Initialised) {
                        return;
                    }

                    // build grid header menu
                    SEL.Reports.Preview.Menu = $("ul#previewColumnHeaderMenu").menu({
                        create: function (event, ui) {
                            // clicks outside of the menu should close it
                            $('body:not(ul#previewColumnHeaderMenu)').click(function () {
                                $("ul#previewColumnHeaderMenu").hide();
                            });

                            // handle clicks on each menu item
                            $(this).children().each(function (index, menuItem) {
                                $(menuItem).on("click", function (event) {
                                    event.preventDefault();
                                    var headerIndex = SEL.Reports.Preview.Menu.data("column-header");
                                    if ($("#preview").find("div.e-headercelldiv.e-emptyCell").length > 0 && $(".e-groupdroparea")[0].childElementCount >= 2) {
                                        headerIndex = headerIndex - ($(".e-groupdroparea")[0].childElementCount - 1);
                                    }
                                    var treeNode = SEL.Reports.Columns.SelectedTreeNodes[headerIndex];
                                    var menuItem = $(event.currentTarget);

                                    if (menuItem.hasClass("preview-menu-item-delete")) {
                                        SEL.Reports.Columns.TargetColumnIndex = headerIndex;
                                        if (treeNode.text === $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis + " :selected").text() || treeNode.text === $('#' + SEL.Reports.IDs.Charts.General.ddlYAxis + " :selected").text()) {
                                            $('#dialog-confirm-column-delete').dialog('open');
                                        } else {
                                            SEL.Reports.Preview.DeleteColumn(headerIndex);
                                        }

                                    } else if (menuItem.hasClass("preview-menu-item-hide")) {
                                        treeNode.hide = !menuItem.find(".selectable").hasClass('selectedMenuItem');

                                        SEL.Reports.Columns.SelectedTreeNodes[headerIndex] = treeNode;
                                        SEL.Reports.Preview.AddContextMenu();

                                    } else {
                                        var setColumnOption = function (columnOption, treeNode, menuItem) {
                                            treeNode[columnOption] = false;
                                            if (menuItem.hasClass("preview-menu-item-" + columnOption)) {
                                                treeNode[columnOption] = !menuItem.find(".selectable").hasClass('selectedMenuItem');
                                            }
                                        };

                                        setColumnOption("count", treeNode, menuItem);
                                        setColumnOption("sum", treeNode, menuItem);
                                        setColumnOption("avg", treeNode, menuItem);
                                        setColumnOption("max", treeNode, menuItem);
                                        setColumnOption("min", treeNode, menuItem);

                                        SEL.Reports.Columns.SelectedTreeNodes[headerIndex] = treeNode;
                                        SEL.Reports.Preview.Refresh();
                                    }
                                });
                            });
                        }
                    }).hide();

                    SEL.Reports.Preview.GridLoadingMask = $("#previewLoadingText");
                    SEL.Reports.Preview.CurrencySymbol = $("#" + SEL.Reports.IDs.General.CurrencySymbol).val();
                    SEL.Reports.Preview.Initialised = true;

                },
                Initialised: false,
                Timer: null,
                AddContextMenu: function () {
                    setTimeout(function () { $("#preview tbody td.e-rowcell, #preview thead th.e-headercell").addClass("easytree-droppable easytree-accept"); }, 100);

                    $("#preview").find("div.e-headercelldiv:not(.e-emptyCell)").each(function (index, element) {
                        var column = SEL.Reports.Columns.SelectedTreeNodes[index];
                        if (column) {
                            var addIconToColumnHeader = function (property) {
                                if (column[property] === true) {
                                    var hide = false;
                                    if (property !== "hide" && !$(element).context.innerHTML.contains(property.toUpperCase() + " of ")) {
                                        if ($(element).context.innerHTML.contains("ejsel-hide-icon"))
                                        {
                                            $(element).find(".ejsel-hide-icon").remove();
                                            hide = true;
                                        }
                                        $(element).prepend(" " + property.toUpperCase() + " of ");

                                    }
                                    if (!$(element).context.innerHTML.contains("ejsel-" + property + "-icon")) {
                                        $(element).prepend("<div class='ejsel-" + property + "-icon ejsel-grid-header-icon'></div>");
                                    }
                                    if (hide) {
                                        $(element).prepend("<div class='ejsel-hide-icon ejsel-grid-header-icon'></div>");
                                    }

                                }
                                else {
                                    $(element).find(".ejsel-" + property + "-icon").remove();
                                    $(element).html($(element).html().replace(property.toUpperCase() + " of ", ""));
                                }
                            };
                            // add the hide/aggregate icons                            
                            addIconToColumnHeader("hide");
                            addIconToColumnHeader("avg");
                            addIconToColumnHeader("min");
                            addIconToColumnHeader("max");
                            addIconToColumnHeader("sum");
                            addIconToColumnHeader("count");


                            // add the edit button for static and calculated columns
                            if ((column.fieldtype === "Z" || column.fieldtype === "W") && $(element).find(".e-edit").length === 0) {
                                $(element).prepend("<span class='e-icon e-edit ejsel-grid-header-edit-button'></span>");
                            }

                            if ($(element).context.innerHTML.contains("e-settings ejsel-grid-header-menu-button")) {
                                $(element).find(".e-settings.ejsel-grid-header-menu-button").hide();
                            }
                            $(element).prepend("<span class='e-icon e-settings ejsel-grid-header-menu-button'></span>");

                            if ($(element).context.innerText === "") {
                                $(element).css("padding-top", "10px");
                            } else {
                                $(element).css("padding-top", "0");
                            }
                        }

                    });
                },
                Reorder: function(args) {
                    var columnDictionary = {};
                    // Make a dictionary of the backing collection.
                    for (var k = 0; k < SEL.Reports.Columns.SelectedTreeNodes.length; k++) {
                        var field = SEL.Reports.Columns.SelectedTreeNodes[k].FieldName === undefined ? SEL.Reports.Columns.SelectedTreeNodes[k].text : SEL.Reports.Columns.SelectedTreeNodes[k].FieldName;
                        columnDictionary[field] = SEL.Reports.Columns.SelectedTreeNodes[k];
                    }

                    SEL.Reports.Columns.SelectedTreeNodes = [];
                    //Clear the backing collection then re-populate it in the required order as taken from the grid.
                    for (var l = 0; l < args.model.columns.length; l++) {
                        SEL.Reports.Columns.SelectedTreeNodes.push(columnDictionary[args.model.columns[l].field]);
                    }
                    SEL.Reports.Preview.AddContextMenu();
                },
                BuildGrid: function (data, ejColumns, ejGroupSettings, ejSortSettings) {
                    // build a new grid
                    $("#preview").html("").ejGrid({
                        dataSource: data,
                        enableHeaderHover: true,
                        allowResizing: true,
                        allowReordering: true,
                        allowSorting: true,
                        allowMultiSorting: true,
                        columns: ejColumns,
                        commonWidth: 180,
                        allowScrolling: true,
                        sortSettings: ejSortSettings,
                        allowGrouping: true,
                        groupSettings: ejGroupSettings,
                        enableRowHover: false,
                        columnDrop: function (args) {
                            if (args.draggableType === "headercell" && args.type === "columnDrop") {
                                if (args.target.parents('#menuOwner').length === 1) {
                                    // dragging a column header into the tree will delete the column
                                    var gridObj = $("#preview").data("ejGrid");
                                    var headerIndex = gridObj.getColumnIndexByField(args.column.field);
                                    var treeNode = SEL.Reports.Columns.SelectedTreeNodes[headerIndex];
                                    SEL.Reports.Columns.TargetColumnIndex = headerIndex;
                                    if (treeNode.text === $('#' + SEL.Reports.IDs.Charts.General.ddlXAxis + " :selected").text() || treeNode.text === $('#' + SEL.Reports.IDs.Charts.General.ddlYAxis + " :selected").text()) {
                                        $('#dialog-confirm-column-delete').dialog('open');

                                    } else {
                                        SEL.Reports.Preview.DeleteColumn(headerIndex);
                                    }
                                } else {
                                    SEL.Reports.Preview.Reorder(args);
                                }
                            }
                            $("#removeColumnIcon").hide();
                        },
                        actionComplete: function (argument) {
                            // inject the menu button and icons into each grid column heading, this appears to be the best place to do it with a syncfusion grid
                            if (argument.requestType === "refresh" || argument.requestType === "grouping" || argument.requestType === "ungrouping") {
                                SEL.Reports.Preview.AddContextMenu();
                            }

                            $('.e-number').hide();

                        },
                        actionBegin: function (argument) {
                            if (argument.requestType === "sorting") {
                                var groupColumnCount = argument.model.groupSettings.groupedColumns.length;

                                // if the column is already sorted and in the "descending" state, then clear the sort from the column
                                var sortedColumnIndex;
                                var sortedColumn = $.grep(argument.model.sortSettings.sortedColumns, function (item, index) {
                                    sortedColumnIndex = index;
                                    return argument.columnName === item.field;
                                });

                                if (sortedColumn.length) {
                                    var columnIndex = SEL.Reports.Preview.Grid.getColumnIndexByField(sortedColumn[0].field);
                                    var header = SEL.Reports.Preview.Grid.getHeaderContent().find("th.e-headercell")[columnIndex];

                                    if (($(header).find(".e-descending").length || groupColumnCount > 0 ) && SEL.Reports.Columns.SelectedTreeNodes[columnIndex].groupby === false) {
                                        argument.model.sortSettings.sortedColumns.splice(sortedColumnIndex, 1);
                                    }

                                }

                                // Re-order the sorted columns so they match the generated SQL. They will always be ordered left to right.
                                var newList = [];

                                $(argument.model.columns).each(function (index, item) {
                                    SEL.Reports.Columns.SelectedTreeNodes[index].sortorder = 0;
                                });

                                $(argument.model.sortSettings.sortedColumns).each(function (index, item) {
                                    var sortColumn = {
                                        field: item.field,
                                        direction: item.direction,
                                        index: SEL.Reports.Preview.Grid.getColumnIndexByField(item.field)
                                    };

                                    SEL.Reports.Columns.SelectedTreeNodes[sortColumn.index].sortorder = sortColumn.direction === "ascending" ? 1 : 2;
                                    newList.push(sortColumn);
                                });

                                newList.sort(function (a, b) { return a.index - b.index });
                                argument.model.sortSettings.sortedColumns = newList;
                                $("#preview").data("ejGridSortedColumns", newList);
                            }

                            if (argument.requestType === "grouping") {
                                var groupColumnIndex = SEL.Reports.Preview.Grid.getColumnIndexByField(argument.columnName);
                                SEL.Reports.Columns.SelectedTreeNodes[groupColumnIndex].groupby = true;
                                var newSorted = [];
                                $(argument.model.sortSettings.sortedColumns).each(function (index, item)
                                {
                                    var grouped = false;
                                    for (var i = 0; i < argument.model.groupSettings.groupedColumns.length; i++)
                                    {
                                        if (item.field === argument.model.groupSettings.groupedColumns[i])
                                        {
                                            newSorted.push(item);
                                            grouped = true;
                                        }

                                    }
                                    if (!grouped) {
                                        var index = SEL.Reports.Preview.Grid.getColumnIndexByField(item.field);
                                        SEL.Reports.Columns.SelectedTreeNodes[index].sortorder = 0
                                    }
                                });

                                argument.model.sortSettings.sortedColumns = newSorted;
                                $("#preview").data("ejGridSortedColumns", newSorted);
                            }

                            if (argument.requestType === "ungrouping") {
                                var ungroupColumnIndex = SEL.Reports.Preview.Grid.getColumnIndexByField(argument.columnName);
                                SEL.Reports.Columns.SelectedTreeNodes[ungroupColumnIndex].groupby = false;
                            }
                        },
                        load: function (argument) {

                            SEL.Reports.Preview.Grid = $("#preview").data("ejGrid");
                            // don't sort when the user clicks on the cog
                            $("#preview").on("mouseover", ".ejsel-grid-header-menu-button, .ejsel-grid-header-edit-button", function () {
                                argument.model.allowSorting = false;
                            });
                            $("#preview").on("mouseout", ".ejsel-grid-header-menu-button, .ejsel-grid-header-edit-button", function () {
                                argument.model.allowSorting = true;
                            });
                            // set up events for the injected menu buttons
                            $("#preview").on("click", ".ejsel-grid-header-menu-button", function (event) {
                                if ($("#previewColumnHeaderMenu").is(":visible")) {
                                    SEL.Reports.Preview.Menu.hide();
                                    return false;
                                }
                                var columnIndex = $(this).parents("th").index();

                                if ($("#preview").find("div.e-headercelldiv.e-emptyCell").length > 0) {
                                    columnIndex = $(this).parents("th")[0].cellIndex - 1;
                                }
                                // relate the menu to the column header it's about to be shown at, so that we know which column to work with when the menu is used
                                SEL.Reports.Preview.Menu.data("column-header", columnIndex);
                                var cell = $(this).parents(".e-grid").find(".e-gridcontent tr:first-child td:nth-child(" + (columnIndex + 1) + ")");

                                SEL.Reports.Preview.SetupMenu();
                                SEL.Reports.Preview.Menu
                                    .css("width", cell.outerWidth() - 5)
                                    .show()
                                    .position({
                                        my: "left-3 top+15",
                                        at: "left-3 top+15",
                                        of: $(this)
                                    });

                                return false;
                            });

                            $("#preview").on("click", ".ejsel-grid-header-edit-button", function (event)
                            {
                                var columnIndex = $(this).parents("th").index()-$('.e-grouptopleftcell').length
                                var column = SEL.Reports.Columns.SelectedTreeNodes[columnIndex];
                                SEL.Reports.Calculated.AddEditCalculatedColumn(column.id, column.fieldtype === "Z");

                                return false;
                            });


                        },
                        destroy: function (argument) {
                            // remove any attached event handlers before removing the grid
                            $("#preview").off();
                        },
                        create: function () {
                            $(".e-gridcontent").css("width", $("#preview>.e-gridheader").width() + 'px');
                        }
                    });
                    if ($('.e-grid .e-groupdroparea').html().contains("Drag a column header here to group")) {
                        $('.e-grid .e-groupdroparea').html("Drag a column header here to group by its column");
                    }
                },
                AllowGridReload: true,
                Refresh: function (reloadGrid) {
                    SEL.Reports.Preview.AllowGridReload = reloadGrid === false ? false : true;
                    clearTimeout(SEL.Reports.Preview.Timer);
                    if (SEL.Reports.Preview.AllowGridReload) {
                        var ejGrid = $("#preview").data("ejGrid");
                        if (ejGrid) {
                            var columns1 = SEL.Reports.Columns.SelectedTreeNodes;
                            if (ejGrid._$headerCols !== null) {
                                if (ejGrid._$headerCols.length < columns1.length) {
                                    var column1 = columns1[columns1.length - 1];
                                    setTimeout(function () {
                                        ejGrid.columns(column1.text, "add");
                                        if (SEL.Reports.Columns.TargetColumnIndex !== null) {
                                            ejGrid.reorderColumns(column1.text, ejGrid.getColumnByIndex(SEL.Reports.Columns.TargetColumnIndex).field);
                                            SEL.Reports.Columns.TargetColumnIndex = null;
                                            SEL.Reports.Preview.Reorder(ejGrid);
                                        }
                                    }, 100);
                                }
                            }
                            SEL.Reports.Preview.AddContextMenu();
                        } else {
                            $("#pgPreview").css('pointer-events', 'none');
                            // Add 10 empty rows
                            var data = [
                                { A: 0 }, { A: 0 }, { A: 0 }, { A: 0 }, { A: 0 }, { A: 0 }, { A: 0 }, { A: 0 }, { A: 0 },
                                { A: 0 }
                            ];
                            var columns = SEL.Reports.Columns.SelectedTreeNodes;
                            var ejColumns = [];
                            var ejSortSettings = { sortedColumns: [] };
                            var ejGroupSettings = { enableDropAreaAutoSizing: false, groupedColumns: [] };
                            for (var i = 0; i <= columns.length - 1; i++) {
                                var column = columns[i];
                                ejColumns.push({
                                    headerText: column.text,
                                    field: column.text
                                });

                                if (column.Order > 0) {
                                    var sortColumn = { field: column.Text, direction: column.Order === 1 ? "ascending" : "descending" };
                                    ejSortSettings.sortedColumns.push(sortColumn);
                                }

                                if (column.groupby === true) {
                                    ejGroupSettings.groupedColumns.push(column.Text);
                                }
                            }

                            SEL.Reports.Preview.BuildGrid(data, ejColumns, ejGroupSettings, ejSortSettings);
                            $("#pgPreview").css('pointer-events', 'auto');
                            $(".e-groupdroparea").addClass("easytree-reject");
                            SEL.Reports.Preview.AddContextMenu();
                        }

                        SEL.Reports.Preview.Initialise();
                        $("#previewInstructions").hide();

                        if (!SEL.Reports.Columns.SelectedTreeNodes.length) {
                            var grid = $("#preview").data("ejGrid");
                            if (grid) {
                                grid.destroy();
                            }

                            $("#previewInstructions").show().addClass("easytree-droppable");
                            return;
                        }

                        SEL.Reports.Preview.GridLoadingMask.show();
                    }
                    
                    $('#chartPreview').css('display', 'none');
                    var reportCriteria = SEL.Reports.Criteria.GetSelectedCriteria();
                    $('#divRuntimeCriteriaComment').css('display', 'none');
                    for (var i = 0; i < reportCriteria.length; i++) {
                        if (reportCriteria[i].RunTime) {
                            $('#divRuntimeCriteriaComment').css('display', '');
                            continue;
                        }
                    }
                    SEL.Reports.Chart.PopulateFields();

                    SEL.Reports.IDs.RequestNumber = 0;
                    if (SEL.Reports.Columns.SelectedTreeNodes.length) {
                        SEL.Reports.Preview.Timer = setTimeout(function() {
                                $(".easytree-droppable").removeClass("easytree-droppable");
                                $("#pgPreview").css('pointer-events', 'none'); // unbind mouse events
                                var params = new SEL.Reports.Misc.WebserviceParameters.PreviewReport();
                                SEL.Ajax.Service('/shared/webServices/svcReports.asmx/',
                                    'CreateReportPreview',
                                    params,
                                    function(data) {
                                        SEL.Reports.IDs.RequestNumber = data.d;
                                        SEL.Reports.Preview.GetReportStatus();
                                    },
                                    SEL.Reports.Misc.ErrorHandler);
                            },
                            3000);
                    }
                },
                RemoveColumn: function (index) {
                    var column = SEL.Reports.Preview.Grid.getColumnByIndex(index);

                    if (SEL.Reports.Columns.SelectedTreeNodes.length && !$("html.ie8").length) {
                        SEL.Reports.Preview.Grid.columns(column.field, "remove");
                        setTimeout(function () {
                            //refresh grid only if one or more columns are aggregated
                            if ($(".e-headercell .ejsel-grid-header-icon").length > 0) {
                                SEL.Reports.Preview.Refresh();
                            }
                        }, 100);
                    } else {
                        SEL.Reports.Preview.Refresh();
                    }
                },
                UngroupColumn: function (index) {
                    var column = SEL.Reports.Preview.Grid.getColumnByIndex(index);
                    $("#preview").ejGrid("ungroupColumn", column.field);
                },
                GetReportStatus: function () {
                    var params = new SEL.Reports.Misc.WebserviceParameters.GetReportProgress();
                    SEL.Ajax.Service('/shared/webServices/svcReports.asmx/', 'GetReportData', params, SEL.Reports.Preview.ReportProgressChanged, SEL.Reports.Misc.ErrorHandler);
                },
                ReportProgressChanged: function (data) {
                    var response = data.d;
                    switch (response.Progress) {
                        case 0:
                        case 3:
                            var timeoutId = setTimeout(function () { SEL.Reports.Preview.GetReportStatus(); }, 500);
                            break;
                        case 1:

                            // build a column list compatible with the grid, a sortedColumnList and a groupedColumn
                            var ejColumns = [];
                            var ejSortSettings = { sortedColumns: [] };
                            var ejGroupSettings = {enableDropAreaAutoSizing: false, groupedColumns: [] };
                            for (var i = 0; i <= response.Columns.length - 1; i++) {
                                var column = response.Columns[i];

                                var format = undefined;
                                var textAlign = ej.TextAlign.Left;
                                switch (column.FieldType) {
                                    case "A":
                                    case "C":
                                    case "FC":
                                    case "M": {
                                        format = SEL.Reports.Preview.CurrencySymbol + "{0:n2}";
                                        textAlign = ej.TextAlign.Right;
                                        break;
                                    }
                                    case "D": {
                                        format = "{0:dd/MM/yyyy}";
                                        break;
                                    }
                                    case "DT": {
                                        format = "{0:dd/MM/yyyy HH:mm}";
                                        break;
                                    }
                                    case "T": {
                                        format = "{0:hh:mm}";
                                        break;
                                    }
                                    case "F":
                                    case "FD": {
                                        format = "{0:n2}";
                                        textAlign = ej.TextAlign.Right;
                                        break;
                                    }
                                    case "FI":
                                    case "I":
                                    case "N":
                                    case "B": {
                                        textAlign = ej.TextAlign.Right;
                                        break;
                                    }
                                }

                                ejColumns.push({
                                    headerText: column.HeaderText,
                                    field: column.FieldName,
                                    isPrimaryKey: column.IsPrimaryKey,
                                    format: format,
                                    textAlign: textAlign
                                });

                                SEL.Reports.Columns.SelectedTreeNodes[i].FieldName = column.FieldName;

                                if (column.SortDirection > 0) {
                                    var sortColumn = { field: column.FieldName, direction: column.SortDirection === 1 ? "ascending" : "descending" };
                                    ejSortSettings.sortedColumns.push(sortColumn);
                                }

                                if (column.GroupBy === true) {
                                    ejGroupSettings.groupedColumns.push(column.FieldName);
                                }
                            }

                            SEL.Reports.Preview.GridLoadingMask.hide();

                            if (SEL.Reports.Preview.AllowGridReload) {
                                // remove the current preview grid if it exists
                                var grid = $("#preview").data("ejGrid");
                                if (grid) {
                                    grid.destroy();
                                }
                                SEL.Reports.Preview.BuildGrid(ej.parseJSON(response.Data), ejColumns, ejGroupSettings, ejSortSettings);
                            }
                            $('.e-number').hide();
                            // bind mouse events
                            $("#pgPreview").css('pointer-events', 'auto');
                            $("#Dropper").addClass("easytree-droppable");
                            $(".e-groupdroparea").addClass("easytree-reject");
                            $("#easytree-reject,#removeColumnIcon").hide();
                            if (response.ChartPath) {
                                $('#chartPreview').css('display', '');
                                $('#imgChartPreview').attr('src', response.ChartPath);
                                $('#imgChart').attr('src', response.ChartPath);

                            } else {
                                $('#chartPreview').css('display', 'none');
                                $('#imgChart').css('display', 'none');
                            }
                            break;
                        case 2:
                            SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.', 'Message from ' + moduleNameHTML);
                            break;
                        default:
                    }
                }
            }
        };
    }
    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    } else {
        execute();
    }
}(SEL));
