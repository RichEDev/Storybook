/// <summary>
/// Document Merge Methods
/// </summary>  
(function (SEL, $g, $f, $e, $ddlValue) {
    var scriptName = "DocMerge";

    function execute() {
        SEL.registerNamespace("SEL.DocMerge");

        SEL.DocMerge =
        {
            DomIDs:
            {
                Config: {
                    txtConfigLabel: null,
                    tabMaintainConfig: null,
                    hdnCurrentConfigId: null,
                    chkIsDefaultDocumentGroupingConfig: null,
                    hdnDefaultDocumentGroupingId: null,
                    ddlReportSource: null,
                    hdnReportSortingName: null,
                    divSortingFields: null,
                    txtConfigDescription: null
                },
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
            Ids: {
                FilterDataType: null,
                FilterConditionType: null,
                List: null,
                Index: 0,
                ReportColumnIndex: 0,
                FilterIndex: 0,
                CurrentId: null,
                CurrentDocumentId: null,
                EditMode: null,
                GroupingConfigurationId: null,
                PopupMode: null,
                DuplicateLabelExists: false,
                CurrentConfig: null,
                StatusTimer: null,
                currentMergeRequestNumber: null
            },
            SortOrdering: {
                Ascending: 1,
                Descending: 2
            },

            tmpPanelID: null,
            tmpModalID: null,

            GetStatusCompleteUnknownMax: 0,
            GetStatusCompleteUnknownCount: 0,

            DisplayMasterLoader: function (on) {
                if (on) {
                    $('#ajaxLoaderMaster').css('display', 'block');
                } else {
                    $('#ajaxLoaderMaster').css('display', 'none');
                }

                return;
            },

            DisplayFormLoaders: function (on) {
                if (on) {
                    $('#ajaxLoaderReport').css('display', 'inline');
                } else {
                    $('#ajaxLoaderReport').css('display', 'none');
                }

                return;
            },
            DisplayConfigLoaders: function (on) {
                if (on) {
                    $('#ajaxLoaderConfig').css('display', 'inline');
                } else {
                    $('#ajaxLoaderConfig').css('display', 'none');
                }

                return;
            },
            LaunchModal: function (pnlId, modalId, saveNew) {
                SEL.DocMerge.DisplayFormLoaders(true);

                $g(pnlRepSrc).style.display = 'none';
                $g(pnlId).style.display = '';

                switch (pnlId) {
                    case pnlRepSrc:
                        if (currentProjectId === 0) {
                            SEL.DocMerge.SaveProject(modalId, pnlId);
                            return;
                        } else {
                            Spend_Management.svcDocumentMerge.GetReportsCategories(SEL.DocMerge.GetReportCategoriesComplete, SEL.DocMerge.GetReportCategoriesError);
                        }
                        break;
                    default:
                        break;
                }
                $f(modalId).show();
                $(document).keydown(function (e) {
                    if (e.keyCode === 27) // esc
                    {
                        SEL.Common.HideModal(modalId);
                    }
                });

                return;
            },

            CancelModal: function () {
                $f(repSrcModal).hide();
                return;
            },

            SaveProject: function (modalId, pnlId) {
                var configDoms = SEL.DocMerge.DomIDs.Config;
                if (SEL.Common.ValidateForm('project') === false) {
                    SEL.DocMerge.SetActiveTab(0);
                    return;
                }
                SEL.DocMerge.tmpModalID = modalId;
                SEL.DocMerge.tmpPanelID = pnlId;
                Spend_Management.svcDocumentMerge.SaveProject(currentProjectId, $g(txtProjectName).value, $g(txtProjectDescription).value, $g(configDoms.hdnDefaultDocumentGroupingId).value, $('.blankText').val(), SEL.DocMerge.OnSaveConfigurationFromReportSuccess, SEL.DocMerge.OnSaveConfigurationFromReportError);
            },

            OnSaveConfigurationFromReportSuccess: function (data) {
                if (data === -1) {
                    SEL.MasterPopup.ShowMasterPopup('A document configuration of this name already exists.', 'Page Validation Failed');
                } else {
                    currentProjectId = data;
                    PageMethods.UpdateMergeProjectId(data);

                    if (SEL.DocMerge.tmpModalID != undefined) {
                        //from New Report Source click
                        Spend_Management.svcDocumentMerge.GetReportsCategories(SEL.DocMerge.GetReportCategoriesComplete, SEL.DocMerge.GetReportCategoriesError);
                        SEL.DocMerge.LaunchModal(SEL.DocMerge.tmpPanelID, SEL.DocMerge.tmpModalID);
                    }
                }

                return;
            },

            OnSaveConfigurationFromReportError: function (data) {
                SEL.DocMerge.DisplayFormLoaders(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred saving the configuration. Please contact your administrator if the problem persists.', 'Save Configuration Error');
                return;
            },

            CheckForZeroReportSources: function (numreports) {
                return;
            },

            PopulateDDList: function (ddl, listitems, addNew) {
                var option, ddlIndex = 0;
                ddl.options.length = 0;


                option = document.createElement('option');
                option.value = '0';
                if (addNew) {
                    option.text = '[New]';
                } else {
                    option.text = '[None]';
                }

                ddl.options[ddlIndex] = option;
                ddlIndex++;


                if (listitems !== null) {
                    for (var i = 0; i < listitems.length; i++) {
                        option = document.createElement("option");
                        option.value = listitems[i].Value;
                        option.text = listitems[i].Text;
                        option.selected = listitems[i].Selected;
                        ddl.options[ddlIndex] = option;
                        ddlIndex++;
                    }
                }
                return;
            },

            GetDDLSelectedText: function (ddl) {
                var ddlValue = ddl.options[ddl.selectedIndex].text;
                ddlValue = ddlValue.replace(' - (Sorted)', '');
                return ddlValue;
            },
            GetDDLSelectedValue: function (ddl) {
                return ddl.options[ddl.selectedIndex].value;
            },

            SetDDLSelectedIndex: function (ddl, val) {

                for (var i = 0; i < ddl.options.length; i++) {
                    if (ddl.options[i].value === val) {
                        ddl.options[i].selected = true;
                        break;
                    }
                }
            },

            ShuffleLinks: function (tabNum) {
                var lnkR = $g('lnkAddReportSource');
                // PG add new sources here
                // var lnkF = $g('lnkAddFieldSource');

                lnkR.style.display = "none";
                // PG add new sources here
                //lnkF.style.display = "none";

                switch (tabNum) {
                    case 0:
                        lnkR.style.display = "block";
                        break;
                    case 1:
                        // PG add new sources here
                        // lnkF.style.display = "block";
                        break;
                }

                return;
            },

            SetActiveTab: function (tabIdx) {
                $f(tabContainer).set_activeTabIndex(tabIdx);
                return;
            },

            TabClick: function (sender, args) {
                //SEL.DocMerge.DisplayFormLoadersisplayMasterLoader(true);
                //SEL.DocMerge.ShuffleLinks(sender.get_activeTabIndex());
                var configDoms = SEL.DocMerge.DomIDs.Config;
                var grouping = [];
                switch (sender.get_activeTabIndex()) {
                    case 0:
                        Spend_Management.svcDocumentMerge.ProcessGridRequest(currentProjectId, 'reports', $g(configDoms.hdnDefaultDocumentGroupingId).value, SEL.DocMerge.OnGetReportSourceGridComplete, SEL.DocMerge.OnGetReportSourceGridError);
                        break;
                    case 1:
                        if (currentProjectId === 0) {
                            SEL.DocMerge.SaveProject();
                            break;
                        } else {
                            Spend_Management.svcDocumentMerge.ProcessGridRequest(currentProjectId, 'documentconfigs', $g(configDoms.hdnDefaultDocumentGroupingId).value, SEL.DocMerge.OnGetProjectDocConfigsGridComplete, SEL.DocMerge.OnGetProjectDocConfigsGridError);
                        }
                        break;
                    default:
                        break;
                }
                return;
            },

            tabMaintainConfigClick: function (sender, args) {
                var idx = sender.get_activeTabIndex();
                switch (idx) {
                    case 1:
                        SEL.DocMerge.SetDropHeights("Grouping");
                        break;
                    case 2:
                        var sortingColumns = [];
                        if ($.data(document, 'sortingFields') === undefined) {
                            sortingColumns = SEL.DocMerge.GetSortingColumnsFromUI();
                        } else {
                            sortingColumns = $.data(document, 'sortingFields');
                        }
                        var sortingColumnNames = [];

                        for (var property in sortingColumns) {

                            if (sortingColumns.hasOwnProperty(property)) {
                                var sortColumn = sortingColumns[property];
                                sortingColumnNames.push(sortColumn.Name);
                            }

                        }

                        SEL.DocMerge.LoadSortingColumns(SEL.DocMerge.GetSelectedGroupFields(), sortingColumnNames, sortingColumns);
                        SEL.DocMerge.SetDropHeights("Sorting");
                        break;
                    case 3:
                        SEL.DocMerge.SetDropHeights("Filtering");
                        break;
                }
            },

            //------------------------------------------------------------------------------------------------------------------------------------------------------------

            //GRID PROCESSING ----------------------------------------------------------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------------------------------------------------------------------------      

            OnGetReportSourceGridComplete: function (data) {
                $g(divReportSources).innerHTML = data[2];
                SEL.Grid.updateGrid(data[1]);
                SEL.DocMerge.DisplayMasterLoader(false);
                SEL.DocMerge.CheckForZeroReportSources(data[3]);
                return;
            },

            OnGetReportSourceGridError: function (error) {
                SEL.DocMerge.DisplayMasterLoader(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving the report selections: Please contact your administrator if the problem persists.', 'Report Sources Error');
                return;
            },

            OnGetProjectDocConfigsGridComplete: function (data) {
                $('.docGroupingConfigs').html(data[2]);
                SEL.Grid.updateGrid(data[1]);
                SEL.DocMerge.DisplayMasterLoader(false);
                return;
            },

            OnGetProjectDocConfigsGridError: function (error) {
                SEL.DocMerge.DisplayMasterLoader(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving the document grouping configurations: Please contact your administrator if the problem persists.', 'Report Sources Error');
                return;
            },

            //REPORTS SOURCES ----------------------------------------------------------------------------------------------------------------------------------------

            GetReportCategoriesComplete: function (data) {
                SEL.DocMerge.PopulateDDList($g(ddlReportCategory), data, true);
                var ddlR = $g(ddlReport);
                ddlR.selectedIndex = 0;
                ddlR.disabled = true;
                SEL.DocMerge.DisplayFormLoaders(false);
                return;
            },

            GetReportCategoriesError: function () {
                SEL.DocMerge.DisplayFormLoaders(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving report categories. Please contact your administrator if the problem persists.', 'Retrieve Field Report Categories Error');
                return;
            },

            ReportCategoryIndexChanged: function () {
                var ddlRc = $g(ddlReportCategory),
                    ddlR = $g(ddlReport);

                ddlR.disabled = true;

                if ($ddlValue(ddlReportCategory) === '') {
                    ddlR.options[0].selected = true;
                } else if (ddlRc.selectedIndex > 0) {
                    SEL.DocMerge.DisplayFormLoaders(true);
                    Spend_Management.svcDocumentMerge.GetAvailableReports(currentProjectId, $ddlValue(ddlReportCategory), SEL.DocMerge.GetReportSourcesComplete, SEL.DocMerge.GetReportSourcesError);
                }

                return;
            },

            GetReportSourcesComplete: function (args) {
                var ddlR = $g(ddlReport);
                SEL.DocMerge.PopulateDDList(ddlR, args, true);
                ddlR.disabled = false;
                SEL.DocMerge.DisplayFormLoaders(false);
                return;
            },

            GetReportSourcesError: function (error) {
                SEL.DocMerge.DisplayFormLoaders(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving the report sources. Please contact your administrator if the problem persists.', 'Report Sources Error');
                return;
            },

            AddReportSource: function () {
                if (SEL.Common.ValidateForm('vgReport') === false) {
                    return false;
                }

                SEL.DocMerge.DisplayFormLoaders(true);
                Spend_Management.svcDocumentMerge.AddReportSource(currentProjectId, $ddlValue(ddlReport), SEL.DocMerge.AddReportSourceSuccess, SEL.DocMerge.AddReportSourceError);
                return;
            },

            AddReportSourceSuccess: function (data) {
                SEL.DocMerge.DisplayFormLoaders(false);
                var configDoms = SEL.DocMerge.DomIDs.Config;
                if (data[0] !== 0) {
                    $f(repSrcModal).hide();
                    SEL.DocMerge.DisplayMasterLoader(true);
                    Spend_Management.svcDocumentMerge.ProcessGridRequest(currentProjectId, 'reports', $g(configDoms.hdnDefaultDocumentGroupingId).value, SEL.DocMerge.OnGetReportSourceGridComplete, SEL.DocMerge.OnGetReportSourceGridError);
                } else {
                    SEL.MasterPopup.ShowMasterPopup('The report source has not been added. Please ensure you have not already added it.', 'Save Report Source Error');
                }

                return;
            },

            AddReportSourceError: function (error) {
                SEL.DocMerge.DisplayMasterLoader(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving the report sources. Please contact your administrator if the problem persists.', 'Report Sources Error');
                return;
            },

            ChangeGroupingReportStatus: function (mergeSourceId) {
                var params = "{ 'mergeSourceId': '" + mergeSourceId + "' }";
                SEL.DocMerge.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/", "SwapMergeSourceGroupingStatus", params, SEL.DocMerge.ChangeGroupingReportStatusSuccess, SEL.Common.WebService.ErrorHandler);
            },

            ChangeGroupingReportStatusSuccess: function (data) {
                SEL.Grid.refreshGrid('mergesources', SEL.Grid.getCurrentPageNum('mergesources'));
            },

            DeleteReportSourceSuccess: function (mergesourceid) {
                var configDoms = SEL.DocMerge.DomIDs.Config;
                SEL.DocMerge.DisplayMasterLoader(true);
                Spend_Management.svcDocumentMerge.ProcessGridRequest(currentProjectId, 'reports', $g(configDoms.hdnDefaultDocumentGroupingId).value, SEL.DocMerge.OnGetReportSourceGridComplete, SEL.DocMerge.OnGetReportSourceGridError);
                return;
            },

            DeleteReportSource: function (mergeSourceId) {
                if (confirm('Deleting a report source will also remove all related mappings.\nClick OK to proceed.')) {
                    Spend_Management.svcDocumentMerge.DeleteDocumentMergeSource(mergeSourceId, SEL.DocMerge.DeleteReportSourceSuccess, SEL.DocMerge.ErrorReportSourceProcess);
                }
                return;
            },

            ErrorReportSourceProcess: function (error) {
                SEL.DocMerge.DisplayFormLoaders(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving report sources. Please contact your administrator if the problem persists.', 'Report Sources Error');
                return;
            },

            //------------------------------------------------------------------------------------------------------------------------------------------------------------

            //GROUPING ----------------------------------------------------------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------------------------------------------------------------------------


            LaunchGroupingModal: function (configurationId) {

                SEL.DocMerge.DisplayConfigLoaders(true);
                var configDoms = SEL.DocMerge.DomIDs.Config;
                SEL.DocMerge.Ids.GroupingConfigurationId = configurationId;
                var title = 'New Document Grouping Configuration';
                var highestZIndex = 1 + SEL.Common.GetHighestZIndexInt();
                $('.documentGroupingConfigurationDiv').dialog({
                    title: title,
                    zIndex: highestZIndex,
                    autoOpen: false,
                    modal: true,
                    width: 910,
                    height: 520,
                    closeOnEscape: false,
                    draggable: false,
                    resizable: false
                });

                var modal = $('.documentGroupingConfigurationDiv');
                modal.dialog("option", "dialogClass", "modalpanel");
                modal.dialog("option", "dialogClass", "formpanel");
                modal.on('keydown', function (e) {
                    if (e.keyCode === 27) // esc
                    {
                        e.preventDefault();
                        //check if filter condition modal is open
                        if ($g('ctl00_contentmain_tabMaintainConfig_filteringTab_pnlFilter').style.display === '') {
                            $.filterModal.Filters.FilterModal.Cancel();
                            event.preventDefault();
                            event.stopPropagation();
                            return;
                        } else {
                            SEL.DocMerge.ResetGroupingModalControls();
                            $(modal).dialog("close");
                            return;
                        }
                    }
                });
                $(".ui-dialog-titlebar").hide();
                $(".documentGroupingConfigurationTitle").text(title);
                $('.ui-dialog>.ui-dialog-content>.ajax__tab_container>.ajax__tab_body').css('height', '370px');

                modal.dialog('open');

                $f(configDoms.tabMaintainConfig).set_activeTabIndex(0);

                SEL.DocMerge.LoadCommonFields(currentProjectId, 0);
                SEL.DocMerge.GetReportSources(currentProjectId, configurationId);
                $g(configDoms.hdnCurrentConfigId).value = configurationId;
                $g(configDoms.hdnReportSortingName).value = "";
                SEL.DocMerge.LoadGroupingConfiguration(currentProjectId, configurationId);
                $f(configDoms.tabMaintainConfig).get_tabs()[2].set_enabled(false);
                $f(configDoms.tabMaintainConfig).get_tabs()[3].set_enabled(false);
                if (configurationId > 0) {
                    $f(configDoms.tabMaintainConfig).get_tabs()[0].set_enabled(false);
                    $f(configDoms.tabMaintainConfig).get_tabs()[1].set_enabled(false);
                    $f(configDoms.tabMaintainConfig).get_tabs()[4].set_enabled(false);
                } else {
                    SEL.DocMerge.DisplayConfigLoaders(false);
                }
            },

            DeleteGroupingConfiguration: function (configurationId) {

                if (confirm('Are you sure you wish to delete the selected grouping configuration?')) {
                    var params = "{ 'projectId': '" + currentProjectId + "' , ";
                    params += " 'groupingConfigurationId': '" + configurationId + "' }";

                    this.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/",
                        "DeleteGroupingConfiguration",
                        params,
                        SEL.DocMerge.DeleteConfigurationSuccess,
                        SEL.Common.WebService.ErrorHandler);
                }

                return;
            },

            DeleteConfigurationSuccess: function (data) {

                if (data.d === -1) {
                    SEL.MasterPopup.ShowMasterPopup('An error has occured when deleting the grouping configuration');
                }
                if (data.d === 0) {
                    var configDoms = SEL.DocMerge.DomIDs.Config;

                    Spend_Management.svcDocumentMerge.ProcessGridRequest(currentProjectId, 'documentconfigs', $g(configDoms.hdnDefaultDocumentGroupingId).value, SEL.DocMerge.OnGetProjectDocConfigsGridComplete, SEL.DocMerge.OnGetReportSourceGridError);
                }
            },

            LoadCommonFields: function (projectId, groupingConfigurationId) {
                if (projectId > 0 && groupingConfigurationId > 0) {

                    //load existing grouping configuration
                    SEL.DocMerge.LoadGroupingConfiguration(projectId, groupingConfigurationId);
                } else {
                    //New, so load all common columns
                    var params = "{ 'projectId': '" + projectId + "' }";
                    this.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/", "GetCommonFieldsForGroupingSources", params, SEL.DocMerge.GetCommonFieldsForGroupingSourcesSuccess, SEL.Common.WebService.ErrorHandler);
                }

            },
            GetCommonFieldsForGroupingSourcesSuccess: function (data) {
                $.data(document, 'grouping', data.d.GroupingColumnsList);

                //clears any report sorting configurations from the UI that may have been loaded in previous configurations
                var divReportSorting = SEL.DocMerge.DivReportSorting();
                divReportSorting.css('display', 'none');
            },


            GetSelectedGroupFields: function () {
                var groupingColumns = [];
                $('#torchGroupingColumns li').not("[class='groupingHeader groupingHeaderIncluded']").not("[class='groupingHeader groupingHeaderExcluded']").not("[class='noColumns']").each(function () {
                    var columnName = $(this).text().replace("\r\n", "");
                    groupingColumns.push(columnName);
                });

                return groupingColumns;
            },
            SaveConfiguration: function (merging) {

                var configDoms = SEL.DocMerge.DomIDs.Config;

                if (!merging && validateform('vgConfigLabel') === false) {
                    return;
                }

                var configurationId = SEL.DocMerge.Ids.GroupingConfigurationId;

                if (configurationId == "0") {
                    configurationId = 0;
                }

                if (merging === undefined) {
                    merging = false;
                }

                return SEL.DocMerge.SaveConfigurationToDB(currentProjectId, configurationId, $g(configDoms.txtConfigLabel).value, $g(configDoms.txtConfigDescription).value, merging);
            },

            SaveConfigurationToDB: function (projectId, configurationId, label, description, merging) {
                var config = SEL.DocMerge.Ids.CurrentConfig;
                if (projectId > 0) {
                    config.MergeProjectId = projectId;
                    config.GroupingConfigurationId = configurationId;
                    config.ConfigurationLabel = label;
                    config.ConfigurationDescription = description;
                    config.GroupingColumnsList = [];
                    $('#torchGroupingColumns li').not("[class='groupingHeader groupingHeaderIncluded']").not("[class='groupingHeader groupingHeaderExcluded']").not("[class='noColumns']").each(function () {
                        var columnName = $(this).text().replace("\r\n", "");
                        config.GroupingColumnsList.push(columnName);
                    });

                    config.SortingColumnsList = SEL.DocMerge.GetSortingColumnsFromUI();
                    config.FilteringColumns = SEL.DocMerge.GetFiltersFromUI();
                    config.ReportSortingConfigurations = [];
                    var configDoms = SEL.DocMerge.DomIDs.Config;
                    var ddl = $g(configDoms.ddlReportSource);
                    var selectedReportName = SEL.DocMerge.GetDDLSelectedText(ddl);
                    $('#divSortedColumnFields').css('display', 'block');

                    if (selectedReportName != "[None]") {
                        SEL.DocMerge.SaveReportColumsToDOM(selectedReportName);
                    }

                    var divReportSorting = SEL.DocMerge.DivReportSorting();
                    var documentConfigReportColumns = $(divReportSorting).data();

                    for (var reportName in documentConfigReportColumns) {

                        var reportSortingColumns = documentConfigReportColumns[reportName];
                        var columnDetails = [];

                        for (var reportSortingColumn in reportSortingColumns) {

                            if (reportSortingColumns.hasOwnProperty(reportSortingColumn)) {

                                var reportColumnSortingDetails = reportSortingColumns[reportSortingColumn];
                                columnDetails.push({
                                    ColumnName: reportColumnSortingDetails.ColName,
                                    SortingOrder: reportColumnSortingDetails.SortingOrder
                                });
                            }
                        }
                        var torchReportSorting = { ReportName: reportName, TorchReportSortingColumns: columnDetails };
                        config.ReportSortingConfigurations.push(torchReportSorting);
                    }
                    if (merging) {
                        return config;
                    }

                    SEL.DocMerge.SaveDocumentConfiguration(config);
                }
                return false;
            },

            GetFiltersFromUI: function () {
                var filterColumns = new Array();
                var i = 0;
                $('#torchFilteringColumns li').not("[class='groupingHeader groupingHeaderIncluded']").not("[class='groupingHeader groupingHeaderExcluded']").not("[class='noColumns']").each(function () {
                    var columnName = $(this).text().replace("\r\n", "");
                    var columnId = $(this)[0].id;
                    var filterData = $.data(document, columnId);
                    filterData.ColumnName = columnName;
                    filterColumns.push(filterData);
                    i++;
                });

                return filterColumns;

            },


            SaveDocumentConfiguration: function (config) {
                var configDoms;
                var SaveConfigurationComplete = function (data) {
                    SEL.DocMerge.Ids.DuplicateLabelExists = false;

                    if (data === -1) {
                        SEL.MasterPopup.ShowMasterPopup('This Label already exists. Please provide a unique Label name');
                        SEL.DocMerge.Ids.DuplicateLabelExists = true;
                        return false;
                    } else {
                        configDoms = SEL.DocMerge.DomIDs.Config;
                        SEL.DocMerge.Ids.GroupingConfigurationId = data.d;
                        var chkDefaultDocumentGrouping = $g(configDoms.chkIsDefaultDocumentGroupingConfig);
                        if (chkDefaultDocumentGrouping.checked) {
                            $g(configDoms.hdnDefaultDocumentGroupingId).value = data;
                        } else if ($g(configDoms.hdnDefaultDocumentGroupingId).value == data) {
                            $g(configDoms.hdnDefaultDocumentGroupingId).value = 0;
                        }
                        SEL.DocMerge.Ids.DuplicateLabelExists = false;
                    }

                    SEL.DocMerge.ResetGroupingModalControls();
                    configDoms = SEL.DocMerge.DomIDs.Config;

                    var modal = $('.documentGroupingConfigurationDiv');
                    modal.dialog('close');

                    if (SEL.DocMerge.Ids.PopupMode !== null) {
                        SEL.DocMerge.ResetGroupingModalControls();
                        var modal = $('.documentGroupingConfigurationDiv');
                        modal.dialog('close');
                        Spend_Management.svcDocumentMerge.GetDocumentGroupingConfigurationsGrid(SEL.DocMerge.Ids.CurrentId, SEL.DocMerge.Ids.CurrentDocumentId, ceID, entityId, 'true', SEL.DocMerge.OnGetProjectDocConfigsGridComplete, SEL.DocMerge.OnGetProjectDocConfigsGridError);
                    } else {
                        Spend_Management.svcDocumentMerge.ProcessGridRequest(currentProjectId, 'documentconfigs', $g(configDoms.hdnDefaultDocumentGroupingId).value, SEL.DocMerge.OnGetProjectDocConfigsGridComplete, SEL.DocMerge.OnGetProjectDocConfigsGridError);
                    }
                };
                //Save defaultConfiguration to the project
                configDoms = SEL.DocMerge.DomIDs.Config;
                Spend_Management.svcDocumentMerge.SaveConfiguration(currentProjectId, $g(txtProjectName).value, $g(txtProjectDescription).value, $g(configDoms.hdnDefaultDocumentGroupingId).value, config, $('.blankText').val(), SaveConfigurationComplete, SEL.DocMerge.OnSaveConfigurationFromReportError);

            },

            SaveReportColumsToDOM: function (reportId) {
                var reportColumnFields = SEL.DocMerge.GetReportSortingColumnsFromUI();
                var divReportSorting = SEL.DocMerge.DivReportSorting();
                $(divReportSorting).data(reportId, reportColumnFields);
            },
            LoadGroupingConfiguration: function (projectId, groupingConfigurationId) {
                $("input[value='merge']").parent().hide();

                var params = "{ 'projectId': '" + projectId + "' , ";
                params += " 'groupingConfigurationId': '" + groupingConfigurationId + "' }";

                this.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/",
                    "GetGroupingConfiguration",
                    params,
                    SEL.DocMerge.LoadGroupingConfigurationSuccess,
                    SEL.Common.WebService.ErrorHandler);

            },

            LoadGroupingConfigurationSuccess: function (data) {
                if (data.d !== null && data.d !== undefined) {

                    var groupingConfig = data.d;
                    SEL.DocMerge.Ids.CurrentConfig = groupingConfig;
                    var groupingFields = [];
                    var sortingFields = [];
                    var filterFields = [];
                    var reportSorting = [];
                    var excludedColumns = [];
                    var configDoms = SEL.DocMerge.DomIDs.Config;
                    var globalCommonCols;
                    if (groupingConfig.GroupingConfigurationId !== -1) {
                        $g(configDoms.txtConfigLabel).value = groupingConfig.ConfigurationLabel;
                        $g(configDoms.txtConfigDescription).value = groupingConfig.ConfigurationDescription;
                        if (groupingConfig.GroupingConfigurationId !== 0) {
                            //$g(configDoms.txtConfigLabel).disabled = true;
                        }

                        $g(configDoms.hdnCurrentConfigId).value = groupingConfig.GroupingConfigurationId;

                        var title = 'Document Grouping Configuration: ' + groupingConfig.ConfigurationLabel;
                        $(".documentGroupingConfigurationTitle").text(title);

                        var chkDefaultDocumentGrouping = $g(configDoms.chkIsDefaultDocumentGroupingConfig);
                        $('#' + configDoms.hdnDefaultDocumentGroupingId).val() == groupingConfig.GroupingConfigurationId ? chkDefaultDocumentGrouping.checked = true : chkDefaultDocumentGrouping.checked = false;

                        groupingFields = groupingConfig.GroupingColumnsList;
                        sortingFields = groupingConfig.SortingColumnsList;
                        filterFields = groupingConfig.FilteringColumns;
                        reportSorting = groupingConfig.ReportSortingConfigurations;

                        globalCommonCols = undefined;
                        //populates globalCommonCols for the project.
                        globalCommonCols = $.data(document, 'grouping');

                        if (globalCommonCols !== undefined) {
                            for (var iCountCommonCols = 0; iCountCommonCols < globalCommonCols.length; iCountCommonCols++) {

                                var commonColName = globalCommonCols[iCountCommonCols];

                                if (!SEL.DocMerge.isIncludedCol(commonColName, groupingFields)) {
                                    excludedColumns.push(commonColName);
                                }
                            }
                        }

                    }

                    var documentElement = $.data(document);
                    for (key in documentElement) {

                        if (key.search('edit') != -1) {
                            $.removeData(document, key);
                        }
                    }

                    var filterColumns = new Array();
                    var i = 0;
                    SEL.DocMerge.Ids.Index = 0;
                    SEL.DocMerge.Ids.FilterIndex = 0;
                    for (var property in filterFields) {

                        if (filterFields.hasOwnProperty(property)) {
                            var filterColumn = filterFields[property];
                            var id = 'edit' + SEL.DocMerge.Ids.FilterIndex;
                            SEL.DocMerge.Ids.FilterIndex++;
                            $.data(document, id, filterColumn);

                            filterColumns.push({
                                ColName: filterColumn.ColumnName,
                                ColId: id,
                                Sequence: i
                            });

                            i++;
                        }
                    }

                    $.data(document, 'filterFields', filterColumns);

                    SEL.DocMerge.RenderControls(filterColumns, globalCommonCols, "Filtering");
                    SEL.DocMerge.RenderControls(groupingFields, excludedColumns, "Grouping");
                    SEL.DocMerge.ProcessReportSortingConfigs(reportSorting);

                    SEL.DocMerge.ProcessColumnSorting(sortingFields);
                    $f(configDoms.tabMaintainConfig).get_tabs()[0].set_enabled(true);
                    $f(configDoms.tabMaintainConfig).get_tabs()[1].set_enabled(true);
                    $f(configDoms.tabMaintainConfig).get_tabs()[4].set_enabled(true);
                    $f(configDoms.tabMaintainConfig).set_activeTabIndex(0);
                }
                SEL.DocMerge.DisplayConfigLoaders(false);
            },

            ProcessColumnSorting: function (sortingFields) {
                var sortingColumns = [];
                var i = 0;
                for (var property in sortingFields) {

                    if (sortingFields.hasOwnProperty(property)) {
                        var sortingColumn = sortingFields[property];

                        sortingColumns.push({
                            Name: sortingColumn.Name,
                            SortingOrder: sortingColumn.DocumentSortType,
                            Sequence: i
                        });
                        i++;
                    }

                }
                $.data(document, 'sortingFields', sortingColumns);
            },

            ProcessReportSortingConfigs: function (reportSortingConfigs) {

                var reportName;

                for (var config in reportSortingConfigs) {

                    if (reportSortingConfigs.hasOwnProperty(config)) {
                        var obj = reportSortingConfigs[config];
                        reportName = obj.ReportName;
                        var columns = obj.TorchReportSortingColumns;
                        var sortingColumns = [];
                        var i = 0;
                        SEL.DocMerge.Ids.ReportColumnIndex = 0;

                        for (var column in columns) {

                            if (columns.hasOwnProperty(column)) {
                                var columnDetails = columns[column];
                                var id = 'reportSort' + SEL.DocMerge.Ids.ReportColumnIndex;

                                var sortOrder = columnDetails.SortingOrder;
                                SEL.DocMerge.Ids.ReportColumnIndex++;
                                sortingColumns.push({
                                    ColName: columnDetails.ColumnName,
                                    ColId: id,
                                    SortingOrder: sortOrder,
                                    Sequence: i
                                });
                                i++;
                            }
                        }

                        //save report name and its sorting columns to DOM
                        var divReportSorting = SEL.DocMerge.DivReportSorting();
                        $(divReportSorting).data(reportName, sortingColumns);

                    }
                }
            },

            LoadSortingColumns: function (selectedGroupingFields, sortFieldNames, sortingFields) {
                var finishedLoading = $('#ajaxLoaderConfig').css('display') === 'none';
                if (finishedLoading) {
                    var sortingColumns = new Array();
                    var excludedColumns = [];
                    SEL.DocMerge.Ids.Index = 0;

                    for (var iCountCommonCols = 0; iCountCommonCols < selectedGroupingFields.length; iCountCommonCols++) {
                        var commonColName = selectedGroupingFields[iCountCommonCols];

                        if (SEL.DocMerge.isIncludedCol(commonColName, sortFieldNames)) {
                            var id = 'sort' + SEL.DocMerge.Ids.Index;
                            SEL.DocMerge.Ids.Index++;

                            var columnDetails = SEL.DocMerge.GetSortingColumnDetails(sortingFields, "Name", commonColName);
                            var sortOrderAndSequence = columnDetails.split("**");

                            sortingColumns.push({
                                Name: commonColName,
                                ColId: id,
                                SortingOrder: sortOrderAndSequence[0],
                                Sequence: sortOrderAndSequence[1]
                            });

                        } else {
                            excludedColumns.push(commonColName);
                        }
                    }

                    //data no longer need in document as we can now read from the UI. 
                    $.removeData(document, 'sortingFields');

                    sortingColumns.sort(SEL.DocMerge.DynamicSort("Sequence"));
                    SEL.DocMerge.RenderControls(sortingColumns, excludedColumns, "Sorting");
                }
            },

            GetSortingColumnDetails: function (arr, key, colName) {

                for (var i = 0; i < arr.length; i++) {

                    if (arr[i][key] == colName) {
                        return arr[i].SortingOrder + '**' + arr[i].Sequence;
                    }

                }
                return null;
            },

            isIncludedFilter: function (val, obj) {

                //checks if val is in the object. Gets the columnId to preserve any filter conditions          
                var filteringColumns = [];
                for (var prop in obj) {

                    if (obj.hasOwnProperty(prop)) {

                        var column = obj[prop].split("**");

                        if (column[0] === val) {
                            filteringColumns.push(obj[prop]);
                        }

                    }
                }
                return filteringColumns;
            },

            isIncludedCol: function (val, obj) {
                //checks if val is in the object.
                for (var prop in obj) {
                    if (obj.hasOwnProperty(prop)) {
                        if (obj[prop] === val) {
                            return true;
                        }
                    } else {
                        return false;
                    }
                }
            },

            isIncludedReportCol: function (val, obj) {
                //checks if val is in the object.
                for (var prop in obj) {
                    if (obj.hasOwnProperty(prop)) {
                        if (obj[prop].ColName === val) {
                            return true;
                        }
                    } else {
                        return false;
                    }
                }
            },

            DynamicSort: function (property) {
                var sortOrder = 1;
                if (property[0] === "-") {
                    sortOrder = -1;
                    property = property.substr(1);
                }
                return function (a, b) {
                    var result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
                    return result * sortOrder;
                }
            },
            CancelGroupingConfiguration: function () {
                SEL.DocMerge.ResetGroupingModalControls();
                var modal = $('.documentGroupingConfigurationDiv');
                modal.dialog('close');
            },

            DivSorting: function () {
                return $("#divSortedColumnFields");
            },

            DivReportSorting: function () {
                return $("#divSortedColumnFields");
            },

            GetCommonCols: function (projectId) {
                var params = "{ 'projectId': '" + projectId + "' }";
                this.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/",
                    "GetCommonFieldsForGroupingSources",
                    params,
                    SEL.DocMerge.GetCommonColsSuccess,
                    SEL.Common.WebService.ErrorHandler, true);

            },

            GetSortingColumnsFromUI: function () {
                var sortingFields = [];
                var i = 0;
                $('#torchSortingColumns li').not("[class='groupingHeader groupingHeaderIncluded']").not("[class='groupingHeader groupingHeaderExcluded']").not("[class='noColumns']").each(function () {
                    var columnName = $(this).text().replace("\r\n", "");
                    var sortingOrder = SEL.DocMerge.SortOrdering.Ascending;

                    if ($(this).find("img").attr("class") === "editsort down") {
                        sortingOrder = SEL.DocMerge.SortOrdering.Descending;
                    }

                    sortingFields.push({
                        Name: columnName,
                        DocumentSortType: sortingOrder
                    });
                    i++;
                });

                return sortingFields;
            },

            GetReportSortingColumnsFromUI: function () {
                var sortingFields = [];
                var i = 0;

                $('#torchSortedColumnColumns li').not("[class='groupingHeader groupingHeaderIncluded']").not("[class='groupingHeader groupingHeaderExcluded']").not("[class='noColumns']").each(function () {
                    var columnName = $(this).text().replace("\r\n", "");
                    var columnId = $(this)[0].id;
                    var sortingOrder = SEL.DocMerge.SortOrdering.Ascending;

                    if ($(this).find("img").attr("class") === "editsort down") {
                        sortingOrder = SEL.DocMerge.SortOrdering.Descending;
                    }

                    sortingFields.push({
                        ColName: columnName,
                        ColId: columnId,
                        SortingOrder: sortingOrder,
                        Sequence: i
                    });

                    i++;
                });

                return sortingFields;
            },

            GetCommonColsSuccess: function (data) {
                var globalCommonCols;
                globalCommonCols = undefined;
                globalCommonCols = data.d.GroupingColumnsList;
                $.data(document, 'grouping', globalCommonCols);
            },

            ResetGroupingModalControls: function () {
                var configDoms = SEL.DocMerge.DomIDs.Config;
                if (configDoms.chkIsDefaultDocumentGroupingConfig !== null) {
                    $g(configDoms.chkIsDefaultDocumentGroupingConfig).checked = false;
                }

                $g(configDoms.txtConfigLabel).value = "";
                $g(configDoms.txtConfigLabel).disabled = false;
                //SEL.DocMerge.RenderControls('', '', "SortedColumn");
                var divReportSorting = SEL.DocMerge.DivReportSorting();
                $(divReportSorting).removeData();

            },
            RenderControls: function (groupingFields, excludedColumns, type) {
                var html = "<div id='torchContainer'><ul id='torch" + type + "Bin' class='torch" + type + "Connectable'>";
                var typeLabel = type.replace(/([A-Z])/g, ' $1');
                html += "<li class='groupingHeader groupingHeaderExcluded'>Excluded " + typeLabel + "</li>";
                html += "<li class='noColumns'> No columns have been excluded from the document " + typeLabel + " </li>";
                if (excludedColumns !== undefined) {
                    //populate excluded columns
                    for (var iCountExludedCols = 0; iCountExludedCols < excludedColumns.length; iCountExludedCols++) {
                        var excludedColName = excludedColumns[iCountExludedCols];
                        if (excludedColName.substring(0, 2) !== 'ID') {
                            html += "<li class='ui-state-default'>";
                            html += '<img title="Click here to drag this field" id="draghandle_1622" style="width: 19px; height: 16px; float:left;" alt="" src="/static/icons/Custom/HandleBar.png">';
                            html += excludedColName;
                            html += "</li>";
                        }
                    }
                }

                html += "</ul>";
                html += "<ul id='groupingHelp'>";
                html += "<span class='switcherIconLayout'>";
                html += "<img id='img" + type + "Help' Class='viewSwitcherHelp' " +
                    "ImageUrl='AlternateText='Show View Switcher Information' src='/static/icons/24/plain/information.png'style='width: 19px; height: 16px; float:left;'/>";
                html += "</span>";
                html += "</ul>";
                html += "<ul id='torch" + type + "Columns' class='torch" + type + "Connectable'>";
                html += "<li class='groupingHeader groupingHeaderIncluded'>Included " + typeLabel + "</li>";
                if (groupingFields != undefined) {
                    //populate included columns
                    if (type === "Filtering") {

                        for (var property in groupingFields) {

                            if (groupingFields.hasOwnProperty(property)) {
                                var filterColumn = groupingFields[property];
                                var filterColId = filterColumn.ColId;
                                html += "<li class='ui-state-default' id='";
                                html += filterColId;
                                html += "'";
                                html += '<span><img class="editfilter" title="Edit filter information" id="edit_img" src="/shared/images/icons/16/Plain/edit.png" onclick="$.filterModal.Filters.FilterModal.GetFieldID(\'' + filterColId + '\', true)"  style="width: 19px; height: 16px; float:left;"></span>';
                                html += filterColumn.ColName;
                                html += "</li>";
                            }
                        }
                    } else if (type === "Sorting" || type === "SortedColumn") {

                        for (var property in groupingFields) {

                            if (groupingFields.hasOwnProperty(property)) {
                                var sortingColumn = groupingFields[property];
                                var sortingColId = sortingColumn.ColId;
                                var sortingOrder = sortingColumn.SortingOrder;
                                var sortImage = "arrow_up_blue.png";
                                var sortClass = 'up';

                                if (sortingOrder === "desc" || sortingOrder === SEL.DocMerge.SortOrdering.Descending) {
                                    sortImage = "arrow_down_blue.png";
                                    sortClass = 'down';
                                }

                                html += "<li class='ui-state-default' id='";
                                html += sortingColId;
                                html += "'";
                                html += '<span><img class="editsort ';
                                html += sortClass;
                                html += '" title="Ascending / Descending" id="edit_Icon" src="/shared/images/icons/16/Plain/';
                                html += sortImage;
                                html += '" onclick="SEL.DocMerge.SwitchSort(\'' + sortingColId + '\')" style="width: 19px; height: 16px; float:left;"></span>';
                                if (sortingColumn.ColName === undefined) {
                                    html += sortingColumn.Name;
                                } else {
                                    html += sortingColumn.ColName;
                                }

                                html += "</li>";
                            }
                        }
                    } else {
                        {
                            var gotIncluded = false;
                            for (var iCountIncludedCols = 0; iCountIncludedCols < groupingFields.length; iCountIncludedCols++) {
                                var includedColumnName = groupingFields[iCountIncludedCols];

                                if (includedColumnName.substring(0, 2) != 'ID' && includedColumnName.length > 0) {

                                    html += "<li class='ui-state-default'>";
                                    html += '<img title="Click here to drag this field" id="draghandle_1622" style="width: 19px; height: 16px; float:left;" alt="" src="/static/icons/Custom/HandleBar.png">';

                                }
                                html += includedColumnName;

                                html += "</li>";
                                gotIncluded = true;
                            }
                        }

                        if (gotIncluded) {
                            $f(SEL.DocMerge.DomIDs.Config.tabMaintainConfig).get_tabs()[2].set_enabled(true);
                            $f(SEL.DocMerge.DomIDs.Config.tabMaintainConfig).get_tabs()[3].set_enabled(true);

                        }
                    }
                }
                html += "<li class='noColumns'> No columns have been included in the document " + typeLabel + " </li>";
                html += "</ul>";
                html += "</div>";

                $('#div' + type + 'Fields').html(html);

                var receiveFunction;


                var receiveBinFunction = function (event, ui) {
                    var sortedColumns = ui.sender.attr('id') === "torchSortedColumnColumns", currentValue, groupingColumns = ui.sender.attr('id') === "torchGroupingColumns";

                    $('li.noColumns', this).hide();
                    if ($('li:not(.noColumns)', ui.sender).length === 1) {
                        $('li.noColumns', ui.sender).show();
                        $("input[value='merge']").parent().hide();
                        if (sortedColumns) {
                            currentValue = $('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource + ' :selected').text();
                            $('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource + ' :selected').text(currentValue.replace(' - (Sorted)', ''));
                        }
                        if (groupingColumns) {
                            $f(SEL.DocMerge.DomIDs.Config.tabMaintainConfig).get_tabs()[2].set_enabled(false);
                            $f(SEL.DocMerge.DomIDs.Config.tabMaintainConfig).get_tabs()[3].set_enabled(false);
                        }
                    } else {
                        $('li.noColumns', ui.sender).hide();
                    }
                    if (ui.item[0].firstChild.className === "editfilter") {
                        ui.item.remove();

                    }

                };

                var receiveGroupingFunction = function (event, ui) {
                    $('li.noColumns', this).hide();
                    if ($('li:not(.noColumns)', ui.sender).length === 1) {
                        $('li.noColumns', ui.sender).show();
                    } else {
                        $('li.noColumns', ui.sender).hide();
                        $("input[value='merge']").parent().show();
                        $f(SEL.DocMerge.DomIDs.Config.tabMaintainConfig).get_tabs()[2].set_enabled(true);
                        $f(SEL.DocMerge.DomIDs.Config.tabMaintainConfig).get_tabs()[3].set_enabled(true);
                    }
                }

                var receiveFilterFunction = function (event, ui) {
                    $('li.noColumns', this).hide();
                    if ($('li:not(.noColumns)', ui.sender).length === 0) {
                        $('li.noColumns', ui.sender).show();
                    } else {
                        $('li.noColumns', ui.sender).hide();
                    }

                    ui.item.clone().appendTo("#torch" + type + "Bin").fadeIn("slow");
                    var id = "edit" + SEL.DocMerge.Ids.FilterIndex;
                    SEL.DocMerge.Ids.FilterIndex++;
                    ui.item[0].innerHTML = '<img class="editfilter" title="Edit filter information" id="edit_Icon" src="/shared/images/icons/16/Plain/edit.png" onclick="$.filterModal.Filters.FilterModal.GetFieldID(\'' + id + '\', true)" style="width: 19px; height: 16px; float:left;">' + ui.item.text().replace("\r\n", "");
                    ui.item[0].id = id;
                    $.filterModal.Filters.FilterModal.GetFieldID(id, true);
                };
                var receiveSortFunction = function (event, ui) {
                    $('li.noColumns', this).hide();
                    if ($('li:not(.noColumns)', ui.sender).length === 1) {
                        $('li.noColumns', ui.sender).show();

                    } else {
                        $('li.noColumns', ui.sender).hide();

                    }

                    var id = "sort" + SEL.DocMerge.Ids.Index;
                    SEL.DocMerge.Ids.Index++;
                    ui.item[0].innerHTML = '<img class="editsort up" title="Ascending / Descending" id="edit_Icon" src="/shared/images/icons/16/Plain/arrow_up_blue.png" onclick="SEL.DocMerge.SwitchSort(\'' + id + '\')" style="width: 19px; height: 16px; float:left;">' + ui.item.text().replace("\r\n", "");
                    ui.item[0].id = id;
                };

                var receiveReportSortFunction = function (event, ui) {
                    var currentValue;
                    $('li.noColumns', this).hide();

                    if ($('li:not(.noColumns)', ui.sender).length === 1) {
                        $('li.noColumns', ui.sender).show();
                    } else {
                        $('li.noColumns', ui.sender).hide();
                        currentValue = $('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource + ' :selected').text().replace(' - (Sorted)', '');
                        $('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource + ' :selected').text(currentValue + ' - (Sorted)');
                    }

                    var id = "reportSort" + SEL.DocMerge.Ids.ReportColumnIndex;
                    SEL.DocMerge.Ids.ReportColumnIndex++;
                    ui.item[0].innerHTML = '<img class="editsort up" title="Ascending / Descending" id="edit_Icon" src="/shared/images/icons/16/Plain/arrow_up_blue.png" onclick="SEL.DocMerge.SwitchSort(\'' + id + '\')" style="width: 19px; height: 16px; float:left;">' + ui.item.text().replace("\r\n", "");
                    ui.item[0].id = id;
                };

                switch (type) {
                    case "Filtering":
                        receiveFunction = receiveFilterFunction;
                        break;
                    case "Sorting":
                        receiveFunction = receiveSortFunction;
                        break;
                    case "SortedColumn":
                        receiveFunction = receiveReportSortFunction;
                        break;
                    case "Grouping":
                        receiveFunction = receiveGroupingFunction;
                        break;
                    default:
                        break;
                }

                $("#torch" + type + "Columns").sortable({
                    'dropOnEmpty': true,
                    'scroll': true,
                    revert: true,
                    connectWith: ".torch" + type + "Connectable",
                    cursor: "move",
                    helper: "clone",
                    placeholder: "highlight",
                    items: 'li:gt(0):not(.noColumns)',
                    create: function () {
                        if ($('li:not(.noColumns)', this).length === 1) {
                            return $('li.noColumns', this).show();
                        } else {
                            $("input[value='merge']").parent().show();
                        }
                    },
                    receive: receiveFunction,
                    stop: SEL.DocMerge.RemoveUpDown,
                    update: SEL.DocMerge.UpdateDropHeights

                });
                $("#torch" + type + "Bin").sortable({
                    'dropOnEmpty': true,
                    'scroll': true,
                    revert: true,
                    connectWith: ".torch" + type + "Connectable",
                    cursor: "move",
                    helper: "clone",
                    placeholder: "highlight",
                    items: 'li:gt(0):not(.noColumns)',
                    create: function () {
                        if ($('li:not(.noColumns)', this).length === 1) {
                            return $('li.noColumns', this).show();
                        }
                    },
                    receive: receiveBinFunction,
                    update: SEL.DocMerge.UpdateDropHeights
                });
                $('#img' + type + 'Help').hover(function () {
                    var help = $('#info' + type);
                    var icon = $('#img' + type + 'Help');
                    help.attr('style', 'top: 0');
                    help.attr('style', 'left: 0');
                    help.position({
                        my: 'left bottom',
                        at: 'right top',
                        of: icon,
                        collision: 'flipfit',
                        within: '.documentGroupingConfigurationDiv'
                    });
                    help.stop().css('z-index', 300000).fadeIn(200);
                }, function () {
                    $('#info' + type).stop().fadeOut(200);
                });
            },
            UpdateDropHeights: function (event, ui) {

                if (ui != undefined) {

                    if (ui.item[0].id.indexOf("edit") === -1) {

                        var idOne = ui.item[0].parentNode.id;
                        var idTwo = ui.item[0].parentNode.id;
                        var index = idOne.indexOf('Bin');
                        if (index > 0) {
                            idTwo = idOne.substring(0, index) + "Columns";
                        } else {
                            index = idOne.indexOf('Columns');
                            idTwo = idOne.substring(0, index) + "Bin";
                        }

                        var controlOneHeight = $('#' + idOne).height();
                        var controlTwoHeight = $('#' + idTwo).height();
                        if (controlOneHeight !== controlTwoHeight) {
                            if (controlOneHeight > controlTwoHeight) {
                                $('#' + idTwo).height(controlOneHeight);

                            } else {
                                $('#' + idOne).height(controlTwoHeight);

                            }
                        }
                    }
                }
            },

            SetDropHeights: function (type) {

                var bin = 'torch' + type + 'Bin';
                var included = 'torch' + type + 'Columns';
                var controlOneHeight = $('#' + bin).height();
                var controlTwoHeight = $('#' + included).height();
                var maxHeight = Math.max(controlOneHeight, controlTwoHeight);

                $('#' + bin).height(maxHeight);
                $('#' + included).height(maxHeight);
            },

            //------------------------------------------------------------------------------------------------------------------------------------------------------------

            //MERGE PROJECT + PROCESS ----------------------------------------------------------------------------------------------------------------------------------------------

            //------------------------------------------------------------------------------------------------------------------------------------------------------------

            DeleteProject: function (documentmergeprojectid) {
                if (confirm('Click OK to confirm deletion of entire document configuration')) {
                    SEL.DocMerge.DisplayMasterLoader(true);
                    Spend_Management.svcDocumentMerge.DeleteMergeProject(documentmergeprojectid, SEL.DocMerge.DeleteProjectComplete, SEL.DocMerge.ErrorProjectDelete);
                }

                return;
            },

            ErrorProjectDelete: function (error) {
                SEL.DocMerge.DisplayMasterLoader(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to delete the document configuration. Please contact your administrator if the problem persists.', 'Delete Document Configuration Error');
                return;
            },

            DeleteProjectComplete: function (data) {
                if (data[1] !== '') {
                    SEL.DocMerge.DisplayMasterLoader(false);
                    SEL.MasterPopup.ShowMasterPopup(data[1], 'Delete document configuration');
                } else {
                    Spend_Management.svcDocumentMerge.GetMergeProjectsGrid(SEL.DocMerge.GetMergeProjectsGridSuccess, SEL.DocMerge.GetMergeProjectsGridError);
                }

                return;
            },

            GetMergeProjectsGridSuccess: function (data) {
                $g(divConfigurations).innerHTML = data[1];
                SEL.Grid.UpdateGrid(data[0]);
                SEL.DocMerge.DisplayMasterLoader(false);
                return;
            },

            GetMergeProjectsGridError: function (data) {
                SEL.DocMerge.DisplayMasterLoader(false);
                SEL.MasterPopup.ShowMasterPopup('An error occurred retrieving document configuration information. Please contact your administrator if the problem persists.', 'Document Configuration Error');
            },

            DefaultConfigCheckboxClick: function () {
                //required as the TwoColumnEvent needs to call a JS function to 
                return;
            },

            CallExecMerge: function (projectid, mergeStatus) {
                currentMergeProjectId = projectid;

                var pnl = $find(mdlMergeExec);
                if (pnl !== null) {
                    pnl.show();
                }
            },

            PerformMerge: function (projectid, documentid, recordid, entityid, configId, config) {
                $('#loadingMessage').css('display', '');
                $('#reportStatus').css('display', 'none');
                $('#divMergeProgress').html('');
                $('#performMergeButtons').css('display', '');
                var rProgress = $('#reportProgress');
                $('#lnkDownload').remove();
                if (rProgress !== undefined && rProgress !== null) {
                    rProgress.css('display', 'none');
                }
                var rPercent = $('#reportPercentDone');
                if (rPercent !== undefined && rPercent !== null) {
                    rPercent.css('display', 'none');
                }

                if (configId === undefined || configId === null) {
                    configId = -1;
                }
                var configText = '';
                if (config !== undefined) {
                    configId = -2;
                    configText = '&ct=' + JSON.stringify(config);
                }
                currentMergeProjectId = projectid;
                //$('#mergeDialog').dialog({
                //    title: "Merge Document",
                //    width: 350,
                //    Height: 140,
                //    draggable: false,
                //    resizable: false,
                //    modal: true,
                //    closeOnEscape: false
                //});
                //var modal = $('#mergeDialog');
                //modal.dialog("option", "dialogClass", "modalpanel");
                //modal.dialog("option", "dialogClass", "formpanel");
                //$(".ui-dialog-titlebar").hide();


                // create an element for $.dialog if it isn't already on the page
                //var dialogElementId = "documentMergeModal";
                //if (!$("#" + dialogElementId).length) {
                //    $("body").append($("<div>").attr({ "id": dialogElementId }).css({ "overflow": "hidden"}));
                //}

                var executeMerge = function (store) {
                    $('#performMergeButtons').css('display', 'none');
                    $('#loadingMessage').css('display', 'none');

                    var rStatus = $('#reportStatus');
                    if (rStatus !== undefined && rStatus !== null) {
                        rStatus.css('display', '');
                    }
                    var rProgress = $('#reportProgress');
                    if (rProgress !== undefined && rProgress !== null) {
                        rProgress.css('display', 'none');
                    }
                    var rPercent = $('#reportPercentDone');
                    if (rPercent !== undefined && rPercent !== null) {
                        rPercent.css('display', 'none');
                    }
                    $('#divMergeProgress').html('');
                    if (!store) {
                        store = false;
                    }
                    if (SEL.DocMerge.Ids.CurrentConfig !== undefined && SEL.DocMerge.Ids.CurrentConfig !== null) {
                        Spend_Management.svcDocumentMerge.PerformTempDocumentMerge(projectid, documentid, recordid, entityid, SEL.DocMerge.Ids.CurrentConfig, store, SEL.DocMerge.InitMerge, SEL.DocMerge.OnMergeFail);
                    } else {
                        Spend_Management.svcDocumentMerge.PerformDocumentMerge(projectid, documentid, recordid, entityid, configId, store, SEL.DocMerge.InitMerge, SEL.DocMerge.OnMergeFail);
                    }
                };
                $("#mergeDialog").dialog({
                    resizable: false,
                    modal: true,
                    width: 485,
                    height: 205,
                    title: "Perform Document Merge",
                    closeOnEscape: false,
                    close: function () {
                        SEL.Grid.refreshGrid("gridTorchAttachments", SEL.Grid.getCurrentPageNum("gridTorchAttachments"));
                        $('#' + btnPerformMerge).off();
                        $('#' + btnPerformMergeAndSave).off();
                        $('#' + btnPerformMergeAndSave).off();
                    },
                    open: function (event, ui) {
                        SEL.DocMerge.Ids.CurrentConfig = config;
                        $(".ui-icon-closethick").hide();
                        $(".ui-dialog-titlebar-close").hide();
                        $('#' + btnPerformMerge).click(function (event) {
                            executeMerge.call(this);
                        });
                        $('#' + btnPerformMergeAndSave).click(function (event) {
                            executeMerge.call(this, true);
                        });
                        $('#' + btnCancelMerge).click(function (event) {
                            $('#mergeDialog').dialog("close");
                        });
                        $(".ui-dialog-titlebar").hide();
                    },
                    dialogClass: "modalpanel formpanel"
                });
            },
            PopupWindow: function (url, title, w, h) {
                var left = (screen.width / 2) - (w / 2);
                var top = (screen.height / 2) - (h / 2);

                window.open(url, title, 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=no, copyhistory=no, dependent=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
            },

            OnMergeFail: function (error)
            {
                var msg;
                if (typeof(error._message) == "string")
                    msg = error._message;
                else
                    msg = error;
                SEL.MasterPopup.ShowMasterPopup('A problem occurred while attempting to perform the Torch process.<br />' + msg, 'Torch');
                $('#mergeDialog').dialog('close');
                return;
            },

            CancelMerge: function () {
                currentMergeProjectId = null;
                $find(mdlMergeExec).hide();
            },

            DoMerge: function () {
                if (SEL.Common.ValidateForm('valMergeOptions') === false) {
                    return false;
                }

                // get dialog values set
                var exporttype = $ddlValue(lstdocumentformat);
                var toaddress = $g(txtemailto).value;
                var cc = $g(txtemailcc).value;
                var subject = $g(txtsubject).value;
                var msgbody = $g(txtemailbody).value;

                Spend_Management.svcDocumentMerge.PerformDocumentMerge(currentMergeProjectId, exporttype, toaddress, cc, subject, msgbody, SEL.DocMerge.OnMergeEmailSuccess, SEL.DocMerge.OnMergeFail);
                $('#GatheringData').css('display', 'none');
                return true;
            },

            OnMergeEmailSuccess: function () {
                SEL.MasterPopup.ShowMasterPopup('Torch was successful and has been emailed to specified recipients.', 'Torch');
                return;
            },

            ExportKeys: function (projectid) {
                window.open(appPath + '/shared/documentMerge.aspx?mtags=1&mprj=' + projectid, 'Export_Merge_Tags', 'width=550,height=110,resizable=yes,scrollbars=1,status=no,menubar=no,dependent=yes');
            },

            GetMergeProgress: function () {
                Spend_Management.svcDocumentMerge.GetMergeStatus(currentMergeProjectId, SEL.DocMerge.Ids.currentMergeRequestNumber, SEL.DocMerge.GetStatusComplete, SEL.DocMerge.GetStatusError);
            },

            GetStatusError: function (error)
            {
                $("#reportStatus").hide();

                var msg;
                if (typeof (error._message) == "string")
                    msg = error._message;
                else
                    msg = error;

                $('#divMergeProgress')
                    .html('An error ocurred while attempting to call the Torch status update<br />' + msg + '<br /><br />')
                    .append($('<span class="buttonContainer"><input type="button" class="buttonInner" value="Close" onclick="$(\'#mergeDialog\').dialog(\'close\');" /></span>'));
            },

            GetStatusComplete: function (data) {
                if (data !== null) {
                    var mergeProjectId = data[0],
                        mergeRequestNum = data[1],
                        processedCount = data[2],
                        mergeStatus = data[3],
                        totalToProcess = data[4],
                        errorMessage = data[5],
                        divForm,
                        rStatus,
                        rProgress,
                        rPercent;

                    if (totalToProcess > 0) {
                        SEL.DocMerge.SetProgressBar(parseInt(parseInt(processedCount) / parseInt(totalToProcess) * 100));
                    } else if (mergeStatus > 0) {
                        SEL.DocMerge.SetProgressBar(0);
                    }
                    var linktext;
                    switch (mergeStatus) {
                        case 1:
                            // in-progress
                            divForm = $('#divMergeProgress');

                            if (divForm !== undefined && divForm !== null) {
                                if (processedCount != totalToProcess) {
                                    divForm.html('Please wait, report data is being collected...(' + processedCount + '/' + totalToProcess + ')');
                                } else {
                                    rProgress = $('#reportProgress');
                                    if (rProgress !== undefined && rProgress !== null) {
                                        rProgress.css('display', 'none');
                                    }
                                    rPercent = $('#reportPercentDone');
                                    if (rPercent !== undefined && rPercent !== null) {
                                        rPercent.css('display', 'none');
                                    }
                                    divForm.html('Report data collected, Torch is now merging. Please wait.......<img src="/shared/images/ajax-loader.gif" alt="" />');
                                }
                            }


                            SEL.DocMerge.Ids.StatusTimer = setTimeout(SEL.DocMerge.GetMergeProgress, 500);
                            break;
                        case 2:
                            // complete
                            SEL.DocMerge.Ids.CurrentConfig = null;
                            rStatus = $('#reportStatus');
                            if (rStatus !== undefined && rStatus !== null) {
                                rStatus.css('display', 'none');
                            }


                            divForm = $('#divMergeProgress');

                            if (divForm !== undefined && divForm !== null) {
                                divForm.html('');
                                if (SEL.DocMerge.Ids.StatusTimer !== null) {
                                    clearTimeout(SEL.DocMerge.Ids.StatusTimer);
                                }
                                if (navigator.appVersion.indexOf("MSIE 7.") === -1) {
                                    $('#mergeDialog').dialog('close');
                                }
                            }
                            if (navigator.appVersion.indexOf("MSIE 7.") !== -1) {
                                var opening = '<div>Torch complete.</div><div><br/></div>';
                                linktext = 'documentMerge.aspx?mprj=' + mergeProjectId + '&mrn=' + SEL.DocMerge.Ids.currentMergeRequestNumber + '&mc=1';
                                divForm.html(opening + '<span class="buttonContainer"><a id="lnkDownload" href="' + linktext + '">Download</a></span>');
                                $("#lnkDownload").button().click(function (event) {
                                    $('#mergeDialog').dialog('close');
                                });

                            } else {
                                document.location.href = 'documentMerge.aspx?mprj=' + mergeProjectId + '&mrn=' + SEL.DocMerge.Ids.currentMergeRequestNumber + '&mc=1';
                            }
                            break;
                        case 3:
                            // error occurred
                            divForm = $('#divMergeProgress');

                            if (divForm !== undefined && divForm !== null) {
                                linktext = '#';
                                divForm.html('An error has occurred performing the Torch. <br/><div><br/></div><div style="display:none">' + errorMessage + '</div><span class="buttonContainer buttonInner" id="lnkDownload" href="' + linktext + '">Close</span><div style="display:none"></div>');

                            }
                            $("#lnkDownload").button().click(function (event) {
                                $('#mergeDialog').dialog('close');
                            });
                            break;
                        default:
                            // unknown, try again
                            SEL.DocMerge.GetStatusCompleteUnknownCount++;
                            if (SEL.DocMerge.GetStatusCompleteUnknownCount < SEL.DocMerge.GetStatusCompleteUnknownMax)
                            {
                                SEL.DocMerge.Ids.StatusTimer = setTimeout(SEL.DocMerge.GetMergeProgress, 500);
                            }
                            else
                            {
                                $('#reportStatus').hide();
                                $('#divMergeProgress').html('A problem occurred during the Torch process.<br />Please check your document configuration or try Perform Merge and add to Torch History.<br/><div><br/></div><div style="display:none"></div><span class="buttonContainer buttonInner" id="lnkDownload" href="#">Close</span><div style="display:none"></div>');
                                $("#lnkDownload").button().click(function (event)
                                {
                                    $('#mergeDialog').dialog('close');
                                    $('#reportStatus').show();
                                });
                            }
                            break;
                    }

                    $("#lnkDownload").mouseover(function () {
                        $('#lnkDownload').removeClass("ui-state-hover");
                    });
                    $('.ui-button-text').removeClass('ui-button-text').addClass('buttonInner');
                    $('#lnkDownload').removeClass('ui-state-default');
                }
            },

            InitMerge: function (requestnum) {
                $('#loadingMessage').css('display', 'none');
                var rProgress = $('#reportProgress');
                if (rProgress !== undefined && rProgress !== null) {
                    rProgress.css('display', '');
                }
                var rPercent = $('#reportPercentDone');
                if (rPercent !== undefined && rPercent !== null) {
                    rPercent.css('display', '');
                }
                SEL.DocMerge.Ids.currentMergeRequestNumber = requestnum;

                SEL.DocMerge.Ids.StatusTimer = setTimeout(SEL.DocMerge.GetMergeProgress, 500);
            },

            SetProgressBar: function (doneWidth) {
                var percentDone = $('#reportPercentDone');
                var progressBar = $('#reportDone');
                if (doneWidth != '100') {

                    percentDone.html(doneWidth + '%');
                    var divWidth = doneWidth * 2.5;
                    progressBar.css('width', divWidth);
                } else {
                    percentDone.html('');
                    progressBar.css('width', 0);
                }
                return;
            },

            Service: function (path, method, params, sucessFunction, failFunction, isAsync) {
                if (!path.endsWith('/')) {
                    path = path + '/';
                }
                var async = false;
                if (isAsync === undefined) {
                    async = true;
                }
                $.ajax({
                    url: path + method,
                    type: 'POST',
                    data: params,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: sucessFunction,
                    error: failFunction,
                    async: async
                });
            },
            Filters:
                        {
                            FilterModal: null
                        },

            GetReportSources: function (projectId, GroupingConfigurationId) {

                var params = "{ 'projectId': '" + projectId + "', 'groupingConfigurationId': '" + GroupingConfigurationId + "' } ";
                this.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/",
                 "GetReportSources",
                 params,
                 SEL.DocMerge.GetReportSourcesSuccess,
                 SEL.Common.WebService.ErrorHandler);
            },

            GetReportSourcesSuccess: function (data) {
                SEL.DocMerge.PopulateDDList($('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource)[0], data.d, false);
                $('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource).value = 0;
            },

            ReportSourceSelected: function () {

                var params = "{ 'projectId': '" + currentProjectId + "' , ";
                params += " 'reportId': '" + $('#' + SEL.DocMerge.DomIDs.Config.ddlReportSource).val() + "' }";

                this.Service(window.appPath + "/shared/webServices/SvcDocumentMerge.asmx/",
                    "GetReportColumns",
                    params,
                    SEL.DocMerge.GetReportColumnsSuccess,
                    SEL.Common.WebService.ErrorHandler);

            },

            GetReportColumnsSuccess: function (data) {

                var divReportSorting = SEL.DocMerge.DivReportSorting();
                divReportSorting.css('display', 'block');
                var reportName = $g(SEL.DocMerge.DomIDs.Config.hdnReportSortingName).value;
                var ddl = $g(SEL.DocMerge.DomIDs.Config.ddlReportSource);

                var selectedReportName = SEL.DocMerge.GetDDLSelectedText(ddl);

                if (reportName !== "") {
                    //save report columns for the previously selected report - if any.
                    SEL.DocMerge.SaveReportColumsToDOM(reportName);
                }

                $g(SEL.DocMerge.DomIDs.Config.hdnReportSortingName).value = selectedReportName;
                var sortedReportColumns = $(divReportSorting).data(selectedReportName);
                var excludedColumns = [];


                if (sortedReportColumns != undefined) {
                    var allReportColumns = data.d;

                    for (var iCountCommonCols = 0; iCountCommonCols < allReportColumns.length; iCountCommonCols++) {

                        var commonColName = allReportColumns[iCountCommonCols];

                        if (!SEL.DocMerge.isIncludedReportCol(commonColName, sortedReportColumns)) {
                            excludedColumns.push(commonColName);
                        }
                    }
                }
                else {
                    excludedColumns = data.d;
                }

                SEL.DocMerge.RenderControls(sortedReportColumns, excludedColumns, "SortedColumn");
                SEL.DocMerge.SetDropHeights("SortedColumn");

            },
            SwitchSort: function (controlId) {
                var control = $('#' + controlId + '>#edit_Icon');
                if (control.hasClass('up')) {
                    control.attr("src", "/shared/images/icons/16/Plain/arrow_down_blue.png");
                    control.removeClass('up');
                    control.addClass('down');
                } else {
                    control.attr("src", "/shared/images/icons/16/Plain/arrow_up_blue.png");
                    control.removeClass('down');
                    control.addClass('up');
                }
            },
            RemoveUpDown: function (event, ui) {

                if (ui.item[0].id.indexOf("edit") === -1) {

                    var parent = ui.item[0].parentElement.id;
                    if (ui.item[0].id !== "" && parent.indexOf('Bin') !== -1) {
                        var control = $('#' + ui.item[0].id + '>#edit_Icon');
                        if (control.hasClass('up') || control.hasClass('down')) {
                            control.attr('src', "/static/icons/Custom/HandleBar.png");
                            control.attr('title', "Click here to drag this field");
                            control.removeClass('up');
                        }
                    }
                }
            },
            EditMerge: function (projectId, documentId) {
                var title = 'Merge Configurations';
                SEL.DocMerge.Ids.CurrentId = projectId;
                currentProjectId = projectId;
                SEL.DocMerge.Ids.CurrentDocumentId = documentId;
                var highestZIndex = 1 + SEL.Common.GetHighestZIndexInt();
                SEL.DocMerge.Ids.PopupMode = 'true';

                $('.dialogHolder').dialog({
                    title: title,
                    zIndex: highestZIndex,
                    autoOpen: false,
                    modal: true,
                    width: 910,
                    height: 520,
                    closeOnEscape: true,
                    draggable: false,
                    resizable: false,
                    open: function () {
                        Spend_Management.svcDocumentMerge.GetDocumentGroupingConfigurationsGrid(SEL.DocMerge.Ids.CurrentId, SEL.DocMerge.Ids.CurrentDocumentId, ceID, entityId, 'true', SEL.DocMerge.OnGetProjectDocConfigsGridComplete, SEL.DocMerge.OnGetProjectDocConfigsGridError);
                        Spend_Management.svcDocumentMerge.GetProject(SEL.DocMerge.Ids.CurrentId, SEL.DocMerge.OnGetProjectComplete, SEL.DocMerge.OnGetProjectDocConfigsGridError);
                    }
                }
                );

                var modal = $('.dialogHolder');
                modal.dialog("option", "dialogClass", "modalpanel");
                modal.dialog("option", "dialogClass", "formpanel");
                $(".ui-dialog-titlebar").hide();
                $('.ui-dialog>.ui-dialog-content>.ajax__tab_container>.ajax__tab_body').css('height', '370px');
                modal.dialog('open');
            },
            OnGetProjectDocConfigsGrid2Complete: function (data) {
                $('.dialogHolder>form>.docGroupingConfigs').html(data[2]);
                SEL.Grid.updateGrid(data[1]);
                SEL.DocMerge.DisplayMasterLoader(false);
                return;
            },
            OnGetProjectComplete: function (project) {
                $('#' + txtProjectName).val(project.MergeProjectName);
                $('#' + txtProjectDescription).val(project.MergeProjectDescription);
                $('#' + SEL.DocMerge.DomIDs.Config.hdnDefaultDocumentGroupingId).val(project.DefaultDocumentGroupingConfigId);
                $('.blankText').val(project.BlankTableText);
            },
            MergeCurrentConfig: function () {
                var config = SEL.DocMerge.SaveConfiguration(true);
                SEL.DocMerge.PerformMerge(config.MergeProjectId, SEL.DocMerge.Ids.CurrentDocumentId, ceID, entityId, config.GroupingConfigurationId, config);
                SEL.DocMerge.CancelGroupingConfiguration();
            }
        };
    }

    if (window.Sys && window.Sys.loader) {
        window.Sys.loader.registerScript(scriptName, null, execute);
    }
    else {
        execute();
    }
}(SEL, $g, $f, $e, $ddlValue));