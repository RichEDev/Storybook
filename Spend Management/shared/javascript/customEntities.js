var srcRTECntl;
var srcRTETxt;
var srcRTEPanel;
var previousScrollForFloatingButtons;

$(document).ready(function () {
    SEL.Common.SetTextAreaMaxLength();

    // Hide the dashed line (border-bottom) for the last field in each section
    $('.sm_panel').each(function (p, panel) {
        $(panel).children('.sectiontitle').each(function (s, section) {
            var prevDiv = $(section).prev('div');

            if (prevDiv.attr('class') !== 'sectiontitle' && prevDiv.attr('class') !== 'onecolumnpanel') {
                prevDiv.css('border-bottom', 'none');
            }
        });

        var lastDiv = $(panel).children('div').last();

        if (lastDiv.attr('class') !== 'sectiontitle' && lastDiv.attr('class') !== 'onecolumnpanel') {
            lastDiv.css('border-bottom', 'none');
        }
    });

    $(".ui-widget-overlay").css("height", Math.max($("body").height(), $(window).height()));

});

//TODO: Move to SEL.Common if we are happy to have datepicker and timepicker in there
function SetupDateAndTimeFields() {
    // Create the date picker controls
    $('.dateField').datepicker().attr('maxlength', 10);

    // Create the time picker controls
    $('.timeField').timepicker({
        showCloseButton: true,
        showNowButton: true,
        showDeselectButton: true
    }).attr('maxlength', 5);

    // Setup the focus events for date and time fields
    $('.dateField, .timeField').focus(function () {
        // Remove the 'focus token' from the previous control
        $('.hasCalControl').removeClass('hasCalControl');

        // Give the 'focus token' to the new control        
        $(this).addClass('hasCalControl');
    });

    // Setup the blur events for date fields
    $('.dateField').blur(function () {
        var dateValue = $(this).val();

        if ($.isNumeric(dateValue) && dateValue.length === 8) {
            var newDateValue = dateValue.substring(0, 2) + "/";

            newDateValue += dateValue.substring(2, 4) + "/";

            newDateValue += dateValue.substring(4, dateValue.length);

            $(this).val(newDateValue);

            // Refresh validators if the field has been updated
            $(this).parent().nextAll('.inputvalidatorfield').first().children().each(function () {
                var val = $g($(this).attr('id'));
                ValidatorValidate(val);
            });
        }
    });

    // Setup the blur events for time fields
    $('.timeField').blur(function () {
        var timeValue = $(this).val();

        if (timeValue.length === 5 && timeValue.indexOf(":") === 2) {
            if ($.isNumeric(timeValue.replace(":", ""))) {
                if (timeValue.substring(0, 2) < 24 && timeValue.substring(3, 5) < 60) {
                    // Refresh validators if the field has been updated
                    $(this).parent().nextAll('.inputvalidatorfield').first().children().each(function () {
                        var val = $g($(this).attr('id'));
                        ValidatorValidate(val);
                    });
                }
            }
        }
    });

    // Setup the click events for calendar images
    $('.dateCalImg, .timeCalImg').click(function () {
        var inputControl = $(this).parent().prev().children().first();

        if (inputControl.is(':disabled')) return false;

        if (inputControl.hasClass('hasCalControl')) {
            var pickerDiv = $(this).hasClass('dateCalImg') ? '#ui-datepicker-div' : '#ui-timepicker-div';

            if ($(pickerDiv).css('display') === 'none') {
                inputControl.focus();
            }
            else {
                if ($(pickerDiv).is(':animated') === false) {
                    $(pickerDiv).fadeOut(100);
                }
            }
        }
        else {
            inputControl.focus();
        }
    });

    // Ensure that both the time and date portions of the field are entered
    $('.smallDateField, .smallTimeField').change(function () {
        var dateElement = $(this).parent().children().eq(0);
        var timeElement = $(this).parent().children().eq(1);
        var reqDateVal = $(this).parent().nextAll('.inputvalidatorfield').first().children('.reqDateVal')[0];
        var reqTimeVal = $(this).parent().nextAll('.inputvalidatorfield').first().children('.reqTimeVal')[0];
        var mandatoryVal = $(this).parent().nextAll('.inputvalidatorfield').first().children('.reqDateTimeVal')[0];

        if (dateElement.val().length === 0 && timeElement.val().length === 0) {
            reqDateVal.enabled = reqTimeVal.enabled = false;
            ValidatorValidate(reqDateVal);
            ValidatorValidate(reqTimeVal);

            if (mandatoryVal !== undefined) {
                mandatoryVal.enabled = true;
                ValidatorValidate(mandatoryVal);
            }
        }
        else {
            reqDateVal.enabled = (timeElement.val().length > 0) ? true : false;
            reqTimeVal.enabled = (dateElement.val().length > 0) ? true : false;
            ValidatorValidate(reqDateVal);
            ValidatorValidate(reqTimeVal);

            if (mandatoryVal !== undefined) {
                mandatoryVal.enabled = false;
                ValidatorValidate(mandatoryVal);
            }
        }
    });
}

