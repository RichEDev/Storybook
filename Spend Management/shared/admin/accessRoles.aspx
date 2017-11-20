<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="accessRoles.aspx.cs" Inherits="Spend_Management.accessRoles" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:Panel ID="pnlAddNewAccessRole" runat="server">
    <a href="aeAccessRole.aspx" class="submenuitem">New Access Role</a>
    </asp:Panel></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy ID="smpAccessRoles" runat="server">
<Scripts>
<asp:ScriptReference Path="~/shared/javaScript/accessRoles.js" />
</Scripts>
<Services>
<asp:ServiceReference Path="~/shared/webServices/svcAccessRoles.asmx"  InlineScript="false" />
</Services>
</asp:ScriptManagerProxy>
<div class="formpanel formpanel_padding">
    <asp:Literal ID="litAccessRoles" runat="server"></asp:Literal>
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div> 
</div>
</asp:Content>
