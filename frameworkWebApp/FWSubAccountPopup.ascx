<%@ Control Language="VB" AutoEventWireup="false" Inherits="frameworkWebApp.FWSubAccountPopup" Codebehind="FWSubAccountPopup.ascx.vb" %>
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

    function SwitchSubAccount(subAccountID) 
    {
        Spend_Management.svcSubAccounts.SwitchSubAccount(subAccountID, SwitchSubAccountComplete);
    }

    function SwitchSubAccountComplete(data) 
    {
        window.location = appPath + "/home.aspx";
    }
</script>

<asp:Panel ID="pnlSubaccounts" runat="server" CssClass="modalpanel formpanel" style="width:auto; display: none; ">
    <div class="sectiontitle">Switch Sub Account</div>
    <div id="divSubAccounts" runat="server"><asp:Literal ID="litSubAccounts" runat="server"></asp:Literal></div>
    <div class="formbuttons"><asp:Image ID="btnCloseModal" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" AlternateText="Close" /></div>
</asp:Panel>

<asp:HyperLink ID="lnkSubAccountPopup" runat="server" Text="&nbsp;" style="display: none;"></asp:HyperLink>
<cc1:ModalPopupExtender ID="mdlSubAccounts" runat="server" CancelControlID="btnCloseModal" BackgroundCssClass="modalBackground" PopupControlID="pnlSubaccounts" TargetControlID="lnkSubAccountPopup"></cc1:ModalPopupExtender>
