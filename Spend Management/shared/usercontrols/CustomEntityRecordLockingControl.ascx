<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomEntityRecordLockingControl.ascx.cs" Inherits="Spend_Management.shared.usercontrols.CustomEntityRecordLockingControl" %>
<div class="formpanel" id="divElementLockingDialog" style="display: none">
    <div class="sectiontitle" runat="server" ID="lockDialogTitle">{Title} - Locked</div>
    <div ><span id="dialogLockingTitle" runat="server"></span><span> is in use by </span><span id="dialogLockingUser" runat="server"></span>.</div>
    <div>This record will automatically unlock when it is no longer in use.</div>
</div>
<div id="divData" style="display: none">
    <asp:HiddenField runat="server" ID="hiddenActive" />
    <asp:HiddenField runat="server" ID="hiddenLocked" />
</div>

<div class="sm_panel sm_comment" id="divElementLocking" style="display: none">
    <span id="elementLockingTitle" runat="server"></span><span> is in use by </span><span id="elementLockingUser" runat="server"></span>
</div>
