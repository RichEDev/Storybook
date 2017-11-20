<%@ Page language="c#" Inherits="Spend_Management.adminuserdefined" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminuserdefined.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="aeuserdefined.aspx" class="submenuitem" ID="lnkAddUDF" runat="server"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add Userdefined Field</asp:Label></a>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/userdefined.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript" language="javascript">
        var modformid = '<%=modUDFReports.ClientID %>';     
    </script>
    <div class="formpanel formpanel_padding">
        <asp:Literal id="litgrid" runat="server" EnableViewState="False" meta:resourcekey="litgridResource1"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
    <asp:LinkButton ID="lnkUDFReports" runat="server" Style="display: none">LinkButton</asp:LinkButton>
    <asp:Panel ID="pnlUDFReports" runat="server" CssClass="modalpanel formpanel" Style="display: none; width: 750px">
        <div id="divUDFReports"></div>
        <div class="formbuttons">
            <helpers:CSSButton ID="cmdModalClose" runat="server" Text="close" OnClientClick="javascript:HideModal();return false;" UseSubmitBehavior="False" TabIndex="0" /></div>
    </asp:Panel>
    <cc1:ModalPopupExtender ID="modUDFReports" runat="server" TargetControlID="lnkUDFReports" PopupControlID="pnlUDFReports" BackgroundCssClass="modalBackground" CancelControlID="cmdModalClose">
    </cc1:ModalPopupExtender>
</asp:Content>


