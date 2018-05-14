<%@ Page Language="C#" AutoEventWireup="true" Inherits="Spend_Management.shared.reports.View" MasterPageFile="~/masters/smForm.master" Codebehind="View.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy runat="server" ID="smProxy">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.filterDialog.js?date=20180126"/>
            <asp:ScriptReference Path="~/shared/javaScript/sel.reports.js?date=20180502" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.reportViewer.js?date=20180502" />
            <asp:ScriptReference Path="~/shared/javaScript/sel.ajax.js"/>
            <asp:ScriptReference Name="tooltips" />
            <asp:ScriptReference Name="SyncFusion"/>
        </Scripts>
    </asp:ScriptManagerProxy>
    <script id="Change" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon Change" />
    </script>
    <script id="Save" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon Save" />
    </script>
    <script id="Excel" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon Excel" />
    </script>
    <script id="CSV" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon CSV" />
    </script>
    <script id="Flat" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon Flat" />
    </script>
    <script id="Pivot" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon Pivot" />
    </script>
    <script id="ExportOptions" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon ExportOptions" />
    </script>
    <script id="DrilldownReport" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon DrilldownReport" />
    </script>
    <script id="ClearFilter" type="text/x-jsrender">
        <a  class="e-toolbaricons e-icon ClearFilter" />
    </script>
    <style>
        

/*ej*/
th.e-headercell {
    color: #282827!important;
}
.ejsel-grid-header-menu-button, .ejsel-grid-header-edit-button, ejsel-grid-header-icon {
    padding-right: 7px;
}

.ejsel-grid-header-icon {
    width: 16px;
    height: 16px;
    display: inline-block;
    margin: 0 6px -2px 0;
}

.ejsel-waitingpopup {
    background-color: #BBB;
}

th.e-headercell div.e-headercelldiv {
    text-align: left !important;
}

.pagePadding {
    padding-right: 20px;
}

.tabs {
      min-height: 200px;
      float: left;
      width: 100px;
  }

#menuOwner {
    height: 555px;
}
.searching {
    -ms-opacity: .3;
    opacity: .3;
}

ul{
	padding:0px;
	margin:0px;
	}
.columnName {
    width:155px;
}
.closeArrow {
    background-image: url(/static/icons/16/new-icons/navigate_close.png);
    background-repeat: no-repeat;
    background-position: 65px 10px;
    background-size: 16px 16px;
    cursor:pointer;
}

.openArrow {
    background-image: url(/static/icons/16/new-icons/navigate_open.png);
    background-repeat: no-repeat;
    background-position: 65px 12px;
    background-size: 16px 16px;
    cursor:pointer;
    border-bottom-style: solid;
    border-bottom-width: 1px;
    border-bottom-color: #fff;
}

.e-grid .e-headercelldiv{
    margin:-12px;
}

.selectGroup{
   cursor:pointer;
}
.SelectedType {
    background-color: lightgray;
}


#imgChartPreview {
    border: 1px solid; 
    border-color: <%=BorderColour%>;     
    border-image-outset:3px;
    position:absolute;
    top:95px;
    left:606px;
} 

#imgChart {
    position: absolute;
    top: 10px;
    left: 100px;
    border: 1px solid; 
    border-color: <%=BorderColour%>;     
    border-image-outset:3px;
}

.group-first {
    border-top:2px solid  <%=BorderColour%>  !important;
}

.group-last {
    border-bottom:2px solid  <%=BorderColour%>   !important;
}
td input[type="checkbox"] {
          float: left;
          margin: 0 auto;
          width: auto;
    }
.cholder {
background: rgb(199, 223, 247) none repeat scroll 0% 0%;
    border-left: 1px solid <%=BorderColour%> !important;	
}


#_st_clone_ .easytree-title{
    font-size: 20px;
    padding: 10px 20px 20px 20px;
    border: 1px solid #c8c8c8;
    color: <%=TextColour%>;
    background-color: transparent;
    background: <%=BorderColour%>;
}

.e-rowcell.easytree-droppable.easytree-accept.column-drop-border {
    border-left: 3px solid <%=BorderColour%>!important;
    padding-left: 0.5em;
}

.e-rowcell.easytree-droppable.easytree-accept.column-drop-border-right {
    border-right: 3px solid <%=BorderColour%>!important;
    padding-right: 0.47em;
}

.e-grid .e-gridheader .e-headercell.column-drop-border {
    border-left: 3px solid <%=BorderColour%>!important;
    padding-left: 0.35em;
}


.e-grid .e-gridheader .e-headercell.column-drop-border-right {
    border-right: 3px solid <%=BorderColour%>!important;
    padding-right: 0.5em;
}

.e-grid .e-reorderindicate {
  border-right:3px solid <%=BorderColour%>!important;
}

.e-grid .e-gridheader {
    border-bottom-color:  <%=BorderColour%>!important;
}

.e-grid .e-rowcell:empty, .e-rowcell.easytree-droppable.easytree-accept {
  height: 32px !important;
}

#Dropper .e-grid .e-rowcell:empty {
    height: 20px !important;
}
#Dropper .e-grid .filterInfo td {
       border-width: 1px;
}
#Dropper .e-grid .filterInfo td:nth-child(3) {
       border-width: 1px 1px 1px 1px;
}
#Dropper .e-grid .filterInfo td:nth-child(4) {
    border-top: 1px solid #c8c8c8;
       border-width: 1px 1px 1px 0;
}

.e-grid .e-headercell, .e-grid .e-grouptopleftcell {
        background: #fcfcfc!important;
}

.e-grid .e-headercell.e-headercellactive, .e-dragclone ,.e-grid .e-groupdroparea {
     background: <%=BorderColour%>!important;
    color: <%=TextColour%>!important;
}

