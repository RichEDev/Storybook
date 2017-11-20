<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="workflows.aspx.cs" Inherits="Spend_Management.workflowsActionsPage" Title="Workflows" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register TagName="wfUC" TagPrefix="wfUC" Src="~/shared/usercontrols/workflow.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy ID="smp" runat="server">
<Services>
    <asp:ServiceReference Path="~/shared/webServices/svcWorkflows.asmx" InlineScript="false" />
</Services>
<Scripts>
<asp:ScriptReference Path="~/shared/javaScript/workflows.js" />
</Scripts>
</asp:ScriptManagerProxy>
<asp:Panel ID="pnlGrid" runat="server" CssClass="formpanel">
    <wfUC:wfUC ID="wfUC" runat="server" />
    </asp:Panel>
</asp:Content>
