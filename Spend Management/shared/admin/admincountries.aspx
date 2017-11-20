<%@ Page language="c#" Inherits="Spend_Management.admincountries" MasterPageFile="~/masters/smTemplate.master" Codebehind="admincountries.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
<a class="submenuitem" href="aecountry.aspx" id="lnkAddCountry"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">New Country</asp:Label></a>
</asp:Content>
				
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
<asp:ScriptManagerProxy ID="smpCountries"  runat="server">
<Scripts>
<asp:ScriptReference Path="~/shared/javaScript/countries.js" />
</Scripts>
</asp:ScriptManagerProxy>
<script src="../javascript/countries.js" type="text/javascript" language="javascript"></script>
				
	<script type="text/javascript" language="javascript">
        var cmbFilterID = '<%=cmbfilter.ClientID %>';
        var pnlGridID = '<%=pnlGrid.ClientID %>';
    </script>
				
	<div class="formpanel formpanel_padding">
	    <div class="twocolumn">
    		<asp:Label id="Label2" runat="server" meta:resourcekey="Label1Resource1" AssociatedControlID="cmbfilter">Display Filter</asp:Label>
		    <span class="inputs"><asp:DropDownList id="cmbfilter" runat="server" onchange="javascript:SEL.Grid.filterGridCmb('gridCountries',event);" meta:resourcekey="cmbfilterResource1">
			    <asp:ListItem Value="2" meta:resourcekey="ListItemResource1">All Countries</asp:ListItem>
			    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Archived Countries</asp:ListItem>
			    <asp:ListItem Value="0" meta:resourcekey="ListItemResource3">Un-archived Countries</asp:ListItem>
		    </asp:DropDownList></span>
		</div>
		    
        <asp:Panel runat="server" ID="pnlGrid">
            <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        </asp:Panel>
    
        <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div> 

    </div>
    
</asp:Content>


