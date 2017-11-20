<%@ Page language="c#" Inherits="Spend_Management.adminmileage" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminmileage.aspx.cs" EnableViewState="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
            <asp:ScriptReference Path="~/expenses/javaScript/vehicleJourneyRates.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
				<a href="aemileage.aspx" class="submenuitem"><asp:Label id="Label5" runat="server" meta:resourcekey="Label5Resource1">Add Vehicle Journey Rate Category</asp:Label></a>
				
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

<script language="javascript" type="text/javascript">
				
					
				
				
				function displayCategoryWarning(catcomment, rowID)
				{
				    var popup = $find('<%= popEx.ClientID %>');
				    popup._popupBehavior._parentElement = document.getElementById("tbl_gridMileage_" + rowID).getElementsByTagName("td")[2].getElementsByTagName("img")[0];
                    popup.showPopup();
                    var lbl = document.getElementById(contentID + "lblCatComm").innerText = catcomment;   
				}
</script>			
<div class="formpanel formpanel_padding">
    <asp:Literal ID="litGrid" runat="server"></asp:Literal>
    <div class="formbuttons"><asp:HyperLink ID="lnkClose" runat="server" NavigateUrl="~/categorymenu.aspx" ImageUrl="~/shared/images/buttons/btn_close.png"></asp:HyperLink></div>
</div>

    <asp:Image ID="imgcatpopup" runat="server" Style="display:none;" />
    <asp:Panel ID="pnlcatvalid" runat="server" CssClass="modalpanel" Height="100px" 
        Width="360px">
        <div class="inputpaneltitle">
            <asp:Label ID="lblCatpopup" runat="server" Text="Category not valid" meta:resourcekey="lblCatpopupResource1"></asp:Label></div>
        <asp:UpdatePanel ID="upcatvalid" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblCatComm" runat="server" Text="bjhytt"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
        
    <cc1:PopupControlExtender ID="popEx" runat="server" TargetControlID="imgcatpopup" PopupControlID="pnlcatvalid">
    </cc1:PopupControlExtender>
        
        
</asp:Content>


