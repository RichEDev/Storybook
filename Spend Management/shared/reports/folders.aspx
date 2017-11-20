
<%@ Page language="c#" Inherits="Spend_Management.folders" MasterPageFile="~/masters/smForm.master" Codebehind="folders.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/smForm.master" %>

		
				<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmenu">
    
				<A href="aefolder.aspx" class=submenuitem>
                    <asp:Label ID="lbladd" runat="server" Text="Add Category" meta:resourcekey="lbladdResource1"></asp:Label></A>
				<A href="rptlist.aspx" class=submenuitem>
                    <asp:Label ID="lblreports" runat="server" Text="Reports" meta:resourcekey="lblreportsResource1"></asp:Label></A></asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="contentmain">
<script language="javascript">
				
		function deleteFolder(folderid)
		{
			if (confirm('Are you sure you wish to delete the selected category?'))
			{
				url = "folders.aspx?callback=1";
				doCallBack(url,"folderid=" + folderid);
				document.getElementById('folders').deleteRow(getIndex('folders',folderid));
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

        	<div class="formpanel formpanel_padding">
            <div class="formbuttons">
                <asp:ImageButton ID="cmdClose" OnClick="cmdClose_Click" runat="server" ImageUrl="~/shared/images/buttons/btn_close.png" CausesValidation="False"></asp:ImageButton>
            </div>
        </div>

    </asp:Content>