.filterInfo.ui-sortable-placeholder {
    height: 50px;
}

.e-grid .e-groupdroparea.e-hover {
  background: #f6f6f6!important;
    color: #333333 !important;
}
 #reportStatusInformation {
     position: relative;

 }
#imgSpin {
    position: absolute;
    top: 50%;
    left: 30%;

}

.Change:before
{
    content:"\1f589";
}

.Save:before
{
    content:"\1f5ab";
}

.Excel:before
{
    content:"\1f5ce";
}

.CSV:before
{
    content:"\1f5cb";
}
.Flat:before
{
    content:"\1f5bb";
}
.Pivot:before
{
    content:"\1f5ba";
}
.ExportOptions:before {
    content: "\26ed";
}

.DrilldownReport:before {
    content: "\21ca";
}
.ClearFilter:before {
    content: "\e668";
}
.e-hover {
    background-color: <%=this.BorderColour%>;
}

    </style>
    <script type="text/javascript">
        $(document).ready(function() {
            SEL.ReportViewer.IDs.RequestNumber = <%=requestnumber%>;
            SEL.ReportViewer.IDs.ReportId = '<%=this.reportId%>';
            setTimeout(function () { SEL.ReportViewer.GetReportStatus(); }, 500);
            var existingValue = $('#' + '<%=this.divCriteriaValue.ClientID%>').val();
            if (existingValue > '')
            {
                SEL.ReportViewer.CriteriaNodesRefreshComplete(JSON.parse(existingValue));
            }


            $('#Dropper').slideToggle('fast');
            $('#criteriaContainer').addClass("openArrow").click(function ()
            {
                $('#Dropper').slideToggle('fast');
                if ($(this).hasClass("closeArrow")) {
                    $(this).removeClass("closeArrow").addClass("openArrow");
                } else {
                    $(this).removeClass("openArrow").addClass("closeArrow");
                }
            });
            // Page option moving
            $('#maindiv').css('margin-left', '20px');
            $('.submenuholder').hide();


           
        


        });
        function PageOptions() {
            var optionPosition = $('#showPageOptions').position();
            var optionTop = optionPosition.top + 140 + 'px';
            var optionLeft = optionPosition.left + $('#sidebar-wrapper').width() + 'px';
            console.log(optionPosition, optionTop, optionLeft);
            $('.submenuholder').css('position', 'absolute').css('top', optionTop).css('left', optionLeft).css('background-color', 'white').show('slow').unbind().mouseleave(
                function() {
                    $('.submenuholder').hide('slow');
                });
            
        }

        function exportReport(exporttype, reportid) 
        {
            window.open("exportreport.aspx?exporttype=" + exporttype + "&reportid=" + reportid + "&requestnum=" + requestNum, 'export', 'width=300,height=150,status=no,menubar=no');
        }

        function changeColumns() 
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
        function printerFriendly() {
            $("#preview").ejGrid("print");
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
                if (rpts[i].checked === true)
                {
                    drilldown = rpts[i].value;
                    document.getElementById('drilldownreportid').value = rpts[i].value;
                    break;
                }
            }
            
            
            wndDrilldown.close();
            var params = {reportId:reportid, drilldown:drilldown};
            SEL.Ajax.Service('/shared/webServices/svcReports.asmx/',
                'updateDrilldownReport',
                params,
                null,
                SEL.ReportViewer.ErrorHandler);

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

            var params = {reportId:reportid, reportName: reportName, folderId:categoryID};
            SEL.Ajax.Service('/shared/webServices/svcReports.asmx/',
                'SaveAs',
                params,
                savedReport,
                SEL.ReportViewer.ErrorHandler);
        }
        
        function savedReport(results) 
        {
            if (results.d == '00000000-0000-0000-0000-000000000000') {
                SEL.MasterPopup.ShowMasterPopup('The New Name already exists.', 'Message from ' + moduleNameHTML);
                return;
            }
            window.location = 'view.aspx?reportid=' + results.d + '&reportarea=2';
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
    <div id="reportStatusInformation">
        <img alt="loading" src="/static/images/Wedges-3s-200px.gif" style="margin-right: 8px" id="imgSpin"/>
    </div>
    <div id="divReportWindow" style="display: none;">
        <div id="criteriaContainer" class="sectionHeader">
            <h4 style="display: inline;" > Filters</h4>
            </div>
        <div class="dropper easytree-droppable" id="Dropper" style="padding: 10px 0 10px 10px; overflow: hidden;">
            <table class="e-grid e-table e-headercontent"id="criteriaList" style="width:100%;">
            <tr id="criteriaListHeader" class="e-columnheader e-gridheader ">
            <td class="e-headercell" style="width:45px;"><div class="e-headercelldiv">And/Or</div></td><td class="e-headercell"><div class="e-headercelldiv">Column</div></td><td class="e-headercell"><div class="e-headercelldiv">Filter Criteria</div></td><td class="e-headercell"><div class="e-headercelldiv">Value</div></td>
            </tr>
            <tfoot>
                
            </tfoot>
            </table>
        </div>
        <div runat="server" ID="divFilter" clientidmode="Static" class="sm_panel" style="display: none"></div>
        <div class="sectionHeader">Report</div>
        </div>
    <div runat="server" ID="div1" clientidmode="Static" class="sm_panel" style="display: none"></div>
    <div id="previewWindow">
        <div id="preview"></div>
    </div>
    
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
</asp:Content>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:Literal ID="litoptions" runat="server" ></asp:Literal>
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
    <asp:HiddenField runat="server" ID="divCriteriaValue"/>
</asp:Content>