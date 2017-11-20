<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="poolcars.aspx.cs" MasterPageFile="~/masters/smTemplate.master" Inherits="Spend_Management.poolcars" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <a href="aepoolcar.aspx" class="submenuitem"><asp:Label id="lblAddPoolCar" runat="server" meta:resourcekey="Label2Resource1">Add Pool Vehicle</asp:Label></a>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
    <asp:ScriptManagerProxy ID="smp" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javaScript/poolCars.js" />
        </Scripts>
        <Services>
            <asp:ServiceReference Path="~/shared/webServices/svcPoolCars.asmx" InlineScript="false" />
        </Services>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        //<![CDATA[
        var pnlPoolCarsGridID = '<% = pnlGridPoolCars.ClientID %>';
		//]]>
    </script>	

    <div class="formpanel formpanel_padding">
        <div class="sectiontitle"><asp:Label id="lblPoolCars" runat="server" meta:resourcekey="lblPoolCarsResource1">Pool Vehicles</asp:Label></div>
        <asp:Panel ID="pnlGridPoolCars" runat="server">
            <asp:Literal ID="litGridPoolCars" runat="server"></asp:Literal>
        </asp:Panel>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>			
    </div>      

</asp:Content>