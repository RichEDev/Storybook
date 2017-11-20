function exportReport (financialexportid, exporthistoryid)
{
    window.open("../../shared/reports/exportreport.aspx?financialexportid=" + financialexportid + "&exporthistoryid=" + exporthistoryid,'export','width=300,height=150,status=no,menubar=no');
}

//This will be called from the exportreport dialog to remove the report request
function cancelReportRequest(requestNumber) 
{
    ReportFunctions.cancelReportRequest(requestNumber, onSuccess, onError); //.cancelReportRequest(runRequestNumber);

    return;
}

function onSuccess(result)   //, userContext, methodName
{
    return;
}

function onError(exception)
{
    if (exception.get_timedOut()) {
        alert(exception);
    }
    else
    {
        //Exception occurred   
    }
}

function RerunESRInbound(linkObj, accountID, exportHistoryID, exportStatus, financialExportID)
{
    Spend_Management.svcAutomatedTransfers.UpdateTransferStatus(accountID, exportHistoryID, exportStatus, function () { PopulateGrid(financialExportID, 2); }, errorMessage);
    linkObj.innerText = "Completed";
    linkObj.href = "";
    return;
}

function PopulateGrid(financialExportID, appType)
{
    Spend_Management.svcFinancialExports.CreateExportHistoryGrid(financialExportID, appType, PopulateGridComplete, errorMessage);
    return;
}

function PopulateGridComplete(data)
{
    if ($e(dynGrid) === true)
    {
        $g(dynGrid).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
}
function errorMessage(data)
{
    if (data["_message"] != null)
    {
        SEL.MasterPopup.ShowMasterPopup(data["_message"], "Web Service Message");
    }
    else
    {
        SEL.MasterPopup.ShowMasterPopup(data, "Web Service Message");
    }
    return;
}
