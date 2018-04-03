<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" Inherits="itemroles" Title="Untitled Page" Codebehind="itemroles.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
    
    <a href="aeitemrole.aspx" class="submenuitem">New Item Role</a>
    
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
    <script type="text/javascript" src="/expenses/javaScript/sel.ItemRoles.js?date=201803041353"></script>
    
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnClose" Text="close" OnClientClick="document.location = '../../usermanagementmenu.aspx';return false;" UseSubmitBehavior="False" />
        </div>
    </div>

</asp:Content>

