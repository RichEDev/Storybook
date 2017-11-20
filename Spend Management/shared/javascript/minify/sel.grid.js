(function () {
    var scriptName = "Grid";
    function execute() {
        SEL.registerNamespace("SEL.Grid");
        SEL.Grid = {
            /* IDs and Variables */
            arrGrids: [],
            currentRowID: null,
            applicationName: null,
            currentPageNum: {},
            GridSelectType: {
                "CheckBox": 0,
                "RadioButton": 1
            },
            SortDirection: {
                "None": 0,
                "Ascending": 1,
                "Descending": 2
            },
            GridColumnType: {
                "Field": 1,
                "Event": 2,
                "Static": 3,
                "TwoState" : 4,
                "ValueIcon" : 5
            },

            /* Messages */
            GridGenerateError: "Sorry, an error occurred while generating the grid",
            GridSortError: "Sorry, an error occurred while attempting to sort the grid",
            GridPagingError: "Sorry, an error occurred while attempting to change the active page on the grid",
            GridFilterError: "Sorry, an error occurred while attempting to filter the grid",

            showGridWait: function (GridID) {
                if ($e(GridID + '_wait') === true) {
                    $g(GridID + '_wait').style.display = '';
                }
            },
            hideGridWait: function (GridID) {
                if ($e(GridID + '_wait') === true) {
                    $g(GridID + '_wait').style.display = 'none';
                }
            },
            changeGridPage: function (GridID, pageNum) {
                SEL.Grid.showGridWait(GridID);
                var filter = '';
                if ($e(GridID + "_Filter") === true) {
                    filter = $g(GridID + "_Filter").value;
                }
                var grid = SEL.Grid.getGridById(GridID);
                SEL.Grid.currentPageNum[GridID] = pageNum == undefined ? 1 : pageNum;
                Spend_Management.shared.webServices.svcGrid.changePage(CurrentUserInfo.AccountID, GridID, SEL.Grid.currentPageNum[GridID], filter, grid, grid.ServiceClassForInitialiseRowEvent, grid.ServiceClassMethodForInitialiseRowEvent, SEL.Grid.changeGridPageComplete, SEL.Grid.changePageFail);
            },
            getCurrentPageNum: function (GridID) {
                if (SEL.Grid.currentPageNum[GridID] == undefined) {
                    SEL.Grid.currentPageNum[GridID] = 1;
                }
                return SEL.Grid.currentPageNum[GridID];
            },
            changeGridPageComplete: function (data) {
                if ($e(data[0]) === true) {
                    $g(data[0]).innerHTML = data[1];
                    SEL.Grid.updateGrid(data[2]);
                    SEL.Grid.hideGridWait(data[0]);
                }
            },
            changePageFail: function (error) {
                SEL.MasterPopup.ShowMasterPopup(SEL.Grid.GridPagingError, 'Error in ' + SEL.Grid.applicationName);
            },
            refreshGrid: function (g, pn) {
                SEL.Grid.changeGridPage(g, pn);
            },
            sortGrid: function (GridID, column, is_static) {
                SEL.Grid.showGridWait(GridID);
                var filter = '';
                if ($e(GridID + "_Filter") === true) {
                    filter = $g(GridID + "_Filter").value;
                }
                var grid = SEL.Grid.getGridById(GridID);

                if (GridID !== undefined && GridID !== null && GridID == 'gridFunds' && column == 'TransactionType')
                    //// translate text into enum if valuelist is present
                    for (var i = 0; i < grid.columns.length; i += 1) {
                        // the column we're filtering/sorting on
                        var column1 = grid.columns[i];
                        // the column is linked to an enum via the valuelist property
                        for (var enumProperty in column1.valuelist) {
                            if (column1.valuelist[enumProperty].toLowerCase() == filter.toLowerCase()) {
                                filter = enumProperty;
                            } else if (enumProperty == filter) {
                                filter = column1.valuelist[enumProperty].toLowerCase();
                                $g(GridID + "_Filter").value = enumProperty;
                            }
                        }
                    }

                SEL.Grid.currentPageNum[GridID] = 1;
                var scfire = grid == null ? '' : grid.ServiceClassForInitialiseRowEvent;
                var scmfire = grid == null ? '' : grid.ServiceClassMethodForInitialiseRowEvent;
                Spend_Management.shared.webServices.svcGrid.sortGrid(CurrentUserInfo.AccountID, GridID, column, filter, grid, is_static, scfire, scmfire, SEL.Grid.sortGridComplete, SEL.Grid.sortGridError);
            },
            sortGridComplete: function (data) {
                if ($e(data[0]) === true) {
                    $g(data[0]).innerHTML = data[1];
                    SEL.Grid.updateGrid(data[2]);
                    SEL.Grid.hideGridWait(data[0]);
                }
            },
            sortGridError: function (error) {
                SEL.MasterPopup.ShowMasterPopup(SEL.Grid.GridSortError, 'Grid Error');
            },
            getGridById: function (GridID) {
                for (var i = 0; i < SEL.Grid.arrGrids.length; i++) {
                    if (SEL.Grid.arrGrids[i].GridID == GridID) {
                        return SEL.Grid.arrGrids[i];
                    }
                }
                return null;
            },
            updateGrid: function (gridData) {
                var gridFound = false;
                var gridObj = SEL.jsonParse.parse(gridData);

                if (gridObj !== undefined && gridObj !== null) {
                    for (var i = 0; i < SEL.Grid.arrGrids.length; i++) {
                        if (SEL.Grid.arrGrids[i].GridID === gridObj.GridID) {
                            SEL.Grid.arrGrids[i] = gridObj;
                            gridFound = true;
                        }
                    }

                    if (!gridFound) {
                        SEL.Grid.arrGrids.push(gridObj);
                    }

                    $("#" + gridObj.GridID).trigger("gridChanged");
                }
            },
            getTableById: function (GridID) {
                return $g('tbl_' + GridID);
            },
            getGridRowByID: function (GridID, rowID) {
                if ($e('tbl_' + GridID) === true) {
                    var tbl = $g('tbl_' + GridID);
                    rowID = 'tbl_' + GridID + '_' + rowID;
                    for (var i = 0; i < tbl.rows.length; i++) {

                        if (tbl.rows[i].id === rowID) {
                            return tbl.rows[i];
                        }
                    }
                }
                return null;
            },
            deleteGridRow: function (GridID, RowID) {
                var row = SEL.Grid.getGridRowByID(GridID, RowID);
                if (row !== null) {
                    if ($e(GridID) === true) {
                        var tbl = SEL.Grid.getTableById(GridID);
                        tbl.deleteRow(row.rowIndex);

                        var gridDetail = SEL.Grid.getGridById(GridID);
                        gridDetail.rowCount--;
                        SEL.Grid.refreshGrid(GridID, SEL.Grid.currentPageNum[GridID]);
                    }
                }
            },
            getCellById: function (GridID, rowID, cellID) {
                return $g('tbl_' + GridID + '_' + rowID + '_' + cellID);
            },
            filterGrid: function (GridID) {
                if ($e(GridID + "_Filter") === true) {
                    var filter = $g(GridID + "_Filter").value;
                    SEL.Grid.showGridWait(GridID);
                    var grid = SEL.Grid.getGridById(GridID);

                    // translate text into enum if valuelist is present
                    for (var i = 0; i < grid.columns.length; i += 1) {

                        // the column we're filtering/sorting on
                        var column = grid.columns[i];
                        if (column.fieldID === grid.sortedColumnFieldID && column.valuelist) {

                            // the column is linked to an enum via the valuelist property
                            for (var enumProperty in column.valuelist) {
                                // match and subsitute the text value to the enum value
                                if ((GridID !== undefined && GridID !== null&& GridID== 'gridFunds')) {
                                    if (column.valuelist[enumProperty].toLowerCase() == filter.toLowerCase()) {
                                        filter = enumProperty;
                                    } else if (enumProperty == filter) {
                                        filter = column.valuelist[enumProperty].toLowerCase();
                                        $g(GridID + "_Filter").value = enumProperty;
                                    }
                                    }
                                // match and subsitute the text value to the enum value
                               else if (column.valuelist[enumProperty].toLowerCase() == filter.toLowerCase()) {
                                    filter = enumProperty;
                                } 
                            }
                        }
                    }

                    SEL.Grid.currentPageNum[GridID] = 1;
                    var scfire = grid == null ? '' : grid.ServiceClassForInitialiseRowEvent;
                    var scmfire = grid == null ? '' : grid.ServiceClassMethodForInitialiseRowEvent;
                    Spend_Management.shared.webServices.svcGrid.filterGrid(CurrentUserInfo.AccountID, GridID, filter, grid, scfire, scmfire, SEL.Grid.filterGridComplete, SEL.Grid.filterGridError);
                }
                
            },
            filterGridCombo: function(GridID, columnName, useValue) {
                var comboBox = $(".cmbfilter option:selected").first();
                if (comboBox !== undefined && comboBox !== null) {
                    var filter = useValue ? comboBox.val() : comboBox.text();

                    SEL.Grid.showGridWait(GridID);
                    var grid = SEL.Grid.getGridById(GridID);
                    SEL.Grid.currentPageNum[GridID] = 1;
                    var scfire = grid == null ? '' : grid.ServiceClassForInitialiseRowEvent;
                    var scmfire = grid == null ? '' : grid.ServiceClassMethodForInitialiseRowEvent;
                    Spend_Management.shared.webServices.svcGrid.filterGridByCombo(CurrentUserInfo.AccountID, GridID, filter, grid, columnName, scfire, scmfire, SEL.Grid.filterGridComplete, SEL.Grid.filterGridError);
                }
            },
            filterGridCmb: function (GridID, event) {
                var comboBox = $('cmbfilter');
                
                if (comboBox!== null) {
                    var filter = SEL.Grid.getSelectedItem(comboBox.context.activeElement);
                    
                    SEL.Grid.showGridWait(GridID);
                    var grid = SEL.Grid.getGridById(GridID);
                    SEL.Grid.currentPageNum[GridID] = 1;
                    var scfire = grid == null ? '' : grid.ServiceClassForInitialiseRowEvent;
                    var scmfire = grid == null ? '' : grid.ServiceClassMethodForInitialiseRowEvent;
                    Spend_Management.shared.webServices.svcGrid.filterGridByCmb(CurrentUserInfo.AccountID, GridID, filter, grid, scfire, scmfire, SEL.Grid.filterGridComplete, SEL.Grid.filterGridError);
                }
            },
            getSelectedItem: function (comboBox) {
                for (var i = 0; i < comboBox.length ; i++) {
                    if (comboBox[i].selected) {
                        return comboBox[i].value;
                    }
                }
            },
            filterGridComplete: function (data) {
                if ($e(data[0]) === true) {
                    $g(data[0]).innerHTML = data[1];
                    SEL.Grid.updateGrid(data[2]);
                    SEL.Grid.hideGridWait(data[0]);
                }
            },
            filterGridError: function (error) {
                SEL.MasterPopup.ShowMasterPopup(SEL.Grid.GridFilterError, 'Error in ' + SEL.Grid.applicationName);
            },
            selectAllOnGrid: function (GridID) {
                if ($e('selectAll' + GridID) === true) {
                    var chkSelectAll = $g('selectAll' + GridID);

                    var select = chkSelectAll.checked;

                    var chkboxes = document.getElementsByName('select' + GridID);

                    for (var i = 0; i < chkboxes.length; i++) {
                        chkboxes[i].checked = select;
                    }
                }
            },
            getSelectedItemsFromGrid: function (GridID) {
                var selectedItems = new Array();
                var chkboxes = document.getElementsByName('select' + GridID);
                for (var i = 0; i < chkboxes.length; i++) {
                    if (chkboxes[i].checked) {
                        selectedItems.push(new Number(chkboxes[i].value));
                    }
                }
                return selectedItems;
            },
            getSelectedItemFromGrid: function (GridID) {
                var selectedItems = new Array();
                var chkboxes = document.getElementsByName('select' + GridID);
                for (var i = 0; i < chkboxes.length; i++) {
                    if (chkboxes[i].checked) {
                        return chkboxes[i].value;
                    }
                }
                return 0;
            },
            getSelectedItemsGuidFromGrid: function (GridID) {
                var selectedItems = new Array();
                var chkboxes = document.getElementsByName('select' + GridID);
                for (var i = 0; i < chkboxes.length; i++) {
                    if (chkboxes[i].checked) {
                        selectedItems.push(chkboxes[i].value);
                    }
                }
                return selectedItems;
            },
            getItemCountFromGrid: function (GridID) {
                var grid = SEL.Grid.getGridById(GridID);
                return grid.rowCount;
            },
            clearSelectAllOnGrid: function (GridID) {
                if ($e("selectAll" + GridID) === true) {
                    $g("selectAll" + GridID).checked = false;
                    var chkboxes = document.getElementsByName('select' + GridID);

                    for (var i = 0; i < chkboxes.length; i++) {
                        chkboxes[i].checked = false;
                    }
                }
            },
            clearFilter: function (GridID) {
                if ($e(GridID + "_Filter") === true) {
                    if ($g(GridID + "_Filter") === true) {
                        $g(GridID + "_Filter").value = "";
                    }
                }
            },
            doClick: function (butName, GridID, e) {
                //the purpose of this function is to allow the enter key to 
                //point to the correct button to click.
                var key;
                var btn;

                if (window.event) {
                    key = window.event.keyCode;     //IE

                    if (key === 13) {
                        //Get the button the user wants to have clicked
                        btn = $g(butName);
                        if (btn !== null) {
                            SEL.Grid.filterGrid(GridID);
                            event.keyCode = 0;
                        }
                    }
                }
                else {
                    key = e.which;      //firefox

                    if (key == 13) {
                        //Get the button the user wants to have clicked
                        btn = $g(butName);
                        if (btn !== null) {
                            SEL.Grid.filterGrid(GridID);
                            event.keyCode = 0;
                        }
                    }
                }
            },
            updateGridQueryFilterValues: function (GridID, fieldID, values1, values2) {
                var grid = SEL.Grid.getGridById(GridID);
                for (var fieldIdx = 0; fieldIdx < grid.filters.length; fieldIdx++) {
                    if (grid.filters[fieldIdx].fieldID == fieldID) {
                        grid.filters[fieldIdx].values1 = values1;
                        grid.filters[fieldIdx].values2 = values2;
                    }
                }
            }
        };
    }

    if (window.Sys && Sys.loader) {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}
)();
