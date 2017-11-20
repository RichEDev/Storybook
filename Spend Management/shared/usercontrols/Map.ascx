<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Map.ascx.cs" Inherits="Spend_Management.Map" %>
<%@ Register TagPrefix="cc2" Namespace="SpendManagementHelpers" Assembly="SpendManagementHelpers" %>
<asp:ScriptManagerProxy runat="server" ID="smp">
    <Services>
        <asp:ServiceReference InlineScript="false" Path="~/shared/webServices/svcAddresses.asmx" />
    </Services>
    <Scripts>
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.addresses.and.travel.js" />
        <asp:ScriptReference Path="~/shared/javaScript/minify/sel.googlemaps.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<style type="text/css">
.routeModal 
{
	background-color: #FFFFFF; width: 96%; height: <% = height %>; border: 1px solid #000000; min-width: 725px;
}
</style>

<asp:Panel runat="server" ID="mapsContainer" CssClass="routeModal">
    <div id="map" style="margin-left: 320px; height: 100%; border-left: 1px solid #000;"></div>
    <div id="route" style="position: absolute; top: 0; left: 0; width: 320px; overflow: auto; height: 100%;"></div>
    <div id="closeMap" style="position: absolute; bottom: 0; left: 0; width: 100%; height: 42px; text-align: center;">
        <cc2:CSSButton ID="btnClose" runat="server" Text="close" OnClientClick="javascript:SEL.AddressesAndTravel.CloseRoute(); return false;"></cc2:CSSButton>
    </div>
</asp:Panel>

<cc1:ModalPopupExtender ID="mapsModal" runat="server" PopupControlID="mapsContainer" TargetControlID="mapsModalLink"  BackgroundCssClass="modalBackground" ></cc1:ModalPopupExtender>
<asp:HyperLink ID="mapsModalLink" runat="server" style="display:none; visibility: hidden;" Text="Get Route" NavigateUrl="#"></asp:HyperLink>


<asp:Panel runat="server" ID="mapsInfo" style="background-color: #FFFFFF;border: 1px solid #000000; display: none; width: 300px; line-height: 100px; text-align: center"><h3><asp:Image runat="server" ID="mapLoadingIcon" ImageUrl="~/shared/images/ajax-loader.gif" ToolTip="Loading" AlternateText="Loading"/> Loading...</h3></asp:Panel>

<cc1:ModalPopupExtender ID="mapsModalInfo" runat="server" PopupControlID="mapsInfo" TargetControlID="mapsModalInfoLink"  BackgroundCssClass="modalBackground" ></cc1:ModalPopupExtender>
<asp:HyperLink ID="mapsModalInfoLink" runat="server" style="display:none; visibility: hidden;" Text="Loading..." NavigateUrl="#"></asp:HyperLink>


<script language="javascript" type="text/javascript">
    MapVars = function () {
        SEL.AddressesAndTravel.MapModalDomID = "<%= mapsModal.ClientID %>";
        SEL.AddressesAndTravel.MapModalInfoDomID = "<% = mapsModalInfo.ClientID %>";
        SEL.AddressesAndTravel.MapInfoDomID = "<% = mapsInfo.ClientID %>";
        SEL.GoogleMaps.ScriptSource = "<%= GoogleMapsScriptUrl %>";
    };
    MapVars.registerClass("MapVars", Sys.Component);
    Sys.Application.add_init(function () { $create(MapVars, { "id": "MapVars" }, null, null, null); });
</script>