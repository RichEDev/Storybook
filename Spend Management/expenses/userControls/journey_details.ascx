<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="journey_details.ascx.cs" Inherits="Spend_Management.journey_details" %>
<script language="javascript" type="text/javascript">

function viewJourney(claimid, expenseid, parent)
{
    var popup = $find('<%= popupdetails.ClientID %>');
    populateJourney(claimid, expenseid);
    popup._popupBehavior._parentElement = document.getElementById(parent);
    popup.showPopup();
}
    
function populateJourney(claimid, expenseid)
{
    var behaviour = $find('<%= dyndetails.ClientID %>');
    if (behaviour)
    {
        behaviour.populate(claimid + ',' + expenseid);
    }
}

function hideModal()
{
    var popup = $find('<%= popupdetails.ClientID %>');
    popup.hidePopup();
}

</script>                       
   
<asp:Panel ID="pnldetails" runat="server" CssClass="popupSemiTransparent"></asp:Panel>

<cc1:PopupControlExtender ID="popupdetails" runat="server" TargetControlID="lnkdetails" PopupControlID="pnldetails" OffsetX="18" OffsetY="-4"></cc1:PopupControlExtender>

<asp:LinkButton ID="lnkdetails" runat="server" style="display:none">LinkButton</asp:LinkButton>

<cc1:DynamicPopulateExtender 
    ID="dyndetails" 
    runat="server" 
    TargetControlID="pnldetails" 
    ServicePath="~/shared/webServices/svcAutocomplete.asmx" 
    ServiceMethod="getJourneyDetails">
</cc1:DynamicPopulateExtender>

<asp:Panel runat="server" ID="mapContainer"></asp:Panel>