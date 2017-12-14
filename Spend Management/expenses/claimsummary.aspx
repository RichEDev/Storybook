<%@ Page Language="c#" Inherits="Spend_Management.claimsummary" MasterPageFile="~/masters/smTemplate.master" CodeBehind="claimsummary.aspx.cs" EnableViewState="false" EnableSessionState="ReadOnly" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/sel.claims.js?date=20171214" />
        </Scripts>

        <Services>
            <asp:ServiceReference Path="~/expenses/webServices/claims.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:Literal ID="litoptions" runat="server" meta:resourcekey="litoptionsResource1"></asp:Literal>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <div class="formpanel formpanel_padding">
        <asp:Literal ID="litGrid" runat="server"></asp:Literal>
        <div class="formbuttons formpanel_padding">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>

</asp:Content>


