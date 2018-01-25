<%@ Page Language="VB" MasterPageFile="~/FWMaster.master" AutoEventWireup="false" Inherits="frameworkWebApp.Framework2006.ScheduleAction" title="Untitled Page" Codebehind="ScheduleAction.aspx.vb" %>
<%@ MasterType VirtualPath="~/FWMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
<script language="javascript" type="text/javascript">
var curContractId;

function HideMerge()
{
    var cntl = document.getElementById('lnkMergeSchedule');
    if(cntl != null)
    {
        cntl.style.display = 'none';
    }
}

function SetContractId(conId)
{
    curContractId = conId;
}

function doAddSchedule()
{
    if(confirm('Click OK to confirm creation of new schedule'))
    {
        window.location.href='ContractSummary.aspx?action=addschedule&id=' + curContractId;
    }
    else
    {
        window.location.href='ScheduleAction.aspx?contractId=' + curContractId;
    }
}

function doUpdateSchedule()
{
    if(confirm('Click OK to confirm creation of new schedule'))
    {
        window.location.href='ContractSummary.aspx?action=updateschedule&id=' + curContractId;
    }
    else
    {
        window.location.href='ScheduleAction.aspx?contractId=' + curContractId;
    }
}

function doMergeSchedule()
{
    window.location.href='ScheduleManage.aspx?action=merge&cid=' + curContractId;
}

function doAddExisting()
{
    window.location.href='ScheduleManage.aspx?&action=addlinks&id=' + curContractId;
}

function SetPermissions(permInsert,permUpdate,permDelete)
{
    var cntl;
    
    if(permInsert == false && permUpdate == false)
    {
        cntl = document.getElementById('lnkAddSchedule');
        if(cntl != null)
        {
            cntl.style.display = 'none';
        }
        
        cntl = document.getElementById('lnkUpdateSchedule');
        if(cntl != null)
        {
            cntl.style.display = 'none';
        }
        
        cntl = document.getElementById('lnkMergeSchedule');
        if(cntl != null)
        {
            cntl.style.display = 'none';
        }
        
        cntl = document.getElementById('lnkCreateLinks');
        if(cntl != null)
        {
            cntl.style.display = 'none';
        }
    }
}
</script>
<a class="submenuitem" id="lnkAddSchedule" href="javascript:doAddSchedule();" onmouseover="window.status='Create a new schedule of the current contract';return true;" onmouseout="window.status='Done';" title="Create a new schedule of the current contract">Add Schedule</a>
<a class="submenuitem" id="lnkUpdateSchedule" href="javascript:doUpdateSchedule();" onmouseover="window.status='Duplicate the current schedule (Save As New)';return true;" onmouseout="window.status='Done';" title="Duplicate the current schedule (Save As New)">Update Schedule</a>
<a class="submenuitem" id="lnkCreateLinks" href="ScheduleManage.aspx?action=createlinks" onmouseover="window.status='Create schedule links for existing contracts';return true;" onmouseout="window.status='Done';" title="Create schedule links for existing contracts">Create Schedule Links</a>
<a class="submenuitem" id="lnkMergeSchedule" href="javascript:doMergeSchedule();" onmouseover="window.status='Merge multiple schedules to a single schedule entry';return true;" onmouseout="window.status='Done';" title="Merge multiple schedules to a single schedule entry">Merge Schedule</a>
<a class="submenuitem" id="lnkAddScheduleLink" href="javascript:doAddExisting();" onmouseover="window.status='Add a contract as a schedule to the current contract. Retrospectively add a schedule to the current contract.';return true;" onmouseout="window.status='Done';" title="Add a contract as a schedule to the current contract. Retrospectively add a schedule to the current contract.">Add Existing Schedule</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<div class="inputpanel formpanel_padding">
<asp:Label ID="lblError" runat="server" ForeColor="red"></asp:Label>
</div>
<div class="inputpanel formpanel_padding">
<div class="inputpaneltitle">Current Contract Schedules</div>
<asp:Literal runat="server" ID="litCurrentSchedules"></asp:Literal>
</div>
<div class="inputpanel formpanel_padding">
<asp:ImageButton runat="server" ID="cmdClose" ImageUrl="./buttons/page_close.png" CausesValidation="false" />
</div>
</asp:Content>

