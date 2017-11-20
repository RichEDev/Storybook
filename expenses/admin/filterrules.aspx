<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="filterrules.aspx.cs" MasterPageFile="~/exptemplate.master" Inherits="expenses.admin.filterrules" %>

<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics4.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2238, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    <asp:LinkButton ID="lnkAddFilterRule" CssClass ="submenuitem" runat="server" 
        onclick="lnkAddFilterRule_Click">Add Filter Rule</asp:LinkButton>
    <%--<a class="submenuitem" href="aefilterrule.aspx?action=0"><asp:Label id="lblAddFilterRule" runat="server" meta:resourcekey="lblAddFilterRuleResource1">Add Filter Rule</asp:Label></a>--%>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">

    <script language="javascript" type="text/javascript">
				
					
				function deleteFilterRule(accountid, filterid)
				{
					if (confirm('Are you sure you wish to delete the selected filter rule?'))
					{
						PageMethods.deleteFilterRule(accountid, filterid);
						var grid = igtbl_getGridById(contentID + 'gridfilterrules');
						
				    grid.Rows.remove(grid.getActiveRow().getIndex());
					}
				}
    </script>			
    <div class="inputpanel">
	    <div class="inputpaneltitle">
	    <asp:Label id="lblFilterRules" runat="server" meta:resourcekey="lblFilterRulesResource1">Filter Rules</asp:Label></div>
        
        <asp:Label id="lblFilter" runat="server" meta:resourcekey="lblFilterResource1">Display Filter</asp:Label>
		:
		<asp:DropDownList id="cmbfilter" runat="server" AutoPostBack="True" onselectedindexchanged="cmbfilter_SelectedIndexChanged" meta:resourcekey="cmbfilterResource1">
			<asp:ListItem Value="0" meta:resourcekey="ListItemResource1">All Filter Rules</asp:ListItem>
			<asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Cost codes</asp:ListItem>
			<asp:ListItem Value="2" meta:resourcekey="ListItemResource3">Departments</asp:ListItem>
		    <%--<asp:ListItem Value="3">Addresses</asp:ListItem>--%>
            <asp:ListItem Value="4">Project codes</asp:ListItem>
            <asp:ListItem Value="5">Reasons</asp:ListItem>
            <asp:ListItem Value="6">User Defined</asp:ListItem>
		</asp:DropDownList>
	</div>
    <div class="inputpanel">  
        <asp:UpdatePanel ID="pnlFilter" runat="server">
        <ContentTemplate> 
            <igtbl:UltraWebGrid ID="gridfilterrules" runat="server" SkinID="gridskin" 
                OnInitializeLayout="gridfilterrules_InitializeLayout" 
                OnInitializeRow="gridfilterrules_InitializeRow" 
                OnSortColumn="gridfilterrules_SortColumn">
            </igtbl:UltraWebGrid>
          
        </ContentTemplate>
        </asp:UpdatePanel>
			
	</div>
    <div class="formpanel" style="padding-left:0px;">
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
    </div>
</asp:Content>