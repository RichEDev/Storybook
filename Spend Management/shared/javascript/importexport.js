var loggingID, fieldsDropDownID;
function showLogModal(logid, importType) 
{
    var logDiv = document.getElementById("logDiv");

    if (logDiv !== undefined) {
        logDiv.innerHTML = '';
    }
    
    loggingID = logid;
    var viewType = ChangeLogViewType();
    GetElementDropDown(logid);
    var viewTypeList = document.getElementById(ddlLogViewType);
    if (importType === 2) // esr version 2 - Disable success add and success update as we do not know which it was.
    {
        viewTypeList[2].disabled = true;
        viewTypeList[3].disabled = true;
    } else
    {
        viewTypeList[2].disabled = false;
        viewTypeList[3].disabled = false;
    }
    
    
    return;
}

function ChangeLogViewType(logid)
{
    var viewType = parseInt(document.getElementById(ddlLogViewType).value, 10);
    var elementType = parseInt(document.getElementById(ddlLogElementType).value, 10);
    Spend_Management.svcLogging.getLogData(loggingID, viewType, elementType, outputLogData, commandFail);
    $find(modalID).show();
    return;
}

function outputLogData(data) {
    var logDiv = document.getElementById("logDiv");

    if (logDiv !== undefined) {
        logDiv.innerHTML = data;
    }
    return;
}

function hideLogModal() {
    document.getElementById(modalID).hide();
}

function viewLog()
{
    var viewType = document.getElementById(ddlLogViewType).value;
    var elementType = document.getElementById(ddlLogElementType).value;
    var url = appPath + '/shared/admin/logviewer.aspx?logid=' + loggingID + '&viewtype=' + viewType + '&elementtype=' + elementType;
    window.open(url);
}

function deleteImportHistory(historyid) {
    currentRowID = historyid;
    if (confirm('Are you sure you wish to delete the selected previous import history?')) 
    {
        Spend_Management.svcLogging.deleteImportHistory(historyid, deleteSuccess, commandFail);
    }
    return;
}

function deleteSuccess(rowID) 
{
    var historygrid = document.getElementById('historygrid');
    if (historygrid !== null)
    {
        SEL.Grid.deleteGridRow('historygrid', currentRowID);
    }
    return;
}

function refreshHistoryGrid() {
    var lst = document.getElementById(filter_ddlID);
    if (lst !== null) {
        var selectedValue = lst.options[lst.selectedIndex].value;

        Spend_Management.svcLogging.getHistoryGrid(selectedValue, refreshSuccess, commandFail);
    }
    return;
}

function refreshSuccess(gridHTML) {
    if ($e('historygridholder') === true)
    {
        $g('historygridholder').innerHTML = gridHTML[2];
        SEL.Grid.updateGrid(gridHTML[1]);
    }
}

function exportOutboundFile()
{
    alert("Please Implement Me!");
}

function rerunImport(datatID, applicationType) 
{
    if (applicationType === 2)
    {
        Spend_Management.svcLogging.ResubmitFile(datatID, "filename", resubmitComplete);
    } else
    {
        Spend_Management.svcLogging.rerunImport(datatID);
    }
    
}

function resubmitComplete(message)
{
    SEL.MasterPopup.ShowMasterPopup(message, "Web Service Message");
}

function commandFail(error)
{
    if (error._message !== null)
    {
        SEL.MasterPopup.ShowMasterPopup(error._message, "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}

function GetElementDropDown(logid)
{
    Spend_Management.svcLogging.GetLogElementOptions(logid, UpdateElementDropDown, commandFail);
    return;
}

function UpdateElementDropDown(data)
{
    var elementType = document.getElementById(ddlLogElementType);

    for (var i = elementType.length - 1; i > 0; i--)
    {
        elementType.remove(i);
    }
    
    for (i = 0; i < data.length; i++)
    {
        var tmpOption = document.createElement('option');
        tmpOption.text = data[i].Text;
        tmpOption.value = data[i].Value;
        try
        {
            elementType.add(tmpOption, null);
        }
        catch (ex)
        {
            elementType.add(tmpOption); // IE
        }
    }
    return;
}

/*
** Import Template
*/
function DeleteImportTemplate(templateID)
{
    if (confirm('Are you sure you wish to delete the selected Import Template?'))
    {
        Spend_Management.svcImportTemplates.DeleteImportTemplate(templateID, DeleteImportTemplateComplete, errorMessage);
    }
    return;
}
function DeleteImportTemplateComplete(data)
{
    if (data > 0)
    {
        var tbl = document.getElementById('tbl_importTemplates');
        for (var i = 1; i < tbl.rows.length; i++)
        {
            if (tbl.rows[i].id === "tbl_importTemplates_" + data)
            {
                tbl.deleteRow(i);
                break;
            }
        }
    }
    return;
}


function LoadFields(object, dataType)
{
    if (object === undefined || object === null)
    {
        SEL.MasterPopup.ShowMasterPopup("Invalid Object Reference", "Page Error");
        return;
    }

    var fieldsDropDown = object.parentNode.parentNode.getElementsByTagName('td')[2].getElementsByTagName('select')[0];
}

/// <Summary>
    /// Iterate through array of mappings checking that 'NewFieldGuid' has not already been mapped, if itar has been used, set result to false
    /// </Summary>

    function CheckForDuplicateFields(mapsGroup, newFieldGuid)
    {
        var result = true;
        for (var i = 0; i < mapsGroup.length; i++)
        {
            if (mapsGroup[i][3] === newFieldGuid)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    function SaveImportTemplateMappingsComplete(data)
    {
        if (data <= 0)
        {
            SEL.MasterPopup.ShowMasterPopup("There was an error adding your template mapping.", "Save Error");
        } else
        {
            window.location = appPath + "/shared/ImportsExports/importTemplates.aspx";
        }
        return;
    }

    function CancelImportTemplateMapping()
    {
        window.location = appPath + "/shared/ImportsExports/importTemplates.aspx";
        return;
    }


    function errorMessage(data)
    {
        if (data._message !== undefined)
        {
            SEL.MasterPopup.ShowMasterPopup(data._message, "Web Service Error");
        } else
        {
            SEL.MasterPopup.ShowMasterPopup(data, "Web Service Error");
        }
        return;
    }
