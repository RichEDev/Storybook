
<%@ Page language="c#" Inherits="expenses.adminqeforms" MasterPageFile="~/exptemplate.master" Codebehind="adminqeforms.aspx.cs" %>
<%@ MasterType VirtualPath="~/exptemplate.master" %>


				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    

				<a href="aeqeform.aspx" class="submenuitem"><asp:Label id="Label1" runat="server" meta:resourcekey="Label1Resource1">Add Quick Entry Form</asp:Label></a>
				
				</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">				
<script language="javascript">
				
		function deleteQeForm(quickentryid)
		{
			if (confirm('Are you sure you wish to delete the selected quick entry form?'))
			{
				url = "adminqeforms.aspx?callback=1&action=3";
				doCallBack(url,"quickentryid=" + quickentryid);
				document.getElementById('quickentry').deleteRow(getIndex('quickentry',quickentryid));
			}
		}
		
		function getIndex(tblid, id)
		{
			var i;
			var tbl = document.getElementById(tblid);
			
			for (i = 0; i < tbl.rows.length; i++)
			{
				if (tbl.rows[i].id == id)
				{
					
					return i;
				}
			}
		}
				
		function doCallBack(url, dataToSend)
		{
			var xmlRequest;
			try
			{
				xmlRequest = new XMLHttpRequest();
			}
			catch (e)
			{	
				try
				{
					xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
				}
				catch (f)
				{
					xmlRequest = null;
				}
			}
			
			xmlRequest.open("POST",url,false);
			//xmlRequest.setRequestHeader("Content-Type","application/x-wais-source");
			xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			xmlRequest.send(dataToSend);
			return xmlRequest.responseText;
		}
</script>
	<div class="inputpanel table-border2">
		<asp:Literal id="litgrid" runat="server" meta:resourcekey="litgridResource1"></asp:Literal>
	</div>
    <div class="formpanel">
    <div class="formbuttons">
        <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
    </div>
    </div>
    </asp:Content>

