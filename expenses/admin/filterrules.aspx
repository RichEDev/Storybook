<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="filterrules.aspx.cs" MasterPageFile="~/exptemplate.master" Inherits="expenses.admin.filterrules" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton ID="lnkAddFilterRule" CssClass ="submenuitem" runat="server" 
        onclick="lnkAddFilterRule_Click">Add Filter Rule</asp:LinkButton>
    <%--<a class="submenuitem" href="aefilterrule.aspx?action=0"><asp:Label id="lblAddFilterRule" runat="server" meta:resourcekey="lblAddFilterRuleResource1">Add Filter Rule</asp:Label></a>--%>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript" type="text/javascript">
				
					
				function deleteFilterRule(filterid)
				{
					if (confirm('Are you sure you wish to delete the selected filter rule?'))
					{
						PageMethods.deleteFilterRule(filterid);
                        SEL.Grid.deleteGridRow('gridFilterRules', filterid);
					}
				}
    </script>			
    <div class="inputpanel">
	    <div class="inputpaneltitle">
	    <asp:Label id="lblFilterRules" runat="server">Filter Rules</asp:Label></div>
        
        <asp:Label id="lblFilter" runat="server" >Display Filter</asp:Label>
		:
		<asp:DropDownList id="cmbfilter" runat="server" AutoPostBack="True" onselectedindexchanged="cmbfilter_SelectedIndexChanged" meta:resourcekey="cmbfilterResource1">
			<asp:ListItem Value="0">All Filter Rules</asp:ListItem>
			<asp:ListItem Value="1">Cost codes</asp:ListItem>
			<asp:ListItem Value="2">Departments</asp:ListItem>
		   <%-- <asp:ListItem Value="3">Addresses</asp:ListItem>--%>
            <asp:ListItem Value="4">Project codes</asp:ListItem>
            <asp:ListItem Value="5">Reasons</asp:ListItem>
            <asp:ListItem Value="6">User Defined</asp:ListItem>
		</asp:DropDownList>
	</div>
    <div class="formpanel" style="padding-left: 0">
        <asp:Literal ID="litgrid" runat="server"></asp:Literal>
        <div class="formbuttons">
            <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
        </div>
    </div>
</asp:Content>