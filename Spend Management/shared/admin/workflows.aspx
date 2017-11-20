<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" CodeBehind="workflows.aspx.cs" Inherits="Spend_Management.workflows" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
<a href="aeworkflow.aspx" class="submenuitem">Add Workflow</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy ID="smp" runat="server">
    <Scripts>
    <asp:ScriptReference Path="~/shared/javaScript/workflows.js" />
    </Scripts>
    <Services>
    <asp:ServiceReference Path="~/shared/webServices/svcWorkflows.asmx" InlineScript="false" />
    </Services>
</asp:ScriptManagerProxy>
<asp:Panel ID="pnlGrid" runat="server" CssClass="formpanel formpanel_padding">
    <asp:Literal ID="litWorkflows" runat="server"></asp:Literal>
</asp:Panel>
</asp:Content>
