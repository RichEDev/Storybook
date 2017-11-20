<%@ Control Language="VB" AutoEventWireup="false" Inherits="frameworkWebApp.FWTasksPopupSummary" Codebehind="FWTasksPopupSummary.ascx.vb" %>
<asp:ScriptManagerProxy ID="smpTaskSummaryPopup" runat="server">
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcTasks.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<script type="text/javascript" language="javascript">

    /*var taskSummaryPopup = '<%-- = pceTaskSummary.ClientID --%>';*/
    var taskTable = '<% = pnlTaskSummary.ClientID %>';
    var taskSummaryModal = '<% = mdlTaskSummary.ClientID %>';

    /*function openTaskSummaryPopup(parent) {
    var popup = $find(taskSummaryPopup);
    popup._popupBehavior._parentElement = document.getElementById(parent);
    popup.showPopup();
    return;
    }*/

    function openTaskSummaryModal() {
        $find(taskSummaryModal).show();
        return;
    }

    function CompleteTasks() {
        var tasksTableID = document.getElementById(taskTable).getElementsByTagName("table")[0].id;
        var taskInputs = document.getElementById(tasksTableID).getElementsByTagName("input");
        var tasks = new Array();

        for (var i = 1; i < taskInputs.length; i++) {
            if (taskInputs[i].type == "checkbox" && taskInputs[i].checked == true) {
                tasks.push(taskInputs[i].value);
            }
        }

        Spend_Management.svcTasks.CompleteTasks(tasks, RemoveTaskRow, function () { });
        return;
    }

    function RemoveTaskRow(data) {
        var tasksTableID = document.getElementById(taskTable).getElementsByTagName("table")[0].id;
        tasksTableID = tasksTableID.substr(4);
        for (var i = 0; i < data.length; i++) {
            SEL.Grid.deleteGridRow(tasksTableID, data[i]);
        }
        $find(taskSummaryModal).hide();
        Spend_Management.svcTasks.getTasksToCompleteGrid();
        return;
    }

    function CloseTasks() {
        $find(taskSummaryModal).hide();
        return;
    }
</script>

<asp:Panel ID="pnlTaskSummary" runat="server" CssClass="modalpanel formpanel" style="display: none; ">
    <div class="sectiontitle">Task List</div>
    <div class="comment" style="margin: 4px;">
    <strong>Note:</strong>&nbsp;Selecting the check boxes against the tasks and clicking the 'Save' button will mark the tasks as complete. Once completed, the tasks will no longer be listed in this panel.<br /><br />To view closed tasks, go to the 'My Tasks' screen and click 'View Closed' from the page options.
    </div>
    <div style="height: 300px; overflow: auto;"><asp:Literal ID="litTaskSummaryPopup" runat="server"></asp:Literal></div>
    <div class="formbuttons"><asp:Image ID="btnSaveTasksModal" runat="server" ImageUrl="~/shared/images/buttons/btn_save.png" onclick="javascript:CompleteTasks();" AlternateText="Save" />&nbsp;<asp:Image ID="btnCloseTasksModal" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" onclick="javascript:CloseTasks();" AlternateText="Close" /></div>
</asp:Panel>

<asp:HyperLink ID="hlTaskSummaryPopup" runat="server" Text="&nbsp;" style="display: none;"></asp:HyperLink>
<cc1:ModalPopupExtender ID="mdlTaskSummary" runat="server" OkControlID="btnSaveTasksModal" CancelControlID="btnCloseTasksModal" BackgroundCssClass="modalBackground" PopupControlID="pnlTaskSummary" TargetControlID="hlTaskSummaryPopup"></cc1:ModalPopupExtender>
<%--
<asp:Panel ID="pnlTaskSummary" runat="server" CssClass="modalpanel" style="margin-top: 10px; display: none;">
    <div style="height: 300px; overflow: auto; margin-top: 20px;"><asp:Literal ID="litTaskSummaryPopup" runat="server"></asp:Literal></div>
    <div><img src="/shared/images/buttons/btn_save.png" onclick="javascript:CompleteTasks();" alt="Save" /></div>
</asp:Panel>

<asp:HyperLink ID="hlTaskSummaryPopup" runat="server" Text="&nbsp;" style="display: none;"></asp:HyperLink>
 <cc1:PopupControlExtender ID="pceTaskSummary" runat="server" PopupControlID="pnlTaskSummary" TargetControlID="hlTaskSummaryPopup"></cc1:PopupControlExtender>--%>