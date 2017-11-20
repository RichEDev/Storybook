<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="importTemplates.aspx.cs" Inherits="Spend_Management.importTemplates" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:Panel ID="pnlLinks" runat="server"></asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
<asp:ScriptManagerProxy ID="smp" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/importexport.js" />
    </Scripts>
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcImportTemplates.asmx" InlineScript="false" />
    </Services>
</asp:ScriptManagerProxy>
    <div class="formpanel formpanel_padding">
        <div class="sectiontitle">General Details</div>
        <asp:Panel ID="pnlGrid" runat="server"></asp:Panel>
    </div>
    <div class="formpanel formpanel_padding">
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
    </div>
</asp:Content>
