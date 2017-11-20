<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="attachmentList.ascx.cs"
    Inherits="Spend_Management.attachmentList" %>
<%@ Register Src="UploadAttachment.ascx" TagName="UploadAttachment" TagPrefix="uc1" %>
<asp:ScriptManagerProxy runat="server" ID="smProxyattachment">
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcAttachments.asmx" />
    </Services>
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/Attachments.js" />
    </Scripts>
</asp:ScriptManagerProxy>
    <div class="sectiontitle">
        <asp:Label ID="lblAttach" runat="server" meta:resourcekey="lblAttachResource1">Attachments</asp:Label>
    </div>
    <div>
        <a onclick="ShowAttachmentUploadModal()">Add new attachment</a>
        <button type="button" style="display: none" id="modalBtnCancel" onclick="CloseAttachmentUploadModalModal()"></button>
    </div>
    <div style="height:7px;"></div>
    <div>
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
    </div>
<div id="uploadModal" style="display: none">
    <uc1:UploadAttachment ID="usrUpload" runat="server" />
</div>

  <style>
        a {
            cursor: pointer;
            text-decoration: none;
        }
  </style>