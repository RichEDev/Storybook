<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="addressDetailsPopup.ascx.cs" Inherits="Spend_Management.addressDetailsPopup" %>
<asp:ScriptManagerProxy ID="smp" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/addressDetailsPopup.js" />
    </Scripts>
</asp:ScriptManagerProxy>


<asp:Panel ID="pnlAddressDetailsPopup" runat="server" CssClass="popupSemiTransparent" style="display: none">
<div class="popupContent">
    <asp:Image ID="adpLoader" runat="server" ImageUrl="~/shared/images/ajax-loader.gif" AlternateText="Loading..." style="right: 2; top: 2;" />
    <span id="adpAddressDetailsSpan"></span></div>
</asp:Panel>
<cc1:PopupControlExtender ID="adpPopup" runat="server" TargetControlID="hlAddressDetailsPopup" PopupControlID="pnlAddressDetailsPopup" OffsetX="16" OffsetY="16"></cc1:PopupControlExtender>
<asp:HyperLink ID="hlAddressDetailsPopup" runat="server" Text="&nbsp;">&nbsp;</asp:HyperLink>

<script language="javascript" type="text/javascript">

var adpPopup = '<% = adpPopup.ClientID %>';
var adpPanel = '<% = pnlAddressDetailsPopup.ClientID %>';
var adpAddressDetailsSpan = 'adpAddressDetailsSpan';
var adpLoader = '<% = adpLoader.ClientID %>';
var addressDetailsPopup;
// used to initialise the objects - similar to body onload
function addressDetailsPanelLoader() {
    addressDetailsPopup = new AddressDetailsPopup();
}
Sys.Application.add_init(addressDetailsPanelLoader);
// end onload
</script>