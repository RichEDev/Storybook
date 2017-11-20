<%@ Page language="c#" Inherits="Spend_Management.shared.admin.adminprojectcodes" MasterPageFile="~/masters/smTemplate.master" Codebehind="adminprojectcodes.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:HyperLink runat="server" ID="lnkNewProjectCode" NavigateUrl="aeprojectcode.aspx" meta:resourcekey="Label2Resource1"  CssClass="submenuitem">Add Project Code</asp:HyperLink>    
	 <a class="submenuitem" href="/admin/filterrules.aspx?FilterType=4"><asp:Label id="lblFilterRules" runat="server" meta:resourcekey="lblFilterRulesResource1">Filter Rules</asp:Label></a>
  </asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
<script language="javascript" type="text/javascript" src="../javaScript/projectCodes.js"></script>

    <script type="text/javascript" language="javascript">
        var cmbFilterID = '<%=cmbfilter.ClientID %>';
        var pnlGridID = '<%=pnlGrid.ClientID %>';
    </script>
    
	<div class="formpanel formpanel_padding">
	    <div class="twocolumn">
		    <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1" AssociatedControlID="cmbfilter">Display Filter</asp:Label>
		    <span class="inputs"><asp:DropDownList id="cmbfilter" runat="server" onchange="javascript:SEL.Grid.filterGridCmb('gridProjectcodes',event);" meta:resourcekey="cmbfilterResource1">
			    <asp:ListItem Value="2" meta:resourcekey="ListItemResource1">All Project Codes</asp:ListItem>
			    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Archived Project Codes</asp:ListItem>
			    <asp:ListItem Value="0" meta:resourcekey="ListItemResource3">Un-archived Project Codes</asp:ListItem>
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


