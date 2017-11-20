<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true"
    CodeBehind="adminAttachmentBackups.aspx.cs" Inherits="Spend_Management.adminAttachmentBackups" %>

<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:content id="Content1" contentplaceholderid="contentmenu" runat="server">
    <a href="javascript:SEL.AttachmentBackup.CreateFullBackup();" title="New Full Backup" class="submenuitem">New Full Backup</a>
    <a href="javascript:SEL.AttachmentBackup.LaunchFilteredBackup();" title="New Filtered Backup" class="submenuitem">New Filtered Backup</a>
</asp:content>
<asp:content id="Content2" contentplaceholderid="contentleft" runat="server">
    
</asp:content>
<asp:content id="Content3" contentplaceholderid="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.attachment_backup.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcAttachmentBackupManager.asmx" />
        </Services>
    </asp:ScriptManagerProxy>

    <script language="javascript" type="text/javascript">

	</script>

    <div class="formpanel">
	    <div class="sectiontitle">Attachment Backup Packages Ready For Download</div>
        <div id="divGridProcessed" style="width:220px;">
            <asp:Literal runat="server" id="litGridProcessed"></asp:Literal>
        </div>
	    <div class="sectiontitle">Attachment Backup Packages Queued For Processing</div>
        <div id="divGridUnprocessed" style="width:220px;">
            <asp:Literal runat="server" id="litGridUnprocessed"></asp:Literal>
        </div>
		<div class="formbuttons">
            <asp:ImageButton runat="server" ID="btnClose" 
    ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" CausesValidation="False"
    onclick="btnClose_Click" />
	    </div>
    </div>

    <asp:Panel ID="pnBackupVolumes" runat="server" CssClass="errorModal">
        <div id="divBackupVolumes" class="formpanel" style="height:550px;overflow:auto;">
		    <div class="sectiontitle">Download Volumes For This Backup Package</div>
            <div class="twocolumn">
                <div id="divGridAttachmentVolumes" runat="server" clientidmode="Static">
    		        <asp:Literal id="ltGridAttachmentVolumes" runat="server" />
                </div>
		    </div>
		    <div class="formbuttons">
		        <asp:ImageButton id="cmdcancel" runat="server" 
                    ImageUrl="../images/buttons/cancel_up.gif" CausesValidation="False"></asp:ImageButton>
	        </div>
        </div>
    </asp:Panel>

    <asp:Label runat="server" Text="" id="lblDummy" />
    <cc1:ModalPopupExtender ClientIDMode="Static"
        ID="mdlAttachmentVolumes" 
        TargetControlID="lblDummy"
        runat="server" 
        BackgroundCssClass="modalMasterBackground" 
        PopupControlID="pnBackupVolumes" 
        DropShadow="False"
        CancelControlID="cmdcancel">
    </cc1:ModalPopupExtender>


    <asp:Panel ID="pnlBackupFilter" runat="server" CssClass="errorModal" style="display:none;">
        <div id="divMessage" class="formpanel">
		    <div class="sectiontitle">Filter Attachments</div>
            <div class="twocolumn">
    		    <asp:Label id="lblMessage" runat="server" />
		    </div>
		    <div class="formbuttons">
		        <asp:ImageButton id="cmdMessageok" runat="server" 
                    ImageUrl="../images/buttons/btn_close.png" CausesValidation="false"></asp:ImageButton>
	        </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ClientIDMode="Static"
        ID="mdlFilter" 
        TargetControlID="lblDummy"
        runat="server" 
        BackgroundCssClass="modalMasterBackground" 
        PopupControlID="pnlBackupFilter" 
        DropShadow="False"
        CancelControlID="cmdMessageok">
    </cc1:ModalPopupExtender>

</asp:content>
