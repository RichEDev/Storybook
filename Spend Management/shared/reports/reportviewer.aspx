<%@ Page Language="C#" AutoEventWireup="true" Inherits="reports_reportviewer" MasterPageFile="~/masters/smForm.master" Codebehind="reportviewer.aspx.cs" %>

<%@ Register TagPrefix="igmisc" Namespace="Infragistics.WebUI.Misc" Assembly="Infragistics4.WebUI.Misc.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register TagPrefix="igcalc" Namespace="Infragistics.WebUI.UltraWebCalcManager" Assembly="Infragistics4.WebUI.UltraWebCalcManager.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register TagPrefix="igtbar" Namespace="Infragistics.WebUI.UltraWebToolbar" Assembly="Infragistics4.WebUI.UltraWebToolbar.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>

    <asp:Content ID="Content3" runat="server" ContentPlaceHolderID="contentmain">
         <asp:ScriptManagerProxy id="smp" runat="server">
            <Services>
                <asp:ServiceReference path="ReportFunctions.asmx" />
            </Services>
         </asp:ScriptManagerProxy>
         
        <asp:Literal ID="litstyles" runat="server" meta:resourcekey="litstylesResource1"></asp:Literal>
        <script language="javascript" type="text/javascript">
        function exportReport(exporttype, reportid) 
        {
            window.open("exportreport.aspx?exporttype=" + exporttype + "&reportid=" + reportid + "&requestnum=" + requestNum, 'export', 'width=300,height=150,status=no,menubar=no');
        }

        function changeColumns(oldStyle) 
        {
            window.open('aeReport.aspx?requestnum=' + requestNum, 'columns', 'toolbars=yes, scrollbars=yes, resizable=yes');
        }

        function exportOptions() 
        {
            window.open('exportoptions.aspx?reportid=' + reportid + "&requestnum=" + requestNum,'exportoptions','width=700,height=400,scrollbars=1');
        }
        function refreshReport()
        {
            document.location = document.location + "&requestnum=" + requestNum;
        }
        function printerFriendly() 
        {
            window.open('printerfriendly.aspx?requestnum=' + requestNum);
        }
        function viewclaim(requestnum, claimid, expenseid) {
            ReportFunctions.ViewClaim(requestnum, claimid, expenseid, ViewClaimComplete, onError);
        }
        
        function ViewClaimComplete(viewClaim) {
            $('#lblemployee').text(viewClaim.EmployeeName);
            $('#lblclaimno').text(viewClaim.ClaimNumber);
            $('#lbldatepaid').text(viewClaim.DatePaid);
            $('#lbldescription').text(viewClaim.Description);
            $('#litViewClaimGrid').html(viewClaim.Grid[1]);
            SEL.Grid.updateGrid(viewClaim.Grid[0]);
            $('#divViewClaim').dialog({
                modal:true,
                title: "View Claim",
                minWidth: 850,
                dialogClass: "formpanel",
                maxHeight: 650,
                height:650,
                buttons: [{
                    text: "OK",
                    "class": "jQueryUIButton",
                    click: function () {
                    $('#divViewClaim').dialog('close');
                }}]
            });
        }

        function chartWizard(requestnum)
        {
            window.open('chartviewer.aspx?requestnum=' + requestnum,'chartviewer','toolbars=no, scrollbars=yes,resizable=yes');
        }
        
        var wndDrilldown;
        function showDrillDownReports()
        {         
            window.name = "drilldowns";
            wndDrilldown = window.open('drilldown.aspx?reportid=' + document.getElementById('drilldownreportid').value + '&basetable=' + document.getElementById('basetable').value,'drilldown','toolbars=no, width=300, height=450');
        }
        
        function getDrillDownReport()
        {
            var drilldown = 0;
            var rpts = wndDrilldown.document.getElementsByName('optlist');
            for (var i = 0; i < rpts.length; i++)
            {
                if (rpts[i].checked == true)
                {
                    drilldown = rpts[i].value;
                    document.getElementById('drilldownreportid').value = rpts[i].value;
                    break;
                }
            }
            
            
            wndDrilldown.close();
            PageMethods.updateDrilldownReport (accountid, employeeid, reportid, drilldown);

        }
        
        function saveDialog() 
        {
            $find('<%=mdlSaveDialog.ClientID %>').show();
        }
        
        function saveDialogClose() 
        {
            $find('<%=mdlSaveDialog.ClientID %>').hide();
            document.getElementById('<% = txtReportName.ClientID %>').value = reportName;
        }

        var reportName = '<% = reportName %>';
        
        function saveAs() 
        {
            var reportName = document.getElementById('<% = txtReportName.ClientID %>').value;
            var categoryID = document.getElementById("<% = cmbCategory.ClientID %>").options[document.getElementById("<% = cmbCategory.ClientID %>").selectedIndex].value;

            if (categoryID == "0") {
                categoryID = null;
            }

            PageMethods.saveAs(accountid, reportid, employeeid, reportName, categoryID, savedReport);
        }
        
        function savedReport(results) 
        {
            if (results == '00000000-0000-0000-0000-000000000000') {
                SEL.MasterPopup.ShowMasterPopup('The New Name already exists.', 'Message from ' + moduleNameHTML);
                return;
            }
            window.location = 'reportviewer.aspx?reportid=' + results + '&reportarea=2';
            saveDialogClose();
        }

        function viewcontract(requestnum, contractid) {
            var myWin = window.opener;

            if (myWin != null) {
                myWin.location = appPath + '/ContractSummary.aspx?tab=0&requestnum=' + requestnum + '&id=' + contractid, 'viewcontract', 'toolbars=no, scrollbars=yes, resizable=yes';
                myWin.focus();
            }
            else {
                window.open(appPath + '/ContractSummary.aspx?tab=0&requestnum=' + requestnum + '&id=' + contractid, 'viewcontract', 'toolbars=no, scrollbars=yes, resizable=yes');
            }
        }

        function viewsupplier(requestnum, supplierid) {
            var myWin = window.opener;

            if (myWin != null) {
                myWin.location = appPath + '/shared/Supplier_Details.aspx?sid=' + supplierid, 'viewsupplier', 'toolbars=no, scrollbars=yes, resizable=yes';
                myWin.focus();
            }
            else {
                window.open(appPath + '/shared/Supplier_Details.aspx?sid=' + supplierid, 'viewsupplier', 'toolbars=no, scrollbars=yes, resizable=yes');
            }
        }

        function viewtask(requestnum, taskid) {
            var myWin = window.opener;

            if (myWin != null) {
                myWin.location = appPath + '/shared/tasks/ViewTask.aspx?tid=' + taskid, 'viewtask', 'toolbars=no, scrollbars=yes, resizable=yes';
                myWin.focus();
            }
            else {
                window.open(appPath + '/shared/tasks/ViewTask.aspx?tid=' + taskid, 'viewtask', 'toolbars=no, scrollbars=yes, resizable=yes');
            }
        }
    </script>

    <script id="Infragistics" type="text/javascript">
