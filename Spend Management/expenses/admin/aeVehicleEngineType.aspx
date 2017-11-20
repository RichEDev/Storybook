<%@ Page language="c#" Inherits="Spend_Management.aeVehicleEngineType" MasterPageFile="~/masters/smForm.master" Codebehind="aeVehicleEngineType.aspx.cs" AutoEventWireup="True" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<%@ Register Src="~/shared/usercontrols/tooltip.ascx" TagName="tooltip" TagPrefix="tooltip" %>

<asp:Content runat="server" ContentPlaceHolderID="contentmenu">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="contentmain">
    
    <asp:ScriptManagerProxy runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/shared/javascript/sel.ajax.js"/>
            <asp:ScriptReference Path="~/shared/javaScript/minify/sel.vehicleEngineType.js" />
            <asp:ScriptReference Name="tooltips" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
	<div class="formpanel formpanel_padding">
	    <div class="sectiontitle">General Details</div>
	    <div class="twocolumn">
	        <asp:Label runat="server" AssociatedControlID="txtName" CssClass="mandatory">
	            Vehicle engine type*
	        </asp:Label><span class="inputs">
	            <asp:TextBox id="txtName" runat="server" MaxLength="450" />
	        </span><span class="inputicon">
	        </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Vet.HELP.NAME, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" style="border: none;" />
	        </span><span class="inputvalidatorfield">
	        </span><asp:Label runat="server" AssociatedControlID="txtCode">
                Code
	        </asp:Label><span class="inputs">
	            <asp:TextBox id="txtCode" runat="server" MaxLength="50" />
	        </span><span class="inputicon">
	        </span><span class="inputtooltipfield">
                <img class="tooltipicon" onmouseover="SEL.Tooltip.Show(SEL.Vet.HELP.CODE, 'ex', this);" src="/shared/images/icons/16/plain/tooltip.png" alt="" style="border: none;" />
	        </span><span class="inputvalidatorfield">
	        </span>
        </div>
	</div>
    
    <div class="formpanel formpanel_padding">
        <div class="formbuttons">
            <helpers:CSSButton ID="btnSave" runat="server" Text="save" UseSubmitBehavior="False" OnClientClick="SEL.Vet.Save(); return false;" />
            <helpers:CSSButton ID="btnCancel" runat="server" Text="cancel" UseSubmitBehavior="False" OnClientClick="document.location = SEL.Vet.REDIRECT_URL; return false;" />
        </div>
    </div>
    
    <script type="text/javascript">
        SEL.Vet.CONTENT_MAIN = '<%=this.btnSave.Parent.ClientID%>_';
        SEL.Vet.REDIRECT_URL = '<%=this.RedirectUrl%>';
        SEL.Vet.VET_ID = <asp:Literal runat="server" ID="litVehicleEngineTypeId" />;

        $(function ()
        {
            $('form')
                .keypress(function (e)
                {
                    if (e.which == $.ui.keyCode.ENTER)
                    {
                        SEL.Vet.Save();
                    }
                });
        });
    </script>
    
    <tooltip:tooltip runat="server" />
    
</asp:Content>