function deleteRecord(entityid, recordid, viewid, attributeid) {
    if (confirm('Are you sure you wish to delete the selected record?')) {
        Spend_Management.svcCustomEntities.deleteCustomEntityRecord(entityid, recordid, viewid, attributeid, deleteRecordComplete, deleteRecordError);
    }
}

function deleteRecordComplete(data) {
    if (data != null) {
        switch (data[1]) {
            case -1:
                SEL.MasterPopup.ShowMasterPopup('Delete request denied due to audience access control.', 'Message from ' + moduleNameHTML);
                break;
            case -2:
                SEL.MasterPopup.ShowMasterPopup('Delete request denied due to insufficient access role permissions.', 'Message from ' + moduleNameHTML);
                break;
            case -3:
                SEL.MasterPopup.ShowMasterPopup('Delete request denied. Record deletion is not permitted on this GreenLight.', 'Message from ' + moduleNameHTML);
                break;
            case -10:
                SEL.MasterPopup.ShowMasterPopup('Delete request denied as record is referenced by another record.', 'Message from ' + moduleNameHTML);
                break;
            case -100:
                SEL.MasterPopup.ShowMasterPopup('Delete request denied as record is in use by another user.', 'Message from ' + moduleNameHTML);
                break;
            default:
                var gridid = data[0];
                SEL.Grid.deleteGridRow(gridid, data[1]);
                SEL.Grid.refreshGrid(gridid, SEL.Grid.getCurrentPageNum(gridid));
                if (data[2] != null && data[2] > 0) {
                    var str = 'if (tab' + data[2] + 'loaded != undefined) { tab' + data[2] + 'loaded = false; }';
                    eval(str);
                }
                break;
        }
    }
    return;
}

function deleteRecordError(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to delete the requested record.', 'Message from ' + moduleNameHTML);
    return;
}


function archiveRecord(entityid, recordid, viewid, archived, attributeid) {
          Spend_Management.svcCustomEntities.ArchiveCustomEntityRecord(entityid, recordid, viewid,archived, attributeid, archiveRecordComplete, archiveRecordError);   
}

function archiveRecordComplete(data) {
 
    if (data != null) {
        switch (data) {
            case -1:
                SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to archive the requested record.', 'Message from ' + moduleNameHTML);
                break;
            case -2:
                SEL.MasterPopup.ShowMasterPopup('Archive request denied due to insufficient access role permissions.', 'Message from ' + moduleNameHTML);
                break;
            case -3:
                SEL.MasterPopup.ShowMasterPopup('Archive request denied. Record deletion is not permitted on this GreenLight.', 'Message from ' + moduleNameHTML);
                break;         
           
            default:
                var gridid = data[0];
                SEL.Grid.refreshGrid(gridid, SEL.Grid.getCurrentPageNum(gridid));
                if (data[2] != null && data[2] > 0) {
                    var str = 'if (tab' + data[2] + 'loaded != undefined) { tab' + data[2] + 'loaded = false; }';
                    eval(str);
                }
                break;
        }
    }
    return;
}

function archiveRecordError(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred while attempting to archive the requested record.', 'Message from ' + moduleNameHTML);
    return;
}


