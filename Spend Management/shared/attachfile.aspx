<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/masters/smForm.master" CodeBehind="attachfile.aspx.cs" Inherits="Spend_Management.shared.attachfile" %>
<%@ Register src="usercontrols/UploadAttachment.ascx" tagname="UploadAttachment" tagprefix="uc1" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <div class="formpanel">
        <uc1:UploadAttachment ID="UploadAttachment1" runat="server" />
    </div>
</asp:Content>
