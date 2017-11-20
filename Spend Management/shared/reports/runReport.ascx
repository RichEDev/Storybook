<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="runReport.ascx.cs" Inherits="Spend_Management.runReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:ScriptManagerProxy ID="smpReportExport" runat="server">
<Services>
<asp:ServiceReference Path="ReportFunctions.asmx" />
</Services>
</asp:ScriptManagerProxy>

<asp:Panel ID="pnlRunReport" runat="server" style="padding: 5px; border: solid 1px #000000">
<div id="reportStatus">Processing Request</div>
<div id="reportProgress" style="width: 100px; background-color: Aqua;">
    <div style="background-color: Black;">&nbsp;</div>
</div>
<p><div><img src="somewhere" height="30" width="100" onclick="CancelReportRequest()" alt="Cancel Report" /></div></p>
</asp:Panel>

<cc1:ModalPopupExtender ID="mpeReport" runat="server" TargetControlID="pnlRunReport" PopupControlID="lnkPopupModal" BackgroundCssClass="modalBackground" OnCancelScript="CancelReportRequest" OnOkScript="OkScript">
</cc1:ModalPopupExtender>

<script type="text/javascript" language="javascript">

var runReportID;
var runReportName;
var runRequestNumber;

function PopupExportModalShow()
{
    $find('<% = mpeReport.ClientID %>').show();
}

function PopupExportModalHide()
{
    $find('<% = mpeReport.ClientID %>').hide();
}

function GetReportStatus()
{
       expenses.reports.ReportFunctions.getReportProgress(runRequestNumber, updateReportStatus, errorHandling)
}




function StartExportReport(nReportID, sReportRequestNumber)
{
    expenses.reports.ReportFunctions.exportReport(//(int requestnum, byte exporttypeid, int reportid, int financialexportid, int exporthistoryid
    runReportID = nReportID;
    runRequestNumber = sReportRequestNumber;
    GetReportStatus();
    PopupExportModalShow();
}

function updateReportStatus(data) 
{
    var status = data[0];
    var reportProgress = data[1];
    
    if(status == "BeingProcessed") 
    {
        var doneWith = reportProgress;
        setProgressBar(doneWith);
        setStatus("Running");
        
        setTimeout(GetReportStatus(), 1000);
    } else if(status == "Complete") {
        setProgressBar(100);
        setStatus("Complete");
       
    } else if(status == "Failed") {
        setProgressBar(100);
        setStatus("Failed");
    } else {
        setStatus("Queued");
    }
}

function setStatus(status) {
    document.getElementById("reportStatus").innerHTML = status;
    return;
}

function setProgressBar(doneWidth) {
        document.getElementById("reportDone").style.width = doneWith + "px";
        return;
}

function CancelReportRequest()
{
    expenses.reports.ReportFunctions.cancelReportRequest(runRequestNumber, null, errorHandling);
    PopupExportModalHide();
    return;
}

function errorHandling(data)
{
    alert(data);
}
</script>

