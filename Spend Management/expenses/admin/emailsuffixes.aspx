<%@ Page Language="C#" MasterPageFile="~/masters/smTemplate.master" AutoEventWireup="true" Inherits="admin_emailsuffixes" Title="Untitled Page" Codebehind="emailsuffixes.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/masters/smTemplate.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
<a href="aeemailsuffix.aspx" class="submenuitem">Add Suffix</a>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
<script language="javascript">
				
				function deleteSuffix(suffixid)
				{
					if (confirm('Are you sure you wish to delete the selected e-mail suffix?'))
					{
						url = "emailsuffixes.aspx?action=3";
						doCallBack(url,"suffixid=" + suffixid);
						document.getElementById('suffixes').deleteRow(getIndex('suffixes',suffixid));
					}
				}
</script>
<script language="javascript" src="../../shared/javaScript/callback.js"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<div class="inputpanel table-border2">
    <asp:Literal ID="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal>
    </div>
    <div class="formpanel formpanel_padding">
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
    </div>
</asp:Content>

