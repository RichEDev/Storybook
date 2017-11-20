<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadAttachment.ascx.cs" Inherits="Spend_Management.UploadAttachment" %>
<script>
    var divfileuploadID = '<%=divFileUpload.ClientID%>';
    var divprogressID = '<%=divProgress.ClientID%>';
</script>
    <asp:ScriptManagerProxy ID="attSMProxy" runat="server">
	    <Services>
	        <asp:ServiceReference Path="~/shared/webServices/svcAttachments.asmx" />
	    </Services>
	    <Scripts>
	        <asp:ScriptReference Path="~/shared/javaScript/Attachments.js" />
	    </Scripts>
    </asp:ScriptManagerProxy>
    
    <div class="formpanel">
    <div id="divFileUpload" style="height: 120px;" runat="server">
        <asp:Literal ID="litAttachPage" runat="server"></asp:Literal>        
    </div>
    
    <div id="divProgress" style="padding: 20px; display: none; width: 145px; height: 120px;" runat="server">
        <img src="/shared/images/ajax-loader.gif" alt="Activity indicator" />
        <asp:Label ID="lblUploading" runat="server" Text="Uploading..."></asp:Label>
    </div>

</div>

