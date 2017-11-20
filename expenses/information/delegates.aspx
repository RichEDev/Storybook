<%@ Page Language="C#" MasterPageFile="~/exptemplate.master" AutoEventWireup="true" Inherits="information_delegates" Title="Untitled Page" Codebehind="delegates.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
<a class="submenuitem" href="employeeproxy.aspx">Add Delegate</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
<script language="javascript">
				
					
				function deleteDelegate(employeeid)
				{
					if (confirm('Are you sure you wish to delete the selected delegate?'))
					{
						
						url = "delegates.aspx?action=3";
						doCallBack(url,"employeeid=" + employeeid);
						document.getElementById('delegates').deleteRow(getIndex('delegates',employeeid));
					}
				}
</script>
<script language="javascript" src="../callback.js"></script>	
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
    <div class="inputpanel table-border2 formpanel_margine"><asp:Literal ID="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal></div>
    <div class="inputpanel"><asp:ImageButton runat="server" ID="cmdClose" 
            ImageUrl="~/shared/images/buttons/btn_close.png" onclick="cmdClose_Click" /></div>
</asp:Content>

