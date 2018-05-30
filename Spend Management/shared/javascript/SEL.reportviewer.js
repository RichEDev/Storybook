/* <summary>Reports view Methods</summary> */
(function (SEL) {
    var scriptName = "reportViewer";
    function execute() {
        SEL.registerNamespace("SEL.ReportViewer");
        SEL.ReportViewer = {
            CurrencySymbol: "£",
            IDs: {
                RequestNumber : null,
                Grid: null,
                CriteriaId : null,
                ReportId : null,
                HiddenColumn : null,
                CriteriaSelector:
                {
                    Tab: null,
                    TreeContainer: null,
                    Tree: null,
                    Drop: null,
                    Node:null
                }
            },
            GetReportStatus: function() {
                var params = new SEL.ReportViewer.GetReportProgress();
                SEL.Ajax.Service('/shared/webServices/svcReports.asmx/',
                    'GetReportData',
                    params,
                    SEL.ReportViewer.ReportProgressChanged,
                    SEL.ReportViewer.ErrorHandler);
            },
            GetReportProgress: function () {
                this.requestnum = SEL.ReportViewer.IDs.RequestNumber;
            },
            ReportProgressChanged: function(data) {
                var response = data.d;
                switch (response.Progress) {
                case 0:
                case 3:

                    var timeoutId = setTimeout(function() { SEL.ReportViewer.GetReportStatus(); }, 500);
                    break;
                case 1:
                    if (data.d.ChartPath !== null) {
                        $('#imgChart').attr('src', data.d.ChartPath);
                    } else {
                        $('#chartHeader').hide();
                    }

                    // build a column list compatible with the grid, a sortedColumnList and a groupedColumn
                    var ejColumns = [];
                    var ejSortSettings = { sortedColumns: [] };
                    var ejGroupSettings = { enableDropAreaAutoSizing: false, groupedColumns: [] };
                    for (var i = 0; i <= response.Columns.length - 1; i++) {
                        var column = response.Columns[i];

                        var format = undefined;
                        var textAlign = ej.TextAlign.Left;
                        switch (column.FieldType) {
                        case "A":
                        case "C":
                        case "FC":
                        case "M":
                        {
                            format = SEL.ReportViewer.CurrencySymbol + "{0:n2}";
                            textAlign = ej.TextAlign.Right;
                            break;
                        }
                        case "D":
                        {
                            format = "{0:dd/MM/yyyy}";
                            break;
                        }
                        case "DT":
                        {
                            format = "{0:dd/MM/yyyy HH:mm}";
                            break;
                        }
                        case "T":
                        {
                            format = "{0:hh:mm}";
                            break;
                        }
                        case "F":
                        case "FD":
                        {
                            format = "{0:n2}";
                            textAlign = ej.TextAlign.Right;
                            break;
                        }
                        case "FI":
                        case "I":
                        case "N":
                        case "B":
                        {
                            textAlign = ej.TextAlign.Right;
                            break;
                        }
                        }


                        switch (column.HeaderText) {
                        case "Claim ID":
                            ejColumns.push(
                                {headerText: "View Claim",
                                    field: column.FieldName,
                                    isPrimaryKey: column.IsPrimaryKey,
                                    format: format,
                                    commands: [
                                        {
                                            type: "details",
                                            buttonOptions: {
                                                text: "View Claim",
                                                width: "100"
                                            }
                                        }
                                    ],
                                    isUnbound: true,
                                    textAlign: ej.TextAlign.Left,
                                    width: 150}
                            );
                            break;
                        case "Contract ID":
                            ejColumns.push(
                                {headerText: "View Contract",
                                    field: column.FieldName,
                                    isPrimaryKey: column.IsPrimaryKey,
                                    format: format,
                                    commands: [
                                        {
                                            type: "details",
                                            buttonOptions: {
                                                text: "View Contract",
                                                width: "100"
                                            }
                                        }
                                    ],
                                    isUnbound: true,
                                    textAlign: ej.TextAlign.Left,
                                    width: 150}
                            );
                            break;
                        case "Supplier ID":
                            ejColumns.push(
                                {headerText: "View Supplier",
                                    field: column.FieldName,
                                    isPrimaryKey: column.IsPrimaryKey,
                                    format: format,
                                    commands: [
                                        {
                                            type: "details",
                                            buttonOptions: {
                                                text: "View Supplier",
                                                width: "100"
                                            }
                                        }
                                    ],
                                    isUnbound: true,
                                    textAlign: ej.TextAlign.Left,
                                    width: 150}
                            );
                            break;
                        case "Task Id":
                            ejColumns.push(
                                {headerText: "View Task",
                                    field: column.FieldName,
                                    isPrimaryKey: column.IsPrimaryKey,
                                    format: format,
                                    commands: [
                                        {
                                            type: "details",
                                            buttonOptions: {
                                                text: "View Task",
                                                width: "100"
                                            }
                                        }
                                    ],
                                    isUnbound: true,
                                    textAlign: ej.TextAlign.Left,
                                    width: 150}
                            );
                            break;
                        default:
                            ejColumns.push({
                                headerText: column.HeaderText,
                                field: column.FieldName,
                                isPrimaryKey: column.IsPrimaryKey,
                                format: format,
                                textAlign: textAlign
                            }); 
                        }

                        if (column.SortDirection > 0) {
                            var sortColumn = {
                                field: column.FieldName,
                                direction: column.SortDirection === 1 ? "ascending" : "descending"
                            };
                            ejSortSettings.sortedColumns.push(sortColumn);
                        }

                        if (column.GroupBy === true) {
                            ejGroupSettings.groupedColumns.push(column.FieldName);
                        }
                    }

                    // remove the current preview grid if it exists
                    var grid = $("#preview").data("ejGrid");
                    if (grid) {
                        grid.destroy();
                    }
                    SEL.ReportViewer.BuildGrid(ej.parseJSON(response.Data),
                        ejColumns,
                        ejGroupSettings,
                        ejSortSettings);
                    // bind mouse events
                    break;
                case 2:
                    SEL.MasterPopup.ShowMasterPopup('An error has occurred processing your request.',
                        'Message from ' + moduleNameHTML);
                    break;
                default:
                }
            }, 
            BuildGrid: function (data, ejColumns, ejGroupSettings, ejSortSettings) {
                function ClickCellButtonHandler(columnId, value) {
	
                    var drilldown = document.getElementById('drilldownreportid').value;
                    var column = columnId;
	
                    var reportArea = 2;
                    var url = "view.aspx?callback=1&item=1&reportid=" + reportid + "&requestnum=" + requestNum + "&drilldownreportid=" + drilldown + "&reportarea=2";
                    var details;
                    var xmlRequest;
	
                    try
                    {
                        xmlRequest = new XMLHttpRequest();
                    }
                    catch (e)
                    {	
                        try
                        {
                            xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
                        }
                        catch (f)
                        {
                            xmlRequest = null;
                        }
                    }
	
                    var data;
			
                    data = "column=" + column + "&value=" + value;
			
	
                    xmlRequest.onreadystatechange = function()
                    {
	
                        if (xmlRequest.readyState == 4)
                        {
                            details = xmlRequest.responseText;
                            window.open('view.aspx?reportid=' + drilldown + "&requestnum=" + details, 'reportviewer' + details, 'locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');
                        }
                    }
                    xmlRequest.open("POST",url,true);
                    xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                    xmlRequest.send(data);
                }
                    // build a new grid
                $("#preview").html("").ejGrid({
                    dataSource: data,
                    enableHeaderHover: true,
                    allowResizing: true,
                    allowResizeToFit: false,
                    allowReordering: true,
                    allowSorting: true,
                    allowSelection: true,
                    selectionType: ej.Grid.SelectionType.Multiple,
                    selectionSettings: { selectionMode: ["cell"], cellSelectionMode: 1 },
                    allowMultiSorting: true,
                    columns: ejColumns,
                    commonWidth: 180,
                    allowScrolling: true,
                    sortSettings: ejSortSettings,
                    allowGrouping: true,
                    groupSettings: ejGroupSettings,
                    enableRowHover: false,
                    allowPaging: true,
                    pageSettings: { pageSize: 15 },
                    allowFiltering: true,
                    filterSettings: { filterType: "excel" },
                    toolbarSettings: {
                        showToolbar: true,
                        toolbarItems:
                            [ej.Grid.ToolBarItems.PrintGrid],
                        customToolbarItems: [
                            { templateID: "#Save", tooltip: "Save as" }
                            , { templateID: "#Change", tooltip:"Change columns or filters" }
                            , { templateID: "#Excel", tooltip:"Export as excel" }
                            , { templateID: "#CSV", tooltip:"Export as CSV " }
                            , { templateID: "#Flat", tooltip:"Export as a flat file " }
                            , { templateID: "#Pivot", tooltip:"Export as a pivot report " }
                            , { templateID: "#ExportOptions", tooltip:"Export options" }
                            , { templateID: "#DrilldownReport", tooltip:"Drilldown report" }
                            , { templateID: "#ClearFilter", tooltip:"Clear all filters" }
                        ]
                    }
                            ,
                        load: function (argument) {

                            SEL.ReportViewer.IDs.Grid = $("#preview").data("ejGrid");
                            // don't sort when the user clicks on the cog
                        },
                        create: function () {
                            $('#divReportWindow').show("slow");
                            $(".e-gridcontent").css("width", $("#preview>.e-gridheader").width() + 'px');
                            $('#imgSpin').hide("slow");
                        },
                        cellSelected: function(args, item) {
                            SEL.ReportViewer.IDs.CriteriaId++;
                            var gridObj = $('#preview').data("ejGrid");
                            var column = gridObj.getColumnByIndex(args.cellIndex[0]);
                            var drilldown = $('#drilldownreportid').val();
                            if (reportid === drilldown && column.type !== "date") {
                                var cellValue;
                                if (column.commands && column.commands.length > 0 ) {
                                    cellValue = args.data[args.cellIndex[0] + 1];
                                    switch (column.headerText) {
                                    case 'View Claim':
                                        SEL.ReportViewer.ViewClaim(cellValue);
                                        cellValue = args.data[args.cellIndex[0] + 1];        
                                        gridObj.filterColumn(column.field,"equal",cellValue,"and", false);
                                        $('#preview_ClearFilter').removeClass('e-hide');
                                        var treeItem = {"data":column.headerText,"attr":{"text":column.headerText,"id":column.field,"internalId":"","crumbs":"","rel":"node","fieldid":column.fieldID,"joinviaid":0,"fieldtype":"X","columnid":"","comment":""},"state":"","metadata":{"conditionType":1,"criterionOne":cellValue.toString(),"criterionTwo":"","conditionTypeText":"Equals","fieldType":"S","isListItem":false,"firstListItemText":"","runtime":false,"conditionJoiner":1,"group":0}}
                                        SEL.ReportViewer.InsertCriteria(treeItem);
                                        gridObj.hideColumns(column.field);
                                        SEL.ReportViewer.IDs.HiddenColumn = column;
                                        $('#criteriaContainer').animate({backgroundColor: '#FFFFF'}, 'slow').animate({backgroundColor: '#ececec'}, 'slow');

                                        break;
                                    case 'View Contract':
                                        window.open('/ContractSummary.aspx?tab=0&id=' +
                                            cellValue);
                                        break;
                                    case 'View Supplier':
                                        window.open('/shared/Supplier_Details.aspx?sid=' +
                                            cellValue);
                                        break;
                                    case 'View Task':
                                        window.open('/shared/tasks/ViewTask.aspx?tid='  +
                                            cellValue);
                                        break;
                                    default:
                                    }
                                    
                                } else
                                {
                                    cellValue = args.data[args.cellIndex[0] + 1];        
                                    gridObj.filterColumn(column.field,"equal",cellValue,"and", false);
                                    var treeItem = {"data":column.headerText,"attr":{"text":column.headerText,"id":column.field,"internalId":"","crumbs":"","rel":"node","fieldid":column.fieldID,"joinviaid":0,"fieldtype":"X","columnid":"","comment":""},"state":"","metadata":{"conditionType":1,"criterionOne":cellValue.toString(),"criterionTwo":"","conditionTypeText":"Equals","fieldType":"S","isListItem":false,"firstListItemText":"","runtime":false,"conditionJoiner":1,"group":0}}
                                    SEL.ReportViewer.InsertCriteria(treeItem);
                                    $('#preview_ClearFilter').removeClass('e-hide');
                                    $('#criteriaContainer').animate({backgroundColor: '#FFFFF'}, 'slow').animate({backgroundColor: '#ececec'}, 'slow');
                                }

                                SEL.ReportViewer.SetFilterCount();

                            } else {
                                ClickCellButtonHandler(args.cellIndex[0], args.currentCell.text());    
                            }
                        },
                    actionComplete: function(args) {
                        if (args.requestType === "filtering" ) {
                            if (args.currentFilterObject) {
                                $('#preview_ClearFilter').addClass('e-hide');
                            } else {
                                $('#preview_ClearFilter').removeClass('e-hide');
                            }

                            $('.e-rowcell').mouseover(function() { $(this).addClass('e-hover'); }).mouseout(function() { $(this).removeClass('e-hover') });
                        }
                    }

                    });

                
                    if ($('.e-grid .e-groupdroparea').html().contains("Drag a column header here to group")) {
                        $('.e-grid .e-groupdroparea').html("Drag a column header here to group by its column");
                    }


                    $('.e-rowcell').mouseover(function() { $(this).addClass('e-hover'); }).mouseout(function() { $(this).removeClass('e-hover') });

                    $('#preview_Save').attr('data-content', 'Save a copy').click(function() {
                        $('#pnlSaveOption').dialog(
                            {
                                modal: true,
                                resizable: false,
                                title: 'Save a copy of this report'
                            });
                    });

                    $('#preview_Change').attr('data-content', 'Change report columns or filters').click(function() {
                        changeColumns(); 
                    });

                    $('#preview_Excel').attr('data-content', 'Export as an excel file').click(function() {
                        exportReport(2, SEL.ReportViewer.IDs.ReportId);
                    });

                    $('#preview_Pivot').attr('data-content', 'Export as an Excel Pivot report').click(function() {
                        exportReport(5, SEL.ReportViewer.IDs.ReportId);
                    });

                    $('#preview_CSV').attr('data-content', 'Export as a comma seperate file').click(function() {
                        exportReport(3, SEL.ReportViewer.IDs.ReportId);
                    });

                    $('#preview_Flat').attr('data-content', 'Export as a flat file').click(function() {
                        exportReport(4, SEL.ReportViewer.IDs.ReportId);
                    });

                    $('#preview_DrilldownReport').attr('data-content', 'Drilldown Report').click(function() {
                        showDrillDownReports();
                    });

                    $('#preview_ExportOptions').attr('data-content', 'Export Options').click(function() {
                        exportOptions();
                    });


                $('#preview_ClearFilter').attr('data-content', 'Clear all filters').addClass('e-hide').click(
                    function() {
                        var gridObj = $('#preview').data("ejGrid");
                        gridObj.clearFiltering();
                        $(this).addClass('e-hide');
                        $('#divViewClaim').hide();
                        $('.filterInfo[nodeid=""]').remove();
                    if (SEL.ReportViewer.IDs.HiddenColumn !== null) {
                        gridObj.showColumns(SEL.ReportViewer.IDs.HiddenColumn.headerText);
                        SEL.ReportViewer.IDs.HiddenColumn = null;
                    }
                    $('#criteriaContainer').animate({backgroundColor: '#FFFFF'}, 'slow').animate({backgroundColor: '#ececec'}, 'slow');
                    SEL.ReportViewer.SetFilterCount();
                });

                },
            ErrorHandler: function (data) {
                SEL.Common.WebService.ErrorHandler(data);
            },
            ViewClaim: function(claimId) {
                var viewClaimComplete = function (viewClaim) {
                    if (viewClaim) {
                        $('#lblemployee').text(viewClaim.d.EmployeeName);
                        $('#lblclaimno').text(viewClaim.d.ClaimNumber);
                        $('#lbldatepaid').text(viewClaim.d.DatePaid);
                        $('#lbldescription').text(viewClaim.d.Description);
                        $('#divViewClaim').show();
                    }
                }

                var params = { claimId: claimId };
                SEL.Ajax.Service('/shared/webServices/svcReports.asmx/',
                    'ViewClaim',
                    params,
                    viewClaimComplete,
                    SEL.ReportViewer.ErrorHandler);
            },
            CriteriaNodesRefreshComplete: function (data) {
                SEL.ReportViewer.IDs.CriteriaId = 1;
                var groupCount = 0;
                if (data !== null && data !== undefined && data.data !== undefined) {
                    var treeData = data.data;
                    for (var i = 0; i < treeData.length; i++) {
                        var treeItem = treeData[i];
                        treeItem["text"] = treeItem.data;
                        treeItem.attr.id = treeItem.attr.id.replace('copy_', '');
                        treeItem.attr.internalId = treeItem.attr.id;
                        SEL.ReportViewer.InsertCriteria(treeItem);
                        SEL.ReportViewer.IDs.CriteriaId++;
                        SEL.Reports.Criteria.GroupCount = treeItem.metadata.group + 1 > groupCount ? treeItem.metadata.group + 1 : groupCount;
                    }

                    SEL.ReportViewer.InsertGroup(groupCount);
                }

                SEL.ReportViewer.SetFilterCount();
                
                SEL.ReportViewer.IDs.CriteriaId = 9999;
            },
            InsertCriteria: function (treeItem) {
                //show elements
                $('.filterHeader').hide();
                $('#criteriaList').show();
                // add the data
                $.data(document, "filter" + SEL.ReportViewer.IDs.CriteriaId.toString(), treeItem.metadata);
                // insert into div
                SEL.ReportViewer.IDs.CriteriaSelector.Node = treeItem.attr;
                SEL.ReportViewer.IDs.CriteriaSelector.Node["text"] = treeItem["data"];
                // insert criteria and groups
                $('#criteriaList').append(SEL.ReportViewer.DisplaySummaryFilterInformation("filter" + SEL.ReportViewer.IDs.CriteriaId.toString()));                 
                   
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
              DisplaySummaryFilterInformation: function (criteriaId) {                        
                        // Get information from metadata
                  var nodeObj = $.data(document, criteriaId),
                      criteria1 = nodeObj.criterionOne === null ? '' : nodeObj.criterionOne,
                      criteria2 = nodeObj.criterionTwo === null ? '' : nodeObj.criterionTwo,
                      conditionType = typeof nodeObj.conditionType === typeof undefined ? '' : nodeObj.conditionType,
                      conditionTypeText = typeof nodeObj.conditionTypeText === typeof undefined
                          ? ' '
                          : nodeObj.conditionTypeText,
                      isListItem = nodeObj.isListItem,
                      filterType = nodeObj.fieldType === null ? '' : nodeObj.fieldType,
                      filterCellInfo,
                      criteriaCellInfo,
                      fullCriteriaValue = '',
                      group = nodeObj.group === (null || undefined) ? 0 : nodeObj.group;
                        
                        var itemCountEven = $('.filterInfo').not('.filterHeader').length % 2 === 0;
                        var rowClass = itemCountEven ?'e-row' : 'e-alt_row';
                        var andOrOnClick = '';
                        var groupIcon = "";
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

                        var andOr = "<td class='e-rowcell' style=''><div class='' ";
                        if (criteriaId === 'filter1') andOr = andOr + "style='display:none' ";
                        andOr = andOr + "onclick='" + andOrOnClick + "' value=" + andOrValue + " >" + andOrText + "</div></td>";

                        // Set the condition text to display
                        filterCellInfo = "<tr class='filterInfo e-grid " + rowClass + "' id='" + criteriaId + "' nodeid='" + SEL.ReportViewer.IDs.CriteriaSelector.Node.internalId + "' fieldid='" + $.filterModal.Filters.Ids.FilterFieldID + "' fieldtype='" + SEL.ReportViewer.IDs.CriteriaSelector.Node.fieldType + "' joinviaid='" + SEL.ReportViewer.IDs.CriteriaSelector.Node.joinviaid + "' crumbs='' group = '" + group + "'>" + groupIcon + "&nbsp;" + andOr + '<td class="e-rowcell">' + SEL.ReportViewer.IDs.CriteriaSelector.Node.text + '</td><td class="e-rowcell">' + conditionTypeText + '</td>';


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
                  // If the filter has a condition of "Between", display both values
                    if (conditionType.toString() === '8') {
                        var andString = ' and ';

                        if (criteria2 === '') {
                            andString = '';
                        }

                        criteriaCellInfo = criteriaCellInfo + criteria1 + andString + criteria2;
                    }
                    else {
                        criteriaCellInfo += criteria1;
                    }


                    return filterCellInfo + "<td class='e-rowcell'>" + criteriaCellInfo + "</td></tr>";                   
                },
            SetFilterCount: function() { var filterCount = $('.filterInfo').length;
                if (filterCount === 0) {
                    $('#filterCount').html(' ');
                } else {
                    $('#filterCount').html(' (' + filterCount + ')');    
                }
            }
        }
    }
    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL))