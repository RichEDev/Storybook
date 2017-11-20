 <%@ Page language="c#" Inherits="Spend_Management.rptlist" MasterPageFile="~/masters/smTemplate.master" Codebehind="rptlist.aspx.cs" %> 
<%@ Import Namespace="SpendManagementLibrary" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:Literal ID="litoptions" runat="server" meta:resourcekey="litoptionsResource1"></asp:Literal>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

<asp:ScriptManagerProxy ID="smp" runat="server">
     <Services>
     <asp:ServiceReference Path="ReportFunctions.asmx" />
     </Services>
</asp:ScriptManagerProxy>

<script language="javascript" type="text/javascript">
            
            
            function deleteReport(reportid)
            {
                if (confirm('Are you sure you wish to delete the selected report?'))
                {
                    document.location = "rptlist.aspx?action=3&reportid=" + reportid;
                }
            }
            
            function runReport(reportname, reportid, claimants, area)
            {
                window.open('reportviewer.aspx?reportid=' + reportid + '&reportarea='+ area,'reportviewer','locationbar=no,menubar=no,scrollbars=yes,status=1,resizable=1');
            }
            


                    function toggle(divId)
                    {
                        imgId = 'img' + divId;
                        if(document.getElementById(divId).style.display != 'block')
                        {
                            document.getElementById(divId).style.display = 'block';
                            document.getElementById(imgId).src = '/images/buttons/close.gif';
                        }
                        else
                        {
                            document.getElementById(divId).style.display = 'none';
                            document.getElementById(imgId).src = '/images/buttons/open.gif';

                        }
                    }
        function runPivot(reportid, claimants)
        {
            
            window.open('createpivot.aspx?reportid=' + reportid + '&claimants=' + claimants);
        }	
        
        
        function exportReport (reportid, exporttype)
        {
        
            window.open("exportreport.aspx?exporttype=" + exporttype + "&reportid=" + reportid + "&rstSessVars=1",'export','width=300,height=150,status=no,menubar=no');
        }
        function editReport (reportid, reportWithJoinVia)
        {
            document.location = 'aereport.aspx?reportid=' + reportid;
        }
        
        var exportReportID;
        
        function runExportReport(exportType) 
        {
            exportReport(exportReportID, exportType);
        }

        //This will be called from the reportviewer pop up page to remove the report request now the user has finished viewing the page
        function cancelReportRequest(requestNumber) 
        {
            ReportFunctions.cancelReportRequest(requestNumber, onSuccess, onError); //.cancelReportRequest(runRequestNumber);

            return;
        }

        function onSuccess(result)   //, userContext, methodName
        {
            var balh = result;
        }

        function onError(exception) {
            if (exception.get_timedOut()) {
                alert(exception);
            }
            else {
                //Exception occurred   
            }
        }  
                
        function showExportOptions(reportid, controlID)
        {
            exportReportID = reportid;
             $find('<%=pceExport.ClientID %>')._popupBehavior._parentElement = document.getElementById(controlID);
            $find('<%=pceExport.ClientID %>').showPopup();
        }
        
        function scheduleReport()
        {
            window.location = 'aeschedule.aspx?returnto=1&reportid=' + exportReportID;
        }		
</script>
    
<div class="formpanel formpanel_padding">
     <div class="twocolumn">
            <asp:Label id="lblgridreports_cmbfilter" runat="server" AssociatedControlID="gridreports_cmbFilter">Category</asp:Label>
            <span class="inputs" ><asp:DropDownList id="gridreports_cmbFilter" CssClass="cmbfilter" runat="server" onchange="javascript:SEL.Grid.filterGridCombo('gridreports','category');" >
            </asp:DropDownList></span>
        </div>
    <asp:Literal ID="litGrid" runat="server"></asp:Literal>
</div>

        
    <asp:Panel ID="pnlExport" runat="server" style="background-color: #ffffff; border: 1px solid #000000; display: none">
        <div style="padding: 4px;"><a href="javascript:runExportReport(3);">Export to CSV</a></div>
        <div style="padding: 4px;"><a href="javascript:runExportReport(2);">Export to Excel</a></div>
        <div style="padding: 4px;"><a href="javascript:runExportReport(4);">Export to Flat File</a></div>
        <div style="padding: 4px;"><a href="javascript:runExportReport(5);">Export to Pivot</a></div>
        <div style="padding: 4px;"><a href="javascript:scheduleReport();">Schedule Export</a></div>
    </asp:Panel>   
    <asp:HyperLink ID="hlExport" runat="server" style="display: none;">&nbsp;</asp:HyperLink>
    <cc1:PopupControlExtender ID="pceExport" OffsetX="17" runat="server" TargetControlID="hlExport" PopupControlID="pnlExport"></cc1:PopupControlExtender>


        <div class="formpanel formpanel_padding">
            <div class="formbuttons" style="margin-top:30px;">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
        </div>
    </asp:Content>


<asp:Content ID="scriptsContent" runat="server" ContentPlaceHolderID="scripts">
      <script type="text/javascript" src="<%=GlobalVariables.StaticContentLibrary%>/js/expense/jquery.smoothscroll.js"></script> 
    <script type="text/javascript">
        $(document).ready(function () {
            $('#gridreports').customScroll({ cursorcolor: "#19a2e6", autohidemode: false, enablemousewheel: false });
            $('#ascrail2000').hide();
            $('#ascrail2000-hr > div').css('top', '15px');
        });
        </script>
    </asp:Content>