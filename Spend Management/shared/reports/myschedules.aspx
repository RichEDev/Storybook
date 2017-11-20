<%@ Page Language="C#" MasterPageFile="~/masters/smForm.master" AutoEventWireup="true" Inherits="reports_myschedules" Codebehind="myschedules.aspx.cs" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentmenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentleft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="contentmain" Runat="Server">
<asp:ScriptManagerProxy runat="server">
<scripts>
    <asp:ScriptReference Path="~/shared/javaScript/callback.js" />
</scripts>
</asp:ScriptManagerProxy>
<script language="javascript" type="text/javascript">			
	function deleteSchedule(scheduleid)
	{
		if (confirm('Are you sure you wish to delete the selected schedule?'))
		{
			url = "myschedules.aspx?action=3";
			doCallBack(url,"scheduleid=" + scheduleid);
			document.getElementById('schedules').deleteRow(getIndex('schedules',scheduleid));
		}
	}
</script>

<div class="inputpanel">
    <asp:Literal ID="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal>
</div>
</asp:Content>

