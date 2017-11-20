<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="flags.aspx.cs" Inherits="Spend_Management.flags" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content4" ContentPlaceHolderID="styles" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:Panel ID="pnlLinks" runat="server">
    <a href="aeflagrule.aspx" class="submenuitem"><asp:Label id="Label2" runat="server" meta:resourcekey="Label2Resource1">New Flag Rule</asp:Label></a>
        </asp:Panel>
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/SEL.FlagRules.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/expenses/webservices/svcFlagRules.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <div class="formpanel formpanel_padding">
    <asp:Literal ID="litGrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <helpers:CSSButton runat="server" ID="btnClose" Text="close" OnClientClick="document.location = '../../policymenu.aspx';return false;" UseSubmitBehavior="False" />
        </div>
    </div>
</asp:Content>
