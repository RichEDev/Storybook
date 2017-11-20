<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_exportreport" Codebehind="exportreport.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Export Report</title>
    <link rel="stylesheet" type="text/css" media="screen" href="~/shared/css/layout.css" />
    <link rel="stylesheet" type="text/css" media="screen" href="~/shared/css/styles.aspx" />
    <script type="text/javascript">
    
    var iPolling;

    function exportReportComplete(data) {
        if (data == false) {
            document.getElementById("tblReportStatus").style.display = "none";
            document.getElementById("exportReportDetails").innerHTML = "You are currently exporting this report in another window.";
        }
        else {
            GetReportStatus();
        }      
    }
    
    </script>
    
    
    <script type="text/javascript" language="javascript">

var runReportID;
var runReportName;
var errorString = "An error has occured processing this report";
var reportReady = false;
var removeRequest = true;

function GetReportStatus() {
    ReportFunctions.getReportProgress(runRequestNumber, updateReportStatus, errorHandling);
}

function updateReportStatus(data) {
    if(data == null) {
        setProgressBar(100);
        setStatus("Complete");    
    } else {
        var status = data[0];
        var reportProgress = data[1];
        document.title = data[2];
        if (status == "BeingProcessed") {
            var doneWith = reportProgress;
            setProgressBar(doneWith);
            setStatus("Running");
            if (GetReportStatus != undefined) {
                setTimeout(GetReportStatus, 500);
            }
        } else if (status == "Complete") {
            setProgressBar(100);
            reportReady = true;
            setStatus("Complete");

            removeRequest = false;
            document.getElementById("reportProgress").style.display = "none";
            document.getElementById("reportStatus").style.display = "none";
            document.getElementById("reportPercentDone").style.display = "none";
            document.getElementById("litDownload").style.display = "";
            document.getElementById("reportReady").style.display = "";
        } else if (status == "Failed") {
            if (data[4] == 'Excel' && parseInt(data[3]) > 65536) {
                setStatus("This report cannot be exported as it contains more than 65536 rows of data");
                setProgressBar(100);
            }
            else {
                setStatus(errorString);
            }
        } else {
            setStatus("Queued");
            setTimeout(GetReportStatus, 500);
        }
    }
}

function setStatus(status) {
    document.getElementById("reportStatus").innerHTML = status;
    return;
}

function setProgressBar(doneWidth) {
    var currentWidth = parseInt(document.getElementById("reportPercentDone").innerHTML.replace('%', ''));
    if (currentWidth === NaN) {
        currentWidth = 0;
    }
    
    if (doneWidth > currentWidth && doneWidth <= 100) {
        document.getElementById("reportPercentDone").innerHTML = doneWidth + "%";
        var divWidth = doneWidth * 2.5;
        document.getElementById("reportDone").style.width = divWidth + "px";
    }
        
    return;
}

function CancelReportRequest()
{
   //ReportFunctions.cancelReportRequest(runRequestNumber, null, errorHandling);
   window.close();
   return;

}

function OnUnloadFunction() 
{
    if (removeRequest) 
    {
        window.opener.cancelReportRequest(runRequestNumber);
    }
    return;
}

function errorHandling(data)
{
    setStatus(errorString);
}
</script>

</head>
<body onload="window_onload();">
    <form id="form1" runat="server">
    
    <script type="text/javascript" language="javascript">
    var runRequestNumber = '<% = requestnum %>';
    </script>
        <cc1:ToolkitScriptManager ID="scriptman" runat="server">
            <Services>
                <asp:ServiceReference Path="ReportFunctions.asmx" />
            </Services>
        </cc1:ToolkitScriptManager>
        
        <div id="exportReportDetails">
        <table height="100%" width="100%" id="tblReportStatus">
            <tr><td valign="middle" align="center"><div id="reportStatus">Processing Request</div>
                <div id="reportProgress" style="width: 250px; background-image: url('../images/exportReportBackground250px.png'); height: 20px;">
                    <div id="reportDone" style="background-image: url('../images/exportReport250px.png'); height: 20px; text-align: left; float: left;">&nbsp;</div>
                </div>
                <div id="reportPercentDone">0%</div>
</td></tr>
<tr><td><div>
            <div>
                <div id="reportReady" style="display: none">
                    <p></p>
                     <h3><img src="/static/icons/24/plain/document_pulse.png" alt="" style="vertical-align: middle"/>&nbsp;Report Generation Complete</h3>
                    <p></p>
                    <p></p>
                </div>
                <label runat="server" id="litDownload" style="display: none">&nbsp;<helpers:CSSButton runat="server" ID="btnDownload" Text="Download Report"></helpers:CSSButton> </label>
                <helpers:CSSButton runat="server" ID="btnCancel" alt="Cancel Report" style="cursor: pointer" OnClientClick="CancelReportRequest()" Text="Cancel" UseSubmitBehavior="False"></helpers:CSSButton>
            </div>
        </div></td></tr>
<tr><td align="center">&nbsp;&nbsp;<asp:Literal runat="server" ID="litPivotMessage"></asp:Literal><p></p></td></tr>
        </table>
        </div>
    </form>
</body>
</html>