function validateAndSubmit(validationGroup) {
    validateform(validationGroup);
    //validateform();
}

/*
function runWorkflow(workflowID, recordID)
{
    if (confirm("Are you sure you wish to perform this action?"))
    {
        Spend_Management.svcCustomEntities.runWorkflow(workflowID, recordID, runWorkflowComplete);
    }
}

function runWorkflowComplete(data)
{

    if (data == null)
    {

    } 
    else
    {
        switch (parseInt(data.NextStep.Action))
        {
            case 13:
                SEL.MasterPopup.ShowMasterPopup(data.NextStep.Message, "Response");
                break;  
            default:
                alert("Unhandled workflow step.");
                break;
        }
    }
}
*/

function loadOTMTable(data) {
    Spend_Management.svcCustomEntities.getOTMTable(data, onOTMComplete, onOTMError);
}

function onOTMComplete(results) {
    var divID = results[0];
    var data = results[1];

    if ($e(divID) === true) {
        $g(divID).innerHTML = results[3];
        SEL.Grid.updateGrid(results[2]);
        $(".ui-widget-overlay").css("height", Math.max($("body").height(), $(window).height()));
    }
}

function onOTMError(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to retrieve relationship table\n' + error, 'Message from ' + moduleNameHTML);
}

function EditRichTextEditor(tabColl, tabID, sourceTxt, sourceCntl, rtePanel, hideFont) {
    srcRTECntl = 'ctl00_contentmain_' + tabColl + '_' + tabID + '_' + sourceCntl;
    srcRTETxt = 'ctl00_contentmain_' + tabColl + '_' + tabID + '_' + sourceTxt;
    srcRTEPanel = 'ctl00_contentmain_' + tabColl + '_' + tabID + '_' + rtePanel;

    $f('editor')._editableDiv.innerHTML = decodeURIComponent($('#' + srcRTECntl).val());
    if (hideFont === "True") {
        $('.fontnameclass').css('display', 'none');
        $('.backcolorclass').css('display', 'none');
        $('.forecolorclass').css('display', 'none');
        $('.fontsizeclass').css('display', 'none');
    } else {
        $('.fontnameclass').css('display', '');
        $('.backcolorclass').css('display', '');
        $('.forecolorclass').css('display', '');
        $('.fontsizeclass').css('display', '');
    }
    $('.ajax__html_editor_extender_texteditor').unbind();
    $('.ajax__html_editor_extender_texteditor').on('paste', function (e) { pastePlainText(e); });
    $find(rteModal).show();
}

function SaveRichTextEditor() {
    var hiddenField = document.getElementById(srcRTECntl);
    var srcTxt = document.getElementById(srcRTETxt);
    var displayPanel = document.getElementById(srcRTEPanel);
    var htmlEditor = $get("editor_ExtenderContentEditable");
    var removeTags = $('.fontnameclass').css('display') === 'none';

    if (hiddenField !== null && displayPanel !== null && htmlEditor !== null) {
        var formatted_html = CleanWordHTML(htmlEditor.innerHTML, removeTags);

        hiddenField.value = encodeURIComponent(formatted_html);
        displayPanel.innerHTML = formatted_html;
        if (hiddenField.value === '') {
            srcTxt.value = '';
        }
        else {
            srcTxt.value = 'X';
        }
    }
    ClearFileListContents();
    $find(rteModal).hide();
}

function RemoveStyle(html, style) {
    style = "/" + style + "\\s*:.*?;?/";

    html = html.toString().replace(style, '');

    return html;
}

function RemoveSpecificTag(html, tag) {
    tag = "/<" + tag + "[^><]*>|<." + tag + "[^><]*>/";
    html = html.replace(tag, '');
    return html;
}



function cancelRTEdit() {
    srcRTECntl = null;
    srcRTETxt = null;
    ClearFileListContents();
    $find(rteModal).hide();
}

function setCETab() {
    var cntl = document.getElementById(hiddenCETabCntlID);
    var cntlID = document.getElementById(hiddenCETabIDCntlID);

    if (cntl !== null) {
        var tc = $find(tabContainer);
        cntl.value = tc.get_activeTabIndex();

        var tabPanel = $find(tabContainer).get_activeTab();
        var tabid = tabPanel._tab.id.replace(tabPanel._ownerID + '_', '').replace('_tab', '');

        if (cntlID !== null)
            cntlID.value = tabid.replace('tab', '');

        try {
            eval('loadTables_' + tabid + '();');
        }
        catch (e) { }
    }
}

