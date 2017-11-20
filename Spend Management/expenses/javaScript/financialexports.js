var exportid = 0;
var financialExportID = 0;

function runExport()
{
    $find(modESRAssignmentCheck).hide();
    window.open("../../shared/reports/exportreport.aspx?financialexportid=" + exportid, 'export', 'width=300,height=150,status=no,menubar=no');
    return;
}

function checkExport(info)
{
    if (info == "")
    {
        runExport();
    }
    else
    {
        var infoDiv = document.getElementById("ESRInfoDiv");

        if (infoDiv !== null)
        {
            infoDiv.innerHTML = info;
            $find(modESRAssignmentCheck).show();
        }
    }
    return;
}

function exportReport(financialexportid, expeditePaymentReport)
{
    if (expeditePaymentReport != null) {
        if (expeditePaymentReport == 'True') {
            SEL.MasterPopup.ShowMasterPopup("This Expedite Payment Report can be exported through Expedite Payment Service.");
            $(this).css('cursor', 'default');
            $(this).css('text-decoration', 'none')
            preventClick = true;
            return;
        }
        if (confirm('Are you sure you wish to produce a new financial data export?')) {
            exportid = financialexportid;
            Spend_Management.svcFinancialExports.CheckExpenseESRAssignments(exportid, checkExport, onError);
    }
        return;
}

}

function checkTrustDetailsExist(financialexportid, applicationtype, trustID)
{
    exportid = financialexportid;
    trustID = parseInt(trustID);

    if (applicationtype == 2)
    {
        Spend_Management.svcFinancialExports.CheckESRTrustExists(trustID, checkTrustDetailsExistComplete);
    }
    else
    {
        window.location.href = '../../shared/reports/aeschedule.aspx?returnto=3&financialexportid=' + financialexportid;
    }
}
function checkTrustDetailsExistComplete(data)
{
    if (data == true)
    {
        window.location.href = '../../shared/reports/aeschedule.aspx?returnto=3&financialexportid=' + exportid;
    }
    else
    {
        alert('You do not have your ESR trust details set up to export this file, please create these first.');
    }
}


//This will be called from the exportreport dialog to remove the report request
function cancelReportRequest(requestNumber) 
{
    ReportFunctions.cancelReportRequest(requestNumber, onSuccess, onError); //.cancelReportRequest(runRequestNumber);

    return;
}

function onSuccess(result)   //, userContext, methodName
{
    var balh = result;
}

function onError(exception) 
{
    if (exception.get_timedOut()) {
        alert(exception);
    }
    else {
        //Exception occurred   
    }
}



function AddFinancialExport()
{
    EditFinancialExport(0);
    return;
}

function EditFinancialExport(feID)
{
    financialExportID = feID;
    ResetFinancialExportModal();
    Spend_Management.svcFinancialExports.GetFinancialExport(feID, GetFinancialExportComplete, errorMessage);
    return;
}
function GetFinancialExportComplete(data)
{
    if (data != null)
    {
        $('#' + ddlApplicationID).val(data.application).prop('selected', ' true');
        $('#' + ddlNHSTrustID).val(data.NHSTrustID).prop('selected', ' true');
        $('#' + ddlReportID).val(data.reportid).prop('selected', ' true');
        $('#' + ddlExportTypeID).val(data.exporttype).prop('selected', ' true');
        $('#' + chkPreventNegativePayments).prop('checked', data.PreventNegativeAmountPayable);
        $('#' + chkExpeditePayment).prop('checked', data.ExpeditePaymentReport);

        showTrust();
    }
    OpenFinancialExportModal();
    return;
}

// SAVE
function SaveFinancialExport()
{
    var applicationTypeID = $('#' + ddlApplicationID + ' option:selected').val();
    var trustID = $('#' + ddlNHSTrustID + ' option:selected').val();
    if (trustID === undefined) {
        trustID = 0;
    }
    var reportGUID = $('#' + ddlReportID + ' option:selected').val();
    var exportTypeID = $('#' + ddlExportTypeID + ' option:selected').val();
    var preventNegativePayment = $('#' + chkPreventNegativePayments + ':checked').val() == 'on';
    var expeditePaymentReport = $('#' + chkExpeditePayment + ':checked').val() == 'on';
    Spend_Management.svcFinancialExports.SaveFinancialExport(financialExportID, applicationTypeID, trustID, exportTypeID, reportGUID, preventNegativePayment, expeditePaymentReport,SaveFinancialExportComplete, errorMessage);
    return;
}
function SaveFinancialExportComplete(data)
{
    CloseFinancialExportModal();
    return;
}

// CANCEL
function CancelFinancialExport()
{
    $find(financialExportModalID).hide();
    return;
}

function OpenFinancialExportModal()
{
    $find(financialExportModalID).show();
    return;
}
function CloseFinancialExportModal()
{
    PopulateGrid();
    $find(financialExportModalID).hide();
    return;
}
function ResetFinancialExportModal()
{
    document.getElementById(ddlApplicationID).selectedIndex = 0;
    document.getElementById(ddlNHSTrustID).selectedIndex = 0;
    document.getElementById(ddlReportID).selectedIndex = 0;
    document.getElementById(ddlExportTypeID).selectedIndex = 0;
    document.getElementById(chkPreventNegativePayments).checked = false;
    if (document.getElementById(chkExpeditePayment) != null) {
        document.getElementById(chkExpeditePayment).checked = false;
    }
    showTrust();
    return;
}
function PopulateGrid() {    
    PageMethods.CreateFinancialExportsGrid('', PopulateGridComplete);
    return;
}

function PopulateGridComplete(data) {
    if ($e(pnlFinancialExportGrid) === true) {
        $g(pnlFinancialExportGrid).innerHTML = data[1];
        SEL.Grid.updateGrid(data[0]);
    }
}

function DeleteFinancialExport(feID)
{
    if (confirm('Are you sure you wish to delete the selected financial export and its history?'))
    {
        Spend_Management.svcFinancialExports.DeleteFinancialExport(feID, DeleteFinancialExportComplete, errorMessage);
    }
    return;
}
function DeleteFinancialExportComplete(data)
{
    PopulateGrid(data);
    return;
}



function showTrust()
{
    var appVal = document.getElementById(ddlApplicationID).options[document.getElementById(ddlApplicationID).selectedIndex].value;

    if (appVal == 2)
    {
        document.getElementById(trustDivID).style.display = "";
        document.getElementById(exportTypeDivID).style.display = "none";
    }
    else
    {
        document.getElementById(trustDivID).style.display = "none";
        document.getElementById(exportTypeDivID).style.display = "";
    }
    return;
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
