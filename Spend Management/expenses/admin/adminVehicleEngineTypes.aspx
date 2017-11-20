<%@ Page language="c#" Inherits="Spend_Management.adminVehicleEngineTypes" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminVehicleEngineTypes.aspx.cs" EnableViewState="false" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content runat="server" ContentPlaceHolderID="contentmenu">
    <a class="submenuitem" href="aeVehicleEngineType.aspx">
        <asp:Label runat="server" id="lblNew">New Vehicle Engine Type</asp:Label>
    </a>
</asp:Content>
				
<asp:Content runat="server" ContentPlaceHolderID="contentmain">				

    <asp:ScriptManagerProxy runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.vehicleEngineType.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <div class="formpanel formpanel_padding">
        <asp:Literal runat="server" ID="litGrid" />
        <div class="formbuttons">
            <asp:ImageButton runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False" OnClientClick="document.location = SEL.Vet.REDIRECT_URL; return false;"></asp:ImageButton>
        </div>
    </div>

    <script type="text/javascript">
        SEL.Vet.REDIRECT_URL = '<%=this.RedirectUrl%>';
    </script>
    
</asp:Content>