function setActiveCETab() {
    var cntl = document.getElementById(hiddenCETabCntlID);
    if (cntl !== null) {
        var Index = parseInt(cntl.value);

        if (isNaN(Index) == true) {
            Index = 0;
        }

        var container = $find(tabContainer);
        if (container !== null)
            container.set_activeTabIndex(Index);
    }
}

//######################## approvals
function RejectReason(recordid, entityID) {
    var rejModal = $find(mdlRejectID);
    var str = 'RejectRecord(' + recordid + ', ' + entityID + ')';

    if (rejModal && recordid && entityID) {
        document.getElementById(txtReasonForRejectionID).value = '';

        var oRejectBtn = document.getElementById('btnRejectRecord');
        oRejectBtn.onclick = function () { RejectRecord(recordid, entityID) };

        rejModal.show();
    }
    return;
}

function RejectRecord(recordid, entityID) {
    var valStr = document.getElementById(txtReasonForRejectionID).value;

    if (validateform('rejectModal')) {
        Spend_Management.svcCustomEntities.Reject(recordid, entityID, valStr, RejectRecordComplete); //, WebMethodError);
    }
    return;
}

function RejectRecordComplete(data) {
    SEL.Grid.deleteGridRow(sGridID, data);
    CloseRejectModal();
    return;
}

function CloseRejectModal() {
    var rejModal = $find(mdlRejectID);
    var oRejectBtn = document.getElementById('btnRejectRecord');
    oRejectBtn.removeAttribute('onclick');
    if (rejModal) {
        rejModal.hide();
    }
    return;
}

// Summary grid load routines
function loadSummaryTable(entityid, attId, viewid, activeTab, formid, recordId, divId) {
    Spend_Management.svcCustomEntities.getSummaryTable(entityid, attId, viewid, activeTab, formid, recordId, divId, onSummaryLoadComplete, onSummaryLoadFail);
}

function onSummaryLoadComplete(results) {
    var divID = results[0];
    var data = results[1];

    if ($e(divID) === true) {
        $g(divID).innerHTML = results[3];
        SEL.Grid.updateGrid(results[2]);
    }
    return;
}

function onSummaryLoadFail(error) {
    SEL.MasterPopup.ShowMasterPopup('An error occurred attempting to retrieve a summary table\n' + error, 'Message from ' + moduleNameHTML);
}

function ClearFileListContents() {
    //clears previously uploaded images
    var container = document.getElementById('ctl00_contentmain_editor_ajaxFileUpload_QueueContainer');
    container.innerHTML = "";
}

var positionButtonsOnScroll = function () {
    if (($('.floatingFormButtons').parent().hasClass('floatingButtons')) && ($(window).scrollTop() + $(window).height() + $('.buttonInner').height() >= $(document).height())) {
        $('.floatingButtons').addClass('floatEnd');
        $('.floatingFormButtons').parent().removeClass('floatingButtons');
    } else if ($('.floatingFormButtons').parent().hasClass('floatEnd') && ($(window).scrollTop() + $(window).height() + $('.buttonInner').height() < $(document).height())) {
        var currentScroll = $(this).scrollTop();
        if (currentScroll < previousScrollForFloatingButtons) {
            $('.floatingButtons').removeClass('floatEnd');
            $('.floatingFormButtons').parent().addClass('floatingButtons');
        }
        previousScrollForFloatingButtons = currentScroll;
    }
}

