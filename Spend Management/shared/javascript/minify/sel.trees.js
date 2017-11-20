(function (SEL, $, $g, $f, $e, $ddlValue, $ddlText, $ddlSetSelected, cu, moduleNameHTML, ValidatorEnable, ValidatorUpdateDisplay, validateform, ApplicationBasePath) {
    var scriptName = "Trees";
    function execute() {
        SEL.registerNamespace("SEL.Trees");
        SEL.Trees =
        {
            /* Add a tree's info to the current trees object */
            New: function (treeId, webServiceName, initialMethodName, branchMethodName, dropMethodName) {
                if ($e(treeId)) {
                    SEL.Trees.Instances[treeId] = {
                        WebServicePath: webServiceName,
                        GetInitialNodesWebServiceMethodName: initialMethodName,
                        GetBranchNodesWebServiceMethodName: branchMethodName,
                        GetDroppedNodesWebServiceMethodName: dropMethodName
                    };
                }
            },

            /* Holds an object containing info for the current trees on the page, keyed by treeId */
            Instances: {},

            /* Entity and Filter Information */
            IDs: {
                /*Entity: -1,
                View: -1,*/
                FilterNode: -1,
                FilterDataType: -1,
                FilterConditionType: '',
                FilterFieldID: -1,
                FilterPrecision: 2,
                DateFormat: 'dd/mm/yy',
                CustomModal: {
                    Values: null,
                    MetadataKeys: null,
                    CurrentNode: null
                }
            },

            /* Dom ID's */
            DomIDs: {
                Filters: {
                    Panel: null,
                    Tree: null,
                    Drop: null,
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
                },
                CustomModal: {
                    PanelModalObj: null,
                    Panel: null,
                    PanelTitle: null,
                    Controls: null
                }
            },

            Misc: {
                WebServiceError: function (errorMessage) {
                    $('#loadingArea').remove();

                    SEL.MasterPopup.ShowMasterPopup(
                            'An error has occurred processing your request.<span style="display:none;">' +
                                (errorMessage['_message'] ? errorMessage['_message'] : errorMessage) + '</span>',
                            'Message from ' + moduleNameHTML);
                },

                SetupAjax: (function () {
                    $.ajaxSetup({
                        url: ApplicationBasePath,
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        async: true,
                        error: function (r) {
                            SEL.Common.WebService.ErrorHandler(r);
                        }
                    });
                }())
            },
            ///* z-index's */
            zIndices: {
                Modal: 10000,
                HelpIcon: function () { return SEL.Trees.zIndices.Modal + 10; },
                ViewFilterHelpIcon: function () { return SEL.Trees.zIndices.Modal + 20; },
                Misc:
                {
                    InformationMessage: function () { return SEL.Trees.zIndices.Modal + 25000; }
                }
            },

            Messages: {
                EmptyTree: 'There are no {0} selected.',
                NoneOption: '[None]',
                Filters:
                {
                    NoFilterCriteriaSelected: 'Please select a Filter criteria.',
                    NoFilterColumnSelected: 'Please select a column to apply a filter to.',
                    NoListSelection: 'Please select an item from the list.',
                    Title: 'Filter',
                    Yes: 'Yes',
                    No: 'No',
                    Labels:
                    {
                        DateTime: 'Date and time',
                        Date: 'Date',
                        Time: 'Time',
                        Number: 'Number',
                        Amount: 'Currency amount',
                        Value: 'Value',
                        YesNo: 'Yes or No',
                        Days: 'Number of days',
                        Months: 'Number of months',
                        Weeks: 'Number of weeks',
                        Years: 'Number of years'
                    },
                    ValidatorParts:
                    {
                        PleaseEnter: 'Please enter',
                        PleaseSelect: 'Please select',
                        Date: 'date',
                        Time: 'time (hh:mm)',
                        DateTime: 'date and time',
                        Value: 'value',
                        Integer: 'whole number',
                        Currency: 'amount',
                        Decimal: 'number',
                        YesNo: 'Yes or No',
                        A: 'a',
                        An: 'an',
                        And: 'and',
                        Valid: 'valid',
                        GreaterThan: 'greater than',
                        For: 'for',
                        Between: 'between',
                        IntMax: '2,147,483,647',
                        IntMin: '-2,147,483,647',
                        XMin: '1',
                        XMax: '9999'
                    }
                }
            },

            Tree:
            {
                Clear: function (treeId) {
                    if ($e(treeId)) {
                        var tree = $('#' + treeId),
                            jsTreeSettings;
                        if (tree.jstree) {
                            tree.jstree('deselect_all');
                            jsTreeSettings = tree.jstree('get_settings');
                            jsTreeSettings.json_data.data = {};
                            $.jstree._reference(treeId)._set_settings(jsTreeSettings);
                            tree.jstree('refresh', -1);

                            SEL.Trees.Tree.DisplayEmptyMessage(treeId);
                        }
                    }
                },

                Data:
                {
                    Get: function (treeId, requiredAttributes) {
                        var data = {},
                            attribArray = ['id', 'fieldID', 'joinviaid', 'crumbs'],
                            stuff;
                        if ($e(treeId)) {
                            if ($.isArray(requiredAttributes)) { attribArray.concat(requiredAttributes); }

                            stuff = $.jstree._reference(treeId).get_json(-1, attribArray);
                            data = { data: stuff };  //get the whole tree
                        }

                        return data;
                    },

                    Set: function (treeId, data) {
                        if ($e(treeId) && data !== null && data !== undefined) {
                            var tree = $('#' + treeId),
                                jsTreeSettings = tree.jstree('get_settings');
                            jsTreeSettings.json_data.data = {};
                            $.jstree._reference(treeId)._set_settings(jsTreeSettings);
                            jsTreeSettings.json_data.data = data.data;
                            $.jstree._reference(treeId)._set_settings(jsTreeSettings);
                            tree.jstree('refresh', -1);
                            tree.jstree('deselect_all');

                            SEL.Trees.Tree.DisplayEmptyMessage(treeId);
                        }
                    },

                    Load:
                    {
                        /// <param id="treeId">Required: tree id to load</param>
                        /// <param id="parameters">Optional: the data parameters to pass to the method, must match the name and case of the web method's parameters (default should be a tableId to get fields from)</param>
                        /// <param id="postSuccessFunction">Optional: function called after ajax success handler</param>
                        /// <param id="errorFunction">Optional: replace the default ajax error handler function from SEL.Common</param>
                        InitialNodes: function (treeId, parameters, postSuccessFunction, errorFunction) {
                            SEL.Trees.Tree.Data.Load.LoadMethod('GetInitialNodesWebServiceMethodName', treeId, parameters, postSuccessFunction, errorFunction);
                        },

                        /// <param id="treeId">Required: tree id to load</param>
                        /// <param id="parameters">Optional: the data parameters to pass to the method, must match the name and case of the web method's parameters (default should be an objectId to get columns/filters/rows from)</param>
                        /// <param id="postSuccessFunction">Optional: function called after ajax success handler</param>
                        /// <param id="errorFunction">Optional: replace the default ajax error handler function from SEL.Common</param>
                        DroppedNodes: function (treeId, parameters, postSuccessFunction, errorFunction) {
                            SEL.Trees.Tree.Data.Load.LoadMethod('GetDroppedNodesWebServiceMethodName', treeId, parameters, postSuccessFunction, errorFunction);
                        },

                        LoadMethod: function (methodName, treeId, parameters, postSuccessFunction, errorFunction) {
                            if ($e(treeId) && SEL.Trees.Instances.hasOwnProperty(treeId) && SEL.Trees.Instances[treeId].hasOwnProperty(methodName)) {
                                var treeSettings = SEL.Trees.Instances[treeId],
                                    ajaxOptions = {
                                        url: treeSettings.WebServicePath + '/' + treeSettings[methodName],
                                        data: '',
                                        success: function (r) {
                                            if (r !== null && typeof r === 'object' && r.hasOwnProperty("d") && r.d !== null) {
                                                SEL.Trees.Tree.Data.Set(treeId, r.d);
                                            }
                                        }
                                    };

                                if (parameters !== null) {
                                    ajaxOptions.data = '{ ';
                                    for (var param in parameters) {
                                        if (parameters.hasOwnProperty(param) && parameters[param] !== null
                                        && (typeof parameters[param] === 'string'
                                            || typeof parameters[param] === 'number'
                                                || typeof parameters[param] === 'boolean')) {
                                            if (ajaxOptions.data.length > 2) {
                                                ajaxOptions.data += ',';
                                            }
                                            ajaxOptions.data += '"' + param + '":"' + parameters[param] + '"';
                                        }
                                    }
                                    ajaxOptions.data += ' }';
                                }

                                if (typeof postSuccessFunction === 'function') {
                                    ajaxOptions.success = function (r) {
                                        if (r !== null && typeof r === 'object' && r.hasOwnProperty("d") && r.d !== null) {
                                            SEL.Trees.Tree.Data.Set(treeId, r.d);
                                        }

                                        postSuccessFunction.call(postSuccessFunction);
                                    };
                                }

                                if (typeof errorFunction === 'function') {
                                    ajaxOptions.error = errorFunction;
                                }

                                $.ajax(ajaxOptions);
                            }
                        }
                    }

                },

                ContainsNodeID: function (treeId, nodeId) {
                    if ($e(treeId) && $e(nodeId)) {
                        var tree = $('#' + treeId);
                        if (tree.jstree) {
                            return $('#' + treeId + ' li#' + nodeId).length > 0 ? true : false;
                        }
                    }

                    return false;
                },

                NodeCount: function (treeId) {
                    if ($e(treeId)) {
                        var tree = $('#' + treeId);
                        if (tree.jstree) {
                            return tree.find('ul li').length;
                        }
                    }
                    return 0;
                },

                DisplayEmptyMessage: function (treeId) {
                    if ($e(treeId)) {
                        var tree = $('#' + treeId),
                            nodeNoun;
                        if (tree.jstree) {
                            tree.contents().filter(function () { return this.nodeType === 3; }).remove();
                            nodeNoun = tree.attr('nodenoun');
                            if (tree.find('ul li').length === 0) {
                                if (typeof nodeNoun !== 'string' || nodeNoun === '') { nodeNoun = 'entries'; }
                                if (nodeNoun === 'custom')
                                {
                                    tree.append(SEL.Trees.Messages.EmptyTree.replace('{0}', 'columns'));
                                } else
                                {
                                    tree.append(SEL.Trees.Messages.EmptyTree.replace('{0}', nodeNoun));
                                }
                                
                            }
                            else if (nodeNoun === "filters") {
                                SEL.Trees.DomIDs.Filters.Drop = treeId;
                                SEL.Trees.Filters.RefreshFilterSummaryLayout(treeId);
                            }
                            else if (nodeNoun === 'display fields') {
                                SEL.Trees.DomIDs.Filters.Drop = treeId;
                                SEL.Trees.Filters.RefreshDisplayFieldLayout(treeId);
                            }
                            else if (nodeNoun === 'columns') {
                                SEL.Trees.DomIDs.Filters.Drop = treeId;
                                SEL.Trees.Filters.RefreshDisplayFieldLayout(treeId);
                            }
                            else if (nodeNoun === 'custom') {
                                SEL.Trees.DomIDs.Filters.Drop = treeId;
                                SEL.Trees.Filters.RefreshCustomSummaryLayout(treeId);
                            }
                        }
                    }
                },

                SelectedNode: function (treeId) {
                    var selectedNode = null;
                    if ($e(treeId)) {
                        var tree = $('#' + treeId);
                        if (tree.jstree) {
                            selectedNode = tree.jstree('get_selected');
                        }
                    }
                    return selectedNode;
                }
            },

            Node:
            {
                Move:
                {
                    Between: function (fromTreeId, toTreeId, allowDuplicates) {
                        if (allowDuplicates === undefined || allowDuplicates === null) { allowDuplicates = true; }

                        if ($e(fromTreeId) && $e(toTreeId)) {
                            var tree = $('#' + fromTreeId),
                                treeDrop = $('#' + toTreeId),
                                selNode;
                            if (tree.jstree && treeDrop.jstree) {
                                selNode = tree.jstree('get_selected');
                                if (selNode.length === 1) {
                                    if (!allowDuplicates && treeDrop.find('ul li#copy_' + selNode.attr('id')).length > 0) { return false; }

                                    treeDrop.jstree('create', treeDrop, 'last', {
                                        attr: {
                                            id: 'copy_' + selNode.attr('id'),
                                            crumbs: selNode.attr('crumbs'),
                                            rel: selNode.attr('rel'),
                                            fieldid: selNode.attr('fieldid'),
                                            joinviaid: selNode.attr('joinviaid')
                                        },
                                        data: tree.jstree('get_text', selNode)
                                    }, null, true);

                                    if (!allowDuplicates) { SEL.Trees.Node.Disable(fromTreeId, selNode.attr('id')); }
                                }
                                SEL.Trees.Tree.DisplayEmptyMessage(toTreeId);
                            }
                        }
                        return true;
                    },

                    Up: function (treeId) {
                        if ($g(treeId) !== null) {
                            var treeDrop = $('#' + treeId),
                                selNode,
                                relNode;
                            if (treeDrop.jstree) {
                                selNode = treeDrop.jstree('get_selected');
                                if (selNode.length === 1) {
                                    relNode = selNode.prev();
                                    if (selNode.length === 1 && relNode.length === 1) {
                                        treeDrop.jstree('move_node', selNode, relNode, 'before', false);
                                    }
                                }
                            }
                        }
                    },
                    Down: function (treeId) {
                        if ($g(treeId) !== null) {
                            var treeDrop = $('#' + treeId),
                                selNode,
                                relNode;
                            if (treeDrop.jstree) {
                                selNode = treeDrop.jstree('get_selected');
                                if (selNode.length === 1) {
                                    relNode = selNode.next();
                                    if (selNode.length === 1 && relNode.length === 1) {
                                        treeDrop.jstree('move_node', selNode, relNode, 'after', false);
                                    }
                                }
                            }
                        }
                    }
                },

                Remove:
                {
                    Selected: function (treeId) {
                        if ($e(treeId)) {
                            var tree = $('#' + treeId),
                                selNode,
                                assocId;
                            if (tree.jstree) {
                                selNode = tree.jstree('get_selected');
                                if (selNode.length === 1) {
                                    tree.jstree('delete_node', selNode);
                                    assocId = $('#' + treeId).attr('associatedcontrolid');
                                    if ($e(assocId)) {
                                        SEL.Trees.Node.Enable(assocId, selNode.attr('id').replace('copy_', ''));
                                    }
                                }
                                SEL.Trees.Tree.DisplayEmptyMessage(treeId);
                            }
                        }
                    },

                    All: function (treeId) {
                        if ($e(treeId)) {
                            var assocId = $('#' + treeId).attr('associatedcontrolid'),
                                t = SEL.Trees;
                            t.Tree.Clear(treeId);
                            t.Tree.DisplayEmptyMessage(treeId);

                            if ($e(assocId)) {
                                t.Node.Enable(assocId, 'all');
                            }
                        }
                    },

                    Single: function (treeId, nodeId) {
                        if ($e(treeId)) {
                            var tree = $('#' + treeId),
                                selNode,
                                assocId;
                            if (tree.jstree) {
                                selNode = $('#' + nodeId);
                                if (selNode.length === 1) {
                                    tree.jstree('delete_node', selNode);
                                    assocId = $('#' + treeId).attr('associatedcontrolid');
                                    if ($e(assocId)) {
                                        SEL.Trees.Node.Enable(assocId, nodeId.replace('copy_', ''));
                                    }
                                }
                                SEL.Trees.Tree.DisplayEmptyMessage(treeId);
                            }
                        }
                    }
                },
                GetText: function (treeId, nodeId) {
                    if ($e(treeId)) {
                        var tree = $('#' + treeId),
                                selNode;
                        if (tree.jstree) {
                            selNode = $('#' + nodeId);
                            if (selNode.length === 1) {
                                return tree.jstree('get_text', selNode);
                            }
                        }
                    }
                    return '';
                },

                Disable: function (treeId, nodeId, keyword) {
                    if ((keyword === 'refresh' && $e(treeId)) || (keyword === 'refreshBranch' && $e(treeId) && $e(nodeId))) {
                        var assocId = $('#' + treeId).attr('associatedcontrolid');
                        if ($e(assocId)) {
                            $('#' + assocId + ' ul li').each(function (i, n) {
                                if (typeof n.id === 'string') {
                                    SEL.Trees.Node.Disable((keyword === 'refresh' ? treeId : nodeId), n.id.replace('copy_', ''));
                                }
                            });

                            $('#' + treeId).jstree('deselect_all');
                        }
                    }
                    else if ($e(treeId) && $e(nodeId)) {
                        $('#' + treeId).jstree('deselect_all');
                        $('#' + treeId + ' #' + nodeId).addClass('tree-node-disabled');
                    }
                },

                Enable: function (treeId, nodeId) {
                    if (nodeId === 'all' && $e(treeId)) {
                        $('#' + treeId + ' .tree-node-disabled').removeClass('tree-node-disabled');
                    }
                    else if ($e(treeId) && $e(nodeId)) {
                        $('#' + treeId + ' #' + nodeId).removeClass('tree-node-disabled');
                    }
                }
            },

            Filters:
            {
                FilterModal:
                {
                    FilterEditMode: null
                },

                RefreshFilterSummaryLayout: function (treeId) {
                    $('#' + treeId + ' ul li').each(function (x, node) {
                        var nodeObj = $(node),
                        nodeText;

                        if (nodeObj.attr("summaryLayoutSet") === undefined || nodeObj.attr("summaryLayoutSet") === "") {
                            // Set summary layout to true, so that the node will not be re-evaluated until required
                            nodeObj.attr("summaryLayoutSet", "true");

                            // Set the node text from the custom attribute 'attributeName', incase it has previously been dotinated
                            nodeText = nodeObj.attr("attributeName");
                            if (nodeText === undefined) {
                                nodeText = $('#' + treeId).jstree('get_text', nodeObj);

                                nodeObj.attr("attributeName", nodeText);
                            }
                            
                            SEL.Trees.Filters.UpdateFilterSummaryInformation(nodeObj, treeId);

                            nodeObj.css('width', '600px');
                        }

                        // Set the desired CSS Class of the row
                        if (x % 2 === 0) {
                            nodeObj.find('a').contents().filter('span').removeClass("row2").addClass("row1");
                        }
                        else {
                            nodeObj.find('a').contents().filter('span').removeClass("row1").addClass("row2");
                        }
                    });
                },

                RefreshCustomSummaryLayout: function (treeId) {
                    $('#' + treeId + ' ul li').each(function (x, node) {
                        var nodeObj = $(node),
                        nodeText;

                        if (nodeObj.attr("summaryLayoutSet") === undefined || nodeObj.attr("summaryLayoutSet") === "") {
                            // Set summary layout to true, so that the node will not be re-evaluated until required
                            nodeObj.attr("summaryLayoutSet", "true");

                            // Set the node text from the custom attribute 'attributeName', incase it has previously been dotinated
                            nodeText = nodeObj.attr("attributeName");
                            if (nodeText === undefined) {
                                nodeText = $('#' + treeId).jstree('get_text', nodeObj);

                                nodeObj.attr("attributeName", nodeText);
                            }

                            SEL.Trees.Filters.UpdateCustomSummaryInformation(nodeObj, treeId);

                            nodeObj.css('width', '600px');
                        }

                        // Set the desired CSS Class of the row
                        if (x % 2 === 0) {
                            nodeObj.find('a').contents().filter('span').removeClass("row2").addClass("row1");
                        }
                        else {
                            nodeObj.find('a').contents().filter('span').removeClass("row1").addClass("row2");
                        }
                    });
                    
                    SEL.Trees.Node.Disable(SEL.Reports.IDs.ColumnSelector.Tree, null, 'refresh');
                    if (SEL.Reports.Chart !== undefined) {
                        SEL.Reports.Chart.PopulateFields(SEL.Reports.IDs.ColumnSelector.Drop);
                    }
                },

                RefreshDisplayFieldLayout: function (treeId) {
                    $('#' + treeId + ' ul li').each(function (x, node) {
                        var nodeObj = $(node),
                        nodeText;

                        if (nodeObj.attr("summaryLayoutSet") === undefined || nodeObj.attr("summaryLayoutSet") === "") {
                            // Set summary layout to true, so that the node will not be re-evaluated until required
                            nodeObj.attr("summaryLayoutSet", "true");

                            // Set the node text from the custom attribute 'attributeName', incase it has previously been dotinated
                            nodeText = nodeObj.attr("attributeName");
                            if (nodeText === undefined) {
                                nodeText = $('#' + treeId).jstree('get_text', nodeObj);
                                nodeObj.attr("attributeName", nodeText);
                            }

                            SEL.Trees.Filters.UpdateDisplayFieldSummaryInformation(nodeObj, treeId);
                        }
                        //nodeObj.find('ins').contents().filter('span').addClass("jstree-icon");
                    });
                },

                UpdateFilterSummaryInformation: function (nodeObj, treeId) {
                    // Get information from metadata
                    var criteria1 = nodeObj.data().criterionOne === null ? '' : nodeObj.data().criterionOne,
                        criteria2 = nodeObj.data().criterionTwo === null ? '' : nodeObj.data().criterionTwo,
                        conditionType = typeof nodeObj.data().conditionType === typeof undefined ? '' : nodeObj.data().conditionType,
                        conditionTypeText = typeof nodeObj.data().conditionTypeText === typeof undefined ? '-' : nodeObj.data().conditionTypeText,
                        isListItem = nodeObj.data().isListItem,
                        filterType = nodeObj.data().fieldType === null ? '' : nodeObj.data().fieldType,
                        nodeText = nodeObj.attr("attributeName"),
                        attributeCellInfo = "<span class='nodeText'", // Set the Attribute/Field text to display
                        filterCellInfo,
                        criteriaCellInfo,
                        fullCriteriaValue = '',
                        insElement,
                        aElement,
                        nodeEditImage;

                    if (nodeText.length > 18) {
                        attributeCellInfo += "title='" + nodeText + "'>" + nodeText.substring(0, 18) + "...</span>";
                    }
                    else {
                        attributeCellInfo += ">" + nodeText + "</span>";
                    }

                    // Set the condition text to display
                    filterCellInfo = "<span class='filterInfo'>" + conditionTypeText + "</span>";

                    // If adding a new filter or the metadata is not set correctly, set criteria1 accordingly
                    if (criteria1 === undefined || criteria1 === '' || criteria1 === ' ') {
                        criteria1 = '-';
                    }

                    // If the filter is a Yes/No, set criteria1 accordingly
                    if (filterType === 'X' && !SEL.CustomEntityAdministration.ParentFilter.IsParentFilter) {
                        criteria1 = criteria1 === '1' ? 'Yes' : 'No';
                    }

                    if (SEL.CustomEntityAdministration.ParentFilter.IsParentFilter) {
                        var thisNs1Attributes = SEL.CustomEntityAdministration.Forms.FormFieldDetails;
                      var requiredAttributes = thisNs1Attributes.filter(function (element) {
                            return element.FieldType === 9 && element.AttributeID == criteria1;
                      });
                      if (requiredAttributes[0] != undefined) {
                          criteria1 = requiredAttributes[0].DisplayName.toString();
                        }
                    }

                    var reBracketedCriterion = criteria1.replace(/&lt;/g, '<').replace(/&gt;/g, '>');

                    // If the filter is a List, set the list information
                    if (isListItem) {
                        if (criteria1 !== "@ME" && criteria1 !== "@MY_HIERARCHY" && criteria1 !== "@MY_COST_CODE_HIERARCHY" && !SEL.CustomEntityAdministration.ParentFilter.IsParentFilter) {
                            if (conditionType.toString() !== '10' && conditionType.toString() !== '9') {
                                criteria1 = SEL.Trees.Filters.GenerateCriteriaInformationForListItem(nodeObj);
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
                    criteriaCellInfo = '<span class="criteria" ' + fullCriteriaValue + '>';

                    // If the filter has a condition of "Between", display both values
                    if (conditionType.toString() === '8') {
                        var enterAtRuntime = false;
                        var andString = ' and ';
                        
                        if ($e(SEL.Trees.DomIDs.Filters.FilterModal.Runtime)) {
                            enterAtRuntime = $g(SEL.Trees.DomIDs.Filters.FilterModal.Runtime).checked;
                        }
                        if (enterAtRuntime || criteria2 == '') {
                            andString = '';
                        }
                        criteriaCellInfo += '<span style="font-size: 7pt">{criterion1}</span>';
                        criteriaCellInfo += '<span>{andString}</span><span style="font-size: 7pt">{criterion2}</span></span>';

                        criteriaCellInfo = criteriaCellInfo.replace('{andString}', andString).replace('{criterion1}', criteria1).replace('{criterion2}', criteria2);
                    }
                    else {
                        criteriaCellInfo += criteria1 + '</span>';
                    }

                    nodeObj.html("");

                    insElement = $("<ins>");
                    aElement = $("<a>");
                    nodeEditImage = "<span class=\"editImage\"><img id=\"edit_img_{ID}\" src=\"../images/icons/16/Plain/edit.png\" title=\"Edit filter information\"/></span>".replace('{ID}', nodeObj.attr('id'));

                    aElement.attr("tabIndex", "0");
                    aElement.attr("href", "#");
                    aElement.css("height", "30px");

                    aElement.append(insElement);

                    aElement.append(nodeEditImage);

                    aElement.append(attributeCellInfo);

                    aElement.append(filterCellInfo);

                    aElement.append(criteriaCellInfo);

                    aElement.append(nodeText);

                    nodeObj.append(aElement);

                    var test = treeId;

                    // Add the click() event for the node's edit button
                    $("#edit_img_" + nodeObj.attr('id')).click(function () {
                        $.filterModal.Filters.FilterModal.Edit(nodeObj, test, true);

                        return;
                    });

                    // Add the click() event for list items that need to show field selection information
                    SEL.Trees.Filters.GenerateListFieldSelectionInformation(nodeObj);
                },
                UpdateCustomSummaryInformation: function (nodeObj, treeId) {
                    var nodeText = nodeObj.attr("attributeName"),
                    attributeCellInfo = "<span class='nodeText'", // Set the Attribute/Field text to display
                    filterCellInfo,
                    insElement,
                    aElement,
                    nodeEditImage,
                    crumbs = nodeObj.attr("crumbs");

                    if (crumbs !== '') {
                        nodeText += " (" + nodeObj.attr("crumbs") + ")";
                    }

                    if (nodeText.length > 18) {
                        attributeCellInfo += "title='" + nodeText + "'>" + nodeText.substring(0, 18) + "...</span>";
                    }
                    else {
                        attributeCellInfo += ">" + nodeText + "</span>";
                    }

                    nodeObj.html("");

                    insElement = $("<ins>");
                    aElement = $("<a>");
                    nodeEditImage = "<span class=\"editImage\"><img id=\"edit_img_{ID}\" src=\"../images/icons/16/Plain/edit.png\" title=\"Edit column information\"/></span>".replace('{ID}', nodeObj.attr('id'));

                    aElement.attr("tabIndex", "0");
                    aElement.attr("href", "#");
                    aElement.css("height", "30px");
                    aElement.css("overflow", "hidden");

                    aElement.append(insElement);

                    aElement.append(nodeEditImage);

                    aElement.append(attributeCellInfo);

                    // Get information from metadata by iterating through SEL.Trees.DomIDs.Custom.MetadataKeys
                    for (var i = 0; i < SEL.Trees.IDs.CustomModal.MetadataKeys.length; i++) {
                        filterCellInfo = "<span class='filterInfo'>" + SEL.Trees.IDs.CustomModal.MetadataKeys[i] + " - " + nodeObj.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i]) + "</span>";
                        aElement.append(filterCellInfo);
                    }
                    aElement.append(nodeText);
                    nodeObj.append(aElement);

                    var test = treeId;

                    // Add the click() event for the node's edit button
                    $("#edit_img_" + nodeObj.attr('id')).click(function () {
                        SEL.Trees.Custom.Edit(nodeObj, test, true);

                        return;
                    });

                    // Add the click() event for list items that need to show field selection information
                    SEL.Trees.Filters.GenerateListFieldSelectionInformation(nodeObj);
                },
                UpdateDisplayFieldSummaryInformation: function (nodeObj, treeId) {
                    // Get information from metadata
                    var nodeText = nodeObj.attr("attributeName"),
                        attributeCellInfo = "<span class='nodeText'",
                        insElement,
                        aElement,
                        crumbs = nodeObj.attr("crumbs");

                    if (crumbs === '') {
                        attributeCellInfo += ">" + "</span>";
                    } else {
                        attributeCellInfo += ">" + " (" + nodeObj.attr("crumbs") + ")</span>";
                    }

                    insElement = '<ins class="jstree-icon"/>';
                    nodeObj.html("");
                    aElement = $("<a>");
                    aElement.attr("tabIndex", "0");
                    aElement.attr("href", "#");
                    aElement.append(insElement);
                    aElement.append(nodeText);
                    aElement.append(attributeCellInfo);

                    nodeObj.append(aElement);
                },

                GenerateCriteriaInformationForListItem: function (nodeObj) {
                    var firstListItemText = nodeObj.data().firstListItemText,
                    criteriaText = nodeObj.data().criterionOne === null ? '' : nodeObj.data().criterionOne,
                    listItems = criteriaText.split(',');

                    if (firstListItemText.length > 18) {
                        firstListItemText = '<span title="' + firstListItemText + '">' + firstListItemText.substring(0, 18) + '...';
                    }
                    else {
                        firstListItemText = '<span>' + firstListItemText;
                    }

                    if (listItems.length === 1) {
                        criteriaText = firstListItemText + ' </span>';
                    }
                    else {
                        criteriaText = firstListItemText + ' </span>' + '<span id="listItems_' + nodeObj.attr('id');
                        criteriaText += '" style="text-decoration:underline" title="Click here to see all of the items in this list">and ';
                        criteriaText += (listItems.length - 1) + ' other';

                        if (listItems.length > 2) { criteriaText += 's'; }

                        criteriaText += '</span>';
                    }

                    return criteriaText;
                },

                GenerateListFieldSelectionInformation: function (nodeObj) {

                    var listItemLink = $('#listItems_' + nodeObj.attr('id')),
                        fieldId = nodeObj.attr('fieldid'),
                        commaSeperatedListItems = nodeObj.data().criterionOne === null ? '' : nodeObj.data().criterionOne;

                    listItemLink.click(function () {
                        $.ajax({
                            url: ApplicationBasePath + '/shared/webServices/svcTree.asmx/GetListItemsTextForField',
                            data: '{ "fieldid":"' + fieldId + '", "commaSeperatedIDs":"' + commaSeperatedListItems + '" }',
                            success: function (r) {
                                var fdata = r.d;
                                var ns = SEL.Trees;

                                var selectedListItems = fdata[1],
                                    listInfoSpan = $('#tcPopupListItemContainer'),
                                    listInfoText = '';

                                listInfoSpan.html('');

                                $(selectedListItems).each(function (x, listItemText) {
                                    listInfoText += '<span class="popupListItems"';

                                    if (x === $(selectedListItems).length - 1) {
                                        listInfoText += ' style="border-bottom: none"';
                                    }

                                    listInfoText += '>' + listItemText + '</span>';
                                });

                                listInfoSpan.append(listInfoText);

                                listInfoSpan.css('zIndex', ns.zIndices.Misc.InformationMessage());
                                listInfoSpan.css('left', listItemLink.offset().left - listInfoSpan.outerWidth());
                                listInfoSpan.css('top', listItemLink.offset().top - listInfoSpan.outerHeight());
                                listInfoSpan.fadeIn(400);

                                $(document).click(function (e) {
                                    if (e.target.id.indexOf('listItems_') === -1) {
                                        listInfoSpan.fadeOut(200);

                                        $(document).unbind('click');
                                    }
                                });
                                return true;
                            }
                        });
                    });
                }
            },
            Custom:
            {
                Show: function (selNode, treeId, click) {
                    SEL.Trees.Custom.SetModalTitle(selNode, treeId);
                    $f(SEL.Trees.DomIDs.CustomModal.Panel).show();
                    $('#' + SEL.Trees.IDs.CustomModal.Controls[0]).focus();
                                                        },
                Edit: function (selNode, treeId, click) {
                    SEL.Trees.Filters.FilterModal.FilterEditMode = (click) ? 'click' : 'drag';
                    SEL.Trees.IDs.CustomModal.CurrentNode = selNode;
                    if (SEL.Trees.Filters.FilterModal.FilterEditMode === 'click') {
                        SEL.Trees.Custom.SetModalTitle(selNode, treeId);
                        SEL.Trees.Custom.SetModalControlData(selNode, treeId);
                        $f(SEL.Trees.DomIDs.CustomModal.Panel).show();
                    }
                },
                Save: function (location) {

                    var node = SEL.Trees.IDs.CustomModal.CurrentNode;

                    for (var i = 0; i < SEL.Trees.IDs.CustomModal.MetadataKeys.length; i++) {
                        var control = $('#' + SEL.Trees.IDs.CustomModal.Controls[i]);
                        if (control !== null && control !== undefined) {
                            switch (control[0].type) {
                                case "select-one":
                                    node.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i], control.val());
                                    break;
                                case "checkbox":
                                    node.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i], control[0].checked);
                                    break;
                                default:
                                    node.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i], control[0].val);
                            }
                        }
                    }

                    node.attr("summaryLayoutSet", "");
                    node.css('display', 'block');
                    $('#' + node.id).data(node);

                    $('#' + node.id).css('display', 'block');
                    SEL.Trees.Filters.RefreshCustomSummaryLayout(SEL.Trees.DomIDs.Filters.Drop);
                    $f(SEL.Trees.DomIDs.CustomModal.Panel).hide();
                },
                Cancel: function (location) {
                    SEL.Trees.Node.Remove.Single(SEL.Trees.DomIDs.Filters.Drop, SEL.Trees.IDs.CustomModal.CurrentNode.id);
                },
                SetModalTitle: function (selNode, treeId) {
                    var drop = $('#' + treeId);
                    $("#" + SEL.Trees.DomIDs.CustomModal.PanelTitle).html("Column: " + drop.jstree('get_text', selNode));
                },
                SetModalControlData: function (selNode, treeId) {
                    for (var i = 0; i < SEL.Trees.IDs.CustomModal.MetadataKeys.length; i++) {
                        var control = $('#' + SEL.Trees.IDs.CustomModal.Controls[i]);
                        if (control !== null && control !== undefined) {
                            switch (control[0].type) {
                                case "select-one":
                                    control.val(selNode.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i]));
                                    break;
                                case "checkbox":
                                    control[0].checked = selNode.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i]);
                                    break;
                                default:
                                    control[0].val(selNode.data(SEL.Trees.IDs.CustomModal.MetadataKeys[i]));
                            }

                        }
                    }
                }
            }
        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, jQuery, $g, $f, $e, $ddlValue, $ddlText, $ddlSetSelected, CurrentUserInfo, moduleNameHTML, ValidatorEnable, ValidatorUpdateDisplay, validateform, appPath));