<!--
function gridreport_ClickCellButtonHandler(gridName, cellId) {
	var grid = igtbl_getGridById(gridName);
	var cell = igtbl_getCellById(cellId);
	
	var drilldown = document.getElementById('drilldownreportid').value;
	var column, value;
	column = cell.Column.Key;
	
	if (cell.Column.DataType == "7")
	{
	    value = cell.MaskedValue;
	}
	else
	{
    	value = cell.getValue();
    }

    var reportArea = 2;
	var url = "reportviewer.aspx?callback=1&item=1&reportid=" + reportid + "&requestnum=" + requestNum + "&drilldownreportid=" + drilldown + "&reportarea=2";
	var details;
	var xmlRequest;
	
	try
	{
		xmlRequest = new XMLHttpRequest();
	}
	catch (e)
	{	
		try
		{
			xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (f)
		{
			xmlRequest = null;
		}
	}
	
	var data;
	
	
			
	data = "column=" + column + "&value=" + value;
			
	
	xmlRequest.onreadystatechange = function()
	{
	
	    if (xmlRequest.readyState == 4)
	    {
	        
	        details = xmlRequest.responseText;
	        window.open('reportviewer.aspx?reportid=' + drilldown + "&requestnum=" + details, 'reportviewer' + details, 'locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');
	    }
	}
	xmlRequest.open("POST",url,true);
	//xmlRequest.setRequestHeader("Content-Type","application/x-wais-source");
	xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	xmlRequest.send(data);
}// -->
</script>


            <script type="text/javascript" language="javascript">
var runRequestNumber = '<% = requestnum %>';
var runReportID;
var runReportName;

function ExitViewer()
{
    ReportFunctions.ClearReportFromSession(runRequestNumber);
}

function GetReportStatus()
{
    ReportFunctions.getReportProgress(runRequestNumber, updateReportStatus, onError);
}

function updateReportStatus(data)
{
    if (data == null)
    {
        setProgressBar(100);
        setStatus("Complete");
    }
    else
    {
        var status = data[0];
        var reportProgress = data[1];
        document.title = data[2];

        if (status == "BeingProcessed")
        {
            var doneWith = reportProgress;
            setProgressBar(doneWith);
            setStatus("Running");
            setTimeout(GetReportStatus, 500);
        }
        else if (status == "Complete")
        {
            setProgressBar(100);
            setStatus("Complete");
            __doPostBack('<% = upnlReport.UniqueID %>', runRequestNumber);
        }
        else if (status == "Failed")
        {
            //setProgressBar(100);
            var errorString = "An error has occured processing this report";
            setStatus(errorString);
        }
        else
        {
            setStatus("Queued");
            setTimeout(GetReportStatus, 500);
        }
    }
}

function setStatus(status) {
    var statusDiv = document.getElementById("reportStatus");
    
    if(statusDiv != undefined)
    {
        statusDiv.innerHTML = status;
    }
    return;
}

function setProgressBar(doneWidth) {
        var reportPercentDiv = document.getElementById("reportPercentDone");
        if(reportPercentDiv != undefined)
        {
            reportPercentDiv.innerHTML = doneWidth + "%";
            var divWidth = doneWidth * 2.5;
            document.getElementById("reportDone").style.width = divWidth + "px";
        }
        return;
}

function onError(exception) 
{
    if (exception.get_timedOut()) 
    {
        alert(exception);
    }
    
}  

GetReportStatus();
</script>

 <div>   
        
        <igmisc:WebPanel ID="WebPanel1" runat="server" BackColor="White" Expanded="False" Width="95%" meta:resourcekey="WebPanel1Resource1" StyleSetName="">
            <PanelStyle BorderStyle="Solid" BorderWidth="1px">
                <Padding Bottom="5px" Left="5px" Right="5px" Top="5px" />
                <BorderDetails ColorBottom="0, 45, 150" ColorLeft="158, 190, 245" ColorRight="0, 45, 150"
                    ColorTop="0, 45, 150" />

            </PanelStyle>
            <Header Text="Report Filter (Double click to expand)" TextAlignment="Left">
                <ExpandedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </ExpandedAppearance>
                <ExpansionIndicator Height="0px" Width="0px" />
                <CollapsedAppearance>
                    <Styles CssClass="inputpaneltitle">
                    </Styles>
                </CollapsedAppearance>
            </Header>
            <Template>
                <asp:Literal ID="litcriteria" runat="server" meta:resourcekey="litcriteriaResource1"></asp:Literal>
            </Template>
        </igmisc:WebPanel>

<asp:UpdatePanel ID="upnlReport" runat="server">
        <ContentTemplate>
        <div id="litChart" runat="server" Visible="False">
            <div class="sectiontitle">Chart</div>
        </div>

                  <div id="reportStatusInformation" runat="server">
         <div id="reportStatus">Processing Request</div>
         <div id="reportProgress" style="width: 250px; background-image: url('../images/exportReportBackground250px.png'); height: 20px;">
                <div id="reportDone" style="background-image: url('../images/exportReport250px.png'); height: 20px;">&nbsp;</div>
        </div>
        <div id="reportPercentDone">0%</div>
    </div>
   
   
   
    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="100%" Width="100%" meta:resourcekey="WebAsyncRefreshPanel1Resource1"> 
     

       
        
           <asp:Label ID="lblcountmsg" Visible="false" runat="server" Text="Label" ForeColor="Red"></asp:Label>  
           
           
        <igtbl:UltraWebGrid ID="gridreport" runat="server" SkinID="gridskin" OnInitializeLayout="gridreport_InitializeLayout" OnGroupColumn="gridreport_GroupColumn" OnInitializeRow="gridreport_InitializeRow" OnUnGroupColumn="gridreport_UnGroupColumn" PreRender="gridReport_PreRender" EnableViewState="False" meta:resourcekey="gridreportResource1" Width="100%">
            <DisplayLayout BorderCollapseDefault="Separate" Name="gridreport" RowHeightDefault="20px" Version="4.00" RowSelectorsDefault="No" AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowSortingDefault="Yes" ColFootersVisibleDefault="Yes" RowSizingDefault="Free" HeaderClickActionDefault="SortSingle" TableLayout="Fixed">
                <FilterOptionsDefault><FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif" Font-Size="11px" Width="200px"> <Padding Left="2px" /> </FilterDropDownStyle><FilterHighlightRowStyle BackColor="#151C55" ForeColor="White"></FilterHighlightRowStyle></FilterOptionsDefault>
                <ClientSideEvents ClickCellButtonHandler="gridreport_ClickCellButtonHandler" />
            </DisplayLayout>
        </igtbl:UltraWebGrid>
        <igcalc:UltraWebCalcManager ID="calcman" runat="server" OnFormulaCalculationError="calcman_FormulaCalculationError" OnFormulaCircularityError="calcman_FormulaCircularityError" OnFormulaReferenceError="calcman_FormulaReferenceError" OnFormulaSyntaxError="calcman_FormulaSyntaxError" OnCalculationsCompleted="calcman_CalculationsCompleted" OnFormatValue="calcman_FormatValue" OnParseValue="calcman_ParseValue"></igcalc:UltraWebCalcManager>
        
        </igmisc:WebAsyncRefreshPanel>
  </div>
 </ContentTemplate>
</asp:UpdatePanel>
        <cc1:ModalPopupExtender ID="mdlSaveDialog" TargetControlID="lnkSave" PopupControlID="pnlSaveOption" runat="server" BackgroundCssClass="modalBackground" OnCancelScript="saveDialogClose()" OnOkScript="saveDialog()"></cc1:ModalPopupExtender>
        <asp:HyperLink ID="lnkSave" runat="server" style="display: none;" Text="&nbsp;">&nbsp;</asp:HyperLink>   
        <asp:Panel ID="pnlSaveOption" runat="server" style="background-color: #ffffff; border: 1px solid #000000; padding: 10px;">
        <table>
            <tr><td class="labeltd">New Name:</td><td class="inputtd"><asp:TextBox ID="txtReportName" runat="server" style="width: 100%;"></asp:TextBox></td></tr>
            <tr><td class="labeltd">Category:</td><td class="inputtd">
            <asp:DropDownList ID="cmbCategory" runat="server">
            
            </asp:DropDownList>
            </td></tr>
        </table>      
        <p><a href="javascript:saveAs();"><asp:Image ID="imgOK" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" BorderWidth="0" /></a> <a href="javascript:saveDialogClose();"><asp:Image ID="imgCancel" runat="server" ImageUrl="~/shared/images/buttons/cancel_up.gif" BorderWidth="0" /></a></p>
        </asp:Panel>
    </div>
        </asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:Literal ID="litoptions" runat="server" meta:resourcekey="litoptionsResource1"></asp:Literal>
    <div id="divExcel" runat="server"><a id="hrefExcel" href="" class="submenuitem" runat="server">Export to Excel</a></div>
    <asp:Literal ID="litoptions2" runat="server"></asp:Literal> 
    <div id="divPivot" runat="server"><a id="hrefPivot" href="" class="submenuitem" runat="server">Create Pivot Table</a></div>
    <div id="divPrinterFriendly" runat="server"><a id="hrefPrinterFriendly" href="javascript:printerFriendly();" class="submenuitem" runat="server">Printer Friendly</a></div>
    <asp:Literal ID="litoptions3" runat="server"></asp:Literal>   
    <div id="divViewClaim"  style="display: none;">
        <div class="twocolumn">
            <label id="lblemployelabel" >Employee Name</label><span class="inputs"><span id="lblemployee"></span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        
        <div class="twocolumn">
            <label id="lblclaimnum" >Claim No</label><span class="inputs"><span id="lblclaimno">Label</span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span>
            <label id="lbldatepaidlbl" for="lbldatepaid">Date Approved</label><span class="inputs"><span id="lbldatepaid">£0.00</span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div class="twocolumn">
            <label id="lbldescriptionlbl" >Description</label><span class="inputs"  style="width: 250px;"><span id="lbldescription" >Label</span></span><span class="inputicon"></span><span class="inputtooltipfield"></span><span class="inputvalidatorfield"></span></div>
        <div id="litViewClaimGrid"></div>
    </div>
</asp:Content>