function dynamicButtonsPlacement() {
    if ($(document).height() > ($(window).height() + $('.fixedFormButtons ').height())) {
        $('.floatingFormButtons').parent().addClass('floatingButtons fixedFormButtons');  
        var isie = detectIE();
        if ((isie && isie == 10)) {
        $('.main-content-area').addClass('main-content-area-padding');
        }
        if (!(isie && (isie == 7 || isie == 8 || isie == 9 || isie == 10))) {
            $('.main-content-area').addClass('main-content-area-padding');
            $(window).off("scroll", positionButtonsOnScroll);
            $(window).on("scroll", positionButtonsOnScroll);
        }
        //dynamically set the width of the button wrapper
        $('.floatingFormButtons').width($('.ajax__tab_header').width() - 7);
    } else {        
        $('.main-content-area').removeClass('main-content-area-padding');
        $('.floatingFormButtons').parent().removeClass('floatingButtons fixedFormButtons');
    }
}

/// The start of moving this to minify
(function (SEL, $f) {
    var scriptName = "CustomEntities";

    function execute() {
        SEL.registerNamespace("SEL.CustomEntities");

        SEL.CustomEntities = {

            Dom:
            {
                FormSelectionAttribute:
                {
                    ViewAdd:
                    {
                        Modal: null,
                        Panel: null,
                        PanelTitle: null,
                        PanelBody: null
                    }
                }
            },

            FormSelection:
            {
                Mappings:
                {
                    Add: null,
                    Edit: null
                },

                Attribute:
                {
                    ViewAdd: function (entity, view, tab, defaultForm) {
                        var panelId = SEL.CustomEntities.Dom.FormSelectionAttribute.ViewAdd.Panel;
                        $("#" + panelId).data("vars", { "entityId": entity, "viewId": view, "tabId": tab, "defaultForm": defaultForm });

                        $("#" + panelId + " input[type=text]").val("");
                        var listitem = $("#" + panelId + " select").eq(0);
                        if (typeof listitem !== "undefined" && listitem !== null) {
                            listitem.selectedIndex = 0;
                        }

                        $f(SEL.CustomEntities.Dom.FormSelectionAttribute.ViewAdd.Modal).show();
                    },

                    ViewAddSave: function () {
                        var panelId = SEL.CustomEntities.Dom.FormSelectionAttribute.ViewAdd.Panel,
                            v = $("#" + panelId).data("vars"),
                            formId = v.defaultForm,
                            textVal = null,
                            listVal = -1;

                        if ($("#" + panelId + " input[type=text]").filter(":visible").length > 0) {
                            textVal = $("#" + panelId + " input[type=text]").filter(":visible").val();
                        }
                        else {
                            listVal = $("#" + panelId + " select").val();
                        }

                        SEL.Data.Ajax({
                            serviceName: "svcCustomEntities",
                            methodName: "SaveFormSelectionAttributeValue",
                            data: { "entityId": v.entityId, "viewId": v.viewId, "textValue": textVal, "listValue": listVal },
                            success: function (response) {
                                if ("d" in response && response.d > 0) {
                                    formId = response.d;
                                }

                                window.location = "aeentity.aspx?viewid=" + v.viewId + "&entityid=" + v.entityId + "&formid=" + formId + "&tabid=" + v.tabId + "&id=0";

                                $f(SEL.CustomEntities.Dom.FormSelectionAttribute.ViewAdd.Modal).hide();
                            }
                        });
                    },

                    ViewAddCancel: function () {
                        $f(SEL.CustomEntities.Dom.FormSelectionAttribute.ViewAdd.Modal).hide();
                    },

                    ViewEdit: function (entity, view, tab, defaultForm, id) {
                        var formId = defaultForm,
                            mappings = SEL.CustomEntities.FormSelection.Mappings.Edit,
                            entityId = parseInt(entity, 10),
                            viewId = parseInt(view, 10);

                        if (typeof mappings !== "undefined" && mappings !== null && mappings.length > 0 && !isNaN(entityId) && !isNaN(viewId)) {
                            SEL.Data.Ajax({
                                serviceName: "svcCustomEntities",
                                methodName: "GetFormSelectionAttributeMappedEditFormId",
                                data: { entityId: entityId, viewId: viewId, id: id },
                                success: function (response) {
                                    if ("d" in response && response.d > 0) {
                                        formId = response.d;
                                    }

                                    window.location = "aeentity.aspx?viewid=" + view + "&entityid=" + entity + "&formid=" + formId + "&tabid=" + tab + "&id=" + id;
                                }
                            });
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
}(SEL, $f));