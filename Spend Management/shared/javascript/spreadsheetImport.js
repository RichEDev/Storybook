function getImportProgress() 
{
    Spend_Management.importSvc.getImportProgress(updateImportStatus, commandFail)
}

function setImportRowStatus(status, rowNum)
{
    var tbl = document.getElementById(tableID).tBodies[0];

    if (tbl != undefined) 
    {
        if (tbl.rows[rowNum + 1] != null)
        {
            tbl.rows[rowNum + 1].cells[1].innerHTML = status;
        }
    }
    return;
}

function setProgressBar(doneWidth) 
{
    var importPercentDiv = document.getElementById("importPercentDone");
    if (importPercentDiv != undefined) 
    {
        importPercentDiv.innerHTML = doneWidth + "%";
        var divWidth = doneWidth * 2.5;
        document.getElementById("importDone").style.width = divWidth + "px";
    }
    return;
}

function getImportProgressTimed() {

    setTimeout(getImportProgress, 250);
}

function updateImportStatus(data) 
{
    if (data == null) 
    {
        getImportProgressTimed();
//        setProgressBar(100);
//        setStatus("Import Complete");
    } 
    else 
    {

        if (data.length > 0) 
        {
            var currentSheet = data[0][0];
            var currentSheetName = data[0][1];
            var currentRow;
            var importProgress;
            var importStatus = data[0][2];
            var worksheetStatus;
            var calcProgress = false;

            if (importStatus == "Valid" || importStatus == "Importing") {
                showProgressBar();
                calcProgress = true;
            }
            
            //Minus one to match the array index for the sheet
            currentSheet--;
            
            for (var i = 0; i < data.length; i++) {
                currentRow = data[i][3];
                worksheetStatus = data[i][7];

                //Only calculate progress of the currently processing sheet
                if (calcProgress == true) 
                {
                    if (currentSheet == i) 
                    {
                        var processedRows = data[i][5];
                        setProgressBar(processedRows);
                    }
                }

                switch (worksheetStatus) 
                {
                    case "Validating":
                        setImportRowStatus("Validating", i);
                        break;

                    case "Valid":
                        setImportRowStatus("<span style=\"color: green; \">Valid</span>", i);
                        break;

                    case "Invalid":
                        setImportRowStatus("<span style=\"color: red; \">Invalid</span>", i);
                        break;

                    case "Importing":
                        setImportRowStatus("Importing", i);

                        break;

                    case "Imported":
                        setImportRowStatus("<span style=\"color: green; \">Imported</span>", i);
                        break;

                    case "Failed":
                        setImportRowStatus("<span style=\"color: red; \">Failed</span>", i);
                        break

                    default:
                        break;
                }
            }

            switch (importStatus) {
                case "Validating":
                    getImportProgressTimed();
                    showStatusMessage();
                    setStatus("Validating " + currentSheetName);
                    hideUploadDiv();
                    showLogLink();
                    break;

                case "Valid":
                    setStatus("Importing Data");
                    showProgressBar();
                    getImportProgressTimed();
                    break;

                case "Invalid":
                    setStatus("The spreadsheet has some validation errors, please view the log.");
                    showUploadDiv();
                    break;

                case "Importing":
                    getImportProgressTimed();
                    showStatusMessage();
                    setStatus("Importing " + currentSheetName);
                    hideUploadDiv();
                    break;

                case "Complete":
                    setProgressBar(100);
                    setStatus("Import Complete");
                    showUploadDiv();
                    Spend_Management.importSvc.ClearImportFromSession(null, commandFail);
                    break;

                case "Failed":
                    setStatus("Import Failed");
                    showUploadDiv();
                    break;

                default:
                    break;
            }
        }
    }
}

function showLogLink() 
{
    var logLink = document.getElementById("logLink");
    logLink.style.display = "block";
    return;
}

function showProgressBar() 
{
    var progressDiv = document.getElementById("importProgress");

    if (progressDiv != undefined) 
    {
        progressDiv.style.display = "block";
        logLink.style.display = "block";
    }
    return;
}

function showStatusMessage() 
{
    var statusDiv = document.getElementById(statusID);

    if (statusDiv != undefined) 
    {
        statusDiv.style.display = "block";
    }
    return;
}

function showUploadDiv() 
{
    var uploadDiv = document.getElementById(uploadDivID);

    if (uploadDiv != undefined) 
    {
        uploadDiv.style.display = "block";
    }
    return;
}

function hideUploadDiv() 
{
    var uploadDiv = document.getElementById(uploadDivID);

    if (uploadDiv != undefined) {
        uploadDiv.style.display = "none";
    }
    return;
}

function setStatus(status) 
{
    var statusDiv = document.getElementById(statusID);

    if (statusDiv != undefined) 
    {
        statusDiv.innerHTML = status
    }
    return;
}

function commandFail(error)
{
    if (error["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(error["_message"], "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(error, "Web Service Message");
    }
    return;
}

function showLogModal()
{
    var viewType = parseInt(document.getElementById(ddlLogViewType).value);
    var elementType = parseInt(document.getElementById(ddlLogElementType).value);
    GetElementDropDown();
    $find(modalID).show();
    Spend_Management.svcLogging.getLogData(logid, viewType, elementType, outputLogData, commandFail)
}

function outputLogData(data) 
{
    var logDiv = document.getElementById("logDiv");

    if (logDiv != undefined) 
    {
        logDiv.innerHTML = data;
    }
}

function hideLogModal() 
{
    document.getElementById(modalID).hide();
}

function viewLog()
{
    var viewType = document.getElementById(ddlLogViewType).value;
    var elementType = document.getElementById(ddlLogElementType).value;
    var url = '/shared/admin/logviewer.aspx?logid=' + logid + '&viewtype=' + viewType + '&elementtype=' + elementType;
    window.open(url)
}

function ChangeLogViewType()
{
    var viewType = parseInt(document.getElementById(ddlLogViewType).value);
    var elementType = parseInt(document.getElementById(ddlLogElementType).value);
    Spend_Management.svcLogging.getLogData(logid, viewType, elementType, outputLogData, commandFail);
    return;
}


function GetElementDropDown()
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

    for (var i = 0; i < data.length; i++)
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
