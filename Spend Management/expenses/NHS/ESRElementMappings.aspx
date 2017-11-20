<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" CodeBehind="ESRElementMappings.aspx.cs" Inherits="Spend_Management.ESRElementMappings" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" runat="server">
    <asp:Panel ID="pnlLinks" runat="server"></asp:Panel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" runat="server">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/expenses/javaScript/ESRElementMappings.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/expenses/webservices/ESR.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <div class="formpanel" runat="server" id="divUnMappedExpenseItemHolder">
                <div class="sectiontitle">Expense Items Not Mapped</div>
        <div runat="server" id="divUnMappedExpenseItems">

            <asp:Literal ID="litUnMappedExpenseItems" runat="server"></asp:Literal>
        </div>
        <div class="formbuttons"><img alt="Close" src="/shared/images/buttons/btn_close.png" onclick="window.location = 'trusts.aspx'" /></div>
    </div>
    <asp:Panel ID="pnlElementSummary" runat="server" CssClass="formpanel"></asp:Panel>
</asp:Content>
