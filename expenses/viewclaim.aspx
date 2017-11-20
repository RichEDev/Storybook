<%@ Page language="c#" Inherits="expenses.viewclaim" Codebehind="viewclaim.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>View Claim</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		
	</HEAD>
	<body>
		<form method="post" runat="server">
        <cc1:ToolkitScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/shared/reports/ReportFunctions.asmx" />
            </Services>
        </cc1:ToolkitScriptManager>
		<script type="text/javascript">
		    var runRequestNumber = '<% = requestnum %>';
		    var runReportID;
		    var runReportName;
		    var removeRequest = true;
		    
		    function GetReportStatus() {
		        ReportFunctions.getReportProgress(runRequestNumber, updateReportStatus, errorHandling)
		    }

		    function updateReportStatus(data) {
		        if (data == null) {
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
		                setTimeout(GetReportStatus, 500);
		            } else if (status == "Complete") {
		                setProgressBar(100);
		                setStatus("Complete");
		                removeRequest = false;
		                __doPostBack('<% = upnlReport.UniqueID %>', runRequestNumber);
		            } else if (status == "Failed") {
		                //setProgressBar(100);
		                var errorString = "An error has occured processing this report"
		                setStatus(errorString);

		            } else {
		                setStatus("Queued");
		                setTimeout(GetReportStatus, 500);
		            }
		        }
		    }

		    function setStatus(status) {
		        var statusDiv = document.getElementById("reportStatus");

		        if (statusDiv != undefined) {
		            statusDiv.innerHTML = status
		        }
		        return;
		    }

		    function setProgressBar(doneWidth) {
		        var reportPercentDiv = document.getElementById("reportPercentDone");
		        if (reportPercentDiv != undefined) {
		            reportPercentDiv.innerHTML = doneWidth + "%";
		            var divWidth = doneWidth * 2.5;
		            document.getElementById("reportDone").style.width = divWidth + "px";
		        }
		        return;
		    }
		    function errorHandling(data) {

		    }

		    GetReportStatus();
		</script>
			<div class="inputpanel">
				<asp:Literal id="litstyles" runat="server" meta:resourcekey="litstylesResource1"></asp:Literal>
				<table>
					<tr>
						<td class="labeltd"><asp:label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Employee Name:</asp:label>
						</td>
						<td class="inputtd"><asp:label id="lblemployee" runat="server" meta:resourcekey="lblemployeeResource1">Label</asp:label></td>
					</tr>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lblclaimnum" runat="server" Text="Claim No:" meta:resourcekey="lblclaimnumResource1"></asp:Label></td>
						<td class="inputtd"><asp:label id="lblclaimno" runat="server" meta:resourcekey="lblclaimnoResource1">Label</asp:label></td>
						<td class="labeltd">
                            <asp:Label ID="lbldatepaidlbl" runat="server" Text="Date Approved:" meta:resourcekey="lbldatepaidlblResource1"></asp:Label></td>
						<td class="inputtd"><asp:label id="lbldatepaid" runat="server" meta:resourcekey="lbldatepaidResource1"></asp:label></td>
					</tr>
					<tr>
						<td class="labeltd">
                            <asp:Label ID="lbldescriptionlbl" runat="server" Text="Description:" meta:resourcekey="lbldescriptionlblResource1"></asp:Label></td>
						<td colspan="3" class="inputtd"><asp:label id="lbldescription" runat="server" meta:resourcekey="lbldescriptionResource1">Label</asp:label></td>
					</tr>
				</table>
			</div>
		<asp:UpdatePanel ID="upnlReport" runat="server">
		<ContentTemplate>
                 <igtbl:UltraWebGrid ID="gridclaim" runat="server" OnInitializeLayout="gridclaim_InitializeLayout" OnInitializeRow="gridclaim_InitializeRow" SkinID="gridskin" meta:resourcekey="gridclaimResource1">
                    <DisplayLayout ColFootersVisibleDefault="Yes">

                        <ActivationObject BorderColor="" BorderWidth="">
                        </ActivationObject>
                        
                    </DisplayLayout>
                    <Bands>
                        <igtbl:UltraGridBand>
                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                        </igtbl:UltraGridBand>
                    </Bands>
                </igtbl:UltraWebGrid>
				
				<div id="reportStatusInformation" runat="server">
         <div id="reportStatus">Processing Request</div>
         <div id="reportProgress" style="width: 250px; background-image: url('/images/exportReportBackground250px.png'); height: 20px;">
                <div id="reportDone" style="background-image: url('/images/exportReport250px.png'); height: 20px;">&nbsp;</div>
        </div>
        <div id="reportPercentDone">0%</div>
    </div>
    </ContentTemplate>
     </asp:UpdatePanel>
		</form>
	</body>
</html>
