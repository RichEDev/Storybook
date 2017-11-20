<%@ Page language="c#" Inherits="Spend_Management.shared.admin.admindepartments" MasterPageFile="~/masters/smTemplate.master" Codebehind="admindepartments.aspx.cs" %>

<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
                <asp:HyperLink runat="server" ID="lnkNewDepartment" NavigateUrl="aedepartment.aspx" meta:resourcekey="Label2Resource1"  CssClass="submenuitem">Add Department</asp:HyperLink>    
			    <a class="submenuitem" href="/admin/filterrules.aspx?FilterType=2"><asp:Label id="lblFilterRules" runat="server" meta:resourcekey="lblFilterRulesResource1">Filter Rules</asp:Label></a>
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">	
<script type="text/javascript" language="javascript" src="../javaScript/departments.js"></script>

    <script type="text/javascript" language="javascript">
        var cmbFilterID = '<%=cmbfilter.ClientID %>';
        var pnlGridID = '<%=pnlGrid.ClientID %>';
    </script>

	<div class="formpanel formpanel_padding">
	    <div class="twocolumn">
		    <asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1" AssociatedControlID="cmbfilter">Display Filter</asp:Label>
		    <span class="inputs"><asp:DropDownList id="cmbfilter" runat="server" onchange="javascript:SEL.Grid.filterGridCmb('gridDepartments',event);" meta:resourcekey="cmbfilterResource1">
			    <asp:ListItem Value="2" meta:resourcekey="ListItemResource1">All Departments</asp:ListItem>
			    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Archived Departments</asp:ListItem>
			    <asp:ListItem Value="0" meta:resourcekey="ListItemResource3">Un-archived Departments</asp:ListItem>
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


