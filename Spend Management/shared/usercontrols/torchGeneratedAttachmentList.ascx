<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="torchGeneratedAttachmentList.ascx.cs" Inherits="Spend_Management.torchGeneratedAttachmentList" %>
<asp:ScriptManagerProxy ID="smProxy" runat="server">
	<Services>
	    <asp:ServiceReference Path="~/shared/webServices/svcAttachments.asmx" />
	</Services>
	<Scripts>
	    <asp:ScriptReference Path="~/shared/javaScript/Attachments.js" />
	</Scripts>
</asp:ScriptManagerProxy>

<div>
    <asp:Literal ID="torchAttachmentGridLiteral" runat="server"></asp:Literal>
</div>
