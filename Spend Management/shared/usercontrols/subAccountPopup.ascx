<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="subAccountPopup.ascx.cs" Inherits="Spend_Management.shared.usercontrols.subAccountPopup" %>
<asp:ScriptManagerProxy ID="smpTaskSummaryPopup" runat="server">
    <Services>
        <asp:ServiceReference Path="~/shared/webServices/svcSubAccounts.asmx" />
    </Services>
</asp:ScriptManagerProxy>
<script type="text/javascript" language="javascript">
    var mdlSubAccountID = '<%=mdlSubAccounts.ClientID %>';

    function ShowSubAccountModal() 
    {
        $find(mdlSubAccountID).show();
        return;
    }
</script>

<asp:Panel ID="pnlSubaccounts" runat="server" CssClass="modalpanel formpanel" style="width:auto; display: none; ">
    <div class="sectiontitle">Switch Sub Account</div>
    <div id="divSubAccounts" runat="server"><asp:Literal ID="litSubAccounts" runat="server"></asp:Literal></div>
    <div class="formbuttons"><asp:Image ID="btnCloseModal" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" /></div>
</asp:Panel>

<asp:HyperLink ID="lnkSubAccountPopup" runat="server" Text="&nbsp;" style="display: none;"></asp:HyperLink>
<cc1:ModalPopupExtender ID="mdlSubAccounts" runat="server" CancelControlID="btnCloseModal" BackgroundCssClass="modalBackground" PopupControlID="pnlSubaccounts" TargetControlID="lnkSubAccountPopup"></cc1:ModalPopupExtender>